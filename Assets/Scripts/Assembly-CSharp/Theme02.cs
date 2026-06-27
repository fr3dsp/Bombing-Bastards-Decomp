using UnityEngine;

public class Theme02 : MonoBehaviour
{
	private Transform fx;

	private float fxWait;

	private void Awake()
	{
		GameObject original = Resources.Load("Maps/Theme02/Snow") as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		fx = base.transform.Find("fx");
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Script");
		Transform transform = gameObject.transform.Find("Pillars");
		Transform transform2 = gameObject.transform.Find("Blocks");
		Vector2[] array = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0.5f, 0f),
			new Vector2(0f, 0.5f),
			new Vector2(0.5f, 0.5f)
		};
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			transform.GetChild(num).GetComponent<Renderer>().material.mainTextureOffset = array[Random.Range(0, 4)];
		}
		for (int num2 = transform2.childCount - 1; num2 >= 0; num2--)
		{
			transform2.GetChild(num2).GetComponent<Renderer>().material.mainTextureOffset = array[Random.Range(0, 4)];
		}
	}

	private void Update()
	{
		Vector3 localPosition = fx.localPosition;
		if (localPosition.x <= -14.5f)
		{
			localPosition.x = -14.5f;
			if (localPosition.x == -14.5f)
			{
				fxWait += Time.deltaTime;
				if (fxWait > 2.5f)
				{
					fxWait = 0f;
					localPosition.x = 14.5f;
				}
			}
		}
		else
		{
			localPosition.x -= 25f * Time.deltaTime;
		}
		fx.localPosition = localPosition;
	}
}
