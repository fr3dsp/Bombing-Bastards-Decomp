using System;
using System.IO;
using UnityEngine;

public class GameSave
{
	private static string fileName;

	private static void Init()
	{
		if (fileName == null)
		{
			fileName = Application.persistentDataPath + "/save.dat";
		}
	}

	public static void SetDataPath(string path)
	{
		if (path == null)
		{
			fileName = Application.persistentDataPath + "/save.dat";
		}
		else
		{
			fileName = path.Replace('\\', '/') + "/save.dat";
		}
	}

	public static bool Load()
	{
		Init();
		if (!File.Exists(fileName))
		{
			return Reset();
		}
		string text;
		string text2;
		string text3;
		string text4;
		string text5;
		try
		{
			StreamReader streamReader = File.OpenText(fileName);
			text = streamReader.ReadLine();
			text2 = streamReader.ReadLine();
			text3 = streamReader.ReadLine();
			text4 = streamReader.ReadLine();
			text5 = streamReader.ReadLine();
			streamReader.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
			Reset();
			return false;
		}
		bool sound = false;
		bool music = false;
		bool voice = false;
		bool itemV = false;
		bool intro = false;
		int result = 0;
		int[] array = null;
		int[] array2 = null;
		bool flag = false;
		if (text.Length == 5)
		{
			for (int i = 0; i < 5; i++)
			{
				if (text[i] != '0' && text[i] != '1')
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				sound = text[0] == '1';
				music = text[1] == '1';
				voice = text[2] == '1';
				itemV = text[3] == '1';
				intro = text[4] == '1';
			}
		}
		else
		{
			flag = true;
		}
		if (!flag)
		{
			if (int.TryParse(text2, out result))
			{
				if (result < 0 || result > Global.advStar.Length)
				{
					flag = true;
				}
				else if (text2 != string.Empty + result)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
			if (text3.Length == Global.advStar.Length)
			{
				array = new int[text3.Length];
				for (int j = 0; j < text3.Length; j++)
				{
					array[j] = text3[j] - 48;
					if (array[j] < -2 || array[j] > 4)
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
			if (text4.Length == AdventureGameMode.shownText.Length)
			{
				array2 = new int[text4.Length];
				for (int k = 0; k < text4.Length; k++)
				{
					if (text4[k] != '0' && text4[k] != '1')
					{
						flag = true;
						break;
					}
					array2[k] = text4[k] - 48;
				}
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
			if ((array[0] == 0 && result == 0) || (array[0] == 1 && result > 0))
			{
				for (int l = 1; l < array.Length; l++)
				{
					if (l > result)
					{
						if (array[l] != 4)
						{
							flag = true;
							break;
						}
						continue;
					}
					bool flag2 = l % 6 == 0;
					if (l < result)
					{
						if ((array[l] < 1 || array[l] > 3) && (!flag2 || array[l] != -2))
						{
							flag = true;
							break;
						}
					}
					else if (array[l] != 0 && (!flag2 || array[l] != -1))
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			Reset();
			return false;
		}
		Global.SetSound(sound);
		Global.SetMusic(music);
		Global.SetVoice(voice);
		Global.SetItemV(itemV);
		Global.SetIntro(intro);
		Global.advUnlock = result;
		Global.advStar = array;
		Global.onlinePlayerName = ((text5 != null && text5.Length != 0) ? text5 : "Guest");
		AdventureGameMode.shownText = array2;
		return true;
	}

	public static bool Save()
	{
		Init();
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text = string.Empty;
		string text2 = string.Empty;
		string onlinePlayerName = Global.onlinePlayerName;
		empty += ((!Global.IsSoundOn) ? "0" : "1");
		empty += ((!Global.IsMusicOn) ? "0" : "1");
		empty += ((!Global.IsVoiceOn) ? "0" : "1");
		empty += ((!Global.IsItemVOn) ? "0" : "1");
		empty += ((!Global.IsIntroOn) ? "0" : "1");
		empty2 += Global.advUnlock;
		for (int i = 0; i < Global.advStar.Length; i++)
		{
			text += (char)(48 + Global.advStar[i]);
		}
		for (int j = 0; j < AdventureGameMode.shownText.Length; j++)
		{
			text2 += ((AdventureGameMode.shownText[j] != 0) ? "1" : "0");
		}
		try
		{
			StreamWriter streamWriter = File.CreateText(fileName);
			streamWriter.WriteLine(empty);
			streamWriter.WriteLine(empty2);
			streamWriter.WriteLine(text);
			streamWriter.WriteLine(text2);
			streamWriter.WriteLine(onlinePlayerName);
			streamWriter.Close();
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
			return false;
		}
	}

	public static bool Reset()
	{
		Global.advPlanet = 0;
		Global.advUnlock = 1;
		Global.advStage = 0;
		for (int i = 0; i < Global.advStar.Length; i++)
		{
			if (i < Global.advUnlock)
			{
				Global.advStar[i] = 1;
			}
			else if (i == Global.advUnlock)
			{
				Global.advStar[i] = 0;
			}
			else
			{
				Global.advStar[i] = 4;
			}
		}
		for (int j = 0; j < AdventureGameMode.shownText.Length; j++)
		{
			AdventureGameMode.shownText[j] = 0;
		}
		return Save();
	}
}
