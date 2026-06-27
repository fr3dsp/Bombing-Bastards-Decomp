using TNet;
using UnityEngine;

public class GameIcon : MonoBehaviour
{
	public enum Type
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
		Player5 = 4,
		Player6 = 5,
		Player7 = 6,
		Player8 = 7,
		Offline = 8
	}

	private struct Icon
	{
		public GameObject obj;

		public Transform model;

		public GameObject icon;

		public Type type;

		public Material mat;
	}

	private static List<Icon> list;

	private static GameObject res;

	private static GameObject root;

	public static void AddObject(GameObject obj, Type type)
	{
		Icon item = default(Icon);
		item.obj = obj;
		item.model = obj.transform.Find("root/Bastard(Clone)");
		item.icon = Object.Instantiate(res) as GameObject;
		item.type = type;
		item.mat = item.icon.transform.GetChild(0).GetComponent<Renderer>().material;
		list.Add(item);
		item.icon.transform.parent = root.transform;
		item.icon.transform.localScale = Vector3.one;
		item.mat.SetTextureOffset("_MainTex", new Vector2((float)((int)type % 3) / 3f, (float)(2 - (int)type / 3) / 3f));
	}

	public static void RemoveObject(GameObject obj)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].obj == obj)
			{
				Object.Destroy(list[num].icon);
				list.RemoveAt(num);
				break;
			}
		}
	}

	private void Awake()
	{
		res = Resources.Load("Environments/Icon/Icon") as GameObject;
		root = new GameObject("Icons");
		root.transform.parent = base.transform;
		root.transform.localScale = new Vector3(10f, 10f, 10f);
		list = new List<Icon>();
	}

	private void OnDestroy()
	{
		res = null;
		root = null;
		list = null;
	}

	private void Update()
	{
		float num = 1.5f * Time.time;
		float t = num - (float)(int)num;
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			Transform transform = list[num2].obj.transform;
			Vector3 localPosition = transform.localPosition;
			if (list[num2].type == Type.Offline)
			{
				list[num2].mat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.25f + Mathf.PingPong(t, 0.25f)));
			}
			else
			{
				localPosition.y = Mathf.PingPong(t, 0.5f);
			}
			list[num2].icon.transform.localPosition = localPosition;
			list[num2].icon.transform.localScale = list[num2].model.localScale;
		}
	}
}
