using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetting
{
	private static string fileName;

	public static int presetAmount = 0;

	public static Dictionary<int, JoyLayout> presetContainer;

	public static JoyLayout[] defaultLayout;

	public static int[,] playerLayout;

	public static string[] presetName;

	public static string[] restlist = new string[20]
	{
		"640x480", "720x480", "720x576", "800x600", "1024x768", "720x480", "720x576", "800x600", "1024x768", "720x480",
		"640x480", "720x480", "720x576", "800x600", "1024x768", "720x480", "720x576", "800x600", "1024x768", "720x480"
	};

	public static string[] quatlist = new string[3] { "Fastest", "Good", "Fantastic" };

	public static string[] contDevice;

	private static int index = 0;

	private static int lastPresetID = 0;

	public static void Init()
	{
		if (fileName == null)
		{
			fileName = "Controlsetting.dat";
			playerLayout = new int[5, 2];
			defaultLayout = new JoyLayout[5];
			presetContainer = new Dictionary<int, JoyLayout>();
			LoadDefaultControl();
			LoadControlConfig();
		}
		presetName = new string[presetContainer.Count];
		for (int i = 0; i < presetName.Length; i++)
		{
			JoyLayout value;
			if (presetContainer.TryGetValue(i, out value))
			{
				presetName[i] = value.presetName;
				Debug.Log(presetName[i]);
			}
		}
		Resolution[] resolutions = Screen.resolutions;
		if (resolutions != null)
		{
			restlist = new string[resolutions.Length];
		}
		for (int j = 0; j < resolutions.Length; j++)
		{
			restlist[j] = resolutions[j].width + "x" + resolutions[j].height;
		}
		quatlist = QualitySettings.names;
		if (!File.Exists("Graphicsetting.dat"))
		{
			Resolution currentResolution = Screen.currentResolution;
			Debug.Log("cur res :: " + currentResolution.width + "x" + currentResolution.height);
			Screen.SetResolution(currentResolution.width, currentResolution.height, true);
			QualitySettings.SetQualityLevel(3, true);
			Global.Resolution = GetResolutionID();
		}
		else
		{
			LoadGraphicSetting();
		}
		contDevice = new string[Input.GetJoystickNames().Length + 1];
		contDevice[0] = "Keyboard&Mouse";
		for (int k = 1; k < contDevice.Length; k++)
		{
			contDevice[k] = Input.GetJoystickNames()[k - 1];
		}
	}

	public static int GetResolutionID(string resolution = null)
	{
		Resolution currentResolution = Screen.currentResolution;
		if (resolution == null)
		{
			resolution = string.Concat(currentResolution, "x", currentResolution);
		}
		for (int i = 0; i < restlist.Length; i++)
		{
			if (restlist[i].Equals(currentResolution.width + "x" + currentResolution.height))
			{
				return i;
			}
		}
		return restlist.Length - 1;
	}

	public static bool addPreset(string[,] keyset, int joy)
	{
		JoyLayout joyLayout = new JoyLayout();
		lastPresetID = presetContainer[presetContainer.Count - 1].presetID + 1;
		presetAmount++;
		joyLayout.presetID = lastPresetID;
		joyLayout.device = "Controller";
		joyLayout.presetName = "Preset" + (lastPresetID - 1);
		for (int i = 0; i < keyset.GetLength(0); i++)
		{
			joyLayout.game[i, 0] = keyset[i, 0];
			joyLayout.game[i, 1] = keyset[i, 1];
		}
		joyLayout.initKeycode();
		presetContainer.Add(index, joyLayout);
		playerLayout[joy, 0] = joyLayout.presetID;
		playerLayout[joy, 1] = index;
		string[] array = new string[presetName.Length + 1];
		for (int j = 0; j < presetName.Length; j++)
		{
			array[j] = presetName[j];
		}
		array[array.Length - 1] = joyLayout.presetName;
		presetName = array;
		index++;
		array = null;
		joyLayout = null;
		return true;
	}

	public static bool compareProfile(string[,] checkKey)
	{
		for (int i = 0; i < presetContainer.Count; i++)
		{
			int num = 0;
			for (int j = 0; j < presetContainer[i].game.GetLength(0); j++)
			{
				Debug.Log("check " + i + ">" + checkKey[j, 0] + " : " + presetContainer[i].game[j, 0]);
				if (checkKey[j, 0].Equals(presetContainer[i].game[j, 0]))
				{
					num++;
				}
				Debug.Log("check " + i + ">" + checkKey[j, 1] + " : " + presetContainer[i].game[j, 1]);
				if (checkKey[j, 1].Equals(presetContainer[i].game[j, 1]))
				{
					num++;
				}
				if (num == checkKey.Length)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool LoadDefaultControl()
	{
		try
		{
			int num = 0;
			JoyLayout joyLayout = new JoyLayout();
			StreamReader streamReader = File.OpenText("Controldefault.dat");
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text == null || text.Equals(string.Empty) || (!text.Substring(0, 1).Equals("0") && !text.Substring(0, 1).Equals("1")))
				{
					continue;
				}
				int num2 = int.Parse(text.Substring(0, 2));
				switch (num2)
				{
				case 1:
					if (joyLayout == null)
					{
						joyLayout = new JoyLayout();
					}
					num = (joyLayout.presetID = int.Parse(text.Split('=')[1]));
					break;
				case 2:
					joyLayout.presetName = text.Split('=')[1];
					break;
				case 3:
					joyLayout.device = text.Split('=')[1];
					break;
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
					joyLayout.game[num2 - 4, 0] = text.Split('=')[1].Split(',')[0];
					joyLayout.game[num2 - 4, 1] = text.Split('=')[1].Split(',')[1];
					if (num2 == 11)
					{
						joyLayout.initKeycode();
						defaultLayout[num - 1] = joyLayout;
						joyLayout = null;
					}
					break;
				}
			}
			while (text != null);
			Debug.LogWarning("Load file from Controldefault.dat");
			streamReader.Close();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
		return true;
	}

	public static bool LoadControlConfig(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = fileName;
		}
		if (!File.Exists(filePath))
		{
			return DefaultControlConfig();
		}
		try
		{
			JoyLayout joyLayout = new JoyLayout();
			joyLayout.presetID = 0;
			joyLayout.presetName = "None";
			joyLayout.device = "Controller";
			for (int i = 0; i < joyLayout.game.Length; i++)
			{
				joyLayout.game[i / 2, i % 2] = string.Empty;
			}
			joyLayout.initKeycode();
			presetContainer.Add(index, joyLayout);
			joyLayout = null;
			index++;
			joyLayout = new JoyLayout();
			joyLayout.presetID = 1;
			joyLayout.presetName = "Default";
			joyLayout.device = "Controller";
			for (int j = 0; j < joyLayout.game.Length; j++)
			{
				joyLayout.game[j / 2, j % 2] = string.Empty;
			}
			joyLayout.initKeycode();
			presetContainer.Add(index, joyLayout);
			joyLayout = null;
			index++;
			lastPresetID = index;
			StreamReader streamReader = File.OpenText(filePath);
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text == null || text.Equals(string.Empty))
				{
					continue;
				}
				if (text.Substring(0, 2).Equals("pa"))
				{
					int num = int.Parse(text.Substring(text.IndexOf("=") + 1));
					presetAmount = num + 2;
					Debug.Log("amount of preset" + num + " , all = " + presetAmount);
				}
				else if (text.Substring(0, 2).Equals("pl"))
				{
					int num2 = int.Parse(text.Split('=')[1]);
					playerLayout[int.Parse(text.Substring(6, 1)) - 1, 0] = num2;
					if (num2 <= 1)
					{
						playerLayout[int.Parse(text.Substring(6, 1)) - 1, 1] = num2;
					}
					Debug.Log("player:" + text.Substring(6, 1) + " preset " + playerLayout[int.Parse(text.Substring(6, 1)) - 1, 0]);
				}
				else
				{
					if (!text.Substring(0, 1).Equals("0") && !text.Substring(0, 1).Equals("1"))
					{
						continue;
					}
					int num3 = int.Parse(text.Substring(0, 2));
					switch (num3)
					{
					case 1:
					{
						joyLayout = new JoyLayout();
						joyLayout.presetID = int.Parse(text.Split('=')[1]);
						for (int k = 0; k < playerLayout.GetLength(0); k++)
						{
							if (playerLayout[k, 0] == joyLayout.presetID)
							{
								Debug.Log("load layout : " + playerLayout[k, 0] + " , " + index);
								playerLayout[k, 1] = index;
							}
						}
						break;
					}
					case 2:
						joyLayout.presetName = text.Split('=')[1];
						break;
					case 3:
						joyLayout.device = text.Split('=')[1];
						break;
					case 4:
					case 5:
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
						joyLayout.game[num3 - 4, 0] = text.Split('=')[1].Split(',')[0];
						joyLayout.game[num3 - 4, 1] = text.Split('=')[1].Split(',')[1];
						if (num3 == 11)
						{
							joyLayout.initKeycode();
							lastPresetID = joyLayout.presetID;
							presetContainer.Add(index, joyLayout);
							joyLayout = null;
							index++;
						}
						break;
					}
				}
			}
			while (text != null);
			streamReader.Close();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
		bool flag = false;
		Debug.LogWarning("Load file from " + fileName);
		if (flag)
		{
			Debug.LogError(fileName + " is corrupted.");
			return false;
		}
		return true;
	}

	public static bool ApplyNewKey(string[,] keyset, int joy)
	{
		for (int i = 0; i < keyset.GetLength(0); i++)
		{
			presetContainer[playerLayout[joy, 1]].game[i, 0] = keyset[i, 0];
			presetContainer[playerLayout[joy, 1]].game[i, 1] = keyset[i, 1];
		}
		presetContainer[playerLayout[joy, 1]].initKeycode();
		return true;
	}

	public static bool ApplyControlConfig(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = "Controlsetting.dat";
		}
		try
		{
			int num = 1;
			StreamWriter streamWriter = File.CreateText(filePath);
			streamWriter.WriteLine("pa=" + (presetAmount - 2));
			streamWriter.WriteLine(string.Empty);
			streamWriter.WriteLine("#preset id that player is using 0 is default.");
			for (int i = 0; i < 5; i++)
			{
				Debug.Log("player" + (i + 1) + "=" + playerLayout[i, 0]);
				streamWriter.WriteLine("player" + (i + 1) + "=" + playerLayout[i, 0]);
			}
			streamWriter.WriteLine(string.Empty);
			num = 3;
			foreach (KeyValuePair<int, JoyLayout> item in presetContainer)
			{
				if (item.Key > 1)
				{
					JoyLayout value = item.Value;
					streamWriter.WriteLine("01id=" + value.presetID);
					streamWriter.WriteLine("02preset=" + value.presetName);
					streamWriter.WriteLine("03device=" + value.device);
					streamWriter.WriteLine("#gameplay");
					streamWriter.WriteLine("04up=" + value.game[0, 0] + "," + value.game[0, 1]);
					streamWriter.WriteLine("05down=" + value.game[1, 0] + "," + value.game[1, 1]);
					streamWriter.WriteLine("06left=" + value.game[2, 0] + "," + value.game[2, 1]);
					streamWriter.WriteLine("07right=" + value.game[3, 0] + "," + value.game[3, 1]);
					streamWriter.WriteLine("08dropbomb=" + value.game[4, 0] + "," + value.game[4, 1]);
					streamWriter.WriteLine("09pickbomb=" + value.game[5, 0] + "," + value.game[5, 1]);
					streamWriter.WriteLine("10blowbomb=" + value.game[6, 0] + "," + value.game[6, 1]);
					streamWriter.WriteLine("11pause=" + value.game[7, 0] + "," + value.game[7, 1]);
					streamWriter.WriteLine(string.Empty);
					num++;
				}
			}
			streamWriter.Close();
			Debug.LogWarning("Save file to " + fileName);
			return true;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}

	public static bool DeletePreset(int joy)
	{
		if (playerLayout[joy, 1] < 2)
		{
			Debug.Log(presetContainer[playerLayout[joy, 1]].presetName);
			return false;
		}
		for (int i = 0; i < 5; i++)
		{
			Debug.Log("p:" + i + " Old layout : " + playerLayout[i, 1] + " , " + presetContainer[playerLayout[i, 1]].presetName);
		}
		int num = 0;
		Dictionary<int, JoyLayout> dictionary = new Dictionary<int, JoyLayout>();
		presetName = new string[presetContainer.Count - 1];
		presetContainer.Remove(playerLayout[joy, 1]);
		for (int j = 0; j < playerLayout.GetLength(0); j++)
		{
			if (j != joy)
			{
				if (playerLayout[j, 1] == playerLayout[joy, 1])
				{
					playerLayout[j, 1] = 1;
				}
				if (playerLayout[j, 0] == playerLayout[joy, 0])
				{
					playerLayout[j, 0] = 1;
				}
			}
		}
		playerLayout[joy, 1] = 1;
		playerLayout[joy, 0] = 1;
		index--;
		presetAmount--;
		Debug.Log("pr amount : " + presetAmount);
		if (presetAmount == 2)
		{
			lastPresetID = 2;
		}
		foreach (KeyValuePair<int, JoyLayout> item in presetContainer)
		{
			JoyLayout value = item.Value;
			dictionary.Add(num, value);
			presetName[num] = value.presetName;
			for (int k = 0; k < 5; k++)
			{
				if (value.presetID > 2 && value.presetID == playerLayout[k, 0])
				{
					Debug.Log("use new link : " + num + " for player : " + k);
					playerLayout[k, 1] = num;
				}
			}
			num++;
		}
		presetContainer = dictionary;
		dictionary = null;
		for (int l = 0; l < 5; l++)
		{
			Debug.Log("p:" + l + " New layout : " + playerLayout[l, 1] + " , " + presetContainer[playerLayout[l, 1]].presetName);
		}
		return true;
	}

	public static bool ClearPreset()
	{
		Debug.Log("clear all preset");
		presetName = new string[2];
		for (int i = 0; i < presetAmount; i++)
		{
			Debug.Log(i + " check : " + presetContainer[i].presetName);
			if (i > 1)
			{
				Debug.Log("remove : " + presetContainer[i].presetName);
				presetContainer.Remove(i);
			}
			else
			{
				presetName[i] = presetContainer[i].presetName;
			}
		}
		playerLayout[0, 0] = 1;
		playerLayout[0, 1] = 1;
		for (int j = 1; j < playerLayout.GetLength(0); j++)
		{
			playerLayout[j, 0] = 1;
			playerLayout[j, 1] = 1;
		}
		presetAmount = 2;
		lastPresetID = 2;
		index = 2;
		DefaultControlConfig();
		return true;
	}

	public static bool DefaultControlConfig(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = "Controldefault.dat";
		}
		try
		{
			StreamWriter streamWriter = File.CreateText(fileName);
			streamWriter.WriteLine("pa=0");
			streamWriter.WriteLine(string.Empty);
			streamWriter.WriteLine("#preset id that player is using 1 is default.");
			for (int i = 1; i <= 5; i++)
			{
				streamWriter.WriteLine("player" + i + "=1");
			}
			streamWriter.Close();
			Debug.LogWarning("Save file to " + fileName);
			LoadControlConfig();
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
			return false;
		}
	}

	public static bool LoadGraphicSetting(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = "Graphicsetting.dat";
		}
		if (!File.Exists(filePath))
		{
			return DefaultGraphicSetting();
		}
		try
		{
			StreamReader streamReader = File.OpenText(filePath);
			string text;
			do
			{
				text = streamReader.ReadLine();
				if (text == null)
				{
					break;
				}
				string text2 = text.Substring(0, 1);
				if (text2.Equals("f"))
				{
					Global.FullScreen = (text.Split('=')[1].Equals("True") ? true : false);
				}
				else if (text2.Equals("r"))
				{
					Global.Resolution = int.Parse(text.Split('=')[1]);
				}
				else if (text2.Equals("q"))
				{
					Global.Quality = int.Parse(text.Split('=')[1]);
				}
			}
			while (text != null);
			streamReader.Close();
			int width = int.Parse(restlist[Global.Resolution].Substring(0, restlist[Global.Resolution].IndexOf("x")));
			int height = int.Parse(restlist[Global.Resolution].Substring(restlist[Global.Resolution].IndexOf("x") + 1));
			Screen.SetResolution(width, height, Global.FullScreen);
			QualitySettings.SetQualityLevel(Global.Quality, true);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
		Debug.LogWarning("Load file from " + filePath);
		return true;
	}

	public static bool ApplyGraphicSetting(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = "Graphicsetting.dat";
		}
		try
		{
			StreamWriter streamWriter = File.CreateText(filePath);
			streamWriter.WriteLine("fullscreen=" + Global.FullScreen);
			streamWriter.WriteLine("resolution=" + Global.Resolution);
			streamWriter.WriteLine("quality=" + Global.Quality);
			streamWriter.Close();
			Debug.LogWarning("Save file to " + filePath);
			return true;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}

	public static bool DefaultGraphicSetting(string filePath = null)
	{
		if (filePath == null)
		{
			filePath = "Graphicsetting.dat";
		}
		Global.FullScreen = true;
		Global.Resolution = restlist.Length - 1;
		Global.Quality = 2;
		try
		{
			StreamWriter streamWriter = File.CreateText("Graphicsetting.dat");
			streamWriter.WriteLine("fullscreen=t");
			streamWriter.WriteLine("resolution=" + (restlist.Length - 1));
			streamWriter.WriteLine("quality=2");
			streamWriter.Close();
			Debug.LogWarning("Save file to " + filePath);
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
			return false;
		}
	}
}
