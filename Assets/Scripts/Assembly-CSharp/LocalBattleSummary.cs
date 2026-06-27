using System;
using UnityEngine;

public class LocalBattleSummary : MonoBehaviour
{
	private class Player
	{
		public string name;

		public int win;

		public int id;
	}

	private enum State
	{
		idle = 0,
		initial = 1,
		fadeIn = 2,
		showGoal = 3,
		showPlayer = 4,
		showCup = 5,
		getCup = 6,
		swapPlace = 7,
		nextScene = 8
	}

	private int goalCupAnim;

	private Transform propline1_1;

	private Transform propline1_2;

	private Transform ui_goal;

	private Transform[] cup;

	private float propline1_time;

	public int total_wins;

	public float propline3_movey;

	public float propline3_duration_time;

	private Transform propline2;

	private Transform propline3_1;

	private Transform propline3_2;

	private Transform propline3_1_effect;

	private Transform propline3_2_effect;

	private float propline3_time;

	private int propline3_step;

	private GameObject nextBtn;

	public int idWins;

	public float animationDurationTime;

	private bool[] mouseOver = new bool[5];

	private float[] pTime = new float[5];

	private string[] playerName = new string[9] { "OFF", "Player 1", "Player 2", "Player 3", "Player 4", "Player 5", "AI Easy", "AI Normal", "AI Hard" };

	private float pointerSpeed = Global.pointerSpd;

	private Player[] player;

	private GameObject[] pointer;

	private Transform[][] cupPlayer;

	private Transform[] ui_player;

	private Transform[] ui_color;

	private Transform cup_effect;

	private TextMesh[] ui_playerName;

	private float animationTime;

	private int animationStep;

	private int playerAmount;

	private int indexup;

	private int indexdown;

	private Vector3 positionup;

	private Vector3 positiondown;

	private float differencey;

	private GameObject[] togButton;

	private float fadeTime;

	private int[] pointerInit = new int[5];

	private State state;

	private Vector3 touchPos;

	private int[] ctrlStat = new int[5];

	private void Awake()
	{
	}

	private void Start()
	{
		for (int i = 1; i <= 5; i++)
		{
			GameSound.StopBGM("Map" + i.ToString("D2"));
		}
		GameSound.StopBGM("BossFight");
		GameSound.StartBGM("Menu");
		idWins = Global.winID;
		if (Global.localPlayerSlot != null)
		{
			for (int j = 0; j < 8; j++)
			{
				if (Global.localPlayerSlot[j] != CharSlot.Off)
				{
					playerAmount++;
				}
			}
			player = new Player[playerAmount];
			playerAmount = 0;
			for (int k = 0; k < 8; k++)
			{
				if (Global.localPlayerSlot[k] != CharSlot.Off)
				{
					player[playerAmount] = new Player();
					player[playerAmount].id = k;
					player[playerAmount].name = playerName[(int)Global.localPlayerSlot[k]];
					player[playerAmount].win = Global.cupAmount[k];
					if (k == idWins)
					{
						Global.cupAmount[k]++;
					}
					playerAmount++;
				}
			}
		}
		else
		{
			player = new Player[0];
		}
		total_wins = Global.winAmount;
		sortWins();
		ui_player = new Transform[8];
		ui_color = new Transform[8];
		ui_playerName = new TextMesh[8];
		cupPlayer = new Transform[8][];
		cup_effect = base.transform.Find("cup_effect");
		for (int l = 0; l < 8; l++)
		{
			ui_player[l] = base.transform.Find("ui_player_list1_" + (l + 1));
			ui_color[l] = ui_player[l].Find("ui_color");
			cupPlayer[l] = new Transform[5];
			for (int m = 0; m < 5; m++)
			{
				cupPlayer[l][m] = ui_player[l].transform.Find("cup" + (m + 1));
				cupPlayer[l][m].GetComponent<Renderer>().enabled = false;
			}
			if (l < player.Length)
			{
				ui_playerName[l] = ui_player[l].transform.GetComponentInChildren<TextMesh>();
				ui_playerName[l].text = player[l].name;
				ui_color[l].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)player[l].id * 0.125f, 0f));
				ui_color[l].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				ui_player[l].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				for (int n = 0; n < 5; n++)
				{
					cupPlayer[l][n] = ui_player[l].transform.Find("cup" + (n + 1));
					if (n < player[l].win)
					{
						cupPlayer[l][n].GetComponent<Renderer>().enabled = true;
						cupPlayer[l][n].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					}
					else
					{
						cupPlayer[l][n].GetComponent<Renderer>().enabled = false;
					}
				}
			}
			else
			{
				ui_playerName[l] = ui_player[l].transform.GetComponentInChildren<TextMesh>();
				ui_player[l].GetComponent<Renderer>().enabled = false;
				ui_playerName[l].GetComponent<Renderer>().enabled = false;
				ui_color[l].GetComponent<Renderer>().enabled = false;
			}
		}
		animationStep = 0;
		animationTime = 0f;
		cup_effect = base.transform.Find("cup_effect");
		cup_effect.GetComponent<Renderer>().enabled = false;
		indexdown = player.Length - 1;
		indexup = 0;
		differencey = ui_player[0].transform.localPosition.y - ui_player[1].transform.localPosition.y;
		propline1_1 = base.transform.Find("prop_line1_1");
		propline1_2 = base.transform.Find("prop_line1_2");
		ui_goal = base.transform.Find("ui_goal");
		cup = new Transform[5];
		for (int num = 0; num < 5; num++)
		{
			cup[num] = base.transform.Find("cup" + (num + 1));
		}
		propline2 = base.transform.Find("prop_line2_effect");
		propline3_1 = base.transform.Find("prop_line3_1");
		propline3_2 = base.transform.Find("prop_line3_2");
		propline3_1_effect = base.transform.Find("prop_line3_1_effect");
		propline3_2_effect = base.transform.Find("prop_line3_2_effect");
		propline3_time = 0f;
		propline3_step = 0;
		nextBtn = base.transform.Find("btn_next").gameObject;
		pointer = new GameObject[5];
		pointer[0] = GameObject.Find("p_pointer1").gameObject;
		pointer[1] = GameObject.Find("p_pointer2").gameObject;
		pointer[2] = GameObject.Find("p_pointer3").gameObject;
		pointer[3] = GameObject.Find("p_pointer4").gameObject;
		pointer[4] = GameObject.Find("p_pointer5").gameObject;
		togButton = new GameObject[5];
		base.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		state = State.initial;
	}

	private void Update()
	{
		float num = 2.5f;
		switch (state)
		{
		case State.initial:
		{
			propline1_1.localScale = new Vector3(0f, 0.0076f, 0f);
			propline1_2.localScale = new Vector3(0f, 0.0076f, 0f);
			for (int i = 0; i < 5; i++)
			{
				cup[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
			ui_goal.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			ui_goal.localPosition = new Vector3(0.77f, 0.653f, 2f);
			goalCupAnim = 0;
			state = State.fadeIn;
			break;
		}
		case State.fadeIn:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
			{
				fadeTime = 0f;
				checkController();
				state = State.showGoal;
			}
			break;
		case State.showGoal:
			PropLineRunEff();
			break;
		case State.showPlayer:
		{
			float num2 = Mathf.Lerp(0f, 0.5f, animationTime * 16.5f);
			if (animationStep < player.Length)
			{
				Color color = new Color(0.5f, 0.5f, 0.5f, num2);
				ui_player[animationStep].GetComponent<Renderer>().material.SetColor("_TintColor", color);
				ui_color[animationStep].GetComponent<Renderer>().material.SetColor("_TintColor", color);
				color = ui_playerName[animationStep].color;
				color.a = num2 * 2f;
				ui_playerName[animationStep].color = color;
				if (num2 >= 0.5f)
				{
					animationTime = 0f;
					animationStep++;
				}
			}
			else
			{
				animationTime = 0f;
				state = State.showCup;
			}
			animationTime += Time.deltaTime;
			playerInput();
			break;
		}
		case State.showCup:
		{
			float a2 = Mathf.Lerp(0f, 0.5f, animationTime * 2.5f);
			for (int m = 0; m < player.Length; m++)
			{
				for (int n = 0; n < 5; n++)
				{
					cupPlayer[m][n].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a2));
					if ((double)cupPlayer[m][n].GetComponent<Renderer>().material.GetColor("_TintColor").a >= 0.49)
					{
						animationTime = 0f;
						animationStep = 0;
						state = State.getCup;
						n = 5;
						break;
					}
				}
			}
			animationTime += Time.deltaTime;
			playerInput();
			break;
		}
		case State.getCup:
			if (animationStep == 0)
			{
				for (int j = 0; j < player.Length; j++)
				{
					if (idWins == player[j].id)
					{
						Vector3 localPosition = new Vector3(0.02f - (float)player[j].win * 0.15f, 0.47f - (float)j * 0.148f, 1f);
						cup_effect.localPosition = localPosition;
						cup_effect.GetComponent<Renderer>().enabled = true;
						cupPlayer[j][player[j].win].GetComponent<Renderer>().enabled = true;
						indexup = j;
						positionup = ui_player[j].transform.localPosition;
						player[j].win++;
						break;
					}
				}
				for (int k = 0; k < indexup; k++)
				{
					if (player[k].win < player[indexup].win)
					{
						indexdown = k;
						positiondown = ui_player[k].transform.localPosition;
						break;
					}
				}
				cup_effect.localScale = new Vector3(0.114f, 0.13f, 0f);
				animationStep = 1;
			}
			else
			{
				if (cup_effect.localScale.x <= 0.199f)
				{
					float x = Mathf.Lerp(0.114f, 0.2f, animationTime * 2.9f);
					float y = Mathf.Lerp(0.13f, 0.2f, animationTime * 2.9f);
					float a = Mathf.Lerp(1f, 0f, animationTime * 2.9f);
					cup_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
					cup_effect.localScale = new Vector3(x, y, 0f);
				}
				else
				{
					animationTime = 0f;
					animationStep = 0;
					if (indexup > indexdown)
					{
						state = State.swapPlace;
					}
					else
					{
						state = State.idle;
					}
				}
				animationTime += Time.deltaTime;
			}
			playerInput();
			break;
		case State.swapPlace:
		{
			Vector3 vector = positiondown;
			Vector3 vector2 = positionup;
			if (animationStep == 0)
			{
				if ((double)ui_player[indexup].localPosition.y <= (double)vector.y - 0.01)
				{
					float x2 = vector.x;
					float y2 = vector2.y + animationTime;
					float z = vector.z + 1f;
					ui_player[indexup].localPosition = new Vector3(x2, y2, z);
				}
				else
				{
					ui_player[indexup].localPosition = new Vector3(vector.x, vector.y, vector.z);
					animationTime = 0f;
					animationStep = 1;
				}
			}
			else if ((double)ui_player[indexup - 1].localPosition.y >= (double)vector2.y - 0.01)
			{
				for (int l = 0; l < indexup - indexdown; l++)
				{
					float x3 = vector.x;
					float y3 = vector.y - Mathf.Lerp(0f, differencey, animationTime * 4.5f) - differencey * (float)l;
					float z2 = vector.z;
					ui_player[indexdown + l].localPosition = new Vector3(x3, y3, z2);
				}
			}
			else
			{
				animationTime = 0f;
				animationStep = 1;
				state = State.idle;
			}
			animationTime += Time.deltaTime;
			playerInput();
			break;
		}
		case State.idle:
			playerInput();
			break;
		case State.nextScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				if (idWins == -1)
				{
					Loading.LoadScene("Adventure.GameMode");
				}
				else if (Global.cupAmount[idWins] == total_wins)
				{
					Application.LoadLevel("LocalBattle.Result");
				}
				else
				{
					Loading.LoadScene("Adventure.GameMode");
				}
			}
			break;
		}
		PropLineRepeatEff();
		pointerControl();
	}

	private void PropLineRunEff()
	{
		if (propline1_1.localScale.x < 0.83f)
		{
			float x = Mathf.Lerp(0f, 0.832f, propline1_time * 1.6f);
			float x2 = Mathf.Lerp(0f, -0.832f, propline1_time * 1.6f);
			Vector3 localPosition = propline1_1.localPosition;
			Vector3 localPosition2 = propline1_2.localPosition;
			propline1_1.localScale = new Vector3(x, 0.0076f, 0f);
			propline1_1.localPosition = new Vector3(-0.39f + propline1_1.localScale.x / 2f, localPosition.y, localPosition.z);
			propline1_2.localScale = new Vector3(x2, 0.0028f, 0f);
			propline1_2.localPosition = new Vector3(0.39f + propline1_2.localScale.x / 2f, localPosition2.y, localPosition2.z);
			if (propline1_1.localScale.x == 0.83f)
			{
				propline1_time = 0f;
			}
		}
		else if (goalCupAnim < total_wins)
		{
			float a = Mathf.Lerp(0f, 0.5f, propline1_time * 8.5f);
			if ((double)cup[goalCupAnim].GetComponent<Renderer>().material.GetColor("_TintColor").a < 0.5)
			{
				cup[goalCupAnim].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
			}
			else
			{
				cup[goalCupAnim].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
				goalCupAnim++;
				propline1_time = 0f;
			}
		}
		if ((double)ui_goal.localPosition.x > 0.673)
		{
			float x3 = Mathf.Lerp(0.77f, 0.67f, propline1_time * 1.9f);
			float a2 = Mathf.Lerp(0f, 0.65f, propline1_time * 1.6f);
			Vector3 localPosition3 = ui_goal.localPosition;
			ui_goal.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a2));
			ui_goal.localPosition = new Vector3(x3, localPosition3.y, localPosition3.z);
		}
		if (cup[total_wins - 1].GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
		{
			state = State.showPlayer;
		}
		else
		{
			propline1_time += Time.deltaTime;
		}
	}

	private void PropLineRepeatEff()
	{
		if (propline3_step == 0)
		{
			float y = 0.14f + propline3_movey * Mathf.Sin(propline3_time * (float)Math.PI / propline3_duration_time);
			float y2 = -0.2f - propline3_movey * Mathf.Sin(propline3_time * (float)Math.PI / propline3_duration_time);
			propline3_1.localPosition = new Vector3(-0.92f, y, 2f);
			propline3_2.localPosition = new Vector3(-0.92f, y2, 2f);
			propline3_1_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propline3_2_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
		}
		else
		{
			float x = Mathf.Lerp(0.08f, 0.12f, propline3_time * 1.9f);
			float y3 = Mathf.Lerp(0.215f, 0.25f, propline3_time * 1.9f);
			float x2 = Mathf.Lerp(-0.08f, -0.12f, propline3_time * 1.9f);
			propline3_1_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f - propline3_time * 0.8f));
			propline3_2_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f - propline3_time * 0.8f));
			propline3_1_effect.localScale = new Vector3(x, y3, 0f);
			propline3_2_effect.localScale = new Vector3(x2, y3, 0f);
			float x3 = Mathf.Lerp(0.08f, 0.12f, propline3_time * 1.9f);
			float y4 = Mathf.Lerp(0.65f, 0.7f, propline3_time * 1.9f);
			propline2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f - propline3_time * 0.8f));
			propline2.localScale = new Vector3(x3, y4, 0f);
		}
		if (propline3_step == 1 && propline2.GetComponent<Renderer>().material.GetColor("_TintColor").a <= 0.02f)
		{
			propline3_time = 0f;
			propline3_step = 0;
		}
		if (propline3_step == 0 && propline3_time >= propline3_duration_time)
		{
			propline3_time = 0f;
			if (propline3_step != 1)
			{
				propline3_step = 1;
			}
		}
		else
		{
			propline3_time += Time.deltaTime;
		}
	}

	private void sortWins()
	{
		for (int i = 0; i < this.player.Length; i++)
		{
			for (int j = i; j < this.player.Length; j++)
			{
				if (this.player[i].win < this.player[j].win)
				{
					Player player = new Player();
					player = this.player[i];
					this.player[i] = this.player[j];
					this.player[j] = player;
				}
			}
		}
	}

	private void playerInput()
	{
		RaycastHit[] array = new RaycastHit[5];
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (!controller.IsConnected())
			{
				continue;
			}
			Vector3 zero = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
			int num = 0;
			zero = pointer[i].transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out array[i]) && i == num))
			{
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (togButton[i] != null && !array[i].transform.parent.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					idleButton(togButton[i]);
				}
				if (array[i].transform.name == "btn_next")
				{
					TextureUtility.SetSpriteIndex(nextBtn, 3, 1, 1);
					togButton[i] = nextBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(nextBtn, 3, 1, 2);
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						state = State.nextScene;
						break;
					}
				}
			}
			else if (togButton[i] != null)
			{
				controller.DoPadRelease(i);
				mouseOver[i] = false;
				idleButton(togButton[i]);
			}
		}
	}

	private void idleButton(GameObject togButton)
	{
		if (togButton.name.Contains("next") || togButton.name.Contains("back"))
		{
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		togButton = null;
	}

	private void checkController()
	{
	}

	private void pointerControl()
	{
		for (int i = 0; i < 5; i++)
		{
			if (pointerInit[i] == 1)
			{
				pTime[i] += 0.02f;
			}
			GameController controller = GameInput.GetController(i);
			if (controller.IsMouseMove() && !Cursor.visible)
			{
				Cursor.visible = true;
			}
			StickPointer(controller, i);
		}
	}

	private void StickPointer(GameController ctrl, int p)
	{
		if (pTime[p] >= 10f)
		{
			pointerInit[p] = 0;
			pTime[p] = 0f;
			pointer[p].transform.localPosition = new Vector3(-1000f, 0f, -100f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = pointer[p].transform.localPosition;
			if (i == 1)
			{
				float num = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit[p] == 0 && num != 0f && localPosition.x < -181f)
				{
					pointer[p].transform.localPosition = new Vector3(-29f, 13f, -100f);
					pointerInit[p] = 1;
					break;
				}
				if (num < 0f)
				{
					if (localPosition.x >= -181f)
					{
						pointer[p].transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
						pTime[p] = 0f;
					}
				}
				else if (num > 0f && localPosition.x <= 195f)
				{
					pointer[p].transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
					pTime[p] = 0f;
				}
				continue;
			}
			float num2 = ctrl.GetVertical() * 1.25f;
			if (pointerInit[p] == 0 && num2 != 0f && localPosition.x < -181f)
			{
				pointer[p].transform.localPosition = new Vector3(-29f, 13f, -100f);
				pointerInit[p] = 1;
				break;
			}
			if (num2 < 0f)
			{
				if (localPosition.y <= 98f)
				{
					pointer[p].transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
					pTime[p] = 0f;
				}
			}
			else if (num2 > 0f && localPosition.y >= -115f)
			{
				pointer[p].transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
				pTime[p] = 0f;
			}
		}
	}
}
