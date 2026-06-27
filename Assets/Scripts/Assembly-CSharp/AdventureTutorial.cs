using System;
using TNet;
using UnityEngine;

public class AdventureTutorial : MonoBehaviour
{
	public int step;

	public GameObject mark;

	private GameObject markFX;

	private GameObject inviPillar;

	private float holdTime;

	private bool start;

	private Transform gui;

	private Transform ui_dr;

	private Transform ui_dr_effect;

	private Transform prop_dialogMask;

	public float speed = 1f;

	private Transform ui_arrow;

	public int stepPopup;

	public bool isPopup;

	public bool wait;

	private int keepItem;

	private string[] tutorialText;

	private string[] word;

	private TextMesh[] line;

	private int word_index;

	private int line_index;

	private int round_index;

	private bool isWordRun;

	public bool ch_enabled;

	public TextAsset TutorialFile;

	private AudioClip[] soundClip;

	private void Awake()
	{
		Global.ctrlID = 0;
		initPopUp();
		if (Global.IsVoiceOn)
		{
			string[] array = new string[17]
			{
				"tutorial_00.0", "tutorial_00.1", "tutorial_00.2", "tutorial_01.0", "tutorial_02.0", "tutorial_03.0", "tutorial_04.0", "tutorial_05.0", "tutorial_06.1", "tutorial_07.0",
				"tutorial_08.0", "tutorial_09.0", "tutorial_09.1", "tutorial_09.2", "tutorial_10.0", "tutorial_11.0", "tutorial_12.0"
			};
			soundClip = new AudioClip[array.Length];
			for (int num = soundClip.Length - 1; num >= 0; num--)
			{
				soundClip[num] = Resources.Load("Sounds/VOVX/" + array[num]) as AudioClip;
			}
		}
	}

	private void OnDestroy()
	{
		if (Global.advUnlock == 0)
		{
			Global.advStar[Global.advUnlock++] = 1;
			Global.advStar[Global.advUnlock] = 0;
			GameSave.Save();
		}
		GameSound.StopVOV();
		soundClip = null;
	}

	private void Update()
	{
		if (isPopup)
		{
			popUp();
		}
		inputCtrl();
		List<GameCharacter> list = GameCharacter.List();
		foreach (GameCharacter item in list)
		{
			if (item.Type == Character.Player)
			{
				item.enabled = ch_enabled;
				break;
			}
		}
		switch (step)
		{
		case 0:
		{
			stepPopup = 0;
			isPopup = true;
			markFX = Global.Map.AddFX(0, 1, mark, 0f, 1f);
			List<GameCharacter> list13 = GameCharacter.List();
			foreach (GameCharacter item2 in list13)
			{
				if (item2.Type == Character.Player)
				{
					Disarm(item2);
					break;
				}
			}
			step++;
			break;
		}
		case 1:
		{
			List<GameCharacter> list23 = GameCharacter.List();
			{
				foreach (GameCharacter item3 in list23)
				{
					if (item3.Type == Character.Player)
					{
						Vector3 localPosition4 = item3.transform.localPosition;
						float x2 = localPosition4.x;
						float num5 = localPosition4.z - 1f;
						if (x2 * x2 + num5 * num5 < 0.0625f)
						{
							UnityEngine.Object.Destroy(markFX);
							markFX = null;
							item3.Infect(Virus.None, 15f);
							AddInviPillar(0, 0);
							step++;
							resetPopup();
							stepPopup = 1;
							isPopup = true;
						}
						else
						{
							Disarm(item3);
						}
						break;
					}
				}
				break;
			}
		}
		case 2:
		{
			List<GameBomb> list31 = GameBomb.List();
			if (list31.Count > 0)
			{
				list31[0].enabled = false;
				UnityEngine.Object.Destroy(inviPillar);
				markFX = Global.Map.AddFX(-2, -1, mark, 0f, 1f);
				inviPillar = null;
				step++;
				resetPopup();
				stepPopup = 2;
				isPopup = true;
			}
			break;
		}
		case 3:
		{
			List<GameCharacter> list18 = GameCharacter.List();
			foreach (GameCharacter item4 in list18)
			{
				if (item4.Type == Character.Player)
				{
					Disarm(item4);
					break;
				}
			}
			{
				foreach (GameCharacter item5 in list18)
				{
					if (item5.Type == Character.Player)
					{
						Vector3 localPosition2 = item5.transform.localPosition;
						float num = localPosition2.x - -2f;
						float num2 = localPosition2.z - -1f;
						if (num * num + num2 * num2 < 0.0625f)
						{
							UnityEngine.Object.Destroy(markFX);
							markFX = null;
							AddInviPillar(-1, -1);
							step++;
						}
					}
				}
				break;
			}
		}
		case 4:
		{
			List<GameCharacter> list3 = GameCharacter.List();
			foreach (GameCharacter item6 in list3)
			{
				if (item6.Type == Character.Player)
				{
					Disarm(item6);
					break;
				}
			}
			List<GameBomb> list4 = GameBomb.List();
			if (list4.Count == 0)
			{
				UnityEngine.Object.Destroy(inviPillar);
				inviPillar = null;
				step++;
			}
			else
			{
				list4[0].enabled = true;
				list4[0].Explode();
			}
			break;
		}
		case 5:
		{
			List<GameCharacter> list11 = GameCharacter.List();
			foreach (GameCharacter item7 in list11)
			{
				if (item7.Type == Character.Player)
				{
					Disarm(item7);
					break;
				}
			}
			List<GameItem> list12 = GameItem.List();
			foreach (GameItem item8 in list12)
			{
				item8.Deactive(false);
			}
			Transform parent = base.transform.Find("Items");
			GameObject gameObject = GameItem.Create(Item.FireUp);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = new Vector3(-1f, 0f, 1f);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			gameObject = GameItem.Create(Item.Glove);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = new Vector3(1f, 0f, 1f);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			step++;
			resetPopup();
			stepPopup = 3;
			isPopup = true;
			break;
		}
		case 6:
		{
			if (isPopup)
			{
				break;
			}
			List<GameCharacter> list9 = GameCharacter.List();
			foreach (GameCharacter item9 in list9)
			{
				if (item9.Type == Character.Player)
				{
					Disarm(item9);
					break;
				}
			}
			List<GameItem> list10 = GameItem.List();
			if (list10.Count == 1)
			{
				Debug.Log(list10[0].Type);
				if (list10[0].Type == Item.FireUp)
				{
					stepPopup = 5;
					keepItem = 2;
				}
				else
				{
					stepPopup = 4;
					keepItem = 1;
				}
				step++;
				resetPopup();
				isPopup = true;
			}
			break;
		}
		case 7:
		{
			List<GameCharacter> list27 = GameCharacter.List();
			foreach (GameCharacter item10 in list27)
			{
				if (item10.Type == Character.Player)
				{
					Disarm(item10);
					break;
				}
			}
			List<GameItem> list28 = GameItem.List();
			if (list28.Count == 0)
			{
				markFX = Global.Map.AddFX(1, 3, mark, 0f, 1f);
				step++;
				replaceDrText();
				resetPopup();
				stepPopup = 6;
				isPopup = true;
			}
			break;
		}
		case 8:
		{
			List<GameCharacter> list26 = GameCharacter.List();
			{
				foreach (GameCharacter item11 in list26)
				{
					if (item11.Type == Character.Player)
					{
						Vector3 localPosition5 = item11.transform.localPosition;
						float num6 = localPosition5.x - 1f;
						float num7 = localPosition5.z - 3f;
						if (num6 * num6 + num7 * num7 < 0.0625f)
						{
							UnityEngine.Object.Destroy(markFX);
							markFX = null;
							item11.Infect(Virus.None, 15f);
							AddInviPillar(0, 3);
							step++;
							resetPopup();
							stepPopup = 7;
							isPopup = true;
						}
						else
						{
							Disarm(item11);
						}
						break;
					}
				}
				break;
			}
		}
		case 9:
		{
			List<GameBomb> list22 = GameBomb.List();
			if (list22.Count > 0)
			{
				list22[0].enabled = false;
				step++;
			}
			break;
		}
		case 10:
		{
			List<GameBomb> list17 = GameBomb.List();
			if (list17[0].IsHeld)
			{
				list17[0].enabled = true;
				step++;
			}
			break;
		}
		case 11:
		{
			List<GameCharacter> list5 = GameCharacter.List();
			foreach (GameCharacter item12 in list5)
			{
				if (item12.Type == Character.Player)
				{
					Disarm(item12);
					break;
				}
			}
			foreach (GameCharacter item13 in list5)
			{
				if (item13.Type == Character.Player)
				{
					Transform child = item13.transform.Find("root").GetChild(0);
					if (child.GetComponent<Animation>().IsPlaying("holdBomb"))
					{
						holdTime += Time.deltaTime;
						break;
					}
				}
			}
			if (holdTime > 0.4f)
			{
				step++;
				resetPopup();
				stepPopup = 8;
				isPopup = true;
			}
			break;
		}
		case 12:
		{
			List<GameCharacter> list29 = GameCharacter.List();
			foreach (GameCharacter item14 in list29)
			{
				if (item14.Type == Character.Player)
				{
					int num8 = (int)item14.transform.Find("root").GetChild(0).localRotation.eulerAngles.y;
					if (num8 != 90)
					{
						Disarm(item14);
					}
					else
					{
						item14.Infect(Virus.None, 15f);
					}
					break;
				}
			}
			List<GameBomb> list30 = GameBomb.List();
			if (!list30[0].IsHeld)
			{
				UnityEngine.Object.Destroy(inviPillar);
				inviPillar = null;
				step++;
			}
			break;
		}
		case 13:
		{
			List<GameItem> list24 = GameItem.List();
			foreach (GameItem item15 in list24)
			{
				item15.Deactive(false);
			}
			List<GameCharacter> list25 = GameCharacter.List();
			foreach (GameCharacter item16 in list25)
			{
				if (item16.Type == Character.Player)
				{
					Disarm(item16);
					break;
				}
			}
			{
				foreach (GameCharacter item17 in list25)
				{
					if (item17.Type != Character.Monster)
					{
						continue;
					}
					if (item17.IsDead)
					{
						step++;
						resetPopup();
						stepPopup = 9;
						isPopup = true;
						Transform parent2 = base.transform.Find("Items");
						GameObject gameObject2 = GameItem.Create(Item.Remote);
						gameObject2.transform.parent = parent2;
						gameObject2.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
						gameObject2.transform.localScale = new Vector3(10f, 10f, 10f);
						if (!Global.Map.IsBlock(4, 3))
						{
							gameObject2.transform.localPosition = new Vector3(4f, 0f, 3f);
						}
						else if (!Global.Map.IsBlock(8, 3))
						{
							gameObject2.transform.localPosition = new Vector3(8f, 0f, 3f);
						}
					}
					break;
				}
				break;
			}
		}
		case 14:
		{
			List<GameCharacter> list20 = GameCharacter.List();
			foreach (GameCharacter item18 in list20)
			{
				if (item18.Type == Character.Player)
				{
					Disarm(item18);
					break;
				}
			}
			List<GameItem> list21 = GameItem.List();
			if (list21.Count == 0)
			{
				step++;
				resetPopup();
				stepPopup = 10;
				isPopup = true;
				markFX = Global.Map.AddFX(2, 2, mark, 0f, 1f);
			}
			break;
		}
		case 15:
		{
			List<GameCharacter> list19 = GameCharacter.List();
			{
				foreach (GameCharacter item19 in list19)
				{
					if (item19.Type == Character.Player)
					{
						Vector3 localPosition3 = item19.transform.localPosition;
						float num3 = localPosition3.x - 2f;
						float num4 = localPosition3.z - 2f;
						if (num3 * num3 + num4 * num4 < 0.0625f)
						{
							UnityEngine.Object.Destroy(markFX);
							markFX = null;
							item19.Infect(Virus.None, 15f);
							AddInviPillar(2, 1);
							step++;
						}
						else
						{
							Disarm(item19);
						}
						break;
					}
				}
				break;
			}
		}
		case 16:
		{
			List<GameBomb> list15 = GameBomb.List();
			if (list15.Count <= 0)
			{
				break;
			}
			list15[0].enabled = false;
			List<GameCharacter> list16 = GameCharacter.List();
			foreach (GameCharacter item20 in list16)
			{
				if (item20.Type == Character.Player)
				{
					Disarm(item20);
					break;
				}
			}
			UnityEngine.Object.Destroy(inviPillar);
			markFX = Global.Map.AddFX(0, 0, mark, 0f, 1f);
			inviPillar = null;
			step++;
			resetPopup();
			stepPopup = 11;
			isPopup = true;
			break;
		}
		case 17:
		{
			List<GameCharacter> list14 = GameCharacter.List();
			{
				foreach (GameCharacter item21 in list14)
				{
					if (item21.Type == Character.Player)
					{
						Vector3 localPosition = item21.transform.localPosition;
						float x = localPosition.x;
						float z = localPosition.z;
						if (x * x + z * z < 0.0625f)
						{
							UnityEngine.Object.Destroy(markFX);
							markFX = null;
							item21.Infect(Virus.None, 15f);
							AddInviPillar(0, 1);
							AddInviPillar(0, -1);
							step++;
						}
						else
						{
							Disarm(item21);
						}
						break;
					}
				}
				break;
			}
		}
		case 18:
		{
			List<GameBomb> list6 = GameBomb.List();
			if (list6.Count != 0)
			{
				break;
			}
			List<GameItem> list7 = GameItem.List();
			foreach (GameItem item22 in list7)
			{
				item22.Deactive(false);
			}
			List<GameCharacter> list8 = GameCharacter.List();
			foreach (GameCharacter item23 in list8)
			{
				if (item23.Type == Character.Player)
				{
					Disarm(item23);
					break;
				}
			}
			step++;
			resetPopup();
			stepPopup = 12;
			isPopup = true;
			break;
		}
		case 19:
		{
			List<GameCharacter> list2 = GameCharacter.List();
			foreach (GameCharacter item24 in list2)
			{
				if (item24.Type == Character.Player)
				{
					Disarm(item24);
					break;
				}
			}
			if (isPopup)
			{
				break;
			}
			{
				foreach (GameCharacter item25 in list2)
				{
					if (item25.Type == Character.Monster)
					{
						item25.enabled = false;
						break;
					}
				}
				break;
			}
		}
		}
	}

	private void AddInviPillar(int x, int y)
	{
		GameObject gameObject = new GameObject("Invi-Pillar");
		gameObject.transform.parent = base.transform.Find("Pillars");
		gameObject.transform.localPosition = new Vector3(x, 0f, y);
		gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.center = new Vector3(0f, 0.05f, 0f);
		boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
		inviPillar = gameObject;
	}

	private void Disarm(GameCharacter player)
	{
		player.Infect(Virus.Unarmed, 5f);
		Transform transform = player.transform.Find("FX (Virus)");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
	}

	private void initPopUp()
	{
		gui = base.transform.Find("GUI").Find("popupDialog");
		gui.localPosition = new Vector3(0f, -1352f, 0f);
		ui_dr = base.transform.Find("GUI").Find("ui_Dr");
		ui_dr.GetComponent<Renderer>().enabled = false;
		ui_dr_effect = base.transform.Find("GUI").Find("ui_Dr_effect");
		ui_dr_effect.GetComponent<Renderer>().enabled = false;
		ui_arrow = gui.Find("ui_arrow");
		ui_arrow.GetComponent<Renderer>().enabled = false;
		prop_dialogMask = base.transform.Find("GUI").Find("prop_dialogMask");
		prop_dialogMask.localPosition = new Vector3(15f, -1191f, 51f);
		line = gui.GetComponentsInChildren<TextMesh>();
		TextMesh[] array = line;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = string.Empty;
		}
		TextAsset tutorialFile = TutorialFile;
		tutorialText = tutorialFile.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
	}

	private void startPopup()
	{
		Vector3 localPosition = gui.localPosition;
		ch_enabled = false;
		if (localPosition.y >= -152f)
		{
			isWordRun = true;
			start = true;
			word = tutorialText[stepPopup].Split('*');
			round_index = 0;
			if (Global.IsVoiceOn)
			{
				int num = ((stepPopup != 6) ? stepPopup : ((keepItem != 1) ? 4 : 5));
				string text = "tutorial_" + num.ToString("D2") + "." + round_index;
				AudioClip clip = null;
				for (int num2 = soundClip.Length - 1; num2 >= 0; num2--)
				{
					if (soundClip[num2].name == text)
					{
						clip = soundClip[num2];
						break;
					}
				}
				GameSound.PreloadVOV(text, clip);
				GameSound.StartVOV(text);
			}
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = true;
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		localPosition.y += speed * Time.deltaTime * 1200f;
		if (localPosition.y >= -152f)
		{
			localPosition.y = -152f;
		}
		gui.localPosition = localPosition;
		isPopup = true;
		int num3 = (int)((localPosition.y + 152f + 1200f) / 12f);
		if (num3 <= 5)
		{
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = false;
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(2f / 3f, 0f));
			ui_dr.localScale = new Vector3(543f, 5.43f, 0f);
			ui_dr.localPosition = new Vector3(800f, -991f, 50f);
			float y = -931 - (100 - num3 * 10) * 260 / 100;
			prop_dialogMask.localPosition = new Vector3(15f, y, 51f);
		}
		else if (num3 <= 10)
		{
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = false;
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(2f / 3f, 0f));
			ui_dr.localScale = new Vector3(543f, 54.3f, 0f);
			ui_dr.localPosition = new Vector3(800f, -951f, 50f);
			float y2 = -931 - (100 - num3 * 10) * 260 / 100;
			prop_dialogMask.localPosition = new Vector3(15f, y2, 51f);
		}
		else if (num3 <= 15)
		{
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = false;
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(2f / 3f, 0f));
			ui_dr.localScale = new Vector3(543f, 270f, 0f);
			ui_dr.localPosition = new Vector3(800f, -736f, 50f);
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num3 <= 20)
		{
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 25)
		{
			ui_dr.GetComponent<Renderer>().enabled = false;
			ui_dr_effect.GetComponent<Renderer>().enabled = false;
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num3 <= 30)
		{
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num3 <= 35)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(1f / 3f, 0.5f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 40)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(2f / 3f, 0.5f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 45)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 50)
		{
			ui_dr.GetComponent<Renderer>().enabled = false;
			ui_dr_effect.GetComponent<Renderer>().enabled = false;
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num3 <= 55)
		{
			ui_dr.GetComponent<Renderer>().enabled = true;
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num3 <= 65)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(1f / 3f, 0.5f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 70)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(2f / 3f, 0.5f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 75)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else if (num3 <= 80)
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(1f / 3f, 0f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, 0.5f));
			ui_dr.localScale = new Vector3(543f, 543f, 0f);
			ui_dr.localPosition = new Vector3(800f, -461f, 50f);
			ui_dr_effect.GetComponent<Renderer>().enabled = true;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = true;
		}
		ui_dr.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
	}

	private void endPopup()
	{
		Vector3 localPosition = gui.localPosition;
		ch_enabled = true;
		if (localPosition.y <= -1352f)
		{
			isWordRun = false;
			start = false;
			isPopup = false;
			stepPopup++;
		}
		else if (localPosition.y <= -452f)
		{
			ui_dr.GetComponent<Renderer>().enabled = false;
		}
		else if (localPosition.y <= -152f)
		{
			ui_dr_effect.GetComponent<Renderer>().enabled = false;
			ui_dr.GetComponent<SpriteTextureTutorial>().enabled = false;
			Vector2 offset = new Vector2(2f / 3f, 0f);
			ui_dr.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			ui_dr.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * (300f + localPosition.y) / 300f));
		}
		if (localPosition.y <= -1352f)
		{
			localPosition.y = -1352f;
		}
		else
		{
			localPosition.y -= speed * Time.deltaTime * 1200f;
		}
		gui.localPosition = localPosition;
		int num = 100 - (int)((localPosition.y + 152f + 1200f) / 12f);
		if (num < 30)
		{
			prop_dialogMask.localPosition = new Vector3(15f, -931f, 51f);
		}
		else if (num >= 30 && num <= 80)
		{
			float y = -1191 + (100 - (num - 30) * 2) * 260 / 100;
			prop_dialogMask.localPosition = new Vector3(15f, y, 51f);
		}
		else
		{
			prop_dialogMask.localPosition = new Vector3(15f, -1191f, 51f);
		}
	}

	private void resetPopup()
	{
		if (isPopup)
		{
			isWordRun = false;
			start = false;
			isPopup = false;
		}
	}

	private void replaceDrText()
	{
		if (keepItem == 1)
		{
			stepPopup = 5;
			string newValue = tutorialText[5];
			tutorialText[6] = tutorialText[6].Replace("#replace_word", newValue);
		}
		else if (keepItem == 2)
		{
			stepPopup = 5;
			string newValue2 = tutorialText[4];
			tutorialText[6] = tutorialText[6].Replace("#replace_word", newValue2);
		}
	}

	private void popUp()
	{
		if (!start)
		{
			startPopup();
		}
		else if (isWordRun)
		{
			if (line_index == 4)
			{
				wait = true;
				ui_arrow.GetComponent<Renderer>().enabled = true;
				isWordRun = false;
				line_index = 0;
			}
			else if (word_index == word[round_index * 4 + line_index].Length)
			{
				line_index++;
				word_index = 0;
			}
			else
			{
				word_index++;
				line[line_index].text = word[round_index * 4 + line_index].Substring(0, word_index);
			}
		}
		else
		{
			if (wait)
			{
				return;
			}
			ui_arrow.GetComponent<Renderer>().enabled = false;
			TextMesh[] array = line;
			foreach (TextMesh textMesh in array)
			{
				textMesh.text = string.Empty;
			}
			GameSound.StopVOV();
			if (round_index >= word.Length / 4 - 1)
			{
				endPopup();
				return;
			}
			round_index++;
			isWordRun = true;
			if (Global.IsVoiceOn)
			{
				GameSound.StartVOV("tutorial_" + stepPopup.ToString("D2") + "." + round_index);
			}
		}
	}

	private void inputCtrl()
	{
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (controller == null || (!controller.DoBombDrop() && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space)))
			{
				continue;
			}
			if (isPopup && wait)
			{
				wait = false;
			}
			else if (isPopup && isWordRun)
			{
				line_index = 4;
				word_index = 0;
				for (int j = 0; j < 4; j++)
				{
					line[j].text = word[round_index * 4 + j];
				}
			}
		}
	}
}
