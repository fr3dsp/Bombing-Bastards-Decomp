using UnityEngine;

public class Theme05 : MonoBehaviour
{
	private Transform lightNode;

	private Transform spaceNode;

	private Material pillarMat;

	private Material blockMat;

	private Material[] glowMat;

	private Material glowMatAni;

	private Material glowMatRun;

	private void Awake()
	{
		string[] array = new string[5] { "fx/glowMid", "fx/glowU", "fx/glowD", "fx/glowL", "fx/glowR" };
		glowMat = new Material[4];
		for (int i = 0; i < 4; i++)
		{
			glowMat[i] = base.transform.Find(array[i]).GetComponent<Renderer>().material;
		}
		base.transform.Find(array[4]).GetComponent<Renderer>().material = glowMat[3];
		glowMatAni = base.transform.Find("fxAni/aniL").GetComponent<Renderer>().material;
		base.transform.Find("fxAni/aniR").GetComponent<Renderer>().material = glowMatAni;
		glowMatRun = base.transform.Find("fxRun/runL").GetComponent<Renderer>().material;
		base.transform.Find("fxRun/runR").GetComponent<Renderer>().material = glowMatRun;
		lightNode = base.transform.Find("light");
		spaceNode = base.transform.Find("space");
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
		float time = Time.time;
		float num = (1f + Mathf.Sin(time * 9f)) / 5f + 0.6f;
		glowMat[0].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num));
		float num2 = (1f + Mathf.Sin(time * 4f)) / 4f + 0.5f;
		glowMat[1].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num2));
		float num3 = 1f + Mathf.Sin(time * 4f);
		glowMat[2].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num3));
		float num4 = (1f + Mathf.Sin(time * 1f)) / 2f + 0.5f;
		if (num4 > 1f)
		{
			num4 = 1f;
		}
		glowMat[3].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num4));
		float num5 = (1f + Mathf.Sin(time * 2f)) / 4f + 0.1f;
		glowMatAni.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num5));
		int num6 = (int)(10f * (time - (float)(int)time));
		glowMatAni.SetTextureOffset("_MainTex", new Vector2((float)(num6 % 5 * 205) / 1024f, 0.5f - (float)(num6 / 5) * 0.5f));
		float num7 = 0.5f * time;
		glowMatRun.SetTextureOffset("_MainTex", new Vector2(0f, num7 - (float)(int)num7));
		lightNode.transform.localRotation = Quaternion.Euler(0f, time * 9f, 0f);
		spaceNode.transform.localRotation = Quaternion.Euler(0f, 15f * Mathf.Sin(time / 5f), 0f);
		int num8 = (int)(8f * (time - (float)(int)time));
		num8 %= 4;
		Vector2 mainTextureOffset = Vector2.zero;
		switch (num8)
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
