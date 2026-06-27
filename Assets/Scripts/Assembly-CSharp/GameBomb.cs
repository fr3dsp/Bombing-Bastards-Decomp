using System;
using TNet;
using UnityEngine;

public class GameBomb : MonoBehaviour
{
	public const float Radius = 0.5f;

	public const float BlastRadius = 0.25f;

	private const float flySpeed = 7.5f;

	private const float flyGravity = 25f;

	private const float shootSpeed = 10f;

	private static GameObject[] res;

	private static int fxType;

	private static GameObject fxBomb;

	private static GameObject bossBomb;

	private static List<GameBomb> allBombs;

	private GameCharacter owner;

	private List<GameCharacter> ignoreList;

	private GameObject model;

	private Direction flyOld = Direction.None;

	private Direction flyDir = Direction.None;

	private Vector3 flyVec = Vector3.up;

	private float flyMove;

	private float flyRange;

	private Direction shootDir = Direction.None;

	private bool shootKick;

	private Vector3 flySyncP = Vector3.zero;

	private Vector3 shootSyncP = Vector3.up;

	private bool bounce;

	private float timeFuse = 4f;

	private Bomb type;

	private Fire fireLv = Fire.Lv1;

	private bool dead;

	private float deadTime;

	private ParticleSystem fuseFX;

	public Vector2 externalForce;

	private Vector2 inertia;

	private float sendDelay;

	public GameCharacter Owner
	{
		get
		{
			return owner;
		}
	}

	public Direction FlyDirection
	{
		get
		{
			return flyDir;
		}
	}

	public bool IsFlying
	{
		get
		{
			return flyDir != Direction.None || flyVec.y == 0f;
		}
	}

	public Direction ShootDirection
	{
		get
		{
			return shootDir;
		}
	}

	public bool IsHeld
	{
		get
		{
			return base.transform.parent.name != "Bombs";
		}
	}

	public bool IsLanded
	{
		get
		{
			if (flyDir == Direction.None && flyVec.y != 0f && shootDir == Direction.None)
			{
				Vector3 localPosition = base.transform.localPosition;
				return localPosition.x == Mathf.Round(localPosition.x) && localPosition.y == 0f && localPosition.z == Mathf.Round(localPosition.z);
			}
			return false;
		}
	}

	public bool IsBounce
	{
		get
		{
			return bounce;
		}
	}

	public float TimeFuse
	{
		get
		{
			return timeFuse;
		}
	}

	public Bomb Type
	{
		get
		{
			return type;
		}
	}

	public Fire FireLv
	{
		get
		{
			return fireLv;
		}
	}

	public bool IsExploded
	{
		get
		{
			return dead;
		}
	}

	private float ChargeSize
	{
		get
		{
			int num = Mathf.CeilToInt(5f - timeFuse);
			if (num < 1)
			{
				return 1f;
			}
			int num2 = num * (num + 1) + 1;
			int num3 = 2;
			for (int i = 1; i < num; i++)
			{
				num3 *= 2;
			}
			return (float)(num2 + num3) * 0.5f;
		}
	}

	private void Awake()
	{
		fuseFX = base.gameObject.GetComponentInChildren<ParticleSystem>();
		allBombs.Add(this);
	}

	private void OnDestroy()
	{
		allBombs.Remove(this);
	}

	[RFC(65)]
	private void GameBombOnDestroy(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (base.transform.parent.name != "Bombs")
			{
				base.transform.parent.parent.parent.GetComponent<GameCharacter>().ReleaseBomb();
			}
			GameShadow.RemoveObject(base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (dead)
		{
			switch (type)
			{
			case Bomb.XtraPillar:
			{
				deadTime += Time.deltaTime;
				List<GameCharacter> list3 = GameCharacter.List();
				for (int num28 = list3.Count - 1; num28 >= 0; num28--)
				{
					if (list3[num28].Type == Character.Player && !list3[num28].IsDead)
					{
						Vector3 localPosition5 = base.transform.localPosition;
						Vector3 localPosition6 = list3[num28].transform.localPosition;
						float num29 = (float)fireLv + 0.5f;
						if ((Mathf.RoundToInt(localPosition5.x) == Mathf.RoundToInt(localPosition6.x) && Mathf.Abs(localPosition5.z - localPosition6.z) <= num29) || (Mathf.RoundToInt(localPosition5.z) == Mathf.RoundToInt(localPosition6.z) && Mathf.Abs(localPosition5.x - localPosition6.x) <= num29))
						{
							list3[num28].Infect(Virus.SuperSlow, 4f);
							break;
						}
					}
				}
				if (deadTime >= 2f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				break;
			}
			case Bomb.XtraCharge:
			{
				float num26 = deadTime;
				deadTime += Time.deltaTime;
				if (deadTime > 1f)
				{
					if (num26 <= 1f)
					{
						model.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					}
					else if (deadTime >= 2.5f)
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
				else if (deadTime > 0.25f)
				{
					float num27 = (1f - deadTime) * 4f / 3f;
					model.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f * num27));
				}
				break;
			}
			case Bomb.XtraRing:
			{
				float num = deadTime;
				deadTime += Time.deltaTime;
				float num2 = deadTime - (float)(int)deadTime;
				float num3 = ((!(num2 > 0.125f) || !(num2 < 0.875f)) ? 0f : ((num2 - 0.125f) / 0.75f * 2f));
				num3 = 1f - (num3 - 1f) * (num3 - 1f);
				Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f * num3);
				Vector2 offset = new Vector2(0f, (float)((int)(num2 * 16f) % 8) / 8f);
				Vector2 scale = new Vector2(Mathf.Max(1, (int)deadTime), 1f);
				if (deadTime < 1f)
				{
					Transform transform = model.transform.Find("Lightning");
					Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
					Renderer[] array = componentsInChildren;
					foreach (Renderer renderer in array)
					{
						Material material = renderer.material;
						material.SetColor("_TintColor", color);
						material.SetTextureOffset("_MainTex", offset);
					}
					if (num == 0f)
					{
						GameSound.StartSFX("skillPlasmaDown");
					}
				}
				Transform transform2 = model.transform.Find("PlasmaFXs");
				if (transform2 == null)
				{
					break;
				}
				int num4 = 2;
				Vector3 localPosition = base.transform.localPosition;
				if ((int)deadTime != (int)num)
				{
					num4 = 1;
					for (int j = 0; j < 1; j++)
					{
						float num5 = (int)deadTime;
						for (int k = 0; k < 100; k++)
						{
							Transform child = transform2.GetChild(k + j * 100);
							float f = (float)Math.PI / 50f * (float)k;
							float num6 = Mathf.Cos(f) * num5 * 0.1f;
							float num7 = Mathf.Sin(f) * num5 * 0.1f;
							float num8 = (Mathf.Abs(localPosition.x + num6 * 10f) - 7f) / 3f;
							if (num8 < 0f)
							{
								num8 = 0f;
							}
							float num9 = (Mathf.Abs(localPosition.z + num7 * 10f) - 4f) / 3f;
							if (num9 < 0f)
							{
								num9 = 0f;
							}
							float num10 = Mathf.Sqrt(num8 * num8 + num9 * num9);
							if (num10 < 1f)
							{
								child.localPosition = new Vector3(0f - num6, 0f, 0f - num7);
								child.localScale = new Vector3(1f, 1f - num10, 1f);
								child.GetComponent<Animation>()["idle"].speed = 0f;
								child.GetComponent<Animation>()["idle"].time = num5;
								num4 = 0;
							}
							else
							{
								child.localScale = Vector3.zero;
							}
						}
					}
					if (num < 1f)
					{
						transform2.localPosition = new Vector3(0f, 0.0125f, 0f);
					}
				}
				for (int l = 0; l < 100; l++)
				{
					Transform child2 = transform2.GetChild(l);
					Material material2 = child2.GetComponentInChildren<Renderer>().material;
					Color color2 = color;
					color2.a *= child2.localScale.y;
					material2.SetColor("_TintColor", color2 * 1.5f);
					material2.SetTextureOffset("_MainTex", offset);
					material2.SetTextureScale("_MainTex", scale);
				}
				if (num2 > 0.25f && num2 < 0.75f)
				{
					int num11 = (int)deadTime;
					float num12 = ((float)num11 - 0.75f) * ((float)num11 - 0.75f);
					if (num11 == 0)
					{
						num12 = 0f;
					}
					float num13 = ((float)num11 + 0.75f) * ((float)num11 + 0.75f);
					List<GameItem> list = GameItem.List();
					for (int num14 = list.Count - 1; num14 >= 0; num14--)
					{
						if (!list[num14].IsDeactived)
						{
							Vector3 localPosition2 = list[num14].transform.localPosition;
							float num15 = localPosition2.x - localPosition.x;
							float num16 = localPosition2.z - localPosition.z;
							float num17 = num15 * num15 + num16 * num16;
							if (num17 >= num12 && num17 <= num13)
							{
								list[num14].Deactive();
							}
						}
					}
					for (int num18 = allBombs.Count - 1; num18 >= 0; num18--)
					{
						if (!allBombs[num18].IsExploded && !allBombs[num18].IsHeld)
						{
							Vector3 localPosition3 = allBombs[num18].transform.localPosition;
							float num19 = localPosition3.x - localPosition.x;
							float num20 = localPosition3.z - localPosition.z;
							float num21 = num19 * num19 + num20 * num20;
							if (num21 >= num12 && num21 <= num13)
							{
								allBombs[num18].Explode();
							}
						}
					}
					List<GameCharacter> list2 = GameCharacter.List();
					for (int num22 = list2.Count - 1; num22 >= 0; num22--)
					{
						if (list2[num22].Type == Character.Player && !list2[num22].IsDead)
						{
							Vector3 localPosition4 = list2[num22].transform.localPosition;
							float num23 = localPosition4.x - localPosition.x;
							float num24 = localPosition4.z - localPosition.z;
							float num25 = num23 * num23 + num24 * num24;
							if (num25 >= num12 && num25 <= num13)
							{
								list2[num22].Dead();
							}
						}
					}
				}
				switch (num4)
				{
				case 0:
					GameSound.StartSFX("skillPlasmaRing");
					break;
				case 1:
					transform2.parent = null;
					UnityEngine.Object.Destroy(base.gameObject);
					break;
				}
				break;
			}
			case Bomb.XtraLine:
				break;
			}
			return;
		}
		bool flag = base.transform.parent.name == "Bombs";
		Vector3 localPosition7 = base.transform.localPosition;
		if (ignoreList != null)
		{
			if (flag)
			{
				for (int num30 = ignoreList.Count - 1; num30 >= 0; num30--)
				{
					Vector3 localPosition8 = ignoreList[num30].transform.localPosition;
					float sqrMagnitude = (localPosition7 - localPosition8).sqrMagnitude;
					if (sqrMagnitude > 0.25f && !ignoreList[num30].HasItem(Item.BombPass) && model.GetComponent<Collider>().enabled && ignoreList[num30].GetComponent<Collider>().enabled)
					{
						Physics.IgnoreCollision(ignoreList[num30].GetComponent<Collider>(), model.GetComponent<Collider>(), false);
					}
					if (sqrMagnitude >= 1f)
					{
						if (!ignoreList[num30].HasItem(Item.BombPass) && base.GetComponent<Collider>().enabled && ignoreList[num30].GetComponent<Collider>().enabled)
						{
							Physics.IgnoreCollision(ignoreList[num30].GetComponent<Collider>(), base.GetComponent<Collider>(), false);
						}
						ignoreList.RemoveAt(num30);
					}
				}
				if (ignoreList.Count == 0)
				{
					ignoreList = null;
				}
			}
			else
			{
				ignoreList = null;
			}
		}
		if (type < Bomb.MAX)
		{
			if (flag)
			{
				if (fuseFX != null && fuseFX.isStopped)
				{
					fuseFX.Play();
				}
				if (!model.GetComponent<Animation>().isPlaying)
				{
					model.GetComponent<Animation>().Play("idle");
					GameShadow.AddObject(base.gameObject);
				}
			}
			else
			{
				if (fuseFX != null && fuseFX.isPlaying)
				{
					fuseFX.Stop();
				}
				if (model.GetComponent<Animation>().isPlaying)
				{
					model.GetComponent<Animation>().Stop();
					model.GetComponent<Animation>()["idle"].time = 0f;
					GameShadow.RemoveObject(base.gameObject);
				}
			}
		}
		if (externalForce != Vector2.zero)
		{
			shootDir = Direction.None;
		}
		if (flyDir != Direction.None || flyVec.y == 0f)
		{
			if (flag)
			{
				if (Global.Mode == GameMode.OnlineBattle && flySyncP.y >= 1f)
				{
					localPosition7 = flySyncP;
				}
				float num31 = 7.5f * Time.deltaTime;
				flyMove += num31;
				if (flyMove > flyRange)
				{
					flyMove = flyRange;
				}
				if (flyDir != Direction.None)
				{
					switch (flyDir)
					{
					case Direction.Left:
						localPosition7.x -= num31;
						break;
					case Direction.Down:
						localPosition7.z -= num31;
						break;
					case Direction.Right:
						localPosition7.x += num31;
						break;
					case Direction.Up:
						localPosition7.z += num31;
						break;
					}
				}
				else
				{
					Vector3 vector = flyVec.normalized * num31;
					localPosition7.x += vector.x;
					localPosition7.z += vector.z;
				}
				if (localPosition7.x > 13f)
				{
					localPosition7.x -= 26f;
				}
				else if (localPosition7.x < -13f)
				{
					localPosition7.x += 26f;
				}
				if (localPosition7.z > 10f)
				{
					localPosition7.z -= 20f;
				}
				else if (localPosition7.z < -10f)
				{
					localPosition7.z += 20f;
				}
				float num32 = flyMove / flyRange * 2f - 1f;
				localPosition7.y = 1f + flyRange / 1.5f * (1f - num32 * num32);
				flySyncP = localPosition7;
				if (flyMove == flyRange)
				{
					localPosition7.x = Mathf.Round(localPosition7.x);
					localPosition7.y = 1f;
					localPosition7.z = Mathf.Round(localPosition7.z);
					flySyncP = Vector3.zero;
					List<GameObject> objects = Global.Map.GetObjects(localPosition7.x, localPosition7.z, 0.25f);
					if (objects != null)
					{
						objects.Remove(base.gameObject);
						if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
						{
							for (int num33 = objects.Count - 1; num33 >= 0; num33--)
							{
								GameCharacter component = objects[num33].GetComponent<GameCharacter>();
								if (component != null)
								{
									if (type > Bomb.MAX)
									{
										objects.RemoveAt(num33);
										Vector3 localPosition9 = component.transform.localPosition;
										float num34 = localPosition9.x - localPosition7.x;
										float num35 = localPosition9.z - localPosition7.z;
										if (num34 * num34 + num35 * num35 < 0.5625f)
										{
											component.Dead();
										}
									}
									else
									{
										component.Stun();
									}
								}
								else
								{
									GameBomb component2 = objects[num33].GetComponent<GameBomb>();
									if (component2 != null)
									{
										if (type > Bomb.MAX)
										{
											objects.RemoveAt(num33);
											component2.Explode();
										}
									}
									else
									{
										GameItem component3 = objects[num33].GetComponent<GameItem>();
										if (component3 != null && type > Bomb.MAX)
										{
											objects.RemoveAt(num33);
											component3.Deactive();
										}
									}
								}
							}
						}
					}
					if (objects != null && objects.Count == 0)
					{
						flyDir = Direction.None;
						flyVec.y = 1f;
						List<GameCharacter> list4 = GameCharacter.List();
						for (int num36 = list4.Count - 1; num36 >= 0; num36--)
						{
							if (!list4[num36].IsDead && list4[num36].HasItem(Item.BombPass))
							{
								if (list4[num36].GetComponent<Collider>().enabled)
								{
									if (model.GetComponent<Collider>().enabled)
									{
										Physics.IgnoreCollision(list4[num36].GetComponent<Collider>(), model.GetComponent<Collider>());
									}
									if (base.GetComponent<Collider>().enabled)
									{
										Physics.IgnoreCollision(list4[num36].GetComponent<Collider>(), base.GetComponent<Collider>());
									}
								}
								if (list4[num36].Type == Character.Boss || list4[num36].Type == Character.Monster)
								{
									Transform transform3 = list4[num36].transform.Find("root");
									Physics.IgnoreCollision(transform3.GetComponent<Collider>(), base.GetComponent<Collider>());
									Physics.IgnoreCollision(transform3.GetComponent<Collider>(), model.GetComponent<Collider>());
								}
							}
						}
					}
					else if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
					{
						if (objects != null && bounce)
						{
							switch (UnityEngine.Random.Range(0, 4))
							{
							case 0:
								flyDir = Direction.Up;
								break;
							case 1:
								flyDir = Direction.Right;
								break;
							case 2:
								flyDir = Direction.Down;
								break;
							case 3:
								flyDir = Direction.Left;
								break;
							}
						}
						else if (flyDir == Direction.None)
						{
							if (Mathf.Abs(flyVec.x) < Mathf.Abs(flyVec.z))
							{
								flyDir = ((flyVec.z < 0f) ? Direction.Down : Direction.Up);
							}
							else
							{
								flyDir = ((!(flyVec.x < 0f)) ? Direction.Right : Direction.Left);
							}
							flyVec.y = 1f;
						}
						int range = 1;
						switch (flyDir)
						{
						case Direction.Left:
							range = ((!(localPosition7.x < -8f)) ? 1 : ((int)(18f + localPosition7.x)));
							break;
						case Direction.Down:
							range = ((!(localPosition7.z < -5f)) ? 1 : ((int)(15f + localPosition7.z)));
							break;
						case Direction.Right:
							range = ((!(localPosition7.x > 8f)) ? 1 : ((int)(18f - localPosition7.x)));
							break;
						case Direction.Up:
							range = ((!(localPosition7.z > 5f)) ? 1 : ((int)(15f - localPosition7.z)));
							break;
						}
						Fly(flyDir, range, true);
					}
				}
				base.transform.localPosition = localPosition7;
			}
			else
			{
				flyDir = Direction.None;
				flyVec.y = 1f;
			}
			externalForce = Vector2.zero;
		}
		else if (shootDir != Direction.None)
		{
			if (flag)
			{
				if (Global.Mode == GameMode.OnlineBattle && shootSyncP.y == 0f)
				{
					localPosition7 = shootSyncP;
					shootSyncP = Vector3.up;
				}
				float num37 = 10f * Time.deltaTime;
				Vector3 vector2 = localPosition7;
				switch (shootDir)
				{
				case Direction.Left:
					vector2.x -= num37;
					break;
				case Direction.Down:
					vector2.z -= num37;
					break;
				case Direction.Right:
					vector2.x += num37;
					break;
				case Direction.Up:
					vector2.z += num37;
					break;
				}
				List<GameObject> objects2 = Global.Map.GetObjects(vector2.x, vector2.z, 0.5f);
				if (objects2 != null)
				{
					objects2.Remove(base.gameObject);
					for (int num38 = objects2.Count - 1; num38 >= 0; num38--)
					{
						GameCharacter component4 = objects2[num38].GetComponent<GameCharacter>();
						if (component4 != null && component4.HasItem(Item.BombPass))
						{
							objects2.RemoveAt(num38);
						}
						else
						{
							Vector3 vector3 = objects2[num38].transform.localPosition - vector2;
							bool flag2 = true;
							switch (shootDir)
							{
							case Direction.Left:
								if (vector3.x > 0f)
								{
									flag2 = false;
									objects2.RemoveAt(num38);
								}
								break;
							case Direction.Down:
								if (vector3.z > 0f)
								{
									flag2 = false;
									objects2.RemoveAt(num38);
								}
								break;
							case Direction.Right:
								if (vector3.x < 0f)
								{
									flag2 = false;
									objects2.RemoveAt(num38);
								}
								break;
							case Direction.Up:
								if (vector3.z < 0f)
								{
									flag2 = false;
									objects2.RemoveAt(num38);
								}
								break;
							}
							if (component4 != null && flag2)
							{
								switch (shootDir)
								{
								case Direction.Right:
								case Direction.Left:
									if (Mathf.Abs(vector3.z) > 0.75f)
									{
										objects2.RemoveAt(num38);
									}
									break;
								case Direction.Up:
								case Direction.Down:
									if (Mathf.Abs(vector3.x) > 0.75f)
									{
										objects2.RemoveAt(num38);
									}
									break;
								}
							}
						}
					}
				}
				if (objects2 == null || objects2.Count > 0)
				{
					vector2 = localPosition7;
					if (bounce)
					{
						switch (shootDir)
						{
						case Direction.Left:
							shootDir = Direction.Right;
							break;
						case Direction.Down:
							shootDir = Direction.Up;
							break;
						case Direction.Right:
							shootDir = Direction.Left;
							break;
						case Direction.Up:
							shootDir = Direction.Down;
							break;
						}
					}
					else
					{
						shootDir = Direction.None;
					}
				}
				else if (shootKick)
				{
					shootKick = false;
					GameSound.StartSFX("bombKick");
				}
				base.transform.localPosition = vector2;
			}
			else
			{
				shootDir = Direction.None;
			}
		}
		else if (flag)
		{
			if (localPosition7.y > 0f)
			{
				localPosition7.y -= 25f * Time.deltaTime;
				if (localPosition7.y <= 0f)
				{
					localPosition7.y = 0f;
					if (type > Bomb.MAX)
					{
						GameSound.StartSFX("skillCrash");
					}
				}
				else if (localPosition7.y < 1f && (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting))
				{
					List<GameCharacter> list5 = GameCharacter.List();
					for (int num39 = list5.Count - 1; num39 >= 0; num39--)
					{
						if (!list5[num39].IsDead && (list5[num39].Type == Character.Player || list5[num39].Type == Character.AI || list5[num39].Type == Character.Monster))
						{
							Vector3 localPosition10 = list5[num39].transform.localPosition;
							float num40 = localPosition10.x - localPosition7.x;
							float num41 = localPosition10.z - localPosition7.z;
							if (num40 * num40 + num41 * num41 < 0.5625f)
							{
								if (type > Bomb.MAX)
								{
									list5[num39].Dead();
								}
								else
								{
									list5[num39].Stun();
									localPosition7.y = 1f;
									Fly(flyOld, 1, true);
								}
							}
						}
					}
					if (localPosition7.y < 1f)
					{
						for (int num42 = allBombs.Count - 1; num42 >= 0; num42--)
						{
							if (allBombs[num42] != this && !allBombs[num42].IsExploded && !allBombs[num42].IsHeld)
							{
								Vector3 localPosition11 = allBombs[num42].transform.localPosition;
								if (Mathf.Abs(localPosition7.x - localPosition11.x) < 0.5f && Mathf.Abs(localPosition7.z - localPosition11.z) < 0.5f && localPosition11.y < localPosition7.y)
								{
									if (type <= Bomb.MAX)
									{
										localPosition7.y = 1f;
										Fly(flyOld, 1, true);
										break;
									}
									allBombs[num42].Explode();
								}
							}
						}
					}
					if (localPosition7.y < 1f)
					{
						List<GameItem> list6 = GameItem.List();
						for (int num43 = list6.Count - 1; num43 >= 0; num43--)
						{
							if (!list6[num43].IsDeactived)
							{
								Vector3 localPosition12 = list6[num43].transform.localPosition;
								if (Mathf.Abs(localPosition7.x - localPosition12.x) < 0.5f && Mathf.Abs(localPosition7.z - localPosition12.z) < 0.5f && localPosition12.y < localPosition7.y)
								{
									if (type <= Bomb.MAX)
									{
										localPosition7.y = 1f;
										Fly(flyOld, 1, true);
										break;
									}
									list6[num43].Deactive();
								}
							}
						}
					}
				}
				base.transform.localPosition = localPosition7;
			}
			else
			{
				float num44 = Mathf.Round(localPosition7.x);
				float num45 = Mathf.Round(localPosition7.z);
				if (inertia != Vector2.zero)
				{
					float x = localPosition7.x;
					float z = localPosition7.z;
					localPosition7.x += inertia.x * Time.deltaTime;
					localPosition7.y = 0f;
					localPosition7.z += inertia.y * Time.deltaTime;
					if (inertia.x < 0f)
					{
						float num46 = Mathf.Floor(x);
						List<GameObject> objects3 = Global.Map.GetObjects(num46, num45, 0.499f);
						if (objects3 != null && objects3.Count > 0)
						{
							objects3.Remove(base.gameObject);
							for (int num47 = objects3.Count - 1; num47 >= 0; num47--)
							{
								if ((bool)objects3[num47].GetComponent<GameCharacter>())
								{
									objects3.RemoveAt(num47);
								}
								else if (objects3[num47].GetComponent<GameBomb>() != null || objects3[num47].GetComponent<GameItem>() != null)
								{
									Vector3 vector4 = new Vector3(num46, 0f, num45);
									Vector3 vector5 = base.transform.localPosition - vector4;
									vector5.y = 0f;
									Vector3 vector6 = objects3[num47].transform.localPosition - vector4;
									vector6.y = 0f;
									float sqrMagnitude2 = vector5.sqrMagnitude;
									float sqrMagnitude3 = vector6.sqrMagnitude;
									if (sqrMagnitude2 == sqrMagnitude3)
									{
										if (base.gameObject.GetInstanceID() < objects3[num47].GetInstanceID())
										{
											objects3.RemoveAt(num47);
										}
									}
									else if (sqrMagnitude2 < sqrMagnitude3)
									{
										objects3.RemoveAt(num47);
									}
								}
							}
						}
						if (objects3 == null || objects3.Count > 0)
						{
							localPosition7.x = x;
						}
						else if (num46 != x && Mathf.Floor(localPosition7.x) < num46)
						{
							localPosition7.x = num46;
							inertia.x = 0f;
						}
					}
					else if (inertia.x > 0f)
					{
						float num48 = Mathf.Ceil(x);
						List<GameObject> objects4 = Global.Map.GetObjects(num48, num45, 0.499f);
						if (objects4 != null && objects4.Count > 0)
						{
							objects4.Remove(base.gameObject);
							for (int num49 = objects4.Count - 1; num49 >= 0; num49--)
							{
								if ((bool)objects4[num49].GetComponent<GameCharacter>())
								{
									objects4.RemoveAt(num49);
								}
								else if (objects4[num49].GetComponent<GameBomb>() != null || objects4[num49].GetComponent<GameItem>() != null)
								{
									Vector3 vector7 = new Vector3(num48, 0f, num45);
									Vector3 vector8 = base.transform.localPosition - vector7;
									vector8.y = 0f;
									Vector3 vector9 = objects4[num49].transform.localPosition - vector7;
									vector9.y = 0f;
									float sqrMagnitude4 = vector8.sqrMagnitude;
									float sqrMagnitude5 = vector9.sqrMagnitude;
									if (sqrMagnitude4 == sqrMagnitude5)
									{
										if (base.gameObject.GetInstanceID() < objects4[num49].GetInstanceID())
										{
											objects4.RemoveAt(num49);
										}
									}
									else if (sqrMagnitude4 < sqrMagnitude5)
									{
										objects4.RemoveAt(num49);
									}
								}
							}
						}
						if (objects4 == null || objects4.Count > 0)
						{
							localPosition7.x = x;
						}
						else if (num48 != x && Mathf.Ceil(localPosition7.x) > num48)
						{
							localPosition7.x = num48;
							inertia.x = 0f;
						}
					}
					if (inertia.y < 0f)
					{
						float num50 = Mathf.Floor(z);
						List<GameObject> objects5 = Global.Map.GetObjects(num44, num50, 0.499f);
						if (objects5 != null && objects5.Count > 0)
						{
							objects5.Remove(base.gameObject);
							for (int num51 = objects5.Count - 1; num51 >= 0; num51--)
							{
								if ((bool)objects5[num51].GetComponent<GameCharacter>())
								{
									objects5.RemoveAt(num51);
								}
								else if (objects5[num51].GetComponent<GameBomb>() != null || objects5[num51].GetComponent<GameItem>() != null)
								{
									Vector3 vector10 = new Vector3(num44, 0f, num50);
									Vector3 vector11 = base.transform.localPosition - vector10;
									vector11.y = 0f;
									Vector3 vector12 = objects5[num51].transform.localPosition - vector10;
									vector12.y = 0f;
									float sqrMagnitude6 = vector11.sqrMagnitude;
									float sqrMagnitude7 = vector12.sqrMagnitude;
									if (sqrMagnitude6 == sqrMagnitude7)
									{
										if (base.gameObject.GetInstanceID() < objects5[num51].GetInstanceID())
										{
											objects5.RemoveAt(num51);
										}
									}
									else if (sqrMagnitude6 < sqrMagnitude7)
									{
										objects5.RemoveAt(num51);
									}
								}
							}
						}
						if (objects5 == null || objects5.Count > 0)
						{
							localPosition7.z = z;
						}
						else if (num50 != z && Mathf.Floor(localPosition7.z) < num50)
						{
							localPosition7.z = num50;
							inertia.y = 0f;
						}
					}
					else if (inertia.y > 0f)
					{
						float num52 = Mathf.Ceil(z);
						List<GameObject> objects6 = Global.Map.GetObjects(num44, num52, 0.499f);
						if (objects6 != null && objects6.Count > 0)
						{
							objects6.Remove(base.gameObject);
							for (int num53 = objects6.Count - 1; num53 >= 0; num53--)
							{
								if ((bool)objects6[num53].GetComponent<GameCharacter>())
								{
									objects6.RemoveAt(num53);
								}
								else if (objects6[num53].GetComponent<GameBomb>() != null || objects6[num53].GetComponent<GameItem>() != null)
								{
									Vector3 vector13 = new Vector3(num44, 0f, num52);
									Vector3 vector14 = base.transform.localPosition - vector13;
									vector14.y = 0f;
									Vector3 vector15 = objects6[num53].transform.localPosition - vector13;
									vector15.y = 0f;
									float sqrMagnitude8 = vector14.sqrMagnitude;
									float sqrMagnitude9 = vector15.sqrMagnitude;
									if (sqrMagnitude8 == sqrMagnitude9)
									{
										if (base.gameObject.GetInstanceID() < objects6[num53].GetInstanceID())
										{
											objects6.RemoveAt(num53);
										}
									}
									else if (sqrMagnitude8 < sqrMagnitude9)
									{
										objects6.RemoveAt(num53);
									}
								}
							}
						}
						if (objects6 == null || objects6.Count > 0)
						{
							localPosition7.z = z;
						}
						else if (num52 != z && Mathf.Ceil(localPosition7.z) > num52)
						{
							localPosition7.z = num52;
							inertia.y = 0f;
						}
					}
					base.transform.localPosition = localPosition7;
				}
				else if (localPosition7.x == num44 && localPosition7.z == num45)
				{
					if (externalForce != Vector2.zero)
					{
						if (externalForce.x < 0f)
						{
							List<GameObject> objects7 = Global.Map.GetObjects(num44 - 1f, num45, 0.5f);
							if (objects7 != null && objects7.Count > 0)
							{
								objects7.Remove(base.gameObject);
								for (int num54 = objects7.Count - 1; num54 >= 0; num54--)
								{
									if (objects7[num54].GetComponent<GameCharacter>() != null)
									{
										objects7.RemoveAt(num54);
									}
								}
							}
							if (objects7 == null || objects7.Count > 0)
							{
								externalForce.x = 0f;
							}
						}
						else if (externalForce.x > 0f)
						{
							List<GameObject> objects8 = Global.Map.GetObjects(num44 + 1f, num45, 0.5f);
							if (objects8 != null && objects8.Count > 0)
							{
								objects8.Remove(base.gameObject);
								for (int num55 = objects8.Count - 1; num55 >= 0; num55--)
								{
									if (objects8[num55].GetComponent<GameCharacter>() != null)
									{
										objects8.RemoveAt(num55);
									}
								}
							}
							if (objects8 == null || objects8.Count > 0)
							{
								externalForce.x = 0f;
							}
						}
						if (externalForce.y < 0f)
						{
							List<GameObject> objects9 = Global.Map.GetObjects(num44, num45 - 1f, 0.5f);
							if (objects9 != null && objects9.Count > 0)
							{
								objects9.Remove(base.gameObject);
								for (int num56 = objects9.Count - 1; num56 >= 0; num56--)
								{
									if (objects9[num56].GetComponent<GameCharacter>() != null)
									{
										objects9.RemoveAt(num56);
									}
								}
							}
							if (objects9 == null || objects9.Count > 0)
							{
								externalForce.y = 0f;
							}
						}
						else if (externalForce.y > 0f)
						{
							List<GameObject> objects10 = Global.Map.GetObjects(num44, num45 + 1f, 0.5f);
							if (objects10 != null && objects10.Count > 0)
							{
								objects10.Remove(base.gameObject);
								for (int num57 = objects10.Count - 1; num57 >= 0; num57--)
								{
									if (objects10[num57].GetComponent<GameCharacter>() != null)
									{
										objects10.RemoveAt(num57);
									}
								}
							}
							if (objects10 == null || objects10.Count > 0)
							{
								externalForce.y = 0f;
							}
						}
						if (externalForce.x != 0f || externalForce.y != 0f)
						{
							if (Mathf.Abs(externalForce.x) < Mathf.Abs(externalForce.y))
							{
								externalForce.x = 0f;
							}
							else
							{
								externalForce.y = 0f;
							}
							inertia = externalForce;
							externalForce = Vector2.zero;
						}
					}
				}
				else
				{
					float num58 = 10f * Time.deltaTime;
					if (localPosition7.x != num44)
					{
						float num59 = num44 - localPosition7.x;
						if (Mathf.Abs(num59) > num58)
						{
							num59 = ((!(num59 > 0f)) ? (0f - num58) : num58);
						}
						localPosition7.x += num59;
					}
					if (localPosition7.z != num45)
					{
						float num60 = num45 - localPosition7.z;
						if (Mathf.Abs(num60) > num58)
						{
							num60 = ((!(num60 > 0f)) ? (0f - num58) : num58);
						}
						localPosition7.z += num60;
					}
					base.transform.localPosition = localPosition7;
				}
			}
		}
		if (flag)
		{
			if (timeFuse > 0f)
			{
				switch (type)
				{
				case Bomb.XtraPillar:
				{
					GameBomb gameBomb = null;
					for (int m = 0; m < allBombs.Count; m++)
					{
						if (allBombs[m].type == Bomb.XtraPillar)
						{
							gameBomb = allBombs[m];
							break;
						}
					}
					if (owner.IsDead)
					{
						timeFuse = 4f;
						return;
					}
					if (this != gameBomb)
					{
						timeFuse = gameBomb.timeFuse;
						return;
					}
					break;
				}
				case Bomb.XtraCharge:
					if (timeFuse - Time.deltaTime < 8f && timeFuse >= 8f)
					{
						GameSound.StartSFX("skillMagic");
					}
					break;
				}
				if (type != Bomb.Remote && flyDir == Direction.None && flyVec.y != 0f && localPosition7.y == 0f)
				{
					timeFuse -= Time.deltaTime;
					if (timeFuse < 0f)
					{
						timeFuse = 0f;
						shootDir = Direction.None;
					}
				}
			}
			else if ((Global.Mode != GameMode.OnlineBattle || TNManager.isHosting) && IsLanded)
			{
				Explode();
			}
		}
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			if (sendDelay < 0.2f)
			{
				sendDelay += Time.deltaTime;
				return;
			}
			sendDelay = 0f;
			GetComponent<TNObject>().SendQuickly(60, Target.Others, Global.onlineMatchID, base.transform.localPosition, dead, timeFuse, base.transform.parent.name == "Bombs", flyDir == Direction.None && flyVec.y != 0f, shootDir == Direction.None, externalForce, inertia);
		}
	}

	[RFC(60)]
	private void GameBombUpdatePhysic(int matchID, Vector3 localPosition, bool dead, float timeFuse, bool inGrpBombs, bool isNotFly, bool isNotShoot, Vector2 externalForce, Vector2 inertia)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (isNotFly)
			{
				flyDir = Direction.None;
				flyVec.y = 1f;
			}
			if (isNotShoot)
			{
				shootDir = Direction.None;
			}
			if (inGrpBombs && flyDir == Direction.None && flyVec.y != 0f && shootDir == Direction.None)
			{
				base.transform.localPosition = localPosition;
			}
			this.dead = dead;
			this.timeFuse = timeFuse;
			this.externalForce = externalForce;
			this.inertia = inertia;
		}
	}

	[RFC(62)]
	private void GameBombFlyX(int matchID, Vector3 p, Vector3 target)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			if (base.transform.parent.name != "Bombs")
			{
				base.transform.parent = GameObject.Find("Script/Bombs").transform;
			}
			base.transform.localPosition = p;
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			base.transform.localScale = new Vector3(10f, 10f, 10f);
			base.GetComponent<Collider>().enabled = true;
			model.GetComponent<Collider>().enabled = true;
			flySyncP = p;
		}
		target.x = Mathf.Round(target.x);
		target.y = 0f;
		target.z = Mathf.Round(target.z);
		if (target.x > 8f)
		{
			target.x = 8f;
		}
		else if (target.x < -8f)
		{
			target.x = -8f;
		}
		if (target.z > 5f)
		{
			target.z = 5f;
		}
		else if (target.z < -5f)
		{
			target.z = -5f;
		}
		p.y = 0f;
		flyDir = Direction.None;
		flyVec = target - p;
		flyMove = 0f;
		flyRange = flyVec.magnitude;
		if (Mathf.Abs(flyVec.y) > Mathf.Abs(flyVec.x))
		{
			flyOld = ((!(flyVec.y > 0f)) ? Direction.Down : Direction.Up);
		}
		else
		{
			flyOld = ((!(flyVec.x > 0f)) ? Direction.Left : Direction.Right);
		}
		if (shootDir != Direction.None)
		{
			shootDir = Direction.None;
		}
	}

	[RFC(61)]
	private void GameBombFly(int matchID, Vector3 p, int direct, int range, bool sfx)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			if (base.transform.parent.name != "Bombs")
			{
				base.transform.parent = GameObject.Find("Script/Bombs").transform;
			}
			base.transform.localPosition = p;
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			base.transform.localScale = new Vector3(10f, 10f, 10f);
			base.GetComponent<Collider>().enabled = true;
			model.GetComponent<Collider>().enabled = true;
			flySyncP = p;
		}
		flyOld = (Direction)direct;
		flyDir = (Direction)direct;
		flyVec.y = 1f;
		flyMove = 0f;
		flyRange = range;
		if (shootDir != Direction.None)
		{
			shootDir = Direction.None;
		}
		if (sfx)
		{
			GameSound.StartSFX("objBounce");
		}
	}

	public void Fly(Vector3 target)
	{
		if (timeFuse == 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(62, Target.Others, Global.onlineMatchID, localPosition, target);
				GameBombFlyX(Global.onlineMatchID, localPosition, target);
			}
		}
		else
		{
			GameBombFlyX(Global.onlineMatchID, localPosition, target);
		}
	}

	public void Fly(Direction direct, int range, bool sfx = false)
	{
		if (timeFuse == 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(61, Target.Others, Global.onlineMatchID, localPosition, (int)direct, range, sfx);
				GameBombFly(Global.onlineMatchID, localPosition, (int)direct, range, sfx);
			}
		}
		else
		{
			GameBombFly(Global.onlineMatchID, localPosition, (int)direct, range, sfx);
		}
	}

	[RFC(63)]
	private void GameBombShoot(int matchID, Vector3 p, int direct)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			if (base.transform.parent.name != "Bombs")
			{
				base.transform.parent = GameObject.Find("Script/Bombs").transform;
			}
			base.transform.localPosition = p;
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			base.transform.localScale = new Vector3(10f, 10f, 10f);
			base.GetComponent<Collider>().enabled = true;
			model.GetComponent<Collider>().enabled = true;
			shootSyncP = p;
		}
		shootKick = direct != (int)shootDir;
		shootDir = (Direction)direct;
		if (flyDir != Direction.None)
		{
			flyDir = Direction.None;
		}
	}

	public void Shoot(Direction direct)
	{
		if (timeFuse == 0f)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(63, Target.Others, Global.onlineMatchID, localPosition, (int)direct);
				GameBombShoot(Global.onlineMatchID, localPosition, (int)direct);
			}
		}
		else
		{
			GameBombShoot(Global.onlineMatchID, localPosition, (int)direct);
		}
	}

	[RFC(64)]
	private void GameBombExplode(int matchID, int hitFrom)
	{
		if (Global.onlineMatchID == matchID)
		{
			dead = true;
			GameShadow.RemoveObject(base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
			if (hitFrom == 540)
			{
				GameSound.StartSFX("bombBlast");
			}
		}
	}

	public bool Explode(Direction hitFrom = Direction.None)
	{
		if (dead)
		{
			return false;
		}
		if (flyDir != Direction.None || flyVec.y == 0f)
		{
			return false;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (localPosition.y != 0f)
		{
			return false;
		}
		if (type == Bomb.XtraRing && timeFuse > 0f)
		{
			return false;
		}
		if (type > Bomb.MAX)
		{
			dead = true;
			GameShadow.RemoveObject(base.gameObject);
			switch (type)
			{
			case Bomb.XtraPlant:
				model.GetComponent<Collider>().enabled = false;
				base.GetComponent<Collider>().enabled = false;
				UnityEngine.Object.Destroy(model.transform.Find("SporeBomb").gameObject);
				UnityEngine.Object.Destroy(base.gameObject, 2.5f);
				model.GetComponentInChildren<ParticleSystem>().Stop();
				break;
			case Bomb.XtraPillar:
				if (timeFuse == 0f)
				{
					base.GetComponent<Collider>().enabled = false;
					UnityEngine.Object.Destroy(model);
					Global.Map.Quake(1f);
					break;
				}
				base.GetComponent<Collider>().enabled = false;
				UnityEngine.Object.Destroy(base.gameObject);
				return true;
			case Bomb.XtraCharge:
				base.GetComponent<Collider>().enabled = false;
				UnityEngine.Object.Destroy(model);
				break;
			case Bomb.XtraLine:
			{
				Global.Map.Quake(0.5f);
				model.GetComponent<Collider>().enabled = false;
				base.GetComponent<Collider>().enabled = false;
				UnityEngine.Object.Destroy(model.transform.Find("FireBomb").gameObject);
				UnityEngine.Object.Destroy(base.gameObject, 2.5f);
				ParticleSystem[] componentsInChildren = model.GetComponentsInChildren<ParticleSystem>();
				ParticleSystem[] array = componentsInChildren;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Stop();
				}
				break;
			}
			case Bomb.XtraRing:
			{
				base.GetComponent<Collider>().enabled = false;
				model.GetComponent<Collider>().enabled = false;
				GameObject gameObject = model.transform.Find("Lightning").gameObject;
				GameObject gameObject2 = model.transform.Find("PlasmaFXs").gameObject;
				gameObject.SetActive(true);
				gameObject2.SetActive(true);
				gameObject.GetComponent<Animation>().Play();
				UnityEngine.Object.Destroy(gameObject, 1f);
				break;
			}
			default:
				UnityEngine.Object.Destroy(base.gameObject);
				break;
			}
		}
		else if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(64, Target.Others, Global.onlineMatchID, (int)hitFrom);
				GameBombExplode(Global.onlineMatchID, (int)hitFrom);
			}
		}
		else
		{
			GameBombExplode(Global.onlineMatchID, (int)hitFrom);
		}
		if (type > Bomb.MAX)
		{
			int radius = 0;
			float fireDuration = 0.25f;
			float num = 1f;
			float fxDuration = 1f;
			switch (type)
			{
			case Bomb.XtraPillar:
				fxDuration = 2.5f;
				radius = (int)fireLv;
				break;
			case Bomb.XtraCharge:
				fxDuration = 2.5f;
				num = ChargeSize / 2f * 1.125f;
				break;
			case Bomb.XtraLine:
				fxDuration = 15f;
				radius = 16;
				fireDuration = 12.5f;
				break;
			}
			Global.Map.Explode(-1, Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z), radius, true, Direction.None, fxBomb, num, fxDuration, fireDuration);
			switch (type)
			{
			case Bomb.XtraPlant:
			{
				GameObject gameObject3 = GameCharacter.Create(Character.Minion, 1, 1);
				gameObject3.transform.parent = owner.transform.parent;
				gameObject3.transform.localPosition = localPosition;
				gameObject3.transform.localScale = new Vector3(10f, 10f, 10f);
				gameObject3.GetComponent<Rigidbody>().isKinematic = true;
				GameObject gameObject4 = UnityEngine.Object.Instantiate(fxBomb) as GameObject;
				gameObject4.transform.parent = gameObject3.transform;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localScale = Vector3.one;
				break;
			}
			case Bomb.XtraPillar:
			{
				GameBomb gameBomb = null;
				for (int j = 0; j < allBombs.Count; j++)
				{
					if (allBombs[j].type == Bomb.XtraPillar)
					{
						gameBomb = allBombs[j];
						break;
					}
				}
				if (this != gameBomb)
				{
					return true;
				}
				Debug.Log("bomb! @" + Time.time);
				foreach (GameCharacter item in GameCharacter.List())
				{
					if (!item.IsDead && item.Type == Character.Minion)
					{
						item.Dead();
					}
				}
				break;
			}
			case Bomb.XtraCharge:
			{
				Debug.LogWarning("bomb!!! charge:" + Mathf.Max(1, Mathf.CeilToInt(5f - timeFuse)) + " @" + Time.time);
				Transform transform = base.transform.parent.parent.Find("FXs");
				for (int num2 = transform.childCount - 1; num2 >= 0; num2--)
				{
					Transform transform2 = transform.GetChild(num2).Find("AirBlast");
					if (transform2 != null)
					{
						ParticleSystem componentInChildren = transform2.parent.GetComponentInChildren<ParticleSystem>();
						float num3 = num * 2f;
						componentInChildren.startColor = new Color(1f, 1f, 1f, 0.125f + 0.125f * num3 / 31.5f);
						componentInChildren.startSpeed *= num3;
						componentInChildren.emissionRate *= num3;
						model = transform2.GetChild(0).gameObject;
						break;
					}
				}
				float num4 = num + 0.75f;
				float num5 = num4 * num4;
				List<GameCharacter> list = GameCharacter.List();
				for (int num6 = list.Count - 1; num6 >= 0; num6--)
				{
					Vector3 localPosition2 = list[num6].transform.localPosition;
					if (localPosition2.y == 0f && list[num6].Type == Character.Player && !list[num6].IsDead)
					{
						float num7 = localPosition.x - localPosition2.x;
						float num8 = localPosition.z - localPosition2.z;
						if (num7 * num7 + num8 * num8 < num5)
						{
							list[num6].Dead();
						}
					}
				}
				List<GameItem> list2 = GameItem.List();
				for (int num9 = list2.Count - 1; num9 >= 0; num9--)
				{
					Vector3 localPosition3 = list2[num9].transform.localPosition;
					if (localPosition3.y == 0f)
					{
						float num10 = localPosition.x - localPosition3.x;
						float num11 = localPosition.z - localPosition3.z;
						if (num10 * num10 + num11 * num11 < num5)
						{
							list2[num9].Deactive();
						}
					}
				}
				for (int num12 = allBombs.Count - 1; num12 >= 0; num12--)
				{
					Vector3 localPosition4 = allBombs[num12].transform.localPosition;
					if (localPosition4.y == 0f && allBombs[num12].type != Bomb.XtraCharge)
					{
						float num13 = localPosition.x - localPosition4.x;
						float num14 = localPosition.z - localPosition4.z;
						if (num13 * num13 + num14 * num14 < num5)
						{
							allBombs[num12].Explode();
						}
					}
				}
				Global.Map.Quake(num4 / 7.5f);
				break;
			}
			}
			GameSound.StartSFX("bombBlast");
		}
		else
		{
			Direction exclude = Direction.None;
			if (type != Bomb.Pierce)
			{
				switch (hitFrom)
				{
				case Direction.Left:
					exclude = Direction.Right;
					break;
				case Direction.Down:
					exclude = Direction.Up;
					break;
				case Direction.Right:
					exclude = Direction.Left;
					break;
				case Direction.Up:
					exclude = Direction.Down;
					break;
				}
			}
			Global.Map.Explode((!base.name.EndsWith("(Ghost)")) ? (-1) : owner.battleID, Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z), (int)fireLv, type == Bomb.Pierce, exclude, null, 1f);
		}
		return true;
	}

	public static List<GameBomb> List()
	{
		return allBombs;
	}

	public static void Init()
	{
		if (res == null)
		{
			res = new GameObject[3];
			for (int i = 0; i < res.Length; i++)
			{
				res[i] = Resources.Load("Bombs/Bomb" + (Bomb)i) as GameObject;
			}
		}
		if (fxType != Global.Level)
		{
			fxType = Global.Level;
			fxBomb = Resources.Load("Skills/" + fxType.ToString("D2") + "/Explosion") as GameObject;
			switch (fxType)
			{
			case 1:
				bossBomb = Resources.Load("Skills/01/PlantBomb") as GameObject;
				break;
			case 2:
				bossBomb = Resources.Load("Skills/02/PillarBomb") as GameObject;
				break;
			case 3:
				bossBomb = Resources.Load("Skills/03/ChargeBomb") as GameObject;
				break;
			case 4:
				bossBomb = Resources.Load("Skills/04/LineBomb") as GameObject;
				break;
			case 5:
				bossBomb = Resources.Load("Skills/05/RingBomb") as GameObject;
				break;
			default:
				bossBomb = Resources.Load("Bombs/BombNormal") as GameObject;
				break;
			}
		}
		if (allBombs == null)
		{
			allBombs = new List<GameBomb>();
		}
		else
		{
			allBombs.Clear();
		}
	}

	public static void Destroy()
	{
		res = null;
		fxType = 0;
		fxBomb = null;
		allBombs.Clear();
	}

	public static GameObject Create(Bomb type, Fire fire, GameCharacter owner, List<GameCharacter> ignoreList)
	{
		GameObject gameObject = new GameObject(string.Concat("Bomb (", type, ")"));
		if (owner.IsGhost)
		{
			gameObject.name += " (Ghost)";
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate((type <= Bomb.MAX) ? res[(int)type] : bossBomb) as GameObject;
		gameObject2.name = "root";
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.transform.localScale = Vector3.one;
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.center = new Vector3(0f, 0.05f, 0f);
		boxCollider.size = new Vector3(0.1f, 0.1f, 0.1f);
		boxCollider = gameObject2.AddComponent<BoxCollider>();
		boxCollider.center = new Vector3(0f, 0.05f, 0f);
		boxCollider.size = new Vector3(0.001f, 0.1f, 0.001f);
		GameBomb gameBomb = gameObject.AddComponent<GameBomb>();
		gameBomb.type = type;
		gameBomb.fireLv = fire;
		gameBomb.owner = owner;
		gameBomb.ignoreList = ignoreList;
		gameBomb.model = gameObject2;
		switch (type)
		{
		case Bomb.XtraPlant:
			gameBomb.timeFuse = 0.01f;
			break;
		case Bomb.XtraPillar:
			gameBomb.timeFuse = 5f;
			break;
		case Bomb.XtraCharge:
			gameBomb.timeFuse = 9f;
			break;
		case Bomb.XtraLine:
			gameBomb.timeFuse = 0.01f;
			break;
		case Bomb.XtraRing:
			gameBomb.timeFuse = 0.01f;
			break;
		}
		if (type == Bomb.XtraPillar && fire < (Fire)0)
		{
			gameBomb.fireLv = (Fire)(0 - fire);
			gameBomb.timeFuse -= 2.5f;
		}
		for (int num = ignoreList.Count - 1; num >= 0; num--)
		{
			if (ignoreList[num].GetComponent<Collider>().enabled)
			{
				if (gameObject.GetComponent<Collider>().enabled)
				{
					Physics.IgnoreCollision(ignoreList[num].GetComponent<Collider>(), gameObject.GetComponent<Collider>());
				}
				if (gameObject2.GetComponent<Collider>().enabled)
				{
					Physics.IgnoreCollision(ignoreList[num].GetComponent<Collider>(), gameObject2.GetComponent<Collider>());
				}
			}
		}
		List<GameCharacter> list = GameCharacter.List();
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if ((list[num2].Type == Character.Boss || list[num2].Type == Character.Monster) && !list[num2].IsDead)
			{
				Transform transform = list[num2].transform.Find("root");
				Physics.IgnoreCollision(transform.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
				Physics.IgnoreCollision(transform.GetComponent<Collider>(), gameObject2.GetComponent<Collider>());
			}
		}
		GameShadow.AddObject(gameObject);
		return gameObject;
	}

	public void AddIgnore(GameCharacter character)
	{
		if (ignoreList == null)
		{
			ignoreList = new List<GameCharacter>();
		}
		if (!(base.transform.parent.name == "Bombs") || !base.GetComponent<Collider>().enabled)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localPosition2 = character.transform.localPosition;
		float sqrMagnitude = (localPosition - localPosition2).sqrMagnitude;
		if (sqrMagnitude < 1f)
		{
			ignoreList.Add(character);
			if (sqrMagnitude <= 0.25f)
			{
				Physics.IgnoreCollision(character.GetComponent<Collider>(), model.GetComponent<Collider>(), true);
			}
			Physics.IgnoreCollision(character.GetComponent<Collider>(), base.GetComponent<Collider>(), true);
		}
		else
		{
			Physics.IgnoreCollision(character.GetComponent<Collider>(), model.GetComponent<Collider>(), false);
			Physics.IgnoreCollision(character.GetComponent<Collider>(), base.GetComponent<Collider>(), false);
		}
	}
}
