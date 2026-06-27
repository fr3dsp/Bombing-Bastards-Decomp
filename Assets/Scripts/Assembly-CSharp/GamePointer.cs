using TNet;
using UnityEngine;

public class GamePointer : MonoBehaviour
{
	private struct Pointer
	{
		public int player;

		public int color;

		public GameObject point;

		public bool enable;

		public Vector3 lookPos;

		public bool isHit;

		public Material material;
	}

	private static Pointer[] list;

	private static GameObject res;

	private static GameObject root;

	private float pointerSpeed = Global.pointerSpd - 160f;

	private Vector2 direction;

	private static Vector3[] buffP = new Vector3[8];

	private static Vector3[] buffQ = new Vector3[8];

	private static float[] buffT = new float[8];

	private float sendDelay;

	public static void AddObject(int player, int color)
	{
		Pointer pointer = new Pointer
		{
			player = player,
			color = color,
			point = (Object.Instantiate(res) as GameObject),
			enable = false,
			lookPos = Vector3.zero
		};
		list[player] = pointer;
		pointer.point.transform.parent = root.transform;
		pointer.point.transform.localPosition = new Vector3(0f, 0f, 0f);
		pointer.point.transform.localScale = Vector3.zero;
		pointer.point.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
		TextureUtility.SetSpriteIndex(pointer.point, 8, 6, color);
	}

	public static void EnablePointer(bool enable, int player)
	{
		list[player].enable = enable;
		if (enable)
		{
			list[player].point.transform.localScale = Vector3.one;
		}
		else
		{
			list[player].point.transform.localScale = Vector3.zero;
		}
	}

	public static Vector3 GetPos(int player)
	{
		return list[player].lookPos;
	}

	public static bool isHit(int player)
	{
		return list[player].isHit;
	}

	private void Awake()
	{
		if (Global.Mode != GameMode.LocalBattle && Global.Mode != GameMode.OnlineBattle)
		{
			base.enabled = false;
			return;
		}
		Vector3 localPosition = Camera.main.transform.localPosition;
		res = Resources.Load("Characters/BastardJet/Pointer") as GameObject;
		root = new GameObject("Pointers");
		root.transform.parent = base.transform;
		root.transform.localPosition = localPosition * 0.9f;
		root.transform.localScale = new Vector3(1f, 1f, 1f);
		list = new Pointer[(Global.Mode != GameMode.OnlineBattle) ? 5 : 8];
		if (Global.Mode == GameMode.OnlineBattle)
		{
			base.gameObject.AddComponent<TNObject>().uid = 21000u;
		}
	}

	private void OnDestroy()
	{
		res = null;
		root = null;
		list = null;
	}

	private void Update()
	{
		Vector2 offset = new Vector2
		{
			y = (float)((int)(Time.time * 10f) % 6) * (1f / 6f)
		};
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].point != null)
			{
				offset.x = (float)list[i].color * 0.125f;
				if (list[i].material == null)
				{
					list[i].material = list[i].point.GetComponent<Renderer>().material;
				}
				list[i].material.SetTextureOffset("_MainTex", offset);
			}
		}
		for (int j = 0; j < list.Length; j++)
		{
			GameController controller = GameInput.GetController(j);
			if (list[j].point == null || !controller.IsConnected() || !list[j].enable)
			{
				continue;
			}
			RaycastHit hitInfo = default(RaycastHit);
			int layerMask = 1 << LayerMask.NameToLayer("Ground");
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			Vector3 normalized = (list[j].point.transform.position - gameObject.transform.position).normalized;
			Debug.DrawRay(gameObject.transform.position, normalized * 1000f, Color.green);
			if (Physics.Raycast(gameObject.transform.position, normalized, out hitInfo, 1000f, layerMask))
			{
				Debug.DrawLine(gameObject.transform.position, hitInfo.point, Color.red);
				list[j].lookPos = hitInfo.point;
				list[j].isHit = true;
			}
			else
			{
				list[j].isHit = false;
			}
			if (Global.Mode != GameMode.OnlineBattle || ((GameRemoteController)controller).id == Global.onlinePlayerID)
			{
				Vector3 localPosition = list[j].point.transform.localPosition;
				float num = controller.GetHorizontal() * 1.25f;
				if (num < 0f)
				{
					localPosition.x += Time.deltaTime * pointerSpeed * num;
					if (localPosition.x < -8f)
					{
						localPosition.x = -8f;
					}
					list[j].point.transform.localPosition = localPosition;
				}
				else if (num > 0f)
				{
					localPosition.x += Time.deltaTime * pointerSpeed * num;
					if (localPosition.x > 8f)
					{
						localPosition.x = 8f;
					}
					list[j].point.transform.localPosition = localPosition;
				}
				float num2 = controller.GetVertical() * 1.25f;
				if (num2 < 0f)
				{
					localPosition.z -= Time.deltaTime * pointerSpeed * num2;
					if (localPosition.z > 5f)
					{
						localPosition.z = 5f;
					}
					list[j].point.transform.localPosition = localPosition;
				}
				else if (num2 > 0f)
				{
					localPosition.z -= Time.deltaTime * pointerSpeed * num2;
					if (localPosition.z < -5f)
					{
						localPosition.z = -5f;
					}
					list[j].point.transform.localPosition = localPosition;
				}
				if (Global.Mode == GameMode.OnlineBattle)
				{
					if (sendDelay < 0.1f)
					{
						sendDelay += Time.deltaTime;
						continue;
					}
					sendDelay = 0f;
					GetComponent<TNObject>().SendQuickly(80, Target.Others, Global.onlineMatchID, j, localPosition);
				}
			}
			else if (buffP[j] != buffQ[j])
			{
				list[j].point.transform.localPosition = Vector3.Lerp(buffP[j], buffQ[j], 10f * (Time.time - buffT[j]));
			}
		}
	}

	[RFC(80)]
	private void GamePointerUpdatePhysic(int matchID, int player, Vector3 pos)
	{
		if (Global.onlineMatchID == matchID && list[player].point != null)
		{
			buffP[player] = list[player].point.transform.localPosition;
			buffQ[player] = pos;
			buffT[player] = Time.time;
		}
	}
}
