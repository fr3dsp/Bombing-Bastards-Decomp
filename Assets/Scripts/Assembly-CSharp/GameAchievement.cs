using System;
using ManagedSteam;
using ManagedSteam.CallbackStructures;
using ManagedSteam.Exceptions;
using ManagedSteam.SteamTypes;
using UnityEngine;

public class GameAchievement : MonoBehaviour
{
	public enum Achievement
	{
		Good_Start = 0,
		Ready_of_the_Boss = 1,
		Lucky_Bastard = 2,
		Promising_Bastard = 3,
		Skilled_Bastard = 4,
		Hardcore_Bastard = 5,
		Total_Bastard = 6,
		Fairy_King = 7,
		Fairy_God = 8,
		Ice_King = 9,
		Ice_God = 10,
		Wind_King = 11,
		Wind_God = 12,
		Fire_King = 13,
		Fire_God = 14,
		Spark_King = 15,
		Spark_God = 16,
		MAX = 17
	}

	private static Steam SteamInterface;

	private static string Path;

	private static GameAchievement activeInstance;

	private string status;

	private UserHasLicenseForAppResult hasLicense;

	private bool bGamepadTextInputDismissed;

	public static void ProcessAchievement()
	{
		if (SteamInterface == null)
		{
			return;
		}
		if (Global.advStar[1] > 0 && Global.advStar[1] < 4)
		{
			UnlockAchievement(Achievement.Good_Start);
		}
		bool flag = true;
		for (int i = 1; i < 6; i++)
		{
			if (Global.advStar[i] < 1 || Global.advStar[i] == 4)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			UnlockAchievement(Achievement.Ready_of_the_Boss);
		}
		int num = 0;
		for (int num2 = Global.advStar.Length - 1; num2 >= 0; num2--)
		{
			if (Global.advStar[num2] == 3)
			{
				num++;
			}
		}
		if (num >= 1)
		{
			UnlockAchievement(Achievement.Lucky_Bastard);
			if (num >= 5)
			{
				UnlockAchievement(Achievement.Promising_Bastard);
				if (num >= 10)
				{
					UnlockAchievement(Achievement.Skilled_Bastard);
					if (num >= 20)
					{
						UnlockAchievement(Achievement.Hardcore_Bastard);
						if (num >= 30)
						{
							UnlockAchievement(Achievement.Total_Bastard);
						}
					}
				}
			}
		}
		for (int j = 0; j < 5; j++)
		{
			int num3 = (j + 1) * 6;
			if (Global.advStar[num3] > 0 && Global.advStar[num3] < 4)
			{
				UnlockAchievement((Achievement)(7 + j * 2));
				if (Global.advStar[num3] == 3)
				{
					UnlockAchievement((Achievement)(8 + j * 2));
				}
			}
		}
	}

	public static bool UnlockAchievement(Achievement achievement)
	{
		bool flag = SteamInterface.Stats.SetAchievement("BombingBastards_Achievement_" + ((int)(achievement + 1)).ToString("D2"));
		if (flag)
		{
			SteamInterface.Stats.StoreStats();
		}
		return flag;
	}

	public static bool CheckAchievement(Achievement achievement)
	{
		StatsGetAchievementResult achievement2 = SteamInterface.Stats.GetAchievement("BombingBastards_Achievement_" + ((int)(achievement + 1)).ToString("D2"));
		return achievement2.result && achievement2.sender;
	}

	public static bool ClearAchievement(Achievement achievement)
	{
		return SteamInterface.Stats.ClearAchievement("BombingBastards_Achievement_" + ((int)(achievement + 1)).ToString("D2"));
	}

	private void Awake()
	{
		Steam.RestartAppIfNecessary(314220u);
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
				flag = true;
				Debug.LogError(status, this);
				Debug.LogError(ex.Message, this);
			}
			catch (SteamInitializeFailedException ex2)
			{
				status = "Could not initialize the native Steamworks API. This is usually caused by a missing steam_appid.txt file or if the Steam client is not running.";
				flag = true;
				Debug.LogError(status, this);
				Debug.LogError(ex2.Message, this);
			}
			catch (SteamInterfaceInitializeFailedException ex3)
			{
				status = "Could not initialize the wanted versions of the Steamworks API. Make sure that you have the correct Steamworks SDK version. See the documentation for more info.";
				flag = true;
				Debug.LogError(status, this);
				Debug.LogError(ex3.Message, this);
			}
			catch (DllNotFoundException ex4)
			{
				status = "Could not load a dll file. Make sure that the steam_api.dll/libsteam_api.dylib file is placed at the correct location. See the documentation for more info.";
				flag = true;
				Debug.LogError(status, this);
				Debug.LogError(ex4.Message, this);
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

	private void Start()
	{
		if (SteamInterface == null)
		{
			Debug.LogError("SteamInterface startup failed!");
			Application.Quit();
			return;
		}
		IStats stats = SteamInterface.Stats;
		stats.UserStatsReceived += UserStatsReceived;
		stats.UserStatsStored += UserStatsStored;
		stats.RequestCurrentStats();
		IUser user = SteamInterface.User;
		Path = user.GetUserDataFolder().Path;
		IUtils utils = SteamInterface.Utils;
		utils.LowBatteryPower += LowBatteryPowerFunc;
		utils.SteamShutdown += SteamShutdownFunc;
		utils.GamepadTextInputDismissed += GamepadTextInputDismissedFunc;
		GameSave.SetDataPath(Path);
		GameSave.Load();
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

	private void Update()
	{
		if (SteamInterface != null)
		{
			SteamInterface.Update();
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

	private void UserStatsReceived(UserStatsReceived value)
	{
		Debug.Log("value.GameID: " + value.GameID.ToString() + "; SteamInterface.AppID: " + SteamInterface.AppID.ToString());
		Debug.Log("value.Result: " + value.Result);
		if (!(value.GameID == new GameID(SteamInterface.AppID.AsUInt64)))
		{
			return;
		}
		if (value.Result != Result.OK)
		{
			Debug.LogError("Failed to download stats.");
			return;
		}
		IStats stats = SteamInterface.Stats;
		int data;
		if (!stats.GetStat("TestStatInt", out data))
		{
			Debug.LogWarning("Failed to read TestStatInt");
		}
		float data2;
		if (!stats.GetStat("TestStatFloat", out data2))
		{
			Debug.LogWarning("Failed to read TestStatFloat");
		}
		Debug.Log("TestStatInt = " + data);
		Debug.Log("TestStatFloat = " + data2);
		data++;
		data2 += 0.5f;
		if (!stats.SetStat("TestStatInt", data))
		{
			Debug.LogWarning("Failed to write TestStatInt");
		}
		if (!stats.SetStat("TestStatFloat", data2))
		{
			Debug.LogWarning("Failed to write TestStatFloat");
		}
		stats.StoreStats();
	}

	private void UserStatsStored(UserStatsStored value)
	{
		if (value.GameID == new GameID(SteamInterface.AppID.AsUInt64))
		{
			if (value.Result == Result.OK)
			{
				Debug.Log("Stats saved to the server successfully.");
			}
			else
			{
				Debug.LogWarning("Failed to save stats to the server.");
			}
		}
	}

	private void SteamShutdownFunc(SteamShutdown value)
	{
		Application.Quit();
	}

	private void LowBatteryPowerFunc(LowBatteryPower value)
	{
	}

	private void GamepadTextInputDismissedFunc(GamepadTextInputDismissed value)
	{
		bGamepadTextInputDismissed = true;
	}

	private void OnGUI()
	{
	}
}
