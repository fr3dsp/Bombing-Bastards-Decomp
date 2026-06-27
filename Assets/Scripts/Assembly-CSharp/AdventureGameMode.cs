using System;
using TNet;
using UnityEngine;

public class AdventureGameMode : MonoBehaviour
{
	public static int[] shownText;

	public TextAsset TextFile;

	public float speed = 1f;

	public float DurationTimePopup = 5f;

	private float currentTimePopup;

	private float maxTimePopup;

	private float[] pTime = new float[5];

	private string nameItemVOV;

	private float waitItemVOV;

	private GameObject[] togButton;

	private GameObject[] player;

	private bool[] mouseOver = new bool[6];

	private int activeID = -1;

	private int[] lostBuff;

	private bool[] activePlayer;

	private int lostCtrl = -1;

	public int stage = 2;

	private Transform PopupGUI;

	private TextMesh[] line;

	private float lineOffset;

	private int lineNum;

	private int step;

	private string[] textList;

	private List<int> queueText;

	private Transform TimerGUI;

	private Transform[] TimeUI;

	public bool timeStart;

	public static AdventureGameMode UI;

	private GameObject pauseSystem;

	private float pauseFadeSpeed;

	private float pauseFadeValue;

	private int pauseFadeSign;

	private Material pauseFade;

	private Transform pauseGUI;

	private TextMesh pauseText;

	public int pauseRequest;

	private GameObject[] pauseBtn;

	private int pauseTimeScale;

	private GameObject pauseDisconnect;

	private float confiFadeSpeed;

	private float confiFadeValue;

	private int confiFadeSign;

	private Transform confiGUI;

	private TextMesh confiText;

	private int confiRequest;

	private GameObject[] confiBtn;

	private int[] pointerInit = new int[5];

	private float firstStart = 1.5f;

	private Vector3 rayDirection = new Vector3(0f, -0.866f, 0.5f);

	private Vector3 touchPos;

	private bool isPress;

	private bool mouseFocus;

	private float pointerSpeed = Global.pointerSpd;

	private bool showController
	{
		get
		{
			return (player[0].transform.localScale.x != 0f) ? true : false;
		}
		set
		{
			int num = ((Global.Mode == GameMode.OnlineBattle) ? 1 : 5);
			for (int i = 0; i < num; i++)
			{
				int num2 = 0;
				GameObject gameObject;
				if (Global.Mode == GameMode.Adventure || Global.Mode == GameMode.OnlineBattle)
				{
					gameObject = player[5];
				}
				else
				{
					gameObject = player[i];
					num2 = i;
				}
				if (!value)
				{
					gameObject.transform.localScale = Vector3.zero;
					gameObject.transform.localPosition = new Vector3(2000f, 0f, 1900f);
					pointerInit[num2] = 0;
				}
				else
				{
					gameObject.transform.localScale = new Vector3(180f, 180f, 1f);
				}
			}
		}
	}

	private void Awake()
	{
		activeID = 0;
		UI = this;
		TextAsset textFile = TextFile;
		textList = textFile.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		if (shownText == null)
		{
			shownText = new int[textList.Length];
		}
		else if (Global.IsIntroOn)
		{
			for (int num = shownText.Length - 1; num >= 0; num--)
			{
				if (shownText[num] > 1)
				{
					shownText[num] = 1;
				}
			}
		}
		if (Application.loadedLevelName == "Init")
		{
			base.enabled = false;
			return;
		}
		PopupGUI = base.transform.Find("GUI").Find("PopupGUI");
		PopupGUI.localPosition = new Vector3(0f, -400f, 0f);
		TimerGUI = base.transform.Find("GUI").Find("TimerGUI");
		TimerGUI.localPosition = new Vector3(0f, 1150f, 0f);
		line = PopupGUI.GetComponentsInChildren<TextMesh>();
		lineOffset = -100000f;
		TextMesh[] array = line;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = string.Empty;
			if (lineOffset < textMesh.transform.localPosition.y)
			{
				lineOffset = textMesh.transform.localPosition.y;
			}
		}
		TimeUI = new Transform[4];
		for (int j = 0; j < 4; j++)
		{
			TimeUI[j] = TimerGUI.Find("time/time" + j);
		}
		if (Global.IsBossStage)
		{
			Transform transform = TimerGUI.Find("time");
			Vector3 localPosition = transform.localPosition;
			localPosition.x = -1080f * ((float)Screen.width * 0.975f) / (float)Screen.height;
			transform.localPosition = localPosition;
			TimerGUI.Find("bg_timer").gameObject.SetActive(false);
			TimerGUI.Find("ui_timeGlow").gameObject.SetActive(false);
		}
		stage = Global.advStage;
		queueText = new List<int>();
		pauseFadeValue = 0f;
		pauseFadeSign = 0;
		pauseSystem = base.transform.Find("GUI/PauseGUI").gameObject;
		pauseFade = base.transform.Find("GUI/PauseGUI/bg").GetComponent<Renderer>().material;
		pauseGUI = base.transform.Find("GUI/PauseGUI/dialog");
		pauseGUI.localScale = Vector3.zero;
		pauseBtn = new GameObject[4];
		pauseBtn[0] = pauseGUI.Find("btn_menu").gameObject;
		pauseBtn[1] = pauseGUI.Find("btn_replay").gameObject;
		pauseBtn[2] = pauseGUI.Find("btn_resume").gameObject;
		pauseBtn[3] = pauseGUI.Find("btn_okDC").gameObject;
		pauseBtn[3].SetActive(false);
		Transform transform2 = pauseGUI.Find("title");
		Transform transform3 = pauseGUI.Find("text");
		pauseText = transform3.GetComponent<TextMesh>();
		if (Global.Mode == GameMode.Adventure)
		{
			transform2.localPosition = (transform2.localPosition * 2f + transform3.localPosition) / 3f;
			pauseGUI.Find("box_b").gameObject.SetActive(false);
			transform3.gameObject.SetActive(false);
		}
		else
		{
			pauseGUI.Find("box_a").gameObject.SetActive(false);
			pauseBtn[1].SetActive(false);
		}
		pauseDisconnect = pauseGUI.Find("title_dc").gameObject;
		pauseDisconnect.SetActive(false);
		confiFadeValue = 0f;
		confiFadeSign = 0;
		confiGUI = base.transform.Find("GUI/PauseGUI/confirm");
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("ui/btn_ok").gameObject;
		confiBtn[1] = confiGUI.Find("ui/btn_cancel").gameObject;
		confiText = confiGUI.Find("ui/text").GetComponent<TextMesh>();
		pauseSystem.SetActive(false);
		if (Global.Mode == GameMode.Adventure)
		{
			GameSound.PreloadVOV("levelIntro_" + Global.advStage.ToString("D2"));
		}
	}

	private void Start()
	{
		togButton = new GameObject[6];
		player = new GameObject[6];
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("pointer"))
			{
				int num = int.Parse(transform.transform.GetChild(0).name.Substring(transform.transform.GetChild(0).name.Length - 1));
				if (num < player.Length)
				{
					player[num] = transform.gameObject;
				}
			}
			else if (transform.name.Contains("s_point"))
			{
				player[5] = transform.gameObject;
			}
		}
	}

	private void OnDestroy()
	{
		UI = null;
		for (int i = 1; i <= 30; i++)
		{
			GameSound.StopVOV("levelIntro_" + i.ToString("D2"));
		}
	}

	private void ValidateFadeColor()
	{
		pauseFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Max(confiFadeValue / 4f, pauseFadeValue / 4f)));
	}

	private void Update()
	{
		if (waitItemVOV > 0f)
		{
			waitItemVOV -= Time.deltaTime;
			if (waitItemVOV <= 0f)
			{
				waitItemVOV = 0f;
				GameSound.StartVOV(nameItemVOV);
			}
		}
		if (firstStart > 0f && Global.Mode == GameMode.Adventure)
		{
			firstStart -= Time.deltaTime;
			if (firstStart <= 0f)
			{
				Popup(20 + Global.advStage - 1);
			}
		}
		if (queueText.Count > 0)
		{
			animationPopup();
		}
		int num = ((Global.Mode == GameMode.OnlineBattle) ? 1 : 5);
		for (int i = 0; i < num; i++)
		{
			GameController controller = GameInput.GetController((Global.Mode != GameMode.OnlineBattle) ? i : Global.onlinePlayerSlot);
			if (!controller.IsConnected() || (!controller.DoSelectMenu() && (Global.isFocus || Global.Mode == GameMode.OnlineBattle || pauseSystem.activeSelf)) || pauseFadeSign != 0 || Global.Map.IsFade)
			{
				continue;
			}
			if (confiFadeValue == 1f || confiFadeSign != 0)
			{
				confiFadeSign = -1;
				GameRemoteController.IgnorePlayerAction = false;
				if (Global.Mode == GameMode.OnlineBattle)
				{
					showController = false;
				}
			}
			else if (pauseFadeValue == 0f)
			{
				pauseSystem.SetActive(true);
				if (Global.Mode == GameMode.OnlineBattle)
				{
					GameRemoteController.IgnorePlayerAction = true;
					confiRequest = -1;
					confiFadeSign = 1;
					confiFadeSpeed = 5f * Time.smoothDeltaTime;
					confiText.text = "Do you want to quit this game?";
					Time.timeScale = 1f;
					pauseTimeScale = 0;
					showController = true;
					for (int j = 0; j < 3; j++)
					{
						pauseBtn[j].GetComponent<Collider>().enabled = false;
					}
					confiGUI.gameObject.SetActive(true);
					break;
				}
				pauseFadeSign = 1;
				pauseFadeSpeed = 0.2f;
				Time.timeScale = 0f;
				pauseTimeScale = 0;
				showController = true;
				foreach (GameCharacter item in GameCharacter.List())
				{
					if (item.Type == Character.Player)
					{
						item.enabled = false;
					}
				}
				if (Global.Mode == GameMode.LocalBattle)
				{
					pauseText.text = "The game has paused.";
				}
			}
			else if (pauseFadeValue == 1f && (Global.Mode != GameMode.OnlineBattle || TNManager.isConnected))
			{
				pauseFadeSign = -1;
				showController = false;
			}
			break;
		}
		if (lostCtrl > -1)
		{
			pauseSystem.SetActive(true);
			pauseFadeSign = 1;
			pauseFadeSpeed = 0.2f;
			Time.timeScale = 0f;
			pauseTimeScale = 0;
			showController = true;
			foreach (GameCharacter item2 in GameCharacter.List())
			{
				if (item2.Type == Character.Player)
				{
					item2.enabled = false;
				}
			}
			int num2 = 0;
			num2 = ((lostCtrl != 4) ? (lostCtrl + 1) : 0);
			if (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle)
			{
				pauseText.text = "The game has paused.";
			}
			confiRequest = 2 + lostCtrl;
			confiFadeSpeed = pauseFadeSpeed;
			confiFadeSign = 1;
			confiText.text = "\n\n Communications with the\n" + Global.localPlayerName[num2] + " have been interrupted.\n\nPress the A Button.";
			for (int k = 0; k < 3; k++)
			{
				pauseBtn[k].GetComponent<Collider>().enabled = false;
			}
			for (int l = 0; l < 2; l++)
			{
				confiBtn[l].gameObject.SetActive(false);
			}
			confiGUI.gameObject.SetActive(true);
		}
		if (confiFadeSign != 0)
		{
			if (confiFadeSign > 0)
			{
				confiFadeValue += ((Global.Mode != GameMode.OnlineBattle) ? confiFadeSpeed : (5f * Time.deltaTime));
				if (confiFadeValue >= 1f)
				{
					confiFadeValue = 1f;
					confiFadeSign = 0;
				}
			}
			else
			{
				confiFadeValue -= ((Global.Mode != GameMode.OnlineBattle) ? confiFadeSpeed : (5f * Time.deltaTime));
				if (confiFadeValue <= 0f)
				{
					confiFadeValue = 0f;
					confiFadeSign = 0;
					for (int m = 0; m < 3; m++)
					{
						pauseBtn[m].GetComponent<Collider>().enabled = true;
					}
					confiGUI.gameObject.SetActive(false);
				}
			}
			confiGUI.localScale = new Vector3(confiFadeValue, confiFadeValue, confiFadeValue);
			ValidateFadeColor();
		}
		if (pauseFadeSign != 0)
		{
			if (pauseFadeSign > 0)
			{
				pauseFadeValue += ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (5f * Time.deltaTime));
				if (pauseFadeValue >= 1f)
				{
					pauseFadeValue = 1f;
					pauseFadeSign = 0;
				}
			}
			else
			{
				pauseFadeValue -= ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (5f * Time.deltaTime));
				if (pauseFadeValue <= 0f)
				{
					pauseFadeValue = 0f;
					pauseFadeSign = 0;
					Time.timeScale = 1f;
					pauseTimeScale = 1;
					if (Global.Mode == GameMode.OnlineBattle)
					{
						GameRemoteController.IgnorePlayerAction = false;
						showController = false;
					}
					else
					{
						foreach (GameCharacter item3 in GameCharacter.List())
						{
							if (item3.Type == Character.Player)
							{
								item3.enabled = true;
							}
						}
					}
					pauseSystem.SetActive(false);
				}
			}
			pauseGUI.localScale = new Vector3(pauseFadeValue, pauseFadeValue, pauseFadeValue);
			ValidateFadeColor();
		}
		if (pauseTimeScale == 0 && (pauseFadeValue == 1f || confiFadeValue == 1f))
		{
			playerInput();
			pointerControl();
			if (pauseFadeSign != 0)
			{
				PointerInit();
			}
		}
	}

	private void playerInput()
	{
		if (Global.Map.IsFade)
		{
			return;
		}
		int num = ((Global.Mode == GameMode.OnlineBattle) ? 1 : 5);
		RaycastHit[] array = new RaycastHit[num];
		for (int i = 0; i < num; i++)
		{
			GameController controller = GameInput.GetController((Global.Mode != GameMode.OnlineBattle) ? i : Global.onlinePlayerSlot);
			if (!controller.IsConnected())
			{
				continue;
			}
			Vector3 zero = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Vector3.zero);
			int num2 = 512;
			int num3 = 0;
			zero = player[i].transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Global.Mode == GameMode.Adventure || Global.Mode == GameMode.OnlineBattle)
			{
				zero = player[5].transform.GetChild(0).position;
			}
			if (Physics.Raycast(ray, out array[i], num2) && i == num3 && !mouseFocus)
			{
				mouseFocus = true;
				pTime[0] = 10f;
			}
			if (Physics.Raycast(zero, rayDirection, out array[i], num2) || (Physics.Raycast(ray, out array[i], num2) && i == num3 && pointerInit[0] == 0))
			{
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					mouseOver[i] = false;
					controller.DoPadRelease(i);
					idleButton(togButton[i]);
					mouseFocus = false;
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (!array[i].transform.name.Contains("btn"))
				{
					continue;
				}
				togButton[i] = array[i].transform.gameObject;
				if (!isPress)
				{
					TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 1);
				}
				if (controller.DoEnterHold("Fire1"))
				{
					TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 2);
					controller.DoPadPress(i);
					if (togButton[i].name == "btn_ok" || togButton[i].name == "btn_cancel")
					{
						Transform child = togButton[i].transform.GetChild(0);
						Vector3 localPosition = child.localPosition;
						if (localPosition.x == 0f && localPosition.y == 0f)
						{
							localPosition.x = -0.03f;
							localPosition.y = -0.06f;
							child.localPosition = localPosition;
						}
					}
					isPress = true;
				}
				else
				{
					if (!controller.DoEnterRelease("Fire1") || !isPress)
					{
						continue;
					}
					touchPos = Vector3.zero;
					isPress = false;
					controller.DoPadRelease(i);
					MonoBehaviour.print("click at : " + togButton[i].name);
					if (togButton[i].name == "btn_ok" || togButton[i].name == "btn_cancel")
					{
						Transform child2 = togButton[i].transform.GetChild(0);
						Vector3 localPosition2 = child2.localPosition;
						if (localPosition2.x != 0f || localPosition2.y != 0f)
						{
							localPosition2.x = 0f;
							localPosition2.y = 0f;
							child2.localPosition = localPosition2;
						}
					}
					if (togButton[i].name == "btn_ok")
					{
						pauseRequest = confiRequest;
						Time.timeScale = 1f;
						pauseTimeScale = 1;
						if (Global.Mode == GameMode.OnlineBattle)
						{
							GameRemoteController.IgnorePlayerAction = false;
							showController = false;
						}
						else
						{
							foreach (GameCharacter item in GameCharacter.List())
							{
								if (item.Type == Character.Player)
								{
									item.enabled = true;
								}
							}
						}
					}
					else if (togButton[i].name == "btn_cancel")
					{
						confiFadeSign = -1;
						GameRemoteController.IgnorePlayerAction = false;
						showController = false;
					}
					else if (togButton[i].name == "btn_menu")
					{
						confiRequest = -1;
						confiFadeSpeed = pauseFadeSpeed;
						confiFadeSign = 1;
						confiText.text = "Do you want to quit this game?";
						for (int j = 0; j < 2; j++)
						{
							confiBtn[j].gameObject.SetActive(true);
						}
						for (int k = 0; k < 3; k++)
						{
							pauseBtn[k].GetComponent<Collider>().enabled = false;
						}
						confiGUI.gameObject.SetActive(true);
					}
					else if (togButton[i].name == "btn_replay")
					{
						confiRequest = 1;
						confiFadeSpeed = pauseFadeSpeed;
						confiFadeSign = 1;
						confiText.text = "Do you want to restart this game?";
						for (int l = 0; l < 2; l++)
						{
							confiBtn[l].gameObject.SetActive(true);
						}
						for (int m = 0; m < 3; m++)
						{
							pauseBtn[m].GetComponent<Collider>().enabled = false;
						}
						confiGUI.gameObject.SetActive(true);
					}
					else if (togButton[i].name == "btn_resume")
					{
						if (pauseFadeValue == 1f && pauseFadeSign == 0)
						{
							pauseFadeSign = -1;
							showController = false;
						}
					}
					else if (togButton[i].name == "btn_okDC")
					{
						pauseRequest = -1;
						Time.timeScale = 1f;
						pauseTimeScale = 1;
						GameRemoteController.IgnorePlayerAction = false;
						showController = false;
					}
					GameSound.StartSFX("menuSelect");
					idleButton(togButton[i]);
					togButton[i] = null;
					break;
				}
			}
			else if (togButton[i] != null)
			{
				controller.DoPadRelease(i);
				isPress = false;
				mouseOver[i] = false;
				idleButton(togButton[i]);
				togButton[i] = null;
			}
		}
	}

	private void idleButton(GameObject togButton)
	{
		if (togButton.name.Contains("btn"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		if (togButton.name == "btn_ok" || togButton.name == "btn_cancel")
		{
			Transform child = togButton.transform.GetChild(0);
			Vector3 localPosition = child.localPosition;
			if (localPosition.x != 0f || localPosition.y != 0f)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
				child.localPosition = localPosition;
			}
		}
	}

	private void PointerInit()
	{
	}

	private void pointerControl()
	{
		if (Global.Mode == GameMode.OnlineBattle)
		{
			GameController controller = GameInput.GetController(Global.onlinePlayerSlot);
			if (pointerInit[0] == 1)
			{
				pTime[0] += 0.02f;
			}
			if (controller.IsMouseMove())
			{
				pTime[0] = 100f;
				Cursor.visible = true;
			}
			StickPointer(controller, 0);
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (pointerInit[i] == 1)
			{
				pTime[i] += 0.02f;
			}
			GameController controller2 = GameInput.GetController(i);
			if (controller2.IsMouseMove())
			{
				pTime[0] = 100f;
				Cursor.visible = true;
			}
			StickPointer(controller2, i);
		}
	}

	private void StickPointer(GameController ctrl, int p)
	{
		int num = 0;
		GameObject gameObject;
		if (Global.Mode == GameMode.Adventure || Global.Mode == GameMode.OnlineBattle)
		{
			gameObject = player[5];
		}
		else
		{
			gameObject = player[p];
			num = p;
		}
		if (pTime[num] >= 10f)
		{
			pointerInit[num] = 0;
			pTime[num] = 0f;
			gameObject.transform.localPosition = new Vector3(2000f, 0f, 1900f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = gameObject.transform.localPosition;
			if (i == 1)
			{
				float num2 = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit[num] == 0 && num2 != 0f && localPosition.x > 1750f)
				{
					gameObject.transform.localPosition = new Vector3(0f, 0f, 1900f);
					pointerInit[num] = 1;
					showController = true;
					if (Global.Mode == GameMode.Adventure || Global.Mode == GameMode.OnlineBattle)
					{
						Cursor.visible = false;
					}
					break;
				}
				if (num2 > 0f)
				{
					if (localPosition.x >= -1683f)
					{
						gameObject.transform.localPosition = new Vector3(localPosition.x - ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (12f * Time.deltaTime)) * pointerSpeed * num2, localPosition.y, 1900f);
						pTime[num] = 0f;
					}
				}
				else if (num2 < 0f && localPosition.x <= 1457f)
				{
					gameObject.transform.localPosition = new Vector3(localPosition.x - ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (12f * Time.deltaTime)) * pointerSpeed * num2, localPosition.y, 1900f);
					pTime[num] = 0f;
				}
				continue;
			}
			float num3 = ctrl.GetVertical() * 1.25f;
			if (pointerInit[num] == 0 && num3 != 0f && localPosition.x > 1750f)
			{
				gameObject.transform.localPosition = new Vector3(0f, 0f, 1900f);
				pointerInit[num] = 1;
				showController = true;
				if (Global.Mode == GameMode.Adventure || Global.Mode == GameMode.OnlineBattle)
				{
					Cursor.visible = false;
				}
				break;
			}
			if (num3 < 0f)
			{
				if (localPosition.y <= 772f)
				{
					gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y - ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (12f * Time.deltaTime)) * pointerSpeed * num3, 1900f);
					pTime[num] = 0f;
				}
			}
			else if (num3 > 0f && localPosition.y >= -970f)
			{
				gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y - ((Global.Mode != GameMode.OnlineBattle) ? pauseFadeSpeed : (12f * Time.deltaTime)) * pointerSpeed * num3, 1900f);
				pTime[num] = 0f;
			}
		}
	}

	private void CheckController()
	{
	}

	private void animationPopup()
	{
		switch (step)
		{
		case 0:
		{
			string[] array = textList[queueText[0]].Split('#');
			if (queueText[0] >= 20)
			{
				lineNum = 3;
				string text = array[0];
				for (int i = 1; i < array.Length; i++)
				{
					text = text + "\n" + array[i];
				}
				line[0].text = string.Empty;
				line[1].text = string.Empty;
				line[2].text = text;
				step++;
				string text2 = "levelIntro_" + (queueText[0] - 20 + 1).ToString("D2");
				maxTimePopup = GameSound.GetLengthVOV(text2) + 1f;
				if (Global.IsVoiceOn)
				{
					GameSound.StartVOV(text2);
				}
				break;
			}
			lineNum = Mathf.Min(2, array.Length);
			for (int j = 0; j < lineNum; j++)
			{
				line[j].text = array[j];
			}
			for (int k = lineNum; k < 2; k++)
			{
				line[k].text = string.Empty;
			}
			step++;
			Vector3 localPosition3 = line[0].transform.localPosition;
			if (lineNum == 1)
			{
				localPosition3.y = (lineOffset + line[1].transform.localPosition.y) / 2f;
			}
			else
			{
				localPosition3.y = lineOffset;
			}
			line[0].transform.localPosition = localPosition3;
			line[2].text = string.Empty;
			maxTimePopup = DurationTimePopup * (float)lineNum;
			break;
		}
		case 1:
		{
			Vector3 localPosition2 = PopupGUI.localPosition;
			localPosition2.y += speed * Time.deltaTime * 400f;
			if (localPosition2.y >= 0f)
			{
				currentTimePopup = 0f;
				localPosition2.y = 0f;
				step++;
			}
			PopupGUI.localPosition = localPosition2;
			break;
		}
		case 2:
			currentTimePopup += Time.deltaTime;
			if (currentTimePopup >= maxTimePopup)
			{
				step++;
			}
			break;
		case 3:
		{
			Vector3 localPosition = PopupGUI.localPosition;
			localPosition.y -= speed * Time.deltaTime * 400f;
			if (localPosition.y <= -400f)
			{
				currentTimePopup = 0f;
				localPosition.y = -400f;
				step = 0;
				queueText.RemoveAt(0);
			}
			PopupGUI.localPosition = localPosition;
			break;
		}
		}
	}

	public void ShowTime(float currentTime)
	{
		if (timeStart)
		{
			Vector3 localPosition = TimerGUI.localPosition;
			localPosition.y -= speed * Time.deltaTime * 130f;
			if (localPosition.y <= 975f)
			{
				localPosition.y = 975f;
				timeStart = false;
			}
			TimerGUI.localPosition = localPosition;
		}
		int num = (int)(currentTime / 600f);
		int num2 = (int)(currentTime / 60f);
		int num3 = (int)currentTime - (num * 600 + num2 * 60);
		int num4 = num3 / 10;
		int num5 = num3 % 10;
		TimeUI[0].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)num / 11f, 0f));
		TimeUI[1].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)num2 / 11f, 0f));
		TimeUI[2].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)num4 / 11f, 0f));
		TimeUI[3].GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)num5 / 11f, 0f));
		if (currentTime <= 15f)
		{
			TimerGUI.Find("ui_timeGlow").GetComponent<SpriteTextureTutorial>().alpha_speed = 3f;
		}
		else if (currentTime <= 60f)
		{
			TimerGUI.Find("ui_timeGlow").GetComponent<Renderer>().enabled = true;
			TimerGUI.Find("ui_timeGlow").GetComponent<SpriteTextureTutorial>().alpha_speed = 1f;
		}
		else
		{
			TimerGUI.Find("ui_timeGlow").GetComponent<Renderer>().enabled = false;
		}
	}

	public int[] GetMonsterList()
	{
		if (Global.IsBossStage)
		{
			return null;
		}
		int[] array = new int[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == 0)
			{
				array[i] = 0;
			}
			else
			{
				array[i] = -1;
			}
		}
		switch (stage)
		{
		case 1:
			array = new int[8] { 0, 0, 0, 0, -1, -1, -1, -1 };
			break;
		case 2:
			array = new int[8] { 0, 0, 0, 0, 1, 1, -1, -1 };
			break;
		case 3:
			array = new int[8] { 0, 0, 0, 0, 1, 1, 1, -1 };
			break;
		case 4:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 2, -1 };
			break;
		case 5:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 2, 2 };
			break;
		case 7:
			array = new int[8] { 0, 0, 0, 0, 1, 1, 1, -1 };
			break;
		case 8:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 2, 2 };
			break;
		case 9:
			array = new int[8] { 0, 0, 1, 1, 1, 2, 2, 3 };
			break;
		case 10:
			array = new int[8] { 0, 0, 1, 1, 2, 2, 3, 3 };
			break;
		case 11:
			array = new int[8] { 0, 1, 2, 2, 3, 3, 3, 3 };
			break;
		case 13:
			array = new int[8] { 0, 0, 0, 0, 0, 1, 1, 1 };
			break;
		case 14:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 1, 2 };
			break;
		case 15:
			array = new int[8] { 0, 0, 0, 1, 1, 2, 2, 3 };
			break;
		case 16:
			array = new int[8] { 0, 0, 1, 1, 2, 2, 2, 3 };
			break;
		case 17:
			array = new int[8] { 0, 1, 2, 2, 2, 3, 3, 3 };
			break;
		case 19:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 2, -1 };
			break;
		case 20:
			array = new int[8] { 0, 0, 0, 0, 1, 1, 1, 2 };
			break;
		case 21:
			array = new int[8] { 0, 0, 0, 1, 1, 1, 2, 3 };
			break;
		case 22:
			array = new int[8] { 0, 0, 0, 1, 1, 2, 2, 3 };
			break;
		case 23:
			array = new int[8] { 0, 0, 1, 1, 2, 2, 3, 3 };
			break;
		case 25:
			array = new int[8] { 0, 0, 0, 1, 2, 2, -1, -1 };
			break;
		case 26:
			array = new int[8] { 0, 0, 0, 1, 2, 2, 3, -1 };
			break;
		case 27:
			array = new int[8] { 0, 0, 0, 1, 1, 2, 2, 3 };
			break;
		case 28:
			array = new int[8] { 0, 0, 1, 1, 2, 2, 3, 3 };
			break;
		case 29:
			array = new int[8] { 0, 0, 1, 1, 2, 3, 3, 3 };
			break;
		default:
			array = new int[8];
			break;
		}
		for (int j = 1; j < 8; j++)
		{
			int num = UnityEngine.Random.Range(j, 7);
			int num2 = array[j];
			array[j] = array[num];
			array[num] = num2;
		}
		return array;
	}

	public int[] GetItemList()
	{
		int[] array = new int[13];
		switch (stage)
		{
		case 0:
			return new int[13];
		case 1:
			return new int[13]
			{
				0, 16, 0, 16, 0, 0, 0, 0, 0, 0,
				5, 0, 0
			};
		case 2:
			return new int[13]
			{
				0, 10, 4, 10, 0, 8, 0, 0, 0, 0,
				5, 0, 0
			};
		case 3:
			return new int[13]
			{
				0, 10, 4, 10, 4, 5, 0, 0, 0, 0,
				5, 4, 0
			};
		case 4:
			return new int[13]
			{
				0, 10, 4, 10, 4, 5, 2, 0, 0, 4,
				5, 4, 0
			};
		case 5:
			return new int[13]
			{
				0, 10, 4, 10, 4, 5, 2, 4, 0, 4,
				5, 4, 0
			};
		case 7:
			return new int[13]
			{
				0, 10, 4, 10, 4, 5, 2, 4, 4, 4,
				5, 4, 0
			};
		case 25:
		case 26:
		case 27:
		case 28:
		case 29:
			return new int[13]
			{
				2, 10, 4, 10, 4, 5, 2, 4, 4, 4,
				10, 4, 1
			};
		default:
			return new int[13]
			{
				2, 10, 4, 10, 4, 5, 2, 4, 4, 4,
				5, 4, 1
			};
		}
	}

	public void Popup(int textID)
	{
		if (textID < 13 && Global.IsItemVOn)
		{
			nameItemVOV = "item" + (Item)textID;
			waitItemVOV = 0.3f;
		}
		if (shownText[textID] < 1 || (Global.IsIntroOn && shownText[textID] < 2))
		{
			shownText[textID] = ((!Global.IsIntroOn) ? 1 : 2);
			queueText.Add(textID);
		}
	}

	public void Disconnect()
	{
		pauseSystem.SetActive(true);
		pauseFadeSign = 1;
		pauseFadeSpeed = 5f * Time.smoothDeltaTime;
		GameRemoteController.IgnorePlayerAction = true;
		Time.timeScale = 1f;
		pauseTimeScale = 0;
		showController = true;
		pauseText.text = "You have been disconnected\nfrom the server.";
		pauseBtn[0].SetActive(false);
		pauseBtn[2].SetActive(false);
		pauseBtn[3].SetActive(true);
		pauseGUI.Find("title").gameObject.SetActive(false);
		pauseDisconnect.SetActive(true);
		pauseGUI.localPosition = new Vector3(0f, 0f, 200f);
		for (int i = 0; i < 2; i++)
		{
			confiBtn[i].GetComponent<Collider>().enabled = false;
		}
	}
}
