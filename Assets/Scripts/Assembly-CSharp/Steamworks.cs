using System;
using System.Runtime.InteropServices;
using System.Text;
using ManagedSteam;
using ManagedSteam.CallbackStructures;
using ManagedSteam.Exceptions;
using ManagedSteam.SteamTypes;
using UnityEngine;

public class Steamworks : MonoBehaviour
{
	private static Steamworks activeInstance;

	private string status;

	private string achievementStatus;

	private UserHasLicenseForAppResult hasLicense;

	public static Steam SteamInterface { get; private set; }

	private void Awake()
	{
		Steam.RestartAppIfNecessary(0u);
		if (SteamInterface == null)
		{
			bool flag = false;
			try
			{
				SteamInterface = Steam.Initialize();
			}
			catch (AlreadyLoadedException ex)
			{
				status = "The native dll is already loaded, this should not happen if ReleaseManagedResources is used and Steam.Initialize() is only called once.";
				Debug.LogError(status, this);
				Debug.LogError(ex.Message, this);
				flag = true;
			}
			catch (SteamInitializeFailedException ex2)
			{
				status = "Could not initialize the native Steamworks API. This is usually caused by a missing steam_appid.txt file or if the Steam client is not running.";
				Debug.LogError(status, this);
				Debug.LogError(ex2.Message, this);
				flag = true;
			}
			catch (SteamInterfaceInitializeFailedException ex3)
			{
				status = "Could not initialize the wanted versions of the Steamworks API. Make sure that you have the correct Steamworks SDK version. See the documentation for more info.";
				Debug.LogError(status, this);
				Debug.LogError(ex3.Message, this);
				flag = true;
			}
			catch (DllNotFoundException ex4)
			{
				status = "Could not load a dll file. Make sure that the steam_api.dll/libsteam_api.dylib file is placed at the correct location. See the documentation for more info.";
				Debug.LogError(status, this);
				Debug.LogError(ex4.Message, this);
				flag = true;
			}
			if (flag)
			{
				SteamInterface = null;
				return;
			}
			status = "Steamworks initialized and ready to use.";
			UnityEngine.Object.DontDestroyOnLoad(this);
			activeInstance = this;
			SteamInterface.ExceptionThrown += ExceptionThrown;
			SteamInterface.Friends.GameOverlayActivated += OverlayToggle;
			hasLicense = SteamInterface.User.UserHasLicenseForApp(SteamInterface.User.GetSteamID(), SteamInterface.AppID);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void OverlayToggle(GameOverlayActivated value)
	{
		if (value.Active)
		{
			Debug.Log("Overlay shown");
		}
		else
		{
			Debug.Log("Overlay closed");
		}
	}

	private void ExceptionThrown(Exception e)
	{
		Debug.LogError(e.GetType().Name + ": " + e.Message + "\n" + e.StackTrace);
	}

	private void Update()
	{
		if (SteamInterface != null)
		{
			SteamInterface.Update();
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		GUILayout.Label(status);
		GUILayout.Label(achievementStatus);
		GUILayout.Label(hasLicense.ToString());
		GUILayout.Label(SteamInterface.AppID.ToString());
		if (GUILayout.Button("Unlock Achivement!") && SteamInterface.Stats.SetAchievement("Your-Achievement-Name"))
		{
			achievementStatus = "Achievement Status: Successfully got an achievement!";
		}
		if (GUILayout.Button("Clear Achivement!") && SteamInterface.Stats.ClearAchievement("Your-Achievement-Name"))
		{
			achievementStatus = "Achievement Status: Successfully cleared an achievement!";
		}
		if (GUILayout.Button("Get Achievement!"))
		{
			StatsGetAchievementResult achievement = SteamInterface.Stats.GetAchievement("Your-Achievement-Name");
			if (achievement.result)
			{
				if (achievement.sender)
				{
					achievementStatus = "Achievement Status: achievement is unlocked!";
				}
				else
				{
					achievementStatus = "Achievement Status: achievement is locked!";
				}
			}
		}
		GUILayout.Space(24f);
		if (GUILayout.Button("Write File"))
		{
			byte[] bytes = Encoding.ASCII.GetBytes("Hello World again!!!");
			GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			try
			{
				IntPtr data = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
				SteamInterface.Cloud.Write("test123.dat", data, 128);
			}
			finally
			{
				gCHandle.Free();
			}
		}
		if (GUILayout.Button("Check Number of Files"))
		{
			ICloud cloud = SteamInterface.Cloud;
			Debug.Log("test123.dat Exists? " + cloud.Exists("test123.dat"));
		}
		if (GUILayout.Button("Read File"))
		{
			ICloud cloud2 = SteamInterface.Cloud;
			string fileName = "test123.dat";
			int size = cloud2.GetSize(fileName);
			Debug.Log("File Size: " + size);
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			int num = cloud2.Read(fileName, intPtr, size);
			Debug.Log("Amount Read: " + num);
			string text = Marshal.PtrToStringAnsi(intPtr);
			Debug.Log("Our Read Data is: " + text);
			Marshal.FreeHGlobal(intPtr);
		}
		if (GUILayout.Button("Delete File"))
		{
			ICloud cloud3 = SteamInterface.Cloud;
			cloud3.Delete("test123.dat");
			cloud3.Forget("test123.dat");
		}
		GUILayout.Space(24f);
		if (GUILayout.Button("Show overlay"))
		{
			SteamInterface.Friends.ActivateGameOverlay(OverlayDialog.Friends);
		}
		if (GUILayout.Button("Show overlay (webpage)"))
		{
			SteamInterface.Friends.ActivateGameOverlayToWebPage("http://ludosity.com");
		}
	}

	private void OnDestroy()
	{
		if (activeInstance == this)
		{
			activeInstance = null;
			Cleanup();
		}
	}

	private void OnApplicationQuit()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (SteamInterface != null)
		{
			if (Application.isEditor)
			{
				SteamInterface.ReleaseManagedResources();
			}
			else
			{
				SteamInterface.Shutdown();
			}
			SteamInterface = null;
		}
	}
}
