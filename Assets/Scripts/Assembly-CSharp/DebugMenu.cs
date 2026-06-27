using UnityEngine;

public class DebugMenu : MonoBehaviour
{
	private string[] modeTxt;

	private string[] levelTxt;

	private string[] typeTxt;

	private string[] itemTxt;

	private int index;

	private int pick;

	private float delay;

	private bool ready = true;

	public static bool init = false;

	public static int mode = 0;

	public static int level = 1;

	public static int[] type = new int[8];

	public static bool[] item = new bool[7];

	private void Start()
	{
		if (!init)
		{
			init = true;
			for (int i = 5; i < 8; i++)
			{
				type[i] = 1;
			}
		}
		modeTxt = new string[3] { " Monster", " AI", " Boss" };
		levelTxt = new string[5] { " Lv.1", " Lv.2", " Lv.3", " Lv.4", " Lv.5" };
		typeTxt = new string[3] { " Player #", " Com. #", " Off" };
		itemTxt = new string[7] { "Fire", "Bomb", "Speed", "Pierce", "Pass", "Glove", "Kick" };
	}

	private void Update()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 8; i++)
		{
			if (type[i] == 0)
			{
				num++;
			}
			else if (type[i] == 1)
			{
				if (mode == 0)
				{
					num2++;
				}
				else
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			ready = false;
		}
		else
		{
			ready = num + num2 > 1;
		}
		GameController controller = GameInput.GetController();
		if (!controller.IsConnected())
		{
			return;
		}
		if (ready && controller.DoStartMenu())
		{
			GameBoss.HardMode = true;
			Global.Mode = GameMode.DebugMode;
			Global.advStage = 1 + (level - 1) * 6 + ((mode == 2) ? 5 : 0);
			Application.LoadLevel("GameMode");
		}
		else if (controller.DoSelectMenu())
		{
			Application.LoadLevel("MainMenu");
		}
		if (delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}
		float horizontal = controller.GetHorizontal();
		switch (index)
		{
		case 0:
			if (horizontal < 0f && mode > 0)
			{
				mode--;
				GameSound.StartSFX("bombKick");
			}
			else if (horizontal > 0f && mode < 2)
			{
				mode++;
				GameSound.StartSFX("bombKick");
			}
			break;
		case 1:
			if (horizontal < 0f && level > 1)
			{
				level--;
				GameSound.StartSFX("bombKick");
			}
			else if (horizontal > 0f && level < 5)
			{
				level++;
				GameSound.StartSFX("bombKick");
			}
			break;
		case 10:
			if (horizontal < 0f)
			{
				pick--;
				if (pick < 0)
				{
					pick = 6;
				}
				GameSound.StartSFX("bombDrop");
			}
			else if (horizontal > 0f)
			{
				pick++;
				if (pick > 6)
				{
					pick = 0;
				}
				GameSound.StartSFX("bombDrop");
			}
			break;
		default:
		{
			int num3 = ((index - 2 >= 5) ? 1 : 0);
			if (horizontal < 0f && type[index - 2] > num3)
			{
				type[index - 2]--;
				GameSound.StartSFX("bombKick");
			}
			else if (horizontal > 0f && type[index - 2] < 2)
			{
				type[index - 2]++;
				GameSound.StartSFX("bombKick");
			}
			break;
		}
		}
		float vertical = controller.GetVertical();
		if (vertical < 0f)
		{
			index--;
			if (index < 0)
			{
				index = 10;
			}
			GameSound.StartSFX("bombDrop");
		}
		else if (vertical > 0f)
		{
			index++;
			if (index > 10)
			{
				index = 0;
			}
			GameSound.StartSFX("bombDrop");
		}
		if (horizontal != 0f || vertical != 0f)
		{
			delay = 0.1f;
		}
	}

	private void OnGUI()
	{
		int num = 118;
		int num2 = 21;
		int num3 = 128;
		int num4 = 31;
		float num5 = 4 * num3 * 2;
		float num6 = 11 * num4 * 2;
		float num7 = ((!((float)Screen.width < num5) && !((float)Screen.height < num6)) ? 2f : (2f * Mathf.Min((float)Screen.width / num5, (float)Screen.height / num6)));
		GUI.matrix = Matrix4x4.Scale(new Vector3(num7, num7, 1f));
		float num8 = (float)Screen.width / num7 / 2f;
		float num9 = (float)Screen.height / num7 / 2f;
		int num10 = (num4 - num2) / 2;
		num8 -= (float)(4 * num3 / 2);
		num9 -= (float)(11 * num4 / 2 - num10);
		Color color = GUI.color;
		Color color2 = color;
		Color color3 = new Color(1f, 1f, 1f, 0f);
		Color color4 = new Color(1f, 1f, 1f, 0.4f + Mathf.PingPong(Time.time * 1.5f, 0.6f));
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		GUI.Box(new Rect(num8 - (float)num10 - 3f, num9 - (float)num10, num3 * 4 + num10 * 2, num4 * 2), string.Empty);
		GUI.Box(new Rect(num8 - (float)num10 - 3f, num9 + (float)(2 * num4) - (float)num10, num3 * 4 + num10 * 2, num4 * 8), string.Empty);
		GUI.Box(new Rect(num8 - (float)num10 - 3f, num9 + (float)(10 * num4) - (float)num10, num3 * 4 + num10 * 2, num4), string.Empty);
		GUI.color = color;
		GUI.Label(new Rect(num8, num9, num3 * 4, num4), " Computer Mode:");
		GUI.Label(new Rect(num8, num9 + (float)(10 * num4), num3 * 4, num4), " Item:");
		for (int i = 0; i < 3; i++)
		{
			Rect position = new Rect(num8 + (float)((i + 1) * num3), num9, num, num2);
			GUI.color = color3;
			if (GUI.Button(position, string.Empty))
			{
				if (mode != i)
				{
					GameSound.StartSFX("bombKick");
				}
				if (index != 0)
				{
					GameSound.StartSFX("bombDrop");
				}
				mode = i;
				index = 0;
			}
			GUI.color = color;
			GUI.Toggle(position, mode == i, modeTxt[i]);
		}
		for (int j = 0; j < 5; j++)
		{
			Rect position2 = new Rect(num8 + (float)((j + 2) * num3 / 2), num9 + (float)num4, num / 2, num2);
			GUI.color = color3;
			if (GUI.Button(position2, string.Empty))
			{
				if (level != j + 1)
				{
					GameSound.StartSFX("bombKick");
				}
				if (index != 1)
				{
					GameSound.StartSFX("bombDrop");
				}
				level = j + 1;
				index = 1;
			}
			GUI.color = color;
			GUI.Toggle(position2, level == j + 1, levelTxt[j]);
		}
		if (mode == 2)
		{
			color = new Color(1f, 1f, 1f, 0.4f);
		}
		int num11 = 5;
		for (int k = 1; k <= 8; k++)
		{
			for (int l = ((k > num11) ? 1 : 0); l < 3; l++)
			{
				Rect position3 = new Rect(num8 + (float)((l + 1) * num3), num9 + (float)((k + 1) * num4), num, num2);
				GUI.color = color3;
				if (GUI.Button(position3, string.Empty))
				{
					if (type[k - 1] != l)
					{
						GameSound.StartSFX("bombKick");
					}
					if (index != k + 1)
					{
						GameSound.StartSFX("bombDrop");
					}
					type[k - 1] = l;
					index = k + 1;
				}
				GUI.color = color;
				GUI.Toggle(position3, type[k - 1] == l, (l >= 2) ? typeTxt[l] : (typeTxt[l] + k));
			}
		}
		if (mode == 2)
		{
			color = color2;
		}
		for (int m = 0; m < 7; m++)
		{
			Rect position4 = new Rect(num8 + (float)((m + 1) * num3 / 2), num9 + (float)(10 * num4), num / 2, num2);
			GUI.color = color3;
			if (GUI.Button(position4, string.Empty))
			{
				GameSound.StartSFX("bombKick");
				if (index != 10)
				{
					GameSound.StartSFX("bombDrop");
				}
				pick = m;
				index = 10;
				item[m] = !item[m];
			}
			GUI.color = ((pick != m || index != 10) ? color : color4);
			GUI.Toggle(position4, item[m], (pick != m || index != 10) ? (" " + itemTxt[m]) : ("[" + itemTxt[m] + "]"));
		}
		GUI.color = color4;
		GUI.Box(new Rect(num8 - (float)num10, num9 + (float)(index * num4) - (float)num10 + 3f, num3 * 4 + num10 * 2 - 6, num4 - 6), string.Empty);
		GUI.color = color;
		if (!ready)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.25f);
		}
		if (GUI.Button(new Rect(num8 + (float)(num3 / 8), num9 + (float)(13 * num4 / 2) - (float)num10, 4 * num / 3, num4 * 3), "PLAY!\n\nEnter (PC)\nor + (WiiU)") && ready)
		{
			GameBoss.HardMode = true;
			Global.Mode = GameMode.DebugMode;
			Global.advStage = 1 + (level - 1) * 6 + ((mode == 2) ? 5 : 0);
			Debug.Log(Global.Level + "-" + Global.IsBossStage);
			Application.LoadLevel("GameMode");
		}
	}
}
