using UnityEngine;

public class ControlSchemes : MonoBehaviour
{
	private enum State
	{
		fadeIn = 0,
		initial = 1,
		playerInput = 2,
		nextScene = 3,
		backScene = 4
	}

	private GameObject player;

	private GameObject backBtn;

	private State state;

	private GameObject[] togButton;

	private float fadeTime;

	private bool[] mouseOver = new bool[5];

	public float speed = 45.5f;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private int pointerInit;

	private GameObject prMenu;

	private GameObject[] prList;

	private Transform prBar;

	private int prmax = 10;

	private int prPos;

	private float prStep;

	private float[] prcap = new float[2];

	private int activeTab;

	private Transform[] tabButton;

	private Transform[] tabSet;

	private Transform setBar;

	private float setBarPos;

	private int waitKey;

	private float timestamp;

	private Transform focusbtn;

	private string oldBtn;

	private string newBtn;

	private KeyCode oldKeyCode;

	private bool isKeyConflict;

	private bool conflictalt;

	private int conflictjoy;

	private int conflictkey;

	private int conflictpreset;

	private string[][,] curKey;

	private float[,] checkAxis = new float[6, 15];

	private float popupFadeSpeed;

	private float popupFadeValue;

	private int popupFadeSign;

	private Material popupFade;

	private Transform popupGUI;

	private GameObject popupCont;

	private TextMesh popupText;

	private int popupMode;

	private Vector3 touchPos;

	private bool isPress;

	private bool prFocus;

	private bool setFocus;

	private void Awake()
	{
	}

	private void Start()
	{
		togButton = new GameObject[5];
		tabButton = new Transform[5];
		tabSet = new Transform[5];
		curKey = new string[5][,];
		popupGUI = GameObject.Find("popup/ui").transform;
		popupCont = popupGUI.parent.gameObject;
		popupText = popupGUI.Find("text").GetComponent<TextMesh>();
		popupGUI.localScale = Vector3.zero;
		popupFadeValue = 0f;
		popupFadeSign = 0;
		popupFade = GameObject.Find("popup/bg").GetComponent<Renderer>().material;
		popupCont.SetActive(false);
		state = State.initial;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < curKey.Length; i++)
		{
			curKey[i] = new string[8, 2];
		}
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "btn_back")
			{
				backBtn = transform.gameObject;
			}
			else if (transform.name.Contains("p_pointer"))
			{
				player = transform.gameObject;
			}
			else if (transform.name.Equals("pr_drop"))
			{
				prMenu = transform.gameObject;
			}
			else if (transform.name.Contains("pr_tab"))
			{
				prBar = transform;
			}
			else if (transform.name.Contains("tabbtn"))
			{
				tabButton[int.Parse(transform.name.Substring(6)) - 1] = transform;
			}
			else if (transform.name.Contains("joyset"))
			{
				tabSet[int.Parse(transform.name.Substring(6)) - 1] = transform;
			}
			else if (transform.name.Contains("btn_setbar"))
			{
				setBar = transform;
			}
		}
		TabInit();
		KeySetInit();
		for (int k = 1; k < tabSet.Length; k++)
		{
			tabSet[k].gameObject.SetActive(false);
		}
		for (int l = 1; l <= 6; l++)
		{
			for (int m = 1; m <= 15; m++)
			{
				float axisRaw = Input.GetAxisRaw("Joy" + l + "Axis" + m);
				checkAxis[l - 1, m - 1] = axisRaw;
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
					popupCont.SetActive(false);
				}
			}
			popupFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, popupFadeValue / 4f));
			popupGUI.localScale = new Vector3(popupFadeValue, popupFadeValue, popupFadeValue);
		}
		float num = 2.5f;
		switch (state)
		{
		case State.fadeIn:
			base.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
			{
				fadeTime = 0f;
				state = State.playerInput;
			}
			break;
		case State.initial:
			base.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			checkController();
			state = State.fadeIn;
			break;
		case State.playerInput:
			playerInput();
			pointerControl();
			break;
		case State.backScene:
			base.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				if (Global.Level > 5)
				{
					Application.LoadLevel("Adventure.SelectStage");
				}
				else
				{
					Application.LoadLevel("Options");
				}
			}
			break;
		case State.nextScene:
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
			string text = string.Empty;
			string text2 = string.Empty;
			zero = player.transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out array[i]) && i == num && pointerInit == 0))
			{
				if (waitKey == 1 && controller.DoEnterPress("Fire1"))
				{
					controller.DoPadPress(i);
					if (Time.time - timestamp < 0.2f && focusbtn.name.Equals(array[i].transform.name))
					{
						for (int j = 1; j <= 6; j++)
						{
							for (int k = 1; k <= 15; k++)
							{
								float axisRaw = Input.GetAxisRaw("Joy" + j + "Axis" + k);
								checkAxis[j - 1, k - 1] = axisRaw;
							}
						}
						TextureUtility.SetSpriteIndex(focusbtn.gameObject, 3, 1, 2);
						oldBtn = focusbtn.GetComponentInChildren<TextMesh>().text;
						focusbtn.GetComponentInChildren<TextMesh>().text = "press any key";
						waitKey = 2;
					}
					else
					{
						focusbtn = null;
						waitKey = 0;
					}
					break;
				}
				if (waitKey == 2)
				{
					TextureUtility.SetSpriteIndex(focusbtn.gameObject, 3, 1, 2);
					int num2 = int.Parse(focusbtn.name.Substring(focusbtn.name.Length - 1)) - 1;
					int num3 = (focusbtn.name.Contains("alt") ? 1 : 0);
					if (Input.anyKeyDown)
					{
						bool flag = false;
						for (int l = 0; l < Global.validKeyCodes.Length; l++)
						{
							if (!Input.GetKey(Global.validKeyCodes[l]))
							{
								continue;
							}
							Debug.Log(Global.validKeyCodes[l]);
							text = Global.validKeyCodes[l].ToString();
							if ((text.Equals("Escape") && num2 != 7) || (text.Contains("Mouse") && num2 <= 3))
							{
								break;
							}
							if (text.Contains("Control"))
							{
								text = "Control";
							}
							else if (text.Contains("Alt"))
							{
								text = "Alt";
							}
							else if (text.Contains("Shift"))
							{
								text = "Shift";
							}
							curKey[activeTab][num2, num3] = text;
							text2 = text;
							if (text.Contains("Joystick"))
							{
								for (int m = 0; m < 11; m++)
								{
									for (int n = 0; n < 20; n++)
									{
										if (Input.GetKeyDown((KeyCode)(350 + n + m * 20)))
										{
											Debug.Log("new type KeyCode.Joystick" + (m + 1) + "Button" + n);
											curKey[activeTab][num2, num3] = "Joystick" + (m + 1) + "Button" + n;
											text2 = "Joystick" + (m + 1) + "Button" + n;
											text = "Joy" + (m + 1) + "_Btn" + n;
											flag = true;
										}
									}
								}
							}
							else
							{
								flag = true;
							}
							if (flag)
							{
								focusbtn.GetComponentInChildren<TextMesh>().text = text;
								TextureUtility.SetSpriteIndex(focusbtn.gameObject, 3, 1, 0);
								waitKey = 0;
								break;
							}
						}
					}
					if (num2 != 7)
					{
						for (int num4 = 1; num4 <= 6; num4++)
						{
							for (int num5 = 1; num5 <= 15; num5++)
							{
								float axisRaw2 = Input.GetAxisRaw("Joy" + num4 + "Axis" + num5);
								if (Mathf.Abs(axisRaw2) > 0.8f && controller.DoPadRelease(-1) && axisRaw2 != checkAxis[num4 - 1, num5 - 1])
								{
									Debug.Log(axisRaw2 + " < " + checkAxis[num4 - 1, num5 - 1]);
									if (axisRaw2 > 0.8f)
									{
										focusbtn.GetComponentInChildren<TextMesh>().text = "+Joy" + num4 + "Axis" + num5;
									}
									else if (axisRaw2 < -0.8f)
									{
										focusbtn.GetComponentInChildren<TextMesh>().text = "-Joy" + num4 + "Axis" + num5;
									}
									curKey[activeTab][num2, num3] = focusbtn.GetComponentInChildren<TextMesh>().text;
									text = curKey[activeTab][num2, num3];
									text2 = text;
									TextureUtility.SetSpriteIndex(focusbtn.gameObject, 3, 1, 0);
									waitKey = 0;
									break;
								}
								if (controller.DoEnterRelease("0"))
								{
									controller.DoPadRelease(i);
								}
							}
							if (waitKey == 0)
							{
								break;
							}
						}
					}
					if (waitKey != 0)
					{
						break;
					}
					Debug.Log("check conflit : " + text2 + " , " + text);
					for (int num6 = 0; num6 < curKey[activeTab].GetLength(0); num6++)
					{
						if (curKey[activeTab][num6, 0].Equals(text2) && (num6 != num2 || num3 != 0))
						{
							curKey[activeTab][num6, 0] = string.Empty;
							tabSet[activeTab].Find("key" + (num6 + 1)).Find("btn_setbutton" + (num6 + 1)).GetComponentInChildren<TextMesh>()
								.text = string.Empty;
						}
						if (curKey[activeTab][num6, 1].Equals(text2) && (num6 != num2 || num3 != 1))
						{
							curKey[activeTab][num6, 1] = string.Empty;
							tabSet[activeTab].Find("key" + (num6 + 1)).Find("btn_setalt" + (num6 + 1)).GetComponentInChildren<TextMesh>()
								.text = string.Empty;
						}
					}
					for (int num7 = 0; num7 < curKey.Length; num7++)
					{
						for (int num8 = 0; num8 < curKey[num7].GetLength(0); num8++)
						{
							Debug.Log("j : " + num7 + " , a : " + activeTab);
							if ((curKey[num7][num8, 0].Equals(text2) || curKey[num7][num8, 1].Equals(text2)) && num7 != activeTab)
							{
								if (curKey[num7][num8, 0].Equals(text2))
								{
									conflictalt = false;
								}
								else
								{
									conflictalt = true;
								}
								conflictjoy = num7;
								conflictkey = num8;
								conflictpreset = GameSetting.playerLayout[num7, 0];
								isKeyConflict = true;
								popupText.text = "This key is already used by another controller!\nDo you want to apply this button?";
								popupFadeSign = 1;
								popupFadeSpeed = 12f * Time.smoothDeltaTime;
								popupMode = 3;
								popupCont.SetActive(true);
								return;
							}
						}
					}
					if (GameSetting.playerLayout[activeTab, 1] <= 1)
					{
						GameSetting.addPreset(curKey[activeTab], activeTab);
						TabInit();
						prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
						KeySetInit(activeTab);
						DisableButton(base.transform.Find("btn_prdelete"), false);
					}
					else
					{
						GameSetting.ApplyNewKey(curKey[activeTab], activeTab);
					}
					GameSetting.ApplyControlConfig();
					break;
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (prFocus && controller.DoEnterRelease("Fire1"))
				{
					controller.DoPadRelease(i);
					float num9 = ((float)(GameSetting.presetName.Length - prList.Length) / 2f - (float)prPos) * prStep;
					Debug.Log(num9);
					prBar.localPosition = new Vector3(num9, 0f, 3f);
					prFocus = false;
					break;
				}
				if (setFocus)
				{
					if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						Vector3 localPosition = tabSet[activeTab].localPosition;
						Vector3 localPosition2 = setBar.localPosition;
						if (localPosition.y % 10f > 0f && localPosition.y % 10f < 2.5f)
						{
							tabSet[activeTab].localPosition = new Vector3(localPosition.x, localPosition.y - localPosition.y % 10f, localPosition.z);
							setBar.localPosition = new Vector3(localPosition2.x, 0f - (localPosition.y - localPosition.y % 10f) * 1.1f / 65f + 0.55f, localPosition2.z);
						}
						else if (localPosition.y % 10f > 2.5f && localPosition.y % 10f < 5f)
						{
							tabSet[activeTab].localPosition = new Vector3(localPosition.x, localPosition.y + (5f - localPosition.y % 10f), localPosition.z);
							setBar.localPosition = new Vector3(localPosition2.x, 0f - (localPosition.y - localPosition.y % 10f) * 1.1f / 65f + 0.55f, localPosition2.z);
						}
						else if (localPosition.y % 10f > 5f)
						{
							tabSet[activeTab].localPosition = new Vector3(localPosition.x, localPosition.y - (localPosition.y % 10f - 5f), localPosition.z);
							setBar.localPosition = new Vector3(localPosition2.x, 0f - (localPosition.y - localPosition.y % 10f) * 1.1f / 65f + 0.55f, localPosition2.z);
						}
						TextureUtility.SetSpriteIndex(setBar.gameObject, 3, 1, 0);
						isPress = false;
						setFocus = false;
					}
					else
					{
						MoveBar(controller);
					}
					continue;
				}
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					idleButton(togButton[i]);
					togButton[i] = null;
				}
				if (popupCont.activeSelf)
				{
					if (!array[i].transform.name.Contains("btn_cf"))
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
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 2);
						isPress = true;
						Transform child = togButton[i].transform.GetChild(0);
						Vector3 localPosition3 = child.localPosition;
						if (localPosition3.x == 0f && localPosition3.y == 0f)
						{
							localPosition3.x = -0.03f;
							localPosition3.y = -0.06f;
							child.localPosition = localPosition3;
						}
					}
					else
					{
						if (!controller.DoEnterRelease("Fire1"))
						{
							continue;
						}
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						MonoBehaviour.print("click at : " + togButton[i].name);
						Transform child2 = togButton[i].transform.GetChild(0);
						Vector3 localPosition4 = child2.localPosition;
						if (localPosition4.x != 0f || localPosition4.y != 0f)
						{
							localPosition4.x = 0f;
							localPosition4.y = 0f;
							child2.localPosition = localPosition4;
						}
						if (togButton[i].name.Equals("btn_cf_ok"))
						{
							Debug.Log("popup mode = " + popupMode);
							if (popupMode == 1)
							{
								Debug.Log("add new preset");
								GameSetting.addPreset(curKey[activeTab], activeTab);
								TabInit();
								prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
								KeySetInit(activeTab);
								GameSetting.ApplyControlConfig();
								DisableButton(base.transform.Find("btn_prdelete"), false);
							}
							else if (popupMode == 2)
							{
								if (GameSetting.DeletePreset(activeTab))
								{
									TabInit();
									prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
									KeySetInit();
									GameSetting.ApplyControlConfig();
								}
							}
							else if (popupMode == 3)
							{
								if (GameSetting.playerLayout[activeTab, 1] <= 1)
								{
									GameSetting.addPreset(curKey[activeTab], activeTab);
									TabInit();
									prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
									KeySetInit(activeTab);
									DisableButton(base.transform.Find("btn_prdelete"), false);
								}
								else
								{
									GameSetting.ApplyNewKey(curKey[activeTab], activeTab);
								}
								GameSetting.ApplyControlConfig();
							}
							else if (popupMode == 4)
							{
								fadeTime = 0f;
								state = State.backScene;
							}
							popupFadeSign = -1;
						}
						else
						{
							if (popupMode == 3)
							{
								focusbtn.GetComponentInChildren<TextMesh>().text = oldBtn;
							}
							popupMode = 0;
							popupFadeSign = -1;
						}
						GameSound.StartSFX("menuSelect");
					}
					continue;
				}
				if (array[i].transform.name.Contains("tabbtn"))
				{
					togButton[i] = array[i].transform.gameObject;
					int num10 = int.Parse(togButton[i].name.Substring(6)) - 1;
					if (!isPress && num10 != activeTab)
					{
						TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 1);
					}
					if (controller.DoEnterPress("Fire1"))
					{
						Vector3 localPosition5 = togButton[i].transform.parent.localPosition;
						tabButton[activeTab].parent.localScale = new Vector3(1f, 1f, 1f);
						tabButton[activeTab].GetChild(0).localScale = new Vector3(0.033f, 0.188f, 1f);
						tabButton[activeTab].GetComponentInChildren<TextMesh>().color = new Color(0.5f, 1f, 1f, 0.5f);
						TextureUtility.SetSpriteIndex(tabButton[activeTab].gameObject, 4, 1, 0);
						togButton[i].transform.parent.localScale = new Vector3(1f, 1.5f, 1f);
						togButton[i].transform.parent.localPosition = new Vector3(localPosition5.x, localPosition5.y, 6f);
						togButton[i].transform.GetChild(0).localScale = new Vector3(0.033f, 0.138f, 1f);
						togButton[i].transform.GetComponentInChildren<TextMesh>().color = new Color(1f, 1f, 1f, 1f);
						togButton[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
						TextureUtility.SetSpriteIndex(togButton[i], 4, 1, 3);
						tabSet[activeTab].localPosition = new Vector3(0f, 0f, 35f);
						tabSet[activeTab].gameObject.SetActive(false);
						tabSet[num10].gameObject.SetActive(true);
						tabSet[num10].localPosition = new Vector3(0f, 0f, 30f);
						activeTab = num10;
						for (int num11 = 1; num11 < 5; num11++)
						{
							if (activeTab + num11 < tabButton.Length)
							{
								Vector3 localPosition6 = tabButton[activeTab + num11].parent.localPosition;
								tabButton[activeTab + num11].parent.localPosition = new Vector3(localPosition6.x, localPosition6.y, 11 + num11);
								TextureUtility.SetSpriteIndex(tabButton[activeTab + num11].gameObject, 4, 1, 0);
								tabButton[activeTab + num11].GetComponent<Renderer>().material.SetColor("_TintColor", new Color((float)(120 - (num11 - 1) * 20) / 255f, (float)(130 - (num11 - 1) * 20) / 255f, (float)(140 - (num11 - 1) * 20) / 255f, 0.5f));
							}
							if (activeTab - num11 >= 0)
							{
								Vector3 localPosition7 = tabButton[activeTab - num11].parent.localPosition;
								tabButton[activeTab - num11].parent.localPosition = new Vector3(localPosition7.x, localPosition7.y, 11 + num11);
								TextureUtility.SetSpriteIndex(tabButton[activeTab - num11].gameObject, 4, 1, 0);
								tabButton[activeTab - num11].GetComponent<Renderer>().material.SetColor("_TintColor", new Color((float)(120 - (num11 - 1) * 20) / 255f, (float)(130 - (num11 - 1) * 20) / 255f, (float)(140 - (num11 - 1) * 20) / 255f, 0.5f));
							}
						}
						if (activeTab + 1 < tabButton.Length)
						{
							TextureUtility.SetSpriteIndex(tabButton[activeTab + 1].gameObject, 4, 1, 2);
						}
						if (activeTab - 1 >= 0)
						{
							TextureUtility.SetSpriteIndex(tabButton[activeTab - 1].gameObject, 4, 1, 2);
						}
						setBar.localPosition = new Vector3(0f, 0.55f, 1f);
						prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
						if (activeTab == 0)
						{
							prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Renderer>().enabled = false;
							prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Collider>().enabled = false;
						}
						else
						{
							prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Renderer>().enabled = true;
							prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Collider>().enabled = true;
						}
						if (GameSetting.playerLayout[activeTab, 1] <= 1)
						{
							DisableButton(base.transform.Find("btn_prdelete"), true);
						}
						else
						{
							DisableButton(base.transform.Find("btn_prdelete"), false);
						}
						idleButton(togButton[i]);
						togButton[i] = null;
					}
				}
				else if (array[i].transform.name.Contains("btn"))
				{
					togButton[i] = array[i].transform.gameObject;
					if (togButton[i].name.Contains("pr_drop"))
					{
						togButton[i] = prMenu.transform.parent.Find("pr_dropbtn").gameObject;
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
						if (togButton[i].name.Contains("back"))
						{
							for (int num12 = 0; num12 < GameSetting.playerLayout.GetLength(0); num12++)
							{
								if (GameSetting.playerLayout[num12, 1] <= 1)
								{
									continue;
								}
								for (int num13 = 0; num13 < curKey[num12].GetLength(0); num13++)
								{
									if (curKey[num12][num13, 0].Equals(string.Empty) && curKey[num12][num13, 1].Equals(string.Empty))
									{
										popupText.text = "There is an incomplete input mapping!\nDo you want to continue?";
										popupFadeSign = 1;
										popupFadeSpeed = 12f * Time.smoothDeltaTime;
										popupMode = 4;
										popupCont.SetActive(true);
										return;
									}
								}
							}
							touchPos = Vector3.zero;
							GameSound.StartSFX("menuSelect");
							fadeTime = 0f;
							state = State.backScene;
						}
						else if (togButton[i].name.Contains("pr_drop"))
						{
							if (!prMenu.activeSelf)
							{
								prMenu.SetActive(true);
							}
							else
							{
								prMenu.SetActive(false);
							}
						}
						else if (togButton[i].name.Contains("btn_set"))
						{
							Debug.Log("set button");
							focusbtn = togButton[i].transform;
							waitKey = 1;
							timestamp = Time.time;
						}
						else if (togButton[i].name.Contains("prsave"))
						{
							controller.DoPadRelease(i);
							touchPos = Vector3.zero;
							GameSound.StartSFX("menuSelect");
							idleButton(togButton[i]);
							popupText.text = "Do you want to copy this Preset?";
							popupFadeSign = 1;
							popupFadeSpeed = 12f * Time.smoothDeltaTime;
							popupMode = 1;
							popupCont.SetActive(true);
						}
						else if (togButton[i].name.Contains("prdelete"))
						{
							controller.DoPadRelease(i);
							touchPos = Vector3.zero;
							GameSound.StartSFX("menuSelect");
							idleButton(togButton[i]);
							popupText.text = "Do you want to delete this Preset and\nrevert to default control settings?";
							popupFadeSign = 1;
							popupFadeSpeed = 12f * Time.smoothDeltaTime;
							popupMode = 2;
							popupCont.SetActive(true);
						}
						else if (togButton[i].name.Contains("reset") && GameSetting.ClearPreset())
						{
							TabInit();
							KeySetInit();
							GameSetting.ApplyControlConfig();
						}
						isPress = false;
						idleButton(togButton[i]);
						togButton[i] = null;
						break;
					}
				}
				if (array[i].transform.name.Contains("setbar") || setFocus)
				{
					MoveBar(controller);
				}
				if (array[i].transform.name.Contains("set"))
				{
					Vector3 localPosition8 = tabSet[activeTab].localPosition;
					Vector3 localPosition9 = setBar.localPosition;
					if (Input.GetAxis("mousewheel") < 0f && localPosition8.y < 65f)
					{
						tabSet[activeTab].localPosition = new Vector3(localPosition8.x, localPosition8.y + 5f, localPosition8.z);
						setBar.localPosition = new Vector3(localPosition9.x, 0f - (localPosition8.y + 5f) * 1.1f / 65f + 0.55f, localPosition9.z);
					}
					else if (Input.GetAxis("mousewheel") > 0f && localPosition8.y > 0f)
					{
						tabSet[activeTab].localPosition = new Vector3(localPosition8.x, localPosition8.y - 5f, localPosition8.z);
						setBar.localPosition = new Vector3(localPosition9.x, 0f - (localPosition8.y - 5f) * 1.1f / 65f + 0.55f, localPosition9.z);
					}
				}
				if (!prMenu.activeSelf)
				{
					continue;
				}
				if (array[i].transform.name.Contains("pr_tab") || prFocus)
				{
					Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					if (!Cursor.visible)
					{
						vector = player.transform.position;
					}
					if (array[i].transform.name.Equals("pr_tab"))
					{
						prBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.3f, 0.5f, 0.5f, 0.5f));
					}
					else if (!prFocus)
					{
						prBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.5f));
					}
					if (controller.DoEnterHold("Fire1") || prFocus)
					{
						controller.DoPadPress(i);
						prFocus = true;
						prPos = Mathf.RoundToInt(Mathf.Abs(prBar.localPosition.x - (1f - prBar.localScale.x)) / prStep);
						UpdateMenu(prList, GameSetting.presetName, prPos);
						if (vector.y >= prcap[0])
						{
							prBar.localPosition = new Vector3(1f - prBar.localScale.x, 0f, 3f);
							break;
						}
						if (vector.y <= prcap[1])
						{
							prBar.localPosition = new Vector3(0f - (1f - prBar.localScale.x), 0f, 3f);
							break;
						}
						prBar.position = new Vector3(prBar.position.x, vector.y, prBar.position.z);
						prBar.localPosition = new Vector3(prBar.localPosition.x, 0f, 3f);
					}
				}
				else if (array[i].transform.name.Contains("pr_txt"))
				{
					togButton[i] = array[i].transform.gameObject;
					TextMesh component = togButton[i].transform.GetComponent<TextMesh>();
					component.color = new Color(1f, 1f, 1f, 1f);
					int num14 = int.Parse(component.name.Substring(component.name.IndexOf(")") + 1));
					prBar.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 0.5f));
					if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[num14 + prPos];
						GameSound.StartSFX("menuSelect");
						GameSetting.playerLayout[activeTab, 1] = num14 + prPos;
						GameSetting.playerLayout[activeTab, 0] = GameSetting.presetContainer[num14 + prPos].presetID;
						KeySetInit(activeTab);
						prMenu.SetActive(false);
						if (GameSetting.playerLayout[activeTab, 1] <= 1)
						{
							DisableButton(base.transform.Find("btn_prdelete"), true);
						}
						else
						{
							DisableButton(base.transform.Find("btn_prdelete"), false);
						}
						idleButton(togButton[i]);
						togButton[i] = null;
						break;
					}
				}
				if (!array[i].transform.name.Contains("pr") && !prFocus)
				{
					prMenu.SetActive(false);
					continue;
				}
				float num15 = Mathf.Round(prBar.localPosition.x * 100f);
				if (Input.GetAxis("mousewheel") > 0f && num15 < Mathf.Round((1f - prBar.localScale.x) * 100f))
				{
					prBar.localPosition = new Vector3(prBar.localPosition.x + prStep, 0f, 3f);
					prPos--;
					UpdateMenu(prList, GameSetting.presetName, prPos);
				}
				else if (Input.GetAxis("mousewheel") < 0f && num15 > 0f - Mathf.Round((1f - prBar.localScale.x) * 100f))
				{
					prBar.localPosition = new Vector3(prBar.localPosition.x - prStep, 0f, 3f);
					prPos++;
					UpdateMenu(prList, GameSetting.presetName, prPos);
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
		if (togButton.name.Contains("tabbtn"))
		{
			int num = int.Parse(togButton.name.Substring(6)) - 1;
			if (num == activeTab + 1 || num == activeTab - 1)
			{
				TextureUtility.SetSpriteIndex(togButton, 4, 1, 2);
			}
			else if (num != activeTab)
			{
				TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
			}
		}
		else if (togButton.name.Contains("btn_cf"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		else if (togButton.name.Contains("btn"))
		{
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
		}
		else if (togButton.name.Contains("pr_txt") || togButton.name.Contains("setdrop_txt"))
		{
			togButton.transform.GetComponent<TextMesh>().color = new Color(0f, 0.27f, 0.5f, 0.65f);
		}
		isPress = false;
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
			if (controller.IsConnected())
			{
				StickPointer(controller, i);
			}
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
		prMenu.SetActive(true);
		Transform[] componentsInChildren = prMenu.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("Clone"))
			{
				Object.Destroy(transform.gameObject);
			}
		}
		prPos = 0;
		int num = ((GameSetting.presetName.Length < prmax) ? GameSetting.presetName.Length : prmax);
		prList = new GameObject[num];
		Transform transform2 = prMenu.transform.Find("pr_bg");
		Transform transform3 = prMenu.transform.Find("pr_tail");
		transform2.localScale = new Vector3(1f, 0f - 0.89f * (float)(num + 1), 1f);
		transform2.localPosition = new Vector3(0f, 0f - 0.89f * (float)num, 1f);
		transform3.localPosition = new Vector3(0f, 0f - 0.89f * (float)num * 2f, 2f);
		Transform transform4 = prMenu.transform.Find("pr_txt");
		Transform transform5 = prMenu.transform.Find("pr_prop2");
		for (int j = 0; j < num; j++)
		{
			prList[j] = Object.Instantiate(transform4.gameObject, transform4.position, transform4.rotation) as GameObject;
			prList[j].name = prList[j].name + j;
			prList[j].transform.parent = prMenu.transform;
			prList[j].transform.localScale = transform4.localScale;
			prList[j].GetComponent<TextMesh>().text = GameSetting.presetName[j];
			prList[j].transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((j + 1) * 2) + 0.4f), 5f);
			prList[j].SetActive(true);
			if (j < num - 1)
			{
				GameObject gameObject = Object.Instantiate(transform5.gameObject, transform5.position, transform5.rotation) as GameObject;
				gameObject.transform.parent = prMenu.transform;
				gameObject.transform.localScale = transform5.localScale;
				gameObject.transform.localPosition = new Vector3(0f, 0f - (0.89f * (float)((j + 1) * 2) + 0.89f), 2f);
				gameObject.SetActive(true);
			}
		}
		transform4.gameObject.SetActive(false);
		transform5.gameObject.SetActive(false);
		prBar.parent.localScale = new Vector3(0.89f * (float)num - 0.32f, 0.05f, 1f);
		prBar.parent.localPosition = new Vector3(-0.878f, -0.89f * (float)(num + 1), 3f);
		prBar.localScale = new Vector3((float)num / (float)GameSetting.presetName.Length, 1f, 1f);
		prBar.localPosition = new Vector3(0f - (1f - prBar.localScale.x), 0f, 3f);
		prcap[1] = prBar.position.y;
		prBar.localPosition = new Vector3(1f - prBar.localScale.x, 0f, 3f);
		prcap[0] = prBar.position.y;
		prStep = (1f - prBar.localScale.x) * 2f / (float)(GameSetting.presetName.Length - num);
		prMenu.transform.parent.Find("pr_seltxt").GetComponent<TextMesh>().text = GameSetting.presetName[GameSetting.playerLayout[activeTab, 1]];
		if (activeTab == 0)
		{
			prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Renderer>().enabled = false;
			prMenu.transform.Find("pr_txt(Clone)0").GetComponent<Collider>().enabled = false;
		}
		if (GameSetting.playerLayout[activeTab, 1] <= 1)
		{
			DisableButton(base.transform.Find("btn_prdelete"), true);
		}
		prMenu.SetActive(false);
	}

	private void KeySetInit()
	{
		for (int i = 0; i < 5; i++)
		{
			JoyLayout value;
			if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[i, 1], out value) || GameSetting.playerLayout[i, 0] == 1)
			{
				value = GameSetting.defaultLayout[i];
			}
			tabSet[i].gameObject.SetActive(true);
			for (int j = 0; j < 8; j++)
			{
				Transform transform = tabSet[i].Find("key" + (j + 1));
				TextMesh componentInChildren = transform.Find("btn_setbutton" + (j + 1)).GetComponentInChildren<TextMesh>();
				TextMesh componentInChildren2 = transform.Find("btn_setalt" + (j + 1)).GetComponentInChildren<TextMesh>();
				curKey[i][j, 0] = value.game[j, 0];
				curKey[i][j, 1] = value.game[j, 1];
				componentInChildren.text = TrimKey(value.game[j, 0]);
				componentInChildren2.text = TrimKey(value.game[j, 1]);
			}
			if (i != activeTab)
			{
				tabSet[i].gameObject.SetActive(false);
			}
		}
	}

	private void KeySetInit(int joy)
	{
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[joy, 1], out value) || GameSetting.playerLayout[joy, 1] == 1)
		{
			value = GameSetting.defaultLayout[joy];
		}
		for (int i = 0; i < 8; i++)
		{
			Transform transform = tabSet[joy].Find("key" + (i + 1));
			TextMesh componentInChildren = transform.Find("btn_setbutton" + (i + 1)).GetComponentInChildren<TextMesh>();
			TextMesh componentInChildren2 = transform.Find("btn_setalt" + (i + 1)).GetComponentInChildren<TextMesh>();
			curKey[joy][i, 0] = value.game[i, 0];
			curKey[joy][i, 1] = value.game[i, 1];
			componentInChildren.text = TrimKey(value.game[i, 0]);
			componentInChildren2.text = TrimKey(value.game[i, 1]);
		}
	}

	private string TrimKey(string key)
	{
		if (key.Contains("Joystick"))
		{
			int num = key.IndexOf('k');
			int num2 = key.IndexOf('B');
			int num3 = key.IndexOf('n');
			return "Joy" + key.Substring(num + 1, num2 - num - 1) + "_Btn" + key.Substring(num3 + 1);
		}
		return key;
	}

	private void UpdateMenu(GameObject[] textList, string[] tempList, int pos)
	{
		for (int i = 0; i < textList.Length; i++)
		{
			textList[i].GetComponent<TextMesh>().text = tempList[i + pos];
		}
	}

	private void UpdateMenu(int tab, GameObject[,] textList, string[] tempList, int pos)
	{
		for (int i = 0; i < textList.GetLength(1); i++)
		{
			textList[tab, i].GetComponent<TextMesh>().text = tempList[i + pos];
		}
	}

	private void DisableButton(Transform obj, bool disable)
	{
		if (disable)
		{
			obj.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.3f, 0.3f, 0.3f, 0.5f));
			obj.GetComponent<Collider>().enabled = false;
		}
		else
		{
			obj.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			obj.GetComponent<Collider>().enabled = true;
		}
	}

	private void MoveBar(GameController ctrl)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 position = setBar.position;
		Vector3 localPosition = tabSet[activeTab].localPosition;
		if (!Cursor.visible)
		{
			vector = player.transform.position;
		}
		if (ctrl.DoEnterHold("Fire1") || setFocus)
		{
			ctrl.DoPadPress(0);
			setFocus = true;
			TextureUtility.SetSpriteIndex(setBar.gameObject, 3, 1, 2);
			if ((double)vector.y >= 115.65)
			{
				setBar.localPosition = new Vector3(0f, 0.55f, 1f);
				return;
			}
			if ((double)vector.y <= 68.35)
			{
				setBar.localPosition = new Vector3(0f, -0.55f, 1f);
				return;
			}
			setBar.position = new Vector3(position.x, vector.y, position.z);
			setBar.localPosition = new Vector3(0f, setBar.localPosition.y, 3f);
			tabSet[activeTab].localPosition = new Vector3(localPosition.x, (0f - setBar.localPosition.y + 0.55f) * 65f / 1.1f, localPosition.z);
		}
	}
}
