using System.Net;
using System.Text.RegularExpressions;
using TNet;
using UnityEngine;

public class OnlineBattleServerList : MonoBehaviour
{
	private enum SortRoomKey
	{
		bySlot = 0,
		byPing = 1,
		byName = 2,
		byWin = 3,
		byTime = 4,
		byOptn0 = 5,
		byOptn1 = 6,
		byOptn2 = 7,
		byOptn3 = 8,
		byMaps = 9,
		MAX = 10
	}

	public GUISkin skin;

	private Material fadeMat;

	private float fadeAlpha;

	private int fadeSign;

	private Material busyMat;

	private float busyFade;

	private TextMesh busyText;

	private string nextScene;

	private List<ServerList.Entry> svList;

	private ServerList.Entry svJoin;

	private List<int> svPing;

	private IPAddress ownIP;

	private string ownSvName = string.Empty;

	private bool doCreate;

	private bool doLeave;

	private float firstRun;

	private bool doConnect;

	private bool doJoinCh;

	private bool doUPnP;

	private ServerList.Entry entUPnP;

	private bool visible;

	private GameObject guiNode;

	private bool mouseOver;

	private GameObject togButton;

	private GameObject player;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private int pointerInit;

	private bool isPress;

	private bool joyGetFocus;

	private TextMesh[] shadowName;

	private SortRoomKey[] sortKey;

	private int[] sortVal;

	private Transform[] sortBtn;

	private Material sortMat;

	private Material sortAct;

	private TextMesh searchMesh;

	private string searchWord;

	private string searchWordBak;

	private bool searchGUI;

	private bool searchFocus;

	private GameObject searchClear;

	private TextMesh ipConnMesh;

	private string ipConnIP;

	private string ipConnIPBak;

	private bool ipConnGUI;

	private bool ipConnFocus;

	private GameObject ipConnClear;

	private Transform[] rooms;

	private GameObject barSelect;

	private GameObject btnJoin;

	private int curPage;

	private int maxPage;

	private TextMesh txtPage;

	private GameObject prvPage;

	private GameObject nxtPage;

	private List<IPEndPoint> pingAddr;

	private List<int> pingList;

	private float pingTime;

	private GameObject popupSystem;

	private float popupFadeValue;

	private int popupFadeSign;

	private GameObject popupFadeBG;

	private Transform popupGUI;

	private GameObject popupBtn;

	private TextMesh popupTxt;

	private float confiFadeValue;

	private int confiFadeSign;

	private Transform confiGUI;

	private GameObject[] confiBtn;

	private TextMesh confiTxt;

	private static OnlineBattleServerList onlineLobbyRoom;

	private static OnlineBattleLobbyRoom onlineSettings;

	private void Awake()
	{
		Debug.Log("OnlineBattleServerList.Awake");
		for (int i = 0; i < 8; i++)
		{
			Global.onlinePlayerIDs[i] = -1;
			Global.onlinePlayerNames[i] = null;
			Global.onlinePlayerOns[i] = false;
		}
		if (onlineLobbyRoom != null)
		{
			onlineLobbyRoom.Show();
			GameInput.DestroyRemoteSystem();
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		onlineLobbyRoom = this;
		guiNode = base.transform.Find("GUI").gameObject;
		Transform transform = guiNode.transform.Find("panel");
		Transform transform2 = transform.Find("player_name");
		player = transform.Find("p_pointer").gameObject;
		shadowName = transform2.GetComponentsInChildren<TextMesh>();
		TextMesh[] array = shadowName;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = Global.onlinePlayerName;
		}
		TextMesh component = transform2.GetComponent<TextMesh>();
		for (int num = shadowName.Length - 1; num >= 0; num--)
		{
			if (shadowName[num] == component)
			{
				shadowName[num] = null;
				break;
			}
		}
		fadeMat = transform.Find("fade_bg").GetComponent<Renderer>().material;
		busyMat = transform.Find("PopupGUI/bg").GetComponent<Renderer>().material;
		busyText = transform.Find("pls_wait").GetComponent<TextMesh>();
		rooms = new Transform[6];
		Transform transform3 = transform.Find("rooms");
		for (int k = 0; k < 6; k++)
		{
			rooms[k] = transform3.Find("room_" + k);
			rooms[k].gameObject.SetActive(false);
		}
		barSelect = transform3.Find("bar_select").gameObject;
		barSelect.SetActive(false);
		btnJoin = transform.Find("btn_join").gameObject;
		btnJoin.GetComponent<Collider>().enabled = false;
		TextureUtility.SetSpriteIndex(btnJoin, 4, 1, 3);
		curPage = 0;
		maxPage = 1;
		txtPage = transform.Find("page_count").GetComponent<TextMesh>();
		txtPage.text = "1/1";
		prvPage = transform.Find("page_count/btn_arrow_l").gameObject;
		prvPage.GetComponent<Collider>().enabled = false;
		TextureUtility.SetSpriteIndex(prvPage, 4, 1, 3);
		nxtPage = transform.Find("page_count/btn_arrow_r").gameObject;
		nxtPage.GetComponent<Collider>().enabled = false;
		TextureUtility.SetSpriteIndex(nxtPage, 4, 1, 3);
		Transform transform4 = transform.Find("sort_bar");
		sortKey = new SortRoomKey[10];
		sortVal = new int[10];
		sortBtn = new Transform[10];
		for (int l = 0; l < 10; l++)
		{
			sortKey[l] = (SortRoomKey)l;
			sortVal[l] = 1;
			sortBtn[l] = transform4.Find("btn_sort_" + l);
		}
		sortAct = Object.Instantiate(sortBtn[0].GetComponent<Renderer>().material) as Material;
		sortMat = Object.Instantiate(sortBtn[1].GetComponent<Renderer>().material) as Material;
		searchMesh = transform.Find("fn_search/field_search").GetComponent<TextMesh>();
		searchWord = string.Empty;
		searchClear = transform.Find("fn_search/clear_search").gameObject;
		TextureUtility.SetSpriteIndex(searchClear, 2, 1, 1);
		ipConnMesh = transform.Find("fn_directIP/field_directIP").GetComponent<TextMesh>();
		ipConnIP = string.Empty;
		ipConnClear = transform.Find("fn_directIP/clear_directIP").gameObject;
		TextureUtility.SetSpriteIndex(ipConnClear, 2, 1, 1);
		pingAddr = new List<IPEndPoint>();
		pingList = new List<int>();
		popupFadeValue = 0f;
		popupFadeSign = 0;
		popupSystem = transform.Find("PopupGUI").gameObject;
		popupFadeBG = transform.Find("PopupGUI/bg").gameObject;
		popupGUI = transform.Find("PopupGUI/dialog");
		popupGUI.localScale = Vector3.zero;
		popupBtn = popupGUI.Find("btn_okDC").gameObject;
		popupTxt = popupGUI.Find("text").GetComponent<TextMesh>();
		confiFadeValue = 0f;
		confiFadeSign = 0;
		confiGUI = transform.Find("PopupGUI/confirm");
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("ui/btn_ok").gameObject;
		confiBtn[1] = confiGUI.Find("ui/btn_cancel").gameObject;
		confiTxt = confiGUI.Find("ui/text").GetComponent<TextMesh>();
		TNLobbyClient component2 = base.gameObject.GetComponent<TNLobbyClient>();
		component2.remotePort = Global.lobbyPort;
		if (Global.onlineWAN)
		{
			component2.remoteAddress = Global.lobbyIP;
		}
		component2.enabled = true;
		ownSvName = Global.onlinePlayerName + " Server";
	}

	private void Start()
	{
		Debug.Log("OnlineBattleServerList.Start");
		TNManager.StartUDP(Random.Range(10000, 50000));
		fadeAlpha = 1f;
		fadeSign = -1;
		visible = true;
		guiNode.SetActive(true);
	}

	private void OnDestroy()
	{
		if (onlineLobbyRoom == this)
		{
			onlineLobbyRoom = null;
		}
	}

	public void Show(string msg = null)
	{
		Debug.Log("OnlineBattleServerList.Show " + msg);
		for (int i = 0; i < 8; i++)
		{
			Global.onlinePlayerIDs[i] = -1;
			Global.onlinePlayerNames[i] = null;
			Global.onlinePlayerOns[i] = false;
		}
		fadeAlpha = 1f;
		fadeSign = -1;
		nextScene = null;
		svJoin = null;
		firstRun = 0f;
		TNManager.Disconnect();
		visible = true;
		guiNode.SetActive(true);
		onlineSettings = null;
		PopUpMessage(msg);
	}

	public void Hide()
	{
		Debug.Log("OnlineBattleServerList.Hide");
		visible = false;
		guiNode.SetActive(false);
		GameObject gameObject = GameObject.Find("Online.LobbyRoom");
		if (gameObject != null)
		{
			onlineSettings = gameObject.GetComponent<OnlineBattleLobbyRoom>();
		}
	}

	private void playerInput()
	{
		bool flag = fadeAlpha > 0f;
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 position = player.transform.GetChild(0).position;
			RaycastHit hitInfo;
			if (!flag && ((Physics.Raycast(ray, out hitInfo) && pointerInit == 0) || Physics.Raycast(position, Vector3.forward, out hitInfo)))
			{
				if (hitInfo.transform.name == "bg")
				{
					if (!(togButton != null))
					{
						continue;
					}
					if (togButton.GetComponent<Collider>().enabled)
					{
						if (togButton.name.StartsWith("btn_bar_"))
						{
							togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
						}
						else
						{
							TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
						}
					}
					togButton = null;
					mouseOver = false;
					if (controller.DoEnterPress("x"))
					{
						joyGetFocus = true;
					}
				}
				else if (hitInfo.transform.name.StartsWith("field_"))
				{
					if (togButton != null)
					{
						if (togButton.GetComponent<Collider>().enabled)
						{
							if (togButton.name.StartsWith("btn_bar_"))
							{
								togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
							}
							else
							{
								TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
							}
						}
						togButton = null;
						mouseOver = false;
					}
					if (controller.DoEnterPress("x"))
					{
						controller.DoPadPress(i);
						if (hitInfo.transform.name == "field_search")
						{
							searchMesh.gameObject.SetActive(false);
							searchGUI = true;
							searchFocus = true;
						}
						else if (hitInfo.transform.name == "field_directIP")
						{
							ipConnMesh.gameObject.SetActive(false);
							ipConnGUI = true;
							ipConnFocus = true;
						}
					}
				}
				else if (hitInfo.transform.name.StartsWith("clear_"))
				{
					if (togButton != null)
					{
						if (togButton.GetComponent<Collider>().enabled)
						{
							if (togButton.name.StartsWith("btn_bar_"))
							{
								togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
							}
							else
							{
								TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
							}
						}
						togButton = null;
						mouseOver = false;
					}
					if (controller.DoEnterPress("x"))
					{
						controller.DoPadPress(i);
						joyGetFocus = true;
						if (hitInfo.transform.name == "clear_search")
						{
							searchWord = string.Empty;
							Color color = searchMesh.color;
							color.a = 0.25f;
							searchMesh.color = color;
							searchMesh.text = "Room's Name";
							TextureUtility.SetSpriteIndex(searchClear, 2, 1, 1);
							Refresh();
						}
						else if (hitInfo.transform.name == "clear_directIP")
						{
							ipConnIP = string.Empty;
							Color color2 = ipConnMesh.color;
							color2.a = 0.25f;
							ipConnMesh.color = color2;
							ipConnMesh.text = "IP Address";
							TextureUtility.SetSpriteIndex(ipConnClear, 2, 1, 1);
						}
					}
				}
				else
				{
					if (!hitInfo.transform.name.StartsWith("btn_"))
					{
						continue;
					}
					if (togButton != null && togButton != hitInfo.transform.gameObject)
					{
						mouseOver = false;
						if (togButton.name.StartsWith("btn_bar_"))
						{
							togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
						}
						else
						{
							TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
						}
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
					if (togButton.name.StartsWith("btn_bar_"))
					{
						togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.75f, 0.75f, 0.75f, 0.5f));
					}
					else if (!isPress)
					{
						TextureUtility.SetSpriteIndex(hitInfo.transform.gameObject, 4, 1, 1);
					}
					if (controller.DoEnterHold("x"))
					{
						controller.DoPadPress(i);
						joyGetFocus = true;
						if (togButton.name.StartsWith("btn_bar_"))
						{
							togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.5f));
						}
						else
						{
							TextureUtility.SetSpriteIndex(hitInfo.transform.gameObject, 4, 1, 2);
						}
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
						if (togButton.name.StartsWith("btn_bar_"))
						{
							int num = togButton.name[8] - 48;
							if (!barSelect.activeSelf)
							{
								barSelect.SetActive(true);
							}
							barSelect.transform.localPosition = new Vector3(0f, -12 * num, -3f);
							svJoin = svList[curPage * 6 + num];
							btnJoin.GetComponent<Collider>().enabled = true;
							TextureUtility.SetSpriteIndex(btnJoin, 4, 1, 0);
							Debug.Log("select " + svJoin.name);
							continue;
						}
						Debug.Log("Do action " + togButton.name);
						if (togButton.name.StartsWith("btn_sort_"))
						{
							SetSort(togButton.name[9] - 48);
						}
						else if (togButton.name == "btn_arrow_l")
						{
							if (curPage > 0)
							{
								curPage--;
							}
							Refresh();
						}
						else if (togButton.name == "btn_arrow_r")
						{
							if (curPage < maxPage - 1)
							{
								curPage++;
							}
							Refresh();
						}
						else if (togButton.name == "btn_back")
						{
							TNManager.StopUDP();
							fadeSign = 1;
							nextScene = "OnlineBattle.Menu";
						}
						else if (togButton.name == "btn_create")
						{
							for (int j = 0; j < 8; j++)
							{
								Global.onlinePlayerIDs[j] = -1;
								Global.onlinePlayerNames[j] = null;
								Global.onlinePlayerOns[j] = false;
							}
							Global.udpPort = Random.Range(10000, 50000);
							ownIP = Tools.localAddress;
							doCreate = true;
							TNServerInstance.serverName = GetFormattedServerName(ownSvName);
							Debug.LogWarning("TNServerInstance.Start " + ownSvName);
							if (Global.onlineWAN)
							{
								TNServerInstance.Start(Global.tcpPort, Global.udpPort, "server.dat", TNServerInstance.Type.Udp, Tools.ResolveEndPoint(Global.lobbyIP, Global.lobbyPort));
							}
							else
							{
								TNServerInstance.Start(Global.tcpPort, Global.udpPort, Global.lobbyPort, "server.dat");
							}
						}
						else if (togButton.name == "btn_join")
						{
							foreach (ServerList.Entry item in TNLobbyClient.knownServers.list)
							{
								if (item.externalAddress.Equals(svJoin.externalAddress) && item.internalAddress.Equals(svJoin.internalAddress))
								{
									if (IsServerOpen(item.name))
									{
										if (Global.onlineWAN && IsServerFail(item.name))
										{
											PopUpConfirm("This host may have connectivity issues.\n\nDo you want to try and join\nthe game anyway?");
											entUPnP = item;
											return;
										}
										doConnect = true;
										TNManager.Connect(svJoin.externalAddress, svJoin.internalAddress);
										break;
									}
									break;
								}
							}
							if (!doConnect)
							{
								Refresh();
								PopUpMessage("Unable to connect to the server.");
							}
						}
						else if (togButton.name == "btn_search")
						{
							searchWord = ValidateRoomName(searchWord);
							Refresh();
						}
						else if (togButton.name == "btn_directIP")
						{
							ipConnIP = ValidateIPAddress(ipConnIP);
							if (ipConnIP.Length > 0)
							{
								doConnect = true;
								doCreate = false;
								TNManager.Connect(ipConnIP);
							}
						}
						else if (togButton.name == "btn_refresh")
						{
							Refresh();
						}
						else if (togButton.name == "btn_okDC")
						{
							popupFadeSign = -1;
						}
						else if (togButton.name == "btn_ok")
						{
							confiFadeSign = -1;
							doUPnP = false;
							doConnect = true;
							TNManager.Connect(entUPnP.externalAddress, entUPnP.internalAddress);
							entUPnP = null;
						}
						else if (togButton.name == "btn_cancel")
						{
							confiFadeSign = -1;
							if (doUPnP)
							{
								doUPnP = false;
								doConnect = false;
								doLeave = true;
								TNServerInstance.Stop("server.dat");
							}
							entUPnP = null;
						}
					}
				}
			}
			else if (togButton != null)
			{
				if (togButton.GetComponent<Collider>().enabled)
				{
					if (togButton.name.StartsWith("btn_bar_"))
					{
						togButton.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
					}
					else
					{
						TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
					}
				}
				controller.DoPadRelease(i);
				togButton = null;
				mouseOver = false;
				isPress = false;
			}
			else
			{
				controller.DoPadRelease(i);
				if (controller.DoEnterPress("x"))
				{
					joyGetFocus = true;
				}
			}
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
			player.transform.localPosition = new Vector3(-500f, 0f, -100f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = player.transform.localPosition;
			if (i == 1)
			{
				float num = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit == 0 && num != 0f && localPosition.x < -181f)
				{
					player.transform.localPosition = new Vector3(0f, 0f, -100f);
					pointerInit = 1;
					Cursor.visible = false;
					break;
				}
				if (num < 0f)
				{
					if (localPosition.x >= -181f)
					{
						player.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
						pTime = 0f;
					}
				}
				else if (num > 0f && localPosition.x <= 195f)
				{
					player.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
					pTime = 0f;
				}
				continue;
			}
			float num2 = ctrl.GetVertical() * 1.25f;
			if (pointerInit == 0 && num2 != 0f && localPosition.x < -181f)
			{
				player.transform.localPosition = new Vector3(0f, 0f, -100f);
				pointerInit = 1;
				Cursor.visible = false;
				break;
			}
			if (num2 < 0f)
			{
				if (localPosition.y <= 98f)
				{
					player.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
					pTime = 0f;
				}
			}
			else if (num2 > 0f && localPosition.y >= -115f)
			{
				player.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
				pTime = 0f;
			}
		}
	}

	private void Update()
	{
		if (!visible)
		{
			return;
		}
		if (pingTime == 0f)
		{
			List<ServerList.Entry> list = TNLobbyClient.knownServers.list;
			if (Global.onlineWAN)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (IsServer(list[num].name))
					{
						TNManager.Ping(Tools.ResolveEndPoint(list[num].externalAddress.Address.ToString(), Global.lobbyPort), OnPing);
					}
				}
			}
			else
			{
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					if (IsServer(list[num2].name))
					{
						TNManager.Ping(Tools.ResolveEndPoint(list[num2].internalAddress.Address.ToString(), Global.lobbyPort), OnPing);
					}
				}
			}
		}
		pingTime += Time.deltaTime;
		if (pingTime >= 5f)
		{
			pingTime = 0f;
		}
		Color color = ((!(shadowName[0] == null)) ? shadowName[0] : shadowName[1]).color;
		color.a = Mathf.PingPong(Time.time * 45f, 0.4f) + 0.35f;
		TextMesh[] array = shadowName;
		foreach (TextMesh textMesh in array)
		{
			if (textMesh != null)
			{
				textMesh.color = color;
			}
		}
		if (barSelect.activeSelf)
		{
			barSelect.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.125f + Mathf.PingPong(Time.time * 1.5f, 0.5f)));
		}
		if (doUPnP)
		{
			if (TNServerInstance.upnp.status == UPnP.Status.Success)
			{
				doUPnP = false;
				doConnect = true;
				TNManager.Connect(entUPnP.externalAddress, entUPnP.internalAddress);
				entUPnP = null;
			}
			else if (TNServerInstance.upnp.status == UPnP.Status.Failure && !doConnect)
			{
				doConnect = true;
				Debug.LogWarning("UPnP.Status.Failure");
				PopUpConfirm("Your router does not support UPnP.\nOther players will see the created game,\nbut they will not be able to join.\n\nYou may try to enable forwarding on ports\nUDP and TCP 15127-15129 on your router.");
			}
		}
		playerInput();
		pointerControl();
		if (firstRun < 0.5f)
		{
			firstRun += Time.deltaTime;
			if (firstRun >= 0.5f && !doLeave)
			{
				Refresh();
			}
		}
		if (fadeAlpha == 1f && fadeSign == 0)
		{
			Application.LoadLevel(nextScene);
			return;
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
		if (popupFadeSign != 0)
		{
			if (popupFadeSign > 0)
			{
				popupFadeValue += 5f * Time.deltaTime;
				if (popupFadeValue >= 1f)
				{
					popupFadeValue = 1f;
					popupFadeSign = 0;
				}
			}
			else
			{
				popupFadeValue -= 5f * Time.deltaTime;
				if (popupFadeValue <= 0f)
				{
					popupFadeValue = 0f;
					popupFadeSign = 0;
					popupGUI.gameObject.SetActive(false);
				}
			}
			popupGUI.localScale = new Vector3(popupFadeValue, popupFadeValue, popupFadeValue);
		}
		if (doConnect || doJoinCh || doCreate || doLeave || doUPnP || popupFadeValue > 0f || confiFadeValue > 0f)
		{
			if (busyFade < 0.5f)
			{
				if (busyFade == 0f)
				{
					popupFadeBG.SetActive(true);
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
				popupFadeBG.SetActive(false);
			}
		}
		if (busyFade > 0f)
		{
			float num3 = 2f * (Time.time - (float)(int)Time.time);
			busyText.color = new Color(1f, 1f, 1f, busyFade * 2f * ((!(num3 < 1f)) ? (2f - num3) : num3));
			num3 *= 2f;
			string text = "Please wait";
			for (int num4 = (int)num3; num4 > 0; num4--)
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
		if (nextScene != null)
		{
			return;
		}
		if (doCreate)
		{
			for (int j = 0; j < TNLobbyClient.knownServers.list.Count; j++)
			{
				ServerList.Entry entry = TNLobbyClient.knownServers.list[j];
				if (entry.internalAddress.Address.Equals(ownIP))
				{
					if (Global.onlineWAN)
					{
						doCreate = false;
						doUPnP = true;
						entUPnP = entry;
					}
					else
					{
						doCreate = false;
						doConnect = true;
						TNManager.Connect(entry.externalAddress, entry.internalAddress);
					}
					break;
				}
			}
		}
		if (!doLeave)
		{
			return;
		}
		bool flag = true;
		for (int k = 0; k < TNLobbyClient.knownServers.list.Count; k++)
		{
			ServerList.Entry entry2 = TNLobbyClient.knownServers.list[k];
			if (entry2.internalAddress.Address.Equals(ownIP))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ownIP = null;
			doLeave = false;
			Refresh();
			Debug.Log("TNServerInstance has been stopped");
		}
	}

	private void OnGUI()
	{
		if (!visible)
		{
			return;
		}
		float num = (float)Screen.height / 1080f;
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.Scale(new Vector3(num, num, 1f));
		GUISkin gUISkin = GUI.skin;
		GUI.skin = skin;
		bool flag = joyGetFocus;
		bool flag2 = searchGUI;
		GUI.SetNextControlName("searchField");
		if (searchGUI)
		{
			searchWord = GUI.TextField(new Rect((float)Screen.width / num / 2f - 500f, 333f, 300f, 40f), searchWord, 12);
		}
		else
		{
			GUI.TextField(new Rect((float)Screen.width / num / 2f - 500f, 333f, -1f, 40f), string.Empty);
		}
		if (searchFocus)
		{
			searchFocus = false;
			searchWordBak = searchWord;
			GUI.FocusControl("searchField");
		}
		else
		{
			if (GUI.GetNameOfFocusedControl() == "searchField")
			{
				TextureUtility.SetSpriteIndex(searchClear, 2, 1, (searchWord.Length <= 0) ? 1 : 0);
				Event current = Event.current;
				if (current.type == EventType.KeyUp)
				{
					if (current.keyCode == KeyCode.Escape)
					{
						searchGUI = false;
						searchWord = searchWordBak;
					}
					else if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						searchGUI = false;
						searchWord = ValidateRoomName(searchWord);
						Refresh();
					}
				}
			}
			else
			{
				searchGUI = false;
			}
			if ((flag2 && !searchGUI) || joyGetFocus)
			{
				GUI.FocusControl(null);
				if (searchWord.Length == 0)
				{
					Color color = searchMesh.color;
					color.a = 0.25f;
					searchMesh.color = color;
					searchMesh.text = "Room's Name";
				}
				else
				{
					Color color2 = searchMesh.color;
					color2.a = 1f;
					searchMesh.color = color2;
					searchMesh.text = ValidateRoomName(searchWord);
					searchWord = searchMesh.text;
				}
				searchMesh.gameObject.SetActive(true);
				flag = false;
				TextureUtility.SetSpriteIndex(searchClear, 2, 1, 1);
			}
		}
		bool flag3 = ipConnGUI;
		GUI.SetNextControlName("ipConnField");
		if (ipConnGUI)
		{
			ipConnIP = GUI.TextField(new Rect((float)Screen.width / num / 2f + 34f, 333f, 200f, 40f), ipConnIP, 15);
		}
		else
		{
			GUI.TextField(new Rect((float)Screen.width / num / 2f + 34f, 333f, -1f, 40f), string.Empty);
		}
		if (ipConnFocus)
		{
			ipConnFocus = false;
			ipConnIPBak = ipConnIP;
			GUI.FocusControl("ipConnField");
		}
		else
		{
			if (GUI.GetNameOfFocusedControl() == "ipConnField")
			{
				TextureUtility.SetSpriteIndex(ipConnClear, 2, 1, (ipConnIP.Length <= 0) ? 1 : 0);
				Event current2 = Event.current;
				if (current2.type == EventType.KeyUp)
				{
					if (current2.keyCode == KeyCode.Escape)
					{
						ipConnGUI = false;
						ipConnIP = ipConnIPBak;
					}
					else if (current2.keyCode == KeyCode.Return || current2.keyCode == KeyCode.KeypadEnter)
					{
						ipConnGUI = false;
						ipConnIP = ValidateIPAddress(ipConnIP);
						if (ipConnIP.Length > 0)
						{
							doConnect = true;
							doCreate = false;
							TNManager.Connect(ipConnIP);
						}
					}
				}
			}
			else
			{
				ipConnGUI = false;
			}
			if ((flag3 && !ipConnGUI) || joyGetFocus)
			{
				GUI.FocusControl(null);
				if (ipConnIP.Length == 0)
				{
					Color color3 = ipConnMesh.color;
					color3.a = 0.25f;
					ipConnMesh.color = color3;
					ipConnMesh.text = "IP Address";
				}
				else
				{
					Color color4 = ipConnMesh.color;
					color4.a = 1f;
					ipConnMesh.color = color4;
					ipConnMesh.text = ValidateIPAddress(ipConnIP);
					ipConnIP = ipConnMesh.text;
				}
				ipConnMesh.gameObject.SetActive(true);
				flag = false;
				TextureUtility.SetSpriteIndex(ipConnClear, 2, 1, 1);
			}
		}
		joyGetFocus = flag;
	}

	public void PopUpMessage(string msg)
	{
		if (msg == null || msg.Length <= 2)
		{
			return;
		}
		msg = msg.Trim();
		if (!msg.Contains("\n"))
		{
			int length = msg.Length;
			if (length > 30)
			{
				for (int i = 0; i < length; i++)
				{
					if (msg[i] == ' ' && i > length / 2)
					{
						msg = msg.Substring(0, i) + "\n" + msg.Substring(i + 1);
						break;
					}
				}
			}
		}
		popupTxt.text = msg;
		if (!popupGUI.gameObject.activeSelf)
		{
			popupGUI.gameObject.SetActive(true);
			popupFadeSign = 1;
		}
	}

	public void PopUpConfirm(string msg)
	{
		confiTxt.text = msg;
		if (!confiGUI.gameObject.activeSelf)
		{
			confiGUI.gameObject.SetActive(true);
			confiFadeSign = 1;
		}
	}

	private void OnNetworkConnect(bool success, string error)
	{
		Debug.Log("OnNetworkConnect:" + success + " > " + error);
		if (success)
		{
			doJoinCh = true;
			TNManager.playerName = Global.onlinePlayerName;
			TNManager.JoinChannel(1, null, false, 8, null);
		}
		else if (doConnect)
		{
			Refresh();
			PopUpMessage(error);
		}
		doConnect = false;
	}

	private void OnNetworkDisconnect()
	{
		Debug.Log("OnNetworkDisconnect");
		if (ownIP != null)
		{
			doLeave = true;
			TNServerInstance.Stop("server.dat");
		}
		doConnect = false;
	}

	private void OnNetworkJoinChannel(bool success, string error)
	{
		Debug.Log("OnNetworkJoinChannel:" + success + " > " + error);
		if (success)
		{
			Global.onlinePlayerID = TNManager.playerID;
			if (TNManager.isHosting)
			{
				TNManager.noDelay = true;
				TNManager.SetPlayerLimit(8);
			}
			fadeSign = 1;
			nextScene = "OnlineBattle.LobbyRoom";
		}
		else
		{
			TNManager.Disconnect();
			doConnect = true;
			PopUpMessage(error);
		}
		doJoinCh = false;
	}

	private void OnNetworkLeaveChannel()
	{
		Debug.Log("OnNetworkLeaveChannel");
		if (onlineSettings != null)
		{
			onlineSettings.Back("You have been disconnected\nfrom the server.");
		}
		doJoinCh = false;
	}

	private void OnNetworkPlayerJoin(Player player)
	{
		Debug.Log("OnNetworkPlayerJoin: " + player.name + " (" + player.id + ")");
		if (onlineSettings != null)
		{
			onlineSettings.PlayerJoin(player.id);
		}
	}

	private void OnNetworkPlayerLeave(Player player)
	{
		Debug.Log("OnNetworkPlayerLeave: " + ((player != null) ? (player.name + " (" + player.id + ")") : "null"));
		if (player == null)
		{
			return;
		}
		if (onlineSettings != null)
		{
			onlineSettings.PlayerLeave(player.id);
		}
		for (int i = 0; i < 8; i++)
		{
			if (Global.onlinePlayerIDs[i] == player.id)
			{
				Global.onlinePlayerOns[i] = false;
				break;
			}
		}
	}

	private void OnNetworkError(string error)
	{
		Debug.Log("OnNetworkError: " + error);
		PopUpMessage(error);
	}

	private void OnPing(IPEndPoint ip, int milliSeconds)
	{
		Debug.Log(string.Concat(ip, " ping: ", milliSeconds, " ms"));
		for (int num = pingAddr.Count - 1; num >= 0; num--)
		{
			if (pingAddr[num].Address.Equals(ip.Address))
			{
				pingList[num] = milliSeconds;
				return;
			}
		}
		pingAddr.Add(ip);
		pingList.Add(milliSeconds);
	}

	private void Refresh()
	{
		Debug.Log("LobbyRoom: Refresh!");
		if (TNLobbyClient.isActive)
		{
			svList = new List<ServerList.Entry>();
			svPing = new List<int>();
			List<ServerList.Entry> list = TNLobbyClient.knownServers.list;
			string value = searchWord.ToLower();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				ServerList.Entry entry = list[num];
				if (IsServerOpen(entry.name) && (searchWord.Length <= 0 || GetServerName(entry.name).ToLower().Contains(value)))
				{
					int num2 = 1000000;
					if (Global.onlineWAN)
					{
						for (int num3 = pingAddr.Count - 1; num3 >= 0; num3--)
						{
							if (pingAddr[num3].Address.Equals(entry.externalAddress.Address))
							{
								num2 = pingList[num3];
								break;
							}
						}
					}
					else
					{
						for (int num4 = pingAddr.Count - 1; num4 >= 0; num4--)
						{
							if (pingAddr[num4].Address.Equals(entry.internalAddress.Address))
							{
								num2 = pingList[num4];
								break;
							}
						}
					}
					for (int i = 0; i < svList.Count; i++)
					{
						if (CompareServer(entry, svList[i], num2, svPing[i]))
						{
							svList.Insert(i, entry);
							svPing.Insert(i, num2);
							entry = null;
							break;
						}
					}
					if (entry != null)
					{
						svList.Add(entry);
						svPing.Add(num2);
					}
				}
			}
		}
		else
		{
			svList = null;
			svPing = null;
		}
		maxPage = ((svList == null) ? 1 : Mathf.CeilToInt((float)svList.Count / 6f));
		if (maxPage == 0)
		{
			maxPage = 1;
		}
		if (curPage >= maxPage)
		{
			curPage = maxPage - 1;
		}
		txtPage.text = curPage + 1 + "/" + maxPage;
		Debug.Log("total: " + ((svList != null) ? (svList.Count + " " + (curPage + 1) + "/" + maxPage) : "0"));
		if (curPage == 0)
		{
			prvPage.GetComponent<Collider>().enabled = false;
			TextureUtility.SetSpriteIndex(prvPage, 4, 1, 3);
		}
		else
		{
			prvPage.GetComponent<Collider>().enabled = true;
			TextureUtility.SetSpriteIndex(prvPage, 4, 1, 0);
		}
		if (curPage == maxPage - 1)
		{
			nxtPage.GetComponent<Collider>().enabled = false;
			TextureUtility.SetSpriteIndex(nxtPage, 4, 1, 3);
		}
		else
		{
			nxtPage.GetComponent<Collider>().enabled = true;
			TextureUtility.SetSpriteIndex(nxtPage, 4, 1, 0);
		}
		svJoin = null;
		barSelect.SetActive(false);
		btnJoin.GetComponent<Collider>().enabled = false;
		TextureUtility.SetSpriteIndex(btnJoin, 4, 1, 3);
		UpdateRooms();
	}

	private bool CompareServer(ServerList.Entry ent1, ServerList.Entry ent2, int ping1, int ping2)
	{
		bool[] maps = new bool[5];
		bool[] options = new bool[4];
		bool open;
		int win;
		int time;
		int numPlayer;
		int maxPlayer;
		string serverDetail = GetServerDetail(ent1.name, out open, ref maps, out win, out time, ref options, out numPlayer, out maxPlayer);
		bool[] maps2 = new bool[5];
		bool[] options2 = new bool[4];
		bool open2;
		int win2;
		int time2;
		int numPlayer2;
		int maxPlayer2;
		string serverDetail2 = GetServerDetail(ent2.name, out open2, ref maps2, out win2, out time2, ref options2, out numPlayer2, out maxPlayer2);
		for (int i = 0; i < 10; i++)
		{
			switch (sortKey[i])
			{
			case SortRoomKey.bySlot:
			{
				int num3 = maxPlayer - numPlayer;
				if (num3 == 0)
				{
					num3 = 8;
				}
				int num4 = maxPlayer2 - numPlayer2;
				if (num4 == 0)
				{
					num4 = 8;
				}
				if (num3 != num4)
				{
					if (sortVal[i] > 0)
					{
						return num3 < num4;
					}
					return num3 > num4;
				}
				break;
			}
			case SortRoomKey.byPing:
				if (ping1 != ping2)
				{
					if (sortVal[i] > 0)
					{
						return ping1 < ping2;
					}
					return ping1 > ping2;
				}
				break;
			case SortRoomKey.byName:
				if (serverDetail != serverDetail2)
				{
					if (sortVal[i] > 0)
					{
						return serverDetail.CompareTo(serverDetail2) < 0;
					}
					return serverDetail.CompareTo(serverDetail2) > 0;
				}
				break;
			case SortRoomKey.byWin:
				if (win != win2)
				{
					if (sortVal[i] > 0)
					{
						return win < win2;
					}
					return win > win2;
				}
				break;
			case SortRoomKey.byTime:
				if (time != time2)
				{
					if (sortVal[i] > 0)
					{
						return time < time2;
					}
					return time > time2;
				}
				break;
			case SortRoomKey.byOptn0:
			case SortRoomKey.byOptn1:
			case SortRoomKey.byOptn2:
			case SortRoomKey.byOptn3:
			{
				int num5 = (int)(sortKey[i] - 5);
				if (options[num5] != options2[num5])
				{
					if (sortVal[i] > 0)
					{
						return options[num5];
					}
					return options2[num5];
				}
				break;
			}
			case SortRoomKey.byMaps:
			{
				int num = 5;
				int num2 = 5;
				for (int j = 0; j < 5; j++)
				{
					if (maps[j])
					{
						num--;
					}
					if (maps2[j])
					{
						num2--;
					}
				}
				if (num != num2)
				{
					if (sortVal[i] > 0)
					{
						return num < num2;
					}
					return num > num2;
				}
				break;
			}
			}
		}
		return false;
	}

	private void UpdateRooms()
	{
		bool[] maps = new bool[5];
		bool[] options = new bool[4];
		int num = curPage * 6;
		int num2 = ((svList != null) ? ((num + 6 <= svList.Count) ? 6 : (svList.Count - num)) : 0);
		Debug.Log("show " + num + "/" + num2);
		for (int i = 0; i < num2; i++)
		{
			rooms[i].gameObject.SetActive(true);
			string formatName = svList[num + i].name;
			bool open;
			int win;
			int time;
			int numPlayer;
			int maxPlayer;
			string serverDetail = GetServerDetail(formatName, out open, ref maps, out win, out time, ref options, out numPlayer, out maxPlayer);
			string text = numPlayer + "/" + maxPlayer;
			TextMesh[] componentsInChildren = rooms[i].Find("number").GetComponentsInChildren<TextMesh>();
			foreach (TextMesh textMesh in componentsInChildren)
			{
				textMesh.text = text;
			}
			text = serverDetail;
			TextMesh[] componentsInChildren2 = rooms[i].Find("name").GetComponentsInChildren<TextMesh>();
			foreach (TextMesh textMesh2 in componentsInChildren2)
			{
				textMesh2.text = text;
			}
			text = ((svPing[num + i] >= 10000) ? "- ms" : (svPing[num + i] + " ms"));
			TextMesh[] componentsInChildren3 = rooms[i].Find("ping").GetComponentsInChildren<TextMesh>();
			foreach (TextMesh textMesh3 in componentsInChildren3)
			{
				textMesh3.text = text;
			}
			Transform transform = rooms[i].Find("maps");
			int num3 = 0;
			for (int num4 = 4; num4 >= 0; num4--)
			{
				Transform transform2 = transform.Find("map_" + num4);
				transform2.gameObject.SetActive(maps[num4]);
				if (maps[num4])
				{
					transform2.localPosition = new Vector3(-9 * num3, 0f, 0f);
					num3++;
					transform2.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)num4 * 0.2f, 0f));
				}
			}
			rooms[i].Find("option_0").GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)(win - 1) * 0.2f, 0.5f));
			rooms[i].Find("option_1").GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)(time - 1) * 0.2f, 0f));
			for (int m = 0; m < 4; m++)
			{
				rooms[i].Find("setting_" + m).GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)m * 0.25f, (!options[m]) ? 0f : 0.5f));
			}
		}
		for (int n = num2; n < 6; n++)
		{
			rooms[n].gameObject.SetActive(false);
		}
	}

	private void SetSort(int index)
	{
		if (sortKey[0] == (SortRoomKey)index)
		{
			sortVal[0] *= -1;
			Vector3 localScale = sortBtn[index].localScale;
			localScale.y = Mathf.Abs(localScale.y) * (float)sortVal[0];
			sortBtn[index].localScale = localScale;
		}
		else
		{
			sortBtn[(int)sortKey[0]].GetComponent<Renderer>().material = sortMat;
			sortBtn[index].GetComponent<Renderer>().material = sortAct;
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < 10; i++)
			{
				if (sortKey[i] == (SortRoomKey)index)
				{
					num = i;
					num2 = sortVal[i];
					break;
				}
			}
			for (int num3 = num; num3 > 0; num3--)
			{
				sortKey[num3] = sortKey[num3 - 1];
				sortVal[num3] = sortVal[num3 - 1];
			}
			sortKey[0] = (SortRoomKey)index;
			sortVal[0] = num2;
		}
		Refresh();
	}

	public static string ValidatePlayerName(string playerName)
	{
		playerName = Regex.Replace(playerName.Trim(), "\\s+", " ");
		if (playerName.Length > 8)
		{
			playerName = playerName.Substring(0, 8);
		}
		return playerName;
	}

	public static string ValidateRoomName(string roomName)
	{
		roomName = Regex.Replace(roomName.Trim(), "\\s+", " ");
		if (roomName.Length > 12)
		{
			roomName = roomName.Substring(0, 12);
		}
		return roomName;
	}

	public static string ValidateIPAddress(string ipAddress)
	{
		ipAddress = ipAddress.Trim();
		for (int num = ipAddress.Length - 1; num >= 0; num--)
		{
			if (ipAddress[num] != '.' && (ipAddress[num] < '0' || ipAddress[num] > '9'))
			{
				ipAddress = ipAddress.Remove(num, 1);
			}
		}
		return ipAddress;
	}

	public static string GetFormattedServerName(string serverName, bool open = false, bool[] maps = null, int win = 3, int time = 3, bool[] options = null, int numPlayer = 1, int maxPlayer = 8)
	{
		string text = ((!open) ? "0|" : "1|");
		if (maps == null)
		{
			text += "11111";
		}
		else
		{
			for (int i = 0; i < 5; i++)
			{
				text += ((!maps[i]) ? "0" : "1");
			}
		}
		text += "|";
		text += win;
		text += time;
		text += "|";
		if (options == null)
		{
			text += "1111";
		}
		else
		{
			for (int j = 0; j < 4; j++)
			{
				text += ((!options[j]) ? "0" : "1");
			}
		}
		text += "|";
		text += numPlayer;
		text += maxPlayer;
		text += "|";
		if (TNServerInstance.upnp == null || TNServerInstance.upnp.status == UPnP.Status.Failure)
		{
			return text + serverName + "\t";
		}
		return text + serverName;
	}

	public static string GetServerName(string formatName)
	{
		string text = formatName.Substring(19);
		return (!text.EndsWith("\t")) ? text : text.Substring(0, text.Length - 1);
	}

	public static string GetServerDetail(string formatName, out bool open, ref bool[] maps, out int win, out int time, ref bool[] options, out int numPlayer, out int maxPlayer)
	{
		open = formatName[0] != '0';
		for (int i = 0; i < 5; i++)
		{
			maps[i] = formatName[2 + i] != '0';
		}
		win = formatName[8] - 48;
		time = formatName[9] - 48;
		for (int j = 0; j < 4; j++)
		{
			options[j] = formatName[11 + j] != '0';
		}
		numPlayer = formatName[16] - 48;
		maxPlayer = formatName[17] - 48;
		string text = formatName.Substring(19);
		return (!text.EndsWith("\t")) ? text : text.Substring(0, text.Length - 1);
	}

	public static bool IsServer(string formatName)
	{
		return (formatName[0] == '0' || formatName[0] == '1') && formatName[1] == '|';
	}

	public static bool IsServerOpen(string formatName)
	{
		return IsServer(formatName) && formatName[0] != '0';
	}

	public static bool IsServerFail(string formatName)
	{
		return IsServer(formatName) && formatName.EndsWith("\t");
	}

	[RFC(100)]
	private void OnlineBattleServerListReject(int id)
	{
		if (TNManager.playerID == id)
		{
			TNManager.Disconnect();
		}
	}
}
