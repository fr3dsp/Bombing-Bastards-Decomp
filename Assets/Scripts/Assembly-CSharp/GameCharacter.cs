using TNet;
using UnityEngine;

public abstract class GameCharacter : MonoBehaviour
{
	public const float Radius = 0.5f;

	private static GameObject res;

	private static Material[] mat;

	private static GameObject resGhost;

	private static Material[] matGhost;

	private static int resMonLv;

	private static bool resMonBoss;

	private static GameObject[] resMons;

	private static GameObject stunFX;

	private static GameObject virusFX;

	protected static List<GameCharacter> allChars;

	protected GameCharacter reborn;

	protected GameObject model;

	protected GameObject ghostModel;

	private Vector3 lastPosition;

	protected Vector2 direction;

	protected int bombMaxN = 1;

	protected Bomb bombType;

	protected GameBomb bombHold;

	protected Fire fireLv = Fire.Lv1;

	protected Speed speed = Speed.Lv1;

	protected Virus infect;

	protected float infectT;

	protected float stunTime;

	protected bool dead;

	protected bool isGhost;

	protected Vector3 shootPos = Vector3.zero;

	protected int shootStep;

	protected Vector3 posReborn = Vector3.zero;

	protected bool isReborn;

	protected Character type;

	protected bool hasBombPass;

	protected bool hasGlove;

	protected bool hasKick;

	private Transform head;

	private Transform handL;

	private Transform handR;

	private List<Item> drops;

	protected float blink;

	private Vector3 blinkScale;

	public int battleID;

	public int controllerID = -1;

	public Vector2 externalForce;

	private Vector3 fixedMove;

	private Vector3 fixedExt;

	private float sendDelay;

	public bool IsStunned
	{
		get
		{
			return stunTime > 0f;
		}
	}

	public bool IsGhost
	{
		get
		{
			return isGhost;
		}
	}

	public bool IsDead
	{
		get
		{
			return dead;
		}
	}

	public Character Type
	{
		get
		{
			return type;
		}
	}

	protected abstract void Init(int param);

	private void Awake()
	{
		allChars.Add(this);
	}

	private void Start()
	{
		lastPosition = base.transform.localPosition;
	}

	private void OnDestroy()
	{
		allChars.Remove(this);
	}

	protected abstract float ModSpeed(float rawSpeed);

	protected abstract void ActionUpdate();

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		Rigidbody rigidbody = base.GetComponent<Rigidbody>();
		if (!rigidbody.isKinematic)
		{
			rigidbody.velocity = Vector3.zero;
		}
		if (isGhost)
		{
			Vector3 vector2 = Vector3.zero;
			float num = 1000f;
			Quaternion localRotation = ghostModel.transform.localRotation;
			Vector3 localPosition = base.transform.localPosition;
			if (Global.mapShrink && Global.Map.RemainTime < 31f)
			{
				if (type == Character.Player)
				{
					GamePointer.EnablePointer(false, (Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
				}
				if (localPosition.x >= 9f)
				{
					float x = Mathf.Lerp(10f, 14f, localPosition.x - 10f + deltaTime / 4f);
					base.transform.localPosition = new Vector3(x, 0f, localPosition.z);
				}
				else if (localPosition.x <= -9f)
				{
					float x2 = Mathf.Lerp(-10f, -14f, 0f - localPosition.x - 10f + deltaTime / 4f);
					base.transform.localPosition = new Vector3(x2, 0f, localPosition.z);
				}
				else if (localPosition.z >= 6f)
				{
					float z = Mathf.Lerp(6.5f, 11f, localPosition.z - 6.5f + deltaTime / 4f);
					base.transform.localPosition = new Vector3(localPosition.x, 0f, z);
				}
				else if (localPosition.z <= -7f)
				{
					float z2 = Mathf.Lerp(-7f, -11f, 0f - localPosition.z - 7f + deltaTime / 4f);
					base.transform.localPosition = new Vector3(localPosition.x, 0f, z2);
				}
				if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
				{
					RequestSend();
				}
				fixedMove = vector;
				fixedExt = zero;
				return;
			}
			if (isReborn)
			{
				if (localPosition.x >= 9f)
				{
					float x3 = Mathf.Lerp(10f, 14f, blink += deltaTime);
					base.transform.localPosition = new Vector3(x3, 0f, localPosition.z);
				}
				else if (localPosition.x <= -9f)
				{
					float x4 = Mathf.Lerp(-10f, -14f, blink += deltaTime);
					base.transform.localPosition = new Vector3(x4, 0f, localPosition.z);
				}
				else if (localPosition.z >= 6f)
				{
					float z3 = Mathf.Lerp(6.5f, 11f, blink += deltaTime);
					base.transform.localPosition = new Vector3(localPosition.x, 0f, z3);
				}
				else
				{
					float z4 = Mathf.Lerp(-7f, -11f, blink += deltaTime);
					base.transform.localPosition = new Vector3(localPosition.x, 0f, z4);
				}
				if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
				{
					RequestSend();
				}
				fixedMove = vector;
				fixedExt = zero;
				return;
			}
			if (!dead)
			{
				float num2 = Mathf.Lerp(direction.x, direction.y, blink += deltaTime);
				if (Mathf.Abs(localPosition.x) <= 8f)
				{
					base.transform.localPosition = new Vector3(localPosition.x, 0f, num2);
					if (Mathf.Abs(num2) < Mathf.Abs(direction.y) + 0.1f)
					{
						dead = true;
						base.transform.localPosition = new Vector3(localPosition.x, 0f, direction.y);
						if (type == Character.Player)
						{
							GamePointer.EnablePointer(true, (Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
						}
					}
				}
				else
				{
					base.transform.localPosition = new Vector3(num2, 0f, localPosition.z);
					if (Mathf.Abs(num2) < Mathf.Abs(direction.y) + 0.1f)
					{
						dead = true;
						base.transform.localPosition = new Vector3(direction.y, 0f, localPosition.z);
						if (type == Character.Player)
						{
							GamePointer.EnablePointer(true, (Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
						}
					}
				}
			}
			else
			{
				if (type == Character.AI)
				{
					foreach (GameCharacter allChar in allChars)
					{
						if (!allChar.name.Equals(base.name) && Vector3.Distance(localPosition, allChar.transform.localPosition) < num && !allChar.isGhost)
						{
							num = Vector3.Distance(localPosition, allChar.transform.localPosition);
							vector2 = allChar.transform.position;
						}
					}
				}
				else
				{
					vector2 = GamePointer.GetPos((Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
				}
				Quaternion to = Quaternion.LookRotation((vector2 - base.transform.position).normalized);
				Quaternion quaternion = Quaternion.Slerp(localRotation, to, deltaTime * 14f);
				ghostModel.transform.localRotation = Quaternion.Euler(350f, quaternion.eulerAngles.y, localRotation.z);
				float num3 = 10f - (vector2.z / 10f + 5f);
				float num4 = (vector2 / 10f).z + 5f;
				float num5 = vector2.x / 10f + 8f;
				float num6 = 16f - (vector2.x / 10f + 8f);
				float num7 = 2.8f;
				float num8 = 0.6f;
				float num9 = -10f;
				float num10 = 10f;
				float num11 = -7f;
				float num12 = 6.5f;
				if (num3 < num4 && num3 < num5 && num3 < num6 && num3 <= 2.8f)
				{
					if (localPosition.z <= num11 && Mathf.Abs(localPosition.x) < num10)
					{
						if (localPosition.x >= 0f)
						{
							float x5 = Mathf.Lerp(localPosition.x, num10 + num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(x5, localPosition.y, localPosition.z);
						}
						else
						{
							float x6 = Mathf.Lerp(localPosition.x, num9 - num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(x6, localPosition.y, localPosition.z);
						}
					}
					else if (Mathf.Abs(localPosition.x) >= num10 && localPosition.z < num12)
					{
						float z5 = Mathf.Lerp(localPosition.z, num12 + num8, deltaTime * num7);
						base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z5);
					}
					else
					{
						float x7 = Mathf.Lerp(base.transform.position.x, vector2.x, deltaTime * (num7 + 2f));
						base.transform.position = new Vector3(x7, base.transform.position.y, base.transform.position.z);
					}
				}
				else if (num6 < num3 && num6 < num4 && num6 < num5 && num6 <= 3.9f)
				{
					if (localPosition.x <= num9 && Mathf.Abs(localPosition.z) < num12)
					{
						if (localPosition.z >= 0f)
						{
							float z6 = Mathf.Lerp(localPosition.z, num12 + num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z6);
						}
						else
						{
							float z7 = Mathf.Lerp(localPosition.z, num11 - num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z7);
						}
					}
					else if (Mathf.Abs(localPosition.z) >= num12 && localPosition.x < num10)
					{
						float x8 = Mathf.Lerp(localPosition.x, num10 + num8, deltaTime * num7);
						base.transform.localPosition = new Vector3(x8, localPosition.y, localPosition.z);
					}
					else
					{
						float z8 = Mathf.Lerp(base.transform.position.z, vector2.z, deltaTime * (num7 + 2f));
						base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, z8);
					}
				}
				else if (num4 < num3 && num4 < num5 && num4 < num6 && num4 <= 3.2f)
				{
					if (localPosition.z >= num12 && Mathf.Abs(localPosition.x) < num10)
					{
						if (localPosition.x >= 0f)
						{
							float x9 = Mathf.Lerp(localPosition.x, num10 + num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(x9, localPosition.y, localPosition.z);
						}
						else
						{
							float x10 = Mathf.Lerp(localPosition.x, num9 - num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(x10, localPosition.y, localPosition.z);
						}
					}
					else if (Mathf.Abs(localPosition.x) >= num10 && localPosition.z > num11)
					{
						float z9 = Mathf.Lerp(localPosition.z, num11 - num8, deltaTime * num7);
						base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z9);
					}
					else
					{
						float x11 = Mathf.Lerp(base.transform.position.x, vector2.x, deltaTime * (num7 + 2f));
						base.transform.position = new Vector3(x11, base.transform.position.y, base.transform.position.z);
					}
				}
				else if (num5 < num6 && num5 < num3 && num5 < num4 && num5 <= 3.9f)
				{
					if (localPosition.x >= num10 && Mathf.Abs(localPosition.z) < num12)
					{
						if (localPosition.z >= 0f)
						{
							float z10 = Mathf.Lerp(localPosition.z, num12 + num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z10);
						}
						else
						{
							float z11 = Mathf.Lerp(localPosition.z, num11 - num8, deltaTime * num7);
							base.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z11);
						}
					}
					else if (Mathf.Abs(localPosition.z) >= num12 && localPosition.x > num9)
					{
						float x12 = Mathf.Lerp(localPosition.x, num9 - num8, deltaTime * num7);
						base.transform.localPosition = new Vector3(x12, localPosition.y, localPosition.z);
					}
					else
					{
						float z12 = Mathf.Lerp(base.transform.position.z, vector2.z, deltaTime * (num7 + 2f));
						base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, z12);
					}
				}
				else if (Mathf.Abs(localPosition.z) >= num12)
				{
					float x13 = Mathf.Lerp(base.transform.position.x, vector2.x, deltaTime * (num7 + 2f));
					base.transform.position = new Vector3(x13, base.transform.position.y, base.transform.position.z);
				}
				else
				{
					float z13 = Mathf.Lerp(base.transform.position.z, vector2.z, deltaTime * (num7 + 2f));
					base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, z13);
				}
			}
			ActionUpdate();
			if (shootStep == 1)
			{
				shootStep++;
				shootPos = new Vector3(Mathf.Round(vector2.x / 10f), 0f, Mathf.Round(vector2.z / 10f));
				ghostModel.GetComponent<Animation>().CrossFade("shooting");
				ghostModel.GetComponent<Animation>().CrossFadeQueued("idle");
			}
			else if (shootStep == 2 && ghostModel.GetComponent<Animation>()["shooting"].time > 0.8f)
			{
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					if (Global.Mode != GameMode.OnlineBattle || controllerID == Global.onlinePlayerID)
					{
						GameObject gameObject = Global.Map.AddBomb(this, Bomb.Normal, Fire.Lv1, false);
						Vector3 localPosition2 = base.transform.localPosition;
						if (localPosition2.x > 9.5f)
						{
							localPosition2.x = 9.5f;
						}
						else if (localPosition2.x < -9.5f)
						{
							localPosition2.x = -9.5f;
						}
						if (localPosition2.z > 6.5f)
						{
							localPosition2.z = 6.5f;
						}
						else if (localPosition2.z < -6.5f)
						{
							localPosition2.z = -6.5f;
						}
						Global.Map.FlyBomb(gameObject.GetComponent<GameBomb>(), localPosition2.x, localPosition2.z, shootPos);
					}
				}
				else if (controllerID == Global.onlinePlayerID)
				{
					Global.Map.GetComponent<TNObject>().Send(12, Target.Host, Global.onlineMatchID, battleID, base.transform.localPosition, shootPos);
				}
				shootStep = 0;
			}
			if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
			{
				RequestSend();
			}
			fixedMove = vector;
			fixedExt = zero;
			return;
		}
		if (infectT > 0f)
		{
			infectT -= deltaTime;
			if (infectT < 0f || dead)
			{
				infectT = 0f;
				infect = Virus.None;
				Transform transform = base.transform.Find("FX (Virus)");
				if (transform != null)
				{
					transform.GetComponentInChildren<ParticleSystem>().Stop();
					Object.Destroy(transform.gameObject, 1f);
				}
			}
		}
		if (bombHold != null)
		{
			bombHold.transform.position = (handL.position + handR.position) / 2f;
		}
		if (stunTime > 0f)
		{
			stunTime -= deltaTime;
			if (stunTime < 0f || dead)
			{
				stunTime = 0f;
				Transform transform2 = head.Find("FX (Stun)");
				if (transform2 != null)
				{
					Object.Destroy(transform2.gameObject);
				}
				if (bombHold != null && !dead && !isGhost)
				{
					model.GetComponent<Animation>().CrossFade("holdBomb");
				}
			}
			if (!dead)
			{
				if (type == Character.Monster)
				{
					ActionUpdate();
				}
				if (externalForce != Vector2.zero)
				{
					zero.x = externalForce.x;
					zero.z = externalForce.y;
					lastPosition = base.transform.localPosition;
				}
				if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
				{
					RequestSend();
				}
				fixedMove = vector;
				fixedExt = zero;
				return;
			}
		}
		if (dead)
		{
			if (drops != null && model.GetComponent<Animation>()["die"].time > 0.25f)
			{
				if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
				{
					Global.Map.AddItem(drops);
				}
				drops = null;
			}
			if (blink > 0f)
			{
				blink -= deltaTime * 2f;
				model.transform.localScale = (((int)(blink * 4f) % 2 != 0) ? Vector3.zero : blinkScale);
				if (blink <= 0f)
				{
					blink = -1f;
					model.transform.localScale = Vector3.zero;
					GameShadow.RemoveObject(base.gameObject);
					if (type == Character.Minion)
					{
						Object.Destroy(base.gameObject);
					}
				}
			}
			else if (blink == 0f && !model.GetComponent<Animation>().isPlaying)
			{
				blink = 1f;
				blinkScale = model.transform.localScale;
			}
			ActionUpdate();
			if (blink == -1f && (type == Character.Player || type == Character.AI) && (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle) && Global.ghost && (!Global.mapShrink || Global.Map.RemainTime > 31f))
			{
				dead = false;
				isGhost = true;
				isReborn = false;
				blink = 0f;
				rigidbody.isKinematic = true;
				ghostModel.transform.localScale = Vector3.one;
				Vector3 localPosition3 = base.transform.localPosition;
				float num13 = 10f - (localPosition3.z + 5f);
				float num14 = localPosition3.z + 5f;
				float num15 = localPosition3.x + 8f;
				float num16 = 16f - (localPosition3.x + 8f);
				if (num13 <= num14 && num13 <= num15 && num13 <= num16)
				{
					MonoBehaviour.print("spawn ghostUp");
					direction = new Vector2(11f, 6.5f);
					base.transform.localPosition = new Vector3(localPosition3.x, 0f, 11f);
					ghostModel.transform.localRotation = Quaternion.Euler(350f, 180f, 0f);
				}
				else if (num16 < num13 && num16 < num14 && num16 < num15)
				{
					MonoBehaviour.print("spawn ghostRight");
					direction = new Vector2(14f, 10f);
					base.transform.localPosition = new Vector3(14f, 0f, localPosition3.z);
					ghostModel.transform.localRotation = Quaternion.Euler(350f, 270f, 0f);
				}
				else if (num14 < num13 && num14 < num15 && num14 < num16)
				{
					MonoBehaviour.print("spawn ghostDown");
					direction = new Vector2(-11f, -7f);
					base.transform.localPosition = new Vector3(localPosition3.x, 0f, -11f);
					ghostModel.transform.localRotation = Quaternion.Euler(340f, 0f, 0f);
				}
				else if (num15 < num16 && num15 < num13 && num15 < num14)
				{
					MonoBehaviour.print("spawn ghostLeft");
					direction = new Vector2(-14f, -10f);
					base.transform.localPosition = new Vector3(-14f, 0f, localPosition3.z);
					ghostModel.transform.localRotation = Quaternion.Euler(350f, 90f, 0f);
				}
				else
				{
					MonoBehaviour.print("spawn ghostUp");
					direction = new Vector2(11f, 6.5f);
					base.transform.localPosition = new Vector3(localPosition3.x, 0f, 11f);
					ghostModel.transform.localRotation = Quaternion.Euler(350f, 180f, 0f);
				}
				GameShadow.AddObject(base.gameObject);
				MonoBehaviour.print(battleID + " is dead");
				if (reborn != null)
				{
					reborn.GetComponent<Rigidbody>().isKinematic = false;
					reborn.model.transform.localScale = Vector3.one;
					reborn.GetComponent<Collider>().enabled = true;
					reborn.ghostModel.transform.localScale = Vector3.zero;
					reborn.transform.localPosition = reborn.posReborn;
					reborn.posReborn = Vector3.zero;
					reborn.Blink(3f);
					reborn.dead = false;
					reborn.isGhost = false;
					reborn.isReborn = false;
					for (int num17 = allChars.Count - 1; num17 >= 0; num17--)
					{
						if (reborn != allChars[num17] && !allChars[num17].IsDead && (allChars[num17].Type == Character.Player || allChars[num17].Type == Character.AI) && allChars[num17].GetComponent<Collider>().enabled)
						{
							Physics.IgnoreCollision(reborn.GetComponent<Collider>(), allChars[num17].GetComponent<Collider>());
						}
					}
					List<GameBomb> list = GameBomb.List();
					for (int num18 = list.Count - 1; num18 >= 0; num18--)
					{
						if (!list[num18].IsExploded)
						{
							list[num18].AddIgnore(reborn);
						}
					}
					reborn = null;
				}
			}
			if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
			{
				RequestSend();
			}
			fixedMove = vector;
			fixedExt = zero;
			return;
		}
		if (blink > 0f)
		{
			blink -= deltaTime * 2f;
			model.transform.localScale = (((int)(blink * 4f) % 2 != 1) ? Vector3.zero : blinkScale);
			if (blink <= 0f)
			{
				blink = 0f;
				model.transform.localScale = blinkScale;
			}
		}
		ActionUpdate();
		if (dead)
		{
			if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
			{
				RequestSend();
			}
			fixedMove = vector;
			fixedExt = zero;
			return;
		}
		if (type != Character.Boss && type != Character.Minion)
		{
			Vector3 zero2 = Vector3.zero;
			if (direction == Vector2.zero && externalForce == Vector2.zero)
			{
				if (bombHold == null && !dead && !isGhost && (!model.GetComponent<Animation>().isPlaying || model.GetComponent<Animation>().IsPlaying("walk") || model.GetComponent<Animation>().IsPlaying("stun")))
				{
					model.GetComponent<Animation>().CrossFade("idle");
				}
				ValidateFace();
				vector = Vector3.zero;
			}
			else
			{
				float num19 = (IsInfected(Virus.CrazySpeed) ? 17f : ((!IsInfected(Virus.SuperSlow)) ? ((float)speed / 100f) : 0.85f));
				float num20 = ModSpeed(num19 * deltaTime);
				zero2.x = direction.x;
				zero2.z = direction.y;
				zero2.Normalize();
				vector = zero2 * (num20 / deltaTime);
				zero2 *= num20;
				Vector3 vector3 = lastPosition;
				Vector3 vector4 = lastPosition;
				lastPosition = base.transform.localPosition;
				vector4 = lastPosition;
				Vector3 vector5 = vector4 + zero2;
				if (externalForce != Vector2.zero)
				{
					zero.x = externalForce.x;
					zero.z = externalForce.y;
					vector5.x += externalForce.x * deltaTime;
					vector5.z += externalForce.y * deltaTime;
					externalForce = Vector2.zero;
				}
				vector5.y = 0f;
				if (vector5.x > 8f)
				{
					vector5.x = 8f;
				}
				else if (vector5.x < -8f)
				{
					vector5.x = -8f;
				}
				if (vector5.z > 5f)
				{
					vector5.z = 5f;
				}
				else if (vector5.z < -5f)
				{
					vector5.z = -5f;
				}
				if (direction != Vector2.zero)
				{
					Vector2 vector6 = direction;
					if (vector3.x == lastPosition.x)
					{
						direction.x = 0f;
					}
					if (vector3.z == lastPosition.z)
					{
						direction.y = 0f;
					}
					if (direction.x != 0f || direction.y != 0f)
					{
						ValidateFace();
					}
					else if ((vector6.x != 0f && vector6.y == 0f) || (vector6.x == 0f && vector6.y != 0f))
					{
						direction = vector6;
						ValidateFace();
					}
					direction = Vector2.zero;
					if ((Global.Mode != GameMode.OnlineBattle || TNManager.isHosting) && hasKick && type != Character.Monster)
					{
						Vector3[] array = null;
						switch (Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45)
						{
						case 0:
							array = new Vector3[1]
							{
								new Vector3(0f, 0f, 1f)
							};
							break;
						case 90:
							array = new Vector3[1]
							{
								new Vector3(1f, 0f, 0f)
							};
							break;
						case 180:
							array = new Vector3[1]
							{
								new Vector3(0f, 0f, -1f)
							};
							break;
						case 270:
							array = new Vector3[1]
							{
								new Vector3(-1f, 0f, 0f)
							};
							break;
						case 45:
							array = new Vector3[2]
							{
								new Vector3(0f, 0f, 1f),
								new Vector3(1f, 0f, 0f)
							};
							break;
						case 135:
							array = new Vector3[2]
							{
								new Vector3(0f, 0f, -1f),
								new Vector3(1f, 0f, 0f)
							};
							break;
						case 225:
							array = new Vector3[2]
							{
								new Vector3(0f, 0f, -1f),
								new Vector3(-1f, 0f, 0f)
							};
							break;
						case 315:
							array = new Vector3[2]
							{
								new Vector3(0f, 0f, 1f),
								new Vector3(-1f, 0f, 0f)
							};
							break;
						}
						if (array != null)
						{
							for (int i = 0; i < array.Length; i++)
							{
								GameBomb gameBomb = Global.Map.PickBomb(vector4 + array[i] * 0.75f, 0.251f);
								if (!(gameBomb != null))
								{
									continue;
								}
								Vector3 vector7 = gameBomb.transform.localPosition - vector4;
								if (Mathf.Abs(vector7.z) > Mathf.Abs(vector7.x))
								{
									if (vector7.z > 0f)
									{
										gameBomb.Shoot(Direction.Up);
									}
									else
									{
										gameBomb.Shoot(Direction.Down);
									}
								}
								else if (vector7.x > 0f)
								{
									gameBomb.Shoot(Direction.Right);
								}
								else
								{
									gameBomb.Shoot(Direction.Left);
								}
							}
						}
					}
					if (bombHold == null && !dead && !isGhost && (!model.GetComponent<Animation>().isPlaying || model.GetComponent<Animation>().IsPlaying("idle") || model.GetComponent<Animation>().IsPlaying("stun")))
					{
						model.GetComponent<Animation>().CrossFade("walk");
					}
				}
				else if (bombHold == null && !dead && !isGhost && (!model.GetComponent<Animation>().isPlaying || model.GetComponent<Animation>().IsPlaying("walk") || model.GetComponent<Animation>().IsPlaying("stun")))
				{
					model.GetComponent<Animation>().CrossFade("idle");
				}
			}
			if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID)
			{
				RequestSend();
			}
		}
		fixedMove = vector;
		fixedExt = zero;
	}

	private void FixedUpdate()
	{
		if (fixedMove != Vector3.zero || fixedExt != Vector3.zero)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition += (fixedMove + fixedExt) * Time.fixedDeltaTime;
			base.GetComponent<Rigidbody>().MovePosition(localPosition * 10f);
		}
	}

	private void RequestSend()
	{
		if (sendDelay < 0.2f)
		{
			sendDelay += Time.deltaTime;
			return;
		}
		sendDelay = 0f;
		GetComponent<TNObject>().SendQuickly(30, Target.Others, Global.onlineMatchID, base.transform.localPosition, Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45, model.transform.localScale.x, ghostModel.transform.localRotation.eulerAngles, ghostModel.transform.localScale.x, externalForce);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (Global.Mode == GameMode.OnlineBattle && !TNManager.isHosting)
		{
			if (collision.gameObject.name.StartsWith("Item"))
			{
				GameItem component = collision.gameObject.GetComponent<GameItem>();
				if (component.IsLanded)
				{
					Physics.IgnoreCollision(base.GetComponent<Collider>(), component.GetComponent<Collider>());
				}
			}
			return;
		}
		string text = collision.gameObject.name;
		if (text == "Explosion")
		{
			Dead(Mathf.RoundToInt(collision.transform.localRotation.eulerAngles.y / 10f));
		}
		else if (text == "ExplosionX")
		{
			if (type != Character.Boss && type != Character.Monster)
			{
				Dead();
			}
		}
		else if (text.StartsWith("Boss") || text.StartsWith("Monster"))
		{
			if (type == Character.Player || type == Character.AI)
			{
				Dead();
			}
		}
		else if (text.StartsWith("Item"))
		{
			GameItem component2 = collision.gameObject.GetComponent<GameItem>();
			if (component2.IsLanded)
			{
				GetItem(component2.Type);
				component2.Deactive(false);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		string text = collision.gameObject.name;
		Vector3 localPosition = base.transform.localPosition;
		switch (text)
		{
		case "Floor":
			localPosition.y = 0f;
			break;
		case "Wall East":
		case "Wall West":
			localPosition.x = Mathf.Round(localPosition.x);
			break;
		case "Wall North":
		case "Wall South":
			localPosition.z = Mathf.Round(localPosition.z);
			break;
		case "Pillar":
		case "Block":
		{
			Vector3 localPosition3 = collision.transform.localPosition;
			if (Mathf.Abs(localPosition.x - localPosition3.x) <= 0.5f)
			{
				localPosition.z = Mathf.Round(localPosition.z);
			}
			if (Mathf.Abs(localPosition.z - localPosition3.z) <= 0.5f)
			{
				localPosition.x = Mathf.Round(localPosition.x);
			}
			break;
		}
		default:
		{
			if (!text.StartsWith("Bomb"))
			{
				break;
			}
			GameBomb component = collision.gameObject.GetComponent<GameBomb>();
			if (component.IsLanded)
			{
				Vector3 localPosition2 = collision.transform.localPosition;
				if (Mathf.Abs(localPosition.x - localPosition2.x) <= 0.5f)
				{
					localPosition.z = Mathf.Round(localPosition.z);
				}
				if (Mathf.Abs(localPosition.z - localPosition2.z) <= 0.5f)
				{
					localPosition.x = Mathf.Round(localPosition.x);
				}
			}
			break;
		}
		}
		base.transform.localPosition = localPosition;
	}

	private void OnCollisionExit(Collision collision)
	{
	}

	protected void ImplementGetItem(Item item)
	{
		if (dead || isGhost)
		{
			return;
		}
		if (type == Character.Player || type == Character.AI)
		{
			GameSound.StartSFX("itemPick");
		}
		else
		{
			GameSound.StartSFX("itemDestroy");
		}
		if ((type == Character.Player || type == Character.AI) && item == Item.BombPass && !hasBombPass)
		{
			List<GameBomb> list = GameBomb.List();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (!list[num].IsExploded)
				{
					Transform transform = list[num].transform.Find("root");
					if (list[num].GetComponent<Collider>().enabled)
					{
						Physics.IgnoreCollision(base.GetComponent<Collider>(), list[num].GetComponent<Collider>());
					}
					if (transform.GetComponent<Collider>().enabled)
					{
						Physics.IgnoreCollision(base.GetComponent<Collider>(), transform.GetComponent<Collider>());
					}
				}
			}
		}
		switch (item)
		{
		case Item.FireFull:
			fireLv = Fire.Lv10;
			break;
		case Item.FireUp:
			if (fireLv < Fire.Lv10)
			{
				fireLv++;
			}
			break;
		case Item.FireDown:
			if (fireLv > Fire.Lv1)
			{
				fireLv--;
			}
			break;
		case Item.BombUp:
			if (bombMaxN < 10)
			{
				bombMaxN++;
			}
			break;
		case Item.BombDown:
			if (bombMaxN > 1)
			{
				bombMaxN--;
			}
			break;
		case Item.SpeedUp:
			switch (speed)
			{
			case Speed.Lv1:
				speed = Speed.Lv2;
				break;
			case Speed.Lv2:
				speed = Speed.Lv3;
				break;
			case Speed.Lv3:
				speed = Speed.Lv4;
				break;
			case Speed.Lv4:
				speed = Speed.Lv5;
				break;
			}
			break;
		case Item.SpeedDown:
			switch (speed)
			{
			case Speed.Lv2:
				speed = Speed.Lv1;
				break;
			case Speed.Lv3:
				speed = Speed.Lv2;
				break;
			case Speed.Lv4:
				speed = Speed.Lv3;
				break;
			case Speed.Lv5:
				speed = Speed.Lv4;
				break;
			}
			break;
		case Item.PierceBomb:
			bombType = Bomb.Pierce;
			break;
		case Item.Remote:
			bombType = Bomb.Remote;
			break;
		case Item.BombPass:
			hasBombPass = true;
			break;
		case Item.Glove:
			hasGlove = true;
			break;
		case Item.Kick:
			hasKick = true;
			break;
		case Item.Virus:
			if ((type == Character.Player || type == Character.AI) && (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting))
			{
				switch (Random.Range(0, 4))
				{
				case 0:
					Infect(Virus.CrazySpeed, 15f);
					break;
				case 1:
					Infect(Virus.SuperSlow, 15f);
					break;
				case 2:
					Infect(Virus.Unarmed, 15f);
					break;
				case 3:
					Infect(Virus.Confusing, 15f);
					break;
				}
			}
			break;
		}
	}

	public void GetItem(Item item)
	{
		if (type != Character.Boss && type != Character.Minion)
		{
			if (type == Character.Player && Global.Mode == GameMode.Adventure && AdventureGameMode.UI != null)
			{
				AdventureGameMode.UI.Popup((int)item);
			}
			ImplementGetItem(item);
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(31, Target.Others, Global.onlineMatchID, (int)item);
			}
		}
	}

	public bool HasItem(Item item)
	{
		switch (item)
		{
		case Item.FireFull:
			return fireLv == Fire.Lv10;
		case Item.FireUp:
			return fireLv > Fire.Lv1;
		case Item.FireDown:
			return fireLv < Fire.Lv10;
		case Item.BombUp:
			return bombMaxN > 1;
		case Item.BombDown:
			return bombMaxN < 10;
		case Item.SpeedUp:
			return speed > Speed.Lv1;
		case Item.SpeedDown:
			return speed < Speed.Lv5;
		case Item.PierceBomb:
			return bombType == Bomb.Pierce;
		case Item.Remote:
			return bombType == Bomb.Remote;
		case Item.BombPass:
			return hasBombPass && (type == Character.Player || type == Character.AI);
		case Item.Glove:
			return hasGlove;
		case Item.Kick:
			return hasKick;
		case Item.Virus:
			return infect != Virus.None;
		default:
			return false;
		}
	}

	protected void ImplementInfect(Virus type, float duration)
	{
		if (!dead && !isGhost)
		{
			if (infect == Virus.None && type != Virus.None)
			{
				Debug.Log("infect " + type);
				GameObject gameObject = Object.Instantiate(virusFX) as GameObject;
				gameObject.name = "FX (Virus)";
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
			}
			infect = type;
			infectT = ((!(duration > infectT)) ? infectT : duration);
		}
	}

	public void Infect(Virus type, float duration = 15f)
	{
		ImplementInfect(type, duration);
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			GetComponent<TNObject>().Send(32, Target.Others, Global.onlineMatchID, (int)type, duration);
		}
	}

	public bool IsInfected(Virus type = Virus.Any)
	{
		return (infect & type) != 0;
	}

	public void MoveLeft()
	{
		direction.x = (IsInfected(Virus.Confusing) ? 1 : (-1));
	}

	public void MoveRight()
	{
		direction.x = ((!IsInfected(Virus.Confusing)) ? 1 : (-1));
	}

	public void MoveDown()
	{
		direction.y = (IsInfected(Virus.Confusing) ? 1 : (-1));
	}

	public void MoveUp()
	{
		direction.y = ((!IsInfected(Virus.Confusing)) ? 1 : (-1));
	}

	private void ValidateFace()
	{
		if (this.direction.x == 0f && this.direction.y == 0f)
		{
			int num = Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45;
			Direction direction = Direction.None;
			switch (num)
			{
			case 45:
				direction = Direction.Up;
				break;
			case 135:
				direction = Direction.Down;
				break;
			case 225:
				direction = Direction.Down;
				break;
			case 315:
				direction = Direction.Up;
				break;
			}
			if (direction != Direction.None)
			{
				model.transform.localRotation = Quaternion.Euler(0f, (float)direction, 0f);
			}
		}
		else if (this.direction.x != 0f && this.direction.y != 0f)
		{
			Direction direction2 = ((!(this.direction.x < 0f)) ? Direction.Right : Direction.Left);
			Direction direction3 = ((this.direction.y < 0f) ? Direction.Down : Direction.Up);
			if (Mathf.Abs(direction2 - direction3) < 180)
			{
				model.transform.localRotation = Quaternion.Euler(0f, ((int)direction2 + (int)direction3) / 2, 0f);
			}
			else
			{
				model.transform.localRotation = Quaternion.Euler(0f, 315f, 0f);
			}
		}
		else
		{
			Direction direction4 = Direction.Down;
			if (this.direction.x < 0f)
			{
				direction4 = Direction.Left;
			}
			else if (this.direction.x > 0f)
			{
				direction4 = Direction.Right;
			}
			else if (this.direction.y < 0f)
			{
				direction4 = Direction.Down;
			}
			else if (this.direction.y > 0f)
			{
				direction4 = Direction.Up;
			}
			model.transform.localRotation = Quaternion.Euler(0f, (float)direction4, 0f);
		}
	}

	public void ReleaseBomb()
	{
		bombHold = null;
	}

	protected void ImplementDropBomb()
	{
		if (!dead && !isGhost)
		{
			GameSound.StartSFX("bombDrop");
		}
	}

	public bool DropBomb()
	{
		if (IsInfected(Virus.Unarmed))
		{
			return false;
		}
		if (bombHold != null)
		{
			ThrowBomb();
			return false;
		}
		int num = Global.Map.CountBomb(this);
		if (num < bombMaxN && Global.Map.AddBomb(this, bombType, fireLv) != null)
		{
			ImplementDropBomb();
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(33, Target.Others, Global.onlineMatchID);
			}
			return true;
		}
		return false;
	}

	protected void ImplementPickBomb(GameBomb bomb)
	{
		if (!dead && !isGhost)
		{
			bomb.GetComponent<Collider>().enabled = false;
			bomb.transform.Find("root").GetComponent<Collider>().enabled = false;
			bomb.transform.parent = model.transform;
			bomb.transform.localPosition = Vector3.zero;
			bomb.transform.localScale = Vector3.one;
			bombHold = bomb;
			model.GetComponent<Animation>().CrossFade("pickBomb");
			model.GetComponent<Animation>().CrossFadeQueued("holdBomb");
			GameSound.StartSFX("bombPick");
		}
	}

	public bool PickBomb()
	{
		if (IsInfected(Virus.Unarmed))
		{
			return false;
		}
		if (bombHold != null)
		{
			ThrowBomb();
			return false;
		}
		if (!hasGlove)
		{
			return false;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x = Mathf.Round(localPosition.x);
		localPosition.y = 0f;
		localPosition.z = Mathf.Round(localPosition.z);
		GameBomb gameBomb = Global.Map.PickBomb(localPosition);
		if (gameBomb == null)
		{
			Vector3[] array = null;
			switch (Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45)
			{
			case 0:
				array = new Vector3[1]
				{
					new Vector3(0f, 0f, 1f)
				};
				break;
			case 90:
				array = new Vector3[1]
				{
					new Vector3(1f, 0f, 0f)
				};
				break;
			case 180:
				array = new Vector3[1]
				{
					new Vector3(0f, 0f, -1f)
				};
				break;
			case 270:
				array = new Vector3[1]
				{
					new Vector3(-1f, 0f, 0f)
				};
				break;
			case 45:
				array = new Vector3[2]
				{
					new Vector3(0f, 0f, 1f),
					new Vector3(1f, 0f, 0f)
				};
				break;
			case 135:
				array = new Vector3[2]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(1f, 0f, 0f)
				};
				break;
			case 225:
				array = new Vector3[2]
				{
					new Vector3(0f, 0f, -1f),
					new Vector3(-1f, 0f, 0f)
				};
				break;
			case 315:
				array = new Vector3[2]
				{
					new Vector3(0f, 0f, 1f),
					new Vector3(-1f, 0f, 0f)
				};
				break;
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					gameBomb = Global.Map.PickBomb(localPosition + array[i]);
					if (gameBomb != null)
					{
						break;
					}
				}
			}
		}
		if (gameBomb != null)
		{
			ImplementPickBomb(gameBomb);
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(34, Target.Others, Global.onlineMatchID, (int)gameBomb.GetComponent<TNObject>().uid);
			}
			return true;
		}
		return false;
	}

	protected void ImplementThrowBomb()
	{
		if (!dead && !isGhost)
		{
			bombHold = null;
			model.GetComponent<Animation>().CrossFade("throwBomb");
			GameSound.StartSFX("bombThrow");
		}
	}

	public bool ThrowBomb()
	{
		if (IsInfected(Virus.Unarmed))
		{
			return false;
		}
		if (bombHold == null)
		{
			return false;
		}
		Vector3 localPosition = base.transform.localPosition;
		int x = Mathf.RoundToInt(localPosition.x);
		int y = Mathf.RoundToInt(localPosition.z);
		int i = Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45;
		Direction direction = Direction.None;
		for (; i < 0; i += 360)
		{
		}
		while (i >= 360)
		{
			i -= 360;
		}
		switch (i)
		{
		case 0:
		case 90:
		case 180:
		case 270:
			direction = (Direction)i;
			break;
		case 45:
			direction = Direction.Up;
			break;
		case 135:
			direction = Direction.Down;
			break;
		case 225:
			direction = Direction.Down;
			break;
		case 315:
			direction = Direction.Up;
			break;
		}
		if (direction != Direction.None)
		{
			Global.Map.FlyBomb(bombHold, x, y, direction, 4);
		}
		ImplementThrowBomb();
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			GetComponent<TNObject>().Send(35, Target.Others, Global.onlineMatchID);
		}
		return true;
	}

	protected void ImplementRemoteBomb()
	{
	}

	public bool RemoteBomb()
	{
		if (IsInfected(Virus.Unarmed))
		{
			return false;
		}
		bool result = false;
		List<GameBomb> list = GameBomb.List();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Owner == this && list[i].Type == Bomb.Remote)
			{
				list[i].Explode();
				result = true;
			}
		}
		return result;
	}

	protected void ImplementStun()
	{
		if (!dead && !isGhost)
		{
			stunTime = 2f;
			GameObject gameObject = Object.Instantiate(stunFX) as GameObject;
			gameObject.name = "FX (Stun)";
			gameObject.transform.parent = head;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f - head.rotation.eulerAngles.y, 0f);
			gameObject.transform.localScale = Vector3.one;
			model.GetComponent<Animation>().CrossFade("stun");
		}
	}

	public void Stun()
	{
		if (stunTime == 0f && type != Character.Boss && type != Character.Minion && !(head == null))
		{
			ImplementStun();
			if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
			{
				GetComponent<TNObject>().Send(37, Target.Others, Global.onlineMatchID);
			}
		}
	}

	protected void ImplementDead(int killerID)
	{
		if (killerID != -1)
		{
			foreach (GameCharacter allChar in allChars)
			{
				if (allChar.battleID == killerID)
				{
					allChar.dead = false;
					allChar.isGhost = true;
					allChar.isReborn = true;
					allChar.posReborn = base.transform.localPosition;
					allChar.blink = 0f;
					if (allChar.type == Character.Player)
					{
						GamePointer.EnablePointer(false, (Global.Mode != GameMode.OnlineBattle) ? allChar.controllerID : allChar.battleID);
					}
					reborn = allChar;
					break;
				}
			}
		}
		dead = true;
		bombMaxN = 1;
		bombType = Bomb.Normal;
		bombHold = null;
		fireLv = Fire.Lv1;
		speed = Speed.Lv1;
		infect = Virus.None;
		List<GameBomb> list = GameBomb.List();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].IsExploded)
			{
				Transform transform = list[num].transform.Find("root");
				if (list[num].GetComponent<Collider>().enabled)
				{
					Physics.IgnoreCollision(base.GetComponent<Collider>(), list[num].GetComponent<Collider>(), false);
				}
				if (transform.GetComponent<Collider>().enabled)
				{
					Physics.IgnoreCollision(base.GetComponent<Collider>(), transform.GetComponent<Collider>(), false);
				}
			}
		}
		base.GetComponent<Collider>().enabled = false;
		if (type == Character.Boss || type == Character.Monster)
		{
			base.transform.Find("root").GetComponent<Collider>().enabled = false;
		}
		model.GetComponent<Animation>().CrossFade("die");
		if (type != Character.Minion)
		{
			GameSound.StartSFX("dead" + type);
		}
	}

	public virtual void Dead(int killerID = -1)
	{
		if (dead || blink != 0f || (type == Character.Player && !base.enabled))
		{
			return;
		}
		drops = new List<Item>();
		for (int i = 1; i < bombMaxN; i++)
		{
			drops.Add(Item.BombUp);
		}
		if (bombType != Bomb.Normal)
		{
			drops.Add((bombType != Bomb.Pierce) ? Item.Remote : Item.PierceBomb);
		}
		for (Fire fire = Fire.Lv1; fire < fireLv; fire++)
		{
			drops.Add(Item.FireUp);
		}
		int num = 0;
		switch (speed)
		{
		case Speed.Lv2:
			num = 1;
			break;
		case Speed.Lv3:
			num = 2;
			break;
		case Speed.Lv4:
			num = 3;
			break;
		case Speed.Lv5:
			num = 4;
			break;
		}
		for (int j = 0; j < num; j++)
		{
			drops.Add(Item.SpeedUp);
		}
		if (hasBombPass)
		{
			drops.Add(Item.BombPass);
		}
		if (hasGlove)
		{
			drops.Add(Item.Glove);
		}
		if (hasKick)
		{
			drops.Add(Item.Kick);
		}
		if (infect != Virus.None)
		{
			drops.Add(Item.Virus);
		}
		if (bombHold != null)
		{
			Vector3 localPosition = base.transform.localPosition;
			int x = Mathf.RoundToInt(localPosition.x);
			int y = Mathf.RoundToInt(localPosition.z);
			int num2 = Mathf.RoundToInt(model.transform.localRotation.eulerAngles.y / 45f) * 45;
			Direction direction = Direction.None;
			switch (num2)
			{
			case 45:
				direction = Direction.Up;
				break;
			case 135:
				direction = Direction.Down;
				break;
			case 225:
				direction = Direction.Down;
				break;
			case 315:
				direction = Direction.Up;
				break;
			default:
				direction = (Direction)num2;
				break;
			}
			Global.Map.FlyBomb(bombHold, x, y, direction, 1);
		}
		if (killerID != -1)
		{
			foreach (GameCharacter allChar in allChars)
			{
				if (allChar.battleID == killerID)
				{
					if (!allChar.dead && !allChar.isGhost)
					{
						killerID = -1;
					}
					break;
				}
			}
		}
		ImplementDead(killerID);
		if (Global.Mode == GameMode.OnlineBattle && TNManager.isHosting)
		{
			GetComponent<TNObject>().Send(38, Target.Others, Global.onlineMatchID, killerID);
		}
	}

	public void Blink(float t)
	{
		blink = t;
		blinkScale = model.transform.localScale;
	}

	public static List<GameCharacter> List()
	{
		return allChars;
	}

	public static void Init()
	{
		if (res == null)
		{
			res = Resources.Load("Characters/Bastard/Bastard") as GameObject;
			mat = new Material[8];
			Shader shader = Shader.Find("Unlit/Texture");
			for (int i = 0; i < mat.Length; i++)
			{
				mat[i] = new Material(shader);
				mat[i].name = "bastard-" + i;
				mat[i].mainTexture = Resources.Load("Characters/Bastard/Textures/bastard-" + i) as Texture;
			}
			stunFX = Resources.Load("Environments/Stun/Stun") as GameObject;
			virusFX = Resources.Load("Environments/Virus/Virus") as GameObject;
			resGhost = Resources.Load("Characters/BastardJet/BastardJet") as GameObject;
			matGhost = new Material[8];
			for (int j = 0; j < matGhost.Length; j++)
			{
				matGhost[j] = new Material(shader);
				matGhost[j].name = "BastardJet-" + j;
				matGhost[j].mainTexture = Resources.Load("Characters/BastardJet/Textures/BastardJet-" + j) as Texture;
			}
		}
		if (resMons == null || resMonLv != Global.Level || resMonBoss != Global.IsBossStage)
		{
			resMonLv = Global.Level;
			resMonBoss = Global.IsBossStage;
			if (resMonBoss)
			{
				switch (resMonLv)
				{
				case 1:
					resMons = new GameObject[2];
					resMons[0] = Resources.Load("Characters/Boss01/Boss01") as GameObject;
					resMons[1] = Resources.Load("Skills/01/Mushroom") as GameObject;
					break;
				case 2:
					resMons = new GameObject[2];
					resMons[0] = Resources.Load("Characters/Boss02/Boss02") as GameObject;
					resMons[1] = Resources.Load("Skills/02/IceBlock") as GameObject;
					break;
				case 3:
					resMons = new GameObject[2];
					resMons[0] = Resources.Load("Characters/Boss03/Boss03") as GameObject;
					resMons[1] = Resources.Load("Skills/03/Storm") as GameObject;
					break;
				case 4:
					resMons = new GameObject[2];
					resMons[0] = Resources.Load("Characters/Boss04/Boss04") as GameObject;
					resMons[1] = Resources.Load("Skills/04/FireBall") as GameObject;
					break;
				case 5:
					resMons = new GameObject[2];
					resMons[0] = Resources.Load("Characters/Boss05/Boss05") as GameObject;
					resMons[1] = Resources.Load("Skills/05/PlasmaBall") as GameObject;
					break;
				}
			}
			else
			{
				switch (resMonLv)
				{
				case 1:
					resMons = new GameObject[3];
					resMons[0] = Resources.Load("Characters/Monster01/Monster01") as GameObject;
					resMons[1] = Resources.Load("Characters/Monster02/Monster02") as GameObject;
					resMons[2] = Resources.Load("Characters/Monster03/Monster03") as GameObject;
					break;
				case 2:
					resMons = new GameObject[4];
					resMons[0] = Resources.Load("Characters/Monster04/Monster04") as GameObject;
					resMons[1] = Resources.Load("Characters/Monster05/Monster05") as GameObject;
					resMons[2] = Resources.Load("Characters/Monster06/Monster06") as GameObject;
					resMons[3] = Resources.Load("Characters/Monster07/Monster07") as GameObject;
					break;
				case 3:
					resMons = new GameObject[4];
					resMons[0] = Resources.Load("Characters/Monster08/Monster08") as GameObject;
					resMons[1] = Resources.Load("Characters/Monster09/Monster09") as GameObject;
					resMons[2] = Resources.Load("Characters/Monster10/Monster10") as GameObject;
					resMons[3] = Resources.Load("Characters/Monster11/Monster11") as GameObject;
					break;
				case 4:
					resMons = new GameObject[5];
					resMons[0] = Resources.Load("Characters/Monster12/Monster12") as GameObject;
					resMons[1] = Resources.Load("Characters/Monster13/Monster13") as GameObject;
					resMons[2] = Resources.Load("Characters/Monster14/Monster14") as GameObject;
					resMons[3] = Resources.Load("Characters/Monster15/Monster15") as GameObject;
					resMons[4] = Resources.Load("Characters/Monster16/Monster16") as GameObject;
					break;
				case 5:
					resMons = new GameObject[7];
					resMons[0] = Resources.Load("Characters/Monster17/Monster17") as GameObject;
					resMons[1] = Resources.Load("Characters/Monster18/Monster18") as GameObject;
					resMons[2] = Resources.Load("Characters/Monster19/Monster19") as GameObject;
					resMons[3] = Resources.Load("Characters/Monster20/Monster20") as GameObject;
					resMons[4] = Resources.Load("Characters/Monster21/Monster21") as GameObject;
					resMons[5] = Resources.Load("Characters/Monster22/Monster22") as GameObject;
					resMons[6] = Resources.Load("Characters/Monster23/Monster23") as GameObject;
					break;
				}
			}
		}
		if (allChars == null)
		{
			allChars = new List<GameCharacter>();
		}
		else
		{
			allChars.Clear();
		}
	}

	public static void Destroy()
	{
		res = null;
		mat = null;
		resGhost = null;
		matGhost = null;
		resMons = null;
		stunFX = null;
		virusFX = null;
		allChars.Clear();
	}

	public static GameObject Create(Character type, int id = 0, int lv = 0)
	{
		GameObject gameObject = new GameObject(string.Concat(type, ":", id, " (", lv, ")"));
		SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
		sphereCollider.center = new Vector3(0f, 0.05f, 0f);
		sphereCollider.radius = ((type != Character.Boss) ? 0.05f : 0f);
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbody.freezeRotation = true;
		GameCharacter gameCharacter;
		switch (type)
		{
		case Character.Player:
			gameCharacter = gameObject.AddComponent<GamePlayer>();
			gameCharacter.Init(lv);
			break;
		case Character.Boss:
			gameCharacter = gameObject.AddComponent<GameBoss>();
			gameCharacter.Init(lv);
			break;
		case Character.Minion:
			gameCharacter = gameObject.AddComponent<GameMinion>();
			gameCharacter.Init(lv);
			break;
		case Character.AI:
			gameCharacter = gameObject.AddComponent<GameAI>();
			gameCharacter.Init(lv);
			break;
		default:
			gameCharacter = gameObject.AddComponent<GameMonster>();
			gameCharacter.Init(lv * 10 + id);
			break;
		}
		gameCharacter.battleID = id;
		gameCharacter.type = type;
		if (type == Character.Player)
		{
			gameCharacter.controllerID = lv;
		}
		if (type == Character.Player || type == Character.AI)
		{
			gameCharacter.model = Object.Instantiate(res) as GameObject;
			Renderer[] componentsInChildren = gameCharacter.model.GetComponentsInChildren<Renderer>();
			for (int num = componentsInChildren.Length - 1; num >= 0; num--)
			{
				componentsInChildren[num].material = mat[id];
			}
			if (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle)
			{
				gameCharacter.ghostModel = Object.Instantiate(resGhost) as GameObject;
				componentsInChildren = gameCharacter.ghostModel.GetComponentsInChildren<Renderer>();
				for (int num2 = componentsInChildren.Length - 1; num2 >= 0; num2--)
				{
					componentsInChildren[num2].material = matGhost[id];
				}
			}
		}
		else
		{
			gameCharacter.model = Object.Instantiate(resMons[id]) as GameObject;
		}
		GameObject gameObject2 = new GameObject("root");
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = new Vector3(0f, 0.005f, -0.0125f);
		gameObject2.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
		gameObject2.transform.localScale = Vector3.one;
		gameCharacter.model.transform.parent = gameObject2.transform;
		gameCharacter.model.transform.localPosition = Vector3.zero;
		gameCharacter.model.transform.localRotation = Quaternion.identity;
		gameCharacter.model.transform.localScale = Vector3.one;
		if (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle)
		{
			gameCharacter.ghostModel.transform.parent = gameObject2.transform;
			gameCharacter.ghostModel.transform.localPosition = new Vector3(0f, 0.1f, 0f);
			gameCharacter.ghostModel.transform.localRotation = Quaternion.identity;
			gameCharacter.ghostModel.transform.localScale = Vector3.zero;
		}
		switch (type)
		{
		case Character.Boss:
		{
			BoxCollider boxCollider = gameObject2.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3(0f, 0.04f, 0f);
			boxCollider.size = new Vector3(0.08f, 0.08f, 0.08f);
			break;
		}
		case Character.Monster:
			sphereCollider = gameObject2.AddComponent<SphereCollider>();
			sphereCollider.center = new Vector3(0f, 0.05f, 0f);
			sphereCollider.radius = 0.025f;
			break;
		}
		gameCharacter.direction.y = -1f;
		gameCharacter.ValidateFace();
		gameCharacter.direction.y = 0f;
		List<Transform> list = new List<Transform>();
		list.Add(gameCharacter.model.transform);
		while (list.Count > 0)
		{
			Transform transform = list[0];
			list.RemoveAt(0);
			if (transform.name == "topHead")
			{
				gameCharacter.head = transform;
				break;
			}
			for (int num3 = transform.childCount - 1; num3 >= 0; num3--)
			{
				list.Add(transform.GetChild(num3));
			}
		}
		if (type == Character.Player || type == Character.AI)
		{
			gameCharacter.handL = gameCharacter.model.transform.Find("robot/chest/left_shoulder/left_elbow/left_handle/left_main_handle");
			gameCharacter.handR = gameCharacter.model.transform.Find("robot/chest/right_shoulder/right_elbow/right_handle/right_main_handle");
		}
		gameCharacter.model.GetComponent<Animation>().Play("idle");
		if (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle)
		{
			gameCharacter.ghostModel.GetComponent<Animation>().Play("idle");
		}
		GameShadow.AddObject(gameObject);
		if (gameCharacter.type == Character.Player && (Global.Mode == GameMode.LocalBattle || Global.Mode == GameMode.OnlineBattle))
		{
			GamePointer.AddObject((Global.Mode != GameMode.OnlineBattle) ? lv : id, id);
		}
		return gameObject;
	}
}
