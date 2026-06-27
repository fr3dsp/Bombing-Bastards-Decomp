using System;
using TNet;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private enum State
	{
		initial = 0,
		fadeIn = 1,
		playerInput = 2,
		nextScene = 3
	}

	public int randomTime = 1;

	public float gearSpeed = 10f;

	public float effectSpeed = 12f;

	private bool toggle;

	private int direction = 1;

	private bool isPress;

	private float pointerSpeed = Global.pointerSpd;

	private State state;

	private GameObject[] togButton;

	private GameObject player;

	private int menuCount;

	private int holdID = -1;

	private string gotoScene = "MainMenu";

	private int pointerInit;

	private GameObject[] menu;

	private GameObject gear;

	private GameObject gearEffect;

	private GameObject bannerBG;

	private GameObject whiteBG;

	private GameObject fadeBG;

	private Transform robot;

	private bool[] mouseOver = new bool[5];

	private float fadeTime;

	private int selectAt = -1;

	private float pTime;

	public TextAsset quoteText;

	private float quoteTime = -1f;

	private Transform quoteNode;

	private GameObject confiSys;

	private float confiFadeValue;

	private int confiFadeSign;

	private Material confiFade;

	private Transform confiGUI;

	private int confiRequest;

	private GameObject[] confiBtn;

	private AnimationState aAni;

	private List<Transform> fadeAni;

	private Transform uvAni;

	private GameObject idleAni;

	private bool finishAnim;

	private Transform tDynamiteI;

	private Transform tDynamiteS;

	private float startBounce = -1f;

	private Material dynamiteMat;

	private float shinkPos = 128f;

	private Vector3 touchPos;

	private void Awake()
	{
		Global.advStage = 0;
		fadeBG = GameObject.Find("fade_bg");
		AnimateInit();
		confiFadeValue = 0f;
		confiFadeSign = 0;
		confiFade = GameObject.Find("confirm/bg").GetComponent<Renderer>().material;
		confiGUI = GameObject.Find("confirm/ui").transform;
		confiGUI.localScale = Vector3.zero;
		confiBtn = new GameObject[2];
		confiBtn[0] = confiGUI.Find("btn_ok").gameObject;
		confiBtn[1] = confiGUI.Find("btn_cancel").gameObject;
		confiSys = confiGUI.parent.gameObject;
		confiSys.SetActive(false);
	}

	private void Start()
	{
		for (int i = 1; i <= 5; i++)
		{
			GameSound.StopBGM("Map" + i.ToString("D2"));
		}
		GameSound.StopBGM("BossFight");
		GameSound.StartBGM("Menu");
		selectAt = -1;
		menu = new GameObject[5];
		togButton = new GameObject[5];
		state = State.initial;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform transform = null;
		Transform transform2 = null;
		Transform[] array = componentsInChildren;
		foreach (Transform transform3 in array)
		{
			if (transform3.name.Contains("menu"))
			{
				menuCount++;
				int num = int.Parse(transform3.transform.name.Substring(transform3.transform.name.Length - 1));
				menu[num - 1] = transform3.gameObject.transform.GetChild(0).gameObject;
			}
			else if (transform3.name == "bannergear")
			{
				gear = transform3.gameObject;
			}
			else if (transform3.name == "bannereffect")
			{
				gearEffect = transform3.gameObject;
			}
			else if (transform3.name == "banner")
			{
				transform = transform3;
			}
			else if (transform3.name == "robot")
			{
				robot = transform3;
			}
			else if (transform3.name == "confident")
			{
				transform2 = transform3;
			}
			else if (transform3.name.Contains("p_pointer"))
			{
				player = transform3.gameObject;
			}
		}
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = 1.7777778f;
		float num4 = num2 / num3;
		Transform child = GameObject.Find("version").transform.GetChild(0);
		Vector3 localPosition = child.localPosition;
		localPosition.x *= num4;
		child.localPosition = localPosition;
		if (Global.pirated)
		{
			child.GetComponent<TextMesh>().text = "Version 1.2.0";
		}
		else
		{
			child.GetComponent<TextMesh>().text = "Version " + Global.version;
		}
	}

	private void AnimateInit()
	{
		fadeAni = new List<Transform>();
		Transform transform = base.transform.Find("gui_animate");
		aAni = transform.GetComponent<Animation>()["idle"];
		aAni.speed = 0.8f;
		finishAnim = false;
		string[] array = new string[5] { "banner/bannername_1", "banner/bomb", "company/logo", "company/sanuk", "white" };
		string[] array2 = array;
		foreach (string text in array2)
		{
			fadeAni.Add(transform.Find(text));
		}
		transform.Find("banner/bannerbg").GetComponent<Renderer>().enabled = false;
		transform.Find("banner/bannergear").GetComponent<Renderer>().enabled = false;
		transform.Find("banner/bannername_2").GetComponent<Renderer>().enabled = false;
		transform.Find("banner/bannereffect").GetComponent<Renderer>().enabled = false;
		tDynamiteI = transform.Find("banner/bannername_1/dynamite_i/plane");
		tDynamiteS = transform.Find("banner/bannername_1/dynamite_spark/plane");
		dynamiteMat = tDynamiteI.GetComponent<Renderer>().material;
		dynamiteMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
		uvAni = transform.Find("robot/ani/runing");
		idleAni = transform.Find("robot/ani/idle").gameObject;
		idleAni.SetActive(false);
		quoteNode = base.transform.Find("quote");
		string[] array3 = quoteText.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		quoteNode.Find("text").GetComponent<TextMesh>().text = array3[UnityEngine.Random.Range(0, array3.Length)] + "  \u0336  Dr. Wallow";
		AnimateCtrl(Global.isFirstStart);
	}

	private void AnimateCtrl(bool playAnim)
	{
		if (!playAnim && !idleAni.activeSelf)
		{
			aAni.normalizedTime = 1f;
		}
		if (aAni.normalizedTime >= 0.98f)
		{
			fadeBG.transform.localPosition = new Vector3(0f, 0f, -110f);
			finishAnim = true;
			float num = (float)Screen.width / (float)Screen.height;
			if (num < 1.5f)
			{
				shinkPos = 128f * (1f - (1.5f - num) * 0.4f);
			}
		}
		foreach (Transform item in fadeAni)
		{
			Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f * item.localScale.z);
			item.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			if (item.name == "bannername_1")
			{
				dynamiteMat.SetColor("_TintColor", color);
			}
		}
		if (startBounce == -1f && fadeAni[fadeAni.Count - 1].localScale.z < 1f)
		{
			startBounce = ((!playAnim) ? 1 : 0);
			if (!playAnim)
			{
				tDynamiteI.localScale = Vector3.one;
				tDynamiteS.localScale = Vector3.one;
			}
		}
		if (startBounce >= 0f && startBounce < 1f)
		{
			startBounce += Time.deltaTime * 5f;
			if (startBounce > 1f)
			{
				startBounce = 1f;
			}
			float num2;
			float num3;
			if (startBounce < 0.5f)
			{
				num2 = startBounce + 1f;
				num3 = startBounce * 3f;
			}
			else
			{
				num2 = 2f - startBounce;
				num3 = num2;
			}
			tDynamiteI.localScale = new Vector3(num2, num2, num2);
			tDynamiteS.localScale = new Vector3(num3, num3, num3);
		}
		if (shinkPos != 128f)
		{
			Transform transform = base.transform.Find("gui_animate/banner");
			Transform transform2 = base.transform.Find("gui_animate/robot");
			float x = transform.localPosition.x;
			if (x > shinkPos)
			{
				x -= Time.deltaTime * 50f;
				if (x < shinkPos)
				{
					x = shinkPos;
				}
				float x2 = (0f - x) * 99f / 128f;
				transform.localPosition = new Vector3(x, 45.5f, 0f);
				transform2.localPosition = new Vector3(x2, 1.5f, 0f);
			}
		}
		int num4 = (int)uvAni.localScale.z;
		float x3 = (float)(num4 % 3) / 3f;
		float num5 = (float)(num4 / 3) / 3f;
		uvAni.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(x3, 0f - num5));
		if (aAni.time > 2.66f && aAni.time - Time.deltaTime <= 2.66f)
		{
			GameSound.StartSFX("skillMagic");
		}
		if (num4 == 1 && !GameSound.IsPlayingSFX("bombThrow"))
		{
			GameSound.StartSFX("bombThrow");
		}
		if (num4 == 7 && !GameSound.IsPlayingSFX("bombBlast"))
		{
			GameSound.StartSFX("bombBlast");
		}
		if (num4 == 8 && aAni.time > 1f && !idleAni.activeSelf)
		{
			idleAni.SetActive(true);
			fadeBG.transform.localPosition = new Vector3(0f, 0f, -110f);
			Global.isFirstStart = false;
			quoteTime = 0f;
		}
		if (quoteTime >= 0f)
		{
			quoteTime += Time.deltaTime;
			if (quoteTime < 10f)
			{
				float num6 = ((!(quoteTime < 2f)) ? 0f : (2f - quoteTime));
				quoteNode.localPosition = new Vector3(0f, -40f * num6, 0f);
			}
		}
	}

	private void Update()
	{
		if (confiFadeSign != 0)
		{
			if (confiFadeSign > 0)
			{
				confiFadeValue += 12f * Time.deltaTime;
				if (confiFadeValue >= 1f)
				{
					confiFadeValue = 1f;
					confiFadeSign = 0;
				}
			}
			else
			{
				confiFadeValue -= 12f * Time.deltaTime;
				if (confiFadeValue <= 0f)
				{
					confiFadeValue = 0f;
					confiFadeSign = 0;
					for (int i = 0; i < 5; i++)
					{
						menu[i].transform.parent.GetComponent<Collider>().enabled = true;
					}
					confiSys.SetActive(false);
				}
			}
			confiFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, confiFadeValue / 4f));
			confiGUI.localScale = new Vector3(confiFadeValue, confiFadeValue, confiFadeValue);
		}
		if (Input.anyKeyDown)
		{
			for (int j = 0; j < Global.validKeyCodes.Length; j++)
			{
				if (Input.GetKey(Global.validKeyCodes[j]) && Global.validKeyCodes[j].ToString().Contains("Mouse"))
				{
					return;
				}
			}
			Global.isFirstStart = false;
		}
		AnimateCtrl(Global.isFirstStart);
		float num = 2.5f;
		switch (state)
		{
		case State.fadeIn:
			fadeBG.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * num)));
			if (fadeBG.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f && !Global.isFirstStart)
			{
				checkController();
				fadeTime = 0f;
				state = State.playerInput;
			}
			break;
		case State.initial:
			fadeBG.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
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
				Application.LoadLevel(gotoScene);
			}
			break;
		}
		sceneAnim();
	}

	private void sceneAnim()
	{
		if ((int)(Time.time * 1000f) % randomTime == 0)
		{
			direction = UnityEngine.Random.Range(-1, 2);
		}
		gear.transform.Rotate(0f, 0f, gearSpeed * Time.deltaTime * (float)direction);
		gearEffect.transform.Rotate(0f, 0f, effectSpeed * Time.deltaTime);
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
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					mouseOver[i] = false;
					idleButton(togButton[i]);
					togButton[i] = null;
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (array[i].transform.name.Contains("menu"))
				{
					togButton[i] = array[i].transform.gameObject;
					int num2 = int.Parse(array[i].transform.name.Substring(array[i].transform.name.Length - 1));
					selectAt = num2 - 1;
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(menu[num2 - 1], 3, 1, 1);
					}
					if (controller.DoEnterHold("Fire1"))
					{
						TextureUtility.SetSpriteIndex(menu[selectAt], 3, 1, 2);
						controller.DoPadPress(i);
						isPress = true;
					}
					else
					{
						if (!controller.DoEnterRelease("Fire1") || (!finishAnim && Global.isFirstStart) || !isPress)
						{
							continue;
						}
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						TextureUtility.SetSpriteIndex(menu[selectAt], 3, 1, 0);
						controller.DoPadRelease(i);
						if (selectAt == 0)
						{
							gotoScene = "Adventure.SelectStage";
							state = State.nextScene;
							continue;
						}
						if (selectAt == 1)
						{
							gotoScene = "LocalBattle.Menu";
							state = State.nextScene;
							continue;
						}
						if (selectAt == 2)
						{
							gotoScene = "OnlineBattle.Menu";
							state = State.nextScene;
							Global.Mode = GameMode.OnlineBattle;
							continue;
						}
						if (selectAt == 3)
						{
							gotoScene = "Options";
							state = State.nextScene;
							continue;
						}
						confiFadeSign = 1;
						for (int j = 0; j < 5; j++)
						{
							menu[j].transform.parent.GetComponent<Collider>().enabled = false;
						}
						confiSys.SetActive(true);
					}
				}
				else
				{
					if (!array[i].transform.name.Contains("btn_"))
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
						touchPos = Vector3.zero;
						MonoBehaviour.print("click at : " + togButton[i].name);
						controller.DoPadRelease(i);
						Transform child2 = togButton[i].transform.GetChild(0);
						Vector3 localPosition2 = child2.localPosition;
						if (localPosition2.x != 0f || localPosition2.y != 0f)
						{
							localPosition2.x = 0f;
							localPosition2.y = 0f;
							child2.localPosition = localPosition2;
						}
						if (togButton[i].name == "btn_ok")
						{
							Application.Quit();
						}
						else
						{
							confiFadeSign = -1;
						}
						GameSound.StartSFX("menuSelect");
						idleButton(togButton[i]);
						togButton[i] = null;
						break;
					}
				}
			}
			else if (togButton[i] != null)
			{
				controller.DoPadRelease(i);
				mouseOver[i] = false;
				idleButton(togButton[i]);
				selectAt = -1;
				isPress = false;
				togButton[i] = null;
			}
		}
	}

	private void idleButton(GameObject button)
	{
		if (button.name.Contains("menu"))
		{
			TextureUtility.SetSpriteIndex(button.transform.GetChild(0).gameObject, 3, 1, 0);
		}
		else if (button.name.Contains("btn_"))
		{
			TextureUtility.SetSpriteIndex(button, 4, 1, 0);
			Transform child = button.transform.GetChild(0);
			Vector3 localPosition = child.localPosition;
			if (localPosition.x != 0f || localPosition.y != 0f)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
				child.localPosition = localPosition;
			}
		}
		button = null;
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

	private void checkController()
	{
	}
}
