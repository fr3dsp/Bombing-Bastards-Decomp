using TNet;
using UnityEngine;

public class GameMonster : GameCharacter
{
	private enum Ability
	{
		None = 0,
		Sticky = 1,
		Shoot = 2,
		HitBlock = 3,
		Transformer = 4,
		Respawner = 5,
		Creator = 6
	}

	private int level = 1;

	private float speedMod = 1f;

	private float lifeTime;

	private Ability ability;

	private int abilityRank;

	private int abilityUsed;

	private float isCasting;

	private GameObject bulletRes;

	private GameObject bulletFX;

	private Vector3 bulletHead = Vector3.zero;

	private Vector3 bulletTail = Vector3.zero;

	private Direction bulletDir = Direction.None;

	private Material bulletMat;

	private GameObject smoke;

	private Vector3 gotoPoint = Vector3.down;

	private Direction gotoDir = Direction.None;

	private bool playOnKill = true;

	private bool IsSmartNotConfuse
	{
		get
		{
			return level > 4 && IsInfected(Virus.Confusing);
		}
	}

	protected override void Init(int param)
	{
		int num = param / 10;
		int num2 = param % 10;
		switch (num)
		{
		case 1:
			switch (num2)
			{
			case 0:
				level = 1;
				ability = Ability.None;
				break;
			case 1:
				level = 2;
				ability = Ability.None;
				break;
			case 2:
				level = 1;
				ability = Ability.Sticky;
				abilityRank = 1;
				break;
			}
			break;
		case 2:
			switch (num2)
			{
			case 0:
				level = 2;
				ability = Ability.None;
				break;
			case 1:
				level = 3;
				ability = Ability.None;
				break;
			case 2:
				level = 2;
				ability = Ability.Sticky;
				abilityRank = 2;
				break;
			case 3:
				level = 2;
				ability = Ability.Shoot;
				abilityRank = 1;
				lifeTime = 10f * Random.value;
				break;
			}
			break;
		case 3:
			switch (num2)
			{
			case 0:
				level = 4;
				ability = Ability.None;
				break;
			case 1:
				level = 3;
				ability = Ability.Sticky;
				abilityRank = 3;
				break;
			case 2:
				level = 3;
				ability = Ability.Shoot;
				abilityRank = 2;
				lifeTime = 10f * Random.value;
				break;
			case 3:
				level = 2;
				ability = Ability.HitBlock;
				abilityRank = 1;
				lifeTime = 30f * Random.value;
				break;
			}
			break;
		case 4:
			switch (num2)
			{
			case 0:
				level = 4;
				ability = Ability.Sticky;
				abilityRank = 4;
				break;
			case 1:
				level = 4;
				ability = Ability.Shoot;
				abilityRank = 3;
				lifeTime = 10f * Random.value;
				break;
			case 2:
				level = 3;
				ability = Ability.HitBlock;
				abilityRank = 2;
				lifeTime = 30f * Random.value;
				break;
			case 3:
				level = 4;
				ability = Ability.Transformer;
				abilityRank = 4;
				speedMod = 0.5f;
				break;
			case 4:
				level = 1;
				ability = Ability.None;
				speedMod = 2f;
				break;
			}
			break;
		case 5:
			switch (num2)
			{
			case 0:
				level = 5;
				ability = Ability.Shoot;
				abilityRank = 4;
				lifeTime = 10f * Random.value;
				break;
			case 1:
				level = 4;
				ability = Ability.HitBlock;
				abilityRank = 3;
				lifeTime = 30f * Random.value;
				break;
			case 2:
				level = 4;
				ability = Ability.Transformer;
				abilityRank = 4;
				break;
			case 3:
				level = 4;
				ability = Ability.Creator;
				abilityRank = 5;
				lifeTime = 30f * Random.value;
				break;
			case 4:
				level = 0;
				ability = Ability.Respawner;
				abilityRank = 2;
				lifeTime = 10f;
				break;
			case 5:
				level = 0;
				ability = Ability.Respawner;
				abilityRank = 6;
				lifeTime = 15f;
				break;
			case 6:
				level = 1;
				ability = Ability.None;
				break;
			}
			break;
		}
		if (ability == Ability.Shoot)
		{
			switch (num)
			{
			case 2:
				bulletRes = Resources.Load("Bullets/02") as GameObject;
				break;
			case 3:
				bulletRes = Resources.Load("Bullets/03") as GameObject;
				break;
			case 4:
				bulletRes = Resources.Load("Bullets/04") as GameObject;
				break;
			case 5:
				bulletRes = Resources.Load("Bullets/05") as GameObject;
				break;
			}
		}
		else if (ability == Ability.HitBlock)
		{
			smoke = Resources.Load("Environments/Smoke/Smoke") as GameObject;
		}
	}

	protected override float ModSpeed(float rawSpeed)
	{
		return 4.25f * Time.deltaTime * (float)(level + 3) / 8f * speedMod * 0.5f;
	}

	protected override void ActionUpdate()
	{
		if (ability == Ability.Respawner && dead && lifeTime > 0f)
		{
			ability = Ability.None;
		}
		if ((ability == Ability.Transformer || ability == Ability.Respawner) && blink > 0f)
		{
			base.transform.localScale = Vector3.zero;
		}
		if (bulletDir != Direction.None)
		{
			float num = Mathf.Min(bulletHead.x, bulletTail.x);
			float num2 = Mathf.Min(bulletHead.z, bulletTail.z);
			float num3 = Mathf.Max(bulletHead.x, bulletTail.x);
			float num4 = Mathf.Max(bulletHead.z, bulletTail.z);
			if (bulletDir == Direction.Up || bulletDir == Direction.Down)
			{
				num -= 0.375f;
				num3 += 0.375f;
			}
			else
			{
				num2 -= 0.375f;
				num4 += 0.375f;
			}
			for (int num5 = GameCharacter.allChars.Count - 1; num5 >= 0; num5--)
			{
				if (GameCharacter.allChars[num5].Type == Character.Player && !GameCharacter.allChars[num5].IsDead)
				{
					Vector3 localPosition = GameCharacter.allChars[num5].transform.localPosition;
					if (localPosition.x > num && localPosition.x < num3 && localPosition.z > num2 && localPosition.z < num4)
					{
						GameCharacter.allChars[num5].Dead();
					}
				}
			}
			List<GameBomb> list = GameBomb.List();
			for (int num6 = list.Count - 1; num6 >= 0; num6--)
			{
				if (!list[num6].IsExploded && !list[num6].IsHeld && !list[num6].IsFlying)
				{
					Vector3 localPosition2 = list[num6].transform.localPosition;
					if (localPosition2.x > num && localPosition2.x < num3 && localPosition2.z > num2 && localPosition2.z < num4)
					{
						list[num6].Explode();
					}
				}
			}
			List<GameItem> list2 = GameItem.List();
			for (int num7 = list2.Count - 1; num7 >= 0; num7--)
			{
				if (!list2[num7].IsDeactived && list2[num7].FlyDirection == Direction.None)
				{
					Vector3 localPosition3 = list2[num7].transform.localPosition;
					if (localPosition3.x > num && localPosition3.x < num3 && localPosition3.z > num2 && localPosition3.z < num4)
					{
						list2[num7].Deactive();
					}
				}
			}
			if (abilityRank < 4)
			{
				float num8 = (float)(abilityRank + 1) * 2.5f * Time.deltaTime;
				float num9 = bulletFX.transform.localScale.x;
				float num10 = bulletFX.transform.localScale.z;
				if (num9 == 10f)
				{
					List<GameObject> objects = Global.Map.GetObjects(bulletHead.x, bulletHead.z, 0.01f);
					if (objects != null && objects.Count > 0)
					{
						for (int num11 = objects.Count - 1; num11 >= 0; num11--)
						{
							if (objects[num11].GetComponent<GameMonster>() != null)
							{
								objects.RemoveAt(num11);
							}
						}
					}
					if (objects == null || objects.Count > 0)
					{
						ParticleSystem componentInChildren = bulletFX.GetComponentInChildren<ParticleSystem>();
						if (componentInChildren != null)
						{
							componentInChildren.Stop();
							componentInChildren.transform.parent = bulletFX.transform.parent;
							Object.Destroy(componentInChildren.gameObject, 2f);
						}
						if (objects != null && objects.Count > 0)
						{
							for (int num12 = objects.Count - 1; num12 >= 0; num12--)
							{
								GamePlayer component = objects[num12].GetComponent<GamePlayer>();
								if (component != null)
								{
									component.Dead();
									break;
								}
							}
						}
						num9 = 9.999f;
					}
				}
				if (num10 < 10f)
				{
					num10 += num8 * 10f;
					if (num10 > 10f)
					{
						num10 = 10f;
					}
					bulletFX.transform.localScale = new Vector3(num9, num9, num10);
				}
				if (num9 > 0f)
				{
					if (num9 < 10f)
					{
						num9 -= num8 * 20f;
						if (num9 < 0f)
						{
							num9 = 0f;
						}
						bulletFX.transform.localScale = new Vector3(num9, num9, num10);
					}
					switch (bulletDir)
					{
					case Direction.Left:
						bulletHead.x -= num8;
						if (bulletTail.x > bulletHead.x + (float)abilityRank)
						{
							bulletTail.x = bulletHead.x + (float)abilityRank;
						}
						break;
					case Direction.Down:
						bulletHead.z -= num8;
						if (bulletTail.z > bulletHead.z + (float)abilityRank)
						{
							bulletTail.z = bulletHead.z + (float)abilityRank;
						}
						break;
					case Direction.Right:
						bulletHead.x += num8;
						if (bulletTail.x < bulletHead.x - (float)abilityRank)
						{
							bulletTail.x = bulletHead.x - (float)abilityRank;
						}
						break;
					case Direction.Up:
						bulletHead.z += num8;
						if (bulletTail.z < bulletHead.z - (float)abilityRank)
						{
							bulletTail.z = bulletHead.z - (float)abilityRank;
						}
						break;
					}
				}
				else
				{
					switch (bulletDir)
					{
					case Direction.Left:
						bulletTail.x -= num8;
						if (bulletTail.x <= bulletHead.x)
						{
							bulletDir = Direction.None;
						}
						break;
					case Direction.Down:
						bulletTail.z -= num8;
						if (bulletTail.z <= bulletHead.z)
						{
							bulletDir = Direction.None;
						}
						break;
					case Direction.Right:
						bulletTail.x += num8;
						if (bulletTail.x >= bulletHead.x)
						{
							bulletDir = Direction.None;
						}
						break;
					case Direction.Up:
						bulletTail.z += num8;
						if (bulletTail.z >= bulletHead.z)
						{
							bulletDir = Direction.None;
						}
						break;
					}
				}
				bulletFX.transform.localPosition = bulletHead;
				if (bulletDir == Direction.None)
				{
					Object.Destroy(bulletFX);
				}
			}
			else
			{
				bulletMat.SetTextureOffset("_MainTex", new Vector2(0f, (float)((int)(Time.time * 16f) % 8) / 8f));
				if (lifeTime > 1.5f || dead)
				{
					Object.Destroy(bulletFX, 0.125f);
					bulletDir = Direction.None;
				}
				else
				{
					switch (bulletDir)
					{
					case Direction.Left:
					{
						int x = Mathf.FloorToInt(bulletTail.x);
						int i = (int)bulletTail.z;
						while (x >= -8 && !Global.Map.IsBlock(x, i))
						{
							x--;
						}
						bulletHead.x = (float)x + 0.5f;
						break;
					}
					case Direction.Down:
					{
						int x = (int)bulletTail.x;
						int i = Mathf.FloorToInt(bulletTail.z);
						while (i >= -5 && !Global.Map.IsBlock(x, i))
						{
							i--;
						}
						bulletHead.z = (float)i + 0.5f;
						break;
					}
					case Direction.Right:
					{
						int x = Mathf.CeilToInt(bulletTail.x);
						for (int i = (int)bulletTail.z; x <= 8 && !Global.Map.IsBlock(x, i); x++)
						{
						}
						bulletHead.x = (float)x - 0.5f;
						break;
					}
					case Direction.Up:
					{
						int x = (int)bulletTail.x;
						int i;
						for (i = Mathf.CeilToInt(bulletTail.z); i <= 5 && !Global.Map.IsBlock(x, i); i++)
						{
						}
						bulletHead.z = (float)i - 0.5f;
						break;
					}
					}
					bulletFX.transform.localPosition = bulletHead;
					bulletFX.transform.GetChild(0).GetComponent<Animation>()["idle"].time = Mathf.Round(Mathf.Max(Mathf.Abs(bulletHead.x - bulletTail.x), Mathf.Abs(bulletHead.z - bulletTail.z)));
				}
			}
		}
		if (ability == Ability.Respawner)
		{
			if (dead)
			{
				if (lifeTime == 0f)
				{
					if (model.GetComponent<Animation>().isPlaying)
					{
						return;
					}
					GameObject gameObject = GameCharacter.Create(Character.Monster, abilityRank, Global.Level);
					gameObject.transform.parent = base.transform.parent;
					gameObject.transform.localPosition = base.transform.localPosition;
					gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
					for (int num13 = GameCharacter.allChars.Count - 1; num13 >= 0; num13--)
					{
						if (gameObject != GameCharacter.allChars[num13].gameObject && GameCharacter.allChars[num13].GetComponent<Collider>().enabled)
						{
							Physics.IgnoreCollision(GameCharacter.allChars[num13].GetComponent<Collider>(), gameObject.GetComponent<Collider>());
							if (GameCharacter.allChars[num13].Type == Character.Monster)
							{
								Transform transform = GameCharacter.allChars[num13].transform.Find("root");
								Transform transform2 = gameObject.transform.Find("root");
								if (transform.GetComponent<Collider>().enabled)
								{
									Physics.IgnoreCollision(transform.GetComponent<Collider>(), transform2.GetComponent<Collider>());
									Physics.IgnoreCollision(GameCharacter.allChars[num13].GetComponent<Collider>(), transform2.GetComponent<Collider>());
									Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), transform.GetComponent<Collider>());
								}
							}
						}
					}
					ability = Ability.None;
					playOnKill = false;
				}
				else
				{
					if (!playOnKill || !(model.GetComponent<Animation>()["die"].time > 0.7f))
					{
						return;
					}
					playOnKill = false;
					if (!Global.IsVoiceOn)
					{
						return;
					}
					bool flag = true;
					for (int num14 = GameCharacter.allChars.Count - 1; num14 >= 0; num14--)
					{
						if (GameCharacter.allChars[num14].Type == Character.Player && GameCharacter.allChars[num14].IsDead)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
					string[] currentVOV = GameSound.GetCurrentVOV();
					string[] array = currentVOV;
					foreach (string text in array)
					{
						if (text.StartsWith("cok_") && GameSound.GetTimeVOV(text) < 0.75f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						GameSound.StartVOV("cok_" + Random.Range(1, 31).ToString("D2"));
					}
				}
			}
			else
			{
				lifeTime -= Time.deltaTime;
				if (lifeTime <= 0f)
				{
					lifeTime = 0f;
					type = Character.Minion;
					Dead();
				}
			}
			return;
		}
		Vector3 localPosition4 = base.transform.localPosition;
		if (dead)
		{
			if (ability == Ability.Transformer)
			{
				if (model.GetComponent<Animation>().isPlaying)
				{
					float num15 = Mathf.Round(localPosition4.x);
					float num16 = Mathf.Round(localPosition4.z);
					if (localPosition4.x < num15)
					{
						localPosition4.x += Time.deltaTime;
						if (localPosition4.x > num15)
						{
							localPosition4.x = num15;
						}
					}
					else if (localPosition4.x > num15)
					{
						localPosition4.x -= Time.deltaTime;
						if (localPosition4.x < num15)
						{
							localPosition4.x = num15;
						}
					}
					if (localPosition4.z < num16)
					{
						localPosition4.z += Time.deltaTime;
						if (localPosition4.z > num16)
						{
							localPosition4.z = num16;
						}
					}
					else if (localPosition4.z > num16)
					{
						localPosition4.z -= Time.deltaTime;
						if (localPosition4.z < num16)
						{
							localPosition4.z = num16;
						}
					}
					base.transform.localPosition = localPosition4;
					return;
				}
				GameObject gameObject2 = GameCharacter.Create(Character.Monster, abilityRank, Global.Level);
				gameObject2.transform.parent = base.transform.parent;
				gameObject2.transform.localPosition = base.transform.localPosition;
				gameObject2.transform.Find("root").GetChild(0).localRotation = model.transform.localRotation;
				gameObject2.transform.localScale = new Vector3(10f, 10f, 10f);
				for (int num17 = GameCharacter.allChars.Count - 1; num17 >= 0; num17--)
				{
					if (gameObject2 != GameCharacter.allChars[num17].gameObject && GameCharacter.allChars[num17].GetComponent<Collider>().enabled)
					{
						Physics.IgnoreCollision(GameCharacter.allChars[num17].GetComponent<Collider>(), gameObject2.GetComponent<Collider>());
						if (GameCharacter.allChars[num17].Type == Character.Monster)
						{
							Transform transform3 = GameCharacter.allChars[num17].transform.Find("root");
							Transform transform4 = gameObject2.transform.Find("root");
							if (transform3.GetComponent<Collider>().enabled)
							{
								Physics.IgnoreCollision(transform3.GetComponent<Collider>(), transform4.GetComponent<Collider>());
								Physics.IgnoreCollision(GameCharacter.allChars[num17].GetComponent<Collider>(), transform4.GetComponent<Collider>());
								Physics.IgnoreCollision(gameObject2.GetComponent<Collider>(), transform3.GetComponent<Collider>());
							}
						}
					}
				}
				ability = Ability.None;
			}
			else
			{
				if (!playOnKill || !(model.GetComponent<Animation>()["die"].time > 0.7f))
				{
					return;
				}
				playOnKill = false;
				if (!Global.IsVoiceOn || Global.advStage <= 0)
				{
					return;
				}
				bool flag2 = true;
				for (int num18 = GameCharacter.allChars.Count - 1; num18 >= 0; num18--)
				{
					if (GameCharacter.allChars[num18].Type == Character.Player && GameCharacter.allChars[num18].IsDead)
					{
						flag2 = false;
						break;
					}
				}
				if (!flag2)
				{
					return;
				}
				string[] currentVOV2 = GameSound.GetCurrentVOV();
				string[] array2 = currentVOV2;
				foreach (string text2 in array2)
				{
					if (text2.StartsWith("cok_") && GameSound.GetTimeVOV(text2) < 0.75f)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					GameSound.StartVOV("cok_" + Random.Range(1, 31).ToString("D2"));
				}
			}
			return;
		}
		lifeTime += Time.deltaTime;
		if (base.IsStunned)
		{
			return;
		}
		if (gotoPoint.y == 0f)
		{
			int x2 = (int)gotoPoint.x;
			int y = (int)gotoPoint.z;
			if (!CanStand(x2, y))
			{
				gotoPoint = Vector3.down;
			}
			else
			{
				Vector3 vector = gotoPoint - localPosition4;
				float num19 = (float)level * 0.05f;
				if (vector.sqrMagnitude > 2.25f)
				{
					gotoPoint.y = -1f;
				}
				else if (IsSmartNotConfuse)
				{
					switch (gotoDir)
					{
					case Direction.Left:
						if (vector.x <= 0f - num19 && Mathf.Abs(vector.z) < 0.5f)
						{
							MoveRight();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Down:
						if (vector.z <= 0f - num19 && Mathf.Abs(vector.x) < 0.5f)
						{
							MoveUp();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Right:
						if (vector.x >= num19 && Mathf.Abs(vector.z) < 0.5f)
						{
							MoveLeft();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Up:
						if (vector.z >= num19 && Mathf.Abs(vector.x) < 0.5f)
						{
							MoveDown();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					}
				}
				else
				{
					switch (gotoDir)
					{
					case Direction.Left:
						if (vector.x <= 0f - num19 && Mathf.Abs(vector.z) < 0.5f)
						{
							MoveLeft();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Down:
						if (vector.z <= 0f - num19 && Mathf.Abs(vector.x) < 0.5f)
						{
							MoveDown();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Right:
						if (vector.x >= num19 && Mathf.Abs(vector.z) < 0.5f)
						{
							MoveRight();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					case Direction.Up:
						if (vector.z >= num19 && Mathf.Abs(vector.x) < 0.5f)
						{
							MoveUp();
						}
						else
						{
							gotoPoint.y = -1f;
						}
						break;
					}
				}
			}
		}
		if (gotoPoint.y == 0f)
		{
			return;
		}
		if (isCasting > 0f)
		{
			isCasting -= Time.deltaTime;
			if (isCasting > 0f)
			{
				int num20 = Mathf.RoundToInt(localPosition4.x);
				int num21 = Mathf.RoundToInt(localPosition4.z);
				float num22 = Time.deltaTime * 0.5f;
				float f = (float)num20 - localPosition4.x;
				float f2 = (float)num21 - localPosition4.z;
				if (Mathf.Abs(f) > num22)
				{
					if (localPosition4.x > (float)num20)
					{
						localPosition4.x -= num22;
					}
					else
					{
						localPosition4.x += num22;
					}
				}
				else
				{
					localPosition4.x = num20;
				}
				if (Mathf.Abs(f2) > num22)
				{
					if (localPosition4.z > (float)num21)
					{
						localPosition4.z -= num22;
					}
					else
					{
						localPosition4.z += num22;
					}
				}
				else
				{
					localPosition4.z = num21;
				}
				base.transform.localPosition = localPosition4;
			}
			else
			{
				isCasting = 0f;
				switch (ability)
				{
				case Ability.HitBlock:
					if (lifeTime < 1.5f)
					{
						Direction direction = (Direction)model.transform.localRotation.eulerAngles.y;
						int num24 = Mathf.RoundToInt(localPosition4.x);
						int num25 = Mathf.RoundToInt(localPosition4.z);
						switch (direction)
						{
						case Direction.Up:
							num25++;
							break;
						case Direction.Right:
							num24++;
							break;
						case Direction.Down:
							num25--;
							break;
						case Direction.Left:
							num24--;
							break;
						}
						Global.Map.RemoveBlock(num24, num25);
						Global.Map.AddFX(num24, num25, smoke, 3f, 1f);
						isCasting = 1f;
					}
					break;
				case Ability.Shoot:
					if (lifeTime >= 0.75f && lifeTime - Time.deltaTime < 0.75f)
					{
						isCasting = (float)abilityRank * 0.25f;
						bulletHead = localPosition4;
						bulletDir = (Direction)model.transform.localRotation.eulerAngles.y;
						switch (bulletDir)
						{
						case Direction.Up:
							bulletHead.z += 0.5f;
							break;
						case Direction.Right:
							bulletHead.x += 0.5f;
							break;
						case Direction.Down:
							bulletHead.z -= 0.5f;
							break;
						case Direction.Left:
							bulletHead.x -= 0.5f;
							break;
						}
						bulletTail = bulletHead;
						bulletFX = Global.Map.AddFX((int)bulletHead.x, (int)bulletHead.z, bulletRes, 0f, 1f);
						bulletFX.transform.localPosition = bulletHead;
						bulletFX.transform.localRotation = model.transform.localRotation;
						bulletFX.transform.localScale = new Vector3(10f, 10f, (abilityRank == 4) ? 10 : 0);
						if (abilityRank == 4)
						{
							Animation animation = bulletFX.transform.GetChild(0).GetComponent<Animation>();
							animation["idle"].time = 0f;
							animation["idle"].speed = 0f;
							bulletMat = bulletFX.GetComponentInChildren<Renderer>().material;
						}
					}
					break;
				case Ability.Creator:
				{
					GameObject gameObject3 = GameCharacter.Create(Character.Monster, abilityRank, Global.Level);
					gameObject3.transform.parent = base.transform.parent;
					gameObject3.transform.localPosition = new Vector3(Mathf.Round(localPosition4.x), 0f, Mathf.Round(localPosition4.z));
					gameObject3.transform.localScale = new Vector3(10f, 10f, 10f);
					for (int num23 = GameCharacter.allChars.Count - 1; num23 >= 0; num23--)
					{
						if (gameObject3 != GameCharacter.allChars[num23].gameObject && GameCharacter.allChars[num23].GetComponent<Collider>().enabled)
						{
							Physics.IgnoreCollision(GameCharacter.allChars[num23].GetComponent<Collider>(), gameObject3.GetComponent<Collider>());
							if (GameCharacter.allChars[num23].Type == Character.Monster)
							{
								Transform transform5 = GameCharacter.allChars[num23].transform.Find("root");
								Transform transform6 = gameObject3.transform.Find("root");
								if (transform5.GetComponent<Collider>().enabled)
								{
									Physics.IgnoreCollision(transform5.GetComponent<Collider>(), transform6.GetComponent<Collider>());
									Physics.IgnoreCollision(GameCharacter.allChars[num23].GetComponent<Collider>(), transform6.GetComponent<Collider>());
									Physics.IgnoreCollision(gameObject3.GetComponent<Collider>(), transform5.GetComponent<Collider>());
								}
							}
						}
					}
					break;
				}
				}
			}
		}
		else
		{
			UseAbility(localPosition4);
		}
		if (isCasting == 0f)
		{
			RunActionAsMonster(localPosition4);
		}
	}

	private void RunActionAsMonster(Vector3 p)
	{
		int num = Mathf.RoundToInt(p.x);
		int num2 = Mathf.RoundToInt(p.z);
		int[] array = new int[4]
		{
			num - 1,
			num,
			num + 1,
			num
		};
		int[] array2 = new int[4]
		{
			num2,
			num2 - 1,
			num2,
			num2 + 1
		};
		bool[] array3 = new bool[4];
		bool[] array4 = new bool[4];
		for (int i = 0; i < 4; i++)
		{
			array3[i] = CanStand(array[i], array2[i]);
			array4[i] = array3[i];
		}
		GameBomb[] array5 = new GameBomb[4];
		float[] array6 = new float[4];
		if (level >= 2)
		{
			float num3 = ((!IsInfected(Virus.SuperSlow)) ? 1f : 5f);
			num3 /= ModSpeed(1f);
			for (int j = 0; j < 4; j++)
			{
				if (array4[j])
				{
					array5[j] = IsSafeZone(array[j], array2[j], ref array6[j]);
					if (level >= 4 && array5[j] != null && array5[j].Type != Bomb.Remote && array6[j] > GetMinFuse(array[j], array2[j], array5[j]))
					{
						array5[j] = null;
					}
					if (array5[j] != null || Global.Map.IsOnFire(array[j], array2[j]))
					{
						array4[j] = false;
					}
				}
			}
		}
		int num4 = 0;
		for (int k = 0; k < 4; k++)
		{
			if (array4[k])
			{
				num4++;
			}
		}
		if (num4 > 0 || level >= 3)
		{
			for (int num5 = GameCharacter.allChars.Count - 1; num5 >= 0; num5--)
			{
				if (GameCharacter.allChars[num5].Type == Character.Player && !GameCharacter.allChars[num5].IsDead)
				{
					Vector3 localPosition = GameCharacter.allChars[num5].transform.localPosition;
					float sqrMagnitude = (localPosition - p).sqrMagnitude;
					int num6 = Mathf.RoundToInt(localPosition.x);
					int num7 = Mathf.RoundToInt(localPosition.z);
					if ((num6 == num || num7 == num2) && sqrMagnitude < (float)(level * 2 * (level * 2)))
					{
						int num8 = -1;
						if (num6 == num && num7 == num2)
						{
							SnapTo(num, num2, p);
						}
						else if (num6 == num)
						{
							bool flag = true;
							if (num7 > num2)
							{
								for (int l = num2 + 1; l < num7; l++)
								{
									if (!CanStand(num, l))
									{
										flag = false;
										break;
									}
								}
							}
							else
							{
								for (int num9 = num2 - 1; num9 > num7; num9--)
								{
									if (!CanStand(num, num9))
									{
										flag = false;
										break;
									}
								}
							}
							if (flag)
							{
								num8 = ((num7 <= num2) ? 1 : 3);
							}
						}
						else if (num7 == num2)
						{
							bool flag2 = true;
							if (num6 > num)
							{
								for (int m = num + 1; m < num6; m++)
								{
									if (!CanStand(m, num2))
									{
										flag2 = false;
										break;
									}
								}
							}
							else
							{
								for (int num10 = num - 1; num10 > num6; num10--)
								{
									if (!CanStand(num10, num2))
									{
										flag2 = false;
										break;
									}
								}
							}
							if (flag2)
							{
								num8 = ((num6 > num) ? 2 : 0);
							}
						}
						if (num8 != -1 && (array4[num8] || (level >= 3 && array3[num8] && sqrMagnitude < 1.5625f)))
						{
							switch (num8)
							{
							case 0:
								SmartMoveLeft(num, num2);
								break;
							case 1:
								SmartMoveDown(num, num2);
								break;
							case 2:
								SmartMoveRight(num, num2);
								break;
							case 3:
								SmartMoveUp(num, num2);
								break;
							}
							return;
						}
					}
				}
			}
			if (Random.value <= (float)(37 - level) / 48f)
			{
				switch (gotoDir)
				{
				case Direction.Left:
					if (array4[0])
					{
						SmartMoveLeft(num, num2);
					}
					break;
				case Direction.Down:
					if (array4[1])
					{
						SmartMoveDown(num, num2);
					}
					break;
				case Direction.Right:
					if (array4[2])
					{
						SmartMoveRight(num, num2);
					}
					break;
				case Direction.Up:
					if (array4[3])
					{
						SmartMoveUp(num, num2);
					}
					break;
				}
				if (gotoPoint.y == 0f)
				{
					return;
				}
			}
		}
		float timeFuse = -1f;
		GameBomb gameBomb = IsSafeZone(num, num2, ref timeFuse);
		if (num4 == 0 && gameBomb != null)
		{
			if (level >= 2)
			{
				Vector3 localPosition2 = gameBomb.transform.localPosition;
				int num11 = Mathf.RoundToInt(localPosition2.x);
				int num12 = Mathf.RoundToInt(localPosition2.z);
				if (num11 != num || num12 != num2)
				{
					if (num12 == num2)
					{
						if (num11 > num && array3[0])
						{
							SmartMoveLeft(num, num2);
						}
						else if (num11 < num && array3[2])
						{
							SmartMoveRight(num, num2);
						}
					}
					else if (num12 > num2 && array3[1])
					{
						SmartMoveDown(num, num2);
					}
					else if (num12 < num2 && array3[3])
					{
						SmartMoveUp(num, num2);
					}
					return;
				}
			}
			array4 = array3;
		}
		num4 = 0;
		for (int n = 0; n < 4; n++)
		{
			if (array4[n])
			{
				num4++;
			}
		}
		if (num4 > 0)
		{
			int num13;
			do
			{
				num13 = Random.Range(0, 4);
			}
			while (!array4[num13]);
			switch (num13)
			{
			case 0:
				SmartMoveLeft(num, num2);
				break;
			case 1:
				SmartMoveDown(num, num2);
				break;
			case 2:
				SmartMoveRight(num, num2);
				break;
			case 3:
				SmartMoveUp(num, num2);
				break;
			}
		}
		else
		{
			SnapTo(num, num2, p);
		}
	}

	private void UseAbility(Vector3 p)
	{
		switch (ability)
		{
		case Ability.Sticky:
			if (Random.value * 50f < (float)abilityRank)
			{
				GameGround.Create(Ground.Virus, Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z));
			}
			break;
		case Ability.Shoot:
		{
			if (!(lifeTime > 10f))
			{
				break;
			}
			for (int num2 = GameCharacter.allChars.Count - 1; num2 >= 0; num2--)
			{
				if (GameCharacter.allChars[num2].Type == Character.Player && !GameCharacter.allChars[num2].IsDead)
				{
					Vector3 localPosition = GameCharacter.allChars[num2].transform.localPosition;
					int num3 = Mathf.RoundToInt(p.x);
					int num4 = Mathf.RoundToInt(p.z);
					int num5 = Mathf.RoundToInt(localPosition.x);
					int num6 = Mathf.RoundToInt(localPosition.z);
					if (num5 != num3 && num6 != num4)
					{
						break;
					}
					int num7 = -1;
					if (num5 != num3 || num6 != num4)
					{
						if (num5 == num3)
						{
							bool flag = true;
							if (num6 > num4)
							{
								for (int i = num4 + 1; i < num6; i++)
								{
									if (!CanStand(num3, i, false))
									{
										flag = false;
										break;
									}
								}
							}
							else
							{
								for (int num8 = num4 - 1; num8 > num6; num8--)
								{
									if (!CanStand(num3, num8, false))
									{
										flag = false;
										break;
									}
								}
							}
							if (flag)
							{
								num7 = ((num6 <= num4) ? 1 : 3);
							}
						}
						else if (num6 == num4)
						{
							bool flag2 = true;
							if (num5 > num3)
							{
								for (int j = num3 + 1; j < num5; j++)
								{
									if (!CanStand(j, num4, false))
									{
										flag2 = false;
										break;
									}
								}
							}
							else
							{
								for (int num9 = num3 - 1; num9 > num5; num9--)
								{
									if (!CanStand(num9, num4, false))
									{
										flag2 = false;
										break;
									}
								}
							}
							if (flag2)
							{
								num7 = ((num5 > num3) ? 2 : 0);
							}
						}
					}
					if (num7 != -1)
					{
						int num10 = -1;
						switch (num7)
						{
						case 0:
							num10 = 270;
							break;
						case 1:
							num10 = 180;
							break;
						case 2:
							num10 = 90;
							break;
						case 3:
							num10 = 0;
							break;
						}
						model.transform.localRotation = Quaternion.Euler(0f, num10, 0f);
						model.GetComponent<Animation>().Play("spit");
						isCasting = 0.75f;
						lifeTime = 0f;
					}
					break;
				}
			}
			break;
		}
		case Ability.HitBlock:
		{
			if (!(lifeTime > (float)((5 - abilityRank) * 15)) || !(Random.value < 0.1f))
			{
				break;
			}
			int num11 = Mathf.RoundToInt(p.x);
			int num12 = Mathf.RoundToInt(p.z);
			int[] array = new int[4] { 0, 1, 0, -1 };
			int[] array2 = new int[4] { 1, 0, -1, 0 };
			int num13 = -1;
			for (int k = 0; k < 4; k++)
			{
				if (Global.Map.IsBlock(num11 + array[k], num12 + array2[k]) && (num13 == -1 || Random.value < 0.5f))
				{
					num13 = k;
				}
			}
			if (num13 != -1)
			{
				model.transform.localRotation = Quaternion.Euler(0f, num13 * 90, 0f);
				model.GetComponent<Animation>().Play("hit");
				isCasting = 1f;
				lifeTime = 0f;
				abilityUsed++;
			}
			break;
		}
		case Ability.Creator:
		{
			if (!((float)abilityUsed < lifeTime / 60f - 1f) || !(Random.value < 0.1f))
			{
				break;
			}
			List<GameObject> objects = Global.Map.GetObjects(Mathf.Round(p.x), Mathf.Round(p.z), 0.25f);
			if (objects == null || objects.Count <= 0)
			{
				break;
			}
			for (int num = objects.Count - 1; num >= 0; num--)
			{
				if (objects[num] == base.gameObject)
				{
					objects.RemoveAt(num);
					break;
				}
			}
			if (objects.Count == 0)
			{
				isCasting = 2f;
				abilityUsed++;
			}
			break;
		}
		case Ability.Transformer:
		case Ability.Respawner:
			break;
		}
	}

	private void SmartMoveLeft(int fromX, int fromY)
	{
		gotoPoint = new Vector3(fromX - 1, 0f, fromY);
		gotoDir = Direction.Left;
		if (IsSmartNotConfuse)
		{
			MoveRight();
		}
		else
		{
			MoveLeft();
		}
	}

	private void SmartMoveDown(int fromX, int fromY)
	{
		gotoPoint = new Vector3(fromX, 0f, fromY - 1);
		gotoDir = Direction.Down;
		if (IsSmartNotConfuse)
		{
			MoveUp();
		}
		else
		{
			MoveDown();
		}
	}

	private void SmartMoveRight(int fromX, int fromY)
	{
		gotoPoint = new Vector3(fromX + 1, 0f, fromY);
		gotoDir = Direction.Right;
		if (IsSmartNotConfuse)
		{
			MoveLeft();
		}
		else
		{
			MoveRight();
		}
	}

	private void SmartMoveUp(int fromX, int fromY)
	{
		gotoPoint = new Vector3(fromX, 0f, fromY + 1);
		gotoDir = Direction.Up;
		if (IsSmartNotConfuse)
		{
			MoveDown();
		}
		else
		{
			MoveUp();
		}
	}

	private bool SnapTo(int x, int y, Vector3 fromP)
	{
		float num = (IsInfected(Virus.CrazySpeed) ? 17f : ((!IsInfected(Virus.SuperSlow)) ? ((float)speed / 100f) : 0.85f));
		num *= Time.deltaTime;
		if (IsSmartNotConfuse)
		{
			if (fromP.x < (float)x - num)
			{
				MoveLeft();
			}
			else if (fromP.z < (float)y - num)
			{
				MoveDown();
			}
			else if (fromP.x > (float)x + num)
			{
				MoveRight();
			}
			else
			{
				if (!(fromP.z > (float)y + num))
				{
					return true;
				}
				MoveUp();
			}
		}
		else if (fromP.x > (float)x + num)
		{
			MoveLeft();
		}
		else if (fromP.z > (float)y + num)
		{
			MoveDown();
		}
		else if (fromP.x < (float)x - num)
		{
			MoveRight();
		}
		else
		{
			if (!(fromP.z < (float)y - num))
			{
				return true;
			}
			MoveUp();
		}
		return false;
	}

	private int GetBombFire(GameBomb bomb)
	{
		return (level >= 5) ? ((int)bomb.FireLv) : Mathf.Max(level * 2 - 1, 3);
	}

	private float GetMinFuse(int x, int y, GameBomb bomb)
	{
		if (bomb == null)
		{
			return -1f;
		}
		if (level >= 3 && bomb.Type == Bomb.Remote)
		{
			return 0f;
		}
		float num = ((!IsInfected(Virus.SuperSlow)) ? 1f : 5f);
		num /= ModSpeed(1f);
		int distance = GetDistance(x, y, bomb.transform.localPosition);
		int bombFire = GetBombFire(bomb);
		return ((float)(bombFire - distance + 1) / 7.5f + 1f) * num;
	}

	private GameBomb IsSafeZone(int x, int y, ref float timeFuse, GameBomb skip = null)
	{
		List<GameBomb> list = GameBomb.List();
		GameBomb gameBomb = null;
		float num = ((skip == null) ? 10f : ((skip.Type != Bomb.Remote) ? skip.TimeFuse : 0f));
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if ((level == 1 && list[num2].Owner != this) || list[num2] == skip || (list[num2].Type != Bomb.Remote && (num < list[num2].TimeFuse || (num == list[num2].TimeFuse && skip != null))))
			{
				continue;
			}
			Vector3 localPosition = list[num2].transform.localPosition;
			if (localPosition.y != 0f)
			{
				continue;
			}
			if ((num == list[num2].TimeFuse || (num == 0f && list[num2].Type == Bomb.Remote)) && gameBomb != null && skip == null)
			{
				Vector3 vector = new Vector3(x, 0f, y);
				if ((localPosition - vector).sqrMagnitude >= (gameBomb.transform.localPosition - vector).sqrMagnitude)
				{
					continue;
				}
			}
			int num3 = Mathf.RoundToInt(localPosition.x);
			int num4 = Mathf.RoundToInt(localPosition.z);
			float num5 = list[num2].TimeFuse;
			if (level >= 4 && list[num2].Type != Bomb.Remote)
			{
				float timeFuse2 = 0f;
				GameBomb gameBomb2 = IsSafeZone(num3, num4, ref timeFuse2, list[num2]);
				if (gameBomb2 != null && num5 > timeFuse2)
				{
					num5 = timeFuse2;
				}
			}
			int bombFire = GetBombFire(list[num2]);
			if (num3 == x && num4 == y)
			{
				gameBomb = list[num2];
				num = num5;
			}
			else if (num3 == x)
			{
				if (Mathf.Abs(num4 - y) > bombFire)
				{
					continue;
				}
				bool flag = true;
				for (int i = Mathf.Min(num4, y) + 1; i < Mathf.Max(num4, y); i++)
				{
					flag = CanDetonatePass(x, i, (level >= 3) ? list[num2].Type : Bomb.Normal);
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					gameBomb = list[num2];
					num = num5;
				}
			}
			else
			{
				if (num4 != y || Mathf.Abs(num3 - x) > bombFire)
				{
					continue;
				}
				bool flag2 = true;
				for (int j = Mathf.Min(num3, x) + 1; j < Mathf.Max(num3, x); j++)
				{
					flag2 = CanDetonatePass(j, y, (level >= 3) ? list[num2].Type : Bomb.Normal);
					if (!flag2)
					{
						break;
					}
				}
				if (flag2)
				{
					gameBomb = list[num2];
					num = num5;
				}
			}
		}
		if (gameBomb != null)
		{
			timeFuse = num;
		}
		return gameBomb;
	}

	private bool IsDeadEnd(int fromX, int fromY, int toX, int toY, bool checkSafe = false)
	{
		if (level >= 3)
		{
			int num = toX - fromX;
			int num2 = toY - fromY;
			if (CanStand(toX - num2, toY - num) || CanStand(toX + num2, toY + num))
			{
				float timeFuse = -1f;
				float timeFuse2 = -1f;
				GameBomb gameBomb = IsSafeZone(toX - num2, toY - num, ref timeFuse);
				GameBomb gameBomb2 = IsSafeZone(toX + num2, toY + num, ref timeFuse2);
				if (gameBomb != null && gameBomb2 != null)
				{
					if (CanStand(toX + num, toY + num2))
					{
						return IsDeadEnd(toX, toY, toX + num, toY + num2, true);
					}
					return true;
				}
				return false;
			}
			if (CanStand(toX + num, toY + num2))
			{
				if (checkSafe)
				{
					float timeFuse3 = -1f;
					GameBomb gameBomb3 = IsSafeZone(toX, toY, ref timeFuse3);
					if (gameBomb3 != null)
					{
						return true;
					}
				}
				return IsDeadEnd(toX, toY, toX + num, toY + num2, true);
			}
			return true;
		}
		return false;
	}

	private int GetDistance(int x, int y, Vector3 p)
	{
		int num = Mathf.RoundToInt(p.x);
		int num2 = Mathf.RoundToInt(p.z);
		if (num == x)
		{
			return Mathf.Abs(y - num2);
		}
		if (num2 == y)
		{
			return Mathf.Abs(x - num);
		}
		return -1;
	}

	private bool CanStand(int x, int y, bool checkGround = true)
	{
		List<GameObject> objects = Global.Map.GetObjects(x, y, 0.5f);
		if (objects == null)
		{
			return false;
		}
		for (int num = objects.Count - 1; num >= 0; num--)
		{
			if (!(objects[num] == base.gameObject))
			{
				if (objects[num].name == "Pillar")
				{
					return false;
				}
				if (objects[num].name == "Block")
				{
					return false;
				}
				if (objects[num].name.StartsWith("Monster"))
				{
					Vector3 vector = new Vector3(x, 0f, y);
					Vector3 vector2 = base.transform.localPosition - vector;
					vector2.y = 0f;
					Vector3 vector3 = objects[num].transform.localPosition - vector;
					vector3.y = 0f;
					float sqrMagnitude = vector2.sqrMagnitude;
					float sqrMagnitude2 = vector3.sqrMagnitude;
					if (sqrMagnitude == sqrMagnitude2)
					{
						return battleID > objects[num].GetComponent<GameCharacter>().battleID;
					}
					return sqrMagnitude < sqrMagnitude2;
				}
				if (objects[num].name.StartsWith("Bomb"))
				{
					return false;
				}
			}
		}
		if (checkGround)
		{
			if (Global.Level == 2)
			{
				List<GameGround> list = GameGround.List();
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					if (list[num2].Type == Ground.Crack && list[num2].GetComponent<Collider>() != null)
					{
						Vector3 localPosition = list[num2].transform.localPosition;
						if (localPosition.x == (float)x && localPosition.z == (float)y)
						{
							return false;
						}
					}
				}
			}
			else if (Global.Level == 5)
			{
				List<GameGround> list2 = GameGround.List();
				for (int num3 = list2.Count - 1; num3 >= 0; num3--)
				{
					if (list2[num3].Type == Ground.Pull)
					{
						Vector3 localPosition2 = list2[num3].transform.localPosition;
						float num4 = list2[num3].transform.localScale.x / 20f;
						num4 -= 1f;
						float num5 = localPosition2.x - (float)x;
						float num6 = localPosition2.z - (float)y;
						if (num5 * num5 + num6 * num6 < num4 * num4)
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	private bool CanDetonatePass(int x, int y, Bomb type)
	{
		List<GameObject> objects = Global.Map.GetObjects(x, y, 0.25f);
		if (objects == null)
		{
			return false;
		}
		for (int num = objects.Count - 1; num >= 0; num--)
		{
			if (objects[num].name == "Pillar")
			{
				return false;
			}
			if (objects[num].name == "Block" && type != Bomb.Pierce)
			{
				return false;
			}
			if (objects[num].name.StartsWith("Bomb") && type != Bomb.Pierce)
			{
				return false;
			}
			if (objects[num].name.StartsWith("Item") && type != Bomb.Pierce)
			{
				return false;
			}
		}
		return true;
	}
}
