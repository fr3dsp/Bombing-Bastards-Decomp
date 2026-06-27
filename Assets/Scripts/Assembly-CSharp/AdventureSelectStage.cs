using UnityEngine;

public class AdventureSelectStage : MonoBehaviour
{
	private enum State
	{
		initial = 0,
		fadeIn = 1,
		startAnim = 2,
		playerInput = 3,
		outAnim = 4,
		nextScene = 5,
		backScene = 6
	}

	private GameObject player;

	private GameObject[] stageBtn;

	private GameObject[] planet;

	private GameObject[] propLine3;

	private GameObject[] planetProp;

	private Vector3[,] pPropPos = new Vector3[5, 4]
	{
		{
			new Vector3(-26f, 22f, -6f),
			new Vector3(-14f, -4f, -6f),
			new Vector3(26f, 18f, -6f),
			new Vector3(16f, -12f, -6f)
		},
		{
			new Vector3(-22f, 20f, -6f),
			new Vector3(-10f, 0f, -6f),
			new Vector3(12f, 22f, -6f),
			new Vector3(0f, -8f, -6f)
		},
		{
			new Vector3(-32f, 22f, -6f),
			new Vector3(-12f, -4f, -6f),
			new Vector3(24f, 22f, -6f),
			new Vector3(16f, -12f, -6f)
		},
		{
			new Vector3(-40f, 20f, -6f),
			new Vector3(-26f, -2f, -6f),
			new Vector3(26f, 26f, -6f),
			new Vector3(28f, -10f, -6f)
		},
		{
			new Vector3(-30f, 22f, -6f),
			new Vector3(-12f, -2f, -6f),
			new Vector3(20f, 22f, -6f),
			new Vector3(10f, 10f, -6f)
		}
	};

	private GameObject backBtn;

	private GameObject plBtn;

	private GameObject prBtn;

	private GameObject planetName;

	private GameObject stageBoss;

	private GameObject propGrid;

	private GameObject propLine1;

	private GameObject propLine2;

	private GameObject propPLine1;

	private GameObject propPLine2;

	private GameObject txtArea;

	private GameObject compPoint;

	private GameObject numFrame;

	private GameObject numCircle;

	private State state;

	private GameObject[] togButton;

	private float fadeTime;

	private string stageName = string.Empty;

	private int stageID;

	private bool changePlanet;

	private int animState;

	private float animTime1;

	private float animTime2;

	private float animTime3;

	private float outTime1;

	private float outTime2;

	private float inTime1;

	private float inTime2;

	private float decoTime1;

	private float propTime1;

	private float propTime2;

	private float propTime3;

	private int propState1;

	private int propState2;

	private float timeStamp;

	private float propMove;

	private float propstart;

	private bool[] mouseOver = new bool[5];

	private int pointerInit;

	public float speed = 45.5f;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private Vector3 touchPos;

	private bool isPress;

	private bool mouseFocus;

	private void Awake()
	{
	}

	private void Start()
	{
		base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		for (int i = 1; i <= 5; i++)
		{
			GameSound.StopBGM("Map" + i.ToString("D2"));
		}
		GameSound.StopBGM("BossFight");
		GameSound.StartBGM("Menu");
		togButton = new GameObject[5];
		stageBtn = new GameObject[7];
		planet = new GameObject[5];
		propLine3 = new GameObject[9];
		planetProp = new GameObject[4];
		state = State.initial;
		stageID = Global.advPlanet;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "btn_back")
			{
				backBtn = transform.gameObject;
			}
			else if (transform.name == "btn_planet_l")
			{
				plBtn = transform.gameObject;
			}
			else if (transform.name == "btn_planet_r")
			{
				prBtn = transform.gameObject;
			}
			else if (transform.name.Contains("pointer"))
			{
				player = transform.gameObject;
			}
			else if (transform.name.Contains("btn_stage"))
			{
				int num = int.Parse(transform.name.Substring(transform.name.Length - 1));
				stageBtn[num - 1] = transform.gameObject;
			}
			else if (transform.name.Contains("planet"))
			{
				int num2 = int.Parse(transform.name.Substring(transform.name.Length - 1));
				planet[num2 - 1] = transform.gameObject;
			}
			else if (transform.name.Contains("line3_sub"))
			{
				int num3 = int.Parse(transform.name.Substring(transform.name.Length - 1));
				propLine3[num3] = transform.gameObject;
			}
			else if (transform.name.Contains("pp_center"))
			{
				int num4 = int.Parse(transform.name.Substring(transform.name.Length - 1));
				planetProp[num4 - 1] = transform.gameObject;
			}
			else if (transform.name == "prop_grid")
			{
				propGrid = transform.gameObject;
			}
			else if (transform.name == "prop_line1")
			{
				propLine1 = transform.gameObject;
			}
			else if (transform.name == "prop_line2")
			{
				propLine2 = transform.gameObject;
			}
			else if (transform.name == "name")
			{
				planetName = transform.gameObject;
			}
			else if (transform.name == "stage_boss")
			{
				stageBoss = transform.gameObject;
			}
			else if (transform.name == "prop_starline1")
			{
				propPLine1 = transform.gameObject;
			}
			else if (transform.name == "prop_starline2")
			{
				propPLine2 = transform.gameObject;
			}
			else if (transform.name == "txt_area")
			{
				txtArea = transform.gameObject;
			}
			else if (transform.name == "comp_point")
			{
				compPoint = transform.gameObject;
			}
			else if (transform.name == "num_frame")
			{
				numFrame = transform.gameObject;
			}
			else if (transform.name == "num_circle")
			{
				numCircle = transform.gameObject;
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
				state = State.startAnim;
			}
			break;
		case State.initial:
		{
			for (int l = 0; l < planet.Length; l++)
			{
				planet[l].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
			for (int m = 0; m < propLine3.Length; m++)
			{
				propLine3[m].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
			for (int n = 0; n < stageBtn.Length; n++)
			{
				Vector3 localPosition5 = stageBtn[n].transform.localPosition;
				stageBtn[n].transform.localPosition = new Vector3(localPosition5.x, -66f, localPosition5.z);
			}
			for (int num7 = 0; num7 < planetProp.Length; num7++)
			{
				planetProp[num7].transform.localScale = new Vector3(0f, 1f, 1f);
				planetProp[num7].transform.GetChild(0).GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(0f, 1f));
				planetProp[num7].transform.GetChild(1).localScale = new Vector3(0f, 0f, 1f);
			}
			plBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			prBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			plBtn.transform.GetChild(0).transform.localPosition = new Vector3(1.8f, 0f, 0f);
			prBtn.transform.GetChild(0).transform.localPosition = new Vector3(1.8f, 0f, 0f);
			planetName.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propGrid.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propPLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			propPLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			txtArea.GetComponent<TextMesh>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
			txtArea.transform.localPosition = new Vector3(-15f, 0f, 0f);
			compPoint.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			compPoint.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			compPoint.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			GameObject.Find("comp_frame").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			numFrame.transform.localScale = new Vector3(0f, 1f, 1f);
			numFrame.transform.GetChild(1).localScale = new Vector3(0f, 0f, 1f);
			numFrame.transform.GetChild(0).GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(0f, 1f));
			for (int num8 = 0; num8 < 3; num8++)
			{
				numCircle.transform.GetChild(num8).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
			SetUp(1);
			checkController();
			state = State.fadeIn;
			break;
		}
		case State.startAnim:
			if (animState <= 1)
			{
				animTime1 += Time.deltaTime * 1.8f;
				float num4 = Mathf.Lerp(0f, 1f, animTime1);
				float num5 = Mathf.Lerp(0f, 1f, animTime1 - 0.8f);
				float num6 = Mathf.Lerp(0f, 1f, animTime1 - 1.2f);
				for (int i = 0; i < planet.Length; i++)
				{
					if (i == Global.advPlanet)
					{
						planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.5f)));
					}
					else if (i == Global.advPlanet - 1)
					{
						planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.33f, animTime1 - 0.5f)));
					}
					else if (i == Global.advPlanet - 2)
					{
						planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.16f, animTime1 - 0.5f)));
					}
					else if (i == Global.advPlanet + 1)
					{
						planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.33f, animTime1 - 0.5f)));
					}
					else if (i == Global.advPlanet + 2)
					{
						planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.16f, animTime1 - 0.5f)));
					}
				}
				for (int j = 0; j < propLine3.Length; j++)
				{
					propLine3[j].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.5f)));
				}
				plBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.8f)));
				prBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.8f)));
				if (animTime1 >= 0.8f)
				{
					Vector3 localPosition = plBtn.transform.GetChild(0).transform.localPosition;
					Vector3 localPosition2 = Vector3.Lerp(localPosition, new Vector3(0f, localPosition.y, localPosition.z), 0.1f);
					Vector3 localPosition3 = txtArea.transform.transform.localPosition;
					Vector3 localPosition4 = Vector3.Lerp(localPosition3, new Vector3(0f, localPosition3.y, localPosition3.z), 0.1f);
					plBtn.transform.GetChild(0).transform.localPosition = localPosition2;
					prBtn.transform.GetChild(0).transform.localPosition = localPosition2;
					txtArea.transform.localPosition = localPosition4;
				}
				propPLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				propPLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				propGrid.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				propLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				propLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				txtArea.GetComponent<TextMesh>().color = new Color(0.87f, 1f, 1f, Mathf.Lerp(0f, 1f, animTime1 - 0.8f));
				propPLine1.transform.parent.localScale = new Vector3(num5, num5, 1f);
				propPLine2.transform.parent.localScale = new Vector3(num5, num5, 1f);
				planet[0].transform.parent.localScale = new Vector3(num5, num5, 1f);
				propLine1.transform.parent.localScale = new Vector3(num6, num6, 1f);
				propLine2.transform.parent.localScale = new Vector3(num6, num6, 1f);
				compPoint.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				compPoint.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				compPoint.transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				GameObject.Find("comp_frame").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				for (int k = 0; k < 3; k++)
				{
					numCircle.transform.GetChild(k).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime1 - 0.25f)));
				}
				if (plBtn.transform.GetChild(0).transform.localPosition.x <= 0.02f)
				{
					animState = 1;
				}
			}
			if (animState <= 2 && animState >= 1 && propLine2.transform.parent.localScale.x >= 0.98f)
			{
				animTime1 = 0f;
				animTime2 = 0f;
				animTime3 = 0f;
				animState = 0;
				changePlanet = true;
				state = State.playerInput;
			}
			break;
		case State.playerInput:
			playerInput();
			if (stageID > Global.advPlanet)
			{
				if (animState == 0)
				{
					if (PlayAnimPlanet(2))
					{
						animState = 1;
					}
				}
				else
				{
					PlayAnimMoveLeft();
				}
			}
			else if (stageID < Global.advPlanet)
			{
				if (animState == 0)
				{
					if (PlayAnimPlanet(2))
					{
						animState = 1;
					}
				}
				else
				{
					PlayAnimMoveRight();
				}
			}
			else if (changePlanet)
			{
				animState = 0;
				PlayAnimPlanet(1);
			}
			pointerControl();
			break;
		case State.nextScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				MonoBehaviour.print(stageName);
				if (stageName.Contains("1-0"))
				{
					Global.advStage = 0;
					Global.Mode = GameMode.Adventure;
					Loading.LoadScene("Adventure.Tutorial");
				}
				else if (stageName.Contains("-"))
				{
					int num2 = int.Parse(stageName.Substring(stageName.Length - 3, 1));
					int num3 = int.Parse(stageName.Substring(stageName.Length - 1));
					GameBoss.HardMode = true;
					Global.advStage = (num2 - 1) * 6 + num3;
					Global.Mode = GameMode.Adventure;
					Debug.Log("play map : " + Global.advStage);
					Loading.LoadScene("Adventure.GameMode");
				}
				else
				{
					base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					state = State.playerInput;
				}
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				Loading.LoadScene("MainMenu");
			}
			break;
		}
		PlayAnimProp();
	}

	public void SetUp(int mode)
	{
		if (mode <= 1)
		{
			TextureUtility.SetSpriteIndex(planetName, 1, 5, Global.advPlanet);
			int num = ((Global.advUnlock != 0) ? ((Global.advUnlock % 6 != 0) ? (Global.advUnlock % 6) : 6) : 0);
			int num2 = Global.advPlanet * 6;
			TextureUtility.SetSpriteIndex(planetProp[0].transform.GetChild(1).GetChild(0).gameObject, 5, 1, Global.advPlanet);
			TextureUtility.SetSpriteIndex(planetProp[1].transform.GetChild(1).GetChild(0).gameObject, 5, 1, Global.advPlanet);
			TextureUtility.SetSpriteIndex(planetProp[2].transform.GetChild(1).GetChild(0).gameObject, 5, 1, Global.advPlanet);
			TextureUtility.SetSpriteIndex(planetProp[3].transform.GetChild(1).GetChild(0).gameObject, 5, 1, Global.advPlanet);
			planetProp[0].transform.localPosition = pPropPos[Global.advPlanet, 0];
			planetProp[1].transform.localPosition = pPropPos[Global.advPlanet, 1];
			planetProp[2].transform.localPosition = pPropPos[Global.advPlanet, 2];
			planetProp[3].transform.localPosition = pPropPos[Global.advPlanet, 3];
			for (int i = 0; i < stageBtn.Length; i++)
			{
				if (Global.advPlanet == 0 && i == 0)
				{
					stageBtn[i].SetActive(true);
					TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 0);
					stageBtn[i].transform.Find("tutorial").gameObject.SetActive(true);
					stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().text = Global.advPlanet + 1 + "-" + i;
					stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
					stageBtn[i].transform.Find("stage_star").gameObject.SetActive(false);
					stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 1f);
				}
				else if (Global.advPlanet == 0)
				{
					if (i == stageBtn.Length - 2)
					{
						stageBtn[i].transform.Find("stage_name").gameObject.SetActive(true);
					}
					if (Global.advStar[num2 + i] != 4)
					{
						TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 0);
						TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
						stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().text = Global.advPlanet + 1 + "-" + i;
						if (i == stageBtn.Length - 1)
						{
							stageBtn[i].SetActive(true);
							TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 0);
							TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
							stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
						}
						stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 1f);
					}
					else
					{
						TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 3);
						TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
						stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().text = Global.advPlanet + 1 + "-" + i;
						if (i == stageBtn.Length - 1)
						{
							TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 3);
							TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
							stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
						}
						stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 0f);
					}
				}
				else if (Global.advPlanet > 0)
				{
					if (i == 0)
					{
						stageBtn[i].transform.Find("tutorial").gameObject.SetActive(false);
						stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
						stageBtn[i].transform.Find("stage_star").gameObject.SetActive(false);
						stageBtn[i].SetActive(false);
						continue;
					}
					if (Global.advStar[num2 + i] != 4)
					{
						MonoBehaviour.print("stage : " + (num2 + i) + " star : " + Global.advStar[num2 + i]);
						TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 0);
						TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
						stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().text = Global.advPlanet + 1 + "-" + i;
						if (i == stageBtn.Length - 1)
						{
							TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
							stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
						}
						stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 1f);
					}
					else
					{
						TextureUtility.SetSpriteIndex(stageBtn[i], 4, 1, 3);
						TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
						stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().text = Global.advPlanet + 1 + "-" + i;
						if (i == stageBtn.Length - 1)
						{
							TextureUtility.SetSpriteIndex(stageBtn[i].transform.Find("stage_star").gameObject, 1, 5, Mathf.Max(0, Global.advStar[num2 + i]));
							stageBtn[i].transform.Find("stage_name").gameObject.SetActive(false);
						}
						stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 0f);
					}
				}
				stageBtn[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().color = new Color(0.5f, 0.5f, 0.5f, 0f);
				stageBtn[i].transform.Find("stage_star").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				stageBtn[0].transform.Find("tutorial").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				stageBoss.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			}
		}
		if (mode < 1)
		{
			return;
		}
		for (int j = 0; j < planet.Length; j++)
		{
			if (j == Global.advPlanet)
			{
				planet[j].SetActive(true);
				planet[j].transform.localPosition = new Vector3(0f, -70f, -4f);
				planet[j].transform.localScale = new Vector3(44.6f, 44.6f, 1f);
			}
			else if (j == Global.advPlanet - 1)
			{
				planet[j].SetActive(true);
				planet[j].transform.localPosition = new Vector3(-75f, -45f, -4f);
				planet[j].transform.localScale = new Vector3(24f, 24f, 1f);
			}
			else if (j == Global.advPlanet - 2)
			{
				planet[j].SetActive(true);
				planet[j].transform.localPosition = new Vector3(-94f, -8f, -4f);
				planet[j].transform.localScale = new Vector3(10f, 10f, 1f);
			}
			else if (j == Global.advPlanet + 1)
			{
				planet[j].SetActive(true);
				planet[j].transform.localPosition = new Vector3(75f, -45f, -4f);
				planet[j].transform.localScale = new Vector3(24f, 24f, 1f);
			}
			else if (j == Global.advPlanet + 2)
			{
				planet[j].SetActive(true);
				planet[j].transform.localPosition = new Vector3(94f, -8f, -4f);
				planet[j].transform.localScale = new Vector3(10f, 10f, 1f);
			}
			else
			{
				planet[j].SetActive(false);
			}
			if (j > (Global.advUnlock - 1) / 6)
			{
				TextureUtility.SetSpriteIndex(planet[j], 3, 2, 5);
			}
		}
	}

	public bool PlayAnimIn(int mode)
	{
		inTime1 += Time.deltaTime * 4.4f;
		planetName.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, inTime1)));
		for (int i = 0; i < stageBtn.Length; i++)
		{
			Vector3 localPosition = stageBtn[i].transform.localPosition;
			float a = Mathf.Lerp(0f, 0.5f, inTime1 - 0.5f * (float)i);
			float a2 = Mathf.Lerp(0f, 1f, inTime1 - 0.5f * (float)i);
			float y = Mathf.Lerp(-66f, -50f, inTime1 - 0.5f * (float)i);
			if (stageBtn[i].transform.localPosition.y >= -50.2f && inTime2 < (float)(i + 1))
			{
				inTime2 += 1f;
			}
			else if (inTime2 < (float)(i + 1))
			{
				stageBtn[i].transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
				stageBtn[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
				stageBtn[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
				stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().color = new Color(0.87f, 1f, 1f, a2);
				stageBtn[i].transform.Find("stage_star").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
				if (i == 0)
				{
					stageBtn[0].transform.Find("tutorial").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
				}
				stageBoss.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
			}
			else
			{
				stageBtn[i].transform.localPosition = Vector3.Lerp(localPosition, new Vector3(localPosition.x, -55f, localPosition.z), 0.25f);
				if (stageBtn[stageBtn.Length - 1].transform.localPosition.y <= -54.9f && inTime2 == (float)stageBtn.Length && mode == 0)
				{
					inTime1 = 0f;
					inTime2 = 0f;
					return true;
				}
			}
		}
		return false;
	}

	public bool PlayAnimOut(int mode)
	{
		if (mode >= 0)
		{
			outTime2 += Time.deltaTime * 3.4f;
			if (stageBtn[1].GetComponent<Renderer>().material.GetColor("_TintColor").a <= 0f && stageBtn[stageBtn.Length - 1].GetComponent<Renderer>().material.GetColor("_TintColor").a <= 0f && mode == 0)
			{
				outTime1 = 0f;
				outTime2 = 0f;
				return true;
			}
			for (int i = 0; i < stageBtn.Length; i++)
			{
				Vector3 localPosition = stageBtn[i].transform.localPosition;
				float a = stageBtn[i].GetComponent<Renderer>().material.GetColor("_TintColor").a;
				float a2 = stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().color.a;
				float a3 = Mathf.Lerp(a, 0f, outTime2);
				float a4 = Mathf.Lerp(a2, 0f, outTime2);
				float y = Mathf.Lerp(localPosition.y, -66f, outTime2);
				stageBtn[i].transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
				stageBtn[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a3));
				stageBtn[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a3));
				stageBtn[i].transform.Find("stage_name").GetComponent<TextMesh>().color = new Color(0.87f, 1f, 1f, a4);
				stageBtn[i].transform.Find("stage_star").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a3));
				stageBtn[i].transform.localScale = new Vector3(16.4f, 16.4f, 0f);
				stageBoss.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a3));
				if (i == 0)
				{
					stageBtn[0].transform.Find("tutorial").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a3));
				}
			}
		}
		if (mode >= 1)
		{
			outTime1 += Time.deltaTime * 1.8f;
			float num = Mathf.Lerp(1f, 0f, outTime1 - 1.2f);
			float num2 = Mathf.Lerp(1f, 0f, outTime1 - 0.1f);
			float num3 = Mathf.Lerp(1f, 0f, outTime1 - 0.6f);
			for (int j = 0; j < planet.Length; j++)
			{
				planet[j].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			}
			for (int k = 0; k < propLine3.Length; k++)
			{
				propLine3[k].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			}
			plBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.25f)));
			prBtn.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.25f)));
			if (outTime1 >= 0.25f)
			{
				Vector3 localPosition2 = plBtn.transform.GetChild(0).transform.localPosition;
				Vector3 localPosition3 = Vector3.Lerp(localPosition2, new Vector3(2.5f, localPosition2.y, localPosition2.z), 0.1f);
				Vector3 localPosition4 = txtArea.transform.transform.localPosition;
				Vector3 localPosition5 = Vector3.Lerp(localPosition4, new Vector3(-15f, localPosition4.y, localPosition4.z), 0.1f);
				plBtn.transform.GetChild(0).transform.localPosition = localPosition3;
				prBtn.transform.GetChild(0).transform.localPosition = localPosition3;
				txtArea.transform.localPosition = localPosition5;
			}
			propPLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			propPLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			planetName.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 1.2f)));
			propGrid.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 1.2f)));
			propLine1.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			propLine2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, outTime1 - 0.8f)));
			txtArea.GetComponent<TextMesh>().color = new Color(0.87f, 1f, 1f, Mathf.Lerp(1f, 0f, outTime1 - 1f));
			propPLine1.transform.parent.localScale = new Vector3(num2, num2, 1f);
			propPLine2.transform.parent.localScale = new Vector3(num2, num2, 1f);
			planet[0].transform.parent.localScale = new Vector3(num2, num2, 1f);
			propLine1.transform.parent.localScale = new Vector3(num3, num3, 1f);
			propLine2.transform.parent.localScale = new Vector3(num3, num3, 1f);
			if (propGrid.transform.parent.localScale.y <= 0.02f)
			{
				outTime1 = 0f;
				outTime2 = 0f;
				return true;
			}
		}
		return false;
	}

	public bool PlayAnimMoveLeft()
	{
		animTime1 += Time.deltaTime * 1.8f;
		animTime3 += Time.deltaTime * 1.85f;
		PlayAnimOut(0);
		if (Global.advPlanet <= (Global.advUnlock - 1) / 6)
		{
			planetName.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, animTime1 * 2.2f)));
		}
		for (int i = 0; i < planet.Length; i++)
		{
			if (i == Global.advPlanet)
			{
				planet[i].SetActive(true);
				float num = Mathf.Lerp(44.6f, 24f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(0f, -75f, animTime1), Mathf.Lerp(-70f, -45f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num, num, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0.33f, animTime3)));
				if (planet[i].transform.localPosition.x <= -74.98f)
				{
					animTime1 = 0f;
					animTime2 = 0f;
					animTime3 = 0f;
					inTime1 = 0f;
					inTime2 = 0f;
					Global.advPlanet++;
					SetUp(1);
				}
			}
			else if (i == Global.advPlanet - 1)
			{
				planet[i].SetActive(true);
				float num2 = Mathf.Lerp(24f, 10f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(-75f, -94f, animTime1), Mathf.Lerp(-45f, -8f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num2, num2, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.33f, 0.16f, animTime3)));
			}
			else if (i == Global.advPlanet - 2)
			{
				planet[i].SetActive(true);
				float num3 = Mathf.Lerp(10f, 5f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(-94f, -89f, animTime1), Mathf.Lerp(-8f, 9f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num3, num3, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.16f, 0f, animTime3)));
			}
			else if (i == Global.advPlanet + 1)
			{
				planet[i].SetActive(true);
				float num4 = Mathf.Lerp(24f, 44.6f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(75f, 0f, animTime1), Mathf.Lerp(-45f, -70f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num4, num4, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.33f, 0.5f, animTime3)));
			}
			else if (i == Global.advPlanet + 2)
			{
				planet[i].SetActive(true);
				float num5 = Mathf.Lerp(10f, 24f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(94f, 75f, animTime1), Mathf.Lerp(-8f, -45f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num5, num5, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.16f, 0.33f, animTime3)));
			}
			else if (i == Global.advPlanet + 3)
			{
				planet[i].SetActive(true);
				float num6 = Mathf.Lerp(5f, 10f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(89f, 94f, animTime1), Mathf.Lerp(9f, -8f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num6, num6, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.16f, animTime3)));
			}
			else
			{
				planet[i].SetActive(false);
			}
		}
		return false;
	}

	public bool PlayAnimMoveRight()
	{
		animTime1 += Time.deltaTime * 1.8f;
		animTime3 += Time.deltaTime * 1.85f;
		PlayAnimOut(0);
		if (Global.advPlanet <= (Global.advUnlock - 1) / 6)
		{
			planetName.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, animTime1 * 2.2f)));
		}
		for (int i = 0; i < planet.Length; i++)
		{
			if (i == Global.advPlanet)
			{
				planet[i].SetActive(true);
				float num = Mathf.Lerp(44.6f, 24f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(0f, 75f, animTime1), Mathf.Lerp(-70f, -45f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num, num, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0.33f, animTime3)));
				if (planet[i].transform.localPosition.x >= 74.98f)
				{
					animTime1 = 0f;
					animTime2 = 0f;
					animTime3 = 0f;
					inTime1 = 0f;
					inTime2 = 0f;
					Global.advPlanet--;
					SetUp(1);
				}
			}
			else if (i == Global.advPlanet - 1)
			{
				planet[i].SetActive(true);
				float num2 = Mathf.Lerp(24f, 44.6f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(-75f, 0f, animTime1), Mathf.Lerp(-45f, -70f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num2, num2, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.33f, 0.5f, animTime3)));
			}
			else if (i == Global.advPlanet - 2)
			{
				planet[i].SetActive(true);
				float num3 = Mathf.Lerp(10f, 24f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(-94f, -75f, animTime1), Mathf.Lerp(-8f, -45f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num3, num3, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.16f, 0.33f, animTime3)));
			}
			else if (i == Global.advPlanet + 1)
			{
				planet[i].SetActive(true);
				float num4 = Mathf.Lerp(24f, 10f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(75f, 94f, animTime1), Mathf.Lerp(-45f, -8f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num4, num4, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.33f, 0.16f, animTime3)));
			}
			else if (i == Global.advPlanet + 2)
			{
				planet[i].SetActive(true);
				float num5 = Mathf.Lerp(10f, 5f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(94f, 89f, animTime1), Mathf.Lerp(-8f, 9f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num5, num5, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.16f, 0f, animTime3)));
			}
			else if (i == Global.advPlanet - 3)
			{
				planet[i].SetActive(true);
				float num6 = Mathf.Lerp(5f, 10f, animTime1);
				planet[i].transform.localPosition = new Vector3(Mathf.Lerp(-89f, -94f, animTime1), Mathf.Lerp(9f, -8f, animTime3), -4f);
				planet[i].transform.localScale = new Vector3(num6, num6, 1f);
				planet[i].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.16f, animTime3)));
			}
			else
			{
				planet[i].SetActive(false);
			}
		}
		return false;
	}

	public bool PlayAnimPlanet(int mode)
	{
		if (Global.advPlanet > (Global.advUnlock - 1) / 6)
		{
			return true;
		}
		if (mode < 2)
		{
			if (planetProp[planetProp.Length - 1].transform.GetChild(1).localScale.x <= 0.99f)
			{
				decoTime1 += Time.deltaTime * 3.8f;
			}
			if (PlayAnimIn(0))
			{
				changePlanet = false;
				return true;
			}
		}
		else
		{
			if (decoTime1 < -0.2f)
			{
				decoTime1 = 0f;
				return true;
			}
			decoTime1 -= Time.deltaTime * 6.8f;
		}
		for (int i = 0; i < planetProp.Length; i++)
		{
			planetProp[i].transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, decoTime1), 1f, 1f);
			planetProp[i].transform.GetChild(0).GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(Mathf.Lerp(0f, 0.5f, decoTime1), 1f));
			planetProp[i].transform.GetChild(1).localScale = new Vector3(Mathf.Lerp(0f, 1f, decoTime1 - 1f), Mathf.Lerp(0f, 1f, decoTime1 - 1f), 1f);
		}
		return false;
	}

	public void PlayAnimProp()
	{
		propTime1 += Time.deltaTime * 1.8f;
		propTime2 += Time.deltaTime * 1.8f;
		propTime3 += Time.deltaTime * 1.8f;
		Vector3 localPosition = propLine3[0].transform.localPosition;
		planetProp[0].transform.GetChild(1).GetChild(0).Rotate(0f, 0f, 10f);
		compPoint.transform.localRotation = Quaternion.Euler(75f, 0f, 135f + Mathf.PingPong(propTime2 * 38.5f, 90f));
		if (Time.time - timeStamp < 3f)
		{
			for (int i = 0; i < 3; i++)
			{
				numCircle.transform.GetChild(i).Rotate(0f, 0f, ((float)(-i) + 0.5f) * 10f);
			}
		}
		else if (propState1 == 0)
		{
			propState1 = 1;
			propTime1 = 0f;
		}
		else if (propState1 == 1)
		{
			numFrame.transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, propTime1), 1f, 1f);
			numFrame.transform.GetChild(1).localScale = new Vector3(Mathf.Lerp(0f, 1f, propTime1 * 0.8f), Mathf.Lerp(0f, 1f, propTime1 * 0.8f), 1f);
			numFrame.transform.GetChild(0).GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(Mathf.Lerp(0f, 0.5f, propTime1), 1f));
			if (numFrame.transform.GetChild(1).localScale.x >= 0.99f)
			{
				propTime1 = 0f;
				propState1 = 2;
			}
		}
		else if (propState1 == 2)
		{
			int num = (int)Mathf.Lerp(10f, 99f, propTime1 * 0.4f);
			numFrame.transform.GetChild(1).GetComponent<TextMesh>().text = string.Empty + num;
			if (num == 99)
			{
				numFrame.transform.GetChild(1).GetComponent<TextMesh>().text = "10";
				propTime1 = 0f;
				propState1 = 3;
			}
		}
		else if (propState1 == 3)
		{
			numFrame.transform.localScale = new Vector3(Mathf.Lerp(1f, 0f, propTime1), 1f, 1f);
			numFrame.transform.GetChild(1).localScale = new Vector3(Mathf.Lerp(1f, 0f, propTime1 * 2f), Mathf.Lerp(1f, 0f, propTime1 * 2f), 1f);
			numFrame.transform.GetChild(0).GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(Mathf.Lerp(0.5f, 0f, propTime1), 1f));
			if (numFrame.transform.localScale.x <= 0.01f)
			{
				propTime1 = 0f;
				propState1 = 0;
				timeStamp = Time.time;
			}
		}
		if (propState2 == 0)
		{
			propMove = Random.Range(0f, (localPosition.x - 10f) / 10f);
			propMove = Mathf.FloorToInt(propMove);
			propstart = localPosition.x;
			propTime3 = 0f;
			propState2 = 1;
		}
		else if (propState2 == 1)
		{
			propLine3[0].transform.localPosition = new Vector3(Mathf.Lerp(propstart, propMove * 10f, propTime3 * 0.8f), 0f, -1f);
			for (int j = 1; j < propLine3.Length; j++)
			{
				if (localPosition.x < propLine3[j].transform.localPosition.x)
				{
					propLine3[j].GetComponent<MeshRenderer>().enabled = false;
				}
			}
			if (propLine3[0].transform.localPosition.x <= propMove * 10f + 0.1f)
			{
				propState2 = 2;
			}
		}
		else if (propState2 == 2)
		{
			propMove = Random.Range((localPosition.x + 10f) / 10f, 8f);
			propMove = Mathf.FloorToInt(propMove);
			propstart = localPosition.x;
			propTime3 = 0f;
			propState2 = 3;
		}
		else
		{
			if (propState2 != 3)
			{
				return;
			}
			propLine3[0].transform.localPosition = new Vector3(Mathf.Lerp(propstart, propMove * 10f, propTime3 * 0.8f), 0f, -1f);
			for (int k = 1; k < propLine3.Length; k++)
			{
				if (localPosition.x > propLine3[k].transform.localPosition.x)
				{
					propLine3[k].GetComponent<MeshRenderer>().enabled = true;
				}
			}
			if (propLine3[0].transform.localPosition.x >= propMove * 10f - 0.1f)
			{
				propState2 = 0;
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
			zero = player.transform.GetChild(0).position;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out array[i]) && i == num && !mouseFocus)
			{
				mouseFocus = true;
				pTime = 10f;
			}
			if (Physics.Raycast(zero, Vector3.forward, out array[i]) || (Physics.Raycast(ray, out array[i]) && i == num && pointerInit == 0))
			{
				if (togButton[i] != null && !array[i].transform.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					mouseOver[i] = false;
					idleButton(togButton[i]);
					mouseFocus = false;
				}
				if (!mouseOver[i])
				{
					if (array[i].transform.name.Contains("btn_stage") && array[i].transform.localScale.z == 0f)
					{
						continue;
					}
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (array[i].transform.name.Contains("btn_stage") && array[i].transform.localScale.z == 1f)
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
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						MonoBehaviour.print("stage name = " + togButton[i].name);
						stageName = togButton[i].transform.Find("stage_name").GetComponent<TextMesh>().text;
						MonoBehaviour.print(stageName);
						togButton[i] = null;
						fadeTime = 0f;
						Global.ctrlID = i;
						MonoBehaviour.print("use controller : " + i);
						state = State.nextScene;
					}
				}
				else if (array[i].transform.name == "btn_back")
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
				else if (array[i].transform.name == "btn_planet_l")
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(plBtn.transform.GetChild(0).gameObject, 2, 1, 1);
					}
					togButton[i] = plBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						plBtn.transform.GetChild(0).localPosition = new Vector3(-0.2f, 0f, 0f);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						plBtn.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
						idleButton(togButton[i]);
						togButton[i] = null;
						if (stageID > 0)
						{
							changePlanet = true;
							stageID--;
						}
					}
				}
				else
				{
					if (!(array[i].transform.name == "btn_planet_r"))
					{
						continue;
					}
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(prBtn.transform.GetChild(0).gameObject, 2, 1, 1);
					}
					togButton[i] = prBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						prBtn.transform.GetChild(0).localPosition = new Vector3(-0.2f, 0f, 0f);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						prBtn.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
						idleButton(togButton[i]);
						togButton[i] = null;
						if (stageID < planet.Length - 1)
						{
							changePlanet = true;
							stageID++;
						}
					}
				}
			}
			else if (togButton[i] != null)
			{
				controller.DoPadRelease(i);
				mouseOver[i] = false;
				isPress = false;
				idleButton(togButton[i]);
				togButton[i] = null;
			}
		}
	}

	private void idleButton(GameObject togButton)
	{
		if (togButton.name.Contains("btn_stage"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		else if (togButton.name.Contains("btn_planet"))
		{
			TextureUtility.SetSpriteIndex(togButton.transform.GetChild(0).gameObject, 2, 1, 0);
			togButton.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
		}
		else if (togButton.name.Contains("btn"))
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
}
