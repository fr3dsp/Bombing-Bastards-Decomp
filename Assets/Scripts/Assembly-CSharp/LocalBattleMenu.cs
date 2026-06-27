using System;
using UnityEngine;

public class LocalBattleMenu : MonoBehaviour
{
	private enum State
	{
		fadeIn = 0,
		initial = 1,
		playerInput = 2,
		nextScene = 3,
		backScene = 4
	}

	private GameObject[] panel;

	private GameObject[] player;

	private GameObject backButton;

	private GameObject nextButton;

	private State state;

	private int initCount;

	private int[] buttonStat;

	private int[] pSelect;

	private int[] toggleAnim;

	private string[] aiLevel = new string[3] { "EASY", "NORMAL", "HARD" };

	private GameObject[] togButton;

	private float fadeTime;

	private int playerCount;

	private int aiCount;

	private float animTime;

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
		for (int i = 1; i <= 5; i++)
		{
			GameSound.StopBGM("Map" + i.ToString("D2"));
		}
		GameSound.StopBGM("BossFight");
		GameSound.StartBGM("Menu");
		panel = new GameObject[8];
		player = new GameObject[5];
		togButton = new GameObject[5];
		buttonStat = new int[8];
		state = State.fadeIn;
		initCount = 0;
		aiCount = 0;
		fadeTime = 0f;
		playerCount = 0;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("slot"))
			{
				MonoBehaviour.print(transform.transform.GetChild(0).name);
				int num = int.Parse(transform.transform.GetChild(0).name.Substring(transform.transform.GetChild(0).name.Length - 1));
				panel[num] = transform.gameObject;
			}
			else if (transform.name == "btn_back")
			{
				backButton = transform.gameObject;
			}
			else if (transform.name == "btn_next")
			{
				nextButton = transform.gameObject;
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
				state = State.initial;
			}
			break;
		case State.initial:
		{
			Global.ClearBattleData();
			for (int j = 0; j < initCount; j++)
			{
				if (initCount > 4)
				{
					continue;
				}
				if (buttonStat[j] == 0)
				{
					panel[j].transform.localPosition = Vector3.Lerp(panel[j].transform.localPosition, new Vector3(-46f, panel[j].transform.localPosition.y, 0f), speed / 150f);
					if (panel[j].transform.localPosition.x >= -48f)
					{
						buttonStat[j] = 1;
						panel[j].transform.localPosition = new Vector3(-48f, panel[j].transform.localPosition.y, 0f);
					}
				}
				else if (buttonStat[j] == 1)
				{
					panel[j].transform.localPosition = Vector3.Lerp(panel[j].transform.localPosition, new Vector3(-54f, panel[j].transform.localPosition.y, 0f), speed / 150f);
					if (panel[j].transform.localPosition.x <= -52f)
					{
						buttonStat[j] = 2;
						panel[j].transform.localPosition = new Vector3(-52f, panel[j].transform.localPosition.y, 0f);
					}
				}
				if (buttonStat[j + 4] == 0)
				{
					panel[j + 4].transform.localPosition = Vector3.Lerp(panel[j + 4].transform.localPosition, new Vector3(46f, panel[j + 4].transform.localPosition.y, 0f), speed / 150f);
					if (panel[j + 4].transform.localPosition.x <= 48f)
					{
						buttonStat[j + 4] = 1;
						panel[j + 4].transform.localPosition = new Vector3(48f, panel[j + 4].transform.localPosition.y, 0f);
					}
				}
				else if (buttonStat[j + 4] == 1)
				{
					panel[j + 4].transform.localPosition = Vector3.Lerp(panel[j + 4].transform.localPosition, new Vector3(54f, panel[j + 4].transform.localPosition.y, 0f), speed / 150f);
					if (panel[j + 4].transform.localPosition.x >= 52f)
					{
						buttonStat[j + 4] = 2;
						panel[j + 4].transform.localPosition = new Vector3(52f, panel[j + 4].transform.localPosition.y, 0f);
					}
				}
			}
			if (animTime * 12.5f >= (float)initCount && initCount < 4)
			{
				initCount++;
			}
			else
			{
				animTime += Time.deltaTime;
			}
			if (buttonStat[7] != 2)
			{
				break;
			}
			buttonStat = new int[8] { 1, 0, 0, 0, 0, 0, 0, 0 };
			toggleAnim = new int[8] { 4, 4, 4, 4, 4, 4, 4, 4 };
			pSelect = new int[5] { -1, -1, -1, -1, -1 };
			playerCount = 0;
			if (Global.localPlayerSlot != null)
			{
				for (int k = 0; k < buttonStat.Length; k++)
				{
					buttonStat[k] = (int)Global.localPlayerSlot[k];
					toggleAnim[k] = 4;
				}
			}
			TextureUtility.SetSpriteIndex(nextButton, 4, 1, 3);
			TextureUtility.SetSpriteIndex(backButton, 4, 1, 0);
			checkController();
			for (int l = 0; l < buttonStat.Length; l++)
			{
				if (buttonStat[l] >= 1 && buttonStat[l] <= 5)
				{
					MonoBehaviour.print("this is palyer : " + l);
					toggleAnim[l] = 1;
					pSelect[buttonStat[l] - 1] = l;
					playerCount++;
					Transform child = panel[l].transform.GetChild(0).GetChild(0);
					child.GetComponent<TextMesh>().text = Global.localPlayerName[buttonStat[l] - 1];
					child.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
					child.localPosition = new Vector3(0f, 0.25f, 1f);
					child.GetChild(0).GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 0f);
					panel[l].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
					TextureUtility.SetSpriteIndex(child.parent.gameObject, 5, 1, 4);
				}
				else if (buttonStat[l] >= 6 && buttonStat[l] <= 8)
				{
					MonoBehaviour.print("here come AI number : " + l);
					aiCount++;
					toggleAnim[l] = 1;
					Transform child2 = panel[l].transform.GetChild(0).GetChild(0);
					child2.GetComponent<TextMesh>().text = "AI";
					child2.GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
					child2.localPosition = new Vector3(0f, 0.35f, 1f);
					child2.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 0.5f);
					child2.GetChild(0).GetComponent<TextMesh>().text = aiLevel[buttonStat[l] - 6];
					panel[l].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
					TextureUtility.SetSpriteIndex(child2.parent.gameObject, 5, 1, 4);
				}
			}
			if ((playerCount > 0 && aiCount > 0) || playerCount > 1)
			{
				TextureUtility.SetSpriteIndex(nextButton, 4, 1, 0);
			}
			else
			{
				TextureUtility.SetSpriteIndex(nextButton, 4, 1, 3);
			}
			state = State.playerInput;
			break;
		}
		case State.playerInput:
			playerInput();
			animateButton();
			pointerControl();
			break;
		case State.nextScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				MonoBehaviour.print("go to next state");
				if (Global.localPlayerSlot == null)
				{
					Global.localPlayerSlot = new CharSlot[8];
				}
				for (int i = 0; i < buttonStat.Length; i++)
				{
					Global.localPlayerSlot[i] = (CharSlot)buttonStat[i];
				}
				Application.LoadLevel("LocalBattle.Settings");
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				Global.ClearBattleData();
				Loading.LoadScene("MainMenu");
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
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out array[i]) && i == num))
			{
				int num2 = -1;
				if (array[i].transform.name.Contains("button"))
				{
					num2 = int.Parse(array[i].transform.name.Substring(array[i].transform.name.Length - 1));
				}
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					int id = -1;
					try
					{
						if (togButton[i].transform.childCount > 0)
						{
							id = int.Parse(togButton[i].transform.GetChild(0).name.Substring(togButton[i].transform.GetChild(0).name.Length - 1));
						}
					}
					catch (Exception ex)
					{
						ex.ToString();
					}
					idleButton(togButton[i], id);
					togButton[i] = null;
				}
				if (num2 != -1 && array[i].transform.childCount > 0)
				{
					if (!mouseOver[i])
					{
						mouseOver[i] = true;
						GameSound.StartSFX("mouseOver");
					}
					if (toggleAnim[num2] > 1)
					{
						break;
					}
					togButton[i] = array[i].transform.gameObject;
					if (buttonStat[num2] == 0)
					{
						TextureUtility.SetSpriteIndex(togButton[i], 5, 1, 1);
					}
					else
					{
						TextureUtility.SetSpriteIndex(togButton[i], 5, 1, 2);
					}
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 5, 1, 4);
					}
					if (!controller.DoEnterRelease("Fire1"))
					{
						continue;
					}
					controller.DoPadRelease(i);
					touchPos = Vector3.zero;
					GameSound.StartSFX("menuSelect");
					int num3 = 0;
					num3 = i;
					if (buttonStat[num2] == 0)
					{
						if (pSelect[num3] == -1)
						{
							playerCount++;
							toggleAnim[num2] = 2;
							pSelect[num3] = num2;
							buttonStat[num2] = num3 + 1;
							array[i].transform.GetChild(0).GetComponent<TextMesh>().text = Global.localPlayerName[num3];
							array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
						}
						else if (pSelect[num3] != -1 && pSelect[num3] != num2)
						{
							aiCount++;
							toggleAnim[num2] = 2;
							buttonStat[num2] = 6;
							array[i].transform.GetChild(0).GetComponent<TextMesh>().text = "AI";
							array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
							array[i].transform.GetChild(0).localPosition = new Vector3(0f, 0.35f, 1f);
							array[i].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>()
								.color = new Color(1f, 1f, 1f, 0.5f);
							array[i].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>()
								.text = aiLevel[buttonStat[num2] - 6];
						}
					}
					else if (buttonStat[num2] != 0 && buttonStat[num2] <= 5)
					{
						playerCount--;
						toggleAnim[num2] = 4;
						buttonStat[num2] = 0;
						for (int j = 0; j < pSelect.Length; j++)
						{
							if (pSelect[j] == num2)
							{
								pSelect[j] = -1;
							}
						}
						array[i].transform.GetChild(0).GetComponent<TextMesh>().text = "OFF";
						array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 0.5f);
					}
					else if (buttonStat[num2] >= 6)
					{
						if (pSelect[num3] == -1)
						{
							aiCount--;
							playerCount++;
							toggleAnim[num2] = 2;
							pSelect[num3] = num2;
							buttonStat[num2] = num3 + 1;
							array[i].transform.GetChild(0).GetComponent<TextMesh>().text = Global.localPlayerName[num3];
							array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
							array[i].transform.GetChild(0).localPosition = new Vector3(0f, 0.25f, 1f);
							array[i].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>()
								.color = new Color(0f, 0f, 0f, 0f);
						}
						else if (buttonStat[num2] < 8)
						{
							buttonStat[num2]++;
							array[i].transform.GetChild(0).GetComponent<TextMesh>().text = "AI";
							array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
							array[i].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>()
								.text = aiLevel[buttonStat[num2] - 6];
						}
						else
						{
							aiCount--;
							toggleAnim[num2] = 4;
							buttonStat[num2] = 0;
							array[i].transform.GetChild(0).GetComponent<TextMesh>().text = "OFF";
							array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 0.5f);
							array[i].transform.GetChild(0).localPosition = new Vector3(0f, 0.25f, 1f);
							array[i].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>()
								.color = new Color(0f, 0f, 0f, 0f);
						}
					}
					else if (num3 == 0 && buttonStat[num2] >= 2 && buttonStat[num2] <= 5)
					{
						playerCount--;
						pSelect[buttonStat[num2] - 1] = -1;
						toggleAnim[num2] = 4;
						buttonStat[num2] = 0;
						array[i].transform.GetChild(0).GetComponent<TextMesh>().text = "OFF";
						array[i].transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0f, 0f, 0f, 0.5f);
					}
					if ((playerCount > 0 && aiCount > 0) || playerCount > 1)
					{
						TextureUtility.SetSpriteIndex(nextButton, 4, 1, 0);
					}
					else
					{
						TextureUtility.SetSpriteIndex(nextButton, 4, 1, 3);
					}
				}
				else if (array[i].transform.name.Contains("back"))
				{
					if (!mouseOver[i])
					{
						mouseOver[i] = true;
						GameSound.StartSFX("mouseOver");
					}
					TextureUtility.SetSpriteIndex(backButton, 4, 1, 1);
					togButton[i] = backButton;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(backButton, 4, 1, 2);
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						state = State.backScene;
					}
				}
				else if (array[i].transform.name.Contains("next") && ((playerCount > 0 && aiCount > 0) || playerCount > 1))
				{
					if (!mouseOver[i])
					{
						mouseOver[i] = true;
						GameSound.StartSFX("mouseOver");
					}
					TextureUtility.SetSpriteIndex(nextButton, 4, 1, 1);
					togButton[i] = nextButton;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(nextButton, 4, 1, 2);
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
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
				int id2 = -1;
				if (togButton[i].name.Contains("button"))
				{
					id2 = int.Parse(togButton[i].name.Substring(togButton[i].name.Length - 1));
				}
				idleButton(togButton[i], id2);
				togButton[i] = null;
			}
		}
	}

	private void animateButton()
	{
		for (int i = 0; i < buttonStat.Length; i++)
		{
			if (toggleAnim[i] == 4 || toggleAnim[i] == 5)
			{
				if (i < 4)
				{
					if (toggleAnim[i] == 4)
					{
						panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(-76f, panel[i].transform.localPosition.y, 0f), speed / 150f);
						if (panel[i].transform.localPosition.x <= -74f)
						{
							toggleAnim[i] = 5;
							panel[i].transform.localPosition = new Vector3(-74f, panel[i].transform.localPosition.y, 0f);
						}
					}
					else if (toggleAnim[i] == 5)
					{
						panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(-66f, panel[i].transform.localPosition.y, 0f), speed / 150f);
						if (panel[i].transform.localPosition.x >= -68f)
						{
							toggleAnim[i] = 0;
							panel[i].transform.localPosition = new Vector3(-68f, panel[i].transform.localPosition.y, 0f);
						}
					}
				}
				else if (toggleAnim[i] == 4)
				{
					panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(76f, panel[i].transform.localPosition.y, 0f), speed / 150f);
					if (panel[i].transform.localPosition.x >= 74f)
					{
						toggleAnim[i] = 5;
						panel[i].transform.localPosition = new Vector3(74f, panel[i].transform.localPosition.y, 0f);
					}
				}
				else if (toggleAnim[i] == 5)
				{
					panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(66f, panel[i].transform.localPosition.y, 0f), speed / 150f);
					if (panel[i].transform.localPosition.x <= 68f)
					{
						toggleAnim[i] = 0;
						panel[i].transform.localPosition = new Vector3(68f, panel[i].transform.localPosition.y, 0f);
					}
				}
				panel[i].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
			if (toggleAnim[i] != 2 && toggleAnim[i] != 3)
			{
				continue;
			}
			if (i < 4)
			{
				if (toggleAnim[i] == 2)
				{
					panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(-46f, panel[i].transform.localPosition.y, 0f), speed / 150f);
					if (panel[i].transform.localPosition.x >= -48f)
					{
						toggleAnim[i] = 3;
						panel[i].transform.localPosition = new Vector3(-48f, panel[i].transform.localPosition.y, 0f);
					}
				}
				else if (toggleAnim[i] == 3)
				{
					panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(-54f, panel[i].transform.localPosition.y, 0f), speed / 150f);
					if (panel[i].transform.localPosition.x <= -52f)
					{
						toggleAnim[i] = 1;
						panel[i].transform.localPosition = new Vector3(-52f, panel[i].transform.localPosition.y, 0f);
					}
				}
			}
			else if (toggleAnim[i] == 2)
			{
				panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(46f, panel[i].transform.localPosition.y, 0f), speed / 150f);
				if (panel[i].transform.localPosition.x <= 48f)
				{
					toggleAnim[i] = 3;
					panel[i].transform.localPosition = new Vector3(48f, panel[i].transform.localPosition.y, 0f);
				}
			}
			else if (toggleAnim[i] == 3)
			{
				panel[i].transform.localPosition = Vector3.Lerp(panel[i].transform.localPosition, new Vector3(54f, panel[i].transform.localPosition.y, 0f), speed / 150f);
				if (panel[i].transform.localPosition.x >= 52f)
				{
					toggleAnim[i] = 1;
					panel[i].transform.localPosition = new Vector3(52f, panel[i].transform.localPosition.y, 0f);
				}
			}
			panel[i].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		}
	}

	private void idleButton(GameObject togButton, int id)
	{
		if (id != -1)
		{
			if (buttonStat[id] == 0)
			{
				TextureUtility.SetSpriteIndex(togButton.gameObject, 5, 1, 0);
			}
			else
			{
				TextureUtility.SetSpriteIndex(togButton.gameObject, 5, 1, 3);
			}
		}
		else if (togButton.name.Contains("back"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		else if (togButton.name.Contains("next"))
		{
			if ((playerCount > 0 && aiCount > 0) || playerCount > 1)
			{
				TextureUtility.SetSpriteIndex(nextButton, 4, 1, 0);
			}
			else
			{
				TextureUtility.SetSpriteIndex(nextButton, 4, 1, 3);
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
