using System;
using System.Runtime.InteropServices;
using ManagedSteam;
using ManagedSteam.CallbackStructures;
using ManagedSteam.SteamTypes;
using UnityEngine;

public class Utils : MonoBehaviour
{
	private bool bGamepadTextInputDismissed;

	private void Start()
	{
		if (Steamworks.SteamInterface == null)
		{
			Debug.LogError("SteamInterface startup failed!");
			return;
		}
		IUtils utils = Steamworks.SteamInterface.Utils;
		utils.LowBatteryPower += LowBatteryPowerFunc;
		utils.SteamShutdown += SteamShutdownFunc;
		utils.GamepadTextInputDismissed += GamepadTextInputDismissedFunc;
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

	public string GetGamepadTextInput()
	{
		IUtils utils = Steamworks.SteamInterface.Utils;
		bGamepadTextInputDismissed = false;
		if (utils.ShowGamepadTextInput(GamepadTextInputMode.NormalMode, GamepadTextInputLineMode.SingleLine, "Example text input", 64u))
		{
			while (!bGamepadTextInputDismissed)
			{
			}
		}
		return utils.GetEnteredGamepadTextInput().Text;
	}

	public Texture2D GetTextureFromSteamID(SteamID steamId)
	{
		IFriends friends = Steamworks.SteamInterface.Friends;
		IUtils utils = Steamworks.SteamInterface.Utils;
		ImageHandle largeFriendAvatar = friends.GetLargeFriendAvatar(steamId);
		uint width;
		uint height;
		if (largeFriendAvatar.IsValid && utils.GetImageSize(largeFriendAvatar, out width, out height))
		{
			Texture2D texture2D = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, true);
			Color32[] array = new Color32[width * height];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
				if (utils.GetImageRGBA(largeFriendAvatar, buffer, (int)(width * height * 4)))
				{
					for (int i = 0; i < width; i++)
					{
						for (int j = 0; j < height / 2; j++)
						{
							Color32 color = array[i + width * j];
							array[i + width * j] = array[i + width * (height - 1 - j)];
							array[i + width * (height - 1 - j)] = color;
						}
					}
					texture2D.SetPixels32(array);
					texture2D.Apply();
				}
			}
			finally
			{
				gCHandle.Free();
			}
			return texture2D;
		}
		return null;
	}

	private void Update()
	{
	}
}
