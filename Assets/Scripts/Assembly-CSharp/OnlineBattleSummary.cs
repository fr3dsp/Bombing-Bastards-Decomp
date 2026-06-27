using System;
using TNet;
using UnityEngine;

public class OnlineBattleSummary : MonoBehaviour
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

	private GameObject pointer;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private int pointerInit;

	private bool isPress;

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

	private bool readyToNext;

	public int idWins;

	public float animationDurationTime;

	private bool mouseOver;

	private Player[] player;

	private Transform[][] cupPlayer;

	private Transform[] ui_player;

	private Transform[] ui_color;

	private GameObject[] ui_icon;

	private Material[] ui_iconMat;

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

	private GameObject togButton;

	private float fadeTime;

	private State state;

	private int[] readiness = new int[8];

	private float checkSend;

	private bool checkReady = true;

	private float autoGo;

	private GameObject dcPopSystem;

	private float dcPopFadeValue;

	private int dcPopFadeSign;

	private Material dcPopFade;

	private Transform dcPopGUI;

	private GameObject dcPopBtn;

	private float confiFadeValue;

	private int confiFadeSign;

	private Transform confiGUI;

	private GameObject[] confiBtn;

	private bool confiLeave;

	private TextMesh countDownTxt;

	private TextMesh countDownNum;

	private TextMesh plsWaitTxt;

	private float plsWaitOther;

	private void Awake()
	{
		base.gameObject.AddComponent<TNObject>().uid = 11000u;
		dcPopFadeValue = 0f;
		dcPopFadeSign = 0;
		dcPopSystem = base.transform.Find("PauseGUI").gameObject;
		dcPopFade = base.transform.Find("PauseGUI/bg").GetComponent<Renderer>().material;
		dcPopGUI = base.transform.Find("PauseGUI/dialog");
		pointer = GameObject.Find("p_pointer").gameObject;
		dcPopGUI.localScale = Vector3.zero;
		dcPopBtn = dcPopGUI.Find("btn_okDC").gameObject;
		dcPopSystem.SetActive(false);
		confiFadeValue = 0f;
		confiFadeSign = 0;
		confiGUI = base.transform.Find("PauseGUI/confirm");
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("ui/btn_ok").gameObject;
		confiBtn[1] = confiGUI.Find("ui/btn_cancel").gameObject;
		countDownTxt = base.transform.Find("count_down_txt").GetComponent<TextMesh>();
		countDownNum = base.transform.Find("count_down_num").GetComponent<TextMesh>();
		plsWaitTxt = base.transform.Find("pls_wait").GetComponent<TextMesh>();
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
		for (int j = 0; j < 8; j++)
		{
			if (Global.onlinePlayerIDs[j] != -1)
			{
				playerAmount++;
			}
		}
		player = new Player[playerAmount];
		playerAmount = 0;
		for (int k = 0; k < 8; k++)
		{
			if (Global.onlinePlayerIDs[k] != -1)
			{
				player[playerAmount] = new Player();
				player[playerAmount].id = k;
				player[playerAmount].name = Global.onlinePlayerNames[k];
				player[playerAmount].win = Global.cupAmount[k];
				if (k == idWins)
				{
					Global.cupAmount[k]++;
				}
				playerAmount++;
			}
		}
		total_wins = Global.winAmount;
		sortWins();
		ui_player = new Transform[8];
		ui_color = new Transform[8];
		ui_icon = new GameObject[8];
		ui_iconMat = new Material[8];
		ui_playerName = new TextMesh[8];
		cupPlayer = new Transform[8][];
		cup_effect = base.transform.Find("cup_effect");
		for (int l = 0; l < 8; l++)
		{
			ui_player[l] = base.transform.Find("ui_player_list1_" + (l + 1));
			ui_color[l] = ui_player[l].Find("ui_color");
			ui_icon[l] = ui_color[l].Find("ui_icon").gameObject;
			ui_iconMat[l] = ui_icon[l].GetComponent<Renderer>().material;
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
		base.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		state = State.initial;
	}

	private void ValidateFadeColor()
	{
		dcPopFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Max(Mathf.Max(confiFadeValue / 4f, dcPopFadeValue / 4f), plsWaitOther / 4f)));
	}

	private void Update()
	{
		if (checkReady)
		{
			if (!TNManager.isConnected)
			{
				GameObject gameObject = GameObject.Find("Online.ServerList");
				if (gameObject != null)
				{
					gameObject.GetComponent<OnlineBattleServerList>().PopUpMessage("You have been disconnected\nfrom the server.");
				}
				Application.LoadLevel("OnlineBattle.ServerList");
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (Global.onlinePlayerIDs[i] != -1)
				{
					num++;
					if (readiness[i] > 0 || !Global.onlinePlayerOns[i])
					{
						num2++;
					}
				}
			}
			if (num > num2)
			{
				if (TNManager.isHosting)
				{
					if (checkSend >= 0.2f)
					{
						checkSend = 0f;
					}
					if (checkSend == 0f)
					{
						GetComponent<TNObject>().Send(130, Target.Others, Global.onlineMatchID);
						OnlineBattleSummaryCheck(Global.onlineMatchID);
					}
					checkSend += Time.deltaTime;
				}
				return;
			}
			checkReady = false;
		}
		Color color = new Color(0.216f, 0.494f, 0.584f, 0.5f);
		for (int j = 0; j < 8; j++)
		{
			if (Global.onlinePlayerIDs[j] == -1 || Global.onlinePlayerOns[j])
			{
				continue;
			}
			for (int k = 0; k < playerAmount; k++)
			{
				if (player[k].id == j)
				{
					if (!ui_icon[k].activeSelf)
					{
						ui_icon[k].SetActive(true);
					}
					if (state > State.showPlayer || state == State.idle)
					{
						ui_playerName[k].color = color;
					}
					ui_iconMat[k].SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
					break;
				}
			}
		}
		if (state <= State.showPlayer && state != State.idle)
		{
			float num3 = Mathf.Lerp(0f, 0.5f, animationTime * 16.5f) * 2f;
			for (int l = 0; l < 8; l++)
			{
				if (ui_icon[l].activeSelf)
				{
					float a = ui_color[l].GetComponent<Renderer>().material.GetColor("_TintColor").a;
					ui_iconMat[l].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a * 2f * Mathf.PingPong(1.5f * Time.time, 0.75f)));
				}
			}
		}
		else
		{
			Color color2 = new Color(0.5f, 0.5f, 0.5f, Mathf.PingPong(1.5f * Time.time, 0.75f));
			for (int m = 0; m < 8; m++)
			{
				if (ui_icon[m].activeSelf)
				{
					ui_iconMat[m].SetColor("_TintColor", color2);
				}
			}
		}
		Material material = base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material;
		if (!TNManager.isConnected && material.GetColor("_TintColor").a == 0f && !dcPopGUI.gameObject.activeSelf)
		{
			dcPopGUI.gameObject.SetActive(true);
			dcPopFadeSign = 1;
			dcPopSystem.SetActive(true);
			nextBtn.GetComponent<Collider>().enabled = false;
		}
		GameController controller = GameInput.GetController(Global.onlinePlayerSlot);
		if (controller.DoSelectMenu() && dcPopFadeSign == 0)
		{
			if (confiFadeValue == 1f || confiFadeSign != 0)
			{
				confiFadeSign = -1;
			}
			else if (dcPopFadeValue == 0f)
			{
				dcPopSystem.SetActive(true);
				confiFadeSign = 1;
				confiGUI.gameObject.SetActive(true);
				nextBtn.GetComponent<Collider>().enabled = false;
			}
			else if (dcPopFadeValue == 1f && TNManager.isConnected)
			{
				dcPopFadeSign = -1;
			}
		}
		if (confiFadeSign != 0)
		{
			if (confiFadeSign > 0)
			{
				confiFadeValue += 5f * Time.deltaTime;
				if (confiFadeValue >= 1f)
				{
					confiFadeValue = 1f;
					confiFadeSign = 0;
				}
			}
			else
			{
				confiFadeValue -= 5f * Time.deltaTime;
				if (confiFadeValue <= 0f)
				{
					confiFadeValue = 0f;
					confiFadeSign = 0;
					confiGUI.gameObject.SetActive(false);
					nextBtn.GetComponent<Collider>().enabled = true;
				}
			}
			confiGUI.localScale = new Vector3(confiFadeValue, confiFadeValue, confiFadeValue);
			ValidateFadeColor();
		}
		if (dcPopFadeSign != 0)
		{
			if (dcPopFadeSign > 0)
			{
				dcPopFadeValue += 5f * Time.deltaTime;
				if (dcPopFadeValue >= 1f)
				{
					dcPopFadeValue = 1f;
					dcPopFadeSign = 0;
				}
			}
			dcPopGUI.localScale = new Vector3(dcPopFadeValue, dcPopFadeValue, dcPopFadeValue);
			ValidateFadeColor();
		}
		float num4 = 2.5f;
		switch (state)
		{
		case State.initial:
		{
			Transform obj = propline1_1;
			Vector3 localScale = new Vector3(0f, 0.0076f, 0f);
			propline1_2.localScale = localScale;
			obj.localScale = localScale;
			Color color6 = new Color(0.5f, 0.5f, 0.5f, 0f);
			for (int num10 = 0; num10 < 5; num10++)
			{
				cup[num10].GetComponent<Renderer>().material.SetColor("_TintColor", color6);
			}
			ui_goal.GetComponent<Renderer>().material.SetColor("_TintColor", color6);
			ui_goal.localPosition = new Vector3(0.77f, 0.653f, 2f);
			goalCupAnim = 0;
			state = State.fadeIn;
			break;
		}
		case State.fadeIn:
		{
			Color color4 = new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * num4));
			material.SetColor("_TintColor", color4);
			if (color4.a == 0f)
			{
				fadeTime = 0f;
				checkController();
				state = State.showGoal;
			}
			break;
		}
		case State.showGoal:
			PropLineRunEff();
			break;
		case State.showPlayer:
		{
			float num7 = Mathf.Lerp(0f, 0.5f, animationTime * 16.5f);
			if (animationStep < player.Length)
			{
				Color color5 = new Color(0.5f, 0.5f, 0.5f, num7);
				ui_player[animationStep].GetComponent<Renderer>().material.SetColor("_TintColor", color5);
				ui_color[animationStep].GetComponent<Renderer>().material.SetColor("_TintColor", color5);
				int id = player[animationStep].id;
				if (Global.onlinePlayerIDs[id] != -1 && !Global.onlinePlayerOns[id])
				{
					color5 = color;
					color5.a = num7;
				}
				else
				{
					color5 = ui_playerName[animationStep].color;
					color5.a = num7 * 2f;
				}
				ui_playerName[animationStep].color = color5;
				if (num7 >= 0.5f)
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
			if (dcPopFadeSign == 0)
			{
				playerInput();
				pointerControl();
			}
			break;
		}
		case State.showCup:
		{
			float a3 = Mathf.Lerp(0f, 0.5f, animationTime * 2.5f);
			Color color7 = new Color(0.5f, 0.5f, 0.5f, a3);
			for (int num11 = 0; num11 < player.Length; num11++)
			{
				for (int num12 = 0; num12 < 5; num12++)
				{
					cupPlayer[num11][num12].GetComponent<Renderer>().material.SetColor("_TintColor", color7);
					if ((double)color7.a >= 0.49)
					{
						animationTime = 0f;
						animationStep = 0;
						state = State.getCup;
						num12 = 5;
						break;
					}
				}
			}
			animationTime += Time.deltaTime;
			if (dcPopFadeSign == 0)
			{
				playerInput();
				pointerControl();
			}
			break;
		}
		case State.getCup:
			if (animationStep == 0)
			{
				for (int num8 = 0; num8 < player.Length; num8++)
				{
					if (idWins == player[num8].id)
					{
						Vector3 localPosition = new Vector3(0.02f - (float)player[num8].win * 0.15f, 0.47f - (float)num8 * 0.148f, 1f);
						cup_effect.localPosition = localPosition;
						cup_effect.GetComponent<Renderer>().enabled = true;
						cupPlayer[num8][player[num8].win].GetComponent<Renderer>().enabled = true;
						indexup = num8;
						positionup = ui_player[num8].transform.localPosition;
						player[num8].win++;
						break;
					}
				}
				for (int num9 = 0; num9 < indexup; num9++)
				{
					if (player[num9].win < player[indexup].win)
					{
						indexdown = num9;
						positiondown = ui_player[num9].transform.localPosition;
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
					float x3 = Mathf.Lerp(0.114f, 0.2f, animationTime * 2.9f);
					float y3 = Mathf.Lerp(0.13f, 0.2f, animationTime * 2.9f);
					float a2 = Mathf.Lerp(1f, 0f, animationTime * 2.9f);
					cup_effect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a2));
					cup_effect.localScale = new Vector3(x3, y3, 0f);
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
			if (dcPopFadeSign == 0)
			{
				playerInput();
				pointerControl();
			}
			break;
		case State.idle:
		case State.swapPlace:
		{
			if (state == State.swapPlace)
			{
				Vector3 vector = positiondown;
				Vector3 vector2 = positionup;
				if (animationStep == 0)
				{
					if ((double)ui_player[indexup].localPosition.y <= (double)vector.y - 0.01)
					{
						float x = vector.x;
						float y = vector2.y + animationTime;
						float z = vector.z + 1f;
						ui_player[indexup].localPosition = new Vector3(x, y, z);
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
					for (int n = 0; n < indexup - indexdown; n++)
					{
						float x2 = vector.x;
						float y2 = vector.y - Mathf.Lerp(0f, differencey, animationTime * 4.5f) - differencey * (float)n;
						float z2 = vector.z;
						ui_player[indexdown + n].localPosition = new Vector3(x2, y2, z2);
					}
				}
				else
				{
					animationTime = 0f;
					animationStep = 1;
					state = State.idle;
				}
				animationTime += Time.deltaTime;
			}
			if (dcPopFadeSign == 0)
			{
				playerInput();
				pointerControl();
			}
			if (!TNManager.isConnected || !(autoGo < 30f))
			{
				break;
			}
			autoGo += Time.deltaTime;
			if (autoGo >= 30f)
			{
				autoGo = 30f;
				fadeTime = 0f;
				readyToNext = true;
				GetComponent<TNObject>().Send(132, Target.Others, Global.onlineMatchID, Global.onlinePlayerSlot);
				readiness[Global.onlinePlayerSlot] = 2;
				for (int num5 = 0; num5 < playerAmount; num5++)
				{
					if (player[num5].id == Global.onlinePlayerSlot)
					{
						ui_icon[num5].SetActive(true);
						break;
					}
				}
				idleButton(nextBtn);
				dcPopSystem.SetActive(true);
				nextBtn.GetComponent<Collider>().enabled = false;
			}
			float num6 = 2f * (autoGo - (float)(int)autoGo);
			countDownNum.characterSize = ((!(autoGo < 21f)) ? (2.25f + num6) : 2.25f);
			countDownNum.text = Mathf.CeilToInt(30f - autoGo).ToString();
			if (num6 > 1f)
			{
				num6 = 2f - num6;
			}
			Color color3 = new Color(0.8f, 1f, 1f, num6);
			countDownTxt.color = color3;
			countDownNum.color = color3;
			break;
		}
		}
		if (readyToNext)
		{
			Color color8 = countDownTxt.color;
			if (color8.a > 0f)
			{
				color8.a -= 2f * Time.deltaTime;
				countDownTxt.color = color8;
				countDownNum.color = color8;
			}
			bool flag = true;
			for (int num13 = 0; num13 < 8; num13++)
			{
				if (Global.onlinePlayerIDs[num13] != -1 && Global.onlinePlayerOns[num13] && readiness[num13] < 2)
				{
					flag = false;
					break;
				}
			}
			bool isConnected = TNManager.isConnected;
			if (flag || !isConnected || confiLeave)
			{
				Color color9 = new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num4));
				material.SetColor("_TintColor", color9);
				if (color9.a == 0.5f)
				{
					if (isConnected && !confiLeave)
					{
						int num14 = 0;
						for (int num15 = 0; num15 < 8; num15++)
						{
							if (Global.onlinePlayerIDs[num15] != -1 && Global.onlinePlayerOns[num15])
							{
								num14++;
							}
						}
						if ((idWins != -1 && Global.cupAmount[idWins] == total_wins) || num14 == 1)
						{
							Application.LoadLevel("OnlineBattle.Result");
						}
						else
						{
							Global.onlineMatchID++;
							for (int num16 = 0; num16 < 8; num16++)
							{
								if (Global.onlinePlayerIDs[num16] != -1 && !Global.onlinePlayerOns[num16])
								{
									Global.onlinePlayerIDs[num16] = -1;
								}
							}
							Loading.LoadScene("Adventure.GameMode");
						}
					}
					else
					{
						if (!isConnected && dcPopFadeValue == 0f)
						{
							GameObject gameObject2 = GameObject.Find("Online.ServerList");
							if (gameObject2 != null)
							{
								gameObject2.GetComponent<OnlineBattleServerList>().PopUpMessage("You have been disconnected\nfrom the server.");
							}
						}
						Application.LoadLevel("OnlineBattle.ServerList");
					}
				}
			}
			else
			{
				if (readiness[Global.onlinePlayerSlot] == 2)
				{
					if (plsWaitOther < 1f)
					{
						plsWaitOther += 2f * Time.deltaTime;
						if (plsWaitOther > 1f)
						{
							plsWaitOther = 1f;
						}
						ValidateFadeColor();
					}
					float num17 = 2f * (Time.time - (float)(int)Time.time);
					plsWaitTxt.color = new Color(1f, 1f, 1f, plsWaitOther * ((!(num17 < 1f)) ? (2f - num17) : num17));
					num17 *= 2f;
					string text = "Please wait";
					for (int num18 = (int)num17; num18 > 0; num18--)
					{
						text += ".";
					}
					plsWaitTxt.text = text;
				}
				if (dcPopFadeSign == 0)
				{
					playerInput();
					pointerControl();
				}
			}
		}
		PropLineRepeatEff();
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
		RaycastHit hitInfo = default(RaycastHit);
		GameController controller = GameInput.GetController(Global.onlinePlayerSlot);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3 position = pointer.transform.GetChild(0).position;
		if ((Physics.Raycast(ray, out hitInfo) && pointerInit == 0) || Physics.Raycast(position, Vector3.forward, out hitInfo))
		{
			if (!mouseOver)
			{
				mouseOver = true;
				GameSound.StartSFX("mouseOver");
			}
			if (togButton != null && hitInfo.transform != null && !hitInfo.transform.parent.name.Contains(togButton.name))
			{
				idleButton(togButton);
			}
			if (!(hitInfo.transform != null) || !hitInfo.transform.name.StartsWith("btn_"))
			{
				return;
			}
			string text = hitInfo.transform.name;
			switch (text)
			{
			case "btn_next":
				togButton = nextBtn;
				break;
			case "btn_okDC":
				togButton = dcPopBtn;
				break;
			case "btn_ok":
				togButton = confiBtn[0];
				break;
			case "btn_cancel":
				togButton = confiBtn[1];
				break;
			}
			if (!isPress)
			{
				TextureUtility.SetSpriteIndex(togButton, (!(togButton == nextBtn)) ? 4 : 3, 1, 1);
			}
			if (controller.DoEnterHold("Fire1"))
			{
				controller.DoPadPress(Global.onlinePlayerSlot);
				TextureUtility.SetSpriteIndex(togButton, (!(togButton == nextBtn)) ? 4 : 3, 1, 2);
				isPress = true;
				if (text == "btn_ok" || text == "btn_cancel")
				{
					Transform child = togButton.transform.GetChild(0);
					Vector3 localPosition = child.localPosition;
					if (localPosition.x == 0f && localPosition.y == 0f)
					{
						localPosition.x = -0.03f;
						localPosition.y = -0.06f;
						child.localPosition = localPosition;
					}
				}
			}
			else
			{
				if (!controller.DoEnterRelease("Fire1"))
				{
					return;
				}
				controller.DoPadRelease(Global.onlinePlayerSlot);
				GameSound.StartSFX("menuSelect");
				isPress = false;
				if (text == "btn_ok" || text == "btn_cancel")
				{
					Transform child2 = togButton.transform.GetChild(0);
					Vector3 localPosition2 = child2.localPosition;
					if (localPosition2.x != 0f || localPosition2.y != 0f)
					{
						localPosition2.x = 0f;
						localPosition2.y = 0f;
						child2.localPosition = localPosition2;
					}
				}
				togButton = null;
				if (text == "btn_cancel")
				{
					confiFadeSign = -1;
					return;
				}
				fadeTime = 0f;
				readyToNext = true;
				if (text == "btn_next")
				{
					dcPopSystem.SetActive(true);
					nextBtn.GetComponent<Collider>().enabled = false;
					if (!TNManager.isConnected)
					{
						return;
					}
					GetComponent<TNObject>().Send(132, Target.Others, Global.onlineMatchID, Global.onlinePlayerSlot);
					readiness[Global.onlinePlayerSlot] = 2;
					for (int i = 0; i < playerAmount; i++)
					{
						if (player[i].id == Global.onlinePlayerSlot)
						{
							ui_icon[i].SetActive(true);
							break;
						}
					}
				}
				else if (text == "btn_ok")
				{
					confiLeave = true;
				}
			}
		}
		else if (togButton != null)
		{
			mouseOver = false;
			idleButton(togButton);
		}
	}

	private void idleButton(GameObject togButton)
	{
		if (togButton.name.Contains("next") || togButton.name.Contains("back"))
		{
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		else
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		togButton = null;
	}

	private void checkController()
	{
	}

	private void pointerControl()
	{
		if (pointerInit == 1)
		{
			pTime += Time.deltaTime;
		}
		GameController controller = GameInput.GetController(Global.onlinePlayerSlot);
		if (controller.IsMouseMove())
		{
			pTime = 100f;
			Cursor.visible = true;
		}
		StickPointer(controller, Global.onlinePlayerSlot);
	}

	private void StickPointer(GameController ctrl, int p)
	{
		if (pTime >= 10f)
		{
			pointerInit = 0;
			pTime = 0f;
			pointer.transform.localPosition = new Vector3(-500f, 0f, -100f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = pointer.transform.localPosition;
			if (i == 1)
			{
				float num = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit == 0 && num != 0f && localPosition.x < -181f)
				{
					pointer.transform.localPosition = new Vector3(0f, 0f, -100f);
					pointerInit = 1;
					Cursor.visible = false;
					break;
				}
				if (num < 0f)
				{
					if (localPosition.x >= -181f)
					{
						pointer.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
						pTime = 0f;
					}
				}
				else if (num > 0f && localPosition.x <= 195f)
				{
					pointer.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
					pTime = 0f;
				}
				continue;
			}
			float num2 = ctrl.GetVertical() * 1.25f;
			if (pointerInit == 0 && num2 != 0f && localPosition.x < -181f)
			{
				pointer.transform.localPosition = new Vector3(0f, 0f, -100f);
				pointerInit = 1;
				Cursor.visible = false;
				break;
			}
			if (num2 < 0f)
			{
				if (localPosition.y <= 98f)
				{
					pointer.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
					pTime = 0f;
				}
			}
			else if (num2 > 0f && localPosition.y >= -115f)
			{
				pointer.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
				pTime = 0f;
			}
		}
	}

	[RFC(130)]
	private void OnlineBattleSummaryCheck(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (readiness[Global.onlinePlayerSlot] == 0)
			{
				readiness[Global.onlinePlayerSlot] = 1;
			}
			GetComponent<TNObject>().Send(131, Target.Others, Global.onlineMatchID, TNManager.playerID);
		}
	}

	[RFC(131)]
	private void OnlineBattleSummaryConfirm(int matchID, int playerID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (Global.onlinePlayerIDs[i] == playerID)
			{
				if (readiness[i] == 0)
				{
					readiness[i] = 1;
				}
				break;
			}
		}
	}

	[RFC(132)]
	private void OnlineBattleSummaryReady(int matchID, int slot)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		readiness[slot] = 2;
		for (int i = 0; i < playerAmount; i++)
		{
			if (player[i].id == slot)
			{
				ui_icon[i].SetActive(true);
				break;
			}
		}
	}
}
