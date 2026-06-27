using TNet;
using UnityEngine;

public class GameShadow : MonoBehaviour
{
	private struct Shadow
	{
		public GameObject obj;

		public GameObject shadow;

		public Material mat;
	}

	private static List<Shadow> list;

	private static GameObject res;

	private static GameObject root;

	private static float shadowSize;

	public static void AddObject(GameObject obj)
	{
		Shadow item = new Shadow
		{
			obj = obj,
			shadow = (Object.Instantiate(res) as GameObject)
		};
		if (Global.Level == 3)
		{
			item.mat = item.shadow.GetComponent<Renderer>().material;
		}
		list.Add(item);
		item.shadow.transform.parent = root.transform;
	}

	public static void RemoveObject(GameObject obj)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].obj == obj)
			{
				Object.Destroy(list[num].shadow);
				list.RemoveAt(num);
				break;
			}
		}
	}

	private void Awake()
	{
		res = Resources.Load("Environments/Shadow/Shadow") as GameObject;
		root = new GameObject("Shadows");
		root.transform.parent = base.transform;
		root.transform.localScale = new Vector3(10f, 10f, 10f);
		list = new List<Shadow>();
		float num = 0f;
		Vector3[] vertices = res.GetComponent<MeshFilter>().mesh.vertices;
		for (int num2 = vertices.Length - 1; num2 >= 0; num2--)
		{
			if (num < vertices[num2].x)
			{
				num = vertices[num2].x;
			}
		}
		shadowSize = num * 2f * 10f;
	}

	private void OnDestroy()
	{
		res = null;
		root = null;
		list = null;
	}

	private void Update()
	{
		float num = 8.5f;
		float num2 = 5.5f;
		for (int num3 = list.Count - 1; num3 >= 0; num3--)
		{
			Transform transform = list[num3].obj.transform;
			Vector3 localPosition = transform.localPosition;
			localPosition.y = -0.01f;
			if (Global.Level == 3)
			{
				float num4 = transform.localScale.x / 10f * shadowSize;
				float num5 = num4 / 2f;
				float num6 = num - num5;
				float num7 = num2 - num5;
				bool flag = localPosition.x >= 0f - num6 && localPosition.x <= num6;
				bool flag2 = localPosition.z >= 0f - num7 && localPosition.z <= num7;
				if (flag && flag2)
				{
					list[num3].shadow.transform.localPosition = localPosition;
					list[num3].shadow.transform.localScale = transform.localScale;
					list[num3].mat.SetTextureOffset("_MainTex", Vector2.zero);
					list[num3].mat.SetTextureScale("_MainTex", Vector2.one);
				}
				else
				{
					float num8 = num + num5;
					float num9 = num2 + num5;
					bool flag3 = localPosition.x <= 0f - num8 || localPosition.x >= num8;
					bool flag4 = localPosition.z <= 0f - num9 || localPosition.z >= num9;
					if (flag3 || flag4)
					{
						list[num3].shadow.transform.localPosition = localPosition;
						list[num3].shadow.transform.localScale = Vector3.zero;
					}
					else
					{
						Vector3 localScale = transform.localScale;
						float num10 = 1f;
						float num11 = 1f;
						float x = 0f;
						float y = 0f;
						if (!flag && !flag3)
						{
							float num12 = ((!(localPosition.x > 0f)) ? (0f - num6) : num6) - localPosition.x;
							localPosition.x += num12 / 2f;
							num10 = (num4 - Mathf.Abs(num12)) / num4;
							localScale.x *= num10;
							if (localPosition.x > 0f)
							{
								x = 1f - num10;
							}
						}
						if (!flag2 && !flag4)
						{
							float num13 = ((!(localPosition.z > 0f)) ? (0f - num7) : num7) - localPosition.z;
							localPosition.z += num13 / 2f;
							num11 = (num4 - Mathf.Abs(num13)) / num4;
							localScale.z *= num11;
							if (localPosition.z > 0f)
							{
								y = 1f - num11;
							}
						}
						list[num3].shadow.transform.localPosition = localPosition;
						list[num3].shadow.transform.localScale = localScale;
						list[num3].mat.SetTextureOffset("_MainTex", new Vector2(x, y));
						list[num3].mat.SetTextureScale("_MainTex", new Vector2(num10, num11));
					}
				}
			}
			else
			{
				list[num3].shadow.transform.localPosition = localPosition;
				list[num3].shadow.transform.localScale = transform.localScale;
			}
		}
	}
}
