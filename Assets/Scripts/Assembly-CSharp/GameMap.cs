using System;
using TNet;
using UnityEngine;

public class GameMap : MonoBehaviour
{
	public const int MaxWidth = 17;

	public const int MaxHeight = 11;

	public const int MaxX = 8;

	public const int MaxY = 5;

	public const int FarX = 13;

	public const int FarY = 10;

	public Camera mainCamera;

	private GameObject bg;

	private GameObject pillars;

	private GameObject blocks;

	private GameObject far;

	private GameObject characters;

	private GameObject bombs;

	private GameObject items;

	private GameObject explosions;

	private GameObject blockRes;

	private GameObject rEffect;

	private GameObject rGo;

	private GameObject rReady;

	private GameObject rGoBomb;

	private GameObject rReadyBack;

	private GameObject wEffect;

	private GameObject wTxt;

	private GameObject wEff;

	private GameObject wEff2;

	private GameObject wWave;

	private Vector3[] respawnPoints;

	private List<Vector3> dropItems;

	private int[] itemList;

	private GameObject[][] pillarArr;

	private GameObject[][] blockArr;

	private GameObject fxs;

	private GameObject bombFX;

	private float quakeTime;

	public TextAsset mapResFile;

	private bool timeRun;

	public float remainTime;

	private bool readyGo;

	private bool warning;

	private int shrinkPatternID = -1;

	private int shrinkFlipSign;

	private int shrinkBlock;

	private int shrinkBlockMax;

	private float shrinkDiffTime;

	private int shrinkPointX;

	private int shrinkPointY;

	private int shrinkQointX;

	private int shrinkQointY;

	private Vector2[] shrinkBuffer;

	private string hurryID;

	private GameObject fadeBG;

	private float fadeOut = -0.25f;

	private float willEnd;

	public Texture barBG;

	public Texture barProgress;

	private static int[] readiness;

	private static int tnObjRunID;

	private bool[] iconDC = new bool[8];

	private bool popupDC = true;

	private List<int> blockXY;

	private float checkSend;

	private bool checkReady = true;

	private float checkTimeOut;

	private int fps;

	private float fpsCount;

	private bool fpsEnable;

	public bool IsFade
	{
		get
		{
			return fadeOut < 0.125f;
		}
	}

	public float RemainTime
	{
		get
		{
			return remainTime;
		}
	}

	private void Awake()
	{
		if (Global.Mode == GameMode.OnlineBattle)
		{
			Global.mapID = OnlineBattleLobbyRoom.GetNextMapID();
			tnObjRunID = (int)(GetComponent<TNObject>().uid + 1);
		}
		Global.Map = this;
		GameCharacter.Init();
		GameBomb.Init();
		GameItem.Init();
		GameGround.Init();
		fadeBG = base.transform.Find("GUI/fade_bg").gameObject;
		ValidateFadeBG();
		if (Global.Mode != GameMode.Adventure || Global.advStage != 0)
		{
			rEffect = GameObject.Find("ready effect").gameObject;
			rGo = GameObject.Find("go").gameObject;
			rReady = GameObject.Find("ready").gameObject;
			rGoBomb = GameObject.Find("obomb").gameObject;
			rReadyBack = GameObject.Find("readyback").gameObject;
			wEffect = GameObject.Find("warning effect").gameObject;
			wEff = GameObject.Find("woeff").gameObject;
			wEff2 = GameObject.Find("woeff_001").gameObject;
			wTxt = GameObject.Find("watchouttxt").gameObject;
			wWave = GameObject.Find("wowave").gameObject;
			wWave.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			wTxt.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
		}
		int level = Global.Level;
		string text = "Maps/Theme" + level.ToString("D2") + "/";
		dropItems = new List<Vector3>();
		pillarArr = new GameObject[17][];
		blockArr = new GameObject[17][];
		for (int i = 0; i < 17; i++)
		{
			pillarArr[i] = new GameObject[11];
			blockArr[i] = new GameObject[11];
		}
		fxs = new GameObject("FXs");
		fxs.transform.parent = base.transform;
		fxs.transform.localScale = new Vector3(10f, 10f, 10f);
		bombFX = Resources.Load("Environments/Explosion/Explosion") as GameObject;
		bg = new GameObject("BG");
		bg.transform.parent = base.transform;
		bg.transform.localScale = new Vector3(10f, 10f, 10f);
		GameObject gameObject = new GameObject("Floor");
		gameObject.transform.parent = bg.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = LayerMask.NameToLayer("Ground");
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.center = new Vector3(0f, -0.5f, 0f);
		boxCollider.size = new Vector3(17f, 1f, 11f);
		GameObject gameObject2 = new GameObject("Wall East");
		gameObject2.transform.parent = bg.transform;
		gameObject2.transform.localScale = Vector3.one;
		BoxCollider boxCollider2 = gameObject2.AddComponent<BoxCollider>();
		boxCollider2.center = new Vector3(9f, 0.5f, 0f);
		boxCollider2.size = new Vector3(1f, 1f, 11f);
		GameObject gameObject3 = new GameObject("Wall North");
		gameObject3.transform.parent = bg.transform;
		gameObject3.transform.localScale = Vector3.one;
		BoxCollider boxCollider3 = gameObject3.AddComponent<BoxCollider>();
		boxCollider3.center = new Vector3(0f, 0.5f, 6f);
		boxCollider3.size = new Vector3(17f, 1f, 1f);
		GameObject gameObject4 = new GameObject("Wall West");
		gameObject4.transform.parent = bg.transform;
		gameObject4.transform.localScale = Vector3.one;
		BoxCollider boxCollider4 = gameObject4.AddComponent<BoxCollider>();
		boxCollider4.center = new Vector3(-9f, 0.5f, 0f);
		boxCollider4.size = new Vector3(1f, 1f, 11f);
		GameObject gameObject5 = new GameObject("Wall South");
		gameObject5.transform.parent = bg.transform;
		gameObject5.transform.localScale = Vector3.one;
		BoxCollider boxCollider5 = gameObject5.AddComponent<BoxCollider>();
		boxCollider5.center = new Vector3(0f, 0.5f, -6f);
		boxCollider5.size = new Vector3(17f, 1f, 1f);
		string[] array = null;
		if (mapResFile != null)
		{
			TextAsset textAsset = mapResFile;
			array = textAsset.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			respawnPoints = new Vector3[8];
			for (int j = 0; j < array.Length; j++)
			{
				for (int k = 0; k < array[j].Length; k++)
				{
					if (array[j][k] >= '0' && array[j][k] <= '9')
					{
						respawnPoints[array[j][k] - 48] = new Vector3(k - 8, 0f, 5 - j);
					}
				}
			}
		}
		else
		{
			respawnPoints = new Vector3[8];
			respawnPoints[0] = new Vector3(-8f, 0f, 5f);
			respawnPoints[1] = new Vector3(8f, 0f, -5f);
			respawnPoints[2] = new Vector3(8f, 0f, 5f);
			respawnPoints[3] = new Vector3(-8f, 0f, -5f);
			respawnPoints[4] = new Vector3(-4f, 0f, 3f);
			respawnPoints[5] = new Vector3(4f, 0f, -3f);
			respawnPoints[6] = new Vector3(4f, 0f, 3f);
			respawnPoints[7] = new Vector3(-4f, 0f, -3f);
		}
		GameObject original = Resources.Load(text + "Pillar") as GameObject;
		pillars = new GameObject("Pillars");
		pillars.transform.parent = base.transform;
		pillars.transform.localScale = new Vector3(10f, 10f, 10f);
		if (array != null)
		{
			for (int l = 0; l < array.Length; l++)
			{
				for (int m = 0; m < array[l].Length; m++)
				{
					if (array[l][m] == 'x')
					{
						GameObject gameObject6 = UnityEngine.Object.Instantiate(original) as GameObject;
						gameObject6.name = "Pillar";
						gameObject6.transform.parent = pillars.transform;
						gameObject6.transform.localPosition = new Vector3(m - 8, 0f, 5 - l);
						gameObject6.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
						gameObject6.transform.localScale = new Vector3(10f, 10f, 10f);
						BoxCollider boxCollider6 = gameObject6.AddComponent<BoxCollider>();
						boxCollider6.center = new Vector3(0f, 0.05f, 0f);
						boxCollider6.size = new Vector3(0.1f, 0.1f, 0.1f);
						pillarArr[m][10 - l] = gameObject6;
					}
				}
			}
		}
		else
		{
			for (int n = -4; n <= 4; n += 2)
			{
				for (int num = -7; num <= 7; num += 2)
				{
					GameObject gameObject7 = UnityEngine.Object.Instantiate(original) as GameObject;
					gameObject7.name = "Pillar";
					gameObject7.transform.parent = pillars.transform;
					gameObject7.transform.localPosition = new Vector3(num, 0f, n);
					gameObject7.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					gameObject7.transform.localScale = new Vector3(10f, 10f, 10f);
					BoxCollider boxCollider7 = gameObject7.AddComponent<BoxCollider>();
					boxCollider7.center = new Vector3(0f, 0.05f, 0f);
					boxCollider7.size = new Vector3(0.1f, 0.1f, 0.1f);
					pillarArr[num + 8][n + 5] = gameObject7;
				}
			}
		}
		blockRes = Resources.Load(text + "Block") as GameObject;
		blocks = new GameObject("Blocks");
		blocks.transform.parent = base.transform;
		blocks.transform.localScale = new Vector3(10f, 10f, 10f);
		if (!Global.IsBossStage)
		{
			if (array != null)
			{
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					for (int num3 = 0; num3 < array[num2].Length; num3++)
					{
						if (array[num2][num3] == '*')
						{
							AddBlock(num3 - 8, 5 - num2);
						}
					}
				}
			}
			else if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				blockXY = ((Global.Mode != GameMode.OnlineBattle) ? null : new List<int>());
				for (int num4 = -5; num4 <= 5; num4++)
				{
					for (int num5 = -8; num5 <= 8; num5++)
					{
						if ((num5 > -7 || num4 < 4) && (num5 < 7 || num4 < 4) && (num5 > -7 || num4 > -4) && (num5 < 7 || num4 > -4) && (num5 >= -2 || num5 <= -5 || num4 <= 1 || num4 >= 4) && (num5 <= 2 || num5 >= 5 || num4 <= 1 || num4 >= 4) && (num5 >= -2 || num5 <= -5 || num4 >= -1 || num4 <= -4) && (num5 <= 2 || num5 >= 5 || num4 >= -1 || num4 <= -4) && (num5 % 2 == 0 || num4 % 2 != 0) && !(UnityEngine.Random.value < 0.6f))
						{
							if (blockXY == null)
							{
								AddBlock(num5, num4);
								continue;
							}
							blockXY.Add(num5);
							blockXY.Add(num4);
						}
					}
				}
				for (int num6 = -8; num6 <= 8; num6 += 2)
				{
					bool flag = true;
					for (int num7 = -1; num7 <= 1; num7++)
					{
						if (blockArr[num6 + 8][num7 + 5] != null)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						if (blockXY == null)
						{
							AddBlock(num6, UnityEngine.Random.Range(-1, 2));
							continue;
						}
						blockXY.Add(num6);
						blockXY.Add(UnityEngine.Random.Range(-1, 2));
					}
				}
				for (int num8 = -5; num8 <= 5; num8 += 2)
				{
					bool flag2 = true;
					bool flag3 = true;
					for (int num9 = -2; num9 <= 0; num9++)
					{
						if (blockArr[num9 + 8][num8 + 5] != null)
						{
							flag2 = false;
							break;
						}
					}
					for (int num10 = 0; num10 <= 2; num10++)
					{
						if (blockArr[num10 + 8][num8 + 5] != null)
						{
							flag3 = false;
							break;
						}
					}
					if (flag2 && flag3 && UnityEngine.Random.value < 1f / 3f)
					{
						if (blockXY == null)
						{
							AddBlock(0, num8);
							continue;
						}
						blockXY.Add(0);
						blockXY.Add(num8);
						continue;
					}
					if (flag2)
					{
						if (blockXY == null)
						{
							AddBlock(UnityEngine.Random.Range(-2, 0), num8);
						}
						else
						{
							blockXY.Add(UnityEngine.Random.Range(-2, 0));
							blockXY.Add(num8);
						}
					}
					if (flag3)
					{
						if (blockXY == null)
						{
							AddBlock(UnityEngine.Random.Range(1, 3), num8);
							continue;
						}
						blockXY.Add(UnityEngine.Random.Range(1, 3));
						blockXY.Add(num8);
					}
				}
				for (int num11 = -1; num11 <= 1; num11 += 2)
				{
					for (int num12 = -5; num12 <= 5; num12 += 2)
					{
						int num13 = num11 * ((Mathf.Abs(num12) != 3) ? 4 : 5);
						int num14 = num11 * 6;
						if (num14 < num13)
						{
							int num15 = num13;
							num13 = num14;
							num14 = num15;
						}
						bool flag4 = true;
						for (int num16 = num13; num16 <= num14; num16++)
						{
							if (blockArr[num16 + 8][num12 + 5] != null)
							{
								flag4 = false;
								break;
							}
						}
						if (flag4)
						{
							if (blockXY == null)
							{
								AddBlock(UnityEngine.Random.Range(num13, num14 + 1), num12);
								continue;
							}
							blockXY.Add(UnityEngine.Random.Range(num13, num14 + 1));
							blockXY.Add(num12);
						}
					}
				}
			}
		}
		original = Resources.Load(text + "Far") as GameObject;
		far = UnityEngine.Object.Instantiate(original) as GameObject;
		far.transform.parent = mainCamera.transform;
		far.transform.localPosition = new Vector3(0f, 0f, mainCamera.farClipPlane - 1f);
		far.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);
		far.transform.localScale = new Vector3(16.2f, 16.2f, 16.2f);
		bombs = new GameObject("Bombs");
		bombs.transform.parent = base.transform;
		bombs.transform.localScale = new Vector3(10f, 10f, 10f);
		items = new GameObject("Items");
		items.transform.parent = base.transform;
		items.transform.localScale = new Vector3(10f, 10f, 10f);
		characters = new GameObject("Characters");
		characters.transform.parent = base.transform;
		characters.transform.localScale = new Vector3(10f, 10f, 10f);
		explosions = new GameObject("Explosions");
		explosions.transform.parent = base.transform;
		explosions.transform.localScale = new Vector3(10f, 10f, 10f);
		if (Global.Mode != GameMode.OnlineBattle)
		{
			switch (level)
			{
			case 1:
				far.AddComponent<Theme01>();
				break;
			case 2:
				far.AddComponent<Theme02>();
				break;
			case 3:
				far.AddComponent<Theme03>();
				break;
			case 4:
				far.AddComponent<Theme04>();
				break;
			case 5:
				far.AddComponent<Theme05>();
				break;
			}
		}
		if ((Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle) && Global.mapShrink)
		{
			shrinkPointX = 8;
			shrinkPointY = 5;
			shrinkQointX = 8;
			shrinkQointY = -5;
			shrinkBuffer = new Vector2[0];
			if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				shrinkPatternID = UnityEngine.Random.Range(0, 8);
				switch (shrinkPatternID)
				{
				case 0:
					shrinkBlockMax = 94;
					break;
				case 1:
					shrinkBlockMax = 94;
					break;
				case 2:
					shrinkBlockMax = 94;
					break;
				case 3:
					shrinkBlockMax = 48;
					break;
				case 4:
					shrinkBlockMax = 93;
					break;
				case 5:
					shrinkBlockMax = 54;
					break;
				case 6:
					shrinkBlockMax = 6;
					break;
				case 7:
					shrinkBlockMax = 9;
					break;
				}
				shrinkBlock = 0;
				shrinkDiffTime = 28f / (float)(shrinkBlockMax - 1);
				shrinkFlipSign = ((UnityEngine.Random.value < 0.5f) ? 1 : (-1));
			}
		}
		hurryID = UnityEngine.Random.Range(1, 10).ToString("D2");
		GameSound.PreloadVOV("hurry_" + hurryID);
		if (Global.Mode == GameMode.OnlineBattle)
		{
			base.gameObject.AddComponent<GameIcon>();
		}
	}

	private void Start()
	{
		GameSound.StopBGM("Menu");
		GameSound.StartBGM((!Global.IsBossStage) ? ("Map" + Global.Level.ToString("D2")) : "BossFight");
		if (Global.Mode == GameMode.Adventure)
		{
			AdventureTutorial adventureTutorial = null;
			int[] array = null;
			if (AdventureGameMode.UI != null)
			{
				array = AdventureGameMode.UI.GetMonsterList();
				if (Global.IsBossStage)
				{
					timeRun = false;
					remainTime = 0f;
					AdventureGameMode.UI.ShowTime(remainTime);
				}
				else
				{
					timeRun = true;
					remainTime = 300f;
					AdventureGameMode.UI.ShowTime(remainTime);
				}
			}
			else
			{
				adventureTutorial = base.gameObject.GetComponent<AdventureTutorial>();
				if (adventureTutorial != null)
				{
					array = new int[8] { 0, 0, -1, -1, -1, -1, -1, -1 };
				}
			}
			if (array == null)
			{
				GameObject gameObject = GameCharacter.Create(Character.Player);
				gameObject.transform.parent = characters.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, -5f);
				gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
				gameObject = GameCharacter.Create(Character.Boss, 0, Global.Level);
				gameObject.transform.parent = characters.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 10f);
				gameObject.transform.localScale = new Vector3(25f, 25f, 25f);
			}
			else
			{
				int level = Global.Level;
				for (int i = 0; i < 8; i++)
				{
					GameObject gameObject2 = null;
					if (i == 0)
					{
						gameObject2 = GameCharacter.Create(Character.Player);
					}
					else if (array[i] != -1)
					{
						gameObject2 = GameCharacter.Create(Character.Monster, array[i], level);
					}
					if (gameObject2 != null)
					{
						gameObject2.transform.parent = characters.transform;
						gameObject2.transform.localPosition = respawnPoints[i];
						gameObject2.transform.localScale = new Vector3(10f, 10f, 10f);
					}
				}
			}
			itemList = ((!Global.IsBossStage && !(adventureTutorial != null)) ? AdventureGameMode.UI.GetItemList() : null);
			GameItem.Destroyable = true;
		}
		else if (Global.Mode == GameMode.LocalBattle)
		{
			timeRun = true;
			remainTime = Global.timeAmount * 60;
			for (int j = 0; j < 8; j++)
			{
				GameObject gameObject3 = null;
				if (Global.localPlayerSlot[j] >= CharSlot.Player1 && Global.localPlayerSlot[j] <= CharSlot.Player5)
				{
					int num = (int)(Global.localPlayerSlot[j] - 1);
					MonoBehaviour.print(j + " : " + num);
					gameObject3 = GameCharacter.Create(Character.Player, j, num);
				}
				else if (Global.localPlayerSlot[j] == CharSlot.AIEasy)
				{
					gameObject3 = GameCharacter.Create(Character.AI, j, 1);
				}
				else if (Global.localPlayerSlot[j] == CharSlot.AINormal)
				{
					gameObject3 = GameCharacter.Create(Character.AI, j, 3);
				}
				else if (Global.localPlayerSlot[j] == CharSlot.AIHard)
				{
					gameObject3 = GameCharacter.Create(Character.AI, j, 5);
				}
				if (gameObject3 != null)
				{
					gameObject3.transform.parent = characters.transform;
					gameObject3.transform.localPosition = respawnPoints[j];
					gameObject3.transform.localScale = new Vector3(10f, 10f, 10f);
				}
			}
			itemList = null;
			GameItem.Destroyable = Global.itemDestroy;
		}
		else if (Global.Mode == GameMode.OnlineBattle)
		{
			timeRun = true;
			remainTime = Global.timeAmount * 60;
			itemList = null;
			GameItem.Destroyable = Global.itemDestroy;
		}
		List<GameCharacter> list = GameCharacter.List();
		for (int num2 = list.Count - 1; num2 >= 1; num2--)
		{
			for (int num3 = num2 - 1; num3 >= 0; num3--)
			{
				Physics.IgnoreCollision(list[num2].GetComponent<Collider>(), list[num3].GetComponent<Collider>());
				if (list[num2].Type == Character.Monster && list[num3].Type == Character.Monster)
				{
					Transform transform = list[num2].transform.Find("root");
					Transform transform2 = list[num3].transform.Find("root");
					Physics.IgnoreCollision(transform.GetComponent<Collider>(), transform2.GetComponent<Collider>());
					Physics.IgnoreCollision(list[num2].GetComponent<Collider>(), transform2.GetComponent<Collider>());
					Physics.IgnoreCollision(list[num3].GetComponent<Collider>(), transform.GetComponent<Collider>());
				}
			}
		}
		if (Global.IsBossStage)
		{
			for (int num4 = list.Count - 1; num4 >= 1; num4--)
			{
				if (list[num4].Type == Character.Boss)
				{
					IgnoreMapCollision(list[num4].transform.Find("root").gameObject);
					break;
				}
			}
			Item[] array2 = new Item[3]
			{
				Item.FireUp,
				Item.BombUp,
				Item.SpeedUp
			};
			int[] array3 = new int[array2.Length];
			int[] array4 = new int[array2.Length];
			for (int num5 = array2.Length - 1; num5 >= 0; num5--)
			{
				bool flag;
				do
				{
					flag = false;
					array3[num5] = UnityEngine.Random.Range(-5, 6);
					array4[num5] = UnityEngine.Random.Range(-3, 1);
					for (int k = num5 + 1; k < array2.Length; k++)
					{
						if (array3[num5] == array3[k] && array4[num5] == array4[k])
						{
							flag = true;
							break;
						}
					}
				}
				while (flag || (array3[num5] % 2 != 0 && array4[num5] % 2 == 0));
				GameObject gameObject4 = GameItem.Create(array2[num5]);
				gameObject4.transform.parent = items.transform;
				gameObject4.transform.localPosition = new Vector3(array3[num5], 0f, array4[num5]);
				gameObject4.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				gameObject4.transform.localScale = new Vector3(10f, 10f, 10f);
			}
		}
		if (Global.Mode == GameMode.Adventure)
		{
			GenerateGroundAdventure();
		}
		else if (Global.Mode == GameMode.LocalBattle)
		{
			GenerateGroundBattle();
		}
		if (Global.Mode == GameMode.Adventure && Global.advStage == 0)
		{
			return;
		}
		foreach (GameCharacter item in list)
		{
			if (item.Type != Character.Monster)
			{
				item.enabled = false;
			}
		}
		if (Global.Mode == GameMode.OnlineBattle)
		{
			TNManager.client.SetTimeout(10);
			rEffect.GetComponent<Animation>().Stop();
		}
		else
		{
			readyGo = true;
		}
	}

	private void OnDestroy()
	{
		if (Global.Map == this)
		{
			Global.Map = null;
		}
		InitOnlineMode();
	}

	private void GenerateGroundAdventure()
	{
		switch (Global.advStage)
		{
		case 3:
			GameGround.Create(Ground.Redirect, -4, -3);
			GameGround.Create(Ground.Redirect, -4, 3, 90);
			GameGround.Create(Ground.Redirect, 4, 3, 180);
			GameGround.Create(Ground.Redirect, 4, -3, 270);
			break;
		case 4:
			GameGround.Create(Ground.Redirect, -4, -3);
			GameGround.Create(Ground.Redirect, -4, 3, 90);
			GameGround.Create(Ground.Redirect, 4, 3, 180);
			GameGround.Create(Ground.Redirect, 4, -3, 270);
			GameGround.Create(Ground.Redirect, 8, -5);
			GameGround.Create(Ground.Redirect, -8, -5, 90);
			GameGround.Create(Ground.Redirect, -8, 5, 180);
			GameGround.Create(Ground.Redirect, 8, 5, 270);
			break;
		case 5:
			GameGround.Create(Ground.Redirect, -4, -3);
			GameGround.Create(Ground.Redirect, -4, 3, 90);
			GameGround.Create(Ground.Redirect, 4, 3, 180);
			GameGround.Create(Ground.Redirect, 4, -3, 270);
			GameGround.Create(Ground.Redirect, 8, -5);
			GameGround.Create(Ground.Redirect, -8, -5, 90);
			GameGround.Create(Ground.Redirect, -8, 5, 180);
			GameGround.Create(Ground.Redirect, 8, 5, 270);
			GameGround.Create(Ground.Teleport, -6, -3);
			GameGround.Create(Ground.Teleport, -6, 3, 2);
			GameGround.Create(Ground.Teleport, 6, 3);
			GameGround.Create(Ground.Teleport, 6, -3, 2);
			RemoveBlock(6, 3);
			RemoveBlock(5, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(-5, 3);
			RemoveBlock(6, -3);
			RemoveBlock(5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-5, -3);
			if (!IsBlock(-8, 3))
			{
				AddBlock(-7, 3);
			}
			if (!IsBlock(-6, 5))
			{
				AddBlock(-6, 4);
			}
			break;
		case 8:
			GameGround.Create(Ground.Crack, 6, 3);
			GameGround.Create(Ground.Crack, -6, 3);
			GameGround.Create(Ground.Crack, 6, -3);
			GameGround.Create(Ground.Crack, -6, -3);
			RemoveBlock(6, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(6, -3);
			RemoveBlock(-6, -3);
			if (!IsBlock(-8, 3))
			{
				AddBlock(-7, 3);
			}
			if (!IsBlock(-6, 5))
			{
				AddBlock(-6, 4);
			}
			break;
		case 9:
			GameGround.Create(Ground.Crack, -6, 1);
			GameGround.Create(Ground.Crack, -6, -3);
			GameGround.Create(Ground.Crack, -2, -1);
			GameGround.Create(Ground.Crack, -2, 3);
			GameGround.Create(Ground.Crack, 2, 1);
			GameGround.Create(Ground.Crack, 2, -3);
			GameGround.Create(Ground.Crack, 6, -1);
			GameGround.Create(Ground.Crack, 6, 3);
			RemoveBlock(-6, 1);
			RemoveBlock(-6, -3);
			RemoveBlock(-2, -1);
			RemoveBlock(-2, 3);
			RemoveBlock(2, 1);
			RemoveBlock(2, -3);
			RemoveBlock(6, -1);
			RemoveBlock(6, 3);
			break;
		case 10:
			GameGround.Create(Ground.Redirect, -4, -3);
			GameGround.Create(Ground.Redirect, -4, 3, 90);
			GameGround.Create(Ground.Redirect, 4, 3, 180);
			GameGround.Create(Ground.Redirect, 4, -3, 270);
			GameGround.Create(Ground.Redirect, 8, -5);
			GameGround.Create(Ground.Redirect, -8, -5, 90);
			GameGround.Create(Ground.Redirect, -8, 5, 180);
			GameGround.Create(Ground.Redirect, 8, 5, 270);
			GameGround.Create(Ground.Crack, -2, 5);
			GameGround.Create(Ground.Crack, -4, 1);
			GameGround.Create(Ground.Crack, 2, -5);
			GameGround.Create(Ground.Crack, 4, -1);
			GameGround.Create(Ground.Crack, -8, -1);
			GameGround.Create(Ground.Crack, -2, -3);
			GameGround.Create(Ground.Crack, 8, 1);
			GameGround.Create(Ground.Crack, 2, 3);
			GameGround.Create(Ground.Crack, 0, 0);
			RemoveBlock(-4, 1);
			RemoveBlock(-2, 5);
			RemoveBlock(4, -1);
			RemoveBlock(2, -5);
			RemoveBlock(-8, -1);
			RemoveBlock(-2, -3);
			RemoveBlock(8, 1);
			RemoveBlock(2, 3);
			RemoveBlock(0, 0);
			break;
		case 11:
			GameGround.Create(Ground.Crack, -8, 1);
			GameGround.Create(Ground.Crack, -6, -1);
			GameGround.Create(Ground.Crack, -4, 3);
			GameGround.Create(Ground.Crack, -4, -5);
			GameGround.Create(Ground.Crack, -2, 5);
			GameGround.Create(Ground.Crack, -2, -3);
			GameGround.Create(Ground.Crack, -2, 1);
			GameGround.Create(Ground.Crack, 8, -1);
			GameGround.Create(Ground.Crack, 6, 1);
			GameGround.Create(Ground.Crack, 4, -3);
			GameGround.Create(Ground.Crack, 4, 5);
			GameGround.Create(Ground.Crack, 2, -5);
			GameGround.Create(Ground.Crack, 2, 3);
			GameGround.Create(Ground.Crack, 2, -1);
			GameGround.Create(Ground.Crack, 0, 0);
			RemoveBlock(-8, 1);
			RemoveBlock(-6, -1);
			RemoveBlock(-4, 3);
			RemoveBlock(-4, -5);
			RemoveBlock(-2, 5);
			RemoveBlock(-2, -3);
			RemoveBlock(-2, 1);
			RemoveBlock(8, -1);
			RemoveBlock(6, 1);
			RemoveBlock(4, -3);
			RemoveBlock(4, 5);
			RemoveBlock(2, -5);
			RemoveBlock(2, 3);
			RemoveBlock(2, -1);
			RemoveBlock(0, 0);
			GameGround.Create(Ground.Teleport, -4, -3);
			GameGround.Create(Ground.Teleport, 4, 3);
			break;
		case 20:
			GameGround.Create(Ground.Explode, 6, 3);
			GameGround.Create(Ground.Explode, -6, 3);
			GameGround.Create(Ground.Explode, 6, -3);
			GameGround.Create(Ground.Explode, -6, -3);
			RemoveBlock(6, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(6, -3);
			RemoveBlock(-6, -3);
			if (!IsBlock(-8, 3))
			{
				AddBlock(-7, 3);
			}
			if (!IsBlock(-6, 5))
			{
				AddBlock(-6, 4);
			}
			break;
		case 21:
			GameGround.Create(Ground.Explode, -6, 1);
			GameGround.Create(Ground.Explode, -6, -3);
			GameGround.Create(Ground.Explode, -2, -1);
			GameGround.Create(Ground.Explode, -2, 3);
			GameGround.Create(Ground.Explode, 2, 1);
			GameGround.Create(Ground.Explode, 2, -3);
			GameGround.Create(Ground.Explode, 6, -1);
			GameGround.Create(Ground.Explode, 6, 3);
			RemoveBlock(-6, 1);
			RemoveBlock(-6, -3);
			RemoveBlock(-2, -1);
			RemoveBlock(-2, 3);
			RemoveBlock(2, 1);
			RemoveBlock(2, -3);
			RemoveBlock(6, -1);
			RemoveBlock(6, 3);
			break;
		case 22:
			GameGround.Create(Ground.Explode, -8, -1);
			GameGround.Create(Ground.Explode, -6, -5);
			GameGround.Create(Ground.Explode, -4, 1);
			GameGround.Create(Ground.Explode, -2, 5);
			GameGround.Create(Ground.Explode, -2, -3);
			GameGround.Create(Ground.Explode, 0, 0);
			GameGround.Create(Ground.Explode, 8, 1);
			GameGround.Create(Ground.Explode, 6, 5);
			GameGround.Create(Ground.Explode, 4, -1);
			GameGround.Create(Ground.Explode, 2, -5);
			GameGround.Create(Ground.Explode, 2, 3);
			RemoveBlock(-8, -1);
			RemoveBlock(-6, -5);
			RemoveBlock(-4, 1);
			RemoveBlock(-2, 5);
			RemoveBlock(-2, -3);
			RemoveBlock(0, 0);
			RemoveBlock(8, 1);
			RemoveBlock(6, 5);
			RemoveBlock(4, -1);
			RemoveBlock(2, -5);
			RemoveBlock(2, 3);
			break;
		case 23:
			GameGround.Create(Ground.Explode, -8, 1);
			GameGround.Create(Ground.Explode, -6, -1);
			GameGround.Create(Ground.Explode, -4, -3);
			GameGround.Create(Ground.Explode, -2, -5);
			GameGround.Create(Ground.Explode, 0, -3);
			GameGround.Create(Ground.Explode, -2, 1);
			GameGround.Create(Ground.Explode, -4, 3);
			GameGround.Create(Ground.Explode, 8, -1);
			GameGround.Create(Ground.Explode, 6, 1);
			GameGround.Create(Ground.Explode, 4, 3);
			GameGround.Create(Ground.Explode, 2, 5);
			GameGround.Create(Ground.Explode, 0, 3);
			GameGround.Create(Ground.Explode, 2, -1);
			GameGround.Create(Ground.Explode, 4, -3);
			RemoveBlock(-8, 1);
			RemoveBlock(-6, -1);
			RemoveBlock(-4, -3);
			RemoveBlock(-2, -5);
			RemoveBlock(0, -3);
			RemoveBlock(-2, 1);
			RemoveBlock(-4, 3);
			RemoveBlock(8, -1);
			RemoveBlock(6, 1);
			RemoveBlock(4, 3);
			RemoveBlock(2, 5);
			RemoveBlock(0, 3);
			RemoveBlock(2, -1);
			RemoveBlock(4, -3);
			break;
		case 25:
			GameGround.Create(Ground.Teleport, 6, 3, 3);
			GameGround.Create(Ground.Teleport, -6, 3, 3);
			GameGround.Create(Ground.Teleport, 6, -3, 3);
			GameGround.Create(Ground.Teleport, -6, -3, 3);
			RemoveBlock(6, 3);
			RemoveBlock(5, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(-5, 3);
			RemoveBlock(6, -3);
			RemoveBlock(5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-5, -3);
			if (!IsBlock(-8, 3))
			{
				AddBlock(-7, 3);
			}
			if (!IsBlock(-6, 5))
			{
				AddBlock(-6, 4);
			}
			break;
		case 26:
			GameGround.Create(Ground.Pull, 6, 3, 3);
			GameGround.Create(Ground.Pull, -6, -3, 3);
			RemoveBlock(6, 2);
			RemoveBlock(5, 3);
			RemoveBlock(6, 3);
			RemoveBlock(7, 3);
			RemoveBlock(6, 4);
			RemoveBlock(-6, -2);
			RemoveBlock(-5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-7, -3);
			RemoveBlock(-6, -4);
			GameGround.Create(Ground.Teleport, -6, 1, 3);
			GameGround.Create(Ground.Teleport, 0, -5, 3);
			GameGround.Create(Ground.Teleport, 0, 0, 3);
			GameGround.Create(Ground.Teleport, 0, 5, 3);
			GameGround.Create(Ground.Teleport, 6, -1, 3);
			RemoveBlock(-6, 1);
			RemoveBlock(-6, 0);
			RemoveBlock(0, -5);
			RemoveBlock(0, -4);
			RemoveBlock(0, 0);
			RemoveBlock(0, -1);
			RemoveBlock(0, 1);
			RemoveBlock(0, 5);
			RemoveBlock(0, 4);
			RemoveBlock(6, -1);
			RemoveBlock(6, 0);
			break;
		case 27:
			GameGround.Create(Ground.Pull, 6, 3, 3);
			GameGround.Create(Ground.Pull, -6, -3, 3);
			RemoveBlock(6, 2);
			RemoveBlock(5, 3);
			RemoveBlock(6, 3);
			RemoveBlock(7, 3);
			RemoveBlock(6, 4);
			RemoveBlock(-6, -2);
			RemoveBlock(-5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-7, -3);
			RemoveBlock(-6, -4);
			GameGround.Create(Ground.Pull, 0, 0, 5);
			RemoveBlock(-1, 2);
			RemoveBlock(0, 2);
			RemoveBlock(1, 2);
			RemoveBlock(-2, 1);
			RemoveBlock(-1, 1);
			RemoveBlock(0, 1);
			RemoveBlock(1, 1);
			RemoveBlock(2, 1);
			RemoveBlock(-2, 0);
			RemoveBlock(-1, 0);
			RemoveBlock(0, 0);
			RemoveBlock(1, 0);
			RemoveBlock(2, 0);
			RemoveBlock(-2, -1);
			RemoveBlock(-1, -1);
			RemoveBlock(0, -1);
			RemoveBlock(1, -1);
			RemoveBlock(2, -1);
			RemoveBlock(-1, -2);
			RemoveBlock(0, -2);
			RemoveBlock(1, -2);
			GameGround.Create(Ground.Teleport, -8, 1, 3);
			GameGround.Create(Ground.Teleport, -2, -5, 3);
			GameGround.Create(Ground.Teleport, 2, 5, 3);
			GameGround.Create(Ground.Teleport, 8, -1, 3);
			RemoveBlock(-8, 1);
			RemoveBlock(-8, 0);
			RemoveBlock(-2, -5);
			RemoveBlock(-2, -4);
			RemoveBlock(2, 5);
			RemoveBlock(2, 4);
			RemoveBlock(8, -1);
			RemoveBlock(8, 0);
			break;
		case 28:
			GameGround.Create(Ground.Pull, 6, 3, 3);
			GameGround.Create(Ground.Pull, -6, 3, 3);
			GameGround.Create(Ground.Pull, 6, -3, 3);
			GameGround.Create(Ground.Pull, -6, -3, 3);
			RemoveBlock(6, 2);
			RemoveBlock(5, 3);
			RemoveBlock(6, 3);
			RemoveBlock(7, 3);
			RemoveBlock(6, 4);
			RemoveBlock(-6, 2);
			RemoveBlock(-5, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(-7, 3);
			RemoveBlock(-6, 4);
			RemoveBlock(6, -2);
			RemoveBlock(5, -3);
			RemoveBlock(6, -3);
			RemoveBlock(7, -3);
			RemoveBlock(6, -4);
			RemoveBlock(-6, -2);
			RemoveBlock(-5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-7, -3);
			RemoveBlock(-6, -4);
			GameGround.Create(Ground.Pull, 0, 0, 5);
			RemoveBlock(-1, 2);
			RemoveBlock(0, 2);
			RemoveBlock(1, 2);
			RemoveBlock(-2, 1);
			RemoveBlock(-1, 1);
			RemoveBlock(0, 1);
			RemoveBlock(1, 1);
			RemoveBlock(2, 1);
			RemoveBlock(-2, 0);
			RemoveBlock(-1, 0);
			RemoveBlock(0, 0);
			RemoveBlock(1, 0);
			RemoveBlock(2, 0);
			RemoveBlock(-2, -1);
			RemoveBlock(-1, -1);
			RemoveBlock(0, -1);
			RemoveBlock(1, -1);
			RemoveBlock(2, -1);
			RemoveBlock(-1, -2);
			RemoveBlock(0, -2);
			RemoveBlock(1, -2);
			break;
		case 29:
			GameGround.Create(Ground.Pull, 0, 0);
			GameGround.Create(Ground.Teleport, 6, 3, 3);
			GameGround.Create(Ground.Teleport, -6, 3, 3);
			GameGround.Create(Ground.Teleport, 6, -3, 3);
			GameGround.Create(Ground.Teleport, -6, -3, 3);
			RemoveBlock(6, 3);
			RemoveBlock(5, 3);
			RemoveBlock(-6, 3);
			RemoveBlock(-5, 3);
			RemoveBlock(6, -3);
			RemoveBlock(5, -3);
			RemoveBlock(-6, -3);
			RemoveBlock(-5, -3);
			if (!IsBlock(-8, 3))
			{
				AddBlock(-7, 3);
			}
			if (!IsBlock(-6, 5))
			{
				AddBlock(-6, 4);
			}
			break;
		case 6:
		case 7:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 24:
			break;
		}
	}

	private void GenerateGroundBattle()
	{
		switch (Global.mapID)
		{
		case 1:
			switch (UnityEngine.Random.Range(0, 5))
			{
			case 1:
				CreateGround(Ground.Redirect, -4, -3);
				CreateGround(Ground.Redirect, -4, 3, 90);
				CreateGround(Ground.Redirect, 4, 3, 180);
				CreateGround(Ground.Redirect, 4, -3, 270);
				CreateGround(Ground.Redirect, 8, -5);
				CreateGround(Ground.Redirect, -8, -5, 90);
				CreateGround(Ground.Redirect, -8, 5, 180);
				CreateGround(Ground.Redirect, 8, 5, 270);
				break;
			case 2:
				CreateGround(Ground.Redirect, 4, -3);
				CreateGround(Ground.Redirect, -4, -3, 90);
				CreateGround(Ground.Redirect, -4, 3, 180);
				CreateGround(Ground.Redirect, 4, 3, 270);
				CreateGround(Ground.Redirect, -8, -5);
				CreateGround(Ground.Redirect, -8, 5, 90);
				CreateGround(Ground.Redirect, 8, 5, 180);
				CreateGround(Ground.Redirect, 8, -5, 270);
				break;
			case 3:
			{
				CreateGround(Ground.Redirect, -4, -3);
				CreateGround(Ground.Redirect, -4, 3, 90);
				CreateGround(Ground.Redirect, 4, 3, 180);
				CreateGround(Ground.Redirect, 4, -3, 270);
				CreateGround(Ground.Redirect, 8, -5);
				CreateGround(Ground.Redirect, -8, -5, 90);
				CreateGround(Ground.Redirect, -8, 5, 180);
				CreateGround(Ground.Redirect, 8, 5, 270);
				int num6 = 1;
				CreateGround(Ground.Teleport, num6 * -2, -4);
				CreateGround(Ground.Teleport, num6 * -6, 1, 2);
				CreateGround(Ground.Teleport, num6 * 2, 4);
				CreateGround(Ground.Teleport, num6 * 6, -1, 2);
				RemoveBlock(num6 * -2, -4);
				RemoveBlock(num6 * -2, -5);
				RemoveBlock(num6 * -6, 1);
				RemoveBlock(num6 * -7, 1);
				AddBlock(num6 * -8, 1);
				RemoveBlock(num6 * 2, 4);
				RemoveBlock(num6 * 2, 5);
				RemoveBlock(num6 * 6, -1);
				RemoveBlock(num6 * 7, -1);
				AddBlock(num6 * 8, -1);
				if (!IsBlock(num6 * -5, 1))
				{
					AddBlock(num6 * -4, 1);
				}
				if (!IsBlock(num6 * 5, -1))
				{
					AddBlock(num6 * 4, -1);
				}
				if (!IsBlock(num6 * -6, 3))
				{
					AddBlock(num6 * -6, 2);
				}
				if (!IsBlock(num6 * 6, -3))
				{
					AddBlock(num6 * 6, -2);
				}
				break;
			}
			case 4:
			{
				CreateGround(Ground.Redirect, 4, -3);
				CreateGround(Ground.Redirect, -4, -3, 90);
				CreateGround(Ground.Redirect, -4, 3, 180);
				CreateGround(Ground.Redirect, 4, 3, 270);
				CreateGround(Ground.Redirect, -8, -5);
				CreateGround(Ground.Redirect, -8, 5, 90);
				CreateGround(Ground.Redirect, 8, 5, 180);
				CreateGround(Ground.Redirect, 8, -5, 270);
				int num5 = -1;
				CreateGround(Ground.Teleport, num5 * -2, -4, 2);
				CreateGround(Ground.Teleport, num5 * -6, 1);
				CreateGround(Ground.Teleport, num5 * 2, 4, 2);
				CreateGround(Ground.Teleport, num5 * 6, -1);
				RemoveBlock(num5 * -2, -4);
				RemoveBlock(num5 * -2, -5);
				RemoveBlock(num5 * -6, 1);
				RemoveBlock(num5 * -7, 1);
				AddBlock(num5 * -8, 1);
				RemoveBlock(num5 * 2, 4);
				RemoveBlock(num5 * 2, 5);
				RemoveBlock(num5 * 6, -1);
				RemoveBlock(num5 * 7, -1);
				AddBlock(num5 * 8, -1);
				if (!IsBlock(num5 * -5, 1))
				{
					AddBlock(num5 * -4, 1);
				}
				if (!IsBlock(num5 * 5, -1))
				{
					AddBlock(num5 * 4, -1);
				}
				if (!IsBlock(num5 * -6, 3))
				{
					AddBlock(num5 * -6, 2);
				}
				if (!IsBlock(num5 * 6, -3))
				{
					AddBlock(num5 * 6, -2);
				}
				break;
			}
			}
			break;
		case 2:
		{
			int num8;
			int num7;
			int num9;
			switch (UnityEngine.Random.Range(0, 6))
			{
			default:
				return;
			case 1:
				CreateGround(Ground.Crack, 6, 3);
				CreateGround(Ground.Crack, -6, 3);
				CreateGround(Ground.Crack, 6, -3);
				CreateGround(Ground.Crack, -6, -3);
				RemoveBlock(6, 3);
				RemoveBlock(-6, 3);
				RemoveBlock(6, -3);
				RemoveBlock(-6, -3);
				if (!IsBlock(8, 3))
				{
					AddBlock(7, 3);
				}
				if (!IsBlock(6, 5))
				{
					AddBlock(6, 4);
				}
				if (!IsBlock(-8, 3))
				{
					AddBlock(-7, 3);
				}
				if (!IsBlock(-6, 5))
				{
					AddBlock(-6, 4);
				}
				if (!IsBlock(8, -3))
				{
					AddBlock(7, -3);
				}
				if (!IsBlock(6, -5))
				{
					AddBlock(6, -4);
				}
				if (!IsBlock(-8, -3))
				{
					AddBlock(-7, -3);
				}
				if (!IsBlock(-6, -5))
				{
					AddBlock(-6, -4);
				}
				return;
			case 2:
				num8 = 1;
				goto IL_059c;
			case 3:
				num8 = -1;
				goto IL_059c;
			case 4:
				num7 = 1;
				break;
			case 5:
				{
					num7 = -1;
					break;
				}
				IL_059c:
				num9 = num8;
				CreateGround(Ground.Crack, num9 * -1, -5);
				CreateGround(Ground.Crack, num9 * 0, -4);
				CreateGround(Ground.Crack, num9 * 1, -3);
				CreateGround(Ground.Crack, num9 * 0, -2);
				CreateGround(Ground.Crack, num9 * -1, -1);
				CreateGround(Ground.Crack, num9 * 0, 0);
				CreateGround(Ground.Crack, num9 * 1, 1);
				CreateGround(Ground.Crack, num9 * 0, 2);
				CreateGround(Ground.Crack, num9 * -1, 3);
				CreateGround(Ground.Crack, num9 * 0, 4);
				CreateGround(Ground.Crack, num9 * 1, 5);
				RemoveBlock(num9 * -1, -5);
				RemoveBlock(num9 * 0, -4);
				RemoveBlock(num9 * 1, -3);
				RemoveBlock(num9 * 0, -2);
				RemoveBlock(num9 * -1, -1);
				RemoveBlock(num9 * 0, 0);
				RemoveBlock(num9 * 1, 1);
				RemoveBlock(num9 * 0, 2);
				RemoveBlock(num9 * -1, 3);
				RemoveBlock(num9 * 0, 4);
				RemoveBlock(num9 * 1, 5);
				CreateGround(Ground.Teleport, 6, 0);
				CreateGround(Ground.Teleport, -6, 0);
				RemoveBlock(6, 1);
				RemoveBlock(-6, 1);
				RemoveBlock(6, 0);
				RemoveBlock(-6, 0);
				RemoveBlock(6, -1);
				RemoveBlock(-6, -1);
				return;
			}
			int num10 = num7;
			CreateGround(Ground.Crack, num10 * -6, 1);
			CreateGround(Ground.Crack, num10 * -6, -3);
			CreateGround(Ground.Crack, num10 * -2, -1);
			CreateGround(Ground.Crack, num10 * -2, 3);
			CreateGround(Ground.Crack, num10 * 2, 1);
			CreateGround(Ground.Crack, num10 * 2, -3);
			CreateGround(Ground.Crack, num10 * 6, -1);
			CreateGround(Ground.Crack, num10 * 6, 3);
			RemoveBlock(num10 * -6, 1);
			RemoveBlock(num10 * -6, -3);
			RemoveBlock(num10 * -2, -1);
			RemoveBlock(num10 * -2, 3);
			RemoveBlock(num10 * 2, 1);
			RemoveBlock(num10 * 2, -3);
			RemoveBlock(num10 * 6, -1);
			RemoveBlock(num10 * 6, 3);
			break;
		}
		case 4:
		{
			int num3;
			switch (UnityEngine.Random.Range(0, 4))
			{
			default:
				return;
			case 1:
				CreateGround(Ground.Explode, 6, 3);
				CreateGround(Ground.Explode, -6, 3);
				CreateGround(Ground.Explode, 6, -3);
				CreateGround(Ground.Explode, -6, -3);
				RemoveBlock(6, 3);
				RemoveBlock(-6, 3);
				RemoveBlock(6, -3);
				RemoveBlock(-6, -3);
				if (!IsBlock(8, 3))
				{
					AddBlock(7, 3);
				}
				if (!IsBlock(6, 5))
				{
					AddBlock(6, 4);
				}
				if (!IsBlock(-8, 3))
				{
					AddBlock(-7, 3);
				}
				if (!IsBlock(-6, 5))
				{
					AddBlock(-6, 4);
				}
				if (!IsBlock(8, -3))
				{
					AddBlock(7, -3);
				}
				if (!IsBlock(6, -5))
				{
					AddBlock(6, -4);
				}
				if (!IsBlock(-8, -3))
				{
					AddBlock(-7, -3);
				}
				if (!IsBlock(-6, -5))
				{
					AddBlock(-6, -4);
				}
				return;
			case 2:
				num3 = 1;
				break;
			case 3:
				num3 = -1;
				break;
			}
			int num4 = num3;
			CreateGround(Ground.Explode, num4 * -6, 1);
			CreateGround(Ground.Explode, num4 * -6, -3);
			CreateGround(Ground.Explode, num4 * -2, -1);
			CreateGround(Ground.Explode, num4 * -2, 3);
			CreateGround(Ground.Explode, num4 * 2, 1);
			CreateGround(Ground.Explode, num4 * 2, -3);
			CreateGround(Ground.Explode, num4 * 6, -1);
			CreateGround(Ground.Explode, num4 * 6, 3);
			RemoveBlock(num4 * -6, 1);
			RemoveBlock(num4 * -6, -3);
			RemoveBlock(num4 * -2, -1);
			RemoveBlock(num4 * -2, 3);
			RemoveBlock(num4 * 2, 1);
			RemoveBlock(num4 * 2, -3);
			RemoveBlock(num4 * 6, -1);
			RemoveBlock(num4 * 6, 3);
			break;
		}
		case 5:
		{
			int num;
			int num2;
			switch (UnityEngine.Random.Range(0, 5))
			{
			case 1:
				CreateGround(Ground.Pull, 6, 3, 3);
				CreateGround(Ground.Pull, -6, 3, 3);
				CreateGround(Ground.Pull, 6, -3, 3);
				CreateGround(Ground.Pull, -6, -3, 3);
				RemoveBlock(6, 2);
				RemoveBlock(5, 3);
				RemoveBlock(6, 3);
				RemoveBlock(7, 3);
				RemoveBlock(6, 4);
				RemoveBlock(-6, 2);
				RemoveBlock(-5, 3);
				RemoveBlock(-6, 3);
				RemoveBlock(-7, 3);
				RemoveBlock(-6, 4);
				RemoveBlock(6, -2);
				RemoveBlock(5, -3);
				RemoveBlock(6, -3);
				RemoveBlock(7, -3);
				RemoveBlock(6, -4);
				RemoveBlock(-6, -2);
				RemoveBlock(-5, -3);
				RemoveBlock(-6, -3);
				RemoveBlock(-7, -3);
				RemoveBlock(-6, -4);
				CreateGround(Ground.Pull, 0, 0, 5);
				RemoveBlock(-1, 2);
				RemoveBlock(0, 2);
				RemoveBlock(1, 2);
				RemoveBlock(-2, 1);
				RemoveBlock(-1, 1);
				RemoveBlock(0, 1);
				RemoveBlock(1, 1);
				RemoveBlock(2, 1);
				RemoveBlock(-2, 0);
				RemoveBlock(-1, 0);
				RemoveBlock(0, 0);
				RemoveBlock(1, 0);
				RemoveBlock(2, 0);
				RemoveBlock(-2, -1);
				RemoveBlock(-1, -1);
				RemoveBlock(0, -1);
				RemoveBlock(1, -1);
				RemoveBlock(2, -1);
				RemoveBlock(-1, -2);
				RemoveBlock(0, -2);
				RemoveBlock(1, -2);
				break;
			case 2:
				num = 1;
				goto IL_0c33;
			case 3:
				num = -1;
				goto IL_0c33;
			case 4:
				{
					CreateGround(Ground.Pull, 0, 0);
					break;
				}
				IL_0c33:
				num2 = num;
				CreateGround(Ground.Teleport, num2 * -2, -4, 3);
				CreateGround(Ground.Teleport, num2 * -6, 1, 3);
				CreateGround(Ground.Teleport, num2 * 2, 4, 3);
				CreateGround(Ground.Teleport, num2 * 6, -1, 3);
				RemoveBlock(num2 * -2, -4);
				RemoveBlock(num2 * -2, -5);
				RemoveBlock(num2 * -6, 1);
				RemoveBlock(num2 * -7, 1);
				AddBlock(num2 * -8, 1);
				RemoveBlock(num2 * 2, 4);
				RemoveBlock(num2 * 2, 5);
				RemoveBlock(num2 * 6, -1);
				RemoveBlock(num2 * 7, -1);
				AddBlock(num2 * 8, -1);
				if (!IsBlock(num2 * -5, 1))
				{
					AddBlock(num2 * -4, 1);
				}
				if (!IsBlock(num2 * 5, -1))
				{
					AddBlock(num2 * 4, -1);
				}
				if (!IsBlock(num2 * -6, 3))
				{
					AddBlock(num2 * -6, 2);
				}
				if (!IsBlock(num2 * 6, -3))
				{
					AddBlock(num2 * 6, -2);
				}
				CreateGround(Ground.Pull, 0, 0);
				break;
			}
			break;
		}
		case 3:
			break;
		}
	}

	private void ValidateFadeBG()
	{
		float num;
		if (fadeOut == float.MaxValue)
		{
			num = 0f;
		}
		else
		{
			num = ((!(fadeOut >= 0f)) ? (-8f * fadeOut) : (1f - 8f * fadeOut));
			if (num > 1f)
			{
				num = 1f;
			}
		}
		if (num > 0f)
		{
			if (!fadeBG.activeSelf)
			{
				fadeBG.SetActive(true);
			}
			fadeBG.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num));
		}
		else if (fadeBG.activeSelf)
		{
			fadeBG.SetActive(false);
		}
	}

	[RFC(25)]
	private void GameMapUpdateFade(int matchID, float fadeOut)
	{
		if (Global.onlineMatchID == matchID)
		{
			this.fadeOut = fadeOut;
			ValidateFadeBG();
			Global.advClear = false;
			if (fadeOut == 0.5f)
			{
				DisableAll(false);
			}
		}
	}

	[RFC(26)]
	private void GameMapUpdateTime(int matchID, float remainTime)
	{
		if (Global.onlineMatchID == matchID && this.remainTime > remainTime)
		{
			this.remainTime = remainTime;
			AdventureGameMode.UI.ShowTime(remainTime);
		}
	}

	[RFC(28)]
	private void GameMapDefineWinner(int matchID, int winnerID)
	{
		if (Global.onlineMatchID == matchID)
		{
			Global.winID = winnerID;
			Application.LoadLevel("OnlineBattle.Summary");
		}
	}

	private void OnGUI()
	{
		if (Global.Mode == GameMode.OnlineBattle && fadeOut < 0f)
		{
			float num = -8f * fadeOut;
			if (num > 1f)
			{
				num = 1f;
			}
			float num2 = (float)Screen.height / 1080f;
			GUI.color = new Color(1f, 1f, 1f, num);
			GUI.matrix = Matrix4x4.Scale(new Vector3(num2, num2, 1f));
			float num3 = (float)Screen.width / num2 / 2f;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (Global.onlinePlayerIDs[i] != -1 && Global.onlinePlayerOns[i])
				{
					num4 += readiness[i];
					num5 += 5;
				}
			}
			int num6 = ((num5 != 0) ? Mathf.RoundToInt(1000f * (float)num4 / (float)num5) : 1000);
			float num7 = Time.time - (float)(int)Time.time;
			GUI.DrawTexture(new Rect(num3 - 512f, 894f, 1024f, 32f), barBG);
			GUI.DrawTextureWithTexCoords(new Rect(num3 - 500f, 902f, num6, 16f), barProgress, new Rect(0f, 1f - num7, (float)num6 / 32f, 0.5f));
		}
		if (!fpsEnable || !(Time.timeScale > 0f))
		{
			return;
		}
		GUI.color = Color.black;
		for (int j = 0; j < 2; j++)
		{
			GUI.Label(new Rect(20 - j, Screen.height - 30 - j, 150f, 20f), "FPS: " + fps);
			if (Global.Mode == GameMode.OnlineBattle)
			{
				GUI.Label(new Rect(20 - j, Screen.height - 50 - j, 150f, 20f), "Ping: " + TNManager.ping + ((!TNManager.canUseUDP) ? " ms (TCP)" : " ms (TCP+UDP)"));
			}
			GUI.color = Color.white;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F10))
		{
			fpsEnable = !fpsEnable;
		}
		if (fpsEnable && Time.timeScale > 0f)
		{
			fpsCount += Time.deltaTime;
			if (fpsCount > 1f)
			{
				fpsCount = 0f;
				fps = Mathf.RoundToInt(1f / Time.deltaTime);
			}
		}
		List<GameCharacter> list = GameCharacter.List();
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isConnected)
			{
				for (int i = 0; i < 8; i++)
				{
					if (Global.onlinePlayerIDs[i] == -1 || Global.onlinePlayerOns[i] || iconDC[i])
					{
						continue;
					}
					foreach (GameCharacter item2 in list)
					{
						if (item2.battleID == i)
						{
							GameIcon.AddObject(item2.gameObject, GameIcon.Type.Offline);
							iconDC[i] = true;
							break;
						}
					}
				}
				if (checkReady)
				{
					if (checkTimeOut < 30f)
					{
						checkTimeOut += Time.deltaTime;
						if (checkTimeOut >= 30f)
						{
							TNManager.Disconnect();
						}
					}
					int num = 0;
					int num2 = 0;
					for (int j = 0; j < 8; j++)
					{
						if (Global.onlinePlayerIDs[j] != -1)
						{
							num++;
							if (readiness[j] == 5 || !Global.onlinePlayerOns[j])
							{
								num2++;
							}
						}
					}
					if (num > num2)
					{
						if (!TNManager.isHosting)
						{
							return;
						}
						if (checkSend >= 0.2f)
						{
							checkSend = 0f;
						}
						if (checkSend == 0f)
						{
							int num3 = 10;
							for (int k = 0; k < 8; k++)
							{
								if (Global.onlinePlayerIDs[k] != -1 && Global.onlinePlayerOns[k] && num3 > readiness[k])
								{
									num3 = readiness[k];
								}
							}
							switch (num3)
							{
							case 0:
								GetComponent<TNObject>().Send(19, Target.Others, Global.onlineMatchID, shrinkPatternID, shrinkFlipSign, shrinkBlock, shrinkBlockMax, shrinkDiffTime, shrinkPointX, shrinkPointY, shrinkQointX, shrinkQointY, shrinkBuffer);
								GameMapReadyShrink(Global.onlineMatchID, shrinkPatternID, shrinkFlipSign, shrinkBlock, shrinkBlockMax, shrinkDiffTime, shrinkPointX, shrinkPointY, shrinkQointX, shrinkQointY, shrinkBuffer);
								break;
							case 1:
							{
								int num5 = 0;
								for (int n = 0; n < 11; n++)
								{
									for (int num6 = 0; num6 < 17; num6++)
									{
										if (blockArr[num6][n] != null)
										{
											num5++;
										}
									}
								}
								GetComponent<TNObject>().Send(20, Target.Others, Global.onlineMatchID, num5);
								GameMapReadyAddBlock(Global.onlineMatchID, num5);
								break;
							}
							case 2:
								GetComponent<TNObject>().Send(21, Target.Others, Global.onlineMatchID);
								GameMapReadyCreateCharacter(Global.onlineMatchID);
								break;
							case 3:
							{
								int num4 = 0;
								for (int l = 0; l < 11; l++)
								{
									for (int m = 0; m < 17; m++)
									{
										if (blockArr[m][l] != null)
										{
											num4++;
										}
									}
								}
								int count = GameGround.List().Count;
								GetComponent<TNObject>().Send(22, Target.Others, Global.onlineMatchID, num4, count);
								GameMapReadyCreateGround(Global.onlineMatchID, num4, count);
								break;
							}
							case 4:
								GetComponent<TNObject>().Send(23, Target.Others, Global.onlineMatchID);
								GameMapReadyCheck(Global.onlineMatchID);
								break;
							}
						}
						checkSend += Time.deltaTime;
						return;
					}
					checkReady = false;
					readyGo = true;
					rEffect.GetComponent<Animation>().Play();
				}
			}
			else if (popupDC)
			{
				popupDC = false;
				AdventureGameMode.UI.Disconnect();
			}
		}
		if (readyGo)
		{
			float num7 = rReady.transform.localScale.z - 1f;
			if (num7 < 0f)
			{
				num7 = 0f;
			}
			rGo.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, rGo.transform.localScale.z - 1f));
			rGoBomb.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, rGoBomb.transform.localScale.z - 1f));
			rReady.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, num7));
			rReadyBack.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, num7));
			Vector2 offset = new Vector2(rReadyBack.transform.localScale.z - 2f, 0f);
			rReady.GetComponent<Renderer>().material.SetTextureOffset("_BlendTex", offset);
			if (!rEffect.GetComponent<Animation>().isPlaying)
			{
				foreach (GameCharacter item3 in list)
				{
					item3.enabled = true;
				}
				readyGo = false;
			}
			if (Global.IsBossStage)
			{
				foreach (GameCharacter item4 in list)
				{
					if (item4.Type == Character.Boss)
					{
						Vector3 localPosition = item4.transform.localPosition;
						localPosition.y = 0f;
						item4.transform.localPosition = localPosition;
						break;
					}
				}
			}
		}
		if (remainTime <= 32f && (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle) && Global.mapShrink)
		{
			if (!warning)
			{
				wEffect.GetComponent<Animation>().Blend("Default Take");
				warning = true;
			}
			wWave.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (wWave.transform.localScale.z - 1f) / 2f));
			wTxt.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (wTxt.transform.localScale.z - 1f) / 2f));
		}
		int num8 = 0;
		int num9 = 0;
		int winID = -1;
		for (int num10 = list.Count - 1; num10 >= 0; num10--)
		{
			if (!list[num10].IsDead && !list[num10].IsGhost)
			{
				if (list[num10].Type == Character.Player || list[num10].Type == Character.AI)
				{
					num8++;
				}
				else if (list[num10].Type == Character.Boss || list[num10].Type == Character.Monster)
				{
					num9++;
				}
			}
		}
		if (AdventureGameMode.UI != null)
		{
			if (Global.IsBossStage)
			{
				if (num9 > 0 && !readyGo)
				{
					remainTime += Time.deltaTime;
				}
				AdventureGameMode.UI.ShowTime(remainTime);
			}
			else if (remainTime > 0f)
			{
				if ((Global.Mode != GameMode.OnlineBattle || TNManager.isHosting) && shrinkPatternID != -1 && remainTime <= 30f && num8 > 1)
				{
					Shrink();
				}
				if (num8 == 1 && Global.Mode != GameMode.Adventure)
				{
					Transform transform = fxs.transform;
					for (int num11 = transform.childCount - 1; num11 >= 0; num11--)
					{
						GameGround component = transform.GetChild(num11).GetComponent<GameGround>();
						if (component != null && component.Type == Ground.TimeOut)
						{
							component.enabled = false;
							Animation[] componentsInChildren = component.GetComponentsInChildren<Animation>();
							Animation[] array = componentsInChildren;
							foreach (Animation animation in array)
							{
								animation.Stop();
							}
							ParticleSystem[] componentsInChildren2 = component.GetComponentsInChildren<ParticleSystem>();
							ParticleSystem[] array2 = componentsInChildren2;
							foreach (ParticleSystem particleSystem in array2)
							{
								particleSystem.Stop();
							}
						}
					}
				}
				if (timeRun && !readyGo && (Global.Mode != GameMode.OnlineBattle || (TNManager.isHosting && TNManager.isConnected)))
				{
					float num14 = remainTime;
					remainTime -= Time.deltaTime;
					if (remainTime <= 0f)
					{
						remainTime = 0f;
						Global.advClear = false;
						fadeOut = (((Global.Mode != GameMode.LocalBattle && Global.Mode != GameMode.OnlineBattle) || !Global.mapShrink) ? 0.125f : 4.5f);
						ValidateFadeBG();
						if (Global.Mode == GameMode.OnlineBattle)
						{
							GetComponent<TNObject>().Send(25, Target.Others, Global.onlineMatchID, fadeOut);
						}
					}
					AdventureGameMode.UI.ShowTime(remainTime);
					if (Global.IsVoiceOn && Global.Mode == GameMode.Adventure && num14 > 60f && remainTime <= 60f)
					{
						GameSound.StartVOV("hurry_" + hurryID);
					}
					if (Global.Mode == GameMode.OnlineBattle)
					{
						GetComponent<TNObject>().SendQuickly(26, Target.Others, Global.onlineMatchID, remainTime);
					}
				}
			}
		}
		if (quakeTime > 0f)
		{
			quakeTime -= Time.deltaTime;
			if (quakeTime > 0f)
			{
				float num15 = 0.05f - Mathf.PingPong(Time.time * 1.25f, 0.1f);
				Camera[] allCameras = Camera.allCameras;
				foreach (Camera camera in allCameras)
				{
					camera.fieldOfView = 10f - Mathf.Abs(num15);
					camera.rect = new Rect(0f, num15 / 10f, 1f, 1f);
				}
			}
			else
			{
				Camera[] allCameras2 = Camera.allCameras;
				foreach (Camera camera2 in allCameras2)
				{
					camera2.fieldOfView = 10f;
					camera2.rect = new Rect(0f, 0f, 1f, 1f);
				}
			}
		}
		if (dropItems.Count > 0)
		{
			if (itemList == null)
			{
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					for (int num18 = dropItems.Count - 1; num18 >= 0; num18--)
					{
						Item item = (Item)UnityEngine.Random.Range(0, 13);
						if ((item != Item.Virus || (!(UnityEngine.Random.value < 0.9f) && Global.virus)) && ((item != Item.FireDown && item != Item.BombDown && item != Item.SpeedDown && item != Item.PierceBomb && item != Item.Remote && item != Item.FireFull) || !(UnityEngine.Random.value < 0.75f)))
						{
							CreateItem(item, dropItems[num18]);
						}
					}
				}
			}
			else
			{
				int num19 = 0;
				int num20 = 0;
				for (int num21 = 0; num21 < 13; num21++)
				{
					num19 += itemList[num21];
				}
				if (num19 > 0)
				{
					for (int num22 = 0; num22 < 11; num22++)
					{
						for (int num23 = 0; num23 < 17; num23++)
						{
							if (blockArr[num23][num22] != null)
							{
								num20++;
							}
						}
					}
					for (int num24 = dropItems.Count - 1; num24 >= 0; num24--)
					{
						if (!(UnityEngine.Random.value > (float)num19 / (float)num20))
						{
							float num25 = -1f;
							int num26 = -1;
							for (int num27 = 0; num27 < 13; num27++)
							{
								float num28 = UnityEngine.Random.value * (float)itemList[num27];
								if (num25 < num28)
								{
									num25 = num28;
									num26 = num27;
								}
							}
							itemList[num26]--;
							GameObject gameObject = GameItem.Create((Item)num26);
							gameObject.transform.parent = items.transform;
							gameObject.transform.localPosition = dropItems[num24];
							gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
							gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
							num19--;
						}
					}
				}
			}
			dropItems.Clear();
		}
		if (AdventureGameMode.UI != null && AdventureGameMode.UI.pauseRequest != 0)
		{
			if (fadeOut > 0.125f)
			{
				fadeOut = 0.125f;
			}
			ValidateFadeBG();
		}
		if (fadeOut != float.MaxValue)
		{
			if (fadeOut > 0f)
			{
				fadeOut -= Time.deltaTime;
				if (fadeOut <= 0f)
				{
					fadeOut = 0f;
				}
				ValidateFadeBG();
				if (!(fadeOut <= 3f))
				{
					return;
				}
				if (Global.advClear && Global.Mode == GameMode.Adventure)
				{
					for (int num29 = list.Count - 1; num29 >= 0; num29--)
					{
						if (list[num29].Type == Character.Player && list[num29].IsDead)
						{
							Global.advClear = false;
							break;
						}
					}
					if (Global.advClear)
					{
						DisableAll();
					}
				}
				if (fadeOut != 0f)
				{
					return;
				}
				switch (Global.Mode)
				{
				case GameMode.DebugMode:
					Application.LoadLevel(Application.loadedLevel);
					break;
				case GameMode.Adventure:
					if (AdventureGameMode.UI != null && AdventureGameMode.UI.pauseRequest == -1)
					{
						Application.LoadLevel("Adventure.SelectStage");
					}
					else if (AdventureGameMode.UI != null && AdventureGameMode.UI.pauseRequest == 1)
					{
						Application.LoadLevel("Adventure.GameMode");
					}
					else if (Global.advStage == 0)
					{
						Application.LoadLevel("Adventure.SelectStage");
					}
					else if (Global.advClear && Global.advStage == 30)
					{
						Application.LoadLevel("Adventure.Ending");
					}
					else
					{
						Application.LoadLevel("Adventure.Result");
					}
					break;
				case GameMode.LocalBattle:
					if (AdventureGameMode.UI.pauseRequest == -1)
					{
						Application.LoadLevel("LocalBattle.Menu");
						break;
					}
					winID = -1;
					if (num8 == 1)
					{
						for (int num31 = list.Count - 1; num31 >= 0; num31--)
						{
							if (!list[num31].IsDead && !list[num31].IsGhost && (list[num31].Type == Character.Player || list[num31].Type == Character.AI))
							{
								winID = list[num31].battleID;
								MonoBehaviour.print("we have winner and it is : " + winID);
							}
						}
					}
					MonoBehaviour.print("winner id and player num is : " + winID + " : " + num8);
					Global.winID = winID;
					Application.LoadLevel("LocalBattle.Summary");
					break;
				case GameMode.OnlineBattle:
					if (!TNManager.isConnected)
					{
						if (popupDC)
						{
							GameObject gameObject2 = GameObject.Find("Online.ServerList");
							if (gameObject2 != null)
							{
								gameObject2.GetComponent<OnlineBattleServerList>().PopUpMessage("You have been disconnected\nfrom the server.");
							}
						}
						Application.LoadLevel("OnlineBattle.ServerList");
					}
					else if (AdventureGameMode.UI.pauseRequest == -1)
					{
						Application.LoadLevel("OnlineBattle.ServerList");
					}
					else
					{
						if (!TNManager.isHosting)
						{
							break;
						}
						winID = -1;
						if (num8 == 1)
						{
							for (int num30 = list.Count - 1; num30 >= 0; num30--)
							{
								if (!list[num30].IsDead && !list[num30].IsGhost && (list[num30].Type == Character.Player || list[num30].Type == Character.AI))
								{
									winID = list[num30].battleID;
									MonoBehaviour.print("we have winner and it is : " + winID);
								}
							}
						}
						MonoBehaviour.print("winner id and player num is : " + winID + " : " + num8);
						GetComponent<TNObject>().Send(28, Target.All, Global.onlineMatchID, winID);
					}
					break;
				default:
					Loading.LoadScene("MainMenu");
					break;
				}
			}
			else if (fadeOut < 0f)
			{
				fadeOut += Time.deltaTime;
				if (fadeOut >= 0f)
				{
					fadeOut = float.MaxValue;
				}
				ValidateFadeBG();
			}
			return;
		}
		if (Global.Mode == GameMode.Adventure && Global.advStage == 0)
		{
			GameController controller = GameInput.GetController(Global.ctrlID);
			if (controller.IsConnected() && controller.DoSelectMenu() && fadeOut > 0.125f)
			{
				fadeOut = 0.125f;
			}
		}
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			return;
		}
		if ((num9 >= 0 && num8 == 0) || (num9 == 0 && num8 == 1))
		{
			willEnd += Time.deltaTime;
			float num32 = ((Global.Mode != GameMode.Adventure) ? 4f : 1.5f);
			if (!(willEnd > num32))
			{
				return;
			}
			if (Global.Mode == GameMode.Adventure)
			{
				if (Global.advStage == 0)
				{
					for (int num33 = list.Count - 1; num33 >= 0; num33--)
					{
						if (list[num33].Type == Character.Monster && !list[num33].enabled)
						{
							fadeOut = 0.75f;
							break;
						}
					}
				}
				else if (Global.IsBossStage && num8 > 0)
				{
					Global.advRemainTime = remainTime;
					Global.advClear = true;
					fadeOut = 7.5f;
				}
				else
				{
					Global.advRemainTime = remainTime;
					Global.advClear = num8 > 0 && num9 == 0;
					fadeOut = 3f;
					timeRun = false;
				}
			}
			else
			{
				if (num8 == 1)
				{
					Global.winID = winID;
				}
				else
				{
					Global.winID = -1;
				}
				fadeOut = 0.5f;
				DisableAll(false);
				if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
				{
					GetComponent<TNObject>().Send(25, Target.Others, Global.onlineMatchID, fadeOut);
				}
			}
		}
		else
		{
			willEnd = 0f;
		}
	}

	private void DisableAll(bool doWin = true)
	{
		bool flag = false;
		List<GameCharacter> list = GameCharacter.List();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].Type == Character.Player && list[num].enabled)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if (list[num2].Type == Character.Player)
			{
				list[num2].enabled = false;
				if (doWin)
				{
					Transform child = list[num2].transform.Find("root").GetChild(0);
					child.localRotation = Quaternion.Euler(0f, 180f, 0f);
					child.GetComponent<Animation>().CrossFade("win");
				}
				break;
			}
		}
		List<GameBomb> list2 = GameBomb.List();
		for (int num3 = list2.Count - 1; num3 >= 0; num3--)
		{
			if (list2[num3].Type < Bomb.MAX)
			{
				list2[num3].enabled = false;
			}
		}
		List<GameItem> list3 = GameItem.List();
		for (int num4 = list3.Count - 1; num4 >= 0; num4--)
		{
			list3[num4].enabled = false;
		}
		List<GameGround> list4 = GameGround.List();
		for (int num5 = list4.Count - 1; num5 >= 0; num5--)
		{
			if (list4[num5].Type == Ground.Explode)
			{
				Vector3 localScale = list4[num5].transform.localScale;
				localScale.y = 0f;
				list4[num5].transform.localScale = localScale;
			}
		}
	}

	public List<GameObject> GetObjects(float x, float y, float radius)
	{
		if (x < -8f || x > 8f || y < -5f || y > 5f)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		int num = Mathf.FloorToInt(x) + 8;
		int num2 = Mathf.FloorToInt(y) + 5;
		int num3 = Mathf.CeilToInt(x) + 8;
		int num4 = Mathf.CeilToInt(y) + 5;
		if (num == num3)
		{
			if (num2 == num4)
			{
				if (pillarArr[num][num2] != null)
				{
					list.Add(pillarArr[num][num2]);
				}
				else if (blockArr[num][num2] != null)
				{
					list.Add(blockArr[num][num2]);
				}
			}
			else
			{
				if (pillarArr[num][num2] != null)
				{
					list.Add(pillarArr[num][num2]);
				}
				else if (blockArr[num][num2] != null)
				{
					list.Add(blockArr[num][num2]);
				}
				if (pillarArr[num][num4] != null)
				{
					list.Add(pillarArr[num][num4]);
				}
				else if (blockArr[num][num4] != null)
				{
					list.Add(blockArr[num][num4]);
				}
			}
		}
		else if (num2 == num4)
		{
			if (pillarArr[num][num2] != null)
			{
				list.Add(pillarArr[num][num2]);
			}
			else if (blockArr[num][num2] != null)
			{
				list.Add(blockArr[num][num2]);
			}
			if (pillarArr[num3][num2] != null)
			{
				list.Add(pillarArr[num3][num2]);
			}
			else if (blockArr[num3][num2] != null)
			{
				list.Add(blockArr[num3][num2]);
			}
		}
		else
		{
			if (pillarArr[num][num2] != null)
			{
				list.Add(pillarArr[num][num2]);
			}
			else if (blockArr[num][num2] != null)
			{
				list.Add(blockArr[num][num2]);
			}
			if (pillarArr[num][num4] != null)
			{
				list.Add(pillarArr[num][num4]);
			}
			else if (blockArr[num][num4] != null)
			{
				list.Add(blockArr[num][num4]);
			}
			if (pillarArr[num3][num2] != null)
			{
				list.Add(pillarArr[num3][num2]);
			}
			else if (blockArr[num3][num2] != null)
			{
				list.Add(blockArr[num3][num2]);
			}
			if (pillarArr[num3][num4] != null)
			{
				list.Add(pillarArr[num3][num4]);
			}
			else if (blockArr[num3][num4] != null)
			{
				list.Add(blockArr[num3][num4]);
			}
		}
		List<GameBomb> list2 = GameBomb.List();
		float num5 = radius + 0.5f;
		for (int num6 = list2.Count - 1; num6 >= 0; num6--)
		{
			Vector3 localPosition = list2[num6].transform.localPosition;
			if (!list2[num6].IsExploded && !list2[num6].IsHeld && localPosition.y == 0f && Mathf.Abs(localPosition.x - x) < num5 && Mathf.Abs(localPosition.z - y) < num5)
			{
				list.Add(list2[num6].gameObject);
			}
		}
		List<GameItem> list3 = GameItem.List();
		num5 = radius + ((num != num3 || num2 != num4) ? 0.5f : 0.25f);
		for (int num7 = list3.Count - 1; num7 >= 0; num7--)
		{
			Vector3 localPosition2 = list3[num7].transform.localPosition;
			if (!list3[num7].IsDeactived && localPosition2.y == 0f && Mathf.Abs(localPosition2.x - x) < num5 && Mathf.Abs(localPosition2.z - y) < num5)
			{
				list.Add(list3[num7].gameObject);
			}
		}
		List<GameCharacter> list4 = GameCharacter.List();
		num5 = radius + 0.5f;
		for (int num8 = list4.Count - 1; num8 >= 0; num8--)
		{
			Vector3 localPosition3 = list4[num8].transform.localPosition;
			if (!list4[num8].IsDead && Mathf.Abs(localPosition3.x - x) < num5 && Mathf.Abs(localPosition3.z - y) < num5)
			{
				list.Add(list4[num8].gameObject);
			}
		}
		return list;
	}

	public bool IsOnFire(int x, int y)
	{
		Transform transform = explosions.transform;
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			Vector3 localPosition = transform.GetChild(num).localPosition;
			if (localPosition.x == (float)x && localPosition.z == (float)y)
			{
				return true;
			}
		}
		return false;
	}

	public int CountBomb(GameCharacter owner, bool byGhost = false)
	{
		int num = 0;
		List<GameBomb> list = GameBomb.List();
		if (byGhost)
		{
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				if (list[num2].Owner == owner && !list[num2].IsHeld && list[num2].Type != Bomb.Remote)
				{
					num++;
				}
			}
		}
		else
		{
			for (int num3 = list.Count - 1; num3 >= 0; num3--)
			{
				if (list[num3].Owner == owner && !list[num3].IsHeld)
				{
					num++;
				}
			}
		}
		return num;
	}

	[RFC(10)]
	private void GameMapAddBomb(int matchID, int x, int y, int type, int fire, int ownerID, int[] ignoreIDs, int tnObjID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		GameCharacter owner = null;
		List<GameCharacter> list = new List<GameCharacter>();
		List<GameCharacter> list2 = GameCharacter.List();
		for (int num = list2.Count - 1; num >= 0; num--)
		{
			if (ownerID == list2[num].battleID)
			{
				owner = list2[num];
				break;
			}
		}
		for (int i = 0; i < ignoreIDs.Length; i++)
		{
			for (int num2 = list2.Count - 1; num2 >= 0; num2--)
			{
				if (list2[num2].battleID == ignoreIDs[i])
				{
					list.Add(list2[num2]);
				}
			}
		}
		GameObject gameObject = GameBomb.Create((Bomb)type, (Fire)fire, owner, list);
		gameObject.transform.parent = bombs.transform;
		gameObject.transform.localPosition = new Vector3(x, 0f, y);
		gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
		if (Global.Mode == GameMode.OnlineBattle)
		{
			AddTNObject(gameObject, tnObjID);
		}
	}

	public GameObject AddBomb(GameCharacter owner, Bomb type, Fire fire, bool checkAdd = true)
	{
		List<GameCharacter> list = new List<GameCharacter>();
		Vector3 localPosition = owner.transform.localPosition;
		int num = Mathf.RoundToInt(localPosition.x);
		int num2 = Mathf.RoundToInt(localPosition.z);
		bool flag = false;
		if (checkAdd)
		{
			localPosition.x = num;
			localPosition.z = num2;
			List<GameObject> objects = GetObjects(num, num2, 0.5f);
			for (int num3 = objects.Count - 1; num3 >= 0; num3--)
			{
				GameCharacter component = objects[num3].GetComponent<GameCharacter>();
				if (component != null)
				{
					if (component.Type == Character.Minion)
					{
						objects.RemoveAt(num3);
					}
					else
					{
						list.Add(component);
						objects.RemoveAt(num3);
					}
				}
			}
			if (objects.Count == 0)
			{
				List<GameCharacter> list2 = GameCharacter.List();
				for (int num4 = list2.Count - 1; num4 >= 0; num4--)
				{
					if (!list2[num4].IsDead && list2[num4].HasItem(Item.BombPass) && !list.Contains(list2[num4]))
					{
						list.Add(list2[num4]);
					}
				}
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			GameObject gameObject = GameBomb.Create(type, fire, owner, list);
			gameObject.transform.parent = bombs.transform;
			gameObject.transform.localPosition = new Vector3(num, 0f, num2);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				int[] array = new int[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = list[i].battleID;
				}
				GetComponent<TNObject>().Send(10, Target.Others, Global.onlineMatchID, num, num2, (int)type, (int)fire, owner.battleID, array, tnObjRunID);
				AddTNObject(gameObject, tnObjRunID++);
			}
			return gameObject;
		}
		return null;
	}

	public GameBomb PickBomb(Vector3 at, float range = 0.5f)
	{
		float x = at.x;
		float z = at.z;
		if (x < -8f || x > 8f || z < -5f || z > 5f)
		{
			return null;
		}
		List<GameBomb> list = GameBomb.List();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].Type <= Bomb.MAX)
			{
				Vector3 localPosition = list[num].transform.localPosition;
				if (localPosition.y == 0f && Mathf.Abs(localPosition.x - x) < range && Mathf.Abs(localPosition.z - z) < range)
				{
					return list[num];
				}
			}
		}
		return null;
	}

	[RFC(11)]
	private void GameMapActBomb(int matchID, int bombID, float x, float y, float z)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		foreach (GameBomb item in GameBomb.List())
		{
			if (item.GetComponent<TNObject>().uid == (uint)bombID)
			{
				if (item.transform.parent != bombs.transform)
				{
					item.transform.parent = bombs.transform;
				}
				Debug.Log("Act bomb " + x + "," + y);
				item.transform.localPosition = new Vector3(x, z, y);
				item.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				item.transform.localScale = new Vector3(10f, 10f, 10f);
				item.GetComponent<Collider>().enabled = true;
				break;
			}
		}
	}

	[RFC(12)]
	private void GameMapAddAndFlyBomb(int matchID, int battleID, Vector3 p, Vector3 shootPos)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		GameCharacter owner = null;
		foreach (GameCharacter item in GameCharacter.List())
		{
			if (item.battleID == battleID)
			{
				owner = item;
				break;
			}
		}
		GameObject gameObject = AddBomb(owner, Bomb.Normal, Fire.Lv1, false);
		if (p.x > 9.5f)
		{
			p.x = 9.5f;
		}
		else if (p.x < -9.5f)
		{
			p.x = -9.5f;
		}
		if (p.z > 6.5f)
		{
			p.z = 6.5f;
		}
		else if (p.z < -6.5f)
		{
			p.z = -6.5f;
		}
		FlyBomb(gameObject.GetComponent<GameBomb>(), p.x, p.z, shootPos);
	}

	public void FlyBomb(GameBomb bomb, float x, float y, Vector3 target)
	{
		if (bomb.transform.parent != bombs.transform)
		{
			bomb.transform.parent = bombs.transform;
		}
		bomb.transform.localPosition = new Vector3(x, 1f, y);
		bomb.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		bomb.transform.localScale = new Vector3(10f, 10f, 10f);
		bomb.GetComponent<Collider>().enabled = true;
		bomb.transform.Find("root").GetComponent<Collider>().enabled = true;
		bomb.Fly(target);
	}

	public void FlyBomb(GameBomb bomb, int x, int y, Direction direct, int range)
	{
		if (bomb.transform.parent != bombs.transform)
		{
			bomb.transform.parent = bombs.transform;
		}
		bomb.transform.localPosition = new Vector3(x, 1f, y);
		bomb.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		bomb.transform.localScale = new Vector3(10f, 10f, 10f);
		bomb.GetComponent<Collider>().enabled = true;
		bomb.transform.Find("root").GetComponent<Collider>().enabled = true;
		bomb.Fly(direct, range);
	}

	public void ShootBomb(GameBomb bomb, int x, int y, Direction direct)
	{
		if (bomb.transform.parent != bombs.transform)
		{
			bomb.transform.parent = bombs.transform;
		}
		bomb.transform.localPosition = new Vector3(x, 0f, y);
		bomb.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		bomb.transform.localScale = new Vector3(10f, 10f, 10f);
		bomb.GetComponent<Collider>().enabled = true;
		bomb.transform.Find("root").GetComponent<Collider>().enabled = true;
		bomb.Shoot(direct);
	}

	public void AddItem(List<Item> list)
	{
		bool[][] array = new bool[17][];
		for (int i = 0; i < 17; i++)
		{
			array[i] = new bool[11];
		}
		for (int j = 0; j < 11; j++)
		{
			for (int k = 0; k < 17; k++)
			{
				if (pillarArr[k][j] != null)
				{
					array[k][j] = true;
				}
			}
		}
		for (int l = 0; l < 11; l++)
		{
			for (int m = 0; m < 17; m++)
			{
				if (blockArr[m][l] != null)
				{
					array[m][l] = true;
				}
			}
		}
		List<GameBomb> list2 = GameBomb.List();
		for (int num = list2.Count - 1; num >= 0; num--)
		{
			Vector3 localPosition = list2[num].transform.localPosition;
			int num2 = Mathf.RoundToInt(localPosition.x - 0.25f) + 8;
			int num3 = Mathf.RoundToInt(localPosition.z - 0.25f) + 5;
			int num4 = Mathf.RoundToInt(localPosition.x + 0.25f) + 8;
			int num5 = Mathf.RoundToInt(localPosition.z + 0.25f) + 5;
			if (num2 >= 0 && num2 < 17)
			{
				if (num3 >= 0 && num3 < 11)
				{
					array[num2][num3] = true;
				}
				if (num5 >= 0 && num5 < 11)
				{
					array[num2][num5] = true;
				}
			}
			if (num4 >= 0 && num4 < 17)
			{
				if (num3 >= 0 && num3 < 11)
				{
					array[num4][num3] = true;
				}
				if (num5 >= 0 && num5 < 11)
				{
					array[num4][num5] = true;
				}
			}
		}
		List<GameItem> list3 = GameItem.List();
		for (int num6 = list3.Count - 1; num6 >= 0; num6--)
		{
			Vector3 localPosition2 = list3[num6].transform.localPosition;
			if (localPosition2.y == 0f)
			{
				array[(int)localPosition2.x + 8][(int)localPosition2.z + 5] = true;
			}
		}
		List<GameCharacter> list4 = GameCharacter.List();
		for (int num7 = list4.Count - 1; num7 >= 0; num7--)
		{
			if (!list4[num7].IsDead)
			{
				Vector3 localPosition3 = list4[num7].transform.localPosition;
				int num8 = Mathf.RoundToInt(localPosition3.x - 0.25f) + 8;
				int num9 = Mathf.RoundToInt(localPosition3.z - 0.25f) + 5;
				int num10 = Mathf.RoundToInt(localPosition3.x + 0.25f) + 8;
				int num11 = Mathf.RoundToInt(localPosition3.z + 0.25f) + 5;
				if (num8 >= 0 && num8 < 17)
				{
					if (num9 >= 0 && num9 < 11)
					{
						array[num8][num9] = true;
					}
					if (num11 >= 0 && num11 < 11)
					{
						array[num8][num11] = true;
					}
				}
				if (num10 >= 0 && num10 < 17)
				{
					if (num9 >= 0 && num9 < 11)
					{
						array[num10][num9] = true;
					}
					if (num11 >= 0 && num11 < 11)
					{
						array[num10][num11] = true;
					}
				}
			}
		}
		Transform transform = explosions.transform;
		for (int num12 = transform.childCount - 1; num12 >= 0; num12--)
		{
			Vector3 localPosition4 = transform.GetChild(num12).localPosition;
			int num13 = Mathf.RoundToInt(localPosition4.x) + 8;
			int num14 = Mathf.RoundToInt(localPosition4.z) + 5;
			array[num13][num14] = true;
		}
		List<Vector3> list5 = new List<Vector3>();
		for (int n = 0; n < 11; n++)
		{
			for (int num15 = 0; num15 < 17; num15++)
			{
				if (!array[num15][n])
				{
					list5.Add(new Vector3(num15 - 8, 0f, n - 5));
				}
			}
		}
		int num16 = list.Count - 1;
		while (num16 >= 0 && list5.Count != 0)
		{
			int num17 = UnityEngine.Random.Range(0, list5.Count);
			CreateItem(list[num16], list5[num17]);
			list5.RemoveAt(num17);
			num16--;
		}
	}

	public bool IsBlock(int x, int y)
	{
		int num = x + 8;
		int num2 = y + 5;
		if (num < 0 || num >= 17 || num2 < 0 || num2 >= 11)
		{
			return false;
		}
		return blockArr[num][num2] != null;
	}

	[RFC(13)]
	private void GameMapAddBlock(int matchID, int x, int y)
	{
		if (Global.onlineMatchID == matchID)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(blockRes) as GameObject;
			gameObject.name = "Block";
			gameObject.transform.parent = blocks.transform;
			gameObject.transform.localPosition = new Vector3(x, 0f, y);
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3(0f, 0.05f, 0f);
			boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
			blockArr[x + 8][y + 5] = gameObject;
		}
	}

	public bool AddBlock(int x, int y)
	{
		int num = x + 8;
		int num2 = y + 5;
		if (num < 0 || num >= 17 || num2 < 0 || num2 >= 11)
		{
			return false;
		}
		if (blockArr[num][num2] != null)
		{
			return false;
		}
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(13, Target.Others, Global.onlineMatchID, x, y);
				GameMapAddBlock(Global.onlineMatchID, x, y);
			}
		}
		else
		{
			GameMapAddBlock(Global.onlineMatchID, x, y);
		}
		return true;
	}

	[RFC(14)]
	private void GameMapRemoveBlock(int matchID, int x, int y)
	{
		if (Global.onlineMatchID == matchID)
		{
			int num = x + 8;
			int num2 = y + 5;
			UnityEngine.Object.Destroy(blockArr[num][num2]);
			blockArr[num][num2] = null;
		}
	}

	public bool RemoveBlock(int x, int y)
	{
		int num = x + 8;
		int num2 = y + 5;
		if (num < 0 || num >= 17 || num2 < 0 || num2 >= 11)
		{
			return false;
		}
		if (blockArr[num][num2] != null)
		{
			if (Global.Mode == GameMode.OnlineBattle)
			{
				if (TNManager.isHosting)
				{
					GetComponent<TNObject>().Send(14, Target.Others, Global.onlineMatchID, x, y);
					GameMapRemoveBlock(Global.onlineMatchID, x, y);
				}
			}
			else
			{
				GameMapRemoveBlock(Global.onlineMatchID, x, y);
			}
			return true;
		}
		return false;
	}

	public void Explode(int battleID, int x, int y, int radius, bool pierce = false, Direction exclude = Direction.None, GameObject specialFX = null, float fxScale = 1f, float fxDuration = 1f, float fireDuration = 0.1f)
	{
		bool flag = specialFX != null;
		GameObject specialFX2 = ((!flag) ? bombFX : specialFX);
		if (radius < 0 && blockArr[x + 8][y + 5] != null)
		{
			AddExplosionFX((float)x - 0.5f, y, Direction.Left, specialFX2, fxScale, fxDuration);
			AddExplosionFX(x, (float)y - 0.5f, Direction.Down, specialFX2, fxScale, fxDuration);
			AddExplosionFX((float)x + 0.5f, y, Direction.Right, specialFX2, fxScale, fxDuration);
			AddExplosionFX(x, (float)y + 0.5f, Direction.Up, specialFX2, fxScale, fxDuration);
		}
		int num = DoExplode(battleID, x, y, Direction.None, flag);
		if (num == 0 || (num == 1 && pierce))
		{
			AddExplosionCollider(battleID, x, y, fireDuration, flag);
		}
		if (radius < 0)
		{
			return;
		}
		if (radius > 0)
		{
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			if (exclude != Direction.Left)
			{
				for (int i = 1; i <= radius; AddExplosionCollider(battleID, x - i, y, fireDuration, flag), i++)
				{
					num = DoExplode(battleID, x - i, y, Direction.Left, flag);
					if (num != 2)
					{
						AddExplosionFX(x - i, y, Direction.Left, specialFX2, fxScale, fxDuration);
						flag2 = true;
					}
					switch (num)
					{
					case 1:
						if (pierce)
						{
							continue;
						}
						break;
					default:
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			if (exclude != Direction.Down)
			{
				for (int j = 1; j <= radius; AddExplosionCollider(battleID, x, y - j, fireDuration, flag), j++)
				{
					num = DoExplode(battleID, x, y - j, Direction.Down, flag);
					if (num != 2)
					{
						AddExplosionFX(x, y - j, Direction.Down, specialFX2, fxScale, fxDuration);
						flag3 = true;
					}
					switch (num)
					{
					case 1:
						if (pierce)
						{
							continue;
						}
						break;
					default:
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			if (exclude != Direction.Right)
			{
				for (int k = 1; k <= radius; AddExplosionCollider(battleID, x + k, y, fireDuration, flag), k++)
				{
					num = DoExplode(battleID, x + k, y, Direction.Right, flag);
					if (num != 2)
					{
						AddExplosionFX(x + k, y, Direction.Right, specialFX2, fxScale, fxDuration);
						flag4 = true;
					}
					switch (num)
					{
					case 1:
						if (pierce)
						{
							continue;
						}
						break;
					default:
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			if (exclude != Direction.Up)
			{
				for (int l = 1; l <= radius; AddExplosionCollider(battleID, x, y + l, fireDuration, flag), l++)
				{
					num = DoExplode(battleID, x, y + l, Direction.Up, flag);
					if (num != 2)
					{
						AddExplosionFX(x, y + l, Direction.Up, specialFX2, fxScale, fxDuration);
						flag5 = true;
					}
					switch (num)
					{
					case 1:
						if (pierce)
						{
							continue;
						}
						break;
					default:
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			if (flag2)
			{
				AddExplosionFX((float)x - 0.5f, y, Direction.Left, specialFX2, fxScale, fxDuration);
			}
			if (flag3)
			{
				AddExplosionFX(x, (float)y - 0.5f, Direction.Down, specialFX2, fxScale, fxDuration);
			}
			if (flag4)
			{
				AddExplosionFX((float)x + 0.5f, y, Direction.Right, specialFX2, fxScale, fxDuration);
			}
			if (flag5)
			{
				AddExplosionFX(x, (float)y + 0.5f, Direction.Up, specialFX2, fxScale, fxDuration);
			}
		}
		else if (flag)
		{
			if (num != 2)
			{
				AddExplosionFX(x, y, Direction.None, specialFX2, fxScale, fxDuration);
			}
		}
		else if (num != 2)
		{
			AddExplosionFX((float)x - 0.5f, y, Direction.Left, specialFX2, fxScale, fxDuration);
			AddExplosionFX(x, (float)y - 0.5f, Direction.Down, specialFX2, fxScale, fxDuration);
			AddExplosionFX((float)x + 0.5f, y, Direction.Right, specialFX2, fxScale, fxDuration);
			AddExplosionFX(x, (float)y + 0.5f, Direction.Up, specialFX2, fxScale, fxDuration);
		}
	}

	private int DoExplode(int battleID, int x, int y, Direction dir, bool specialAttack)
	{
		if (x < -8 || x > 8 || y < -5 || y > 5)
		{
			return 2;
		}
		int num = x + 8;
		int num2 = y + 5;
		if (pillarArr[num][num2] != null)
		{
			return 2;
		}
		if (blockArr[num][num2] != null)
		{
			dropItems.Add(new Vector3(x, 0f, y));
			UnityEngine.Object.Destroy(blockArr[num][num2]);
			blockArr[num][num2] = null;
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(14, Target.Others, Global.onlineMatchID, x, y);
			}
			return 1;
		}
		List<GameBomb> list = GameBomb.List();
		float num3 = 0.75f;
		for (int num4 = list.Count - 1; num4 >= 0; num4--)
		{
			Vector3 localPosition = list[num4].transform.localPosition;
			if (localPosition.y == 0f && Mathf.Abs(localPosition.x - (float)x) < num3 && Mathf.Abs(localPosition.z - (float)y) < num3 && list[num4].Explode(dir))
			{
				return 1;
			}
		}
		List<GameItem> list2 = GameItem.List();
		num3 = 0.5f;
		for (int num5 = list2.Count - 1; num5 >= 0; num5--)
		{
			Vector3 localPosition2 = list2[num5].transform.localPosition;
			if (localPosition2.y == 0f && Mathf.Abs(localPosition2.x - (float)x) < num3 && Mathf.Abs(localPosition2.z - (float)y) < num3)
			{
				if (list2[num5].Type == Item.Virus || !GameItem.Destroyable)
				{
					list2[num5].Fly(dir, 4);
				}
				else
				{
					list2[num5].Deactive();
				}
				return 1;
			}
		}
		List<GameCharacter> list3 = GameCharacter.List();
		num3 = 0.75f;
		for (int num6 = list3.Count - 1; num6 >= 0; num6--)
		{
			Vector3 localPosition3 = list3[num6].transform.localPosition;
			float num7 = ((list3[num6].Type != Character.Player && list3[num6].Type != Character.AI) ? num3 : (num3 * 0.75f));
			if (localPosition3.y == 0f && Mathf.Abs(localPosition3.x - (float)x) < num7 && Mathf.Abs(localPosition3.z - (float)y) < num7 && (list3[num6].Type != Character.Boss || !specialAttack))
			{
				list3[num6].Dead(battleID);
			}
		}
		return 0;
	}

	[RFC(15)]
	private void GameMapAddExplosionFX(int matchID, float x, float y, int dir)
	{
		if (Global.onlineMatchID == matchID)
		{
			AddExplosionFX(x, y, (Direction)dir, bombFX, 1f, 1f);
		}
	}

	private void AddExplosionFX(float x, float y, Direction dir, GameObject specialFX, float scale, float duration)
	{
		scale *= 10f;
		GameObject gameObject = UnityEngine.Object.Instantiate(specialFX) as GameObject;
		gameObject.transform.parent = fxs.transform;
		gameObject.transform.localPosition = new Vector3(x, 0.5f, y);
		gameObject.transform.localRotation = Quaternion.Euler(0f, (float)dir, 0f);
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
		UnityEngine.Object.Destroy(gameObject, duration);
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			GetComponent<TNObject>().SendQuickly(15, Target.Others, Global.onlineMatchID, x, y, (int)dir);
		}
	}

	private void AddExplosionCollider(int battleID, int x, int y, float duration, bool specialAttack)
	{
		GameObject gameObject = new GameObject((!specialAttack) ? "Explosion" : "ExplosionX");
		gameObject.transform.parent = explosions.transform;
		gameObject.transform.localPosition = new Vector3(x, 0f, y);
		gameObject.transform.localRotation = Quaternion.Euler(0f, battleID * 10, 0f);
		gameObject.transform.localScale = Vector3.one;
		SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.center = new Vector3(0f, 0.25f, 0f);
		sphereCollider.radius = 0.25f;
		UnityEngine.Object.Destroy(gameObject, duration);
	}

	public GameObject AddFX(int x, int y, GameObject resFX, float duration = 0f, float scale = 1f)
	{
		scale *= 10f;
		GameObject gameObject = UnityEngine.Object.Instantiate(resFX) as GameObject;
		gameObject.transform.parent = fxs.transform;
		gameObject.transform.localPosition = new Vector3(x, 0f, y);
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
		if (duration > 0f)
		{
			UnityEngine.Object.Destroy(gameObject, duration);
		}
		return gameObject;
	}

	public void RemoveFX(GameObject fx)
	{
		UnityEngine.Object.Destroy(fx);
	}

	public void Quake(float duration)
	{
		quakeTime = duration;
	}

	public void IgnoreMapCollision(GameObject obj)
	{
		for (int num = bg.transform.childCount - 1; num >= 0; num--)
		{
			Physics.IgnoreCollision(obj.GetComponent<Collider>(), bg.transform.GetChild(num).GetComponent<Collider>());
		}
		for (int i = 0; i < 11; i++)
		{
			for (int j = 0; j < 17; j++)
			{
				if (pillarArr[j][i] != null)
				{
					Physics.IgnoreCollision(obj.GetComponent<Collider>(), pillarArr[j][i].GetComponent<Collider>());
				}
			}
		}
	}

	[RFC(27)]
	private void GameMapUpdateShrink(int matchID, int shrinkPatternID, int shrinkFlipSign, int shrinkBlock, int shrinkBlockMax, float shrinkDiffTime, int shrinkPointX, int shrinkPointY, int shrinkQointX, int shrinkQointY, Vector2[] shrinkBuffer)
	{
		if (Global.onlineMatchID == matchID)
		{
			this.shrinkPatternID = shrinkPatternID;
			this.shrinkFlipSign = shrinkFlipSign;
			this.shrinkBlock = shrinkBlock;
			this.shrinkBlockMax = shrinkBlockMax;
			this.shrinkDiffTime = shrinkDiffTime;
			this.shrinkPointX = shrinkPointX;
			this.shrinkPointY = shrinkPointY;
			this.shrinkQointX = shrinkQointX;
			this.shrinkQointY = shrinkQointY;
			this.shrinkBuffer = shrinkBuffer;
		}
	}

	private void Shrink()
	{
		int num = (int)((28f - (remainTime - 2f)) / shrinkDiffTime) + 1;
		if (num > shrinkBlockMax || shrinkBlock >= num)
		{
			return;
		}
		for (int i = shrinkBlock; i < num; i++)
		{
			switch (shrinkPatternID)
			{
			case 0:
				if (pillarArr[shrinkPointX + 8][shrinkPointY + 5] == null)
				{
					CreateGround(Ground.TimeOut, shrinkFlipSign * shrinkPointX, shrinkPointY);
					CreateGround(Ground.TimeOut, shrinkFlipSign * -shrinkPointX, -shrinkPointY);
				}
				if (i < 16)
				{
					shrinkPointX--;
				}
				else if (i < 25)
				{
					shrinkPointY--;
				}
				else if (i < 40)
				{
					shrinkPointX++;
				}
				else if (i < 47)
				{
					shrinkPointY++;
				}
				else if (i < 60)
				{
					shrinkPointX--;
				}
				else if (i < 65)
				{
					shrinkPointY--;
				}
				else if (i < 76)
				{
					shrinkPointX++;
				}
				else if (i < 79)
				{
					shrinkPointY++;
				}
				else if (i < 88)
				{
					shrinkPointX--;
				}
				else if (i < 89)
				{
					shrinkPointY--;
				}
				else
				{
					shrinkPointX++;
				}
				break;
			case 1:
				if (pillarArr[shrinkPointX + 8][shrinkPointY + 5] == null)
				{
					CreateGround(Ground.TimeOut, shrinkFlipSign * shrinkPointX, shrinkPointY);
					CreateGround(Ground.TimeOut, shrinkFlipSign * -shrinkPointX, -shrinkPointY);
				}
				if (i < 10)
				{
					shrinkPointY--;
				}
				else if (i < 25)
				{
					shrinkPointX--;
				}
				else if (i < 34)
				{
					shrinkPointY++;
				}
				else if (i < 47)
				{
					shrinkPointX++;
				}
				else if (i < 54)
				{
					shrinkPointY--;
				}
				else if (i < 65)
				{
					shrinkPointX--;
				}
				else if (i < 70)
				{
					shrinkPointY++;
				}
				else if (i < 79)
				{
					shrinkPointX++;
				}
				else if (i < 82)
				{
					shrinkPointY--;
				}
				else if (i < 89)
				{
					shrinkPointX--;
				}
				else if (i < 90)
				{
					shrinkPointY++;
				}
				else
				{
					shrinkPointX++;
				}
				break;
			case 2:
			{
				if (pillarArr[shrinkPointX + 8][shrinkPointY + 5] == null)
				{
					CreateGround(Ground.TimeOut, shrinkFlipSign * shrinkPointX, shrinkPointY);
					CreateGround(Ground.TimeOut, shrinkFlipSign * -shrinkPointX, -shrinkPointY);
				}
				int num2 = i % 22;
				if (num2 < 10)
				{
					shrinkPointY--;
				}
				else if (num2 < 11)
				{
					shrinkPointX--;
				}
				else if (num2 < 21)
				{
					shrinkPointY++;
				}
				else
				{
					shrinkPointX--;
				}
				break;
			}
			case 3:
				if (pillarArr[shrinkPointX + 8][shrinkPointY + 5] == null)
				{
					CreateGround(Ground.TimeOut, shrinkFlipSign * shrinkPointX, shrinkPointY);
					CreateGround(Ground.TimeOut, shrinkFlipSign * -shrinkPointX, -shrinkPointY);
				}
				if (i < 15)
				{
					shrinkPointX--;
				}
				else if (i < 23)
				{
					shrinkPointY--;
				}
				else if (i < 35)
				{
					shrinkPointX++;
				}
				else if (i < 39)
				{
					shrinkPointY++;
				}
				else if (i < 45)
				{
					shrinkPointX--;
				}
				else if (i < 46)
				{
					shrinkPointY--;
				}
				else
				{
					shrinkPointX++;
				}
				if (pillarArr[shrinkQointX + 8][shrinkQointY + 5] == null)
				{
					CreateGround(Ground.TimeOut, shrinkFlipSign * shrinkQointX, shrinkQointY);
					CreateGround(Ground.TimeOut, shrinkFlipSign * -shrinkQointX, -shrinkQointY);
				}
				if (i < 9)
				{
					shrinkQointY++;
				}
				else if (i < 23)
				{
					shrinkQointX--;
				}
				else if (i < 29)
				{
					shrinkQointY--;
				}
				else if (i < 39)
				{
					shrinkQointX++;
				}
				else if (i < 41)
				{
					shrinkQointY++;
				}
				else if (i < 43)
				{
					shrinkQointX--;
				}
				else if (i < 44)
				{
					shrinkQointY--;
				}
				else
				{
					shrinkQointX++;
				}
				break;
			case 4:
				if (i < 90)
				{
					int num3;
					int num4;
					int num5;
					int num6;
					int num7;
					if (i < 26)
					{
						num3 = i;
						num4 = 2;
						num5 = 52;
						num6 = 8;
						num7 = 5;
					}
					else if (i < 48)
					{
						num3 = i - 26;
						num4 = 1;
						num5 = 22;
						num6 = 7;
						num7 = 4;
					}
					else if (i < 66)
					{
						num3 = i - 48;
						num4 = 2;
						num5 = 36;
						num6 = 6;
						num7 = 3;
					}
					else if (i < 80)
					{
						num3 = i - 66;
						num4 = 1;
						num5 = 14;
						num6 = 5;
						num7 = 2;
					}
					else
					{
						num3 = i - 80;
						num4 = 2;
						num5 = 20;
						num6 = 4;
						num7 = 1;
					}
					if (num3 == 0 && shrinkBuffer.Length != num5)
					{
						shrinkBuffer = new Vector2[num5];
						int num8 = 0;
						int num9 = 2 / num4;
						for (int num10 = num6 + 1 - num9; num10 >= -num6; num10 -= num9)
						{
							shrinkBuffer[num8++] = new Vector2(num10, num7);
							shrinkBuffer[num8++] = new Vector2(-num10, -num7);
						}
						num7--;
						for (int num11 = num7; num11 >= -num7; num11 -= num9)
						{
							shrinkBuffer[num8++] = new Vector2(num6, num11);
							shrinkBuffer[num8++] = new Vector2(-num6, -num11);
						}
						for (int num12 = num5 - 1; num12 >= 0; num12--)
						{
							int num13 = UnityEngine.Random.Range(0, num12 + 1);
							if (num13 != num12)
							{
								Vector2 vector = shrinkBuffer[num12];
								shrinkBuffer[num12] = shrinkBuffer[num13];
								shrinkBuffer[num13] = vector;
							}
						}
					}
					for (int m = 0; m < num4; m++)
					{
						int num14 = num3 * num4 + m;
						CreateGround(Ground.TimeOut, (int)shrinkBuffer[num14].x, (int)shrinkBuffer[num14].y);
					}
				}
				else if (i < 92)
				{
					shrinkPointX = ((!(UnityEngine.Random.value < 0.5f)) ? 2 : (-2));
					foreach (GameGround item in GameGround.List())
					{
						if (item.Type <= Ground.TimeOut)
						{
							Vector3 localPosition = item.transform.localPosition;
							if (Mathf.RoundToInt(localPosition.x) == shrinkPointX && Mathf.RoundToInt(localPosition.z) == 0)
							{
								shrinkPointX *= -1;
								break;
							}
						}
					}
					CreateGround(Ground.TimeOut, shrinkPointX, 0);
				}
				else
				{
					CreateGround(Ground.TimeOut, 0, 0);
				}
				break;
			case 5:
				if (i < 48)
				{
					int num15 = i % 12;
					int num16;
					int num17;
					int num18;
					if (num15 < 6)
					{
						num16 = num15;
						num17 = 2;
						num18 = 11;
					}
					else
					{
						num16 = num15 - 6;
						num17 = 1;
						num18 = 6;
					}
					if (num16 == 0 && shrinkBuffer.Length != num18)
					{
						shrinkBuffer = new Vector2[num18 * 2];
						int num19 = 8 - i / 6;
						int num20 = 5;
						int num21 = 0;
						int num22 = 2 / num17;
						for (int num23 = num20; num23 >= -num20; num23 -= num22)
						{
							shrinkBuffer[num21++] = new Vector2(num19, num23);
						}
						for (int num24 = num20; num24 >= -num20; num24 -= num22)
						{
							shrinkBuffer[num21++] = new Vector2(-num19, -num24);
						}
						for (int n = 0; n < 2; n++)
						{
							for (int num25 = num18 - 1; num25 >= 0; num25--)
							{
								int num26 = num18 * n;
								int num27 = UnityEngine.Random.Range(0, num25 + 1);
								if (num27 != num25)
								{
									Vector2 vector2 = shrinkBuffer[num25 + num26];
									shrinkBuffer[num25 + num26] = shrinkBuffer[num27 + num26];
									shrinkBuffer[num27 + num26] = vector2;
								}
							}
						}
					}
					for (int num28 = 0; num28 < 2; num28++)
					{
						for (int num29 = 0; num29 < num17; num29++)
						{
							int num30 = num16 * num17 + num29;
							if (num30 < num18)
							{
								num30 += num18 * num28;
								CreateGround(Ground.TimeOut, (int)shrinkBuffer[num30].x, (int)shrinkBuffer[num30].y);
							}
						}
					}
				}
				else
				{
					int num31 = i - 48;
					CreateGround(Ground.TimeOut, 0, 5 - num31);
					CreateGround(Ground.TimeOut, 0, -(5 - num31));
				}
				break;
			case 6:
			{
				for (int k = i; k < 17 - i; k++)
				{
					if (pillarArr[k][i] == null)
					{
						CreateGround(Ground.TimeOut, k - 8, i - 5);
						CreateGround(Ground.TimeOut, k - 8, -(i - 5));
					}
				}
				for (int l = i; l < 11 - i; l++)
				{
					if (pillarArr[i][l] == null)
					{
						CreateGround(Ground.TimeOut, i - 8, l - 5);
						CreateGround(Ground.TimeOut, -(i - 8), l - 5);
					}
				}
				break;
			}
			case 7:
			{
				for (int j = 0; j < 11; j++)
				{
					if (pillarArr[i][j] == null)
					{
						CreateGround(Ground.TimeOut, i - 8, j - 5);
						CreateGround(Ground.TimeOut, -(i - 8), j - 5);
					}
				}
				break;
			}
			}
		}
		shrinkBlock = num;
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			GetComponent<TNObject>().Send(27, Target.Others, Global.onlineMatchID, shrinkPatternID, shrinkFlipSign, shrinkBlock, shrinkBlockMax, shrinkDiffTime, shrinkPointX, shrinkPointY, shrinkQointX, shrinkQointY, shrinkBuffer);
		}
	}

	[RFC(16)]
	private void GameMapCreateCharacter(int matchID, int slot, int id, int tnObjID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		GameObject gameObject = GameCharacter.Create(Character.Player, slot, id);
		gameObject.transform.parent = characters.transform;
		gameObject.transform.localPosition = respawnPoints[slot];
		gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
		if (Global.Mode == GameMode.OnlineBattle)
		{
			AddTNObject(gameObject, tnObjID);
			if (slot == Global.onlinePlayerSlot)
			{
				GameIcon.AddObject(gameObject, (GameIcon.Type)slot);
			}
			gameObject.GetComponent<GameCharacter>().enabled = false;
		}
	}

	private void CreateCharacter(int slot, int id)
	{
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(16, Target.Others, Global.onlineMatchID, slot, id, tnObjRunID);
				GameMapCreateCharacter(Global.onlineMatchID, slot, id, tnObjRunID++);
			}
		}
		else
		{
			GameMapCreateCharacter(Global.onlineMatchID, slot, id, 0);
		}
	}

	[RFC(17)]
	private void GameMapCreateItem(int matchID, int type, Vector3 position, int tnObjID)
	{
		if (Global.onlineMatchID == matchID)
		{
			GameObject gameObject = GameItem.Create((Item)type);
			gameObject.transform.parent = items.transform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			if (Global.Mode == GameMode.OnlineBattle)
			{
				AddTNObject(gameObject, tnObjID);
			}
		}
	}

	private void CreateItem(Item type, Vector3 position)
	{
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(17, Target.Others, Global.onlineMatchID, (int)type, position, tnObjRunID);
				GameMapCreateItem(Global.onlineMatchID, (int)type, position, tnObjRunID++);
			}
		}
		else
		{
			GameMapCreateItem(Global.onlineMatchID, (int)type, position, 0);
		}
	}

	[RFC(18)]
	private void GameMapCreateGround(int matchID, int type, int x, int y, int param, int tnObjID)
	{
		if (Global.onlineMatchID == matchID)
		{
			GameObject obj = GameGround.Create((Ground)type, x, y, param);
			if (Global.Mode == GameMode.OnlineBattle)
			{
				AddTNObject(obj, tnObjID);
			}
		}
	}

	public void CreateGround(Ground type, int x, int y, int param = 0)
	{
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(18, Target.Others, Global.onlineMatchID, (int)type, x, y, param, tnObjRunID);
				GameMapCreateGround(Global.onlineMatchID, (int)type, x, y, param, tnObjRunID++);
			}
		}
		else
		{
			GameMapCreateGround(Global.onlineMatchID, (int)type, x, y, param, 0);
		}
	}

	[RFC(19)]
	private void GameMapReadyShrink(int matchID, int shrinkPatternID, int shrinkFlipSign, int shrinkBlock, int shrinkBlockMax, float shrinkDiffTime, int shrinkPointX, int shrinkPointY, int shrinkQointX, int shrinkQointY, Vector2[] shrinkBuffer)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (TNManager.isHosting)
		{
			if (readiness[Global.onlinePlayerSlot] == 0)
			{
				readiness[Global.onlinePlayerSlot] = 1;
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 1);
			return;
		}
		if (readiness[Global.onlinePlayerSlot] == 0)
		{
			readiness[Global.onlinePlayerSlot] = 1;
			this.shrinkPatternID = shrinkPatternID;
			this.shrinkFlipSign = shrinkFlipSign;
			this.shrinkBlock = shrinkBlock;
			this.shrinkBlockMax = shrinkBlockMax;
			this.shrinkDiffTime = shrinkDiffTime;
			this.shrinkPointX = shrinkPointX;
			this.shrinkPointY = shrinkPointY;
			this.shrinkQointX = shrinkQointX;
			this.shrinkQointY = shrinkQointY;
			this.shrinkBuffer = shrinkBuffer;
		}
		GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 1);
	}

	[RFC(20)]
	private void GameMapReadyAddBlock(int matchID, int blockN)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (TNManager.isHosting)
		{
			if (readiness[Global.onlinePlayerSlot] == 1)
			{
				readiness[Global.onlinePlayerSlot] = 2;
				blockN = blockXY.Count;
				for (int i = 0; i < blockN; i += 2)
				{
					AddBlock(blockXY[i], blockXY[i + 1]);
				}
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 2);
			return;
		}
		int num = 0;
		for (int j = 0; j < 11; j++)
		{
			for (int k = 0; k < 17; k++)
			{
				if (blockArr[k][j] != null)
				{
					num++;
				}
			}
		}
		if (num == blockN)
		{
			if (readiness[Global.onlinePlayerSlot] == 1)
			{
				readiness[Global.onlinePlayerSlot] = 2;
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 2);
		}
	}

	[RFC(21)]
	private void GameMapReadyCreateCharacter(int matchID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (TNManager.isHosting)
		{
			if (readiness[Global.onlinePlayerSlot] == 2)
			{
				readiness[Global.onlinePlayerSlot] = 3;
				for (int i = 0; i < 8; i++)
				{
					if (Global.onlinePlayerIDs[i] != -1)
					{
						CreateCharacter(i, Global.onlinePlayerIDs[i]);
					}
				}
				GenerateGroundBattle();
				List<GameCharacter> list = GameCharacter.List();
				for (int num = list.Count - 1; num >= 1; num--)
				{
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						Physics.IgnoreCollision(list[num].GetComponent<Collider>(), list[num2].GetComponent<Collider>());
					}
				}
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 3);
			return;
		}
		int num3 = 0;
		for (int j = 0; j < 8; j++)
		{
			if (Global.onlinePlayerIDs[j] != -1)
			{
				num3++;
			}
		}
		List<GameCharacter> list2 = GameCharacter.List();
		if (num3 != list2.Count)
		{
			return;
		}
		if (readiness[Global.onlinePlayerSlot] == 2)
		{
			readiness[Global.onlinePlayerSlot] = 3;
			for (int num4 = list2.Count - 1; num4 >= 1; num4--)
			{
				for (int num5 = num4 - 1; num5 >= 0; num5--)
				{
					Physics.IgnoreCollision(list2[num4].GetComponent<Collider>(), list2[num5].GetComponent<Collider>());
				}
			}
		}
		GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 3);
	}

	[RFC(22)]
	private void GameMapReadyCreateGround(int matchID, int blockN, int groundN)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (TNManager.isHosting)
		{
			if (readiness[Global.onlinePlayerSlot] == 3)
			{
				readiness[Global.onlinePlayerSlot] = 4;
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 4);
			return;
		}
		for (int i = 0; i < 11; i++)
		{
			for (int j = 0; j < 17; j++)
			{
				if (blockArr[j][i] != null)
				{
					blockN--;
				}
			}
		}
		if (blockN == 0 && groundN == GameGround.List().Count)
		{
			if (readiness[Global.onlinePlayerSlot] == 3)
			{
				readiness[Global.onlinePlayerSlot] = 4;
			}
			GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 4);
		}
	}

	[RFC(23)]
	private void GameMapReadyCheck(int matchID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (readiness[Global.onlinePlayerSlot] == 4)
		{
			readiness[Global.onlinePlayerSlot] = 5;
			switch (Global.Level)
			{
			case 1:
				far.AddComponent<Theme01>();
				break;
			case 2:
				far.AddComponent<Theme02>();
				break;
			case 3:
				far.AddComponent<Theme03>();
				break;
			case 4:
				far.AddComponent<Theme04>();
				break;
			case 5:
				far.AddComponent<Theme05>();
				break;
			}
		}
		GetComponent<TNObject>().Send(24, Target.Others, Global.onlineMatchID, TNManager.playerID, 5);
	}

	[RFC(24)]
	private void GameMapReadyConfirm(int matchID, int playerID, int readyStep)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (Global.onlinePlayerIDs[i] == playerID)
			{
				readiness[i] = readyStep;
				break;
			}
		}
	}

	public static void InitOnlineMode()
	{
		readiness = new int[8];
	}

	private void AddTNObject(GameObject obj, int tnObjID)
	{
		if (!(obj == null))
		{
			obj.AddComponent<TNObject>().uid = (uint)tnObjID;
			if (tnObjRunID < tnObjID)
			{
				tnObjRunID = tnObjID;
			}
		}
	}
}
