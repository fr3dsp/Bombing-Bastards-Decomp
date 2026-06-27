using UnityEngine;

public class Theme01 : MonoBehaviour
{
	private Transform sky;

	private Material pillarMat;

	private Material blockMat;

	private void Awake()
	{
		GameObject original = Resources.Load("Maps/Theme01/FX") as GameObject;
		for (int i = 0; i < 3; i++)
		{
			Transform parent = base.transform.Find("fx" + (i + 1));
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}
		sky = base.transform.Find("sky");
		GameObject original2 = Resources.Load("Maps/Theme01/Fog") as GameObject;
		GameObject gameObject2 = Object.Instantiate(original2) as GameObject;
		gameObject2.transform.parent = base.transform.parent;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.transform.localScale = Vector3.one;
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
		if (transform2.childCount > 0)
		{
			blockMat = transform2.GetChild(0).GetComponent<Renderer>().material;
		}
		for (int num = transform.childCount - 1; num >= 0; num--)
		{
			transform.GetChild(num).GetComponent<Renderer>().material = pillarMat;
		}
		for (int num2 = transform2.childCount - 1; num2 >= 0; num2--)
		{
			transform2.GetChild(num2).GetComponent<Renderer>().material = blockMat;
		}
	}

	private void Update()
	{
		Vector3 localPosition = sky.localPosition;
		localPosition.x -= 0.25f * Time.deltaTime;
		if (localPosition.x < -10.24f)
		{
			localPosition.x += 10.24f;
		}
		sky.localPosition = localPosition;
		float time = Time.time;
		int num = (int)(4f * (time - (float)(int)time));
		Vector2 mainTextureOffset = Vector2.zero;
		switch (num)
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
		if (blockMat != null)
		{
			blockMat.mainTextureOffset = mainTextureOffset;
		}
	}
}
