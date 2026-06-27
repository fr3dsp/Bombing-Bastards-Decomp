using System;
using UnityEngine;

public class LocalBattleResult : MonoBehaviour
{
	private Transform[] tLightBase;

	private Transform[] tLightBG;

	private Material[] mLightBase;

	private Material[] mLightBG;

	private Transform[] tAniBoard;

	private Material mAniBoard;

	private Material mFade;

	private int iFade;

	private float fFade;

	private Transform tBoard;

	private Transform tBase;

	private void Awake()
	{
		tLightBase = new Transform[4];
		tLightBG = new Transform[4];
		mLightBase = new Material[4];
		mLightBG = new Material[4];
		tAniBoard = new Transform[4];
		for (int i = 0; i < 4; i++)
		{
			tLightBase[i] = GameObject.Find("base/l_" + (i + 1)).transform;
			Renderer renderer = tLightBase[i].GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = tLightBase[i].GetComponentInChildren<Renderer>();
			}
			mLightBase[i] = renderer.material;
		}
		for (int j = 0; j < 4; j++)
		{
			tLightBG[j] = GameObject.Find("bg/l_" + (j + 1)).transform;
			Renderer renderer2 = tLightBG[j].GetComponent<Renderer>();
			if (renderer2 == null)
			{
				renderer2 = tLightBG[j].GetComponentInChildren<Renderer>();
			}
			mLightBG[j] = renderer2.material;
		}
		tAniBoard[0] = GameObject.Find("board/title").transform;
		tAniBoard[1] = GameObject.Find("board/line").transform;
		tAniBoard[2] = GameObject.Find("board/rect_fx_1").transform;
		tAniBoard[3] = GameObject.Find("board/rect_fx_2").transform;
		mAniBoard = tAniBoard[0].GetComponent<Renderer>().material;
		tBoard = GameObject.Find("board").transform;
		tBase = GameObject.Find("base").transform;
		GameObject.Find("board/joy").GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, (float)(8 - Global.localPlayerSlot[Global.winID]) / 8f));
		for (int k = Global.winAmount + 1; k <= 5; k++)
		{
			GameObject.Find("board/trophy_" + k).SetActive(false);
		}
		if (Global.winID != 0)
		{
			Material material = new Material(Shader.Find("Unlit/Texture"));
			material.name = "bastard-" + Global.winID;
			material.mainTexture = Resources.Load("Characters/Bastard/Textures/bastard-" + Global.winID) as Texture;
			Renderer[] componentsInChildren = GameObject.Find("base/winner").transform.GetChild(0).GetComponentsInChildren<Renderer>();
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				componentsInChildren[num].material = material;
			}
		}
		mFade = GameObject.Find("bg/fade").GetComponent<Renderer>().material;
		iFade = -1;
		fFade = 0.5f;
		mFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fFade));
	}

	private void Update()
	{
		if (iFade == 0 && fFade == 0.5f)
		{
			Application.LoadLevel("LocalBattle.Menu");
			return;
		}
		if (iFade != -1 || fFade < 0.25f)
		{
			Vector3 localPosition = tBoard.localPosition;
			if (tBoard.name.Contains("*"))
			{
				if (localPosition.y < 2.25f)
				{
					localPosition.y += Time.deltaTime * 75f / 5f;
					if (localPosition.y > 2.25f)
					{
						localPosition.y = 2.25f;
					}
					tBoard.localPosition = localPosition;
				}
			}
			else if (localPosition.y > 0f)
			{
				localPosition.y -= Time.deltaTime * 75f;
				if (localPosition.y < 0f)
				{
					localPosition.y = 0f;
					tBoard.name += "*";
				}
				tBoard.localPosition = localPosition;
			}
			Vector3 localPosition2 = tBase.localPosition;
			if (tBase.name.Contains("*"))
			{
				if (localPosition2.y > -5.25f)
				{
					localPosition2.y -= Time.deltaTime * 125f / 5f;
					if (localPosition2.y < -5.25f)
					{
						localPosition2.y = -5.25f;
					}
					tBase.localPosition = localPosition2;
				}
			}
			else if (localPosition2.y < 0f)
			{
				localPosition2.y += Time.deltaTime * 125f;
				if (localPosition2.y > 0f)
				{
					localPosition2.y = 0f;
					tBase.name += "*";
				}
				tBase.localPosition = localPosition2;
			}
		}
		float num = Time.time * 0.4f;
		num -= (float)(int)num;
		float f = num * 2f * (float)Math.PI;
		float num2 = Mathf.Sin(f) * 0.2f;
		float num3 = Mathf.Cos(f) * 0.1f;
		tLightBase[0].localScale = new Vector3(0.8f + num2, 1f, 1f);
		tLightBase[1].localScale = new Vector3(0.8f - num2, 1f, 1f);
		tLightBG[2].localPosition = new Vector3(-94.2f + num * 250f, 310f - (1f - num) * 175f, -20f);
		mLightBase[3].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.4f + num3));
		mLightBG[3].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.15f + num3));
		float num4 = Mathf.Cos(num * 4f * (float)Math.PI) * 0.1f;
		mLightBase[0].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.4f + num4));
		mLightBase[1].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.4f + num4));
		float num5 = num * 1.5f;
		float num6 = num * 3f;
		if (num6 > 1f)
		{
			num6 = (3f - num6) / 2f;
		}
		Vector3 localPosition3 = tAniBoard[2].localPosition;
		Vector3 localPosition4 = tAniBoard[3].localPosition;
		localPosition3.x = (num5 * 2f - 1f) * 60f;
		localPosition4.x = (num6 * 2f - 1f) * 60f;
		tAniBoard[2].localPosition = localPosition3;
		tAniBoard[2].localScale = new Vector3((!(num5 > 1f)) ? (num4 * 5f) : 0f, 1f, 1f);
		tAniBoard[3].localPosition = localPosition4;
		tAniBoard[3].localScale = new Vector3(num6, 1f, 1f);
		num = Time.time;
		num -= (float)(int)num;
		tLightBase[2].localScale = new Vector3(1f - num, 1f, 1f);
		mLightBase[2].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num));
		num = Time.time * 0.3f;
		num -= (float)(int)num;
		float num7 = Mathf.Sin(num * 2f * (float)Math.PI) * 3f;
		tLightBG[0].localRotation = Quaternion.Euler(0f, 0f, 325f + num7);
		tLightBG[1].localRotation = Quaternion.Euler(0f, 0f, 325f - num7);
		float num8 = Mathf.PingPong(Time.time * 2f, 2f) - 1f;
		Vector3 localPosition5 = tAniBoard[1].localPosition;
		localPosition5.x = -7.2f + num8 * 61.6f;
		tAniBoard[1].localPosition = localPosition5;
		tAniBoard[1].localScale = new Vector3(0.5f + (num8 + 1f) / 4f, 1f, 1f);
		float num9 = num * 5f;
		if (num9 >= 4f)
		{
			num9 -= 4f;
			num9 *= 4f;
		}
		mAniBoard.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f + (1f - Mathf.Cos(num9 * (float)Math.PI)) / 8f));
		if (iFade != 0)
		{
			fFade += (float)iFade * Time.deltaTime / 2f;
			if (iFade > 0)
			{
				if (fFade >= 0.5f)
				{
					fFade = 0.5f;
					iFade = 0;
				}
			}
			else if (fFade <= 0f)
			{
				fFade = 0f;
				iFade = 0;
			}
			mFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fFade));
		}
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (controller.IsConnected() && controller.DoEnterHold("Fire1"))
			{
				iFade = 1;
				break;
			}
		}
	}
}
