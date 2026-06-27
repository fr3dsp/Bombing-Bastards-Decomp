using System;
using UnityEngine;

public class LocalBattleSelectStage : MonoBehaviour
{
	private enum State
	{
		fadeIn = 0,
		initial = 1,
		playerInput = 2,
		nextScene = 3,
		backScene = 4
	}

	private Transform[] map;

	private GameObject[] player;

	private GameObject backBtn;

	private GameObject nextBtn;

	private GameObject mapName;

	private GameObject mapCount;

	private GameObject mapFrame;

	private State state;

	private GameObject[] togButton;

	private float fadeTime;

	private int playAnim;

	private bool[] mouseOver = new bool[5];

	private float[] pTime = new float[5];

	public int mapIndex;

	public float speed = 45.5f;

	public float scaleSpeedUp = 0.6f;

	public float scaleSpeedDown = 0.5f;

	private float pointerSpeed = Global.pointerSpd;

	private int[] pointerInit = new int[5];

	private Vector3 touchPos;

	private int[] ctrlStat = new int[5];

	private void Awake()
	{
	}

	private void Start()
	{
		map = new Transform[4];
		player = new GameObject[5];
		togButton = new GameObject[5];
		state = State.initial;
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("map"))
			{
				int num = int.Parse(transform.transform.name.Substring(transform.transform.name.Length - 1));
				map[num - 1] = transform;
			}
			else if (transform.name == "btn_back")
			{
				backBtn = transform.gameObject;
			}
			else if (transform.name == "btn_next")
			{
				nextBtn = transform.gameObject;
			}
			else if (transform.name == "name")
			{
				mapName = transform.gameObject;
			}
			else if (transform.name == "count")
			{
				mapCount = transform.gameObject;
			}
			else if (transform.name == "frame")
			{
				mapFrame = transform.gameObject;
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
		{
			mapIndex = Global.mapID - 1;
			for (int i = 0; i < mapIndex; i++)
			{
				map[0].localPosition = new Vector3(-69f, -6.29f, -2f);
				map[0].localScale = new Vector3(40.4f, 28.4f, 1f);
				map[1].localPosition = new Vector3(0f, -6.29f, -4f);
				map[1].localScale = new Vector3(57.2f, 40.2f, 1f);
				map[2].localPosition = new Vector3(69f, -6.29f, -2f);
				map[2].localScale = new Vector3(40.4f, 28.4f, 1f);
				map[3].localPosition = new Vector3(0f, -6.29f, -1f);
				map[3].localScale = new Vector3(28f, 19f, 1f);
				int num2 = i + 1;
				if (num2 < 0)
				{
					num2 -= 5;
				}
				TextureUtility.SetSpriteIndex(map[2].gameObject, 5, 1, num2 + 1);
				Transform transform = map[0];
				map[0] = map[1];
				map[1] = map[2];
				map[2] = map[3];
				map[3] = transform;
			}
			mapName.GetComponent<NoiseAnim>().ignoreY = true;
			TextureUtility.SetSpriteIndex(mapName, 1, 5, mapIndex);
			mapCount.GetComponent<TextMesh>().text = mapIndex + 1 + "/5";
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			mapFrame.transform.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
			map[1].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.15f, 0.15f, 0.15f, 0.5f));
			map[2].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.15f, 0.15f, 0.15f, 0.5f));
			map[3].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.15f, 0.15f, 0.15f, 0.5f));
			checkController();
			state = State.fadeIn;
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
				mapName.GetComponent<NoiseAnim>().type = -1;
				mapName.GetComponent<NoiseAnim>().ignoreY = false;
				Global.mapID = mapIndex + 1;
				Global.Mode = GameMode.LocalBattle;
				Loading.LoadScene("Adventure.GameMode");
			}
			break;
		case State.backScene:
			base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * num)));
			if (base.gameObject.transform.Find("fade_bg").GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				mapName.GetComponent<NoiseAnim>().type = -1;
				mapName.GetComponent<NoiseAnim>().ignoreY = false;
				Global.mapID = mapIndex + 1;
				Application.LoadLevel("LocalBattle.Settings");
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
				if (togButton[i] != null && !array[i].transform.parent.name.Contains(togButton[i].name))
				{
					controller.DoPadRelease(i);
					idleButton(togButton[i]);
				}
				if (!mouseOver[i])
				{
					mouseOver[i] = true;
					GameSound.StartSFX("mouseOver");
				}
				if (array[i].transform.name == "arrow_l" && playAnim == 0)
				{
					togButton[i] = array[i].transform.gameObject;
					TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
					}
					if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						if (mapIndex > 0)
						{
							mapIndex--;
						}
						else
						{
							mapIndex = 4;
						}
						mapCount.GetComponent<TextMesh>().text = mapIndex + 1 + "/5";
						playAnim = 1;
					}
				}
				else if (array[i].transform.name == "arrow_r" && playAnim == 0)
				{
					togButton[i] = array[i].transform.gameObject;
					TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 1);
					if (controller.DoEnterHold("Fire1"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton[i], 3, 1, 2);
					}
					if (controller.DoEnterRelease("Fire1"))
					{
						controller.DoPadRelease(i);
						touchPos = Vector3.zero;
						GameSound.StartSFX("menuSelect");
						if (mapIndex < 4)
						{
							mapIndex++;
						}
						else
						{
							mapIndex = 0;
						}
						mapCount.GetComponent<TextMesh>().text = mapIndex + 1 + "/5";
						playAnim = -1;
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

	private void animateButton()
	{
		if (playAnim > 0)
		{
			MonoBehaviour.print("move to right");
			if (playAnim == 1)
			{
				int num = mapIndex - 1;
				if (num < 0)
				{
					num += 5;
				}
				TextureUtility.SetSpriteIndex(map[2].gameObject, 5, 1, num);
				mapName.GetComponent<NoiseAnim>().type = 0;
				mapName.GetComponent<NoiseAnim>().ignoreY = false;
				mapName.GetComponent<AnimateTexture>().alphaStart = 0f;
				playAnim = 2;
			}
			if (playAnim == 2)
			{
				float a = Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * 9.5f);
				float num2 = Mathf.Lerp(0.5f, 0.15f, fadeTime);
				mapFrame.transform.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a));
				map[0].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(num2, num2, num2, 0.5f));
				map[3].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f - num2 + 0.2f, 0.5f - num2 + 0.2f, 0.5f - num2 + 0.2f, 0.5f));
				map[2].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(num2, num2, num2, 0.5f));
				if (mapFrame.transform.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
				{
					fadeTime = 0f;
					playAnim = 3;
				}
			}
			if (playAnim != 4)
			{
				Vector3 localPosition = map[0].localPosition;
				Vector3 localScale = map[0].localScale;
				float x = Mathf.Lerp(localPosition.x, 69f, speed * Time.deltaTime);
				float num3 = Mathf.Lerp(localScale.x, 40.4f, scaleSpeedDown * Time.deltaTime);
				float num4 = Mathf.Lerp(localScale.y, 28.4f, scaleSpeedDown * Time.deltaTime);
				if (localPosition.x > 50f)
				{
					num3 = Mathf.Lerp(localScale.x, 40.4f, scaleSpeedDown * Time.deltaTime);
					num4 = Mathf.Lerp(localScale.y, 28.4f, scaleSpeedDown * Time.deltaTime);
				}
				else
				{
					num3 = Mathf.Lerp(localScale.x, 40.4f, (scaleSpeedDown - 4f) * Time.deltaTime);
					num4 = Mathf.Lerp(localScale.y, 28.4f, (scaleSpeedDown - 4f) * Time.deltaTime);
				}
				map[0].localPosition = new Vector3(x, -6.29f, -2f);
				map[0].localScale = new Vector3(num3, num4, 1f);
				localPosition = map[2].localPosition;
				localScale = map[2].localScale;
				x = Mathf.Lerp(localPosition.x, -69f, speed * Time.deltaTime);
				num3 = Mathf.Lerp(localScale.x, 40.4f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				num4 = Mathf.Lerp(localScale.y, 28.4f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				map[2].localPosition = new Vector3(x, -6.29f, -2f);
				map[2].localScale = new Vector3(num3, num4, 1f);
				localPosition = map[3].localPosition;
				localScale = map[3].localScale;
				x = Mathf.Lerp(localPosition.x, 0f, speed * Time.deltaTime);
				num3 = Mathf.Lerp(localScale.x, 57.2f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				num4 = Mathf.Lerp(localScale.y, 40.2f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				map[3].localPosition = new Vector3(x, -6.29f, -4f);
				map[3].localScale = new Vector3(num3, num4, 1f);
				localPosition = map[1].localPosition;
				localScale = map[1].localScale;
				x = Mathf.Lerp(localPosition.x, 0f, speed * Time.deltaTime);
				num3 = Mathf.Lerp(localScale.x, 28f, scaleSpeedDown * Time.deltaTime);
				num4 = Mathf.Lerp(localScale.y, 19f, scaleSpeedDown * Time.deltaTime);
				map[1].localPosition = new Vector3(x, -6.29f, -1f);
				map[1].localScale = new Vector3(num3, num4, 1f);
				if (localPosition.x <= 0.5f && localScale.x <= 28.5f)
				{
					Transform transform = map[3];
					map[3] = map[2];
					map[2] = map[1];
					map[1] = map[0];
					map[0] = transform;
					mapName.GetComponent<NoiseAnim>().type = -2;
					mapName.GetComponent<NoiseAnim>().ignoreY = true;
					mapName.GetComponent<AnimateTexture>().alphaStart = 0.4f;
					TextureUtility.SetSpriteIndex(mapName, 1, 5, mapIndex);
					playAnim = 4;
				}
			}
			else if (playAnim == 4)
			{
				mapFrame.transform.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * 4.5f)));
				if (mapFrame.transform.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
				{
					fadeTime = 0f;
					playAnim = 0;
				}
			}
		}
		else
		{
			if (playAnim >= 0)
			{
				return;
			}
			MonoBehaviour.print("move to left " + playAnim);
			if (playAnim == -1)
			{
				int num5 = mapIndex + 1;
				if (num5 > 5)
				{
					num5 -= 5;
				}
				TextureUtility.SetSpriteIndex(map[2].gameObject, 5, 1, num5);
				mapName.GetComponent<NoiseAnim>().type = 0;
				mapName.GetComponent<NoiseAnim>().ignoreY = false;
				mapName.GetComponent<AnimateTexture>().alphaStart = 0f;
				playAnim = -2;
			}
			if (playAnim == -2)
			{
				float a2 = Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * 9.5f);
				float num6 = Mathf.Lerp(0.5f, 0.15f, fadeTime);
				mapFrame.transform.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, a2));
				map[0].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(num6, num6, num6, 0.5f));
				map[2].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(num6, num6, num6, 0.5f));
				map[1].GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f - num6 + 0.2f, 0.5f - num6 + 0.2f, 0.5f - num6 + 0.2f, 0.5f));
				if (mapFrame.transform.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
				{
					fadeTime = 0f;
					playAnim = -3;
				}
			}
			if (playAnim != -4)
			{
				Vector3 localPosition2 = map[0].localPosition;
				Vector3 localScale2 = map[0].localScale;
				float x2 = Mathf.Lerp(localPosition2.x, -69f, speed * Time.deltaTime);
				float num7 = Mathf.Lerp(localScale2.x, 40.4f, scaleSpeedDown * Time.deltaTime);
				float num8 = Mathf.Lerp(localScale2.y, 28.4f, scaleSpeedDown * Time.deltaTime);
				if (localPosition2.x < -50f)
				{
					num7 = Mathf.Lerp(localScale2.x, 40.4f, scaleSpeedDown * Time.deltaTime);
					num8 = Mathf.Lerp(localScale2.y, 28.4f, scaleSpeedDown * Time.deltaTime);
				}
				else
				{
					num7 = Mathf.Lerp(localScale2.x, 40.4f, (scaleSpeedDown - 4f) * Time.deltaTime);
					num8 = Mathf.Lerp(localScale2.y, 28.4f, (scaleSpeedDown - 4f) * Time.deltaTime);
				}
				map[0].localPosition = new Vector3(x2, -6.29f, -2f);
				map[0].localScale = new Vector3(num7, num8, 1f);
				localPosition2 = map[2].localPosition;
				localScale2 = map[2].localScale;
				x2 = Mathf.Lerp(localPosition2.x, 69f, speed * Time.deltaTime);
				num7 = Mathf.Lerp(localScale2.x, 40.4f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				num8 = Mathf.Lerp(localScale2.y, 28.4f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				map[2].localPosition = new Vector3(x2, -6.29f, -2f);
				map[2].localScale = new Vector3(num7, num8, 1f);
				localPosition2 = map[3].localPosition;
				localScale2 = map[3].localScale;
				x2 = Mathf.Lerp(localPosition2.x, 0f, speed * Time.deltaTime);
				num7 = Mathf.Lerp(localScale2.x, 28f, scaleSpeedDown * Time.deltaTime);
				num8 = Mathf.Lerp(localScale2.y, 19f, scaleSpeedDown * Time.deltaTime);
				map[3].localPosition = new Vector3(x2, -6.29f, -1f);
				map[3].localScale = new Vector3(num7, num8, 1f);
				localPosition2 = map[1].localPosition;
				localScale2 = map[1].localScale;
				x2 = Mathf.Lerp(localPosition2.x, 0f, speed * Time.deltaTime);
				num7 = Mathf.Lerp(localScale2.x, 57.2f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				num8 = Mathf.Lerp(localScale2.y, 40.2f, Mathf.Cos(scaleSpeedUp * (float)Math.PI * 0.5f));
				map[1].localPosition = new Vector3(x2, -6.29f, -4f);
				map[1].localScale = new Vector3(num7, num8, 1f);
				if (localPosition2.x <= 0.5f && localScale2.x >= 56.5f)
				{
					Transform transform2 = map[0];
					map[0] = map[1];
					map[1] = map[2];
					map[2] = map[3];
					map[3] = transform2;
					mapName.GetComponent<NoiseAnim>().type = -2;
					mapName.GetComponent<NoiseAnim>().ignoreY = true;
					mapName.GetComponent<AnimateTexture>().alphaStart = 0.4f;
					TextureUtility.SetSpriteIndex(mapName, 1, 5, mapIndex);
					playAnim = -4;
				}
			}
			else if (playAnim == -4)
			{
				mapFrame.transform.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * 4.5f)));
				if (mapFrame.transform.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
				{
					TextureUtility.SetSpriteIndex(mapName, 1, 5, mapIndex);
					fadeTime = 0f;
					playAnim = 0;
				}
			}
		}
	}

	private void idleButton(GameObject togButton)
	{
		if (togButton.name.Contains("btn"))
		{
			TextureUtility.SetSpriteIndex(togButton, 4, 1, 0);
		}
		else if (togButton.name.Contains("arrow"))
		{
			TextureUtility.SetSpriteIndex(togButton, 3, 1, 0);
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
