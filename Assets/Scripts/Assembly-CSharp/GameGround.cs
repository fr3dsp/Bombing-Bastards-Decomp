using System;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class GameGround : MonoBehaviour
{
	private static GameObject[] res;

	private static int resLv;

	private static TNet.List<GameGround> allGrounds;

	private Ground type;

	private int param;

	private Material material;

	private float lifeTime;

	private int crackStep;

	private float windMaxT;

	private bool pullGrow;

	private float pullSize;

	private Dictionary<int, Vector3> nearObj;

	private float sendDelay;

	public Ground Type
	{
		get
		{
			return type;
		}
	}

	private void Awake()
	{
		allGrounds.Add(this);
	}

	private void OnDestroy()
	{
		allGrounds.Remove(this);
	}

	[RFC(73)]
	private void GameGroundOnDestroy(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		int num = 0;
		GameGround gameGround = null;
		Vector3 localPosition = base.transform.localPosition;
		switch (type)
		{
		case Ground.TimeOut:
		{
			if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
			{
				break;
			}
			if (lifeTime < 1.875f)
			{
				lifeTime += Time.deltaTime;
				if (lifeTime >= 1.875f)
				{
					lifeTime = 1.875f;
					base.name = "Explosion";
					base.transform.parent = base.transform.parent.parent.Find("Explosions");
					SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
					sphereCollider.center = new Vector3(0f, 0.025f, 0f);
					sphereCollider.radius = 0.025f;
					Global.Map.Explode(-1, Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z), -1, false, Direction.None, null, 1f);
					num = 1;
				}
				break;
			}
			TNet.List<GameBomb> list11 = GameBomb.List();
			for (int num65 = list11.Count - 1; num65 >= 0; num65--)
			{
				if (!list11[num65].IsExploded && !list11[num65].IsHeld)
				{
					Vector3 localPosition11 = list11[num65].transform.localPosition;
					if (Mathf.Abs(localPosition.x - localPosition11.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition11.z) < 0.5f && localPosition11.y < 0.5f)
					{
						list11[num65].Explode();
						break;
					}
				}
			}
			TNet.List<GameItem> list12 = GameItem.List();
			for (int num66 = list12.Count - 1; num66 >= 0; num66--)
			{
				if (!list12[num66].IsDeactived)
				{
					Vector3 localPosition12 = list12[num66].transform.localPosition;
					if (Mathf.Abs(localPosition.x - localPosition12.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition12.z) < 0.5f && localPosition12.y < 0.5f)
					{
						list12[num66].Deactive();
						break;
					}
				}
			}
			break;
		}
		case Ground.Virus:
		{
			bool flag2 = false;
			bool flag3 = false;
			if (lifeTime < 15f)
			{
				lifeTime += Time.deltaTime;
				TNet.List<GameCharacter> list13 = GameCharacter.List();
				for (int num67 = list13.Count - 1; num67 >= 0; num67--)
				{
					if (!list13[num67].IsDead && list13[num67].Type == Character.Player)
					{
						Vector3 localPosition13 = list13[num67].transform.localPosition;
						if (Mathf.Abs(localPosition.x - localPosition13.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition13.z) < 0.5f)
						{
							switch (param)
							{
							case 1:
								list13[num67].Infect(Virus.CrazySpeed, 2f);
								break;
							case 2:
								list13[num67].Infect(Virus.SuperSlow, 4f);
								break;
							case 3:
								list13[num67].Infect(Virus.Unarmed, 5f);
								break;
							case 4:
								list13[num67].Infect(Virus.Confusing, 5f);
								break;
							case 5:
								list13[num67].Stun();
								switch (UnityEngine.Random.Range(0, 4))
								{
								case 0:
									list13[num67].Infect(Virus.CrazySpeed, 4f);
									break;
								case 1:
									list13[num67].Infect(Virus.SuperSlow, 6f);
									break;
								case 2:
									list13[num67].Infect(Virus.Unarmed, 7f);
									break;
								case 3:
									list13[num67].Infect(Virus.Confusing, 7f);
									break;
								}
								break;
							}
							lifeTime = 15f;
							flag3 = true;
							break;
						}
					}
				}
				if (lifeTime >= 15f)
				{
					flag2 = true;
				}
			}
			else
			{
				lifeTime += Time.deltaTime;
				if (lifeTime > 20f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			bool flag4 = false;
			switch (param)
			{
			case 1:
			{
				flag4 = true;
				float time3 = Time.time;
				int num69 = 0;
				time3 -= (float)(int)time3;
				time3 *= 4f;
				num69 = (int)time3;
				material.SetTextureOffset("_MainTex", new Vector2(0.5f * (float)(num69 % 2), 0.5f * (float)(num69 / 2)));
				break;
			}
			case 2:
				if (flag2)
				{
					GetComponentInChildren<ParticleSystem>().Stop();
				}
				break;
			case 3:
			{
				if (!flag2)
				{
					break;
				}
				ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem particleSystem in array)
				{
					if (particleSystem.isPlaying)
					{
						particleSystem.Stop();
					}
					else if (flag3)
					{
						particleSystem.Play();
					}
				}
				break;
			}
			case 4:
			{
				flag4 = true;
				float time2 = Time.time;
				int num68 = 0;
				time2 -= (float)(int)time2;
				time2 *= 4f;
				num68 = (int)time2;
				material.SetTextureOffset("_MainTex", new Vector2(0.5f * (float)(num68 % 2), 0.5f * (float)(num68 / 2)));
				if (flag2)
				{
					GetComponentInChildren<ParticleSystem>().Stop();
				}
				break;
			}
			}
			if (flag4)
			{
				float num70 = ((lifeTime < 0.5f) ? (5f * lifeTime * 2f) : ((!(lifeTime > 15f)) ? 5f : (5f * Mathf.Max(0f, 1f - (lifeTime - 15f) * 2f))));
				base.transform.localScale = new Vector3(5f + num70, 5f + num70, 5f + num70);
				material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num70 / 5f));
			}
			break;
		}
		case Ground.Crack:
			if (crackStep < 2)
			{
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					float num71 = ((crackStep != 0) ? 1f : 0.5f);
					bool flag5 = false;
					TNet.List<GameCharacter> list14 = GameCharacter.List();
					for (int num72 = list14.Count - 1; num72 >= 0; num72--)
					{
						if (!list14[num72].IsDead && (list14[num72].Type == Character.Player || list14[num72].Type == Character.AI))
						{
							Vector3 localPosition14 = list14[num72].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition14.x) < num71 && Mathf.Abs(localPosition.z - localPosition14.z) < num71 && localPosition14.y < 0.25f)
							{
								flag5 = true;
								break;
							}
						}
					}
					if (!flag5)
					{
						TNet.List<GameBomb> list15 = GameBomb.List();
						for (int num73 = list15.Count - 1; num73 >= 0; num73--)
						{
							if (!list15[num73].IsExploded && !list15[num73].IsHeld)
							{
								Vector3 localPosition15 = list15[num73].transform.localPosition;
								if (Mathf.Abs(localPosition.x - localPosition15.x) < num71 && Mathf.Abs(localPosition.z - localPosition15.z) < num71 && localPosition15.y < 0.25f)
								{
									flag5 = true;
									break;
								}
							}
						}
					}
					if (!flag5)
					{
						TNet.List<GameItem> list16 = GameItem.List();
						for (int num74 = list16.Count - 1; num74 >= 0; num74--)
						{
							if (!list16[num74].IsDeactived)
							{
								Vector3 localPosition16 = list16[num74].transform.localPosition;
								if (Mathf.Abs(localPosition.x - localPosition16.x) < num71 && Mathf.Abs(localPosition.z - localPosition16.z) < num71 && localPosition16.y < 0.25f)
								{
									flag5 = true;
									break;
								}
							}
						}
					}
					if (crackStep == 0)
					{
						if (flag5)
						{
							crackStep = 1;
							num = crackStep;
						}
					}
					else if (!flag5)
					{
						crackStep = 2;
						num = crackStep;
						BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
						boxCollider.center = new Vector3(0f, 0.05f, 0f);
						boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
					}
				}
			}
			else
			{
				if (crackStep == 2 && (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting))
				{
					lifeTime += Time.deltaTime;
					if (lifeTime >= 0.5f)
					{
						crackStep = 3;
						num = crackStep;
					}
				}
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					TNet.List<GameBomb> list17 = GameBomb.List();
					for (int num75 = list17.Count - 1; num75 >= 0; num75--)
					{
						if (!list17[num75].IsExploded && !list17[num75].IsHeld)
						{
							Vector3 localPosition17 = list17[num75].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition17.x) < 0.25f && Mathf.Abs(localPosition.z - localPosition17.z) < 0.25f && localPosition17.y < 0.25f)
							{
								if (Global.Mode == GameMode.OnlineBattle)
								{
									list17[num75].GetComponent<TNObject>().Send(65, Target.Others, Global.onlineMatchID);
								}
								GameShadow.RemoveObject(list17[num75].gameObject);
								UnityEngine.Object.Destroy(list17[num75].gameObject);
							}
						}
					}
					TNet.List<GameItem> list18 = GameItem.List();
					for (int num76 = list18.Count - 1; num76 >= 0; num76--)
					{
						if (!list18[num76].IsDeactived)
						{
							Vector3 localPosition18 = list18[num76].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition18.x) < 0.25f && Mathf.Abs(localPosition.z - localPosition18.z) < 0.25f && localPosition18.y < 0.25f)
							{
								if (Global.Mode == GameMode.OnlineBattle)
								{
									list18[num76].GetComponent<TNObject>().Send(53, Target.Others, Global.onlineMatchID);
								}
								GameShadow.RemoveObject(list18[num76].gameObject);
								UnityEngine.Object.Destroy(list18[num76].gameObject);
							}
						}
					}
				}
			}
			material.SetTextureOffset("_MainTex", new Vector2(0.5f * (float)(crackStep % 2), 0.5f * (float)(crackStep / 2)));
			break;
		case Ground.Wind:
		{
			if (lifeTime >= windMaxT)
			{
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					if (Global.Mode == GameMode.OnlineBattle)
					{
						GetComponent<TNObject>().Send(73, Target.Others, Global.onlineMatchID);
					}
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
			lifeTime += Time.deltaTime;
			float num54 = 3.3333333f;
			float x4 = 0f;
			float y = 0f;
			switch ((Direction)param)
			{
			case Direction.Left:
				x4 = 0f - num54;
				break;
			case Direction.Down:
				y = 0f - num54;
				break;
			case Direction.Right:
				x4 = num54;
				break;
			case Direction.Up:
				y = num54;
				break;
			}
			Vector2 externalForce4 = new Vector2(x4, y);
			TNet.List<GameCharacter> list8 = GameCharacter.List();
			for (int num55 = list8.Count - 1; num55 >= 0; num55--)
			{
				if (!list8[num55].IsDead)
				{
					Vector3 localPosition8 = list8[num55].transform.localPosition;
					float num56 = localPosition8.x - localPosition.x;
					float num57 = localPosition8.z - localPosition.z;
					if (num56 >= -0.5f && num56 < 0.5f && num57 >= -0.5f && num57 < 0.5f && localPosition8.y == 0f)
					{
						list8[num55].externalForce = externalForce4;
					}
				}
			}
			if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				TNet.List<GameBomb> list9 = GameBomb.List();
				for (int num58 = list9.Count - 1; num58 >= 0; num58--)
				{
					if (!list9[num58].IsExploded && !list9[num58].IsHeld && !list9[num58].IsFlying)
					{
						Vector3 localPosition9 = list9[num58].transform.localPosition;
						float num59 = localPosition9.x - localPosition.x;
						float num60 = localPosition9.z - localPosition.z;
						if (num59 >= -0.5f && num59 < 0.5f && num60 >= -0.5f && num60 < 0.5f && localPosition9.y == 0f)
						{
							list9[num58].transform.localPosition = localPosition;
							list9[num58].Fly((Direction)param, 2);
						}
					}
				}
				TNet.List<GameItem> list10 = GameItem.List();
				for (int num61 = list10.Count - 1; num61 >= 0; num61--)
				{
					if (!list10[num61].IsDeactived)
					{
						Vector3 localPosition10 = list10[num61].transform.localPosition;
						float num62 = localPosition10.x - localPosition.x;
						float num63 = localPosition10.z - localPosition.z;
						if (num62 >= -0.5f && num62 < 0.5f && num63 >= -0.5f && num63 < 0.5f && localPosition10.y == 0f)
						{
							list10[num61].transform.localPosition = localPosition;
							list10[num61].Fly((Direction)param, 2);
						}
					}
				}
			}
			material.SetTextureOffset("_MainTex", new Vector2(lifeTime * 7.5f, 0f));
			float num64 = ((lifeTime > windMaxT) ? 0f : ((lifeTime > windMaxT - 0.5f) ? (windMaxT - lifeTime) : ((!(lifeTime < 0.5f)) ? 0.5f : lifeTime)));
			material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, num64 * (0.5f + (1f + Mathf.Sin(lifeTime * 2.5f)) / 4f)));
			break;
		}
		case Ground.Explode:
		{
			if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				lifeTime += Time.deltaTime;
				if (base.transform.localScale.y == 0f)
				{
					ParticleSystem[] componentsInChildren2 = GetComponentsInChildren<ParticleSystem>();
					ParticleSystem[] array2 = componentsInChildren2;
					foreach (ParticleSystem particleSystem2 in array2)
					{
						particleSystem2.Stop();
					}
				}
				if (lifeTime >= 7.5f)
				{
					lifeTime -= 7.5f;
					if (base.transform.localScale.y > 0f)
					{
						ParticleSystem[] componentsInChildren3 = GetComponentsInChildren<ParticleSystem>();
						ParticleSystem[] array3 = componentsInChildren3;
						foreach (ParticleSystem particleSystem3 in array3)
						{
							particleSystem3.Play();
						}
						num = 1;
					}
				}
				else if (lifeTime < 2.5f)
				{
					TNet.List<GameCharacter> list19 = GameCharacter.List();
					for (int num77 = list19.Count - 1; num77 >= 0; num77--)
					{
						if (!list19[num77].IsDead && (list19[num77].Type == Character.Player || list19[num77].Type == Character.AI) && list19[num77].enabled)
						{
							Vector3 localPosition19 = list19[num77].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition19.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition19.z) < 0.5f && localPosition19.y < 1.5f)
							{
								list19[num77].Dead();
							}
						}
					}
					TNet.List<GameBomb> list20 = GameBomb.List();
					for (int num78 = list20.Count - 1; num78 >= 0; num78--)
					{
						if (!list20[num78].IsExploded && !list20[num78].IsHeld && list20[num78].enabled)
						{
							Vector3 localPosition20 = list20[num78].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition20.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition20.z) < 0.5f && localPosition20.y < 1.5f)
							{
								list20[num78].transform.localPosition = localPosition;
								list20[num78].Explode();
								break;
							}
						}
					}
					TNet.List<GameItem> list21 = GameItem.List();
					for (int num79 = list21.Count - 1; num79 >= 0; num79--)
					{
						if (!list21[num79].IsDeactived && list21[num79].enabled)
						{
							Vector3 localPosition21 = list21[num79].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition21.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition21.z) < 0.5f && localPosition21.y < 1.5f)
							{
								list21[num79].transform.localPosition = localPosition;
								list21[num79].Deactive();
								break;
							}
						}
					}
				}
			}
			int num80 = 0;
			num80 = ((!(lifeTime < 2.8125f) && !(lifeTime >= 7.1875f)) ? ((!(lifeTime < 3.125f) && !(lifeTime >= 6.875f)) ? ((int)((lifeTime - 1.875f) * 4f + 1f) % 2) : 2) : 3);
			material.SetTextureOffset("_MainTex", new Vector2(0.5f * (float)(num80 % 2), 0.5f * (float)(num80 / 2)));
			break;
		}
		case Ground.Pull:
		{
			float num17 = ((param >= 100) ? 0f : ((float)param * 0.15f + 0.25f));
			float num18 = num17 * num17;
			float num19 = pullSize * pullSize;
			TNet.List<GameCharacter> list4 = GameCharacter.List();
			for (int num20 = list4.Count - 1; num20 >= 0; num20--)
			{
				if ((list4[num20].Type == Character.Player || list4[num20].Type == Character.AI) && list4[num20].enabled)
				{
					Vector3 localPosition4 = list4[num20].transform.localPosition;
					float num21 = localPosition.x - localPosition4.x;
					float num22 = localPosition.z - localPosition4.z;
					float num23 = num21 * num21 + num22 * num22;
					if (num23 < num18)
					{
						if (list4[num20].IsDead)
						{
							if (num21 != 0f || num22 != 0f)
							{
								Vector2 vector3 = new Vector2(num21, num22);
								vector3.Normalize();
								vector3 *= num17 * Time.deltaTime;
								localPosition4.x += vector3.x;
								localPosition4.z += vector3.y;
							}
							float num24 = (0f - base.transform.localScale.x) / 50f;
							if (localPosition4.y > num24)
							{
								localPosition4.y += num24 * Time.deltaTime;
								if (localPosition4.y < num24)
								{
									localPosition4.y = num24;
								}
							}
							list4[num20].transform.localPosition = localPosition4;
							Transform transform = list4[num20].transform.Find("root/Bastard(Clone)");
							float x = transform.localScale.x;
							if (x > 0.001f)
							{
								x -= 0.999f * Time.deltaTime;
								if (x < 0.001f)
								{
									x = 0.001f;
								}
								transform.localScale = new Vector3(x, x, x);
							}
						}
						else
						{
							if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
							{
								list4[num20].Dead();
							}
							GameShadow.RemoveObject(list4[num20].gameObject);
						}
					}
					else if (num23 < num19)
					{
						float num25 = Mathf.Sqrt(num23);
						float num26 = ((num25 > 2.5f) ? 1f : ((!(num25 < 1.5f)) ? 0.5f : 0f));
						Vector2 externalForce = new Vector2(num21 + num22 * num26, num22 - num21 * num26);
						externalForce.Normalize();
						externalForce *= 4.99f * (pullSize - num25 + 1f) / pullSize;
						list4[num20].externalForce = externalForce;
					}
				}
			}
			TNet.List<GameBomb> list5 = GameBomb.List();
			for (int num27 = list5.Count - 1; num27 >= 0; num27--)
			{
				if (!list5[num27].IsExploded && !list5[num27].IsHeld)
				{
					Vector3 localPosition5 = list5[num27].transform.localPosition;
					float num28 = localPosition.x - localPosition5.x;
					float num29 = localPosition.z - localPosition5.z;
					float num30 = num28 * num28 + num29 * num29;
					if (num30 < num18 && localPosition5.y < 0.5f)
					{
						if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
						{
							list5[num27].Explode();
						}
					}
					else if (num30 < num19 && list5[num27].enabled)
					{
						float num31 = Mathf.Sqrt(num30);
						float num32 = ((num31 > 2.5f) ? 1f : ((!(num31 < 1.5f)) ? 0.5f : 0f));
						Vector2 externalForce2 = new Vector2(num28 + num29 * num32, num29 - num28 * num32);
						externalForce2.Normalize();
						externalForce2 *= 4.99f * (pullSize - num31 + 1f) / pullSize;
						list5[num27].externalForce = externalForce2;
					}
				}
			}
			TNet.List<GameItem> list6 = GameItem.List();
			for (int num33 = list6.Count - 1; num33 >= 0; num33--)
			{
				if (!list6[num33].IsDeactived)
				{
					Vector3 localPosition6 = list6[num33].transform.localPosition;
					float num34 = localPosition.x - Mathf.Round(localPosition6.x);
					float num35 = localPosition.z - Mathf.Round(localPosition6.z);
					float num36 = num34 * num34 + num35 * num35;
					if (num36 < num18 && localPosition6.y < 0.5f)
					{
						if (list6[num33].enabled)
						{
							list6[num33].enabled = false;
							GameShadow.RemoveObject(list6[num33].gameObject);
						}
						else
						{
							num34 = localPosition.x - localPosition6.x;
							num35 = localPosition.z - localPosition6.z;
							if (num34 != 0f || num35 != 0f)
							{
								Vector2 vector4 = new Vector2(num34, num35);
								vector4.Normalize();
								vector4 *= num17 * Time.deltaTime;
								localPosition6.x += vector4.x;
								localPosition6.z += vector4.y;
							}
							float num37 = (0f - base.transform.localScale.x) / 50f;
							if (localPosition6.y > num37)
							{
								localPosition6.y += num37 * Time.deltaTime;
								if (localPosition6.y < num37)
								{
									localPosition6.y = num37;
								}
							}
							list6[num33].transform.localPosition = localPosition6;
							float x2 = list6[num33].transform.localScale.x;
							if (x2 > 0.01f)
							{
								x2 -= 9.99f * Time.deltaTime;
								list6[num33].transform.localScale = new Vector3(x2, x2, x2);
								if (x2 <= 0.01f)
								{
									if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
									{
										list6[num33].GetComponent<TNObject>().Send(53, Target.Others, Global.onlineMatchID);
									}
									UnityEngine.Object.Destroy(list6[num33].gameObject);
								}
							}
						}
					}
					else if (num36 < num19 && list6[num33].enabled)
					{
						float num38 = Mathf.Sqrt(num36);
						float num39 = ((num38 > 2.5f) ? 1f : ((!(num38 < 1.5f)) ? 0.5f : 0f));
						Vector2 externalForce3 = new Vector2(num34 + num35 * num39, num35 - num34 * num39);
						externalForce3.Normalize();
						externalForce3 *= 4.99f * (pullSize - num38 + 1f) / pullSize;
						list6[num33].externalForce = externalForce3;
					}
				}
			}
			lifeTime += Time.deltaTime;
			float num40;
			for (num40 = 1f - lifeTime / 10f; num40 < 0f; num40 += 1f)
			{
			}
			float num41 = 0.5f + (1f + Mathf.Sin(Time.time)) / 8f;
			material.SetColor("_TintColor", new Color(num41, num41, num41, 0.5f));
			material.SetTextureOffset("_MainTex", new Vector2(0f, num40));
			if (!pullGrow)
			{
				break;
			}
			float num42 = 3.3333333f / ((Global.Mode != GameMode.LocalBattle && Global.Mode != GameMode.OnlineBattle) ? 2.5f : ((float)Global.timeAmount));
			if (param < 10)
			{
				param = Mathf.CeilToInt(lifeTime / 10f * num42);
			}
			float x3 = base.transform.localScale.x;
			if (x3 < (float)(10 * param))
			{
				x3 += Time.deltaTime * num42;
				if (x3 > (float)(10 * param))
				{
					x3 = 10 * param;
				}
				base.transform.localScale = new Vector3(x3, x3, x3);
				pullSize = x3 / 10f * 0.5f + 0.125f;
			}
			if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
			{
				break;
			}
			float num43 = (float)param * 0.25f;
			float num44 = num43 * num43;
			int num45 = Mathf.FloorToInt(localPosition.x - num43);
			int num46 = Mathf.FloorToInt(localPosition.z - num43);
			int num47 = Mathf.CeilToInt(localPosition.x + num43);
			int num48 = Mathf.CeilToInt(localPosition.z + num43);
			for (int j = num46; j <= num48; j++)
			{
				for (int k = num45; k <= num47; k++)
				{
					if (Global.Map.IsBlock(k, j))
					{
						float num49 = (float)k - localPosition.x;
						float num50 = (float)j - localPosition.z;
						if (num49 * num49 + num50 * num50 <= num44)
						{
							Global.Map.Explode(-1, k, j, -1, false, Direction.None, null, 1f);
						}
					}
				}
			}
			break;
		}
		case Ground.Redirect:
		{
			if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				TNet.List<GameBomb> list7 = GameBomb.List();
				for (int num51 = list7.Count - 1; num51 >= 0; num51--)
				{
					if (!list7[num51].IsExploded && !list7[num51].IsHeld && list7[num51].ShootDirection != Direction.None && list7[num51].ShootDirection != (Direction)param)
					{
						Vector3 localPosition7 = list7[num51].transform.localPosition;
						if (Mathf.Abs(localPosition.x - localPosition7.x) < 0.25f && Mathf.Abs(localPosition.z - localPosition7.z) < 0.25f)
						{
							switch ((Direction)param)
							{
							case Direction.Right:
							case Direction.Left:
								localPosition7.z = localPosition.z;
								break;
							case Direction.Up:
							case Direction.Down:
								localPosition7.x = localPosition.x;
								break;
							}
							list7[num51].transform.localPosition = localPosition7;
							list7[num51].Shoot((Direction)param);
							break;
						}
					}
				}
			}
			float time = Time.time;
			time -= (float)(int)time;
			time *= 4f;
			int num52 = (int)time;
			int num53 = 4 - param / 90;
			material.SetTextureOffset("_MainTex", new Vector2(0.25f * (float)num52, 0.25f * (float)num53));
			break;
		}
		case Ground.Teleport:
		{
			int num2 = ((Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting) ? 1 : 3);
			for (int i = 0; i < num2; i++)
			{
				int num3 = -1;
				MonoBehaviour monoBehaviour;
				do
				{
					monoBehaviour = null;
					switch (i)
					{
					case 0:
					{
						TNet.List<GameCharacter> list3 = GameCharacter.List();
						if (num3 == -1)
						{
							num3 = list3.Count;
						}
						for (int num6 = num3 - 1; num6 >= 0; num6--)
						{
							if (!list3[num6].IsDead)
							{
								monoBehaviour = list3[num6];
								num3 = num6;
								break;
							}
						}
						break;
					}
					case 1:
					{
						TNet.List<GameBomb> list2 = GameBomb.List();
						if (num3 == -1)
						{
							num3 = list2.Count;
						}
						for (int num5 = num3 - 1; num5 >= 0; num5--)
						{
							if (!list2[num5].IsExploded && !list2[num5].IsHeld)
							{
								monoBehaviour = list2[num5];
								num3 = num5;
								break;
							}
						}
						break;
					}
					case 2:
					{
						TNet.List<GameItem> list = GameItem.List();
						if (num3 == -1)
						{
							num3 = list.Count;
						}
						for (int num4 = num3 - 1; num4 >= 0; num4--)
						{
							if (!list[num4].IsDeactived)
							{
								monoBehaviour = list[num4];
								num3 = num4;
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
					int instanceID = monoBehaviour.gameObject.GetInstanceID();
					Vector3 localPosition2 = monoBehaviour.transform.localPosition;
					float num7 = localPosition2.x - localPosition.x;
					float num8 = localPosition2.z - localPosition.z;
					float num9 = num7 * num7 + num8 * num8;
					if (num9 < 1f / 9f)
					{
						if (!nearObj.ContainsKey(instanceID) || !(localPosition2.y < 0.25f))
						{
							continue;
						}
						GameGround gameGround2 = null;
						for (int num10 = allGrounds.Count - 1; num10 >= 0; num10--)
						{
							if (allGrounds[num10] != this && allGrounds[num10].type == type && allGrounds[num10].param == param && (gameGround2 == null || UnityEngine.Random.value < 0.5f))
							{
								gameGround2 = allGrounds[num10];
							}
						}
						bool flag = false;
						Vector3 localPosition3 = gameGround2.transform.localPosition;
						switch (i)
						{
						case 0:
							if (Global.Mode == GameMode.OnlineBattle && ((GameCharacter)monoBehaviour).controllerID != Global.onlinePlayerID)
							{
								break;
							}
							flag = true;
							if (!((GameCharacter)monoBehaviour).HasItem(Item.BombPass))
							{
								GameBomb gameBomb = Global.Map.PickBomb(localPosition3);
								if (gameBomb != null)
								{
									Vector3 vector2 = nearObj[instanceID];
									float num13 = localPosition2.x - vector2.x;
									float num14 = localPosition2.z - vector2.z;
									Direction direction = Direction.None;
									direction = ((!(Mathf.Abs(num13) > Mathf.Abs(num14))) ? ((num14 < 0f) ? Direction.Down : Direction.Up) : ((!(num13 < 0f)) ? Direction.Right : Direction.Left));
									if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
									{
										gameBomb.Fly(direction, 1);
										GameSound.StartSFX("bombKick");
										((GameCharacter)monoBehaviour).Stun();
									}
									else
									{
										GetComponent<TNObject>().Send(70, Target.Host, Global.onlineMatchID, (int)gameBomb.GetComponent<TNObject>().uid, (int)direction, ((GameCharacter)monoBehaviour).controllerID);
									}
								}
							}
							localPosition3.x += num7;
							localPosition3.y = 0f;
							localPosition3.z += num8;
							monoBehaviour.transform.localPosition = localPosition3;
							break;
						case 1:
						case 2:
						{
							flag = true;
							Vector3 vector = nearObj[instanceID];
							float num11 = localPosition2.x - vector.x;
							float num12 = localPosition2.z - vector.z;
							Direction direct = ((!(Mathf.Abs(num11) > Mathf.Abs(num12))) ? ((num12 < 0f) ? Direction.Down : Direction.Up) : ((!(num11 < 0f)) ? Direction.Right : Direction.Left));
							localPosition3.y = 1f;
							monoBehaviour.transform.localPosition = localPosition3;
							if (i == 1)
							{
								((GameBomb)monoBehaviour).Fly(direct, 1);
							}
							else
							{
								((GameItem)monoBehaviour).Fly(direct, 1);
							}
							break;
						}
						}
						if (flag)
						{
							gameGround2.transform.Find("fxUp").GetComponent<ParticleSystem>().Play();
							base.transform.Find("fxDown").GetComponent<ParticleSystem>().Play();
							gameGround = gameGround2;
							num = 2;
						}
					}
					else
					{
						if (num9 < 0.25f)
						{
							continue;
						}
						if (num9 < 1f)
						{
							if (!nearObj.ContainsKey(instanceID))
							{
								nearObj.Add(instanceID, localPosition2);
							}
						}
						else
						{
							nearObj.Remove(instanceID);
						}
					}
				}
				while (monoBehaviour != null);
			}
			if (lifeTime == 0f)
			{
				lifeTime = 1f;
				Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				switch (param)
				{
				case 0:
					color.r = 0.25f;
					color.g = 0.5f;
					color.b = 0.75f;
					break;
				case 1:
					color.r = 0.5f;
					color.g = 0.75f;
					color.b = 0.05f;
					break;
				case 2:
					color.r = 0.75f;
					color.g = 0.25f;
					color.b = 0.5f;
					break;
				}
				base.transform.Find("fxUp").GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", color);
				base.transform.Find("fxDown").GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", color);
			}
			if (param < 3)
			{
				float num15 = Time.time / 1.5f;
				num15 -= (float)(int)num15;
				num15 *= 8f;
				if (num15 >= 4f)
				{
					num15 = 8f - num15;
				}
				material.SetTextureOffset("_MainTex", new Vector2(0.25f * (float)(int)num15, 0.25f * (float)((param > 0) ? (4 - param) : 0)));
			}
			else
			{
				float num16 = Time.time * 2f;
				num16 -= (float)(int)num16;
				num16 *= 4f;
				material.SetTextureOffset("_MainTex", new Vector2(0.25f * (float)(int)num16, 0.25f * (float)((param > 0) ? (4 - param) : 0)));
			}
			break;
		}
		}
		if (Global.Mode != GameMode.OnlineBattle)
		{
			return;
		}
		if (TNManager.isHosting)
		{
			float num81 = 0f;
			switch (type)
			{
			case Ground.Wind:
				num81 = windMaxT;
				break;
			case Ground.Explode:
				num81 = base.transform.localScale.y;
				break;
			case Ground.Pull:
				num81 = pullSize;
				break;
			}
			if (sendDelay < 0.2f)
			{
				sendDelay += Time.deltaTime;
			}
			else
			{
				sendDelay = 0f;
				GetComponent<TNObject>().SendQuickly(71, Target.Others, Global.onlineMatchID, param, lifeTime, num81);
			}
		}
		if (num != 0)
		{
			GetComponent<TNObject>().Send(72, Target.Others, Global.onlineMatchID, num);
		}
		if (gameGround != null)
		{
			gameGround.GetComponent<TNObject>().Send(72, Target.Others, Global.onlineMatchID, 1);
		}
	}

	public static TNet.List<GameGround> List()
	{
		return allGrounds;
	}

	public static void Init()
	{
		int level = Global.Level;
		if (res == null || resLv != level)
		{
			res = new GameObject[8];
			resLv = level;
			for (int num = 7; num >= 0; num--)
			{
				string text = string.Concat("Grounds/", (Ground)num, "/");
				Ground ground = (Ground)num;
				text = ((ground != Ground.TimeOut && ground != Ground.Virus) ? (text + "Ground") : (text + level.ToString("D2")));
				res[num] = Resources.Load(text) as GameObject;
			}
		}
		if (allGrounds == null)
		{
			allGrounds = new TNet.List<GameGround>();
		}
		else
		{
			allGrounds.Clear();
		}
	}

	public static void Destroy()
	{
		allGrounds.Clear();
	}

	public float GetLifeTime()
	{
		return lifeTime;
	}

	public static GameObject Create(Ground type, int x, int y, int param = 0)
	{
		foreach (GameGround allGround in allGrounds)
		{
			if (allGround.type <= type)
			{
				Vector3 localPosition = allGround.transform.localPosition;
				if (Mathf.RoundToInt(localPosition.x) == x && Mathf.RoundToInt(localPosition.z) == y)
				{
					return null;
				}
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(res[(int)type]) as GameObject;
		GameGround gameGround = gameObject.AddComponent<GameGround>();
		gameGround.type = type;
		gameGround.param = param;
		Renderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer == null)
		{
			renderer = gameObject.GetComponentInChildren<MeshRenderer>();
		}
		if (renderer != null)
		{
			gameGround.material = renderer.material;
		}
		switch (type)
		{
		case Ground.Virus:
			gameGround.param = Global.Level;
			gameObject.transform.GetChild(0).localPosition = new Vector3((float)(-x) * 0.0005f, -0.040000003f, -0.040000003f * Mathf.Tan((float)Math.PI / 6f) - (float)y * 0.0005f);
			break;
		case Ground.Wind:
		{
			gameGround.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
			bool flag = (param == 270 && x == -8) || (param == 90 && x == 8);
			bool flag2 = (param == 180 && y == -5) || (param == 0 && y == 5);
			bool flag3 = (param == 270 && x == 8) || (param == 90 && x == -8);
			bool flag4 = (param == 180 && y == 5) || (param == 0 && y == -5);
			if (flag || flag2 || flag3 || flag4)
			{
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				Color32[] array = new Color32[4];
				if (flag || flag2)
				{
					array[0] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					array[1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
					array[2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					array[3] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				}
				else if (flag3 || flag4)
				{
					array[0] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
					array[1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					array[2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
					array[3] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
				component.mesh.colors32 = array;
			}
			if (Global.Mode == GameMode.Adventure)
			{
				gameGround.windMaxT = 1.5f * (float)(Global.advStage % 6);
				break;
			}
			float num = 1.5f * (float)UnityEngine.Random.Range(1, 6);
			for (int num2 = allGrounds.Count - 1; num2 >= 0; num2--)
			{
				if (allGrounds[num2].type == Ground.Wind && allGrounds[num2].windMaxT > 0f)
				{
					num = allGrounds[num2].windMaxT;
					break;
				}
			}
			gameGround.windMaxT = num;
			break;
		}
		case Ground.Explode:
			gameGround.lifeTime = 2.5f;
			break;
		case Ground.Pull:
			gameGround.pullSize = ((param >= 100) ? ((float)param / 100f) : ((float)param)) * 0.5f + 0.125f;
			if (param < 100)
			{
				gameGround.pullGrow = param < 1;
				gameObject.transform.GetChild(0).localPosition = new Vector3((float)(-x) * 0.0005f, -0.020000001f, -0.020000001f * Mathf.Tan((float)Math.PI / 6f) - (float)y * 0.0005f);
			}
			else
			{
				gameObject.transform.GetChild(0).gameObject.SetActive(false);
			}
			break;
		case Ground.Teleport:
			gameGround.nearObj = new Dictionary<int, Vector3>();
			break;
		}
		int num3;
		int num4;
		switch (type)
		{
		case Ground.Virus:
			num3 = 180;
			num4 = ((gameGround.param != 1 && gameGround.param != 4) ? 10 : 0);
			break;
		case Ground.Wind:
			num3 = 180 + param;
			num4 = 10;
			break;
		case Ground.Pull:
			num3 = 180;
			num4 = 10 * param;
			break;
		default:
			num3 = 180;
			num4 = 10;
			break;
		}
		gameObject.transform.parent = GameObject.Find("Script/FXs").transform;
		gameObject.transform.localPosition = new Vector3(x, 0f, y);
		gameObject.transform.localRotation = Quaternion.Euler(0f, num3, 0f);
		gameObject.transform.localScale = new Vector3(num4, num4, num4);
		return gameObject;
	}

	[RFC(70)]
	private void GameGroundTeleportAction(int matchID, int bombID, int direct, int playerID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		foreach (GameBomb item in GameBomb.List())
		{
			if (item.GetComponent<TNObject>().uid == (uint)bombID)
			{
				item.Fly((Direction)direct, 1);
				GameSound.StartSFX("bombKick");
				break;
			}
		}
		foreach (GameCharacter item2 in GameCharacter.List())
		{
			if (item2.controllerID == playerID)
			{
				item2.Stun();
				break;
			}
		}
	}

	[RFC(71)]
	private void GameGroundUpdatePhysic(int matchID, int param, float lifeTime, float arg)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		switch (type)
		{
		case Ground.Wind:
			windMaxT = arg;
			break;
		case Ground.Explode:
		{
			Vector3 localScale = base.transform.localScale;
			localScale.y = arg;
			base.transform.localScale = localScale;
			if (arg == 0f)
			{
				ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Stop();
				}
			}
			break;
		}
		case Ground.Pull:
			if (pullGrow)
			{
				pullSize = arg;
				float num = (pullSize - 0.125f) / 0.5f * 10f;
				if (base.transform.localScale.x < num)
				{
					base.transform.localScale = new Vector3(num, num, num);
				}
			}
			break;
		}
		this.param = param;
		this.lifeTime = lifeTime;
	}

	[RFC(72)]
	private void GameGroundUpdateAction(int matchID, int arg)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		switch (type)
		{
		case Ground.TimeOut:
		{
			base.name = "Explosion";
			base.transform.parent = base.transform.parent.parent.Find("Explosions");
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.center = new Vector3(0f, 0.025f, 0f);
			sphereCollider.radius = 0.025f;
			break;
		}
		case Ground.Crack:
			crackStep = arg;
			if (crackStep >= 2 && base.GetComponent<Collider>() == null)
			{
				BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
				boxCollider.center = new Vector3(0f, 0.05f, 0f);
				boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			break;
		case Ground.Explode:
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
			break;
		}
		case Ground.Teleport:
			if ((arg & 1) != 0)
			{
				ParticleSystem component = base.transform.Find("fxUp").GetComponent<ParticleSystem>();
				if (component.isStopped)
				{
					component.Play();
				}
			}
			if ((arg & 2) != 0)
			{
				ParticleSystem component2 = base.transform.Find("fxDown").GetComponent<ParticleSystem>();
				if (component2.isStopped)
				{
					component2.Play();
				}
			}
			break;
		case Ground.Wind:
		case Ground.Pull:
			break;
		}
	}
}
