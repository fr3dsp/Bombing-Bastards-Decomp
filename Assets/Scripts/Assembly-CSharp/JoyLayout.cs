using System;
using UnityEngine;

public class JoyLayout
{
	public int presetID;

	public string presetName = string.Empty;

	public string device = string.Empty;

	public string[,] game = new string[8, 2];

	public KeyCode[,] gameKey = new KeyCode[8, 2];

	public void initKeycode()
	{
		for (int i = 0; i < game.GetLength(0); i++)
		{
			try
			{
				if (game[i, 0].Contains("Axis"))
				{
					gameKey[i, 0] = KeyCode.None;
				}
				else if (game[i, 0].Contains("Joy"))
				{
					int num = game[i, 0].IndexOf('k');
					int num2 = game[i, 0].IndexOf('B');
					int num3 = game[i, 0].IndexOf('n');
					int num4 = int.Parse(game[i, 0].Substring(num + 1, num2 - num - 1)) - 1;
					int num5 = int.Parse(game[i, 0].Substring(num3 + 1));
					gameKey[i, 0] = (KeyCode)(350 + num5 + num4 * 20);
				}
				else if (!game[i, 0].Equals(string.Empty))
				{
					gameKey[i, 0] = (KeyCode)(int)Enum.Parse(typeof(KeyCode), game[i, 0]);
				}
				else
				{
					gameKey[i, 0] = KeyCode.None;
				}
			}
			catch (Exception message)
			{
				gameKey[i, 0] = KeyCode.None;
				Debug.LogError(message);
			}
			try
			{
				if (game[i, 1].Contains("Axis"))
				{
					gameKey[i, 1] = KeyCode.None;
				}
				else if (game[i, 1].Contains("Joy"))
				{
					int num6 = game[i, 1].IndexOf('k');
					int num7 = game[i, 1].IndexOf('B');
					int num8 = game[i, 1].IndexOf('n');
					int num9 = int.Parse(game[i, 1].Substring(num6 + 1, num7 - num6 - 1)) - 1;
					int num10 = int.Parse(game[i, 1].Substring(num8 + 1));
					gameKey[i, 1] = (KeyCode)(350 + num10 + num9 * 20);
				}
				else if (!game[i, 1].Equals(string.Empty))
				{
					gameKey[i, 1] = (KeyCode)(int)Enum.Parse(typeof(KeyCode), game[i, 1]);
				}
				else
				{
					gameKey[i, 1] = KeyCode.None;
				}
			}
			catch (Exception message2)
			{
				gameKey[i, 1] = KeyCode.None;
				Debug.LogError(message2);
			}
		}
	}
}
