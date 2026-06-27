using System;
using TNet;
using UnityEngine;

public class GameBoss : GameCharacter
{
	public static bool HardMode = true;

	private int level = 1;

	private int hp = 10;

	private float entering;

	private int rotating;

	private float phaseOld;

	private float phaseCur;

	private float phaseMax;

	private int plantEnrageStep;

	private GameObject plantStunFX;

	private GameObject pillarMagic;

	private GameObject pillarMagicFX;

	private Material pillarMagicMat;

	private float pillarMagicT;

	private GameObject pillarRing;

	private GameObject pillarFog;

	private bool pillarBomb;

	private Material pillarBombMat;

	private int pillarBombN;

	private float pillarBombT;

	private int pillarInc;

	private int pillarCount;

	private int pillarIndex;

	private int pillarSign;

	private Vector3[] pillars;

	private GameObject[] pillarBombs;

	private GameMinion[] pillarMobs;

	private GameObject[][] pillarFogs;

	private GameObject pillarFallFX;

	private GameObject pillarBreath;

	private ParticleSystem pillarBreathFX;

	private GameObject chargeBombFX;

	private GameBomb[] chargeBombs;

	private float[] chargeBombT;

	private bool chargeAddsRnd;

	private GameObject[] chargeAdds;

	private int chargeEnrageCount;

	private int chargeEnrageStep;

	private float chargeEnrageAngle;

	private GameObject chargeStormFXRes;

	private GameObject chargeStormFX;

	private GameObject chargeStormDust;

	private float[] chargeStormSpeeds;

	private int chargeStormSign;

	private float[][] chargeStormWait;

	private float[][] chargeStormTime;

	private GameMinion[][] chargeStormMobs;

	private float chargeStormPhaseMax;

	private GameObject lineFireLandFX;

	private GameObject lineFireHurtFX;

	private int lineLastHP;

	private float lineHurtDur;

	private int lineMaxNum;

	private bool lineRoar;

	private int lineRoarBall;

	private int lineRoarCount;

	private Vector3 lineJumpPos;

	private Vector3 lineJumpEnd;

	private float lineJumpAOE;

	private GameObject lineFireAOE;

	private GameObject lineStunAOE;

	private GameObject lineFireInhale;

	private GameObject lineFireBreath;

	private GameObject lineFireBreathX;

	private GameObject lineFireBreathFX;

	private GameObject lineFireRage;

	private GameObject lineBombDrop;

	private bool lineBombRight;

	private int ringActionID;

	private GameObject ringBarrier;

	private Material[] ringBarrierMat;

	private float[] ringBarrierA;

	private float[] ringBarrierASpd;

	private float[] ringBarrierAAmp;

	private float ringBarrierPause;

	private float ringBarrierDmg;

	private Transform ringBarrierNode;

	private Vector3 ringRollPos;

	private Vector3 ringRollEnd;

	private bool ringRolling;

	private Material ringRollinMat;

	private float ringRollinAlpha;

	private Material ringSkinFXMat;

	private float ringSkinFXAlpha;

	private GameObject ringChargeFX;

	private Material ringChargeMat;

	private float ringChargeAlpha;

	private Vector3 ringBallShoot;

	private Vector3[] ringBallPosit;

	private GameObject[] ringBalls;

	private int ringBallN;

	private float ringBallRun;

	private float ringBallRunMax;

	private float ringBallGrowth;

	private Transform ringBallHand;

	private GameObject ringBallBall;

	private Material ringPlasmaMat;

	private GameObject[] ringPlasmaRing;

	private int ringPlasmaRingIdx;

	private float ringNormalizeHaste;

	private GameObject ringLightGlow;

	private Material ringLightGlowMat;

	private float ringLightGlowT;

	protected override void Init(int param)
	{
		level = param;
		phaseOld = 0f;
		phaseCur = 0f;
		switch (level)
		{
		case 1:
			phaseMax = 4f;
			break;
		case 2:
			phaseMax = 9f;
			break;
		case 3:
			phaseMax = 9f;
			break;
		case 4:
			phaseMax = 20f;
			break;
		case 5:
			phaseMax = 10f;
			break;
		}
		switch (level)
		{
		case 1:
			plantStunFX = Resources.Load("Skills/01/StunAOE") as GameObject;
			break;
		case 2:
		{
			pillarMagic = Resources.Load("Skills/02/IceMagic") as GameObject;
			pillarRing = Resources.Load("Skills/02/IceRing") as GameObject;
			pillarFog = Resources.Load("Skills/02/IceFog") as GameObject;
			pillarFallFX = Resources.Load("Skills/02/IceFall") as GameObject;
			pillarBreath = Resources.Load("Skills/02/IceBreath") as GameObject;
			pillars = new Vector3[8];
			pillarBombs = new GameObject[8];
			pillarMobs = new GameMinion[8];
			pillarFogs = new GameObject[17][];
			for (int k = 0; k < 17; k++)
			{
				pillarFogs[k] = new GameObject[11];
			}
			pillarBombN = 3;
			break;
		}
		case 3:
		{
			chargeBombFX = Resources.Load("Skills/03/Inhale") as GameObject;
			chargeBombs = new GameBomb[6];
			chargeBombT = new float[6];
			chargeStormFXRes = Resources.Load("Skills/03/EyeStorm") as GameObject;
			chargeStormDust = Resources.Load("Skills/03/Dust") as GameObject;
			chargeStormSpeeds = new float[3] { 5f, 6f, 6.5f };
			chargeStormWait = new float[3][];
			chargeStormTime = new float[3][];
			chargeStormMobs = new GameMinion[3][];
			for (int l = 0; l < 3; l++)
			{
				int num3 = 4 + l * 2;
				chargeStormWait[l] = new float[num3];
				chargeStormTime[l] = new float[num3];
				chargeStormMobs[l] = new GameMinion[num3];
			}
			break;
		}
		case 4:
			lineFireLandFX = Resources.Load("Skills/04/FireLand") as GameObject;
			lineFireHurtFX = Resources.Load("Skills/04/FireHurt") as GameObject;
			lineFireAOE = Resources.Load("Skills/04/FireAOE") as GameObject;
			lineStunAOE = Resources.Load("Skills/04/StunAOE") as GameObject;
			lineFireInhale = Resources.Load("Skills/04/Inhale") as GameObject;
			lineFireBreath = Resources.Load("Skills/04/FireBreath") as GameObject;
			lineFireBreathX = Resources.Load("Skills/04/FireBreathX") as GameObject;
			lineFireRage = Resources.Load("Skills/04/FireRage") as GameObject;
			lineBombDrop = Resources.Load("Skills/04/FireRing") as GameObject;
			lineLastHP = hp;
			lineMaxNum = 3;
			lineRoarCount = 1;
			lineBombRight = true;
			break;
		case 5:
		{
			ringBarrier = Resources.Load("Skills/05/Barrier") as GameObject;
			ringChargeFX = Resources.Load("Skills/05/Charge") as GameObject;
			ringLightGlow = Resources.Load("Skills/05/LightGlow") as GameObject;
			ringActionID = -1;
			ringBallPosit = new Vector3[9];
			ringPlasmaMat = UnityEngine.Object.Instantiate(Resources.Load("Skills/05/Materials/lightning")) as Material;
			ringPlasmaRing = new GameObject[2];
			GameObject original = Resources.Load("Skills/05/PlasmaFX") as GameObject;
			for (int i = 0; i < 2; i++)
			{
				ringPlasmaRing[i] = new GameObject("PlasmaFXs");
				for (int j = 0; j < 100; j++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
					gameObject.name = "PlasmaFX(" + j.ToString("D3") + ")";
					gameObject.transform.parent = ringPlasmaRing[i].transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.Euler(0f, (float)(-j) * 3.6f, 0f);
					gameObject.transform.localScale = Vector3.one;
					SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
					Mesh mesh = UnityEngine.Object.Instantiate(componentInChildren.sharedMesh) as Mesh;
					Vector2[] uv = mesh.uv;
					for (int num = uv.Length - 1; num >= 0; num--)
					{
						uv[num].x += (float)j / 100f;
					}
					if (j >= 100)
					{
						for (int num2 = uv.Length - 1; num2 >= 0; num2--)
						{
							uv[num2].y += 0.25f;
						}
					}
					mesh.uv = uv;
					componentInChildren.material = ringPlasmaMat;
					componentInChildren.sharedMesh = mesh;
					gameObject.GetComponent<Animation>()["idle"].time = 0f;
					gameObject.GetComponent<Animation>()["idle"].speed = 0f;
				}
			}
			break;
		}
		}
	}

	public override void Dead(int killerID = -1)
	{
		switch (level)
		{
		case 4:
			if (lineJumpAOE > 0f)
			{
				return;
			}
			break;
		case 5:
			if (ringBarrierDmg == Time.time)
			{
				return;
			}
			if (ringBarrierPause == 0f)
			{
				ringBarrierPause = 3f;
				ringBarrierDmg = Time.time;
				for (int num = ringBarrierA.Length - 1; num >= 0; num--)
				{
					ringBarrierASpd[num] = -2.5f;
				}
				GameSound.StartSFX("skillBarrierDown");
				Transform transform = base.transform.parent.parent.Find("Explosions");
				for (int num2 = transform.childCount - 1; num2 >= 0; num2--)
				{
					Physics.IgnoreCollision(transform.GetChild(num2).GetComponent<Collider>(), base.transform.Find("root").GetComponent<Collider>());
				}
				return;
			}
			if (ringRolling)
			{
				return;
			}
			break;
		}
		if (blink == 0f && !dead)
		{
			hp--;
			Debug.Log("hp => " + hp);
			if (hp == 0)
			{
				base.Dead(killerID);
			}
			else
			{
				Blink(2f);
			}
		}
	}

	protected override float ModSpeed(float rawSpeed)
	{
		return rawSpeed;
	}

	protected override void ActionUpdate()
	{
		Vector3 localPosition = base.transform.localPosition;
		float deltaTime = Time.deltaTime;
		if (entering < 5f)
		{
			entering += deltaTime;
			if (localPosition.z > 6f)
			{
				localPosition.z -= 2f * deltaTime;
				if (localPosition.z < 6f)
				{
					localPosition.z = 6f;
				}
			}
			switch (level)
			{
			case 1:
				if (entering < 2f)
				{
					if (entering == deltaTime)
					{
						model.GetComponent<Animation>().Play("jump");
						model.GetComponent<Animation>().CrossFadeQueued("idle");
						model.GetComponent<Animation>()["jump"].time = 1f;
					}
					localPosition.y = 10f * (1f - (entering - 1f) * (entering - 1f));
					break;
				}
				localPosition.y = 0f;
				if (entering - deltaTime < 2f)
				{
					Global.Map.Quake(1f);
					GameSound.StartSFX("skillSmash");
					GameObject gameObject2 = Global.Map.AddFX(0, 6, plantStunFX, 2.5f, 8.5f);
					gameObject2.transform.Find("Spore").GetComponent<ParticleSystem>().Stop();
					gameObject2.transform.Find("Dust").GetComponent<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.125f);
				}
				break;
			case 2:
				if (entering == deltaTime)
				{
					model.GetComponent<Animation>().Play("walk");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
					List<Transform> list = new List<Transform>();
					list.Add(model.transform);
					while (list.Count > 0)
					{
						Transform transform = list[0];
						list.RemoveAt(0);
						if (transform.name == "ikHandle18")
						{
							GameObject gameObject3 = UnityEngine.Object.Instantiate(pillarBreath) as GameObject;
							gameObject3.transform.parent = transform;
							gameObject3.transform.localPosition = Vector3.zero;
							gameObject3.transform.localRotation = Quaternion.identity;
							gameObject3.transform.localScale = Vector3.one;
							pillarBreathFX = gameObject3.GetComponentInChildren<ParticleSystem>();
							break;
						}
						for (int num3 = transform.childCount - 1; num3 >= 0; num3--)
						{
							list.Add(transform.GetChild(num3));
						}
					}
					list.Clear();
				}
				else if (entering >= 2f && entering - deltaTime < 2f)
				{
					GameSound.StartSFX("skillRoar");
				}
				localPosition.y = 0f;
				break;
			case 3:
				if (entering == deltaTime)
				{
					model.GetComponent<Animation>().CrossFade("flyFwd");
				}
				else if (entering > 2f && !model.GetComponent<Animation>().IsPlaying("flyBwd"))
				{
					model.GetComponent<Animation>().CrossFade("flyBwd", 0.5f);
				}
				if (entering < 2f)
				{
					localPosition.z = 5.5f + (2f - entering) * 3f;
				}
				else if (entering > 3f)
				{
					localPosition.z = 5.5f + (entering - 3f) * 3f;
				}
				else
				{
					localPosition.z = 5.5f;
				}
				localPosition.y = 1.5f;
				break;
			case 4:
				if (entering < 2f)
				{
					if (entering == Time.deltaTime)
					{
						model.GetComponent<Animation>().Play("jump");
						model.GetComponent<Animation>().CrossFadeQueued("idle");
					}
					localPosition.y = 10f * (1f - (entering - 1f) * (entering - 1f));
					break;
				}
				localPosition.y = 0f;
				if (entering - deltaTime < 2f)
				{
					Global.Map.Quake(1f);
					Global.Map.AddFX(0, 6, lineStunAOE, 2.5f, 6f);
					GameSound.StartSFX("skillSmash");
				}
				break;
			case 5:
				if (entering == deltaTime)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(ringBarrier) as GameObject;
					gameObject.transform.parent = model.transform.parent;
					gameObject.transform.localPosition = new Vector3(0f, 0.0625f, -0.025f);
					gameObject.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
					gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
					Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
					int num = componentsInChildren.Length;
					ringBarrierMat = new Material[num];
					ringBarrierA = new float[num];
					ringBarrierASpd = new float[num];
					ringBarrierAAmp = new float[num];
					for (int i = 0; i < num; i++)
					{
						ringBarrierMat[i] = componentsInChildren[i].material;
						ringBarrierMat[i].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
						ringBarrierA[i] = 0f;
						ringBarrierASpd[i] = -1f;
						ringBarrierAAmp[i] = 1f - Mathf.Max(0f, Mathf.Abs(componentsInChildren[i].transform.localPosition.y) - 0.01f) * 36f;
					}
					ringBarrierNode = gameObject.transform;
					ringBallHand = model.transform.Find("root_ctrl/ctrl/Body/right_shoulder/right_elbow/right_wrist/right_paw/right_middle/right_middlelv2");
					Renderer componentInChildren = GetComponentInChildren<SkinnedMeshRenderer>();
					ringRollinMat = new Material(Shader.Find("Particles/Additive"));
					ringSkinFXMat = UnityEngine.Object.Instantiate(Resources.Load("Skills/05/Materials/plasma")) as Material;
					ringRollinMat.name = componentInChildren.sharedMaterial.name + "(Additive)";
					ringRollinMat.mainTexture = componentInChildren.sharedMaterial.mainTexture;
					ringChargeMat = ringChargeFX.GetComponent<Renderer>().material;
					Color color = new Color(0.5f, 0.5f, 0.5f, 0f);
					ringRollinMat.SetColor("_TintColor", color);
					ringSkinFXMat.SetColor("_TintColor", color);
					ringChargeMat.SetColor("_TintColor", color);
					componentInChildren.materials = new Material[4] { componentInChildren.material, ringRollinMat, ringSkinFXMat, ringChargeMat };
					localPosition.z = 7f;
				}
				else if (entering > 3f && entering - deltaTime < 3f)
				{
					model.GetComponent<Animation>().Play("goIn");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
				}
				if (entering < 3f)
				{
					localPosition.z = 12f;
					localPosition.y = 0f;
				}
				else if (entering < 4f)
				{
					float num2 = 4f - entering;
					localPosition.z = 6f + 7f * num2;
					localPosition.y = 5f * (1f - (num2 * 2f - 1f) * (num2 * 2f - 1f));
				}
				else if (entering - deltaTime < 4f)
				{
					Global.Map.Quake(1f);
					GameSound.StartSFX("skillSmash");
				}
				else if (entering - deltaTime < 5f)
				{
					GameSound.StartSFX("skillBarrierUp");
				}
				break;
			}
			base.transform.localPosition = localPosition;
			return;
		}
		rotating = -1;
		phaseOld = phaseCur;
		phaseCur += deltaTime;
		while (phaseCur >= phaseMax)
		{
			phaseCur -= phaseMax;
		}
		switch (level)
		{
		case 1:
			RunActionTypePlant();
			break;
		case 2:
			RunActionTypePillar();
			break;
		case 3:
			RunActionTypeCharge();
			break;
		case 4:
			RunActionTypeLine();
			break;
		case 5:
			RunActionTypeRing();
			break;
		}
		if (dead)
		{
			return;
		}
		float num4 = model.transform.localRotation.eulerAngles.y;
		float num5 = num4;
		if (rotating == 0)
		{
			num5 = ((!(Mathf.Abs(localPosition.z) > Mathf.Abs(localPosition.x))) ? ((float)((!(localPosition.x > 0f)) ? 90 : 270)) : ((float)((localPosition.z > 0f) ? 180 : 0)));
		}
		else if (rotating == 1)
		{
			for (int num6 = GameCharacter.allChars.Count - 1; num6 >= 0; num6--)
			{
				if (GameCharacter.allChars[num6].Type == Character.Player)
				{
					Vector3 localPosition2 = GameCharacter.allChars[num6].transform.localPosition;
					float num7 = localPosition2.x - localPosition.x;
					float num8 = localPosition2.z - localPosition.z;
					num5 = ((!(Mathf.Abs(localPosition.z) > Mathf.Abs(localPosition.x))) ? ((!(localPosition.x > 0f)) ? (90f - Mathf.Atan(num8 / num7) * 90f / (float)Math.PI) : (270f - Mathf.Atan(num8 / num7) * 90f / (float)Math.PI)) : ((!(localPosition.z > 0f)) ? (0f + Mathf.Atan(num7 / num8) * 90f / (float)Math.PI) : (180f + Mathf.Atan(num7 / num8) * 90f / (float)Math.PI)));
					break;
				}
			}
			if (num5 < 0f)
			{
				num5 += 360f;
			}
		}
		if (num4 == num5)
		{
			return;
		}
		if (Mathf.Abs(num4 - num5) > 180f)
		{
			if (num5 < num4)
			{
				num5 += 360f;
			}
			else
			{
				num4 += 360f;
			}
		}
		float num9 = 90f * deltaTime;
		if (num5 > num4 + num9)
		{
			num5 = num4 + num9;
		}
		else if (num5 < num4 - num9)
		{
			num5 = num4 - num9;
		}
		model.transform.localRotation = Quaternion.Euler(0f, num5, 0f);
	}

	private void RunActionTypePlant()
	{
		if (dead)
		{
			base.transform.localPosition = new Vector3(0f, 0f, 6f);
			return;
		}
		float y = 0f;
		if (phaseOld == 0f || phaseCur < phaseOld)
		{
			bool flag = HardMode && hp <= 5;
			bool flag2 = true;
			Debug.Log("start phase" + plantEnrageStep + " @" + Time.time);
			if (flag && plantEnrageStep < 3)
			{
				if (plantEnrageStep == 0)
				{
					Debug.LogWarning("Boss enrage!!! @" + Time.time);
					model.GetComponent<Animation>().CrossFade("enrage");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
					GameSound.StartSFX("skillRoar");
					Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>();
					Material material = null;
					Renderer[] array = componentsInChildren;
					foreach (Renderer renderer in array)
					{
						if (renderer.sharedMaterial.mainTexture != null)
						{
							material = renderer.material;
							break;
						}
					}
					material.color = new Color(1f, 0.5f, 0.5f, 0.5f);
					Renderer[] array2 = componentsInChildren;
					foreach (Renderer renderer2 in array2)
					{
						renderer2.material = material;
					}
				}
				plantEnrageStep++;
				phaseMax = ((plantEnrageStep != 2) ? 3f : 4.5f);
				rotating = 0;
				flag2 = plantEnrageStep == 3;
			}
			if (flag2)
			{
				bool flag3 = false;
				for (int num = GameCharacter.allChars.Count - 1; num >= 0; num--)
				{
					if (GameCharacter.allChars[num].Type == Character.Player && !GameCharacter.allChars[num].IsDead)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					Debug.LogWarning("Boss throw @" + Time.time);
					model.GetComponent<Animation>().CrossFade("throw");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
					model.GetComponent<Animation>()["throw"].time = 0f;
				}
				if (flag)
				{
					phaseMax = 2f;
				}
				rotating = 1;
			}
		}
		else if (plantEnrageStep > 0 && plantEnrageStep < 3)
		{
			if (plantEnrageStep == 1)
			{
				if (phaseOld < 2f && phaseCur >= 2f)
				{
					Debug.LogWarning("Boss start jump @" + Time.time);
					model.GetComponent<Animation>().CrossFade("jump");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
				}
			}
			else if (phaseCur < 2f)
			{
				y = 10f * (1f - (phaseCur - 1f) * (phaseCur - 1f));
			}
			else if (phaseOld < 2f)
			{
				Vector3 localPosition = base.transform.localPosition;
				for (int num2 = GameCharacter.allChars.Count - 1; num2 >= 0; num2--)
				{
					if (GameCharacter.allChars[num2].Type == Character.Player && !GameCharacter.allChars[num2].IsDead)
					{
						Vector3 localPosition2 = GameCharacter.allChars[num2].transform.localPosition;
						float num3 = localPosition2.x - localPosition.x;
						float num4 = localPosition2.z - localPosition.z;
						if (num3 * num3 + num4 * num4 < 76.5625f)
						{
							GameCharacter.allChars[num2].Stun();
							GameCharacter.allChars[num2].Infect(Virus.SuperSlow, 4.5f);
						}
					}
				}
				Global.Map.Quake(2f);
				Global.Map.AddFX(0, 6, plantStunFX, 2.5f, 8.5f);
				GameSound.StartSFX("skillSmash");
				Debug.Log("Boss land @" + Time.time);
			}
			rotating = 0;
		}
		else
		{
			if (phaseOld < 1f && phaseCur >= 1f && model.GetComponent<Animation>().IsPlaying("throw"))
			{
				Debug.Log("Boss's bomb incoming @" + Time.time);
				for (int num5 = GameCharacter.allChars.Count - 1; num5 >= 0; num5--)
				{
					if (GameCharacter.allChars[num5].Type == Character.Player)
					{
						GameObject gameObject = GameBomb.Create(Bomb.XtraPlant, Fire.Lv1, this, new List<GameCharacter>());
						Global.Map.FlyBomb(gameObject.GetComponent<GameBomb>(), 0f, 6f, GameCharacter.allChars[num5].transform.localPosition);
						GameSound.StartSFX("bombThrow");
					}
				}
			}
			rotating = 1;
		}
		base.transform.localPosition = new Vector3(0f, y, 6f);
	}

	private void RunActionTypePillar()
	{
		float num = 2.5f;
		if (pillarMagicFX != null)
		{
			float num2 = 8 / pillarInc + 4;
			pillarMagicT += Time.deltaTime;
			if (pillarMagicT >= num2 / num)
			{
				UnityEngine.Object.Destroy(pillarMagicFX);
				pillarMagicFX = null;
			}
			else
			{
				float num3 = pillarMagicT * num;
				float num4 = (int)num3 / 2 * 2;
				float num5 = num3 - num4;
				if (num3 < 1f)
				{
					pillarMagicMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.25f * num3 * 2f));
				}
				else if (num3 > num2 - 1f)
				{
					pillarMagicMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.25f * (num2 - num3) * 2f));
				}
				else
				{
					pillarMagicMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.25f * ((!(num5 < 1f)) ? (2f - num5) : num5) + 0.25f));
				}
			}
		}
		if (pillarBombMat != null)
		{
			float num6 = pillarBombT;
			pillarBombT += Time.deltaTime;
			num6 -= (float)(int)num6;
			float num7;
			float a;
			if (pillarBombT < 4f)
			{
				num7 = ((!(num6 < 0.5f)) ? (0.75f - num6 / 4f) : (0.5f + num6 / 4f));
				a = 0.5f;
			}
			else
			{
				num7 = 0.5f + (pillarBombT - 4f) / 2f;
				a = num7;
			}
			pillarBombMat.SetColor("_TintColor", new Color(num7, num7, num7, a));
		}
		if (dead)
		{
			base.transform.localPosition = new Vector3(0f, 0f, 6f);
			return;
		}
		float num8 = 5.5f;
		if (phaseOld == 0f || phaseCur < phaseOld)
		{
			bool flag = HardMode && hp <= 5;
			if (flag && phaseMax > num8)
			{
				phaseMax = num8;
				phaseCur = num8 - 2f;
				pillarBomb = true;
				pillarCount = 8;
				Debug.LogWarning("Boss enrage(roar) @" + Time.time);
				model.GetComponent<Animation>().CrossFade("roarX");
				model.GetComponent<Animation>().CrossFadeQueued("idleX");
				GameSound.StartSFX("skillRoar");
				Material material = null;
				int num9 = 4;
				List<Transform> list = new List<Transform>();
				list.Add(model.transform);
				while (list.Count > 0)
				{
					Transform transform = list[0];
					list.RemoveAt(0);
					if (transform.name == "polySurface18" || transform.name == "polySurface19" || transform.name == "polySurface20" || transform.name == "polySurface21")
					{
						if (material == null)
						{
							material = new Material(Shader.Find("Particles/Additive"));
							material.name = "Boss02_Texture(Clone)";
							material.mainTexture = transform.GetComponent<Renderer>().sharedMaterial.mainTexture;
						}
						GameObject gameObject = UnityEngine.Object.Instantiate(transform.gameObject) as GameObject;
						gameObject.GetComponent<Renderer>().material = material;
						gameObject.transform.parent = transform;
						gameObject.transform.localPosition = Vector3.zero;
						gameObject.transform.localRotation = Quaternion.identity;
						gameObject.transform.localScale = new Vector3(1.02f, 1.02f, 1.02f);
						num9--;
						if (num9 == 0)
						{
							break;
						}
					}
					else
					{
						for (int num10 = transform.childCount - 1; num10 >= 0; num10--)
						{
							list.Add(transform.GetChild(num10));
						}
					}
				}
				list.Clear();
				Global.Map.Quake(2f);
				return;
			}
			pillarInc = ((!flag) ? 1 : 2);
			pillarBomb = flag || UnityEngine.Random.value < (float)pillarBombN / 3f;
			pillarCount = 0;
			pillarIndex = UnityEngine.Random.Range(0, 8);
			pillarSign = ((!(UnityEngine.Random.value < 0.5f)) ? 1 : (-1));
			if (pillarBomb)
			{
				pillarBombN--;
			}
			else
			{
				pillarBombN = 3;
			}
			for (int num11 = GameCharacter.allChars.Count - 1; num11 >= 0; num11--)
			{
				if (GameCharacter.allChars[num11].Type == Character.Player)
				{
					Vector3 localPosition = GameCharacter.allChars[num11].transform.localPosition;
					float num12 = 0f;
					int num13;
					int num14;
					if (pillarBomb)
					{
						num13 = Mathf.RoundToInt(localPosition.x);
						num14 = Mathf.RoundToInt(localPosition.z);
						if (num13 % 2 == 0 || num14 % 2 != 0)
						{
							num12 = GameCharacter.allChars[num11].transform.Find("root").GetChild(0).localRotation.eulerAngles.y;
						}
						if (num13 % 2 == 0)
						{
							num13 = ((num12 > 180f) ? (num13 - 1) : ((num12 > 0f && num12 < 180f) ? (num13 + 1) : ((pillarIndex >= 4) ? (num13 + 1) : (num13 - 1))));
						}
						if (num14 % 2 != 0)
						{
							num14 = ((num12 > 90f && num12 < 270f) ? (num14 - 1) : ((!(num12 < 90f) && !(num12 > 270f)) ? ((pillarIndex >= 2 && pillarIndex < 6) ? (num14 + 1) : (num14 - 1)) : (num14 + 1)));
						}
						int num15 = 5;
						int num16 = 2;
						if (num13 < -num15)
						{
							num13 = -num15;
						}
						else if (num13 > num15)
						{
							num13 = num15;
						}
						if (num14 < -num16)
						{
							num14 = -num16;
						}
						else if (num14 > num16)
						{
							num14 = num16;
						}
						if (Mathf.Abs(num13) == 1 && num14 == 2)
						{
							num13 += ((num13 <= 0) ? (-2) : 2);
						}
					}
					else
					{
						do
						{
							num13 = UnityEngine.Random.Range(-5, 6);
							num14 = UnityEngine.Random.Range(-2, 3);
							if (num13 % 2 == 0)
							{
								num13 = ((pillarIndex >= 4) ? (num13 + 1) : (num13 - 1));
							}
							if (num14 % 2 != 0)
							{
								num14 = ((pillarIndex >= 2 && pillarIndex < 6) ? (num14 + 1) : (num14 - 1));
							}
						}
						while ((num13 >= -3 && num13 <= 3 && num14 == 2) || (num13 >= -1 && num13 <= 1 && num14 == 0) || Mathf.Abs((float)num13 - localPosition.x) > 7f);
					}
					float y = ((!pillarBomb) ? 40 : 0);
					pillars[0] = new Vector3(num13 + 1, y, num14 + 3);
					pillars[1] = new Vector3(num13 + 3, y, num14 + 1);
					pillars[2] = new Vector3(num13 + 3, y, num14 - 1);
					pillars[3] = new Vector3(num13 + 1, y, num14 - 3);
					pillars[4] = new Vector3(num13 - 1, y, num14 - 3);
					pillars[5] = new Vector3(num13 - 3, y, num14 - 1);
					pillars[6] = new Vector3(num13 - 3, y, num14 + 1);
					pillars[7] = new Vector3(num13 - 1, y, num14 + 3);
					Debug.LogWarning("Boss " + ((!pillarBomb) ? "slam" : "shoot") + " @" + Time.time);
					if (pillarBomb)
					{
						if (!flag)
						{
							model.GetComponent<Animation>().CrossFade("magic");
							model.GetComponent<Animation>().CrossFadeQueued("idle");
							model.GetComponent<Animation>()["magic"].time = 0f;
						}
						else
						{
							model.GetComponent<Animation>().CrossFade("magicX");
							model.GetComponent<Animation>().CrossFadeQueued("idleX");
							model.GetComponent<Animation>()["magicX"].time = 0f;
						}
						GameSound.StartSFX("skillMagic");
					}
					else
					{
						model.GetComponent<Animation>().CrossFade("slam");
						model.GetComponent<Animation>().CrossFadeQueued("idle");
					}
					rotating = (pillarBomb ? 1 : 0);
					break;
				}
			}
		}
		else
		{
			int num17 = (int)(phaseOld * num);
			int num18 = (int)(phaseCur * num);
			if (num17 != num18)
			{
				if (num18 <= 2)
				{
					if (pillarBomb)
					{
						int num19 = pillarIndex;
						if (num18 == 1)
						{
							Vector3 vector = (pillars[0] + pillars[4]) / 2f;
							pillarMagicT = 0f;
							pillarMagicFX = Global.Map.AddFX(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z), pillarMagic, 0f, 3f);
							Renderer componentInChildren = pillarMagicFX.GetComponentInChildren<Renderer>();
							if (pillarMagicMat == null)
							{
								pillarMagicMat = componentInChildren.material;
							}
							componentInChildren.material = pillarMagicMat;
							pillarMagicMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
						}
						else if (pillarSign > 0)
						{
							num19++;
							if (num19 > 7)
							{
								num19 = 0;
							}
						}
						else
						{
							num19--;
							if (num19 < 0)
							{
								num19 = 7;
							}
						}
						for (int i = 0; i < pillarInc; i++)
						{
							if (pillars[num19].x >= -8f && pillars[num19].z >= -5f && pillars[num19].x <= 8f && pillars[num19].z <= 5f)
							{
								Global.Map.AddFX((int)pillars[num19].x, (int)pillars[num19].z, pillarRing, 3f / num, 1.25f);
							}
							if (i == 0)
							{
								Debug.Log("Boss shoot @" + Time.time);
								if (phaseMax != num8)
								{
									model.GetComponent<Animation>().CrossFade("magic");
									model.GetComponent<Animation>().CrossFadeQueued("idle");
									model.GetComponent<Animation>()["magic"].time = 0f;
								}
								else
								{
									model.GetComponent<Animation>().CrossFade("magicX");
									model.GetComponent<Animation>().CrossFadeQueued("idleX");
									model.GetComponent<Animation>()["magicX"].time = 0f;
								}
								GameSound.StartSFX("skillMagic");
								num19 += 4;
								num19 %= 8;
							}
						}
					}
				}
				else if (num18 <= 3 && !pillarBomb)
				{
					if (num18 == 3)
					{
						Global.Map.Quake(2f);
						GameSound.StartSFX("skillSmash");
					}
				}
				else if (num18 == 8 && phaseMax == num8)
				{
					Debug.LogWarning("Boss enrage(roar) @" + Time.time);
					model.GetComponent<Animation>().CrossFade("roarX");
					model.GetComponent<Animation>().CrossFadeQueued("idleX");
					GameSound.StartSFX("skillRoar");
				}
				else if (num18 == 10 && phaseMax == num8)
				{
					for (int j = 0; j < 8; j++)
					{
						bool flag2;
						int num20;
						int num21;
						do
						{
							flag2 = false;
							num20 = UnityEngine.Random.Range(-8, 9);
							num21 = UnityEngine.Random.Range(-5, 6);
							if ((num20 % 2 == 0 && num21 % 2 != 0) || (num20 % 2 != 0 && num21 % 2 == 0) || (num21 == 5 && Mathf.Abs(num20) < 2))
							{
								flag2 = true;
								continue;
							}
							for (int num22 = GameCharacter.allChars.Count - 1; num22 >= 0; num22--)
							{
								if (GameCharacter.allChars[num22].Type == Character.Minion)
								{
									Vector3 localPosition2 = GameCharacter.allChars[num22].transform.localPosition;
									if (localPosition2.x == (float)num20 && localPosition2.z == (float)num21)
									{
										flag2 = true;
										break;
									}
								}
							}
						}
						while (flag2);
						GameObject gameObject2 = GameCharacter.Create(Character.Minion, 1, level);
						gameObject2.transform.parent = base.transform.parent;
						gameObject2.transform.localPosition = new Vector3(num20, 40f, num21);
						gameObject2.transform.localRotation = Quaternion.identity;
						gameObject2.transform.localScale = new Vector3(10f, 10f, 10f);
						gameObject2.GetComponent<Rigidbody>().isKinematic = true;
						gameObject2.transform.Find("root").GetChild(0).localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 4) * 90 + 45, 0f);
						GameObject gameObject3 = UnityEngine.Object.Instantiate(pillarFallFX) as GameObject;
						gameObject3.transform.parent = gameObject2.transform;
						gameObject3.transform.localPosition = Vector3.zero;
						gameObject3.transform.localScale = Vector3.one;
					}
				}
				else
				{
					switch (num18)
					{
					case 14:
						if (pillarBomb)
						{
							if (phaseMax != num8)
							{
								Debug.LogWarning("Boss roar @" + Time.time);
								model.GetComponent<Animation>().CrossFade("roar");
								model.GetComponent<Animation>().CrossFadeQueued("idle");
								GameSound.StartSFX("skillRoar");
							}
						}
						else
						{
							Debug.LogWarning("Boss breathe @" + Time.time);
							model.GetComponent<Animation>().CrossFade("breathe");
							model.GetComponent<Animation>().CrossFadeQueued("idle");
							GameSound.StartSFX("skillBreathe");
							pillarBreathFX.Play();
						}
						break;
					case 16:
					{
						if (pillarBomb)
						{
							break;
						}
						for (int m = -5; m <= 5; m++)
						{
							for (int n = -8; n <= 8; n++)
							{
								if (n % 2 == 0 || m % 2 != 0)
								{
									int num31 = n + 8;
									int num32 = m + 5;
									pillarFogs[num31][num32] = Global.Map.AddFX(n, m, pillarFog, 4f / num, 1f);
								}
							}
						}
						break;
					}
					case 19:
					{
						if (pillarBomb)
						{
							break;
						}
						for (int num33 = 0; num33 < 8; num33++)
						{
							if (pillarMobs[num33] != null)
							{
								pillarMobs[num33].Dead();
								pillarMobs[num33] = null;
								GameSound.StartSFX("skillBreak");
							}
						}
						for (int num34 = 0; num34 < 11; num34++)
						{
							for (int num35 = 0; num35 < 17; num35++)
							{
								pillarFogs[num35][num34] = null;
							}
						}
						break;
					}
					default:
					{
						if (pillarCount >= 8)
						{
							break;
						}
						for (int k = 0; k < pillarInc; k++)
						{
							int num23 = pillarIndex;
							if (k == 1)
							{
								num23 += 4;
								num23 %= 8;
							}
							if (!(pillars[num23].x >= -8f) || !(pillars[num23].z >= -5f) || !(pillars[num23].x <= 8f) || !(pillars[num23].z <= 5f))
							{
								continue;
							}
							if (pillarBomb)
							{
								Vector3 vector2 = pillars[num23];
								List<GameItem> list2 = GameItem.List();
								for (int num24 = list2.Count - 1; num24 >= 0; num24--)
								{
									if (!list2[num24].IsDeactived)
									{
										Vector3 localPosition3 = list2[num24].transform.localPosition;
										if (vector2.x == localPosition3.x && vector2.z == localPosition3.z)
										{
											list2[num24].Deactive();
											break;
										}
									}
								}
								List<GameBomb> list3 = GameBomb.List();
								for (int num25 = list3.Count - 1; num25 >= 0; num25--)
								{
									if (!list3[num25].IsExploded && !list3[num25].IsHeld)
									{
										Vector3 localPosition4 = list3[num25].transform.localPosition;
										if (vector2.x == localPosition4.x && vector2.z == localPosition4.z)
										{
											list3[num25].Explode();
											break;
										}
									}
								}
								for (int num26 = GameCharacter.allChars.Count - 1; num26 >= 0; num26--)
								{
									if (GameCharacter.allChars[num26].Type == Character.Player && !GameCharacter.allChars[num26].IsDead)
									{
										Vector3 localPosition5 = GameCharacter.allChars[num26].transform.localPosition;
										float num27 = localPosition5.x - vector2.x;
										float num28 = localPosition5.z - vector2.z;
										if (num27 * num27 + num28 * num28 < 0.5625f)
										{
											GameCharacter.allChars[num26].Dead();
										}
									}
								}
								GameObject gameObject4 = Global.Map.AddBomb(this, Bomb.XtraPillar, (phaseMax != num8) ? Fire.Lv3 : ((Fire)(-3)), false);
								gameObject4.transform.localPosition = pillars[num23];
								pillarBombs[pillarCount + k] = gameObject4;
								Renderer[] componentsInChildren = gameObject4.transform.GetChild(0).Find("IceBomb").GetComponentsInChildren<Renderer>();
								if (pillarBombMat == null)
								{
									pillarBombMat = componentsInChildren[0].material;
								}
								for (int num29 = componentsInChildren.Length - 1; num29 >= 0; num29--)
								{
									componentsInChildren[num29].material = pillarBombMat;
								}
								if (pillarCount == 0)
								{
									pillarBombT = ((phaseMax != num8) ? 0f : 2.5f);
								}
								GameSound.StartSFX("skillPierce");
							}
							else
							{
								GameObject gameObject5 = GameCharacter.Create(Character.Minion, 1, level);
								gameObject5.transform.parent = base.transform.parent;
								gameObject5.transform.localPosition = pillars[pillarIndex];
								gameObject5.transform.localRotation = Quaternion.identity;
								gameObject5.transform.localScale = new Vector3(10f, 10f, 10f);
								gameObject5.GetComponent<Rigidbody>().isKinematic = true;
								pillarMobs[num23] = gameObject5.GetComponent<GameMinion>();
								gameObject5.transform.Find("root").GetChild(0).localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 4) * 90, 0f);
								GameObject gameObject6 = UnityEngine.Object.Instantiate(pillarFallFX) as GameObject;
								gameObject6.transform.parent = gameObject5.transform;
								gameObject6.transform.localPosition = Vector3.zero;
								gameObject6.transform.localScale = Vector3.one;
							}
						}
						if (pillarSign > 0)
						{
							pillarIndex++;
							if (pillarIndex > 7)
							{
								pillarIndex = 0;
							}
						}
						else
						{
							pillarIndex--;
							if (pillarIndex < 0)
							{
								pillarIndex = 7;
							}
						}
						pillarCount += pillarInc;
						if (!pillarBomb || pillarCount >= 8 - pillarInc)
						{
							break;
						}
						int num30 = pillarIndex;
						if (pillarSign > 0)
						{
							num30++;
							if (num30 > 7)
							{
								num30 = 0;
							}
						}
						else
						{
							num30--;
							if (num30 < 0)
							{
								num30 = 7;
							}
						}
						for (int l = 0; l < pillarInc; l++)
						{
							if (pillars[num30].x >= -8f && pillars[num30].z >= -5f && pillars[num30].x <= 8f && pillars[num30].z <= 5f)
							{
								Global.Map.AddFX((int)pillars[num30].x, (int)pillars[num30].z, pillarRing, 3f / num, 1.25f);
							}
							if (l != 0)
							{
								continue;
							}
							if (pillarCount <= 8 - pillarInc * 2)
							{
								Debug.Log("Boss shoot @" + Time.time);
								if (phaseMax != num8)
								{
									model.GetComponent<Animation>().CrossFade("magic");
									model.GetComponent<Animation>().CrossFadeQueued("idle");
									model.GetComponent<Animation>()["magic"].time = 0f;
								}
								else
								{
									model.GetComponent<Animation>().CrossFade("magicX");
									model.GetComponent<Animation>().CrossFadeQueued("idleX");
									model.GetComponent<Animation>()["magicX"].time = 0f;
								}
								GameSound.StartSFX("skillMagic");
							}
							num30 += 4;
							num30 %= 8;
						}
						break;
					}
					}
				}
			}
			if (!pillarBomb && num18 >= 16 && num18 < 19)
			{
				bool flag3 = false;
				for (int num36 = 0; num36 < 8; num36++)
				{
					if (pillarMobs[num36] != null && pillarMobs[num36].IsDead)
					{
						flag3 = true;
						break;
					}
				}
				int num37 = 8;
				int num38 = 5;
				for (int num39 = 0; num39 < 8; num39++)
				{
					if (num37 > (int)pillars[num39].x)
					{
						num37 = (int)pillars[num39].x;
					}
					if (num38 > (int)pillars[num39].z)
					{
						num38 = (int)pillars[num39].z;
					}
				}
				num37++;
				num38++;
				int num40 = num37 + 4;
				int num41 = num38 + 4;
				for (int num42 = -5; num42 <= 5; num42++)
				{
					for (int num43 = -8; num43 <= 8; num43++)
					{
						if (num43 % 2 == 0 || num42 % 2 != 0)
						{
							int num44 = num43 + 8;
							int num45 = num42 + 5;
							if (flag3 || num43 < num37 || num43 > num40 || num42 < num38 || num42 > num41)
							{
								pillarFogs[num44][num45].transform.localPosition = new Vector3(num43, 0f, num42);
							}
							else
							{
								pillarFogs[num44][num45].transform.localPosition = new Vector3(num43, 100f, num42);
							}
						}
					}
				}
				for (int num46 = 0; num46 < 8; num46++)
				{
					if (pillarMobs[num46] != null)
					{
						int num47 = (int)pillars[num46].x + 8;
						int num48 = (int)pillars[num46].z + 5;
						if (pillarMobs[num46].IsDead)
						{
							pillarFogs[num47][num48].transform.localPosition = new Vector3(pillars[num46].x, 0f, pillars[num46].z);
						}
						else
						{
							pillarFogs[num47][num48].transform.localPosition = new Vector3(pillars[num46].x, 100f, pillars[num46].z);
						}
						break;
					}
				}
				if (num18 < 18)
				{
					for (int num49 = GameCharacter.allChars.Count - 1; num49 >= 0; num49--)
					{
						if (GameCharacter.allChars[num49].Type == Character.Player && !GameCharacter.allChars[num49].IsDead)
						{
							Vector3 localPosition6 = GameCharacter.allChars[num49].transform.localPosition;
							int num50 = Mathf.RoundToInt(localPosition6.x);
							int num51 = Mathf.RoundToInt(localPosition6.z);
							if (flag3 || num50 < num37 || num50 > num40 || num51 < num38 || num51 > num41)
							{
								GameCharacter.allChars[num49].Infect(Virus.SuperSlow, 4f);
							}
						}
					}
				}
			}
			if (num18 < 2)
			{
				rotating = (pillarBomb ? 1 : 0);
			}
			else if (num18 >= ((phaseMax != num8) ? 14 : 8) - 1)
			{
				rotating = 0;
			}
		}
		base.transform.localPosition = new Vector3(0f, 0f, 6f);
	}

	private void RunActionTypeCharge()
	{
		if ((model.GetComponent<Animation>().IsPlaying("flyFwd") || model.GetComponent<Animation>().IsPlaying("flyBwd") || model.GetComponent<Animation>().IsPlaying("hover")) && !GameSound.IsPlayingSFX("skillWingClap"))
		{
			GameSound.StartSFX("skillWingClap");
		}
		for (int i = 0; i < 6; i++)
		{
			if (!(chargeBombs[i] != null))
			{
				continue;
			}
			if (chargeBombs[i].IsExploded)
			{
				chargeBombs[i] = null;
				continue;
			}
			Vector3 localPosition = chargeBombs[i].transform.localPosition;
			if (!(localPosition.y <= 0f))
			{
				continue;
			}
			float num = chargeBombT[i];
			chargeBombT[i] += Time.deltaTime;
			if ((int)num == (int)chargeBombT[i] || (int)chargeBombT[i] < 4)
			{
				continue;
			}
			int num2 = (int)chargeBombT[i] - 3;
			if (num2 <= 5)
			{
				int num3 = num2 * (num2 + 1) + 1;
				int num4 = 2;
				for (int j = 1; j < num2; j++)
				{
					num4 *= 2;
				}
				float num5 = (float)(num3 + num4) * 0.5f;
				GameObject gameObject = Global.Map.AddFX((int)localPosition.x, (int)localPosition.z, chargeBombFX, 1f, num5);
				ParticleSystem componentInChildren = gameObject.GetComponentInChildren<ParticleSystem>();
				componentInChildren.startColor = new Color(1f, 1f, 1f, 0.125f + 0.125f * num5 / 31.5f);
				componentInChildren.startSpeed *= num5;
				componentInChildren.emissionRate *= num5;
				Debug.Log("charge:" + num2 + " @" + Time.time);
				GameSound.StartSFX("skillInhale");
			}
		}
		if (chargeAdds != null && phaseCur >= 3f)
		{
			Vector3 localPosition2 = base.transform.localPosition;
			bool flag = Mathf.Abs(localPosition2.z) > Mathf.Abs(localPosition2.x);
			float num6 = (float)(((!flag) ? 17 : 11) - 2) / 6f;
			if (phaseCur < 4f)
			{
				float num7 = (phaseCur - 3f) * 10f * 0.8f;
				for (int num8 = chargeAdds.Length - 1; num8 >= 0; num8--)
				{
					if (chargeAdds[num8] != null)
					{
						if (chargeAdds[num8].transform.localScale.x < 0f)
						{
							GameShadow.RemoveObject(chargeAdds[num8]);
							UnityEngine.Object.Destroy(chargeAdds[num8]);
							chargeAdds[num8] = null;
						}
						else
						{
							Vector3 localPosition3 = chargeAdds[num8].transform.localPosition;
							localPosition3.x = Mathf.Round(localPosition3.x);
							localPosition3.y = 0f;
							localPosition3.z = Mathf.Round(localPosition3.z);
							chargeAdds[num8].transform.localPosition = localPosition3;
							chargeAdds[num8].transform.localScale = new Vector3(num7, num7, num7);
						}
					}
				}
			}
			else if (phaseCur < 4f + num6 + 1f)
			{
				if (phaseOld < 4f)
				{
					for (int num9 = chargeAdds.Length - 1; num9 >= 0; num9--)
					{
						if (chargeAdds[num9] != null)
						{
							chargeAdds[num9].transform.localScale = new Vector3(8f, 8f, 8f);
						}
					}
				}
				else if (phaseOld < 4f + num6 && phaseCur >= 4f + num6)
				{
					for (int num10 = chargeAdds.Length - 1; num10 >= 0; num10--)
					{
						if (chargeAdds[num10] != null && chargeAdds[num10].transform.localScale.x == 8f)
						{
							Vector3 localPosition4 = chargeAdds[num10].transform.localPosition;
							localPosition4.x = Mathf.Round(localPosition4.x);
							localPosition4.y = 0f;
							localPosition4.z = Mathf.Round(localPosition4.z);
							chargeAdds[num10].transform.localPosition = localPosition4;
							chargeAdds[num10].transform.localScale = new Vector3(7.999f, 7.999f, 7.999f);
						}
					}
				}
				for (int num11 = chargeAdds.Length - 1; num11 >= 0; num11--)
				{
					if (chargeAdds[num11] != null)
					{
						Vector3 localPosition5 = chargeAdds[num11].transform.localPosition;
						float x = chargeAdds[num11].transform.localScale.x;
						if (x == 8f)
						{
							if (flag)
							{
								localPosition5.x = Mathf.Round(localPosition5.x);
								localPosition5.y = 0f;
								localPosition5.z = 4f - 6f * (phaseCur - 4f);
								if (localPosition2.z < 0f)
								{
									localPosition5.z *= -1f;
								}
							}
							else
							{
								localPosition5.x = 7f - 6f * (phaseCur - 4f);
								if (localPosition2.x < 0f)
								{
									localPosition5.x *= -1f;
								}
								localPosition5.y = 0f;
								localPosition5.z = Mathf.Round(localPosition5.z);
							}
							chargeAdds[num11].transform.localPosition = localPosition5;
						}
						else
						{
							x -= 8f * Time.deltaTime;
							if (x > 4f)
							{
								chargeAdds[num11].GetComponent<Collider>().enabled = false;
							}
							if (x > 0f)
							{
								if (flag)
								{
									localPosition5.x = Mathf.Round(localPosition5.x);
									localPosition5.y = 0f;
								}
								else
								{
									localPosition5.y = 0f;
									localPosition5.z = Mathf.Round(localPosition5.z);
								}
								chargeAdds[num11].transform.localPosition = localPosition5;
								chargeAdds[num11].transform.localScale = new Vector3(x, x, x);
							}
							else
							{
								GameShadow.RemoveObject(chargeAdds[num11]);
								UnityEngine.Object.Destroy(chargeAdds[num11]);
								chargeAdds[num11] = null;
							}
						}
					}
				}
			}
			else
			{
				for (int num12 = chargeAdds.Length - 1; num12 >= 0; num12--)
				{
					if (chargeAdds[num12] != null)
					{
						GameShadow.RemoveObject(chargeAdds[num12]);
						UnityEngine.Object.Destroy(chargeAdds[num12]);
					}
				}
				chargeAdds = null;
			}
		}
		if (dead)
		{
			Vector3 localPosition6 = base.transform.localPosition;
			if (Mathf.Abs(localPosition6.z) > Mathf.Abs(localPosition6.x))
			{
				localPosition6.x = 0f;
				localPosition6.y = 0f;
			}
			else
			{
				localPosition6.y = 0f;
				localPosition6.z = 0f;
			}
			base.transform.localPosition = localPosition6;
			return;
		}
		float num13 = 5f;
		if (phaseOld == 0f || phaseCur < phaseOld)
		{
			Debug.Log("start phase @" + Time.time);
			bool flag2 = false;
			if (HardMode)
			{
				if (chargeEnrageCount < 1)
				{
					chargeEnrageCount = 1;
				}
				else if (chargeEnrageCount == 1)
				{
					flag2 = hp <= 5;
				}
			}
			if (flag2 || chargeEnrageStep > 0)
			{
				Debug.LogWarning("Boss enrage!!! " + chargeEnrageStep + " (" + chargeEnrageCount + ")");
				if (chargeEnrageStep == 0)
				{
					chargeEnrageCount++;
					Vector3 localPosition7 = base.transform.localPosition;
					if (Mathf.Abs(localPosition7.z) > Mathf.Abs(localPosition7.x))
					{
						chargeEnrageAngle = ((!(localPosition7.z > 0f)) ? 4.712389f : ((float)Math.PI / 2f));
					}
					else
					{
						chargeEnrageAngle = ((!(localPosition7.x > 0f)) ? ((float)Math.PI) : 0f);
					}
					int num14;
					for (int k = 0; k < 3; k++)
					{
						num14 = chargeStormMobs[k].Length;
						for (int l = 0; l < num14; l++)
						{
							GameObject gameObject2 = GameCharacter.Create(Character.Minion, 1, level);
							gameObject2.transform.parent = base.transform.parent;
							gameObject2.transform.localPosition = new Vector3(0f, 10f, 0f);
							gameObject2.transform.localRotation = Quaternion.identity;
							gameObject2.transform.localScale = Vector3.zero;
							gameObject2.GetComponent<SphereCollider>().radius = 3f / 64f;
							chargeStormMobs[k][l] = gameObject2.GetComponent<GameMinion>();
							Global.Map.IgnoreMapCollision(gameObject2);
							for (int m = 0; m < k; m++)
							{
								int num15 = chargeStormMobs[m].Length;
								for (int n = 0; n < num15; n++)
								{
									Physics.IgnoreCollision(gameObject2.GetComponent<Collider>(), chargeStormMobs[m][n].GetComponent<Collider>());
								}
							}
							for (int num16 = 0; num16 < l; num16++)
							{
								Physics.IgnoreCollision(gameObject2.GetComponent<Collider>(), chargeStormMobs[k][num16].GetComponent<Collider>());
							}
						}
					}
					num14 = chargeStormMobs[0].Length / 2;
					for (int num17 = 0; num17 < num14; num17++)
					{
						chargeStormWait[0][num17] = (float)(num14 - num17 - 1) * 5f + 4f + 3f;
						chargeStormTime[0][num17] = -1f;
					}
					num14 = chargeStormMobs[1].Length / 2;
					for (int num18 = 0; num18 < num14; num18++)
					{
						chargeStormWait[1][num18] = (float)(num14 - num18 - 1) * 6f + 2f + 3f;
						chargeStormTime[1][num18] = -3f;
					}
					num14 = chargeStormMobs[2].Length / 2;
					for (int num19 = 0; num19 < num14; num19++)
					{
						chargeStormWait[2][num19] = (float)(num14 - num19 - 1) * 6.5f + 3f;
						chargeStormTime[2][num19] = -5f;
					}
					chargeStormSign = ((UnityEngine.Random.value < 0.5f) ? 1 : (-1));
					chargeStormFX = Global.Map.AddFX(0, 0, chargeStormFXRes, 0f, 1f);
					chargeStormPhaseMax = 20f;
					phaseMax = chargeStormPhaseMax;
					model.GetComponent<Animation>().CrossFade("hover");
				}
				else if (chargeEnrageStep == 1)
				{
					for (int num20 = 0; num20 < 3; num20++)
					{
						for (int num21 = chargeStormWait[num20].Length - 1; num21 >= 0; num21--)
						{
							chargeStormMobs[num20][num21].GetComponentInChildren<ParticleSystem>().Stop();
						}
					}
					phaseMax = 1f;
				}
				else
				{
					for (int num22 = 0; num22 < 3; num22++)
					{
						for (int num23 = chargeStormWait[num22].Length - 1; num23 >= 0; num23--)
						{
							GameShadow.RemoveObject(chargeStormMobs[num22][num23].gameObject);
							UnityEngine.Object.Destroy(chargeStormMobs[num22][num23].gameObject);
							chargeStormMobs[num22][num23] = null;
						}
					}
				}
				chargeEnrageStep++;
				if (chargeEnrageStep == 3)
				{
					chargeEnrageStep = 0;
					phaseMax = 9f;
					phaseCur = phaseMax;
					model.GetComponent<Animation>().CrossFade("flyFwd");
				}
				return;
			}
			chargeAddsRnd = chargeEnrageCount <= 0 && ((!(UnityEngine.Random.value < 1f / 3f)) ? (UnityEngine.Random.value < 0.5f) : (!chargeAddsRnd));
			Vector3 localPosition8 = base.transform.localPosition;
			bool flag3;
			if (Mathf.Abs(localPosition8.z) > Mathf.Abs(localPosition8.x))
			{
				if (UnityEngine.Random.value < 1f / 3f)
				{
					localPosition8.x = 0f;
					localPosition8.y = 0.5f * num13;
					localPosition8.z = ((!(localPosition8.z < 0f)) ? (-12) : 12);
					flag3 = true;
				}
				else
				{
					localPosition8.x = ((!(UnityEngine.Random.value < 0.5f)) ? (-15) : 15);
					localPosition8.y = 0.5f * num13;
					localPosition8.z = 0f;
					flag3 = false;
				}
			}
			else if (UnityEngine.Random.value < 1f / 3f)
			{
				localPosition8.x = ((!(localPosition8.x < 0f)) ? (-15) : 15);
				localPosition8.y = 0.5f * num13;
				localPosition8.z = 0f;
				flag3 = false;
			}
			else
			{
				localPosition8.x = 0f;
				localPosition8.y = 0.5f * num13;
				localPosition8.z = ((!(UnityEngine.Random.value < 0.5f)) ? (-12) : 12);
				flag3 = true;
			}
			if (flag3)
			{
				model.transform.localRotation = Quaternion.Euler(0f, (localPosition8.z > 0f) ? 180 : 0, 0f);
			}
			else
			{
				model.transform.localRotation = Quaternion.Euler(0f, (!(localPosition8.x > 0f)) ? 90 : 270, 0f);
			}
			base.transform.localPosition = localPosition8;
			model.GetComponent<Animation>().CrossFade("flyFwd");
			return;
		}
		if (chargeEnrageStep > 0)
		{
			float num24 = 0f;
			bool flag4 = false;
			if (chargeEnrageStep == 1)
			{
				if (phaseCur >= 4f)
				{
					num24 = 1f;
					flag4 = true;
				}
				else
				{
					if (phaseCur > 1f && phaseCur <= 3f)
					{
						float num25 = (phaseCur - 1f) * 21.25f * 0.8f;
						chargeStormMobs[0][0].transform.localScale = new Vector3(num25, num25, num25);
						chargeStormMobs[0][2].transform.localScale = new Vector3(num25, num25, num25);
					}
					for (int num26 = 0; num26 < 3; num26++)
					{
						for (int num27 = chargeStormWait[num26].Length - 1; num27 >= 0; num27--)
						{
							chargeStormMobs[num26][num27].transform.localPosition = new Vector3(0f, 0f, 0f);
						}
					}
				}
			}
			else
			{
				num24 = 0.5f * (1f - phaseCur) + 0.5f;
				float num28 = 20f * (1f - phaseCur) * 0.8f;
				for (int num29 = 0; num29 < 3; num29++)
				{
					for (int num30 = chargeStormWait[num29].Length - 1; num30 >= 0; num30--)
					{
						chargeStormMobs[num29][num30].transform.localScale = new Vector3(num28, num28, num28);
					}
				}
			}
			if (num24 > 0f)
			{
				float num31 = Time.deltaTime * 0.875f;
				float[] array = new float[3] { 4f, 6f, 8f };
				float[] array2 = new float[3] { 1f, 3f, 5f };
				for (int num32 = 0; num32 < 3; num32++)
				{
					float num33 = num31 * chargeStormSpeeds[num32] * num24;
					float num34 = 4f * (array[num32] + array2[num32]);
					int num35 = chargeStormWait[num32].Length / 2;
					for (int num36 = 0; num36 < num35; num36++)
					{
						if (flag4 && chargeStormWait[num32][num36] > 0f)
						{
							chargeStormWait[num32][num36] -= num33;
							if (chargeStormWait[num32][num36] > 0f)
							{
								chargeStormMobs[num32][num36].transform.localPosition = Vector3.zero;
								chargeStormMobs[num32][num36 + num35].transform.localPosition = Vector3.zero;
								continue;
							}
							chargeStormTime[num32][num36] -= chargeStormWait[num32][num36];
							chargeStormMobs[num32][num36].transform.localScale = new Vector3(16f, 16f, 16f);
							chargeStormMobs[num32][num36 + num35].transform.localScale = new Vector3(16f, 16f, 16f);
							for (int num37 = 0; num37 < 2; num37++)
							{
								GameObject gameObject3 = UnityEngine.Object.Instantiate(chargeStormDust) as GameObject;
								gameObject3.transform.parent = chargeStormMobs[num32][num36 + num35 * num37].transform;
							}
							int num38 = 0;
							for (int num39 = 0; num39 < 3; num39++)
							{
								int num40 = chargeStormWait[num39].Length / 2;
								for (int num41 = 0; num41 < num40; num41++)
								{
									if (chargeStormWait[num39][num41] > 0f)
									{
										num38++;
									}
								}
							}
							float num42 = (20f + (float)num38 * 2.5f) * 0.8f;
							for (int num43 = 0; num43 < 3; num43++)
							{
								int num44 = chargeStormWait[num43].Length / 2;
								for (int num45 = 0; num45 < num44; num45++)
								{
									if (chargeStormWait[num43][num45] > 0f)
									{
										chargeStormMobs[num43][num45].transform.localScale = new Vector3(num42, num42, num42);
										chargeStormMobs[num43][num45 + num44].transform.localScale = new Vector3(num42, num42, num42);
										num42 = 0f;
									}
								}
							}
							if (num38 == 0 && chargeStormFX != null)
							{
								UnityEngine.Object.Destroy(chargeStormFX, 5f);
								chargeStormFX.GetComponentInChildren<ParticleSystem>().Stop();
								chargeStormFX = null;
							}
						}
						chargeStormTime[num32][num36] += num33;
						float num47;
						float num46;
						if (chargeStormTime[num32][num36] < 0f)
						{
							num46 = 0f;
							num47 = 0f - array2[num32] - chargeStormTime[num32][num36];
						}
						else
						{
							float num48;
							for (num48 = chargeStormTime[num32][num36] + array[num32]; num48 >= num34; num48 -= num34)
							{
							}
							if (num48 < array[num32] * 2f)
							{
								num46 = 0f - array[num32] + num48;
								num47 = 0f - array2[num32];
							}
							else if (num48 < array[num32] * 2f + array2[num32] * 2f)
							{
								num46 = array[num32];
								num47 = 0f - array2[num32] + num48 - array[num32] * 2f;
							}
							else if (num48 < array[num32] * 4f + array2[num32] * 2f)
							{
								num46 = array[num32] - num48 + array[num32] * 2f + array2[num32] * 2f;
								num47 = array2[num32];
							}
							else
							{
								num46 = 0f - array[num32];
								num47 = array2[num32] - num48 + array[num32] * 4f + array2[num32] * 2f;
							}
						}
						num46 = ((num32 != 1) ? (num46 * (float)chargeStormSign) : ((0f - num46) * (float)chargeStormSign));
						chargeStormMobs[num32][num36].transform.localPosition = new Vector3(num46, 0f, num47);
						chargeStormMobs[num32][num36 + num35].transform.localPosition = new Vector3(0f - num46, 0f, 0f - num47);
					}
				}
			}
		}
		else
		{
			if ((chargeAddsRnd || chargeEnrageCount > 0) && phaseOld < 3f && phaseCur >= 3f)
			{
				Vector3 localPosition9 = base.transform.localPosition;
				bool flag5 = Mathf.Abs(localPosition9.z) > Mathf.Abs(localPosition9.x);
				chargeAdds = new GameObject[(((!flag5) ? 11 : 17) + 1) / 2];
				for (int num49 = chargeAdds.Length - 1; num49 >= 0; num49--)
				{
					int num50;
					int num51;
					if (flag5)
					{
						num50 = 8 - num49 * 2;
						num51 = 4;
						if (localPosition9.z < 0f)
						{
							num51 *= -1;
						}
					}
					else
					{
						num50 = 7;
						if (localPosition9.x < 0f)
						{
							num50 *= -1;
						}
						num51 = 5 - num49 * 2;
					}
					GameObject gameObject4 = GameCharacter.Create(Character.Minion, 1, level);
					gameObject4.transform.parent = base.transform.parent;
					gameObject4.transform.localPosition = new Vector3(num50, 0f, num51);
					gameObject4.transform.localRotation = Quaternion.identity;
					gameObject4.transform.localScale = Vector3.zero;
					gameObject4.GetComponent<SphereCollider>().radius = 1f / 32f;
					chargeAdds[num49] = gameObject4;
				}
				GameSound.StartSFX("skillRoar");
			}
			bool flag6 = ((chargeEnrageCount >= 2) ? ((phaseOld < 3f && phaseCur >= 3f) || (phaseOld < 6f && phaseCur >= 6f)) : (phaseOld < 3f && phaseCur >= 3f));
			if (flag6 && !chargeAddsRnd)
			{
				GameObject gameObject5 = Global.Map.AddBomb(this, Bomb.XtraCharge, Fire.Lv1, false);
				bool flag7;
				int num52;
				int num53;
				do
				{
					flag7 = true;
					num52 = UnityEngine.Random.Range(-6, 7);
					num53 = UnityEngine.Random.Range(-3, 4);
					if (num52 % 2 != 0 || num53 % 2 == 0)
					{
						continue;
					}
					flag7 = false;
					List<GameBomb> list = GameBomb.List();
					for (int num54 = list.Count - 1; num54 >= 0; num54--)
					{
						Vector3 localPosition10 = list[num54].transform.localPosition;
						if (localPosition10.x == (float)num52 && localPosition10.z == (float)num53)
						{
							flag7 = true;
							break;
						}
					}
				}
				while (flag7);
				gameObject5.transform.localPosition = new Vector3(num52, 50f, num53);
				for (int num55 = 0; num55 < 6; num55++)
				{
					if (chargeBombs[num55] == null)
					{
						chargeBombs[num55] = gameObject5.GetComponent<GameBomb>();
						chargeBombT[num55] = 0.125f;
						break;
					}
				}
			}
		}
		if (chargeEnrageStep > 0)
		{
			chargeEnrageAngle -= (float)chargeStormSign * (float)Math.PI / 2f * Time.deltaTime;
			float num56 = (float)Math.PI * 2f;
			while (chargeEnrageAngle < 0f)
			{
				chargeEnrageAngle += num56;
			}
			while (chargeEnrageAngle >= num56)
			{
				chargeEnrageAngle -= num56;
			}
			float num57 = ((chargeEnrageStep != 1) ? ((phaseCur + chargeStormPhaseMax) / chargeStormPhaseMax) : (phaseCur / phaseMax));
			num57 = ((!(num57 < 0.5f)) ? (1f - 2.5f * (1f - num57)) : (1f - 2.5f * num57));
			float num58 = 10f + num57 * 5f;
			float num59 = 7f + num57 * 5f;
			float x2 = Mathf.Cos(chargeEnrageAngle) * num58;
			float z = Mathf.Sin(chargeEnrageAngle) * num59;
			base.transform.localPosition = new Vector3(x2, num13, z);
			float y = ((chargeStormSign <= 0) ? ((0f - chargeEnrageAngle) * 180f / (float)Math.PI) : (180f - chargeEnrageAngle * 180f / (float)Math.PI));
			model.transform.localRotation = Quaternion.Euler(0f, y, 0f);
		}
		else
		{
			if (chargeEnrageStep != 0)
			{
				return;
			}
			Vector3 localPosition11 = base.transform.localPosition;
			bool flag8 = Mathf.Abs(localPosition11.z) > Mathf.Abs(localPosition11.x);
			if (flag8)
			{
				localPosition11.x = 0f;
				localPosition11.y = 0f;
				localPosition11.z = ((!(localPosition11.z > 0f)) ? (-6) : 6);
			}
			else
			{
				localPosition11.x = ((!(localPosition11.x > 0f)) ? (-9) : 9);
				localPosition11.y = 0f;
				localPosition11.z = 0f;
			}
			rotating = 0;
			float num60 = 0f;
			float num61 = 0f;
			if (phaseCur < 1f)
			{
				num60 = 1f - phaseCur;
				num61 = 0.5f;
			}
			else if (phaseCur < 1.5f)
			{
				num60 = 0f;
				num61 = 0.5f;
			}
			else if (phaseCur < 2f)
			{
				num60 = 0f;
				num61 = 2f - phaseCur;
				if (phaseOld < 1.5f)
				{
					model.GetComponent<Animation>().CrossFade("land");
					model.GetComponent<Animation>().CrossFadeQueued("cast");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
				}
			}
			else if (phaseCur > 8f)
			{
				num60 = phaseCur - 8f;
				num61 = num60;
				if (phaseOld <= 8f)
				{
					model.GetComponent<Animation>().CrossFade("takeOff");
					model.GetComponent<Animation>().CrossFadeQueued("flyBwd");
				}
			}
			if (flag8)
			{
				localPosition11.z += ((!(localPosition11.z > 0f)) ? (0f - num60) : num60) * 6f;
			}
			else
			{
				localPosition11.x += ((!(localPosition11.x > 0f)) ? (0f - num60) : num60) * 6f;
			}
			localPosition11.y = num13 * num61;
			base.transform.localPosition = localPosition11;
		}
	}

	private void RunActionTypeLine()
	{
		if (lineJumpAOE > 0f)
		{
			lineJumpAOE -= Time.deltaTime;
		}
		Vector3 localPosition = base.transform.localPosition;
		bool flag = Mathf.Abs(localPosition.z) > Mathf.Abs(localPosition.x);
		if (dead)
		{
			if (lineFireBreathFX != null)
			{
				ParticleSystem[] componentsInChildren = lineFireBreathFX.GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Stop();
				}
			}
			if (lineFireRage != null)
			{
				ParticleSystem[] componentsInChildren2 = lineFireRage.GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array2 = componentsInChildren2;
				foreach (ParticleSystem particleSystem2 in array2)
				{
					particleSystem2.Stop();
				}
				lineFireRage = null;
			}
			if (flag)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
			}
			else
			{
				localPosition.y = 0f;
				localPosition.z = 0f;
			}
			base.transform.localPosition = localPosition;
			return;
		}
		if (lineLastHP != hp)
		{
			lineLastHP = hp;
			lineHurtDur = 0.25f;
			Global.Map.AddFX(Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z), lineFireHurtFX, 2.5f, 1f);
		}
		if (lineHurtDur > 0f)
		{
			lineHurtDur -= Time.deltaTime;
			if (lineHurtDur < 0f)
			{
				lineHurtDur = 0f;
			}
			float num = 3.5f * (0.25f - lineHurtDur) * 4f;
			float num2 = num * num;
			for (int num3 = GameCharacter.allChars.Count - 1; num3 >= 0; num3--)
			{
				if (GameCharacter.allChars[num3].Type == Character.Player && !GameCharacter.allChars[num3].IsDead)
				{
					Vector3 localPosition2 = GameCharacter.allChars[num3].transform.localPosition;
					float num4 = localPosition2.x - localPosition.x;
					float num5 = localPosition2.z - localPosition.z;
					if (num4 * num4 + num5 * num5 < num2)
					{
						GameCharacter.allChars[num3].Dead();
					}
				}
			}
			List<GameBomb> list = GameBomb.List();
			for (int num6 = list.Count - 1; num6 >= 0; num6--)
			{
				if (!list[num6].IsExploded && !list[num6].IsHeld && list[num6].Type < Bomb.MAX)
				{
					Vector3 localPosition3 = list[num6].transform.localPosition;
					float num7 = localPosition3.x - localPosition.x;
					float num8 = localPosition3.z - localPosition.z;
					if (num7 * num7 + num8 * num8 < num2)
					{
						list[num6].Explode();
					}
				}
			}
			List<GameItem> list2 = GameItem.List();
			for (int num9 = list2.Count - 1; num9 >= 0; num9--)
			{
				if (!list2[num9].IsDeactived)
				{
					Vector3 localPosition4 = list2[num9].transform.localPosition;
					float num10 = localPosition4.x - localPosition.x;
					float num11 = localPosition4.z - localPosition.z;
					if (num10 * num10 + num11 * num11 < num2)
					{
						list2[num9].Deactive();
					}
				}
			}
		}
		bool flag2 = HardMode && hp <= 5;
		float num12 = 0f;
		float num13 = 0f;
		float num14 = 0f;
		bool flag3;
		if (phaseOld == 0f || phaseCur < phaseOld)
		{
			lineRoar = UnityEngine.Random.value < (float)lineRoarCount / 2f;
			lineRoarBall = 0;
			if (lineRoar)
			{
				lineRoarCount--;
			}
			else
			{
				lineRoarCount = 2;
			}
			if (lineRoar)
			{
				Debug.LogWarning("Boss start roar @" + Time.time);
				model.GetComponent<Animation>().CrossFade("meteor");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				GameSound.StartSFX("skillRoar");
			}
			else
			{
				Debug.LogWarning("Boss breathe in @" + Time.time);
				model.GetComponent<Animation>().CrossFade("breathe");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				GameSound.StartSFX("skillInhale");
			}
			flag3 = true;
			rotating = 0;
			lineMaxNum = ((!flag2) ? 3 : 5);
			if (!lineRoar)
			{
				int x = ((!flag) ? ((!(localPosition.x > 0f)) ? (-8) : 8) : 0);
				int y = (flag ? ((!(localPosition.z > 0f)) ? (-5) : 5) : 0);
				Global.Map.AddFX(x, y, lineFireInhale, 1.5f, 1f);
			}
			if (lineFireBreathFX != null)
			{
				UnityEngine.Object.Destroy(lineFireBreathFX);
				lineFireBreathFX = null;
			}
		}
		else if (phaseCur < 2f)
		{
			flag3 = true;
			rotating = 0;
		}
		else if (phaseCur < 6f)
		{
			flag3 = true;
			rotating = 0;
			if (phaseOld < 2f)
			{
				Global.Map.Quake(4f);
				if (!lineRoar)
				{
					GameSound.StartSFX("skillBreathe");
					int x2 = ((!flag) ? ((!(localPosition.x > 0f)) ? (-8) : 8) : 0);
					int y2 = (flag ? ((!(localPosition.z > 0f)) ? (-5) : 5) : 0);
					if (lineMaxNum == 3)
					{
						lineFireBreathFX = Global.Map.AddFX(x2, y2, lineFireBreath, 0f, 1f);
					}
					else
					{
						lineFireBreathFX = Global.Map.AddFX(x2, y2, lineFireBreathX, 0f, 1f);
					}
					if (flag)
					{
						if (localPosition.z > 0f)
						{
							lineFireBreathFX.transform.localPosition += Vector3.up * 0.75f;
						}
					}
					else
					{
						lineFireBreathFX.transform.localPosition += Vector3.up * 0.125f;
					}
					if (flag)
					{
						if (localPosition.z > 0f)
						{
							lineFireBreathFX.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
						}
						else
						{
							lineFireBreathFX.transform.localRotation = Quaternion.Euler(-45f, 0f, 0f);
						}
					}
					else if (localPosition.x > 0f)
					{
						lineFireBreathFX.transform.localRotation = Quaternion.Euler(-22.5f, 270f - ((lineMaxNum != 3) ? 7.5f : 11.25f), 0f);
					}
					else
					{
						lineFireBreathFX.transform.localRotation = Quaternion.Euler(-22.5f, 90f + ((lineMaxNum != 3) ? 7.5f : 11.25f), 0f);
					}
				}
			}
			if (lineRoar)
			{
				float num15 = lineMaxNum * 2;
				int num16 = (int)((phaseCur - 2f) * num15) + 1;
				for (int k = lineRoarBall; k < num16; k++)
				{
					bool flag4;
					int num17;
					int num18;
					do
					{
						flag4 = false;
						num17 = UnityEngine.Random.Range(-8, 9);
						num18 = UnityEngine.Random.Range(-5, 6);
						if (num17 % 2 != 0 && num18 % 2 == 0)
						{
							flag4 = true;
							continue;
						}
						float num19 = (float)num17 - localPosition.x;
						float num20 = (float)num18 - localPosition.z;
						if (num19 * num19 + num20 * num20 < 3.0625f)
						{
							flag4 = true;
							continue;
						}
						for (int num21 = GameCharacter.allChars.Count - 1; num21 >= 0; num21--)
						{
							if (GameCharacter.allChars[num21].Type == Character.Minion)
							{
								Vector3 localPosition5 = GameCharacter.allChars[num21].transform.localPosition;
								if (localPosition5.x == (float)num17 && localPosition5.z == (float)num18)
								{
									flag4 = true;
									break;
								}
							}
						}
					}
					while (flag4);
					GameObject gameObject = GameCharacter.Create(Character.Minion, 1, level);
					gameObject.transform.parent = base.transform.parent;
					gameObject.transform.localPosition = new Vector3(num17, 37.5f, num18);
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
					gameObject.GetComponent<Rigidbody>().isKinematic = true;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(lineFireLandFX) as GameObject;
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localScale = Vector3.one;
					Global.Map.AddFX(num17, num18, lineBombDrop, 2f, 0.75f);
				}
				lineRoarBall = num16;
			}
			else
			{
				for (int l = 0; l < 3; l++)
				{
					int num22 = -1;
					MonoBehaviour monoBehaviour;
					do
					{
						monoBehaviour = null;
						switch (l)
						{
						case 0:
						{
							if (num22 == -1)
							{
								num22 = GameCharacter.allChars.Count;
							}
							for (int num25 = num22 - 1; num25 >= 0; num25--)
							{
								if (GameCharacter.allChars[num25].Type == Character.Player && !GameCharacter.allChars[num25].IsDead)
								{
									monoBehaviour = GameCharacter.allChars[num25];
									num22 = num25;
									break;
								}
							}
							break;
						}
						case 1:
						{
							List<GameBomb> list4 = GameBomb.List();
							if (num22 == -1)
							{
								num22 = list4.Count;
							}
							for (int num24 = num22 - 1; num24 >= 0; num24--)
							{
								if (!list4[num24].IsExploded && !list4[num24].IsHeld)
								{
									monoBehaviour = list4[num24];
									num22 = num24;
									break;
								}
							}
							break;
						}
						case 2:
						{
							List<GameItem> list3 = GameItem.List();
							if (num22 == -1)
							{
								num22 = list3.Count;
							}
							for (int num23 = num22 - 1; num23 >= 0; num23--)
							{
								if (!list3[num23].IsDeactived)
								{
									monoBehaviour = list3[num23];
									num22 = num23;
									break;
								}
							}
							break;
						}
						}
						if (!(monoBehaviour != null))
						{
							continue;
						}
						Vector3 localPosition6 = monoBehaviour.transform.localPosition;
						float num26;
						float num27;
						bool flag5;
						if (flag)
						{
							num26 = localPosition6.x;
							num27 = ((!(localPosition.z > 0f)) ? (localPosition6.z + 5.5f) : (localPosition6.z - 5.5f));
							flag5 = !(Mathf.Abs(num26) > Mathf.Abs(num27)) && Mathf.Abs(num26 / num27) < ((lineMaxNum <= 3) ? 0.5f : 1f);
						}
						else
						{
							num26 = ((!(localPosition.x > 0f)) ? (localPosition6.x + 8.5f) : (localPosition6.x - 8.5f));
							num27 = localPosition6.z;
							flag5 = !(Mathf.Abs(num27) > Mathf.Abs(num26)) && Mathf.Abs(num27 / num26) < ((lineMaxNum <= 3) ? 0.5f : 1f);
						}
						if (!flag5)
						{
							continue;
						}
						float num28 = 25f * (phaseCur - 2f);
						float num29 = num28 * num28;
						if (num26 * num26 + num27 * num27 < num29)
						{
							switch (l)
							{
							case 0:
								((GameCharacter)monoBehaviour).Dead();
								break;
							case 1:
								((GameBomb)monoBehaviour).Explode();
								break;
							case 2:
								((GameItem)monoBehaviour).Deactive();
								break;
							}
						}
					}
					while (monoBehaviour != null);
				}
			}
		}
		else if (phaseCur < 8f)
		{
			flag3 = true;
			rotating = 0;
			lineMaxNum = ((!flag2) ? 3 : 5);
		}
		else if (phaseCur < 9f)
		{
			flag3 = true;
			rotating = 1;
			if (phaseCur >= 8.5f && phaseOld < 8.5f)
			{
				Debug.LogWarning("Boss start shoot @" + Time.time);
				model.GetComponent<Animation>().CrossFade((flag2 && !lineBombRight) ? "throwL" : "throwR");
				lineBombRight = !lineBombRight;
				model.GetComponent<Animation>().CrossFadeQueued("idle");
			}
		}
		else if (phaseCur < 14f)
		{
			flag3 = true;
			rotating = 1;
			float num30 = 5f / (float)lineMaxNum;
			for (int num31 = lineMaxNum - 1; num31 >= 0; num31--)
			{
				float num32 = 8.5f + (float)num31 * num30;
				if (phaseCur >= num32 && phaseOld < num32)
				{
					Debug.LogWarning("Boss start shoot @" + Time.time);
					model.GetComponent<Animation>().CrossFade((flag2 && !lineBombRight) ? "throwL" : "throwR");
					lineBombRight = !lineBombRight;
					model.GetComponent<Animation>().CrossFadeQueued("idle");
					break;
				}
			}
			for (int num33 = lineMaxNum - 1; num33 >= 0; num33--)
			{
				float num34 = 9f + (float)num33 * num30;
				if (phaseCur >= num34 && phaseOld < num34)
				{
					Debug.Log("Boss shoot @" + Time.time);
					for (int num35 = GameCharacter.allChars.Count - 1; num35 >= 0; num35--)
					{
						if (GameCharacter.allChars[num35].Type == Character.Player)
						{
							GameObject gameObject3 = GameBomb.Create(Bomb.XtraLine, Fire.Lv1, this, new List<GameCharacter>());
							Vector3 localPosition7 = GameCharacter.allChars[num35].transform.localPosition;
							if (flag)
							{
								Global.Map.FlyBomb(gameObject3.GetComponent<GameBomb>(), 0f, (!(localPosition.z > 0f)) ? (-5) : 5, localPosition7);
							}
							else
							{
								Global.Map.FlyBomb(gameObject3.GetComponent<GameBomb>(), (!(localPosition.x > 0f)) ? (-8) : 8, 0f, localPosition7);
							}
							GameSound.StartSFX("bombThrow");
							Global.Map.AddFX(Mathf.RoundToInt(localPosition7.x), Mathf.RoundToInt(localPosition7.z), lineBombDrop, 2f, 1.25f);
						}
					}
					break;
				}
			}
		}
		else if (phaseCur < 16f)
		{
			flag3 = true;
			rotating = 0;
			lineMaxNum = ((!flag2) ? 3 : 5);
		}
		else if (phaseCur < 17f)
		{
			flag3 = true;
			rotating = -1;
			if (phaseOld < 16f)
			{
				model.GetComponent<Animation>().CrossFade("jump");
				model.GetComponent<Animation>().CrossFadeQueued("turn");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				lineJumpPos = localPosition;
				lineJumpPos.x = Mathf.Round(lineJumpPos.x);
				lineJumpPos.y = 0f;
				lineJumpPos.z = Mathf.Round(lineJumpPos.z);
				Transform transform = base.transform.parent.parent.Find("Explosions");
				Vector3[] array3 = new Vector3[4]
				{
					new Vector3(9f, 0f, 0f),
					new Vector3(0f, 0f, 6f),
					new Vector3(-9f, 0f, 0f),
					new Vector3(0f, 0f, -6f)
				};
				float[] array4 = new float[4];
				for (int m = 0; m < 4; m++)
				{
					if (array3[m] == lineJumpPos)
					{
						array4[m] = -2f;
						continue;
					}
					bool flag6 = false;
					if (Mathf.Abs(array3[m].z) > Mathf.Abs(array3[m].x))
					{
						int num36 = ((array3[m].z > 0f) ? 1 : (-1));
						int num37 = (int)array3[m].z - 3 * num36;
						while (num37 * num36 > 0)
						{
							for (int num38 = transform.childCount - 1; num38 >= 0; num38--)
							{
								Vector3 localPosition8 = transform.GetChild(num38).transform.localPosition;
								if (Mathf.RoundToInt(localPosition8.x) == 1 && Mathf.RoundToInt(localPosition8.z) == num37)
								{
									flag6 = true;
									break;
								}
							}
							if (flag6)
							{
								break;
							}
							num37 -= 2 * num36;
						}
					}
					else
					{
						int num39 = ((array3[m].x > 0f) ? 1 : (-1));
						int num40 = (int)array3[m].x - 3 * num39;
						while (num40 * num39 > 0)
						{
							for (int num41 = transform.childCount - 1; num41 >= 0; num41--)
							{
								Vector3 localPosition9 = transform.GetChild(num41).transform.localPosition;
								if (Mathf.RoundToInt(localPosition9.x) == num40 && Mathf.RoundToInt(localPosition9.z) == 0)
								{
									flag6 = true;
									break;
								}
							}
							if (flag6)
							{
								break;
							}
							num40 -= 2 * num39;
						}
					}
					if (flag6)
					{
						array4[m] = UnityEngine.Random.value - 2f;
					}
					else
					{
						array4[m] = UnityEngine.Random.value;
					}
				}
				float num42 = -3f;
				int num43 = -1;
				for (int n = 0; n < 4; n++)
				{
					if (num42 < array4[n])
					{
						num42 = array4[n];
						num43 = n;
					}
				}
				lineJumpEnd = array3[num43];
				Debug.Log(string.Concat("jump ", lineJumpPos, "->", lineJumpEnd, " : ", array4[0], ", ", array4[1], ", ", array4[2], ", ", array4[3]));
				Debug.LogWarning("Boss start jump @" + Time.time);
			}
			Vector3 vector = lineJumpEnd - lineJumpPos;
			num14 = ((Mathf.Abs(vector.z) > Mathf.Abs(vector.x)) ? ((!(vector.z < 0f)) ? (0f + Mathf.Atan(vector.x / vector.z) * 180f / (float)Math.PI) : (180f + Mathf.Atan(vector.x / vector.z) * 180f / (float)Math.PI)) : ((!(vector.x < 0f)) ? (90f - Mathf.Atan(vector.z / vector.x) * 180f / (float)Math.PI) : (270f - Mathf.Atan(vector.z / vector.x) * 180f / (float)Math.PI)));
			if (num14 < 0f)
			{
				num14 += 360f;
			}
			num12 = 60f;
		}
		else if (phaseCur < 18f)
		{
			flag3 = false;
			rotating = -1;
			Vector3 vector2 = (lineJumpEnd - lineJumpPos) * (phaseCur - 17f);
			float num44 = 2f * (phaseCur - 17.5f);
			localPosition = lineJumpPos + vector2;
			localPosition.y = 7.5f * (1f - num44 * num44);
		}
		else if (phaseCur < 19f)
		{
			flag3 = true;
			rotating = -1;
			if (phaseOld < 18f)
			{
				Global.Map.Quake((lineMaxNum == 3) ? 1 : 2);
				Global.Map.AddFX((int)lineJumpEnd.x, (int)lineJumpEnd.z, lineStunAOE, 2.5f, 6f);
				GameSound.StartSFX("skillSmash");
				for (int num45 = GameCharacter.allChars.Count - 1; num45 >= 0; num45--)
				{
					if (GameCharacter.allChars[num45].Type == Character.Player && !GameCharacter.allChars[num45].IsDead)
					{
						Vector3 localPosition10 = GameCharacter.allChars[num45].transform.localPosition;
						float num46 = localPosition10.x - lineJumpEnd.x;
						float num47 = localPosition10.z - lineJumpEnd.z;
						if (num46 * num46 + num47 * num47 < 39.0625f)
						{
							GameCharacter.allChars[num45].Stun();
						}
					}
				}
				if (lineMaxNum > 3)
				{
					Global.Map.AddFX((int)lineJumpEnd.x, (int)lineJumpEnd.z, lineFireAOE, 2.5f, 6f);
					lineJumpAOE = 0.25f;
				}
			}
			if (lineMaxNum > 3)
			{
				float num48 = 0f;
				if (phaseCur <= 18.25f)
				{
					num48 = 6.25f * (phaseCur - 18f) * 4f;
				}
				else if (phaseOld <= 18.25f)
				{
					num48 = 6.25f;
				}
				if (num48 > 0f)
				{
					float num49 = num48 * num48;
					for (int num50 = GameCharacter.allChars.Count - 1; num50 >= 0; num50--)
					{
						if (GameCharacter.allChars[num50].Type == Character.Player && !GameCharacter.allChars[num50].IsDead)
						{
							Vector3 localPosition11 = GameCharacter.allChars[num50].transform.localPosition;
							float num51 = localPosition11.x - lineJumpEnd.x;
							float num52 = localPosition11.z - lineJumpEnd.z;
							if (num51 * num51 + num52 * num52 < num49)
							{
								GameCharacter.allChars[num50].Dead();
							}
						}
					}
					List<GameBomb> list5 = GameBomb.List();
					for (int num53 = list5.Count - 1; num53 >= 0; num53--)
					{
						if (!list5[num53].IsExploded && !list5[num53].IsHeld)
						{
							Vector3 localPosition12 = list5[num53].transform.localPosition;
							float num54 = localPosition12.x - lineJumpEnd.x;
							float num55 = localPosition12.z - lineJumpEnd.z;
							if (num54 * num54 + num55 * num55 < num49)
							{
								list5[num53].Explode();
							}
						}
					}
					List<GameItem> list6 = GameItem.List();
					for (int num56 = list6.Count - 1; num56 >= 0; num56--)
					{
						if (!list6[num56].IsDeactived)
						{
							Vector3 localPosition13 = list6[num56].transform.localPosition;
							float num57 = localPosition13.x - lineJumpEnd.x;
							float num58 = localPosition13.z - lineJumpEnd.z;
							if (num57 * num57 + num58 * num58 < num49)
							{
								list6[num56].Deactive();
							}
						}
					}
				}
			}
		}
		else
		{
			flag3 = true;
			rotating = -1;
			Vector3 vector3 = -lineJumpEnd;
			num14 = ((Mathf.Abs(vector3.z) > Mathf.Abs(vector3.x)) ? ((!(vector3.z < 0f)) ? (0f + Mathf.Atan(vector3.x / vector3.z) * 180f / (float)Math.PI) : (180f + Mathf.Atan(vector3.x / vector3.z) * 180f / (float)Math.PI)) : ((!(vector3.x < 0f)) ? (90f - Mathf.Atan(vector3.z / vector3.x) * 180f / (float)Math.PI) : (270f - Mathf.Atan(vector3.z / vector3.x) * 180f / (float)Math.PI)));
			if (num14 < 0f)
			{
				num14 += 360f;
			}
			num12 = 360f;
		}
		if (flag3)
		{
			if (flag)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
				localPosition.z = ((!(localPosition.z > 0f)) ? (-6) : 6);
			}
			else
			{
				localPosition.x = ((!(localPosition.x > 0f)) ? (-9) : 9);
				localPosition.y = 0f;
				localPosition.z = 0f;
			}
		}
		base.transform.localPosition = localPosition;
		if (num12 > 0f)
		{
			num13 = model.transform.localRotation.eulerAngles.y;
			if (Mathf.Abs(num13 - num14) > 180f)
			{
				if (num14 < num13)
				{
					num14 += 360f;
				}
				else
				{
					num13 += 360f;
				}
			}
			float num59 = num12 * Time.deltaTime;
			if (num14 > num13 + num59)
			{
				num14 = num13 + num59;
			}
			else if (num14 < num13 - num59)
			{
				num14 = num13 - num59;
			}
			model.transform.localRotation = Quaternion.Euler(0f, num14, 0f);
		}
		if (flag2 && lineFireRage.transform.parent == null)
		{
			lineFireRage = UnityEngine.Object.Instantiate(lineFireRage) as GameObject;
			lineFireRage.transform.parent = base.transform;
			lineFireRage.transform.localPosition = Vector3.zero;
			lineFireRage.transform.localScale = Vector3.one;
			Global.Map.Quake(1f);
		}
	}

	private void RunActionTypeRing()
	{
		ringRollinMat.SetColor("_TintColor", new Color(0.25f, 0.75f, 0.5f, 0.5f * ringRollinAlpha));
		ringSkinFXMat.SetColor("_TintColor", new Color(0.75f, 0.75f, 0.75f, 0.5f * ringSkinFXAlpha));
		ringChargeMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.75f * ringChargeAlpha));
		ringChargeMat.SetTextureOffset("_MainTex", new Vector2(0f, (float)((int)(Time.time * 16f) % 8) / 8f));
		Vector3 localPosition = base.transform.localPosition;
		bool flag = Mathf.Abs(localPosition.z) > Mathf.Abs(localPosition.x);
		if (dead)
		{
			if (flag)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
			}
			else
			{
				localPosition.y = 0f;
				localPosition.z = 0f;
			}
			base.transform.localPosition = localPosition;
			return;
		}
		float num = ((!HardMode) ? 1f : (1f + (float)(10 - hp) / 9f * 0.5f));
		ringNormalizeHaste = num / 1.5f;
		model.GetComponent<Animation>()["ball"].speed = ringNormalizeHaste;
		model.GetComponent<Animation>()["goIn"].speed = ringNormalizeHaste;
		model.GetComponent<Animation>()["goOut"].speed = ringNormalizeHaste;
		model.GetComponent<Animation>()["idle"].speed = ringNormalizeHaste;
		model.GetComponent<Animation>()["roll"].speed = ringNormalizeHaste;
		model.GetComponent<Animation>()["ulti"].speed = ringNormalizeHaste;
		if (ringBarrierPause > 0f)
		{
			ringBarrierPause -= Time.deltaTime * ringNormalizeHaste;
			if (ringBarrierPause < 0f)
			{
				ringBarrierPause = 0f;
				GameSound.StartSFX("skillBarrierUp");
			}
		}
		for (int num2 = ringBarrierA.Length - 1; num2 >= 0; num2--)
		{
			ringBarrierA[num2] += ringBarrierASpd[num2];
			if (ringBarrierASpd[num2] > 0f)
			{
				if (ringBarrierA[num2] >= 100f)
				{
					ringBarrierA[num2] = 100f;
					ringBarrierASpd[num2] = -2.5f * UnityEngine.Random.value;
				}
			}
			else if (ringBarrierA[num2] <= 0f)
			{
				if (ringBarrierPause > 0.125f)
				{
					ringBarrierA[num2] = 0f;
					ringBarrierASpd[num2] = 0f;
				}
				else
				{
					ringBarrierA[num2] = 20f;
					ringBarrierASpd[num2] = 2.5f * UnityEngine.Random.value;
				}
			}
			float num3 = ((!(ringBarrierA[num2] < 30f)) ? ((!(ringBarrierA[num2] < 50f)) ? ((!(ringBarrierA[num2] > 90f)) ? ((!(ringBarrierA[num2] > 70f)) ? 0.5f : ((ringBarrierA[num2] - 70f) / 40f + 0.5f)) : 1f) : ((ringBarrierA[num2] - 30f) / 40f)) : 0f);
			num3 = (num3 * num3 + num3) / 2f * 0.5f * ringBarrierAAmp[num2];
			ringBarrierMat[num2].SetColor("_TintColor", new Color(0.5f + num3, 0.5f + num3, 0.5f + num3, num3));
		}
		float f = model.transform.localRotation.eulerAngles.y * (float)Math.PI / 180f;
		ringBarrierNode.localPosition = new Vector3(0.0125f * Mathf.Sin(f), 0.0625f, 0.0125f * Mathf.Cos(f));
		if (ringBalls != null)
		{
			ringBallRun += Time.deltaTime;
			if (ringBallRun > ringBallRunMax)
			{
				ringBallRun = ringBallRunMax;
			}
			float num4 = ringBallGrowth * ringBallRun + 3.87f;
			if (num4 == 10f)
			{
				num4 += 0.0001f;
			}
			float num5 = ringBallRunMax - 1.5f;
			float num6 = num5 / 2f;
			for (int i = 0; i < ringBallN; i++)
			{
				if (ringBalls[i] != null && ringBalls[i].GetComponent<GameMinion>().IsDead)
				{
					ringBalls[i] = null;
				}
			}
			if (ringBallRun < num5)
			{
				float num7 = ringBallRun / num5;
				for (int j = 0; j < ringBallN; j++)
				{
					if (ringBalls[j] != null)
					{
						float num8 = ringBallPosit[j].x - ringBallShoot.x;
						float num9 = ringBallPosit[j].z - ringBallShoot.z;
						num8 = ringBallShoot.x + num8 * num7;
						num9 = ringBallShoot.z + num9 * num7;
						float num10 = (ringBallRun - num6) / num6;
						float y = 10f * (1f - num10 * num10);
						ringBalls[j].transform.localPosition = new Vector3(num8, y, num9);
						ringBalls[j].transform.localScale = new Vector3(num4, num4, num4);
					}
				}
			}
			else
			{
				for (int k = 0; k < ringBallN; k++)
				{
					if (ringBalls[k] != null)
					{
						ringBalls[k].transform.localPosition = ringBallPosit[k];
						ringBalls[k].transform.localScale = new Vector3(num4, num4, num4);
					}
				}
			}
			if (ringBallRun == ringBallRunMax)
			{
				ringBalls = null;
			}
		}
		if (ringLightGlowMat != null)
		{
			ringLightGlowT += Time.deltaTime;
			if (ringLightGlowT < 1.5f)
			{
				ringLightGlowMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (1f - Mathf.Cos(ringLightGlowT / 0.75f * (float)Math.PI)) / 4f));
			}
			else
			{
				ringLightGlowMat = null;
			}
		}
		float num11 = 0f;
		float num12 = 0f;
		float num13 = 0f;
		bool flag2;
		if (phaseOld == 0f || phaseCur < phaseOld)
		{
			if (ringActionID == 3)
			{
				Global.Map.Quake(1f);
				GameSound.StartSFX("skillSmash");
				List<GameBomb> list = GameBomb.List();
				for (int num14 = list.Count - 1; num14 >= 0; num14--)
				{
					if (!list[num14].IsExploded && !list[num14].IsHeld)
					{
						Vector3 localPosition2 = list[num14].transform.localPosition;
						float num15 = localPosition2.x - localPosition.x;
						float num16 = localPosition2.z - localPosition.z;
						if (num15 * num15 + num16 * num16 < 3.0625f)
						{
							list[num14].Explode();
							break;
						}
					}
				}
			}
			ringActionID++;
			if (ringActionID > ((!HardMode) ? 1 : 3))
			{
				ringActionID = 0;
			}
			switch (ringActionID)
			{
			case 1:
				phaseMax = 3f / ringNormalizeHaste;
				ringRollPos = localPosition;
				ringRollPos.x = Mathf.Round(ringRollPos.x);
				ringRollPos.y = 0f;
				ringRollPos.z = Mathf.Round(ringRollPos.z);
				if (flag)
				{
					if (UnityEngine.Random.value < 1f / 3f)
					{
						ringRollEnd = ringRollPos;
						ringRollEnd.z *= -1f;
					}
					else
					{
						ringRollEnd = new Vector3(9 * ((UnityEngine.Random.value < 0.5f) ? 1 : (-1)), 0f, 0f);
					}
				}
				else
				{
					ringRollEnd = new Vector3(0f, 0f, 6 * ((UnityEngine.Random.value < 0.5f) ? 1 : (-1)));
				}
				model.GetComponent<Animation>().CrossFade("roll");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				break;
			case 3:
			{
				phaseMax = 6f / ringNormalizeHaste;
				if (flag)
				{
					ringBallRunMax = 3.5f;
					ringBallGrowth = 3.1800003f;
					ringBallN = 6;
					for (int n = 0; n < ringBallN; n++)
					{
						ringBallPosit[n] = new Vector3(8f, 0f, 5 - n * 2);
					}
					for (int num19 = UnityEngine.Random.Range(0, 2); num19 < ringBallN; num19 += 2)
					{
						ringBallPosit[num19].x *= -1f;
					}
				}
				else
				{
					ringBallRunMax = 5.3f;
					ringBallGrowth = 2.1000001f;
					ringBallN = 9;
					for (int num20 = 0; num20 < ringBallN; num20++)
					{
						ringBallPosit[num20] = new Vector3(8 - num20 * 2, 0f, 5f);
					}
					for (int num21 = UnityEngine.Random.Range(0, 2); num21 < ringBallN; num21 += 2)
					{
						ringBallPosit[num21].z *= -1f;
					}
				}
				ringBalls = new GameObject[ringBallN];
				for (int num22 = 0; num22 < ringBallN; num22++)
				{
					GameObject gameObject3 = GameCharacter.Create(Character.Minion, 1, level);
					gameObject3.transform.parent = base.transform.parent;
					gameObject3.transform.localPosition = ringBallShoot;
					gameObject3.transform.localScale = Vector3.zero;
					if (flag)
					{
						gameObject3.transform.Find("root").GetChild(0).localRotation = Quaternion.Euler(0f, (!(ringBallPosit[num22].x > 0f)) ? 90 : 270, 0f);
					}
					else
					{
						gameObject3.transform.Find("root").GetChild(0).localRotation = Quaternion.Euler(0f, (ringBallPosit[num22].z > 0f) ? 180 : 0, 0f);
					}
					Physics.IgnoreCollision(base.transform.Find("root").GetComponent<Collider>(), gameObject3.GetComponent<Collider>());
					for (int num23 = GameCharacter.allChars.Count - 1; num23 >= 0; num23--)
					{
						if (GameCharacter.allChars[num23].gameObject != gameObject3 && GameCharacter.allChars[num23].Type == Character.Minion && !GameCharacter.allChars[num23].IsDead)
						{
							Physics.IgnoreCollision(GameCharacter.allChars[num23].GetComponent<Collider>(), gameObject3.GetComponent<Collider>());
						}
					}
					Global.Map.IgnoreMapCollision(gameObject3);
					ringBalls[num22] = gameObject3;
				}
				ringBallShoot = localPosition;
				ringBallRun = 0f;
				ringChargeAlpha = 0f;
				GameSound.StartSFX("skillPlasmaUlti");
				model.GetComponent<Animation>().CrossFade("goOut");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				break;
			}
			default:
			{
				phaseMax = 9f / ringNormalizeHaste;
				int num17;
				int num18;
				do
				{
					num17 = UnityEngine.Random.Range(-7, 7);
					num18 = UnityEngine.Random.Range(-4, 4);
				}
				while (num17 % 2 != 0 || num18 % 2 == 0);
				GameObject gameObject = Global.Map.AddBomb(this, Bomb.XtraRing, Fire.Lv1, false);
				gameObject.transform.localPosition = new Vector3(num17, 37.5f, num18);
				GameShadow.RemoveObject(gameObject);
				ringLightGlowMat = Global.Map.AddFX(num17, num18, ringLightGlow, 2f, 4f).GetComponent<Renderer>().material;
				ringLightGlowT = 0f;
				ringLightGlowMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				Transform transform = gameObject.transform.Find("root");
				Transform transform2 = transform.transform.Find("Lightning");
				Transform transform3 = transform.transform.Find("PlasmaFX");
				Renderer[] componentsInChildren = transform2.gameObject.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					renderer.material = ringPlasmaMat;
				}
				transform2.GetComponent<Animation>()["idle"].time = 0f;
				transform2.gameObject.SetActive(false);
				GameObject gameObject2 = ringPlasmaRing[ringPlasmaRingIdx++];
				if (ringPlasmaRingIdx == 2)
				{
					ringPlasmaRingIdx = 0;
				}
				for (int m = 0; m < 100; m++)
				{
					Transform child = gameObject2.transform.GetChild(m);
					child.localPosition = Vector3.zero;
					child.localScale = Vector3.one;
					child.GetComponent<Animation>()["idle"].time = 0f;
				}
				ringPlasmaMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
				gameObject2.transform.parent = transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.SetActive(false);
				UnityEngine.Object.Destroy(transform3.gameObject);
				model.GetComponent<Animation>().CrossFade("roar");
				model.GetComponent<Animation>().CrossFadeQueued("idle");
				GameSound.StartSFX("skillRoar");
				GameSound.StopSFX("skillRoll");
				ringRollinAlpha = 0f;
				break;
			}
			}
			flag2 = true;
			rotating = 0;
		}
		else
		{
			switch (ringActionID)
			{
			case 1:
			{
				float num38 = 3f * phaseCur / phaseMax;
				if (num38 < 1f)
				{
					flag2 = true;
					rotating = -1;
					ringRolling = false;
					Vector3 vector3 = ringRollEnd - ringRollPos;
					num13 = ((Mathf.Abs(vector3.z) > Mathf.Abs(vector3.x)) ? ((!(vector3.z < 0f)) ? (0f + Mathf.Atan(vector3.x / vector3.z) * 180f / (float)Math.PI) : (180f + Mathf.Atan(vector3.x / vector3.z) * 180f / (float)Math.PI)) : ((!(vector3.x < 0f)) ? (90f - Mathf.Atan(vector3.z / vector3.x) * 180f / (float)Math.PI) : (270f - Mathf.Atan(vector3.z / vector3.x) * 180f / (float)Math.PI)));
					if (num13 < 0f)
					{
						num13 += 360f;
					}
					num11 = 60f;
					ringRollinAlpha = num38;
				}
				else if (num38 < 2f)
				{
					flag2 = false;
					rotating = -1;
					ringRolling = true;
					localPosition = ringRollPos + (ringRollEnd - ringRollPos) * (num38 - 1f);
					List<GameBomb> list2 = GameBomb.List();
					for (int num39 = list2.Count - 1; num39 >= 0; num39--)
					{
						if (!list2[num39].IsExploded && !list2[num39].IsHeld)
						{
							Vector3 localPosition5 = list2[num39].transform.localPosition;
							float num40 = localPosition5.x - localPosition.x;
							float num41 = localPosition5.z - localPosition.z;
							if (num40 * num40 + num41 * num41 < 3.0625f)
							{
								list2[num39].Explode();
								break;
							}
						}
					}
					ringRollinAlpha = 1f;
					if (3f * phaseOld / phaseMax < 1f)
					{
						GameSound.StartSFX("skillRoll");
					}
				}
				else
				{
					flag2 = true;
					rotating = -1;
					ringRolling = false;
					Vector3 vector4 = -ringRollEnd;
					num13 = ((Mathf.Abs(vector4.z) > Mathf.Abs(vector4.x)) ? ((!(vector4.z < 0f)) ? (0f + Mathf.Atan(vector4.x / vector4.z) * 180f / (float)Math.PI) : (180f + Mathf.Atan(vector4.x / vector4.z) * 180f / (float)Math.PI)) : ((!(vector4.x < 0f)) ? (90f - Mathf.Atan(vector4.z / vector4.x) * 180f / (float)Math.PI) : (270f - Mathf.Atan(vector4.z / vector4.x) * 180f / (float)Math.PI)));
					if (num13 < 0f)
					{
						num13 += 360f;
					}
					num11 = 360f;
					ringRollinAlpha = 3f - num38;
				}
				break;
			}
			case 3:
			{
				float num42 = 6f * phaseOld / phaseMax;
				float num43 = 6f * phaseCur / phaseMax;
				float num44 = -1f;
				if (num43 < 1f)
				{
					num44 = num43;
				}
				else if (num43 > 5f)
				{
					num44 = 6f - num43;
				}
				if ((int)num42 != (int)num43)
				{
					if ((int)num42 == 0)
					{
						if (ringBallN == 6)
						{
							localPosition.x = 0f;
							localPosition.y = 0f;
							localPosition.z = 12f;
							if (UnityEngine.Random.value < 0.5f)
							{
								localPosition.z *= -1f;
							}
						}
						else
						{
							localPosition.x = 15f;
							localPosition.y = 0f;
							localPosition.z = 0f;
							if (UnityEngine.Random.value < 0.5f)
							{
								localPosition.x *= -1f;
							}
						}
					}
					else if ((int)num42 == 4)
					{
						model.GetComponent<Animation>().CrossFade("goIn");
						model.GetComponent<Animation>().CrossFadeQueued("idle");
					}
				}
				if (num43 < 0.25f)
				{
					ringSkinFXAlpha = 2f - num43 * 8f;
				}
				else
				{
					ringSkinFXAlpha = 0f;
				}
				if (num44 >= 0f)
				{
					float num45 = 7f;
					float num46 = 7f;
					if (flag)
					{
						localPosition.x = 0f;
						localPosition.z = (float)((localPosition.z > 0f) ? 1 : (-1)) * (6f + num46 * num44);
					}
					else
					{
						localPosition.x = (float)((localPosition.x > 0f) ? 1 : (-1)) * (9f + num45 * num44);
						localPosition.z = 0f;
					}
					localPosition.y = 5f * (1f - (num44 * 2f - 1f) * (num44 * 2f - 1f));
				}
				flag2 = false;
				rotating = 0;
				break;
			}
			default:
			{
				float num24 = 9f * phaseOld / phaseMax;
				float num25 = 9f * phaseCur / phaseMax;
				if (num25 > 4f && num24 <= 4f)
				{
					ringBallBall = GameCharacter.Create(Character.Minion, 1, level);
					ringBallBall.transform.parent = ringBallHand;
					ringBallBall.transform.localPosition = Vector3.zero;
					ringBallBall.transform.localRotation = Quaternion.identity;
					ringBallBall.transform.localScale = Vector3.zero;
					Physics.IgnoreCollision(base.transform.Find("root").GetComponent<Collider>(), ringBallBall.GetComponent<Collider>());
					for (int num26 = GameCharacter.allChars.Count - 1; num26 >= 0; num26--)
					{
						if (GameCharacter.allChars[num26].gameObject != ringBallBall && GameCharacter.allChars[num26].Type == Character.Minion && !GameCharacter.allChars[num26].IsDead)
						{
							Physics.IgnoreCollision(GameCharacter.allChars[num26].GetComponent<Collider>(), ringBallBall.GetComponent<Collider>());
						}
					}
					model.GetComponent<Animation>().CrossFade("ball");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
				}
				else if (num25 > 6f && num24 <= 6f)
				{
					Vector3 vector = ringBallHand.position / 10f;
					vector.x = Mathf.Round(vector.x);
					vector.y = 0f;
					vector.z = Mathf.Round(vector.z);
					if (flag)
					{
						if (vector.x < -1f)
						{
							vector.x = -1f;
						}
						else if (vector.x > 1f)
						{
							vector.x = 1f;
						}
						vector.z = ((!(vector.z < 0f)) ? 5 : (-5));
					}
					else
					{
						if (vector.z < -1f)
						{
							vector.z = -1f;
						}
						else if (vector.z > 1f)
						{
							vector.z = 1f;
						}
						vector.x = ((!(vector.x < 0f)) ? 8 : (-8));
					}
					Vector3[] array2 = (flag ? ((vector.x != 0f) ? new Vector3[2]
					{
						new Vector3(1f, 0f, 0f),
						new Vector3(-1f, 0f, 0f)
					} : new Vector3[3]
					{
						new Vector3(2f, 0f, 0f),
						new Vector3(-2f, 0f, 0f),
						new Vector3(0f, 0f, (!(localPosition.z > 0f)) ? 2 : (-2))
					}) : ((vector.z == 0f) ? new Vector3[2]
					{
						new Vector3(0f, 0f, 1f),
						new Vector3(0f, 0f, -1f)
					} : new Vector3[3]
					{
						new Vector3(0f, 0f, 2f),
						new Vector3(0f, 0f, -2f),
						new Vector3((!(localPosition.x > 0f)) ? 2 : (-2), 0f, 0f)
					}));
					for (int num27 = GameCharacter.allChars.Count - 1; num27 >= 0; num27--)
					{
						if (GameCharacter.allChars[num27].Type == Character.Player)
						{
							Vector3 localPosition3 = GameCharacter.allChars[num27].transform.localPosition;
							float num28 = 1000f;
							int num29 = -1;
							for (int num30 = array2.Length - 1; num30 >= 0; num30--)
							{
								Vector3 vector2 = vector + array2[num30];
								float num31 = localPosition3.x - vector2.x;
								float num32 = localPosition3.z - vector2.z;
								float num33 = num31 * num31 + num32 * num32;
								if (num28 > num33)
								{
									num28 = num33;
									num29 = num30;
								}
							}
							ringBallBall.transform.parent = base.transform.parent;
							ringBallBall.transform.localPosition = vector;
							ringBallBall.transform.localRotation = Quaternion.identity;
							ringBallBall.transform.localScale = new Vector3(10f, 10f, 10f);
							Direction direction = ((!(Mathf.Abs(array2[num29].z) > Mathf.Abs(array2[num29].x))) ? ((!(array2[num29].x < 0f)) ? Direction.Right : Direction.Left) : ((array2[num29].z < 0f) ? Direction.Down : Direction.Up));
							ringBallBall.transform.Find("root").GetChild(0).transform.localRotation = Quaternion.Euler(0f, (float)direction, 0f);
							ringBallBall = null;
							break;
						}
					}
				}
				if (ringBallBall != null)
				{
					float num34 = (num25 - 4f) * 0.2f;
					ringBallBall.transform.localPosition = Vector3.zero;
					ringBallBall.transform.localRotation = Quaternion.identity;
					ringBallBall.transform.localScale = new Vector3(num34, num34, num34);
				}
				flag2 = true;
				rotating = 1;
				if (ringActionID != 2 || !(num25 > 7f))
				{
					break;
				}
				if (num24 <= 7f)
				{
					Debug.Log("Boss is going to use Ulti");
					model.GetComponent<Animation>().CrossFade("ulti");
					model.GetComponent<Animation>().CrossFadeQueued("idle");
					ringChargeAlpha = 1f;
				}
				if (num25 < 9.875f - ringNormalizeHaste)
				{
					float num35 = (int)(40f * num24);
					float num36 = (int)(40f * num25);
					if (num35 != num36)
					{
						GameObject gameObject4 = UnityEngine.Object.Instantiate(ringChargeFX) as GameObject;
						gameObject4.name = "ChargeFX";
						gameObject4.transform.parent = base.transform;
						gameObject4.transform.localPosition = new Vector3((UnityEngine.Random.value - 0.5f) / 50f, 0.06f, (UnityEngine.Random.value - 0.5f) / 50f);
						gameObject4.transform.localRotation = Quaternion.Euler(10 * UnityEngine.Random.Range(3, 10), 0f, 10 * UnityEngine.Random.Range(-12, 13));
						gameObject4.transform.localScale = new Vector3(0.5f + UnityEngine.Random.value, 0.5f + UnityEngine.Random.value, 1f);
						gameObject4.GetComponent<Renderer>().material = ringChargeMat;
						UnityEngine.Object.Destroy(gameObject4, 0.125f);
					}
				}
				for (int num37 = base.transform.childCount - 1; num37 >= 0; num37--)
				{
					Transform child2 = base.transform.GetChild(num37);
					if (child2.name == "ChargeFX")
					{
						Vector3 localPosition4 = child2.localPosition;
						localPosition4.y = 0f;
						Vector3 eulerAngles = child2.localRotation.eulerAngles;
						eulerAngles += localPosition4 * 5000f * Time.deltaTime;
						child2.localRotation = Quaternion.Euler(eulerAngles);
					}
				}
				ringSkinFXAlpha = num25 - 7f;
				break;
			}
			}
		}
		if (flag2)
		{
			if (flag)
			{
				localPosition.x = 0f;
				localPosition.y = 0f;
				localPosition.z = ((!(localPosition.z > 0f)) ? (-6) : 6);
			}
			else
			{
				localPosition.x = ((!(localPosition.x > 0f)) ? (-9) : 9);
				localPosition.y = 0f;
				localPosition.z = 0f;
			}
		}
		base.transform.localPosition = localPosition;
		if (!(num11 > 0f))
		{
			return;
		}
		num12 = model.transform.localRotation.eulerAngles.y;
		if (Mathf.Abs(num12 - num13) > 180f)
		{
			if (num13 < num12)
			{
				num13 += 360f;
			}
			else
			{
				num12 += 360f;
			}
		}
		float num47 = num11 * Time.deltaTime * ringNormalizeHaste;
		if (num13 > num12 + num47)
		{
			num13 = num12 + num47;
		}
		else if (num13 < num12 - num47)
		{
			num13 = num12 - num47;
		}
		model.transform.localRotation = Quaternion.Euler(0f, num13, 0f);
	}
}
