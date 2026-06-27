using System;
using TNet;
using UnityEngine;

public class OnlineBattleResult : MonoBehaviour
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

	private int[] readiness = new int[8];

	private float checkSend;

	private bool checkReady = true;

	private Material plsWaitMat;

	private TextMesh plsWaitTxt;

	private float plsWaitOther;

	private void Awake()
	{
		base.gameObject.AddComponent<TNObject>().uid = 12000u;
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
		if (Global.winID == Global.onlinePlayerSlot)
		{
			tAniBoard[0] = GameObject.Find("board/titleWin").transform;
			GameObject.Find("board/titleLose").SetActive(false);
		}
		else
		{
			tAniBoard[0] = GameObject.Find("board/titleLose").transform;
			GameObject.Find("board/titleWin").SetActive(false);
		}
		tAniBoard[1] = GameObject.Find("board/line").transform;
		tAniBoard[2] = GameObject.Find("board/rect_fx_1").transform;
		tAniBoard[3] = GameObject.Find("board/rect_fx_2").transform;
		mAniBoard = tAniBoard[0].GetComponent<Renderer>().material;
		tBoard = GameObject.Find("board").transform;
		tBase = GameObject.Find("base").transform;
		Material material = new Material(Shader.Find("Unlit/Texture"));
		material.name = "bastard-" + Global.onlinePlayerSlot;
		material.mainTexture = Resources.Load("Characters/Bastard/Textures/bastard-" + Global.onlinePlayerSlot) as Texture;
		Renderer[] componentsInChildren;
		if (Global.winID == Global.onlinePlayerSlot)
		{
			GameObject.Find("base/wloser").SetActive(false);
			componentsInChildren = GameObject.Find("base/winner").transform.GetChild(0).GetComponentsInChildren<Renderer>();
		}
		else
		{
			GameObject.Find("base/winner").SetActive(false);
			componentsInChildren = GameObject.Find("base/wloser").transform.GetChild(0).GetComponentsInChildren<Renderer>();
		}
		for (int num = componentsInChildren.Length - 1; num >= 0; num--)
		{
			if (componentsInChildren[num].sharedMaterial.name != "stun")
			{
				componentsInChildren[num].material = material;
			}
		}
		mFade = GameObject.Find("bg/fade").GetComponent<Renderer>().material;
		iFade = -1;
		fFade = 0.5f;
		mFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fFade));
		plsWaitMat = base.transform.Find("wait/bg").GetComponent<Renderer>().material;
		plsWaitTxt = base.transform.Find("wait/pls_wait").GetComponent<TextMesh>();
	}

	private void Update()
	{
		if (checkReady)
		{
			if (!TNManager.isConnected)
			{
				GameObject gameObject = GameObject.Find("Online.ServerList");
				if (gameObject != null)
				{
					gameObject.GetComponent<OnlineBattleServerList>().PopUpMessage("You have been disconnected\nfrom the server.");
				}
				Application.LoadLevel("OnlineBattle.ServerList");
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (Global.onlinePlayerIDs[i] != -1)
				{
					num++;
					if (readiness[i] > 0 || !Global.onlinePlayerOns[i])
					{
						num2++;
					}
				}
			}
			if (num > num2)
			{
				if (TNManager.isHosting)
				{
					if (checkSend >= 0.2f)
					{
						checkSend = 0f;
					}
					if (checkSend == 0f)
					{
						GetComponent<TNObject>().Send(140, Target.Others, Global.onlineMatchID);
						OnlineBattleResultCheck(Global.onlineMatchID);
					}
					checkSend += Time.deltaTime;
				}
				return;
			}
			checkReady = false;
		}
		if (iFade == 0 && fFade == 0.5f)
		{
			if (TNManager.isConnected)
			{
				Application.LoadLevel("OnlineBattle.LobbyRoom");
				return;
			}
			GameObject gameObject2 = GameObject.Find("Online.ServerList");
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<OnlineBattleServerList>().PopUpMessage("You have been disconnected\nfrom the server.");
			}
			Application.LoadLevel("OnlineBattle.ServerList");
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
		float num3 = Time.time * 0.4f;
		num3 -= (float)(int)num3;
		float f = num3 * 2f * (float)Math.PI;
		float num4 = Mathf.Sin(f) * 0.2f;
		float num5 = Mathf.Cos(f) * 0.1f;
		tLightBase[0].localScale = new Vector3(0.8f + num4, 1f, 1f);
		tLightBase[1].localScale = new Vector3(0.8f - num4, 1f, 1f);
		tLightBG[2].localPosition = new Vector3(-94.2f + num3 * 250f, 310f - (1f - num3) * 175f, -20f);
		mLightBase[3].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.4f + num5));
		mLightBG[3].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.15f + num5));
		float num6 = Mathf.Cos(num3 * 4f * (float)Math.PI) * 0.1f;
		mLightBase[0].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.4f + num6));
		mLightBase[1].SetColor("_TintColor", new Color(0.25f, 0.5f, 0.5f, 0.4f + num6));
		float num7 = num3 * 1.5f;
		float num8 = num3 * 3f;
		if (num8 > 1f)
		{
			num8 = (3f - num8) / 2f;
		}
		Vector3 localPosition3 = tAniBoard[2].localPosition;
		Vector3 localPosition4 = tAniBoard[3].localPosition;
		localPosition3.x = (num7 * 2f - 1f) * 60f;
		localPosition4.x = (num8 * 2f - 1f) * 60f;
		tAniBoard[2].localPosition = localPosition3;
		tAniBoard[2].localScale = new Vector3((!(num7 > 1f)) ? (num6 * 5f) : 0f, 1f, 1f);
		tAniBoard[3].localPosition = localPosition4;
		tAniBoard[3].localScale = new Vector3(num8, 1f, 1f);
		num3 = Time.time;
		num3 -= (float)(int)num3;
		tLightBase[2].localScale = new Vector3(1f - num3, 1f, 1f);
		mLightBase[2].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num3));
		num3 = Time.time * 0.3f;
		num3 -= (float)(int)num3;
		float num9 = Mathf.Sin(num3 * 2f * (float)Math.PI) * 3f;
		tLightBG[0].localRotation = Quaternion.Euler(0f, 0f, 325f + num9);
		tLightBG[1].localRotation = Quaternion.Euler(0f, 0f, 325f - num9);
		float num10 = Mathf.PingPong(Time.time * 2f, 2f) - 1f;
		Vector3 localPosition5 = tAniBoard[1].localPosition;
		localPosition5.x = -7.2f + num10 * 61.6f;
		tAniBoard[1].localPosition = localPosition5;
		tAniBoard[1].localScale = new Vector3(0.5f + (num10 + 1f) / 4f, 1f, 1f);
		float num11 = num3 * 5f;
		if (num11 >= 4f)
		{
			num11 -= 4f;
			num11 *= 4f;
		}
		mAniBoard.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f + (1f - Mathf.Cos(num11 * (float)Math.PI)) / 8f));
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
		GameController controller = GameInput.GetController(Global.onlinePlayerSlot);
		if (controller.DoEnterHold("Fire1") || controller.DoPadPress(Global.onlinePlayerSlot))
		{
			controller.DoPadPress(Global.onlinePlayerSlot);
			GetComponent<TNObject>().Send(142, Target.Others, Global.onlinePlayerSlot);
			readiness[Global.onlinePlayerSlot] = 2;
		}
		if (readiness[Global.onlinePlayerSlot] == 2)
		{
			if (plsWaitOther < 1f)
			{
				plsWaitOther += 2f * Time.deltaTime;
				if (plsWaitOther > 1f)
				{
					plsWaitOther = 1f;
				}
				plsWaitMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, plsWaitOther / 4f));
			}
			float num12 = 2f * (Time.time - (float)(int)Time.time);
			plsWaitTxt.color = new Color(1f, 1f, 1f, plsWaitOther * ((!(num12 < 1f)) ? (2f - num12) : num12));
			num12 *= 2f;
			string text = "Please wait";
			for (int num13 = (int)num12; num13 > 0; num13--)
			{
				text += ".";
			}
			plsWaitTxt.text = text;
		}
		if (fFade != 0f || iFade != 0)
		{
			return;
		}
		bool flag = true;
		for (int j = 0; j < 8; j++)
		{
			if (Global.onlinePlayerIDs[j] != -1 && Global.onlinePlayerOns[j] && readiness[j] < 2)
			{
				flag = false;
				break;
			}
		}
		if (flag || (readiness[Global.onlinePlayerSlot] == 2 && !TNManager.isConnected))
		{
			iFade = 1;
		}
	}

	[RFC(140)]
	private void OnlineBattleResultCheck(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (readiness[Global.onlinePlayerSlot] == 0)
			{
				readiness[Global.onlinePlayerSlot] = 1;
			}
			GetComponent<TNObject>().Send(141, Target.Others, Global.onlineMatchID, TNManager.playerID);
		}
	}

	[RFC(141)]
	private void OnlineBattleResultConfirm(int matchID, int playerID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (Global.onlinePlayerIDs[i] == playerID)
			{
				if (readiness[i] == 0)
				{
					readiness[i] = 1;
				}
				break;
			}
		}
	}

	[RFC(142)]
	private void OnlineBattleResultReady(int slot)
	{
		readiness[slot] = 2;
	}
}
