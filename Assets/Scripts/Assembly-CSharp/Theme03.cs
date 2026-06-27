using UnityEngine;

public class Theme03 : MonoBehaviour
{
	private Material pillarMat;

	private Material cloudMat;

	private Transform fxs;

	private Transform[] layers;

	private Transform[][] nodes;

	private float[][] nodeZ;

	private float windFrq;

	private float windDur;

	private Direction windDir;

	private void Awake()
	{
		GameObject original = Resources.Load("Maps/Theme03/Cloud") as GameObject;
		fxs = base.transform.Find("fxs");
		fxs.parent = base.transform.parent;
		fxs.transform.localPosition = Vector3.zero;
		fxs.transform.localRotation = Quaternion.identity;
		fxs.transform.localScale = Vector3.one;
		for (int i = 0; i < 6; i++)
		{
			Transform transform = fxs.Find("fx" + (i + 1));
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			Vector3 localPosition = transform.localPosition * 16.2f;
			localPosition.z = 0f - localPosition.y + 999f;
			localPosition.y = 0f;
			localPosition.x *= -1f;
			gameObject.transform.parent = fxs;
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Renderer renderer = gameObject.GetComponentInChildren<ParticleSystem>().GetComponent<Renderer>();
			if (cloudMat == null)
			{
				cloudMat = renderer.material;
			}
			else
			{
				renderer.material = cloudMat;
			}
			Object.Destroy(transform.gameObject);
		}
		string[] array = new string[5] { "bg", "farest", "farer", "far", "near" };
		layers = new Transform[5];
		nodes = new Transform[5][];
		nodeZ = new float[5][];
		for (int j = 0; j < 5; j++)
		{
			layers[j] = base.transform.Find(array[j]);
			int childCount = layers[j].childCount;
			nodes[j] = new Transform[childCount];
			nodeZ[j] = new float[childCount];
			for (int k = 0; k < childCount; k++)
			{
				nodes[j][k] = layers[j].GetChild(k);
				nodeZ[j][k] = nodes[j][k].localPosition.z;
			}
		}
		windFrq = 15 * ((Global.Mode != GameMode.Adventure) ? 2 : (6 - Global.advStage % 6));
		windDur = windFrq - 30f;
		if (windDur < 0f)
		{
			windDur = 0f;
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Script");
		Transform transform = gameObject.transform.Find("Pillars");
		Transform transform2 = gameObject.transform.Find("Blocks");
		if (transform.childCount > 0)
		{
			pillarMat = transform.GetChild(0).GetComponent<Renderer>().material;
		}
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			transform.GetChild(num).GetComponent<Renderer>().material = pillarMat;
		}
		Vector2[] array = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0.5f, 0f),
			new Vector2(0f, 0.5f),
			new Vector2(0.5f, 0.5f)
		};
		for (int num2 = transform2.childCount - 1; num2 >= 0; num2--)
		{
			transform2.GetChild(num2).GetComponent<Renderer>().material.mainTextureOffset = array[Random.Range(0, 4)];
		}
	}

	private void Update()
	{
		for (int i = 0; i < 5; i++)
		{
			float x = layers[i].localScale.x;
			float y = layers[i].localPosition.y;
			float num = (y * y + y) / 2f;
			float num2 = (0.1f + 0.005f * num) * Time.deltaTime;
			for (int num3 = nodes[i].Length - 1; num3 >= 0; num3--)
			{
				Vector3 localPosition = nodes[i][num3].localPosition;
				float f = (localPosition.x * x - Time.time) / (1f + 0.01f * num);
				localPosition.x -= num2;
				localPosition.z = nodeZ[i][num3] + y * (1f + ((num3 % 2 != 0) ? Mathf.Cos(f) : Mathf.Sin(f))) / 25f;
				if (i > 0)
				{
					while (localPosition.x <= -14.4f)
					{
						localPosition.x += 28.8f;
					}
				}
				else
				{
					while (localPosition.x <= -9.6f)
					{
						localPosition.x += 19.2f;
					}
				}
				nodes[i][num3].localPosition = localPosition;
			}
		}
		float time = Time.time;
		int num4 = (int)(4f * (time - (float)(int)time));
		Vector2 mainTextureOffset = Vector2.zero;
		switch (num4)
		{
		case 0:
			mainTextureOffset = new Vector2(0f, 0f);
			break;
		case 1:
			mainTextureOffset = new Vector2(0.5f, 0f);
			break;
		case 2:
			mainTextureOffset = new Vector2(0f, 0.5f);
			break;
		case 3:
			mainTextureOffset = new Vector2(0.5f, 0.5f);
			break;
		}
		pillarMat.mainTextureOffset = mainTextureOffset;
		if (Global.IsBossStage)
		{
			return;
		}
		bool flag = (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle) && Global.mapShrink && Global.Map.RemainTime < 31f;
		if (flag)
		{
			float num5 = (Global.Map.RemainTime - 29f) / 4f;
			if (num5 > 0f)
			{
				cloudMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, num5));
			}
			else
			{
				fxs.gameObject.SetActive(false);
			}
		}
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			return;
		}
		windDur += Time.deltaTime;
		if (!(windDur >= windFrq))
		{
			return;
		}
		windDur -= windFrq;
		if (flag)
		{
			return;
		}
		GameMap map = Global.Map;
		switch (Random.Range(0, 4))
		{
		case 0:
		{
			windDir = Direction.Up;
			for (int num7 = -5; num7 <= 5; num7++)
			{
				for (int num8 = -8; num8 <= 8; num8 += 2)
				{
					map.CreateGround(Ground.Wind, num8, num7, (int)windDir);
				}
			}
			for (int num9 = -8; num9 <= 8; num9++)
			{
				map.CreateGround(Ground.Wind, num9, -5, (int)windDir);
			}
			break;
		}
		case 1:
		{
			windDir = Direction.Right;
			for (int num10 = -5; num10 <= 5; num10 += 2)
			{
				for (int num11 = -8; num11 <= 8; num11++)
				{
					map.CreateGround(Ground.Wind, num11, num10, (int)windDir);
				}
			}
			for (int num12 = -5; num12 <= 5; num12++)
			{
				map.CreateGround(Ground.Wind, -8, num12, (int)windDir);
			}
			break;
		}
		case 2:
		{
			windDir = Direction.Down;
			for (int m = -5; m <= 5; m++)
			{
				for (int n = -8; n <= 8; n += 2)
				{
					map.CreateGround(Ground.Wind, n, m, (int)windDir);
				}
			}
			for (int num6 = -8; num6 <= 8; num6++)
			{
				map.CreateGround(Ground.Wind, num6, 5, (int)windDir);
			}
			break;
		}
		case 3:
		{
			windDir = Direction.Left;
			for (int j = -5; j <= 5; j += 2)
			{
				for (int k = -8; k <= 8; k++)
				{
					map.CreateGround(Ground.Wind, k, j, (int)windDir);
				}
			}
			for (int l = -5; l <= 5; l++)
			{
				map.CreateGround(Ground.Wind, 8, l, (int)windDir);
			}
			break;
		}
		}
	}
}
