using TNet;
using UnityEngine;

public class OnlineBattleLobbyRoom : MonoBehaviour
{
	public GUISkin skin;

	private int maxPlayer = 8;

	private bool[] matchMaps = new bool[5] { true, true, true, true, true };

	private int matchWin = 3;

	private int matchTime = 3;

	private bool[] matchOpts = new bool[4] { true, true, true, true };

	private int[] playerIDs;

	private int[] pingValue;

	private bool[] readiness;

	private float countPing = 5f;

	private float countDown;

	private string hostSvName;

	private float hostAutoGo;

	private bool isFresh;

	private bool isWaitData;

	private float waitDataTime;

	private Material fadeMat;

	private float fadeAlpha;

	private int fadeSign;

	private Material busyMat;

	private float busyFade;

	private TextMesh busyText;

	private GameObject readyBG;

	private float readyBGCol;

	private bool goBack;

	private string goBackMsg;

	private bool mouseOver;

	private GameObject togButton;

	private GameObject pointer;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private int pointerInit;

	private bool isPress;

	private TextMesh svNameMesh;

	private string svNameWord;

	private string svNameWordBak;

	private bool svNameGUI;

	private bool svNameFocus;

	private GameObject svNamePen;

	private bool svNameNotice;

	private GameObject slotBtnUp;

	private GameObject slotBtnDown;

	private TextMesh slotBtnText;

	private GameObject[] matchMapTogs;

	private TextMesh matchTxtWin;

	private TextMesh matchTxtTime;

	private GameObject[] matchOptTogs;

	private GameObject[] playerSlots;

	private GameObject[] playerKicks;

	private GameObject[] playerColrs;

	private GameObject[] playerIcons;

	private TextMesh[] playerNames;

	private TextMesh[] playerPings;

	private GameObject playerReady;

	private GameObject playerCurr;

	private GameObject playerHost;

	private float confiFadeValue;

	private int confiFadeSign;

	private GameObject confiFadeBG;

	private Transform confiGUI;

	private GameObject[] confiBtn;

	private bool confiLeave;

	private Material cntDarkMat;

	private float cntDarkCol;

	private GameObject[] cnt54321Obj;

	private Material[] cnt54321Mat;

	private float[] cnt54321Col;

	private float[] cnt54321Val;

	private static int onlineMapCount;

	private static int[] onlineMapIDs;

	private void Awake()
	{
		Debug.Log("OnlineBattleLobbyRoom.Awake");
		playerIDs = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
		pingValue = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
		readiness = new bool[8];
		GameInput.DestroyRemoteSystem();
		Transform transform = base.transform.Find("GUI");
		Transform transform2 = transform.Find("panel");
		pointer = transform2.Find("p_pointer").gameObject;
		fadeMat = transform2.Find("fade_bg").GetComponent<Renderer>().material;
		busyMat = transform2.Find("PopupGUI/bg").GetComponent<Renderer>().material;
		busyText = transform2.Find("pls_wait").GetComponent<TextMesh>();
		svNameMesh = transform2.Find("field_server_name").GetComponent<TextMesh>();
		svNameWord = string.Empty;
		svNamePen = transform2.Find("ui_pen").gameObject;
		Transform transform3 = transform2.Find("slot");
		slotBtnUp = transform3.Find("btn_slot_up").gameObject;
		slotBtnDown = transform3.Find("btn_slot_down").gameObject;
		slotBtnText = transform3.Find("text").GetComponent<TextMesh>();
		if (TNManager.isHosting)
		{
			slotBtnUp.GetComponent<Collider>().enabled = false;
			ManageButtonSprite(slotBtnUp, 3);
			svNameNotice = true;
		}
		else
		{
			slotBtnUp.SetActive(false);
			slotBtnDown.SetActive(false);
			svNamePen.SetActive(false);
		}
		matchMapTogs = new GameObject[5];
		matchOptTogs = new GameObject[4];
		Transform transform4 = transform2.Find("maps");
		for (int i = 0; i < 5; i++)
		{
			matchMapTogs[i] = transform4.Find("tog_map" + i).gameObject;
		}
		matchTxtWin = transform2.Find("match_win/text").GetComponent<TextMesh>();
		matchTxtTime = transform2.Find("match_time/text").GetComponent<TextMesh>();
		Transform transform5 = transform2.Find("options");
		for (int j = 0; j < 4; j++)
		{
			matchOptTogs[j] = transform5.Find("tog_opt" + j).gameObject;
		}
		if (TNManager.isHosting)
		{
			hostSvName = OnlineBattleServerList.GetServerName(TNServerInstance.serverName);
		}
		else
		{
			svNameMesh.GetComponent<Collider>().enabled = false;
			Transform transform6 = transform2.Find("match_win");
			Transform transform7 = transform2.Find("match_time");
			GameObject gameObject = transform6.Find("btn_winL").gameObject;
			GameObject gameObject2 = transform6.Find("btn_winR").gameObject;
			GameObject gameObject3 = transform7.Find("btn_timeL").gameObject;
			GameObject gameObject4 = transform7.Find("btn_timeR").gameObject;
			gameObject.GetComponent<Collider>().enabled = false;
			gameObject2.GetComponent<Collider>().enabled = false;
			gameObject3.GetComponent<Collider>().enabled = false;
			gameObject4.GetComponent<Collider>().enabled = false;
			TextureUtility.SetSpriteIndex(gameObject, 4, 1, 3);
			TextureUtility.SetSpriteIndex(gameObject2, 4, 1, 3);
			TextureUtility.SetSpriteIndex(gameObject3, 4, 1, 3);
			TextureUtility.SetSpriteIndex(gameObject4, 4, 1, 3);
			for (int k = 0; k < 5; k++)
			{
				matchMapTogs[k].GetComponent<Collider>().enabled = false;
			}
			for (int l = 0; l < 4; l++)
			{
				matchOptTogs[l].GetComponent<Collider>().enabled = false;
			}
		}
		playerSlots = new GameObject[8];
		playerKicks = new GameObject[8];
		playerColrs = new GameObject[8];
		playerIcons = new GameObject[8];
		playerNames = new TextMesh[8];
		playerPings = new TextMesh[8];
		Transform transform8 = transform2.Find("players");
		for (int m = 0; m < 8; m++)
		{
			Transform transform9 = transform8.Find("player" + m);
			playerSlots[m] = transform9.Find("btn_player" + m).gameObject;
			playerKicks[m] = transform9.Find("btn_kick" + m).gameObject;
			playerColrs[m] = transform9.Find("color").gameObject;
			playerIcons[m] = transform9.Find("icon").gameObject;
			playerNames[m] = transform9.Find("name").GetComponent<TextMesh>();
			playerPings[m] = transform9.Find("ping").GetComponent<TextMesh>();
			playerColrs[m].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)m * 0.125f, 0f));
		}
		playerCurr = transform8.Find("cur_player").gameObject;
		playerHost = transform8.Find("host_icon").gameObject;
		playerReady = transform2.Find("tog_ready").gameObject;
		readyBG = playerReady.transform.GetChild(0).gameObject;
		UpdatePick();
		UpdateOption();
		UpdateReady();
		confiFadeValue = 0f;
		confiFadeSign = 0;
		confiFadeBG = transform2.Find("PopupGUI/bg").gameObject;
		confiGUI = transform2.Find("PopupGUI/confirm");
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("ui/btn_ok").gameObject;
		confiBtn[1] = confiGUI.Find("ui/btn_cancel").gameObject;
		Transform transform10 = transform2.Find("54321");
		cntDarkMat = transform10.Find("dark").GetComponent<Renderer>().material;
		cntDarkCol = 0f;
		cnt54321Obj = new GameObject[5];
		cnt54321Mat = new Material[5];
		cnt54321Col = new float[5];
		cnt54321Val = new float[5];
		for (int n = 0; n < 5; n++)
		{
			cnt54321Obj[n] = transform10.Find((n + 1).ToString()).gameObject;
			cnt54321Mat[n] = cnt54321Obj[n].GetComponent<Renderer>().material;
			cnt54321Col[n] = 0f;
			cnt54321Val[n] = 0f;
			cnt54321Mat[n].SetTextureOffset("_MainTex", new Vector2((float)(n % 3) / 3f, 0.5f - (float)(n / 3) / 2f));
		}
	}

	private void Start()
	{
		Debug.Log("OnlineBattleLobbyRoom.Start");
		GameObject gameObject = GameObject.Find("Online.ServerList");
		if (gameObject != null)
		{
			gameObject.GetComponent<OnlineBattleServerList>().Hide();
		}
		fadeAlpha = 1f;
		fadeSign = -1;
		isFresh = true;
		isWaitData = true;
		for (int i = 0; i < 8; i++)
		{
			if (Global.onlinePlayerIDs[i] != -1)
			{
				isFresh = false;
				break;
			}
		}
		Debug.Log("isFresh=" + isFresh);
		if (TNManager.isHosting)
		{
			isWaitData = false;
			if (isFresh)
			{
				PlayerJoin(TNManager.playerID);
			}
			else
			{
				for (int j = 0; j < 8; j++)
				{
					if (Global.onlinePlayerIDs[j] != -1 && Global.onlinePlayerOns[j])
					{
						playerIDs[j] = Global.onlinePlayerIDs[j];
						pingValue[j] = ((Global.onlinePlayerID != playerIDs[j]) ? (-1) : (-1000000));
						readiness[j] = false;
					}
				}
				GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
			}
			bool open;
			int numPlayer;
			hostSvName = OnlineBattleServerList.GetServerDetail(TNServerInstance.serverName, out open, ref matchMaps, out matchWin, out matchTime, ref matchOpts, out numPlayer, out maxPlayer);
			numPlayer = 0;
			for (int k = 0; k < 8; k++)
			{
				if (playerIDs[k] != -1)
				{
					numPlayer++;
				}
			}
			TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, true, matchMaps, matchWin, matchTime, matchOpts, numPlayer, maxPlayer);
			GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
			UpdateOption();
		}
		else
		{
			isWaitData = isFresh;
		}
		if (isWaitData)
		{
			busyFade = 0.5f;
			busyMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, busyFade / 2f));
			confiFadeBG.SetActive(true);
		}
	}

	private void playerInput()
	{
		bool flag = fadeAlpha > 0f;
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 position = pointer.transform.GetChild(0).position;
			RaycastHit hitInfo;
			if (!flag && ((Physics.Raycast(ray, out hitInfo) && pointerInit == 0) || Physics.Raycast(position, Vector3.forward, out hitInfo)))
			{
				if (hitInfo.transform.name == "bg")
				{
					if (togButton != null)
					{
						ManageButtonSprite(togButton, 0);
						togButton = null;
						mouseOver = false;
					}
				}
				else if (hitInfo.transform.name.StartsWith("field_"))
				{
					if (togButton != null)
					{
						ManageButtonSprite(togButton, 0);
						togButton = null;
						mouseOver = false;
					}
					if (controller.DoEnterPress("x"))
					{
						controller.DoPadPress(i);
						svNameMesh.gameObject.SetActive(false);
						svNameGUI = true;
						svNameFocus = true;
					}
				}
				else
				{
					if (!hitInfo.transform.name.StartsWith("btn_") && !hitInfo.transform.name.StartsWith("tog_"))
					{
						continue;
					}
					if (togButton != null && togButton != hitInfo.transform.gameObject)
					{
						mouseOver = false;
						ManageButtonSprite(togButton, 0);
					}
					togButton = hitInfo.transform.gameObject;
					if (!mouseOver)
					{
						mouseOver = true;
						if (!GameSound.IsPlayingSFX("mouseOver"))
						{
							GameSound.StartSFX("mouseOver");
						}
					}
					if (!isPress)
					{
						ManageButtonSprite(togButton, 1);
					}
					if (controller.DoEnterHold("x"))
					{
						controller.DoPadPress(i);
						ManageButtonSprite(togButton, 2);
						isPress = true;
					}
					else
					{
						if (!controller.DoEnterRelease("x"))
						{
							continue;
						}
						controller.DoPadRelease(i);
						GameSound.StartSFX("menuSelect");
						isPress = false;
						Debug.Log("Do action " + togButton.name);
						bool flag2 = false;
						if (togButton.name == "btn_back")
						{
							if (TNManager.isHosting)
							{
								confiFadeSign = 1;
								confiGUI.gameObject.SetActive(true);
							}
							else
							{
								goBack = true;
								goBackMsg = null;
								fadeSign = 1;
							}
						}
						else if (togButton.name == "btn_slot_up")
						{
							if (maxPlayer < 8)
							{
								maxPlayer++;
								TNManager.SetPlayerLimit(maxPlayer);
								flag2 = true;
								if (maxPlayer == 8)
								{
									slotBtnUp.GetComponent<Collider>().enabled = false;
									ManageButtonSprite(slotBtnUp, 3);
								}
								else if (maxPlayer == 3)
								{
									slotBtnDown.GetComponent<Collider>().enabled = true;
									ManageButtonSprite(slotBtnDown, 0);
								}
							}
						}
						else if (togButton.name == "btn_slot_down")
						{
							int num = 0;
							for (int j = 0; j < 8; j++)
							{
								if (playerIDs[j] != -1)
								{
									num++;
								}
							}
							if (maxPlayer > 2 && maxPlayer > num)
							{
								maxPlayer--;
								TNManager.SetPlayerLimit(maxPlayer);
								flag2 = true;
								if (maxPlayer == 2)
								{
									slotBtnDown.GetComponent<Collider>().enabled = false;
									ManageButtonSprite(slotBtnDown, 3);
								}
								else if (maxPlayer == 7)
								{
									slotBtnUp.GetComponent<Collider>().enabled = true;
									ManageButtonSprite(slotBtnUp, 0);
								}
							}
						}
						else if (togButton.name.StartsWith("tog_map"))
						{
							int num2 = togButton.name[7] - 48;
							if (matchMaps[num2])
							{
								int num3 = 0;
								for (int k = 0; k < 5; k++)
								{
									if (matchMaps[k])
									{
										num3++;
									}
								}
								if (num3 > 1)
								{
									matchMaps[num2] = false;
								}
							}
							else
							{
								matchMaps[num2] = true;
							}
							flag2 = true;
						}
						else if (togButton.name == "btn_winL")
						{
							matchWin--;
							if (matchWin < 1)
							{
								matchWin = 5;
							}
							flag2 = true;
						}
						else if (togButton.name == "btn_winR")
						{
							matchWin++;
							if (matchWin > 5)
							{
								matchWin = 1;
							}
							flag2 = true;
						}
						else if (togButton.name == "btn_timeL")
						{
							matchTime--;
							if (matchTime < 1)
							{
								matchTime = 5;
							}
							flag2 = true;
						}
						else if (togButton.name == "btn_timeR")
						{
							matchTime++;
							if (matchTime > 5)
							{
								matchTime = 1;
							}
							flag2 = true;
						}
						else if (togButton.name.StartsWith("tog_opt"))
						{
							int num4 = togButton.name[7] - 48;
							matchOpts[num4] = !matchOpts[num4];
							flag2 = true;
						}
						else if (togButton.name.StartsWith("btn_player"))
						{
							int num5 = togButton.name[10] - 48;
							if (playerIDs[num5] == -1)
							{
								GetComponent<TNObject>().Send(110, Target.Host, TNManager.playerID, num5, (!TNManager.isHosting) ? TNManager.ping : (-1000000));
								UpdatePick();
							}
						}
						else if (togButton.name.StartsWith("btn_kick"))
						{
							int num6 = togButton.name[8] - 48;
							GetComponent<TNObject>().Send(114, Target.Others, playerIDs[num6]);
						}
						else if (togButton.name == "tog_ready")
						{
							bool flag3 = false;
							for (int l = 0; l < 8; l++)
							{
								if (playerIDs[l] == Global.onlinePlayerID)
								{
									flag3 = readiness[l];
									break;
								}
							}
							GetComponent<TNObject>().Send(115, Target.All, TNManager.playerID, !flag3);
						}
						else if (togButton.name == "btn_ok")
						{
							goBack = true;
							goBackMsg = null;
							fadeSign = 1;
						}
						else if (togButton.name == "btn_cancel")
						{
							confiFadeSign = -1;
						}
						if (!flag2)
						{
							continue;
						}
						int num7 = 0;
						for (int m = 0; m < 8; m++)
						{
							if (playerIDs[m] != -1)
							{
								num7++;
							}
						}
						TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, true, matchMaps, matchWin, matchTime, matchOpts, num7, maxPlayer);
						GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
						UpdateOption();
					}
				}
			}
			else if (togButton != null)
			{
				ManageButtonSprite(togButton, 0);
				isPress = false;
				togButton = null;
				mouseOver = false;
			}
		}
	}

	private void ManageButtonSprite(GameObject togButton, int state)
	{
		if (!togButton.GetComponent<Collider>().enabled)
		{
			state = 3;
		}
		if (togButton.name.StartsWith("tog_"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 2, (!(togButton.transform.localScale.z > 0f)) ? (state + 4) : state);
		}
		else
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, state);
			if (togButton.name.StartsWith("btn_player"))
			{
				int num = -1;
				for (int i = 0; i < 8; i++)
				{
					if (playerSlots[i] == togButton)
					{
						num = i;
						break;
					}
				}
				if (state == 1)
				{
					playerColrs[num].transform.localPosition = new Vector3(-43f, 1.1f, -1f);
					playerNames[num].transform.localPosition = new Vector3(-35f, 1.4f, -1f);
				}
				else
				{
					playerColrs[num].transform.localPosition = new Vector3(-43f, -0.9f, -1f);
					playerNames[num].transform.localPosition = new Vector3(-35f, -0.6f, -1f);
				}
			}
			else
			{
				bool flag = togButton.name.StartsWith("btn_win");
				bool flag2 = !flag && togButton.name.StartsWith("btn_time");
				if (flag || flag2)
				{
					((!flag) ? matchTxtTime : matchTxtWin).transform.localPosition = new Vector3(0f, (state != 1) ? (-0.5f) : 0f, -1f);
				}
			}
		}
		if (!(togButton.name == "btn_ok") && !(togButton.name == "btn_cancel"))
		{
			return;
		}
		Transform child = togButton.transform.GetChild(0);
		Vector3 localPosition = child.localPosition;
		if (state == 2)
		{
			if (localPosition.x == 0f && localPosition.y == 0f)
			{
				localPosition.x = -0.03f;
				localPosition.y = -0.06f;
				child.localPosition = localPosition;
			}
		}
		else if (localPosition.x != 0f || localPosition.y != 0f)
		{
			localPosition.x = 0f;
			localPosition.y = 0f;
			child.localPosition = localPosition;
		}
	}

	private void pointerControl()
	{
		if (pointerInit == 1)
		{
			pTime += Time.deltaTime;
		}
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (controller.IsMouseMove())
			{
				pTime = 100f;
				Cursor.visible = true;
			}
			StickPointer(controller, i);
		}
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

	private void Update()
	{
		if (svNameNotice)
		{
			svNamePen.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.125f + Mathf.PingPong(Time.time, 0.25f)));
		}
		if (playerCurr.activeSelf)
		{
			playerCurr.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.PingPong(Time.time * 0.8f, 0.2f)));
		}
		if (fadeAlpha == 1f && fadeSign == 0)
		{
			if (goBack)
			{
				GameObject gameObject = GameObject.Find("Online.ServerList");
				if (gameObject != null)
				{
					gameObject.GetComponent<OnlineBattleServerList>().Show(goBackMsg);
				}
				Object.Destroy(base.gameObject);
				return;
			}
			GameInput.InitRemoteSystem(playerIDs);
			GameMap.InitOnlineMode();
			Global.winAmount = matchWin;
			Global.timeAmount = matchTime;
			Global.ghost = matchOpts[0];
			Global.itemDestroy = matchOpts[1];
			Global.mapShrink = matchOpts[2];
			Global.virus = matchOpts[3];
			Global.onlineMatchID = 0;
			if (TNManager.isHosting)
			{
				int num = 0;
				for (int i = 0; i < 8; i++)
				{
					if (playerIDs[i] != -1)
					{
						num++;
					}
				}
				TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, false, matchMaps, matchWin, matchTime, matchOpts, num, maxPlayer);
			}
			Loading.LoadScene("Adventure.GameMode");
			return;
		}
		playerInput();
		pointerControl();
		if (TNManager.isHosting)
		{
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < 8; j++)
			{
				if (playerIDs[j] != -1)
				{
					num2++;
					if (readiness[j])
					{
						num3++;
					}
				}
			}
			if (num2 > 1 && num2 == num3)
			{
				if (countDown > 0f)
				{
					float num4 = countDown;
					countDown -= Time.deltaTime;
					if (countDown > 0f)
					{
						if ((int)num4 != (int)countDown)
						{
							GetComponent<TNObject>().Send(116, Target.All, (int)num4);
						}
					}
					else
					{
						GetComponent<TNObject>().Send(117, Target.All, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
					}
				}
			}
			else
			{
				countDown = 5f;
			}
			if (num2 == 1)
			{
				countPing += Time.deltaTime;
				if (countPing >= 5f)
				{
					countPing = 0f;
					GetComponent<TNObject>().Send(112, Target.Host, TNManager.playerID, -1000000);
				}
			}
			else
			{
				countPing = 0f;
			}
			if (num2 == maxPlayer && num2 == num3 + 1)
			{
				bool flag = false;
				for (int k = 0; k < 8; k++)
				{
					if (playerIDs[k] == Global.onlinePlayerID && !readiness[k])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					hostAutoGo += Time.deltaTime;
				}
				else
				{
					hostAutoGo = 0f;
				}
			}
			else
			{
				hostAutoGo = 0f;
			}
			if (hostAutoGo >= 30f)
			{
				for (int l = 0; l < 8; l++)
				{
					if (playerIDs[l] == Global.onlinePlayerID)
					{
						readiness[l] = true;
						break;
					}
				}
				GetComponent<TNObject>().Send(115, Target.All, TNManager.playerID, true);
			}
		}
		else
		{
			if (isFresh && isWaitData)
			{
				int num5 = 0;
				for (int m = 0; m < 8; m++)
				{
					if (playerIDs[m] != -1)
					{
						num5++;
					}
				}
				if (num5 == 0)
				{
					int num6 = (int)waitDataTime;
					waitDataTime += 2f * Time.deltaTime;
					if (!(waitDataTime >= 5f))
					{
						if (num6 != (int)waitDataTime)
						{
							GetComponent<TNObject>().Send(118, Target.Host);
						}
						return;
					}
					Debug.Log("Empty room???");
					goBack = true;
					goBackMsg = null;
					fadeSign = 1;
				}
				else
				{
					isWaitData = false;
				}
			}
			countPing += Time.deltaTime;
			if (countPing >= 5f)
			{
				countPing = 0f;
				GetComponent<TNObject>().Send(112, Target.Host, TNManager.playerID, TNManager.ping);
			}
			if (countDown > 0f)
			{
				countDown -= Time.deltaTime;
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
				}
			}
			confiGUI.localScale = new Vector3(confiFadeValue, confiFadeValue, confiFadeValue);
		}
		if (readyBG.GetComponent<Collider>().enabled)
		{
			if (readyBGCol < 0.1875f)
			{
				readyBGCol += Time.deltaTime;
				if (readyBGCol > 0.1875f)
				{
					readyBGCol = 0.1875f;
				}
				readyBG.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, readyBGCol));
			}
		}
		else if (readyBGCol > 0f)
		{
			readyBGCol -= Time.deltaTime;
			if (readyBGCol < 0f)
			{
				readyBGCol = 0f;
			}
			readyBG.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, readyBGCol));
		}
		if (isWaitData || confiFadeValue > 0f)
		{
			if (busyFade < 0.5f)
			{
				if (busyFade == 0f)
				{
					confiFadeBG.SetActive(true);
				}
				busyFade += Time.deltaTime * 4f;
				if (busyFade > 0.5f)
				{
					busyFade = 0.5f;
				}
				busyMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, busyFade / 2f));
			}
		}
		else if (busyFade > 0f)
		{
			busyFade -= Time.deltaTime * 4f;
			if (busyFade < 0f)
			{
				busyFade = 0f;
			}
			busyMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, busyFade / 2f));
			if (busyFade == 0f)
			{
				busyText.color = new Color(1f, 1f, 1f, 0f);
				confiFadeBG.SetActive(false);
			}
		}
		if (busyFade > 0f)
		{
			float num7 = 2f * (Time.time - (float)(int)Time.time);
			busyText.color = new Color(1f, 1f, 1f, busyFade * 2f * ((!(num7 < 1f)) ? (2f - num7) : num7));
			num7 *= 2f;
			string text = "Please wait";
			for (int num8 = (int)num7; num8 > 0; num8--)
			{
				text += ".";
			}
			busyText.text = text;
		}
		if (fadeSign < 0)
		{
			fadeAlpha -= Time.deltaTime * 4f;
			if (fadeAlpha <= 0f)
			{
				fadeAlpha = 0f;
				fadeSign = 0;
			}
			fadeMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fadeAlpha / 2f));
		}
		else if (fadeSign > 0)
		{
			fadeAlpha += Time.deltaTime * 4f;
			if (fadeAlpha >= 1f)
			{
				fadeAlpha = 1f;
				fadeSign = 0;
			}
			fadeMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fadeAlpha / 2f));
		}
		bool flag2 = false;
		bool[] array = new bool[5];
		if (countDown < 5f && countDown > 0f)
		{
			if (cntDarkCol < 1f)
			{
				cntDarkCol += Time.deltaTime * 2f;
				if (cntDarkCol > 1f)
				{
					cntDarkCol = 1f;
				}
				flag2 = true;
			}
			int num9 = (int)countDown;
			float num10 = countDown - (float)num9;
			cnt54321Val[num9] = num10;
			cnt54321Col[num9] = ((!(num10 > 0.5f)) ? 1f : ((1f - num10) * 2f));
			array[num9] = true;
			if (num9 < 4)
			{
				cnt54321Val[num9 + 1] = num10 - 1f;
				cnt54321Col[num9 + 1] = ((!(num10 > 0.5f)) ? 0f : ((num10 - 0.5f) * 2f));
				array[num9 + 1] = true;
			}
		}
		else
		{
			if (cntDarkCol > 0f)
			{
				cntDarkCol -= Time.deltaTime * 2f;
				if (cntDarkCol < 0f)
				{
					cntDarkCol = 0f;
				}
				flag2 = true;
			}
			for (int n = 0; n < 5; n++)
			{
				if (cnt54321Val[n] > 0f)
				{
					cnt54321Val[n] -= Time.deltaTime;
					if (cnt54321Val[n] < 0f)
					{
						cnt54321Val[n] = 0f;
					}
					array[n] = true;
				}
				if (cnt54321Col[n] > 0f)
				{
					cnt54321Col[n] -= Time.deltaTime * 2f;
					if (cnt54321Col[n] < 0f)
					{
						cnt54321Col[n] = 0f;
					}
					array[n] = true;
				}
			}
		}
		if (flag2)
		{
			cntDarkMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * cntDarkCol));
		}
		for (int num11 = 0; num11 < 5; num11++)
		{
			if (array[num11])
			{
				float num12 = 1f + (float)(num11 + 1) * ((!(cnt54321Val[num11] > 0f)) ? 0f : (cnt54321Val[num11] * cnt54321Val[num11]));
				num12 *= 1.5f;
				cnt54321Obj[num11].transform.localScale = new Vector3(17.066668f * num12, 25.6f * num12, 1f);
				cnt54321Mat[num11].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * cnt54321Col[num11]));
			}
		}
	}

	private void UpdatePick()
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < 8; i++)
		{
			Player player = ((playerIDs[i] != -1) ? TNManager.GetPlayer(playerIDs[i]) : null);
			int num3 = ((player != null) ? pingValue[i] : (-1));
			playerNames[i].text = ((player != null) ? player.name : "[Empty]");
			playerPings[i].text = ((num3 < 0) ? string.Empty : ((num3 <= 0 || num3 >= 1000) ? "- ms" : (num3 + " ms")));
			if (playerIDs[i] == -1)
			{
				playerSlots[i].GetComponent<Collider>().enabled = true;
				TextureUtility.SetSpriteIndex(playerSlots[i], 4, 1, 0);
				playerIcons[i].SetActive(false);
				playerKicks[i].SetActive(false);
				playerNames[i].color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				playerSlots[i].GetComponent<Collider>().enabled = false;
				TextureUtility.SetSpriteIndex(playerSlots[i], 4, 1, 3);
				playerIcons[i].SetActive(readiness[i]);
				playerKicks[i].SetActive(playerIDs[i] != Global.onlinePlayerID && TNManager.isHosting);
				playerNames[i].color = new Color(7f / 85f, 0.28627452f, 0.38039216f, 1f);
			}
			if (playerIDs[i] == Global.onlinePlayerID)
			{
				num = i;
			}
			if (num3 == -1000000)
			{
				num2 = i;
			}
		}
		int num4 = 0;
		for (int j = 0; j < 8; j++)
		{
			if (playerIDs[j] != -1)
			{
				num4++;
			}
		}
		slotBtnText.text = num4 + "/" + maxPlayer;
		if (num == -1)
		{
			playerCurr.SetActive(false);
		}
		else
		{
			if (!playerCurr.activeSelf)
			{
				playerCurr.SetActive(true);
			}
			Vector3 localPosition = playerCurr.transform.localPosition;
			localPosition.y = -15 * num;
			playerCurr.transform.localPosition = localPosition;
		}
		if (num2 == -1)
		{
			playerHost.SetActive(false);
			return;
		}
		if (!playerHost.activeSelf)
		{
			playerHost.SetActive(true);
		}
		Vector3 localPosition2 = playerHost.transform.localPosition;
		localPosition2.y = -15 * num2;
		playerHost.transform.localPosition = localPosition2;
	}

	private void UpdateOption()
	{
		if (TNManager.isHosting)
		{
			for (int i = 0; i < 5; i++)
			{
				Vector3 localScale = matchMapTogs[i].transform.localScale;
				localScale.z = (matchMaps[i] ? 1 : 0);
				matchMapTogs[i].transform.localScale = localScale;
				TextureUtility.SetSpriteIndex(matchMapTogs[i], 4, 2, (!matchMaps[i]) ? 4 : 0);
			}
			for (int j = 0; j < 4; j++)
			{
				Vector3 localScale2 = matchOptTogs[j].transform.localScale;
				localScale2.z = (matchOpts[j] ? 1 : 0);
				matchOptTogs[j].transform.localScale = localScale2;
				TextureUtility.SetSpriteIndex(matchOptTogs[j], 4, 2, (!matchOpts[j]) ? 4 : 0);
			}
		}
		else
		{
			for (int k = 0; k < 5; k++)
			{
				TextureUtility.SetSpriteIndex(matchMapTogs[k], 4, 2, (!matchMaps[k]) ? 7 : 3);
			}
			for (int l = 0; l < 4; l++)
			{
				TextureUtility.SetSpriteIndex(matchOptTogs[l], 4, 2, (!matchOpts[l]) ? 7 : 3);
			}
		}
		svNameMesh.text = hostSvName;
		matchTxtWin.text = matchWin.ToString();
		matchTxtTime.text = matchTime.ToString();
		int num = 0;
		for (int m = 0; m < 8; m++)
		{
			if (playerIDs[m] != -1)
			{
				num++;
			}
		}
		slotBtnText.text = num + "/" + maxPlayer;
	}

	private void UpdateReady()
	{
		bool flag = false;
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] == Global.onlinePlayerID)
			{
				flag = readiness[i];
				break;
			}
		}
		Vector3 localScale = playerReady.transform.localScale;
		localScale.z = (flag ? 1 : 0);
		playerReady.transform.localScale = localScale;
		TextureUtility.SetSpriteIndex(playerReady, 4, 2, (!flag) ? 4 : 0);
		playerReady.transform.GetChild(0).GetComponent<Collider>().enabled = flag;
		if (TNManager.isHosting || !(countDown < 5f) || !(countDown > 0f))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < 8; j++)
		{
			if (playerIDs[j] != -1)
			{
				num++;
				if (readiness[j])
				{
					num2++;
				}
			}
		}
		if (num <= 1 || num != num2)
		{
			countDown = 0f;
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.height / 1080f;
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.Scale(new Vector3(num, num, 1f));
		GUISkin gUISkin = GUI.skin;
		GUI.skin = skin;
		bool flag = svNameGUI;
		GUI.SetNextControlName("svNameField");
		if (svNameGUI)
		{
			svNameWord = GUI.TextField(new Rect((float)Screen.width / num / 2f - 494f, 207f, 555f, 60f), svNameWord, 12);
		}
		else
		{
			GUI.TextField(new Rect((float)Screen.width / num / 2f - 494f, 207f, -1f, 60f), string.Empty);
		}
		if (svNameFocus)
		{
			if (svNameNotice)
			{
				svNameNotice = false;
				svNamePen.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.25f));
			}
			svNameFocus = false;
			svNameWordBak = svNameWord;
			GUI.FocusControl("svNameField");
			return;
		}
		if (GUI.GetNameOfFocusedControl() == "svNameField")
		{
			Event current = Event.current;
			if (current.type == EventType.KeyUp)
			{
				if (current.keyCode == KeyCode.Escape)
				{
					svNameGUI = false;
					svNameWord = svNameWordBak;
				}
				else if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
				{
					svNameGUI = false;
					svNameWord = OnlineBattleServerList.ValidateRoomName(svNameWord);
				}
			}
		}
		else
		{
			svNameGUI = false;
		}
		if (!flag || svNameGUI)
		{
			return;
		}
		GUI.FocusControl(null);
		if (svNameWord.Length == 0)
		{
			svNameMesh.text = Global.onlinePlayerName + " Server";
		}
		else
		{
			svNameMesh.text = svNameWord;
		}
		svNameMesh.gameObject.SetActive(true);
		hostSvName = svNameMesh.text;
		int num2 = 0;
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] != -1)
			{
				num2++;
			}
		}
		TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, true, matchMaps, matchWin, matchTime, matchOpts, num2, maxPlayer);
		GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
	}

	public void PlayerJoin(int id)
	{
		if (!TNManager.isHosting)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] == -1)
			{
				playerIDs[i] = id;
				pingValue[i] = ((id != Global.onlinePlayerID) ? (-1) : (-1000000));
				readiness[i] = false;
				break;
			}
		}
		int num = 0;
		for (int j = 0; j < 8; j++)
		{
			if (playerIDs[j] != -1)
			{
				num++;
			}
		}
		TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, true, matchMaps, matchWin, matchTime, matchOpts, num, maxPlayer);
		GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
		GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
		UpdatePick();
		UpdateOption();
	}

	public void PlayerLeave(int id)
	{
		if (!TNManager.isHosting)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] == id)
			{
				playerIDs[i] = -1;
				pingValue[i] = -1;
				readiness[i] = false;
				break;
			}
		}
		int num = 0;
		for (int j = 0; j < 8; j++)
		{
			if (playerIDs[j] != -1)
			{
				num++;
			}
		}
		TNServerInstance.serverName = OnlineBattleServerList.GetFormattedServerName(hostSvName, true, matchMaps, matchWin, matchTime, matchOpts, num, maxPlayer);
		GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
		GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
		UpdatePick();
		UpdateOption();
	}

	public void Back(string msg = null)
	{
		goBack = true;
		goBackMsg = msg;
		fadeSign = 1;
	}

	[RFC(110)]
	private void OnlineBattleLobbyRoomPickSlot(int id, int slot, int ping)
	{
		if (slot < 0 || playerIDs[slot] == -1)
		{
			for (int i = 0; i < 8; i++)
			{
				if (playerIDs[i] == id)
				{
					playerIDs[i] = -1;
					pingValue[i] = -1;
					readiness[i] = false;
					break;
				}
			}
			if (slot >= 0)
			{
				playerIDs[slot] = id;
				pingValue[slot] = ping;
				readiness[slot] = false;
			}
		}
		GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
		UpdatePick();
	}

	[RFC(111)]
	private void OnlineBattleLobbyRoomPickUpdate(int[] playerIDs, int[] pingValue, bool[] readyness)
	{
		this.playerIDs = playerIDs;
		this.pingValue = pingValue;
		readiness = readyness;
		UpdatePick();
	}

	[RFC(112)]
	private void OnlineBattleLobbyRoomPing(int id, int ping)
	{
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] == id)
			{
				pingValue[i] = ping;
				break;
			}
		}
		GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
		UpdatePick();
	}

	[RFC(113)]
	private void OnlineBattleLobbyRoomOptionUpdate(string serverName, bool[] matchMap, int matchWin, int matchTime, bool[] matchOpts, int maxPlayer)
	{
		hostSvName = serverName;
		matchMaps = matchMap;
		this.matchWin = matchWin;
		this.matchTime = matchTime;
		this.matchOpts = matchOpts;
		this.maxPlayer = maxPlayer;
		UpdateOption();
	}

	[RFC(114)]
	private void OnlineBattleLobbyRoomKick(int id)
	{
		if (TNManager.playerID == id)
		{
			Back("You were kicked from the game.");
		}
	}

	[RFC(115)]
	private void OnlineBattleLobbyRoomReady(int id, bool ready)
	{
		for (int i = 0; i < 8; i++)
		{
			if (playerIDs[i] == id)
			{
				readiness[i] = ready;
				break;
			}
		}
		UpdatePick();
		UpdateReady();
	}

	[RFC(116)]
	private void OnlineBattleLobbyRoomCountDown(int count)
	{
		Debug.Log(count + "...");
		GameSound.StartSFX("bombDrop");
		if (!TNManager.isHosting && (countDown == 0f || countDown > (float)count))
		{
			countDown = count;
		}
	}

	[RFC(117)]
	private void OnlineBattleLobbyRoomGo(bool[] matchMap, int matchWin, int matchTime, bool[] matchOpts, int maxPlayer)
	{
		GameSound.StartSFX("bombBlast");
		goBack = false;
		goBackMsg = null;
		fadeSign = 1;
		matchMaps = matchMap;
		this.matchWin = matchWin;
		this.matchTime = matchTime;
		this.matchOpts = matchOpts;
		this.maxPlayer = maxPlayer;
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			if (matchMap[i])
			{
				num++;
			}
		}
		onlineMapCount = 0;
		onlineMapIDs = new int[num];
		num = 0;
		for (int j = 0; j < 5; j++)
		{
			if (matchMap[j])
			{
				onlineMapIDs[num++] = j;
			}
		}
		for (int k = 0; k < 8; k++)
		{
			if (Global.onlinePlayerID == playerIDs[k])
			{
				Global.onlinePlayerSlot = k;
			}
			Global.onlinePlayerIDs[k] = playerIDs[k];
			Global.onlinePlayerNames[k] = ((playerIDs[k] != -1) ? TNManager.GetPlayer(playerIDs[k]).name : null);
			Global.onlinePlayerOns[k] = playerIDs[k] != -1;
		}
		Global.ClearBattleData();
		UpdateOption();
		Debug.Log("I'm slot:" + Global.onlinePlayerSlot + ", id:" + Global.onlinePlayerID);
	}

	[RFC(118)]
	private void OnlineBattleLobbyRoomRequestData()
	{
		GetComponent<TNObject>().Send(111, Target.Others, playerIDs, pingValue, readiness);
		GetComponent<TNObject>().Send(113, Target.Others, hostSvName, matchMaps, matchWin, matchTime, matchOpts, maxPlayer);
	}

	public static int GetNextMapID()
	{
		int num = onlineMapIDs[onlineMapCount++];
		Debug.LogWarning("GetNextMapID: " + num);
		if (onlineMapCount >= onlineMapIDs.Length)
		{
			onlineMapCount = 0;
		}
		return num + 1;
	}
}
