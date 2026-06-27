using TNet;
using UnityEngine;

public class GameItem : MonoBehaviour
{
	public const float Radius = 0.25f;

	private const float flySpeed = 7.5f;

	private const float flyGravity = 25f;

	public static bool Destroyable = true;

	private static GameObject res;

	private static Material[] mat;

	private static List<GameItem> allItems;

	private Direction flyOld = Direction.None;

	private Direction flyDir = Direction.None;

	private float flyMove;

	private int flyRange;

	private Item type;

	private bool deactive;

	public Vector2 externalForce;

	private Vector2 inertia;

	private float requestSend;

	private float sendDelay;

	public Direction FlyDirection
	{
		get
		{
			return flyDir;
		}
	}

	public bool IsLanded
	{
		get
		{
			return flyDir == Direction.None && base.transform.localPosition.y == 0f;
		}
	}

	public Item Type
	{
		get
		{
			return type;
		}
	}

	public bool IsDeactived
	{
		get
		{
			return deactive;
		}
	}

	private void Awake()
	{
		allItems.Add(this);
	}

	private void OnDestroy()
	{
		allItems.Remove(this);
	}

	[RFC(53)]
	private void GameItemOnDestroy(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			GameShadow.RemoveObject(base.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (flyDir != Direction.None)
		{
			float num = 7.5f * Time.deltaTime;
			flyMove += num;
			if (flyMove > (float)flyRange)
			{
				flyMove = flyRange;
			}
			switch (flyDir)
			{
			case Direction.Left:
				localPosition.x -= num;
				break;
			case Direction.Down:
				localPosition.z -= num;
				break;
			case Direction.Right:
				localPosition.x += num;
				break;
			case Direction.Up:
				localPosition.z += num;
				break;
			}
			if (localPosition.x > 13f)
			{
				localPosition.x -= 26f;
			}
			else if (localPosition.x < -13f)
			{
				localPosition.x += 26f;
			}
			if (localPosition.z > 10f)
			{
				localPosition.z -= 20f;
			}
			else if (localPosition.z < -10f)
			{
				localPosition.z += 20f;
			}
			float num2 = flyMove / (float)flyRange * 2f - 1f;
			localPosition.y = 1f + (float)flyRange / 1.5f * (1f - num2 * num2);
			if (flyMove == (float)flyRange)
			{
				localPosition.x = Mathf.Round(localPosition.x);
				localPosition.y = 1f;
				localPosition.z = Mathf.Round(localPosition.z);
				List<GameObject> objects = Global.Map.GetObjects(localPosition.x, localPosition.z, 0.25f);
				if (objects != null)
				{
					objects.Remove(base.gameObject);
					if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
					{
						for (int num3 = objects.Count - 1; num3 >= 0; num3--)
						{
							GameCharacter component = objects[num3].GetComponent<GameCharacter>();
							if (component != null && !component.IsDead)
							{
								component.GetItem(type);
								flyDir = Direction.None;
								Deactive();
								break;
							}
						}
					}
				}
				if (!deactive)
				{
					if (objects != null && objects.Count == 0)
					{
						flyDir = Direction.None;
					}
					else if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
					{
						int range = 1;
						switch (flyDir)
						{
						case Direction.Left:
							range = ((!(localPosition.x < -8f)) ? 1 : ((int)(18f + localPosition.x)));
							break;
						case Direction.Down:
							range = ((!(localPosition.z < -5f)) ? 1 : ((int)(15f + localPosition.z)));
							break;
						case Direction.Right:
							range = ((!(localPosition.x > 8f)) ? 1 : ((int)(18f - localPosition.x)));
							break;
						case Direction.Up:
							range = ((!(localPosition.z > 5f)) ? 1 : ((int)(15f - localPosition.z)));
							break;
						}
						Fly(flyDir, range, true);
					}
				}
			}
			base.transform.localPosition = localPosition;
			externalForce = Vector2.zero;
		}
		else if (localPosition.y > 0f)
		{
			localPosition.y -= 25f * Time.deltaTime;
			if (localPosition.y < 0f)
			{
				localPosition.y = 0f;
			}
			else if (localPosition.y < 1f && (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting))
			{
				List<GameBomb> list = GameBomb.List();
				for (int num4 = list.Count - 1; num4 >= 0; num4--)
				{
					if (!list[num4].IsExploded && !list[num4].IsHeld)
					{
						Vector3 localPosition2 = list[num4].transform.localPosition;
						if (Mathf.Abs(localPosition.x - localPosition2.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition2.z) < 0.5f && localPosition2.y < 1f)
						{
							localPosition.y = 1f;
							Fly(flyOld, 1, true);
							break;
						}
					}
				}
				if (localPosition.y < 1f)
				{
					for (int num5 = allItems.Count - 1; num5 >= 0; num5--)
					{
						if (allItems[num5] != this && !allItems[num5].IsDeactived)
						{
							Vector3 localPosition3 = allItems[num5].transform.localPosition;
							if (Mathf.Abs(localPosition.x - localPosition3.x) < 0.5f && Mathf.Abs(localPosition.z - localPosition3.z) < 0.5f && localPosition3.y < 1f)
							{
								localPosition.y = 1f;
								Fly(flyOld, 1, true);
								break;
							}
						}
					}
				}
			}
			base.transform.localPosition = localPosition;
		}
		else
		{
			float num6 = Mathf.Round(localPosition.x);
			float num7 = Mathf.Round(localPosition.z);
			if (inertia != Vector2.zero)
			{
				float x = localPosition.x;
				float z = localPosition.z;
				localPosition.x += inertia.x * Time.deltaTime;
				localPosition.y = 0f;
				localPosition.z += inertia.y * Time.deltaTime;
				if (inertia.x < 0f)
				{
					float num8 = Mathf.Floor(x);
					List<GameObject> objects2 = Global.Map.GetObjects(num8, num7, 0.499f);
					if (objects2 != null && objects2.Count > 0)
					{
						objects2.Remove(base.gameObject);
						for (int num9 = objects2.Count - 1; num9 >= 0; num9--)
						{
							if (objects2[num9].GetComponent<GameCharacter>() != null)
							{
								objects2.RemoveAt(num9);
							}
							else if (objects2[num9].GetComponent<GameBomb>() != null || objects2[num9].GetComponent<GameItem>() != null)
							{
								Vector3 vector = new Vector3(num8, 0f, num7);
								Vector3 vector2 = base.transform.localPosition - vector;
								vector2.y = 0f;
								Vector3 vector3 = objects2[num9].transform.localPosition - vector;
								vector3.y = 0f;
								float sqrMagnitude = vector2.sqrMagnitude;
								float sqrMagnitude2 = vector3.sqrMagnitude;
								if (sqrMagnitude == sqrMagnitude2)
								{
									if (base.gameObject.GetInstanceID() < objects2[num9].GetInstanceID())
									{
										objects2.RemoveAt(num9);
									}
								}
								else if (sqrMagnitude < sqrMagnitude2)
								{
									objects2.RemoveAt(num9);
								}
							}
						}
					}
					if (objects2 == null || objects2.Count > 0)
					{
						localPosition.x = x;
					}
					else if (num8 != x && Mathf.Floor(localPosition.x) < num8)
					{
						localPosition.x = num8;
						inertia.x = 0f;
					}
				}
				else if (inertia.x > 0f)
				{
					float num10 = Mathf.Ceil(x);
					List<GameObject> objects3 = Global.Map.GetObjects(num10, num7, 0.499f);
					if (objects3 != null && objects3.Count > 0)
					{
						objects3.Remove(base.gameObject);
						for (int num11 = objects3.Count - 1; num11 >= 0; num11--)
						{
							if (objects3[num11].GetComponent<GameCharacter>() != null)
							{
								objects3.RemoveAt(num11);
							}
							else if (objects3[num11].GetComponent<GameBomb>() != null || objects3[num11].GetComponent<GameItem>() != null)
							{
								Vector3 vector4 = new Vector3(num10, 0f, num7);
								Vector3 vector5 = base.transform.localPosition - vector4;
								vector5.y = 0f;
								Vector3 vector6 = objects3[num11].transform.localPosition - vector4;
								vector6.y = 0f;
								float sqrMagnitude3 = vector5.sqrMagnitude;
								float sqrMagnitude4 = vector6.sqrMagnitude;
								if (sqrMagnitude3 == sqrMagnitude4)
								{
									if (base.gameObject.GetInstanceID() < objects3[num11].GetInstanceID())
									{
										objects3.RemoveAt(num11);
									}
								}
								else if (sqrMagnitude3 < sqrMagnitude4)
								{
									objects3.RemoveAt(num11);
								}
							}
						}
					}
					if (objects3 == null || objects3.Count > 0)
					{
						localPosition.x = x;
					}
					else if (num10 != x && Mathf.Ceil(localPosition.x) > num10)
					{
						localPosition.x = num10;
						inertia.x = 0f;
					}
				}
				if (inertia.y < 0f)
				{
					float num12 = Mathf.Floor(z);
					List<GameObject> objects4 = Global.Map.GetObjects(num6, num12, 0.499f);
					if (objects4 != null && objects4.Count > 0)
					{
						objects4.Remove(base.gameObject);
						for (int num13 = objects4.Count - 1; num13 >= 0; num13--)
						{
							if (objects4[num13].GetComponent<GameCharacter>() != null)
							{
								objects4.RemoveAt(num13);
							}
							else if (objects4[num13].GetComponent<GameBomb>() != null || objects4[num13].GetComponent<GameItem>() != null)
							{
								Vector3 vector7 = new Vector3(num6, 0f, num12);
								Vector3 vector8 = base.transform.localPosition - vector7;
								vector8.y = 0f;
								Vector3 vector9 = objects4[num13].transform.localPosition - vector7;
								vector9.y = 0f;
								float sqrMagnitude5 = vector8.sqrMagnitude;
								float sqrMagnitude6 = vector9.sqrMagnitude;
								if (sqrMagnitude5 == sqrMagnitude6)
								{
									if (base.gameObject.GetInstanceID() < objects4[num13].GetInstanceID())
									{
										objects4.RemoveAt(num13);
									}
								}
								else if (sqrMagnitude5 < sqrMagnitude6)
								{
									objects4.RemoveAt(num13);
								}
							}
						}
					}
					if (objects4 == null || objects4.Count > 0)
					{
						localPosition.z = z;
					}
					else if (num12 != z && Mathf.Floor(localPosition.z) < num12)
					{
						localPosition.z = num12;
						inertia.y = 0f;
					}
				}
				else if (inertia.y > 0f)
				{
					float num14 = Mathf.Ceil(z);
					List<GameObject> objects5 = Global.Map.GetObjects(num6, num14, 0.499f);
					if (objects5 != null && objects5.Count > 0)
					{
						objects5.Remove(base.gameObject);
						for (int num15 = objects5.Count - 1; num15 >= 0; num15--)
						{
							if (objects5[num15].GetComponent<GameCharacter>() != null)
							{
								objects5.RemoveAt(num15);
							}
							else if (objects5[num15].GetComponent<GameBomb>() != null || objects5[num15].GetComponent<GameItem>() != null)
							{
								Vector3 vector10 = new Vector3(num6, 0f, num14);
								Vector3 vector11 = base.transform.localPosition - vector10;
								vector11.y = 0f;
								Vector3 vector12 = objects5[num15].transform.localPosition - vector10;
								vector12.y = 0f;
								float sqrMagnitude7 = vector11.sqrMagnitude;
								float sqrMagnitude8 = vector12.sqrMagnitude;
								if (sqrMagnitude7 == sqrMagnitude8)
								{
									if (base.gameObject.GetInstanceID() < objects5[num15].GetInstanceID())
									{
										objects5.RemoveAt(num15);
									}
								}
								else if (sqrMagnitude7 < sqrMagnitude8)
								{
									objects5.RemoveAt(num15);
								}
							}
						}
					}
					if (objects5 == null || objects5.Count > 0)
					{
						localPosition.z = z;
					}
					else if (num14 != z && Mathf.Ceil(localPosition.z) > num14)
					{
						localPosition.z = num14;
						inertia.y = 0f;
					}
				}
				base.transform.localPosition = localPosition;
			}
			else if (localPosition.x == num6 && localPosition.z == num7)
			{
				if (externalForce != Vector2.zero)
				{
					if (externalForce.x < 0f)
					{
						List<GameObject> objects6 = Global.Map.GetObjects(num6 - 1f, num7, 0.5f);
						if (objects6 != null && objects6.Count > 0)
						{
							objects6.Remove(base.gameObject);
							for (int num16 = objects6.Count - 1; num16 >= 0; num16--)
							{
								if (objects6[num16].GetComponent<GameCharacter>() != null)
								{
									objects6.RemoveAt(num16);
								}
							}
						}
						if (objects6 == null || objects6.Count > 0)
						{
							externalForce.x = 0f;
						}
					}
					else if (externalForce.x > 0f)
					{
						List<GameObject> objects7 = Global.Map.GetObjects(num6 + 1f, num7, 0.5f);
						if (objects7 != null && objects7.Count > 0)
						{
							objects7.Remove(base.gameObject);
							for (int num17 = objects7.Count - 1; num17 >= 0; num17--)
							{
								if (objects7[num17].GetComponent<GameCharacter>() != null)
								{
									objects7.RemoveAt(num17);
								}
							}
						}
						if (objects7 == null || objects7.Count > 0)
						{
							externalForce.x = 0f;
						}
					}
					if (externalForce.y < 0f)
					{
						List<GameObject> objects8 = Global.Map.GetObjects(num6, num7 - 1f, 0.5f);
						if (objects8 != null && objects8.Count > 0)
						{
							objects8.Remove(base.gameObject);
							for (int num18 = objects8.Count - 1; num18 >= 0; num18--)
							{
								if (objects8[num18].GetComponent<GameCharacter>() != null)
								{
									objects8.RemoveAt(num18);
								}
							}
						}
						if (objects8 == null || objects8.Count > 0)
						{
							externalForce.y = 0f;
						}
					}
					else if (externalForce.y > 0f)
					{
						List<GameObject> objects9 = Global.Map.GetObjects(num6, num7 + 1f, 0.5f);
						if (objects9 != null && objects9.Count > 0)
						{
							objects9.Remove(base.gameObject);
							for (int num19 = objects9.Count - 1; num19 >= 0; num19--)
							{
								if (objects9[num19].GetComponent<GameCharacter>() != null)
								{
									objects9.RemoveAt(num19);
								}
							}
						}
						if (objects9 == null || objects9.Count > 0)
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
				float num20 = 10f * Time.deltaTime;
				if (localPosition.x != num6)
				{
					float num21 = num6 - localPosition.x;
					if (Mathf.Abs(num21) > num20)
					{
						num21 = ((!(num21 > 0f)) ? (0f - num20) : num20);
					}
					localPosition.x += num21;
				}
				if (localPosition.z != num7)
				{
					float num22 = num7 - localPosition.z;
					if (Mathf.Abs(num22) > num20)
					{
						num22 = ((!(num22 > 0f)) ? (0f - num20) : num20);
					}
					localPosition.z += num22;
				}
				base.transform.localPosition = localPosition;
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
			GetComponent<TNObject>().SendQuickly(50, Target.Others, Global.onlineMatchID, base.transform.localPosition, deactive, flyDir == Direction.None, externalForce, inertia);
		}
	}

	[RFC(50)]
	private void GameItemUpdatePhysic(int matchID, Vector3 localPosition, bool deactive, bool isNotFly, Vector2 externalForce, Vector2 inertia)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (isNotFly)
			{
				flyDir = Direction.None;
			}
			if (flyDir == Direction.None)
			{
				base.transform.localPosition = localPosition;
			}
			this.deactive = deactive;
			this.externalForce = externalForce;
			this.inertia = inertia;
		}
	}

	[RFC(51)]
	private void GameItemFly(int matchID, Vector3 p, int direct, int range, bool sfx)
	{
		if (Global.onlineMatchID == matchID)
		{
			if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
			{
				base.transform.localPosition = p;
				Debug.Log("Item fly from " + p);
			}
			flyOld = (Direction)direct;
			flyDir = (Direction)direct;
			flyMove = 0f;
			flyRange = range;
			if (sfx)
			{
				GameSound.StartSFX("objBounce");
			}
		}
	}

	public void Fly(Direction direct, int range, bool sfx = false)
	{
		Vector3 localPosition = base.transform.localPosition;
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(51, Target.Others, Global.onlineMatchID, localPosition, (int)direct, range, sfx);
				GameItemFly(Global.onlineMatchID, localPosition, (int)direct, range, sfx);
			}
		}
		else
		{
			GameItemFly(Global.onlineMatchID, localPosition, (int)direct, range, sfx);
		}
	}

	[RFC(52)]
	private void GameItemDeactive(int matchID, Vector3 p, bool sfx)
	{
		if (Global.onlineMatchID == matchID)
		{
			deactive = true;
			base.GetComponent<Collider>().enabled = false;
			p.y = 1f;
			base.transform.localPosition = p;
			GameShadow.RemoveObject(base.gameObject);
			Object.Destroy(base.gameObject);
			if (sfx)
			{
				GameSound.StartSFX("itemDestroy");
			}
		}
	}

	public void Deactive(bool sfx = true)
	{
		if (deactive || flyDir != Direction.None)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (Global.Mode == GameMode.OnlineBattle)
		{
			if (TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(52, Target.Others, Global.onlineMatchID, localPosition, sfx);
				GameItemDeactive(Global.onlineMatchID, localPosition, sfx);
			}
		}
		else
		{
			GameItemDeactive(Global.onlineMatchID, localPosition, sfx);
		}
	}

	public static List<GameItem> List()
	{
		return allItems;
	}

	public static void Init()
	{
		if (res == null)
		{
			res = Resources.Load("Items/Item") as GameObject;
			mat = new Material[13];
			Shader shader = Shader.Find("Unlit/Texture");
			for (int i = 0; i < mat.Length; i++)
			{
				mat[i] = new Material(shader);
				mat[i].name = string.Concat("item (", (Item)i, ")");
				mat[i].mainTexture = Resources.Load("Items/Textures/item-" + (Item)i) as Texture;
			}
		}
		if (allItems == null)
		{
			allItems = new List<GameItem>();
		}
		else
		{
			allItems.Clear();
		}
	}

	public static void Destroy()
	{
		res = null;
		mat = null;
		allItems.Clear();
	}

	public static GameObject Create(Item type)
	{
		GameObject gameObject = Object.Instantiate(res) as GameObject;
		gameObject.name = string.Concat("Item (", type, ")");
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		for (int num = componentsInChildren.Length - 1; num >= 0; num--)
		{
			componentsInChildren[num].material = mat[(int)type];
		}
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.center = new Vector3(0f, 0.05f, 0f);
		boxCollider.size = new Vector3(0.05f, 0.1f, 0.05f);
		GameItem gameItem = gameObject.AddComponent<GameItem>();
		gameItem.type = type;
		GameShadow.AddObject(gameObject);
		return gameObject;
	}
}
