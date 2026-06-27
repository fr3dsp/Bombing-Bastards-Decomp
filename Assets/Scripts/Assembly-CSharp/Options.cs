using UnityEngine;

public class Options : MonoBehaviour
{
	private enum State
	{
		fadeIn = 0,
		initial = 1,
		playerInput = 2,
		nextScene = 3,
		backScene = 4
	}

	private Transform[] toggleBtn;

	private GameObject player;

	private GameObject creditBtn;

	private GameObject backBtn;

	private GameObject resetBtn;

	private GameObject controlBtn;

	private GameObject displayBtn;

	private State state;

	private GameObject[] togButton;

	public int[] status;

	private float fadeTime;

	private bool[] mouseOver = new bool[5];

	public float speed = 45.5f;

	private float pointerSpeed = Global.pointerSpd;

	private int nextScene;

	private float pTime;

	private int pointerInit;

	private float popupFadeSpeed;

	private float popupFadeValue;

	private int popupFadeSign;

	private Material popupFade;

	private int popupRequest;

	private GameObject confiSys;

	private Transform confiGUI;

	private GameObject[] confiBtn;

	private Transform disSetting;

	private Transform resBar;

	private Transform quaBar;

	private GameObject fullBtn;

	private GameObject windowBtn;

	private GameObject resBtn;

	private GameObject quaBtn;

	private GameObject resMenu;

	private GameObject quaMenu;

	private GameObject[] resList;

	private GameObject[] quaList;

	private int quaIndex;

	private int resIndex;

	private int quaPos;

	private int resPos;

	private float quaStep;

	private float resStep;

	private Vector3 touchPos;

	private bool isPress;

	private bool mouseFocus;

	private int maxres = 5;

	private int maxqua = 4;

	private float[] rescap = new float[2];

	private void Awake()
	{
		popupFadeValue = 0f;
		popupFadeSign = 0;
		popupFade = GameObject.Find("confirm/bg").GetComponent<Renderer>().material;
		confiGUI = GameObject.Find("confirm/ui").transform;
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("btn_cf_ok").gameObject;
		confiBtn[1] = confiGUI.Find("btn_cf_cancel").gameObject;
		confiSys = confiGUI.parent.gameObject;
		confiSys.SetActive(false);
		disSetting = GameObject.Find("displaysetting/ui").transform;
		fullBtn = disSetting.Find("frame_gs/tog_gs_full").gameObject;
		windowBtn = disSetting.Find("frame_gs/tog_gs_window").gameObject;
		resBtn = disSetting.Find("frame_gs/bg_gs_dropscr/btn_gs_dropscr").gameObject;
		quaBtn = disSetting.Find("frame_gs/bg_gs_dropqua/btn_gs_dropqua").gameObject;
		resMenu = disSetting.Find("frame_gs/bg_gs_dropscr/gs_scrdroped").gameObject;
		quaMenu = disSetting.Find("frame_gs/bg_gs_dropqua/gs_quadroped").gameObject;
		resBar = resMenu.transform.Find("gs_scrtabbg/gs_scrtab");
		quaBar = quaMenu.transform.Find("gs_quatabbg/gs_quatab");
		TabInit();
		disSetting.parent.gameObject.SetActive(false);
		disSetting.localScale = Vector3.zero;
		if (Screen.fullScreen)
		{
			TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
			TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 3);
		}
		else
		{
			TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 0);
			TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 3);
		}
		Global.FullScreen = Screen.fullScreen;
		resBtn.transform.parent.Find("gs_seltxtscr").GetComponent<TextMesh>().text = GameSetting.restlist[Global.Resolution];
		quaBtn.transform.parent.Find("gs_seltxtqua").GetComponent<TextMesh>().text = GameSetting.quatlist[Global.Quality];
	}

	private void Start()
	{
		toggleBtn = new Transform[5];
		togButton = new GameObject[5];
		status = new int[5];
		state = State.initial;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("switch"))
			{
				int num = int.Parse(transform.transform.name.Substring(transform.transform.name.Length - 1));
				toggleBtn[num - 1] = transform;
			}
			else if (transform.name == "btn_back")
			{
				backBtn = transform.gameObject;
			}
			else if (transform.name == "btn_credit")
			{
				creditBtn = transform.gameObject;
			}
			else if (transform.name == "btn_reset")
			{
				resetBtn = transform.gameObject;
			}
			else if (transform.name == "btn_control")
			{
				controlBtn = transform.gameObject;
			}
			else if (transform.name == "btn_display")
			{
				displayBtn = transform.gameObject;
			}
			else if (transform.name.Contains("p_pointer"))
			{
				player = transform.gameObject;
			}
		}
	}

	private void Update()
	{
		if (popupFadeSign != 0)
		{
			if (popupFadeSign > 0)
			{
				popupFadeValue += popupFadeSpeed;
				if (popupFadeValue >= 1f)
				{
					popupFadeValue = 1f;
					popupFadeSign = 0;
				}
			}
			else
			{
				popupFadeValue -= popupFadeSpeed;
				if (popupFadeValue <= 0f)
				{
					popupFadeValue = 0f;
					popupFadeSign = 0;
					creditBtn.GetComponent<Collider>().enabled = true;
					backBtn.GetComponent<Collider>().enabled = true;
					resetBtn.GetComponent<Collider>().enabled = true;
					controlBtn.GetComponent<Collider>().enabled = true;
					displayBtn.GetComponent<Collider>().enabled = true;
					if (popupRequest == 1)
					{
						confiSys.SetActive(false);
					}
					else
					{
						disSetting.parent.gameObject.SetActive(false);
					}
					popupRequest = 0;
				}
			}
			popupFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, popupFadeValue / 4f));
			if (popupRequest == 1)
			{
				confiGUI.localScale = new Vector3(popupFadeValue, popupFadeValue, popupFadeValue);
			}
			else
			{
				disSetting.localScale = new Vector3(popupFadeValue, popupFadeValue, popupFadeValue);
			}
		}
		float num = 2.5f;
		switch (state)
		{
		case State.fadeIn:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
			{
				fadeTime = 0f;
				state = State.playerInput;
			}
			break;
		case State.initial:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			if (Global.IsSoundOn)
			{
				status[0] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[0].gameObject, 2, 2, 0);
			}
			if (Global.IsMusicOn)
			{
				status[1] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[1].gameObject, 2, 2, 0);
			}
			if (Global.IsVoiceOn)
			{
				status[2] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[2].gameObject, 2, 2, 0);
			}
			if (Global.IsItemVOn)
			{
				status[3] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[3].gameObject, 2, 2, 0);
			}
			if (Global.IsIntroOn)
			{
				status[4] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[4].gameObject, 2, 2, 0);
			}
			checkController();
			state = State.fadeIn;
			break;
		case State.playerInput:
			playerInput();
			pointerControl();
			break;
		case State.nextScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				GameSave.Save();
				if (nextScene == 1)
				{
					Application.LoadLevel("Credits");
				}
				else if (nextScene == 2)
				{
					Application.LoadLevel("ControlSchemes");
				}
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				GameSave.Save();
				Loading.LoadScene("Mainmenu");
			}
			break;
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
			zero = player.transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out array[i]) && i == num && pointerInit == 0))
			{
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					idleButton(togButton[i]);
					togButton[i] = null;
				}
				if (array[i].transform.name.Contains("switch"))
				{
					togButton[i] = array[i].transform.gameObject;
					int num2 = int.Parse(togButton[i].transform.name.Substring(togButton[i].transform.name.Length - 1));
					if (status[num2 - 1] == 0)
					{
						if (!isPress)
						{
							TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 3);
						}
						if (controller.DoEnterPress("Fire1"))
						{
							controller.DoPadPress(i);
							GameSound.StartSFX("menuSelect");
							status[num2 - 1] = 1;
							TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 1);
							switch (num2)
							{
							case 1:
								Global.SetSound(true);
								break;
							case 2:
								Global.SetMusic(true);
								break;
							case 3:
								Global.SetVoice(true);
								break;
							case 4:
								Global.SetItemV(true);
								break;
							case 5:
								Global.SetIntro(true);
								break;
							}
							if (i == num)
							{
								touchPos = Vector3.zero;
								mouseOver[i] = false;
								idleButton(togButton[i]);
								togButton[i] = null;
							}
						}
					}
					else
					{
						TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 1);
						if (controller.DoEnterPress("Fire1"))
						{
							controller.DoPadPress(i);
							GameSound.StartSFX("menuSelect");
							status[num2 - 1] = 0;
							TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 3);
							switch (num2)
							{
							case 1:
								Global.SetSound(false);
								break;
							case 2:
								Global.SetMusic(false);
								break;
							case 3:
								Global.SetVoice(false);
								break;
							case 4:
								Global.SetItemV(false);
								break;
							case 5:
								Global.SetIntro(false);
								break;
							}
							if (i == num)
							{
								touchPos = Vector3.zero;
								mouseOver[i] = false;
								idleButton(togButton[i]);
								togButton[i] = null;
							}
						}
					}
					if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
					}
				}
				if (array[i].transform.name == "btn_back")
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(backBtn, 4, 1, 1);
					}
					togButton[i] = backBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(backBtn, 4, 1, 2);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						state = State.backScene;
					}
				}
				else if (array[i].transform.name == "btn_credit")
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(creditBtn, 3, 1, 1);
					}
					togButton[i] = creditBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(creditBtn, 3, 1, 2);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						nextScene = 1;
						state = State.nextScene;
					}
				}
				else if (array[i].transform.name == "btn_reset" || array[i].transform.name == "btn_display")
				{
					togButton[i] = array[i].transform.gameObject;
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
					}
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
						isPress = true;
					}
					else
					{
						if (!controller.DoEnterRelease("Fire1"))
						{
							continue;
						}
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						idleButton(togButton[i]);
						popupFadeSign = 1;
						popupFadeSpeed = 12f * Time.smoothDeltaTime;
						creditBtn.GetComponent<Collider>().enabled = false;
						backBtn.GetComponent<Collider>().enabled = false;
						resetBtn.GetComponent<Collider>().enabled = false;
						controlBtn.GetComponent<Collider>().enabled = false;
						displayBtn.GetComponent<Collider>().enabled = false;
						if (array[i].transform.name == "btn_reset")
						{
							popupRequest = 1;
							confiSys.SetActive(true);
							continue;
						}
						popupRequest = 2;
						disSetting.parent.gameObject.SetActive(true);
						if (Screen.fullScreen)
						{
							TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
							TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 3);
						}
						else
						{
							TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 0);
							TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 3);
						}
						Global.FullScreen = Screen.fullScreen;
						Debug.Log(Global.Resolution + " : " + Global.Quality + " : " + GameSetting.restlist.Length + " : " + GameSetting.quatlist.Length);
						resBtn.transform.parent.Find("gs_seltxtscr").GetComponent<TextMesh>().text = GameSetting.restlist[Global.Resolution];
						quaBtn.transform.parent.Find("gs_seltxtqua").GetComponent<TextMesh>().text = GameSetting.quatlist[Global.Quality];
					}
				}
				else if (array[i].transform.name == "btn_control")
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(controlBtn, 3, 1, 1);
					}
					togButton[i] = controlBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(controlBtn, 3, 1, 2);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						nextScene = 2;
						state = State.nextScene;
					}
				}
				else if (array[i].transform.name.Contains("btn") && confiSys.gameObject.activeSelf)
				{
					togButton[i] = array[i].transform.gameObject;
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 1);
					}
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 2);
						isPress = true;
						Transform child = togButton[i].transform.GetChild(0);
						Vector3 localPosition = child.localPosition;
						if (localPosition.x == 0f && localPosition.y == 0f)
						{
							localPosition.x = -0.03f;
							localPosition.y = -0.06f;
							child.localPosition = localPosition;
						}
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						MonoBehaviour.print("click at : " + togButton[i].name);
						Transform child2 = togButton[i].transform.GetChild(0);
						Vector3 localPosition2 = child2.localPosition;
						if (localPosition2.x != 0f || localPosition2.y != 0f)
						{
							localPosition2.x = 0f;
							localPosition2.y = 0f;
							child2.localPosition = localPosition2;
						}
						if (togButton[i].name == "btn_cf_ok")
						{
							Debug.Log("start clear all preset");
							GameSetting.ClearPreset();
							GameSave.Reset();
							popupFadeSign = -1;
						}
						else
						{
							popupFadeSign = -1;
						}
						GameSound.StartSFX("menuSelect");
						break;
					}
				}
				else
				{
					if (!disSetting.gameObject.activeSelf)
					{
						continue;
					}
					if (mouseFocus)
					{
						if (controller.DoEnterRelease("Fire1"))
						{
							controller.DoPadRelease(i);
							float num3 = ((float)(GameSetting.restlist.Length - resList.Length) / 2f - (float)resPos) * resStep;
							Debug.Log(num3);
							resBar.localPosition = new Vector3(num3, 0f, 3f);
							mouseFocus = false;
							break;
						}
					}
					else if (array[i].transform.name.Contains("btn") || array[i].transform.name.Contains("bg_gs_drop"))
					{
						if (array[i].transform.name.Contains("bg_gs_dropscr"))
						{
							togButton[i] = resBtn;
						}
						else if (array[i].transform.name.Contains("bg_gs_dropqua"))
						{
							togButton[i] = quaBtn;
						}
						else
						{
							togButton[i] = array[i].transform.gameObject;
						}
						if (!isPress)
						{
							TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
						}
						if (controller.DoEnterHold("Fire1"))
						{
							controller.DoPadPress(i);
							TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
							isPress = true;
						}
						else if (controller.DoEnterRelease("Fire1"))
						{
							controller.DoPadRelease(i);
							touchPos = Vector3.zero;
							if (togButton[i].name.Contains("dropscr"))
							{
								if (!resMenu.activeSelf)
								{
									resMenu.SetActive(true);
									resMenu.transform.parent.localPosition = new Vector3(0f, -0.02f, 5f);
								}
								else
								{
									resMenu.transform.parent.localPosition = new Vector3(0f, -0.02f, 1f);
									resMenu.SetActive(false);
								}
							}
							else if (togButton[i].name.Contains("dropqua"))
							{
								if (!quaMenu.activeSelf)
								{
									quaMenu.SetActive(true);
									quaMenu.transform.parent.localPosition = new Vector3(0f, -0.72f, 5f);
								}
								else
								{
									quaMenu.transform.parent.localPosition = new Vector3(0f, -0.72f, 1f);
									quaMenu.SetActive(false);
								}
							}
							else if (togButton[i].name.Contains("ok"))
							{
								popupFadeSign = -1;
								int width = int.Parse(GameSetting.restlist[Global.Resolution].Substring(0, GameSetting.restlist[Global.Resolution].IndexOf("x")));
								int height = int.Parse(GameSetting.restlist[Global.Resolution].Substring(GameSetting.restlist[Global.Resolution].IndexOf("x") + 1));
								Screen.SetResolution(width, height, Global.FullScreen);
								QualitySettings.SetQualityLevel(Global.Quality, true);
								GameSetting.ApplyGraphicSetting();
							}
							else if (togButton[i].name.Contains("cancel"))
							{
								popupFadeSign = -1;
								Global.FullScreen = Screen.fullScreen;
								Global.Resolution = GameSetting.GetResolutionID();
								Global.Quality = QualitySettings.GetQualityLevel();
								if (Screen.fullScreen)
								{
									TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
									TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 3);
								}
								else
								{
									TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 0);
									TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 3);
								}
								Global.FullScreen = Screen.fullScreen;
								resBtn.transform.parent.Find("gs_seltxtscr").GetComponent<TextMesh>().text = GameSetting.restlist[Global.Resolution];
								quaBtn.transform.parent.Find("gs_seltxtqua").GetComponent<TextMesh>().text = GameSetting.quatlist[Global.Quality];
							}
							else
							{
								popupFadeSign = -1;
								GameSetting.DefaultGraphicSetting();
								int width2 = int.Parse(GameSetting.restlist[Global.Resolution].Substring(0, GameSetting.restlist[Global.Resolution].IndexOf("x")));
								int height2 = int.Parse(GameSetting.restlist[Global.Resolution].Substring(GameSetting.restlist[Global.Resolution].IndexOf("x") + 1));
								Screen.SetResolution(width2, height2, Global.FullScreen);
								QualitySettings.SetQualityLevel(2, true);
								TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 3);
								TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
								resBtn.transform.parent.Find("gs_seltxtscr").GetComponent<TextMesh>().text = GameSetting.restlist[Global.Resolution];
								quaBtn.transform.parent.Find("gs_seltxtqua").GetComponent<TextMesh>().text = GameSetting.quatlist[Global.Quality];
							}
							isPress = false;
							GameSound.StartSFX("menuSelect");
							idleButton(togButton[i]);
							break;
						}
					}
					else if (array[i].transform.name.Contains("tog"))
					{
						togButton[i] = array[i].transform.gameObject;
						if (!Global.FullScreen && togButton[i].name.Equals("tog_gs_full"))
						{
							TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 1);
							if (controller.DoEnterPress("Fire1"))
							{
								TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 3);
								TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
								GameSound.StartSFX("menuSelect");
								Global.FullScreen = true;
								idleButton(togButton[i]);
								break;
							}
						}
						else if (Global.FullScreen && togButton[i].name.Equals("tog_gs_window"))
						{
							TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 1);
							if (controller.DoEnterPress("Fire1"))
							{
								TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 3);
								TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 0);
								GameSound.StartSFX("menuSelect");
								Global.FullScreen = false;
								idleButton(togButton[i]);
								break;
							}
						}
					}
					if (resMenu.activeSelf)
					{
						if (array[i].transform.name.Contains("gs_scrtab") || mouseFocus)
						{
							Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
							if (!Cursor.visible)
							{
								vector = player.transform.position;
							}
							if (array[i].transform.name.Equals("gs_scrtab"))
							{
								resBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.3f, 0.5f, 0.5f, 0.5f));
							}
							else if (!mouseFocus)
							{
								resBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
							}
							if (controller.DoEnterHold("Fire1") || mouseFocus)
							{
								mouseFocus = true;
								controller.DoPadPress(i);
								resPos = Mathf.RoundToInt(Mathf.Abs(resBar.localPosition.x - (1f - resBar.localScale.x)) / resStep);
								Debug.Log(resPos);
								UpdateMenu(resList, GameSetting.restlist, resPos);
								if (vector.y >= rescap[0])
								{
									resBar.localPosition = new Vector3(1f - resBar.localScale.x, 0f, 3f);
									break;
								}
								if (vector.y <= rescap[1])
								{
									resBar.localPosition = new Vector3(0f - (1f - resBar.localScale.x), 0f, 3f);
									break;
								}
								resBar.position = new Vector3(resBar.position.x, vector.y, resBar.position.z);
								resBar.localPosition = new Vector3(resBar.localPosition.x, 0f, 3f);
							}
						}
						else if (array[i].transform.name.Contains("gs_txtscr"))
						{
							togButton[i] = array[i].transform.gameObject;
							TextMesh component = togButton[i].transform.GetComponent<TextMesh>();
							component.color = new Color(0.745f, 0.902f, 0.922f, 1f);
							int num4 = int.Parse(component.name.Substring(component.name.IndexOf(")") + 1));
							resBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
							if (controller.DoEnterRelease("Fire1"))
							{
								controller.DoPadRelease(i);
								resBtn.transform.parent.Find("gs_seltxtscr").GetComponent<TextMesh>().text = GameSetting.restlist[num4 + resPos];
								GameSound.StartSFX("menuSelect");
								Global.Resolution = num4 + resPos;
								resMenu.transform.parent.localPosition = new Vector3(0f, -0.02f, 1f);
								resMenu.SetActive(false);
								idleButton(togButton[i]);
								break;
							}
						}
						if (!array[i].transform.name.Contains("scr") && !mouseFocus)
						{
							resMenu.transform.parent.localPosition = new Vector3(0f, -0.02f, 1f);
							resMenu.SetActive(false);
							continue;
						}
						float num5 = Mathf.Round(resBar.localPosition.x * 100f);
						if (Input.GetAxis("mousewheel") > 0f && num5 < Mathf.Round((1f - resBar.localScale.x) * 100f))
						{
							resBar.localPosition = new Vector3(resBar.localPosition.x + resStep, 0f, 3f);
							resPos--;
							UpdateMenu(resList, GameSetting.restlist, resPos);
						}
						else if (Input.GetAxis("mousewheel") < 0f && num5 > 0f - Mathf.Round((1f - resBar.localScale.x) * 100f))
						{
							resBar.localPosition = new Vector3(resBar.localPosition.x - resStep, 0f, 3f);
							resPos++;
							UpdateMenu(resList, GameSetting.restlist, resPos);
						}
					}
					else
					{
						if (!quaMenu.activeSelf)
						{
							continue;
						}
						if (array[i].transform.name.Contains("gs_txtqua"))
						{
							togButton[i] = array[i].transform.gameObject;
							TextMesh component2 = togButton[i].transform.GetComponent<TextMesh>();
							component2.color = new Color(0.745f, 0.902f, 0.922f, 1f);
							if (controller.DoEnterRelease("Fire1"))
							{
								controller.DoPadRelease(i);
								int num6 = int.Parse(component2.name.Substring(component2.name.IndexOf(")") + 1));
								quaBtn.transform.parent.Find("gs_seltxtqua").GetComponent<TextMesh>().text = GameSetting.quatlist[num6 + quaPos];
								GameSound.StartSFX("menuSelect");
								Global.Quality = num6 + quaPos;
								quaMenu.transform.parent.localPosition = new Vector3(0f, -0.72f, 1f);
								quaMenu.SetActive(false);
								idleButton(togButton[i]);
								break;
							}
						}
						if (!array[i].transform.name.Contains("qua"))
						{
							quaMenu.transform.parent.localPosition = new Vector3(0f, -0.72f, 1f);
							quaMenu.SetActive(false);
							continue;
						}
						float num7 = Mathf.Round(quaBar.localPosition.x * 100f);
						if (Input.GetAxis("mousewheel") > 0f && num7 < Mathf.Round((1f - quaBar.localScale.x) * 100f))
						{
							quaBar.localPosition = new Vector3(quaBar.localPosition.x + quaStep, 0f, 3f);
							quaPos--;
							UpdateMenu(quaList, GameSetting.quatlist, quaPos);
						}
						else if (Input.GetAxis("mousewheel") < 0f && num7 > 0f - Mathf.Round((1f - quaBar.localScale.x) * 100f))
						{
							quaBar.localPosition = new Vector3(quaBar.localPosition.x - quaStep, 0f, 3f);
							quaPos++;
							UpdateMenu(quaList, GameSetting.quatlist, quaPos);
						}
					}
				}
			}
			else if (togButton[i] != null)
			{
				controller.DoPadRelease(i);
				mouseOver[i] = false;
				idleButton(togButton[i]);
				togButton[i] = null;
			}
		}
	}

	private void idleButton(GameObject togButton)
	{
		isPress = false;
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
		if (togButton.name.Contains("back") || togButton.name.Contains("cf_cancel") || togButton.name.Contains("cf_ok"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		else if ((int)(togButton.GetComponent<Renderer>().material.GetTextureScale("_MainTex").x * 1000f) == 333)
		{
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		else if (togButton.name.Contains("switch"))
		{
			int num = int.Parse(togButton.transform.name.Substring(togButton.transform.name.Length - 1));
			if (status[num - 1] == 0)
			{
				TextureUtility.SetSpriteIndex(togButton, 2, 2, 2);
			}
			else
			{
				TextureUtility.SetSpriteIndex(togButton, 2, 2, 0);
			}
		}
		else if (togButton.name.Contains("tog"))
		{
			if (Global.FullScreen)
			{
				TextureUtility.SetSpriteIndex(windowBtn, 4, 1, 0);
			}
			else
			{
				TextureUtility.SetSpriteIndex(fullBtn, 4, 1, 0);
			}
		}
		else if (togButton.name.Contains("gs_txt"))
		{
			togButton.transform.GetComponent<TextMesh>().color = new Color(0.745f, 0.902f, 0.922f, 0.502f);
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

	private void TabInit()
	{
		int num = ((GameSetting.restlist.Length < maxres) ? GameSetting.restlist.Length : maxres);
		int num2 = ((GameSetting.quatlist.Length < maxqua) ? GameSetting.quatlist.Length : maxqua);
		resList = new GameObject[num];
		quaList = new GameObject[num2];
		Transform transform = resMenu.transform.Find("gs_dropscrbg");
		Transform transform2 = resMenu.transform.Find("gs_dropscrtail");
		transform.localScale = new Vector3(1f, 0f - 0.89f * (float)(num + 1), 1f);
		transform.localPosition = new Vector3(0f, 0f - 0.89f * (float)num, 1f);
		transform2.localPosition = new Vector3(0f, 0f - 0.89f * (float)num * 2f, 2f);
		transform = quaMenu.transform.Find("gs_dropquabg");
		transform2 = quaMenu.transform.Find("gs_dropquatail");
		transform.localScale = new Vector3(1f, 0f - 0.89f * (float)(num2 + 1), 1f);
		transform.localPosition = new Vector3(0f, 0f - 0.89f * (float)num2, 1f);
		transform2.localPosition = new Vector3(0f, 0f - 0.89f * (float)num2 * 2f, 2f);
		Transform transform3 = resMenu.transform.Find("gs_txtscr");
		Transform transform4 = resMenu.transform.Find("gs_prop2");
		for (int i = 0; i < num; i++)
		{
			resList[i] = Object.Instantiate(transform3.gameObject, transform3.position, transform3.rotation) as GameObject;
			resList[i].name = resList[i].name + i;
			resList[i].transform.parent = resMenu.transform;
			resList[i].transform.localScale = transform3.localScale;
			resList[i].GetComponent<TextMesh>().text = GameSetting.restlist[i];
			resList[i].transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((i + 1) * 2) + 0.4f), 5f);
			if (i < num - 1)
			{
				GameObject gameObject = Object.Instantiate(transform4.gameObject, transform4.position, transform4.rotation) as GameObject;
				gameObject.transform.parent = resMenu.transform;
				gameObject.transform.localScale = transform4.localScale;
				gameObject.transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((i + 1) * 2) + 0.89f), 2f);
			}
		}
		transform3.gameObject.SetActive(false);
		transform4.gameObject.SetActive(false);
		Transform transform5 = quaMenu.transform.Find("gs_txtqua");
		Transform transform6 = quaMenu.transform.Find("gs_prop2");
		for (int j = 0; j < num2; j++)
		{
			quaList[j] = Object.Instantiate(transform5.gameObject, transform5.position, transform5.rotation) as GameObject;
			quaList[j].name = quaList[j].name + j;
			quaList[j].transform.parent = quaMenu.transform;
			quaList[j].transform.localScale = transform5.localScale;
			quaList[j].GetComponent<TextMesh>().text = GameSetting.quatlist[j];
			quaList[j].transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((j + 1) * 2) + 0.4f), 5f);
			if (j < num2 - 1)
			{
				GameObject gameObject2 = Object.Instantiate(transform6.gameObject, transform6.position, transform6.rotation) as GameObject;
				gameObject2.transform.parent = quaMenu.transform;
				gameObject2.transform.localScale = transform6.localScale;
				gameObject2.transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((j + 1) * 2) + 0.89f), 2f);
			}
		}
		transform5.gameObject.SetActive(false);
		transform6.gameObject.SetActive(false);
		resBar.parent.localScale = new Vector3(0.89f * (float)num - 0.32f, 0.05f, 1f);
		resBar.parent.localPosition = new Vector3(-0.89f, -0.89f * (float)(num + 1), 2f);
		quaBar.parent.localScale = new Vector3(0.89f * (float)num2 - 0.32f, 0.05f, 1f);
		quaBar.parent.localPosition = new Vector3(-0.89f, -0.89f * (float)(num2 + 1), 2f);
		resBar.localScale = new Vector3((float)num / (float)GameSetting.restlist.Length, 1f, 1f);
		resBar.localPosition = new Vector3(0f - (1f - resBar.localScale.x), 0f, 3f);
		rescap[1] = resBar.position.y;
		resBar.localPosition = new Vector3(1f - resBar.localScale.x, 0f, 3f);
		rescap[0] = resBar.position.y;
		quaBar.localScale = new Vector3((float)num2 / (float)GameSetting.quatlist.Length, 1f, 1f);
		quaBar.localPosition = new Vector3(1f - quaBar.localScale.x, 0f, 3f);
		resStep = (1f - resBar.localScale.x) * 2f / (float)(GameSetting.restlist.Length - num);
		quaStep = (1f - quaBar.localScale.x) * 2f / (float)(GameSetting.quatlist.Length - num2);
		Debug.Log("Step = " + resStep + " , " + quaStep);
		Debug.Log("Cap >> " + rescap[0] + " , " + rescap[1]);
		resMenu.SetActive(false);
		quaMenu.SetActive(false);
	}

	private void UpdateMenu(GameObject[] textList, string[] tempList, int pos)
	{
		for (int i = 0; i < textList.Length; i++)
		{
			textList[i].GetComponent<TextMesh>().text = tempList[i + pos];
		}
	}
}
