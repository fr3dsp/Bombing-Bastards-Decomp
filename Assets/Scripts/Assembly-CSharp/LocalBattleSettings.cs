using UnityEngine;

public class LocalBattleSettings : MonoBehaviour
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

	private GameObject[] player;

	private GameObject winBtn;

	private GameObject timeBtn;

	private GameObject backBtn;

	private GameObject nextBtn;

	private State state;

	private GameObject[] togButton;

	private int[] TextWins;

	private int[] TextTime;

	private int[] status;

	private float fadeTime;

	private bool[] mouseOver = new bool[5];

	private float[] pTime = new float[5];

	public float speed = 45.5f;

	private float pointerSpeed = Global.pointerSpd;

	private int[] pointerInit = new int[5];

	private Vector3 touchPos;

	private int[] ctrlStat = new int[5];

	private void Awake()
	{
	}

	private void Start()
	{
		toggleBtn = new Transform[4];
		player = new GameObject[5];
		togButton = new GameObject[5];
		status = new int[6] { 0, 0, 0, 0, 1, 1 };
		TextWins = new int[5] { 1, 2, 3, 4, 5 };
		TextTime = new int[5] { 1, 2, 3, 4, 5 };
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
			else if (transform.name == "btn_next")
			{
				nextBtn = transform.gameObject;
			}
			else if (transform.name == "btn_time")
			{
				timeBtn = transform.gameObject;
			}
			else if (transform.name == "btn_win")
			{
				winBtn = transform.gameObject;
			}
			else if (transform.name.Contains("pointer"))
			{
				int num2 = int.Parse(transform.transform.GetChild(0).name.Substring(transform.transform.GetChild(0).name.Length - 1));
				if (num2 < player.Length)
				{
					player[num2] = transform.gameObject;
				}
			}
		}
	}

	private void Update()
	{
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
			status[4] = Global.winIndex;
			winBtn.transform.GetChild(0).GetComponent<TextMesh>().text = string.Empty + TextWins[status[4]];
			status[5] = Global.timeIndex;
			timeBtn.transform.GetChild(0).GetComponent<TextMesh>().text = string.Empty + TextTime[status[5]];
			if (Global.ghost)
			{
				status[0] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[0].gameObject, 2, 2, 0);
			}
			if (Global.itemDestroy)
			{
				status[1] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[1].gameObject, 2, 2, 0);
			}
			if (Global.mapShrink)
			{
				status[2] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[2].gameObject, 2, 2, 0);
			}
			if (Global.virus)
			{
				status[3] = 1;
				TextureUtility.SetSpriteIndex(toggleBtn[3].gameObject, 2, 2, 0);
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
				Global.winIndex = status[4];
				Global.timeIndex = status[5];
				Global.winAmount = TextWins[status[4]];
				Global.timeAmount = TextTime[status[5]];
				Global.ghost = status[0] == 1;
				Global.itemDestroy = status[1] == 1;
				Global.mapShrink = status[2] == 1;
				Global.virus = status[3] == 1;
				Application.LoadLevel("LocalBattle.SelectStage");
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				Global.winIndex = status[4];
				Global.timeIndex = status[5];
				Global.winAmount = TextWins[status[4]];
				Global.timeAmount = TextTime[status[5]];
				Global.ghost = status[0] == 1;
				Global.itemDestroy = status[1] == 1;
				Global.mapShrink = status[2] == 1;
				Global.virus = status[3] == 1;
				Application.LoadLevel("LocalBattle.Menu");
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
			zero = player[i].transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo) || i == num)
			{
			}
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out hitInfo) && i == num))
			{
				if (array[i].transform == null)
				{
					array[i] = hitInfo;
				}
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					mouseOver[i] = false;
					idleButton(togButton[i]);
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (array[i].transform.name.Contains("switch"))
				{
					togButton[i] = array[i].transform.gameObject;
					int num2 = int.Parse(togButton[i].transform.name.Substring(togButton[i].transform.name.Length - 1));
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
					}
					if (status[num2 - 1] == 0)
					{
						TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 3);
						if (controller.DoEnterRelease("Fire1"))
						{
							controller.DoPadRelease(i);
							touchPos = Vector3.zero;
							GameSound.StartSFX("menuSelect");
							status[num2 - 1] = 1;
							TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 1);
							if (i == num)
							{
								mouseOver[i] = false;
								idleButton(togButton[i]);
								togButton[i] = null;
							}
						}
					}
					else
					{
						TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 1);
						if (controller.DoEnterRelease("Fire1"))
						{
							controller.DoPadRelease(i);
							touchPos = Vector3.zero;
							GameSound.StartSFX("menuSelect");
							status[num2 - 1] = 0;
							TextureUtility.SetSpriteIndex(togButton[i], 2, 2, 3);
							if (i == num)
							{
								mouseOver[i] = false;
								idleButton(togButton[i]);
								togButton[i] = null;
							}
						}
					}
				}
				if (array[i].transform.name == "btn_win")
				{
					togButton[i] = array[i].transform.gameObject;
					TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
					Vector3 localPosition = togButton[i].transform.GetChild(0).localPosition;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						if (localPosition.y != 0.78f)
						{
							if (status[4] < TextTime.Length - 1)
							{
								status[4]++;
							}
							else
							{
								status[4] = 0;
							}
						}
						localPosition.y = 0.78f;
						togButton[i].transform.GetChild(0).localPosition = localPosition;
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
						togButton[i].transform.GetChild(0).GetComponent<TextMesh>().text = string.Empty + TextWins[status[4]];
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						GameSound.StartSFX("menuSelect");
						localPosition.y = 0.89f;
						togButton[i].transform.GetChild(0).localPosition = localPosition;
					}
				}
				else if (array[i].transform.name == "btn_time")
				{
					togButton[i] = array[i].transform.gameObject;
					TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
					Vector3 localPosition2 = togButton[i].transform.GetChild(0).localPosition;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						if (localPosition2.y != 0.61f)
						{
							if (status[5] < TextTime.Length - 1)
							{
								status[5]++;
							}
							else
							{
								status[5] = 0;
							}
						}
						localPosition2.y = 0.61f;
						togButton[i].transform.GetChild(0).localPosition = localPosition2;
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
						togButton[i].transform.GetChild(0).GetComponent<TextMesh>().text = string.Empty + TextTime[status[5]];
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						GameSound.StartSFX("menuSelect");
						localPosition2.y = 0.72f;
						togButton[i].transform.GetChild(0).localPosition = localPosition2;
					}
				}
				else if (array[i].transform.name == "btn_back")
				{
					TextureUtility.SetSpriteIndex(backBtn, 4, 1, 1);
					togButton[i] = backBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(backBtn, 4, 1, 2);
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
				else if (array[i].transform.name == "btn_next")
				{
					TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 1);
					togButton[i] = nextBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 2);
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						state = State.nextScene;
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
		if (togButton.name.Contains("win"))
		{
			Vector3 localPosition = togButton.transform.GetChild(0).localPosition;
			localPosition.y = 0.89f;
			togButton.transform.GetChild(0).localPosition = localPosition;
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		else if (togButton.name.Contains("time"))
		{
			Vector3 localPosition2 = togButton.transform.GetChild(0).localPosition;
			localPosition2.y = 0.72f;
			togButton.transform.GetChild(0).localPosition = localPosition2;
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		else if (togButton.name.Contains("next") || togButton.name.Contains("back"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
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
			player[p].transform.localPosition = new Vector3(-1000f, 0f, -100f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = player[p].transform.localPosition;
			if (i == 1)
			{
				float num = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit[p] == 0 && num != 0f && localPosition.x < -181f)
				{
					player[p].transform.localPosition = new Vector3(0f, 0f, -100f);
					pointerInit[p] = 1;
					break;
				}
				if (num < 0f)
				{
					if (localPosition.x >= -181f)
					{
						player[p].transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
						pTime[p] = 0f;
					}
				}
				else if (num > 0f && localPosition.x <= 195f)
				{
					player[p].transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
					pTime[p] = 0f;
				}
				continue;
			}
			float num2 = ctrl.GetVertical() * 1.25f;
			if (pointerInit[p] == 0 && num2 != 0f && localPosition.x < -181f)
			{
				player[p].transform.localPosition = new Vector3(0f, 0f, -100f);
				pointerInit[p] = 1;
				break;
			}
			if (num2 < 0f)
			{
				if (localPosition.y <= 98f)
				{
					player[p].transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
					pTime[p] = 0f;
				}
			}
			else if (num2 > 0f && localPosition.y >= -115f)
			{
				player[p].transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
				pTime[p] = 0f;
			}
		}
	}
}
