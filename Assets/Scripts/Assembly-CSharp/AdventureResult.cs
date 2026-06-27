using UnityEngine;

public class AdventureResult : MonoBehaviour
{
	private enum State
	{
		initial = 0,
		fadeIn = 1,
		clearAnim = 2,
		failAnim = 3,
		playerInput = 4,
		nextScene = 5,
		backScene = 6,
		replay = 7
	}

	private GameObject player;

	private GameObject backBtn;

	private GameObject nextBtn;

	private GameObject reBtn;

	private GameObject panelClear;

	private GameObject[] star;

	private GameObject[] time;

	private GameObject panelFail;

	private GameObject txtFail;

	private TextMesh txtStage;

	private GameObject mCam;

	private GameObject tvCam;

	private Transform unlockPanel;

	private GameObject unlockButtonEff;

	private GameObject unlockUILock;

	private GameObject unlockUIBoss;

	private GameObject unlockProp;

	private GameObject txtNextStage;

	private GameObject txtUnlockStage;

	private GameObject skipPanel;

	private GameObject condBG;

	private TextMesh txtCond;

	private State state;

	private GameObject[] togButton;

	private float fadeTime;

	private float animTime;

	private float fxTime;

	private float remainTime;

	private int animStat;

	private int starCount;

	private bool[] mouseOver = new bool[5];

	private int pointerInit;

	public float speed = 45.5f;

	private float pTime;

	private float pointerSpeed = Global.pointerSpd;

	private string vovName;

	private bool willUnlock;

	private Vector3 touchPos;

	private bool isPress;

	private bool mouseFocus;

	private void Awake()
	{
		SoundMusicControl.ctrl.enabled = false;
		if (Global.advClear)
		{
			if (Global.Stage == 6 && Global.advPlanet == Global.advMaxP - 1)
			{
				vovName = null;
			}
			else
			{
				vovName = "cow_" + Random.Range(1, 21).ToString("D2");
			}
		}
		else if (Global.IsBossStage && Global.Level < 5 && Global.advStage == Global.advUnlock && Global.advStar[Global.advStage] == -1)
		{
			vovName = "levelIntro_skip";
		}
		else
		{
			vovName = "col_" + Random.Range(1, 21).ToString("D2");
		}
		if (vovName != null)
		{
			GameSound.PreloadVOV(vovName);
		}
	}

	private void Start()
	{
		for (int i = 1; i <= 5; i++)
		{
			GameSound.StopBGM("Map" + i.ToString("D2"));
		}
		GameSound.StopBGM("BossFight");
		star = new GameObject[3];
		time = new GameObject[4];
		togButton = new GameObject[5];
		animTime = 0f;
		fadeTime = 0f;
		fxTime = 0f;
		animStat = 0;
		starCount = 0;
		remainTime = Global.advRemainTime;
		state = State.initial;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "btn_back")
			{
				backBtn = transform.gameObject;
			}
			else if (transform.name == "btn_next")
			{
				nextBtn = transform.gameObject;
			}
			else if (transform.name == "btn_replay")
			{
				reBtn = transform.gameObject;
			}
			else if (transform.name.Contains("p_pointer"))
			{
				player = transform.gameObject;
			}
			else if (transform.name.Contains("panelClear"))
			{
				panelClear = transform.gameObject;
				Transform[] componentsInChildren2 = panelClear.GetComponentsInChildren<Transform>();
				foreach (Transform transform2 in componentsInChildren2)
				{
					if (transform2.name.Contains("star"))
					{
						int num = int.Parse(transform2.name.Substring(transform2.name.Length - 1));
						star[num - 1] = transform2.gameObject;
					}
					else if (transform2.name.Contains("time"))
					{
						for (int l = 0; l < 4; l++)
						{
							time[l] = transform2.GetChild(l).gameObject;
						}
					}
					else if (transform2.name.Contains("cond_bg"))
					{
						condBG = transform2.gameObject;
						txtCond = condBG.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
					}
				}
			}
			else if (transform.name.Contains("panelFail"))
			{
				panelFail = transform.gameObject;
				txtFail = transform.Find("txt_fail").gameObject;
			}
			else if (transform.name.Contains("txt_stage"))
			{
				txtStage = transform.GetComponent<TextMesh>();
			}
			else if (transform.name.Contains("un_panel"))
			{
				unlockPanel = transform;
			}
			else if (transform.name.Contains("txt_nextstage"))
			{
				txtNextStage = transform.gameObject;
			}
			else if (transform.name.Contains("txt_unlock"))
			{
				txtUnlockStage = transform.gameObject;
			}
			else if (transform.name.Contains("un_buttoneff"))
			{
				unlockButtonEff = transform.gameObject;
			}
			else if (transform.name.Contains("un_uilock"))
			{
				unlockUILock = transform.gameObject;
			}
			else if (transform.name.Contains("un_uiboss"))
			{
				unlockUIBoss = transform.gameObject;
			}
			else if (transform.name.Contains("un_prop"))
			{
				unlockProp = transform.gameObject;
			}
			else if (transform.name.Contains("bg_dr"))
			{
				skipPanel = transform.gameObject;
			}
		}
		mCam = GameObject.Find("Main Camera");
		tvCam = GameObject.Find("TV Camera");
	}

	private void OnDestroy()
	{
		if (Global.IsMusicOn)
		{
			GameSound.SetVolumeBGM(1f);
		}
		if (SoundMusicControl.ctrl != null)
		{
			SoundMusicControl.ctrl.enabled = true;
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
				if (Global.advClear)
				{
					state = State.clearAnim;
				}
				else
				{
					state = State.failAnim;
				}
			}
			break;
		case State.initial:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			if (Global.Stage == 6)
			{
				txtStage.text = "BOSS " + (Global.advPlanet + 1);
			}
			else
			{
				txtStage.text = "AREA " + (Global.advPlanet + 1) + "-" + Global.Stage;
			}
			if (Global.UnlockStage == 6)
			{
				txtUnlockStage.GetComponent<TextMesh>().text = "BOSS " + (Global.advPlanet + 1) + " UNLOCKED";
			}
			else
			{
				int num2 = ((Global.Stage != 6) ? (Global.advPlanet + 1) : (Global.advPlanet + 2));
				txtUnlockStage.GetComponent<TextMesh>().text = "AREA " + num2 + "-" + Global.UnlockStage + " UNLOCKED";
				txtNextStage.GetComponent<TextMesh>().text = num2 + "-" + Global.UnlockStage;
			}
			unlockPanel.localPosition = new Vector3(0f, -20f, -10f);
			unlockProp.transform.localScale = new Vector3(0f, 9.8f, 1f);
			TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 3);
			if (Global.advStar[Global.advStage] > 0 && Global.advStar[Global.advStage] < 4)
			{
				TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 0);
			}
			foreach (Transform item in unlockPanel)
			{
				if (item.name.Contains("txt"))
				{
					item.GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, 0f);
				}
				else
				{
					item.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				}
			}
			if (Global.advClear)
			{
				GameSound.StartSFX("youWin");
				panelFail.SetActive(false);
				GameObject[] array = star;
				foreach (GameObject gameObject in array)
				{
					gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					gameObject.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					gameObject.transform.localScale = new Vector3(65f, 62f, 1f);
				}
				int num3 = (int)(remainTime / 600f);
				int num4 = (int)(remainTime / 60f);
				int num5 = (int)remainTime - (num3 * 600 + num4 * 60);
				int num6 = num5 / 10;
				int num7 = num5 % 10;
				TextureUtility.SetSpriteIndex(time[0], 10, 1, num3);
				TextureUtility.SetSpriteIndex(time[1], 10, 1, num4);
				TextureUtility.SetSpriteIndex(time[2], 10, 1, num6);
				TextureUtility.SetSpriteIndex(time[3], 10, 1, num7);
				if (Global.IsBossStage)
				{
					if (num4 <= 4 || (num4 == 5 && num6 == 0 && num7 == 0))
					{
						starCount = 3;
						condBG.SetActive(false);
					}
					else if (num4 <= 7 || (num4 == 8 && num6 == 0 && num7 == 0))
					{
						starCount = 2;
						txtCond.text = "For 3 stars: clear level within 5 minutes";
					}
					else
					{
						starCount = 1;
						txtCond.text = "For 2 stars: clear level within 8 minutes";
					}
				}
				else if (num4 < 2)
				{
					starCount = 1;
					txtCond.text = "For 2 stars: clear level with 2 minutes left";
				}
				else if (num4 < 3)
				{
					starCount = 2;
					txtCond.text = "For 3 stars: clear level with 3 minutes left";
				}
				else
				{
					starCount = 3;
					condBG.SetActive(false);
				}
			}
			else
			{
				GameSound.StartSFX("youLose");
				txtFail.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				txtFail.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				txtFail.transform.localScale = new Vector3(180f, 19f, 1f);
				skipPanel.transform.localPosition = new Vector3(0f, -38f, 0f);
				skipPanel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				skipPanel.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, 0f);
				condBG.SetActive(false);
			}
			fadeTime = 0f;
			state = State.fadeIn;
			break;
		case State.clearAnim:
			if (animStat == 0 && fadeTime < (float)starCount)
			{
				if (star[(int)fadeTime].transform.localScale.x == 25.6f)
				{
					animTime = 0f;
					if (shakeCam(0.1f))
					{
						GameSound.StartSFX("skillCrash");
						fxTime = 0f;
						fadeTime += 1f;
						if (fadeTime == (float)starCount)
						{
							animStat = 1;
							fadeTime = 0f;
						}
					}
				}
				else
				{
					float num8 = Mathf.Lerp(65f, 25.6f, animTime += Time.deltaTime * 5.5f);
					star[(int)fadeTime].transform.localScale = new Vector3(num8, num8, 1f);
					star[(int)fadeTime].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime * 2f)));
					star[(int)fadeTime].transform.localRotation = Quaternion.Euler(0f, 180f, Mathf.Lerp(-60f, 0f, animTime));
				}
			}
			else if (animStat == 1 && starCount > 0)
			{
				if (star[starCount - 1].transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
				{
					animTime = 0f;
					fadeTime = 2f;
				}
				else if (fadeTime < 2f)
				{
					if (animTime >= 2f)
					{
						fadeTime = 1f;
					}
					animTime += Time.deltaTime * 8.5f;
					if (starCount >= 1)
					{
						star[0].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime)));
					}
					if (starCount >= 2)
					{
						star[1].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime - 1f)));
					}
					if (starCount >= 3)
					{
						star[2].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, animTime - 2f)));
					}
				}
				if (fadeTime == 2f && star[starCount - 1].transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
				{
					fxTime = 0f;
					fadeTime = 0f;
					animTime = 0f;
					animStat = 2;
					if (Global.advUnlock != Global.advStage || Global.advStage >= 30)
					{
						animStat = 4;
						TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 0);
					}
				}
				else if (fadeTime >= 1f)
				{
					fxTime += Time.deltaTime * 8.5f;
					if (starCount >= 1)
					{
						star[0].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fxTime)));
					}
					if (starCount >= 2)
					{
						star[1].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fxTime - 1f)));
					}
					if (starCount >= 3)
					{
						star[2].transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fxTime - 2f)));
					}
				}
			}
			else if (animStat == 2)
			{
				fxTime += Time.deltaTime * 8.5f;
				if (fadeTime == 0f)
				{
					unlockPanel.localPosition = new Vector3(0f, Mathf.Lerp(-20f, 0f, fxTime), -10f);
					foreach (Transform item2 in unlockPanel)
					{
						if (item2.name.Contains("txt_unlock"))
						{
							item2.GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, Mathf.Lerp(0f, 1f, fxTime));
						}
						else if (item2.name.Contains("txt_nextstage"))
						{
							if (Global.UnlockStage < 6)
							{
								item2.GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, Mathf.Lerp(0f, 1f, fxTime));
							}
						}
						else if (item2.name.Contains("boss"))
						{
							if (Global.UnlockStage == 6)
							{
								unlockUIBoss.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fxTime)));
							}
						}
						else
						{
							item2.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fxTime)));
						}
					}
					if (unlockPanel.localPosition.y >= -0.99f)
					{
						fxTime = 0f;
						fadeTime = 1f;
					}
				}
				else if (fadeTime == 1f)
				{
					unlockProp.transform.localScale = new Vector3(Mathf.Lerp(0f, 59.14f, fxTime * 0.5f), 9.8f, 1f);
					if (unlockProp.transform.localScale.x >= 59.139f)
					{
						fxTime = 0f;
						fadeTime = 2f;
					}
				}
				else if (fadeTime == 2f)
				{
					unlockUILock.GetComponent<SpriteTexture>().play = true;
					fadeTime = 3f;
				}
				else if (fadeTime == 3f && unlockUILock.GetComponent<SpriteTexture>().showIndex == 14)
				{
					unlockButtonEff.GetComponent<SpriteTexture>().play = true;
					animStat = 3;
				}
			}
			else if (animStat == 3)
			{
				if (unlockButtonEff.GetComponent<SpriteTexture>().showIndex == 18)
				{
					TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 0);
					animStat = 4;
				}
			}
			else
			{
				if (animStat != 4)
				{
					break;
				}
				if (Global.advStar[Global.advStage] < starCount)
				{
					Global.advStar[Global.advStage] = starCount;
					if (Global.advUnlock == Global.advStage)
					{
						Global.advUnlock++;
						if (Global.advUnlock < Global.advStar.Length)
						{
							Global.advStar[Global.advUnlock] = 0;
						}
					}
				}
				if (Global.Stage == 6 && Global.advPlanet < Global.advMaxP - 1)
				{
					Global.advPlanet++;
				}
				animStat = 0;
				fadeTime = 0f;
				fxTime = 0f;
				checkController();
				GameSave.Save();
				GameAchievement.ProcessAchievement();
				state = State.playerInput;
			}
			break;
		case State.failAnim:
			if (animStat == 0)
			{
				if (txtFail.transform.localScale.x == 92f)
				{
					animTime = 0f;
					if (shakeCam(0.2f))
					{
						fxTime = 0f;
						checkController();
						fadeTime = 0f;
						if (Global.IsBossStage && Global.Level < 5 && Global.advStage == Global.advUnlock && Global.advStar[Global.advStage] > -2)
						{
							Global.advStar[Global.advStage]--;
							if (Global.advStar[Global.advStage] == -2)
							{
								willUnlock = true;
								animStat = 1;
							}
							else
							{
								animStat = 4;
							}
						}
						else if (Global.IsBossStage)
						{
							TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 0);
							animStat = 4;
						}
						else
						{
							animStat = 4;
						}
					}
					txtFail.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
				}
				else
				{
					float x = Mathf.Lerp(250f, 92f, animTime += Time.deltaTime * 5.5f);
					float y = Mathf.Lerp(24f, 10f, animTime += Time.deltaTime * 5.5f);
					txtFail.transform.localScale = new Vector3(x, y, 1f);
					txtFail.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * 20f)));
				}
			}
			else if (animStat == 1)
			{
				fxTime += Time.deltaTime * 2.5f;
				skipPanel.transform.localPosition = new Vector3(0f, Mathf.Lerp(-38f, 7f, fxTime), 0f);
				skipPanel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fxTime)));
				skipPanel.transform.GetChild(0).GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, Mathf.Lerp(0f, 1f, fxTime));
				if (skipPanel.transform.localPosition.y >= 6.99f)
				{
					animStat = 2;
					fxTime = 0f;
				}
			}
			else if (animStat == 2)
			{
				fxTime += Time.deltaTime * 6.5f;
				if (fadeTime == 0f)
				{
					unlockPanel.localPosition = new Vector3(0f, Mathf.Lerp(-20f, 0f, fxTime), -10f);
					foreach (Transform item3 in unlockPanel)
					{
						if (item3.name.Contains("txt_unlock"))
						{
							item3.GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, Mathf.Lerp(0f, 1f, fxTime));
						}
						else if (item3.name.Contains("txt_nextstage"))
						{
							if (Global.UnlockStage < 6)
							{
								item3.GetComponent<TextMesh>().color = new Color(0.8f, 0.94f, 0.94f, Mathf.Lerp(0f, 1f, fxTime));
							}
						}
						else if (item3.name.Contains("boss"))
						{
							if (Global.UnlockStage == 6)
							{
								unlockUIBoss.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fxTime)));
							}
						}
						else
						{
							item3.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fxTime)));
						}
					}
					if (unlockPanel.localPosition.y >= -0.99f)
					{
						fxTime = 0f;
						fadeTime = 1f;
					}
				}
				else if (fadeTime == 1f)
				{
					unlockProp.transform.localScale = new Vector3(Mathf.Lerp(0f, 59.14f, fxTime * 0.5f), 9.8f, 1f);
					if (unlockProp.transform.localScale.x >= 59.139f)
					{
						fxTime = 0f;
						fadeTime = 2f;
					}
				}
				else if (fadeTime == 2f)
				{
					unlockUILock.GetComponent<SpriteTexture>().play = true;
					fadeTime = 3f;
				}
				else if (fadeTime == 3f && unlockUILock.GetComponent<SpriteTexture>().showIndex >= 14)
				{
					unlockButtonEff.GetComponent<SpriteTexture>().play = true;
					animStat = 3;
				}
			}
			else if (animStat == 3)
			{
				if (unlockButtonEff.GetComponent<SpriteTexture>().showIndex >= 18)
				{
					TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 0);
					animStat = 4;
				}
			}
			else
			{
				if (animStat != 4)
				{
					break;
				}
				animStat = 0;
				fadeTime = 0f;
				fxTime = 0f;
				if (Global.advStar[Global.advStage] == -2 && willUnlock)
				{
					Global.advClear = true;
					Global.advUnlock++;
					Global.advPlanet++;
					if (Global.advUnlock < Global.advStar.Length)
					{
						Global.advStar[Global.advUnlock] = 0;
					}
				}
				GameSave.Save();
				GameAchievement.ProcessAchievement();
				state = State.playerInput;
			}
			break;
		case State.playerInput:
			if ((GameSound.GetTimeSFX("youWin") > 3.8f || !GameSound.IsPlayingSFX("youWin")) && (GameSound.GetTimeSFX("youLose") > 3.5f || !GameSound.IsPlayingSFX("youLose")))
			{
				if (Global.IsVoiceOn && vovName != null)
				{
					GameSound.StartVOV(vovName);
					vovName = null;
				}
				if (Global.IsMusicOn && !GameSound.IsPlayingVOV())
				{
					GameSound.SetVolumeBGM(Mathf.Lerp(0f, 1f, fxTime += Time.deltaTime * 0.6f));
					if (!GameSound.IsPlayingBGM("Menu"))
					{
						GameSound.StartBGM("Menu");
					}
				}
			}
			playerInput();
			pointerControl();
			break;
		case State.nextScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				MonoBehaviour.print(Global.advStage);
				MonoBehaviour.print("stage debug : " + Global.IsBossStage + " : " + Global.advStage + " : " + Global.advUnlock);
				if (Global.IsBossStage && Global.Level < 5 && Global.advStage < Global.advUnlock && !Global.advClear)
				{
					MonoBehaviour.print("advance planet : " + (Global.advPlanet + 1));
					Global.advPlanet++;
				}
				Global.advStage++;
				if (Global.Level > 5 && Global.advClear)
				{
					Application.LoadLevel("Adventure.SelectStage");
				}
				else if (Global.Level > 5)
				{
					Global.advPlanet = Global.advMaxP - 1;
					Application.LoadLevel("Adventure.SelectStage");
				}
				else
				{
					Loading.LoadScene("Adventure.GameMode");
				}
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				Application.LoadLevel("Adventure.SelectStage");
			}
			break;
		case State.replay:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				MonoBehaviour.print(Global.advStage);
				if (Global.IsBossStage && Global.advClear && Global.Level < 5)
				{
					MonoBehaviour.print("still at old planet : " + (Global.advPlanet - 1));
					Global.advPlanet--;
				}
				Loading.LoadScene("Adventure.GameMode");
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
					togButton[i] = null;
					mouseFocus = false;
				}
				if (array[i].transform.name == "btn_next" && !Global.advClear && Global.advStage >= Global.advUnlock)
				{
					break;
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
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
				else if (array[i].transform.name == "btn_next" && (Global.advClear || Global.advStage < Global.advUnlock))
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 1);
					}
					togButton[i] = nextBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(nextBtn, 4, 1, 2);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						Global.ctrlID = i;
						state = State.nextScene;
					}
				}
				else if (array[i].transform.name == "btn_replay")
				{
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(reBtn, 4, 1, 1);
					}
					togButton[i] = reBtn;
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(reBtn, 4, 1, 2);
						isPress = true;
					}
					else if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						togButton[i] = null;
						fadeTime = 0f;
						Global.ctrlID = i;
						state = State.replay;
					}
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
		if (togButton.name.Contains("next") && !Global.advClear && Global.advStage >= Global.advUnlock)
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 3);
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

	public bool shakeCam(float round)
	{
		if (fxTime < round)
		{
			fxTime += Time.deltaTime;
			if (fxTime > 0f)
			{
				float num = 0.12f - Mathf.PingPong(Time.time * 1.25f, 0.24f);
				Camera[] allCameras = Camera.allCameras;
				foreach (Camera camera in allCameras)
				{
					camera.fieldOfView = 10f - Mathf.Abs(num);
					camera.rect = new Rect(0f, num / 10f, 1f, 1f);
				}
			}
			return false;
		}
		Camera[] allCameras2 = Camera.allCameras;
		foreach (Camera camera2 in allCameras2)
		{
			camera2.fieldOfView = 10f;
			camera2.rect = new Rect(0f, 0f, 1f, 1f);
		}
		return true;
	}
}
