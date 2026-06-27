using System;
using UnityEngine;

public class Theme04 : MonoBehaviour
{
	private Material[][] glowMat;

	private Material lavaMat;

	private Material pillarMat;

	private Material blockMat;

	private ParticleSystem[] bubble;

	private Material[][] bubbleMat;

	private void Awake()
	{
		char[] array = new char[2] { 'L', 'R' };
		glowMat = new Material[2][];
		for (int i = 0; i < 2; i++)
		{
			MeshRenderer component = base.transform.Find("bg" + array[i]).GetComponent<MeshRenderer>();
			glowMat[i] = new Material[4];
			glowMat[i][0] = component.material;
			for (int j = 1; j < 4; j++)
			{
				glowMat[i][j] = UnityEngine.Object.Instantiate(Resources.Load("Maps/Theme04/Materials/fxGlow" + j + array[i])) as Material;
			}
			component.materials = glowMat[i];
		}
		lavaMat = base.transform.Find("lava").GetComponent<MeshRenderer>().material;
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Maps/Theme04/FXs")) as GameObject;
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		bubble = gameObject.GetComponentsInChildren<ParticleSystem>();
		bubbleMat = new Material[2][];
		int num = bubble.Length;
		for (int k = 0; k < 2; k++)
		{
			bubbleMat[k] = new Material[num];
		}
		for (int l = 0; l < num; l++)
		{
			if (bubble[l].name != "bubble")
			{
				bubble[l] = null;
			}
			else
			{
				bubbleMat[0][l] = bubble[l].GetComponent<Renderer>().material;
			}
		}
		Material original = Resources.Load("Maps/Theme04/Materials/bubbleR") as Material;
		for (int m = 0; m < num; m++)
		{
			bubbleMat[1][m] = UnityEngine.Object.Instantiate(original) as Material;
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
		for (int i = 1; i < 4; i++)
		{
			Color color = new Color(0.5f, 0.5f, 0.5f, (1f + Mathf.Sin((float)(i * 2) + time)) / 6f);
			for (int j = 0; j < 2; j++)
			{
				glowMat[j][i].SetColor("_TintColor", color);
			}
		}
		float x = (1f + Mathf.Cos(time / 4f)) / 2f;
		float num = time / 10f + Mathf.Sin(time / 10f * 2f * (float)Math.PI) / 16f;
		num -= (float)(int)num;
		lavaMat.SetTextureOffset("_MainTex", new Vector2(x, num));
		for (int num2 = bubble.Length - 1; num2 >= 0; num2--)
		{
			if (bubble[num2] != null && bubble[num2].particleCount == 0)
			{
				bubble[num2].GetComponent<Renderer>().material = bubbleMat[UnityEngine.Random.Range(0, 2)][num2];
			}
		}
		int num3 = (int)(4f * (time - (float)(int)time));
		Vector2 mainTextureOffset = Vector2.zero;
		switch (num3)
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
