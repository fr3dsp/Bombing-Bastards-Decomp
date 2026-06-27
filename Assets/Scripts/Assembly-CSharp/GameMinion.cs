using TNet;
using UnityEngine;

public class GameMinion : GameCharacter
{
	private int level = 1;

	private float sporeTime = -1f;

	private ParticleSystem sporeFX;

	private float fireTime;

	private Vector3 moveTar = Vector3.up;

	private float plasmaTime;

	private Material[] plasmaMat;

	protected override void Init(int param)
	{
		level = param;
	}

	public override void Dead(int killerID = -1)
	{
		if (level == 5)
		{
			if (base.transform.parent.name != "Characters")
			{
				return;
			}
			plasmaTime = 0f;
		}
		if (level < 3)
		{
			base.Dead(killerID);
		}
	}

	protected override float ModSpeed(float rawSpeed)
	{
		return rawSpeed;
	}

	private void OnCollisionStay(Collision collision)
	{
		switch (level)
		{
		case 3:
			if (collision.gameObject.name.StartsWith("Player"))
			{
				collision.gameObject.GetComponent<GameCharacter>().Dead();
			}
			else if (collision.gameObject.name.StartsWith("Bomb"))
			{
				GameBomb component = collision.gameObject.GetComponent<GameBomb>();
				float x = base.transform.localScale.x;
				if (x > 8f && component.Type != Bomb.XtraCharge)
				{
					collision.gameObject.GetComponent<GameBomb>().Explode();
				}
				else if (x == 8f)
				{
					base.transform.localScale = new Vector3(7.999f, 7.999f, 7.999f);
				}
				else if (x < 4f)
				{
					base.transform.localScale = new Vector3(-0.01f, -0.01f, -0.01f);
				}
			}
			break;
		case 5:
			if (collision.gameObject.name == "Explosion")
			{
				plasmaTime = 0f;
				base.Dead();
			}
			break;
		case 4:
			break;
		}
	}

	protected override void ActionUpdate()
	{
		if (level == 3)
		{
			GameCharacter gameCharacter = null;
			for (int num = GameCharacter.allChars.Count - 1; num >= 0; num--)
			{
				if (GameCharacter.allChars[num].Type == Character.Minion && !GameCharacter.allChars[num].IsDead && GameCharacter.allChars[num].transform.localScale.x >= 8f)
				{
					gameCharacter = GameCharacter.allChars[num];
					break;
				}
			}
			if (gameCharacter == null)
			{
				if (GameSound.IsPlayingSFX("skillStorm"))
				{
					GameSound.StopSFX("skillStorm");
				}
			}
			else if (!GameSound.IsPlayingSFX("skillStorm"))
			{
				GameSound.StartSFX("skillStorm");
			}
		}
		else if (level == 5)
		{
			if (plasmaMat == null)
			{
				plasmaMat = new Material[4];
				string[] array = new string[4] { "node/pPipe1", "node/pPipe2", "node/pSphere1", "node/pSphere2" };
				for (int i = 0; i < 4; i++)
				{
					plasmaMat[i] = model.transform.Find(array[i]).GetComponent<Renderer>().material;
					if (i > 1)
					{
						float num2 = (float)(i - 1) * 0.5f;
						plasmaMat[i].SetColor("_TintColor", new Color(num2, num2, num2, num2));
					}
				}
				model.transform.parent.localPosition = Vector3.zero;
				model.transform.parent.localRotation = Quaternion.identity;
			}
			plasmaTime += Time.deltaTime;
			if (dead)
			{
				if (plasmaTime < 0.5f)
				{
					float num3 = 0.5f - plasmaTime;
					for (int j = 0; j < 2; j++)
					{
						plasmaMat[j].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, num3));
					}
					for (int k = 2; k < 4; k++)
					{
						float num4 = (float)(k - 1) * 0.5f;
						plasmaMat[k].SetColor("_TintColor", new Color(num4, num4, num4, num4 * num3));
					}
				}
				else
				{
					for (int l = 0; l < 4; l++)
					{
						plasmaMat[l].SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0f));
					}
					plasmaTime = 1f;
				}
			}
			else
			{
				while (plasmaTime >= 1f)
				{
					plasmaTime -= 1f;
				}
			}
			Vector2 offset = new Vector2(0f, (float)((int)(plasmaTime * 16f) % 8) / 8f);
			plasmaMat[0].SetTextureOffset("_MainTex", offset);
			plasmaMat[1].SetTextureOffset("_MainTex", offset);
			if (base.transform.parent.name != "Characters")
			{
				return;
			}
			GameCharacter gameCharacter2 = null;
			for (int num5 = GameCharacter.allChars.Count - 1; num5 >= 0; num5--)
			{
				if (GameCharacter.allChars[num5].Type == Character.Minion && !GameCharacter.allChars[num5].IsDead && GameCharacter.allChars[num5].transform.localPosition.y < 0.5f)
				{
					gameCharacter2 = GameCharacter.allChars[num5];
					break;
				}
			}
			if (gameCharacter2 == null)
			{
				if (GameSound.IsPlayingSFX("skillPlasmaBall"))
				{
					GameSound.StopSFX("skillPlasmaBall");
				}
			}
			else if (!GameSound.IsPlayingSFX("skillPlasmaBall"))
			{
				GameSound.StartSFX("skillPlasmaBall");
			}
		}
		if (dead)
		{
			return;
		}
		switch (level)
		{
		case 1:
			if (sporeTime == -1f)
			{
				sporeTime = 0f;
				model.GetComponent<Animation>().CrossFadeQueued("stand");
			}
			sporeTime += Time.deltaTime;
			if (sporeTime >= 2f)
			{
				sporeTime = 0f;
				if (sporeFX == null)
				{
					sporeFX = base.gameObject.GetComponentInChildren<ParticleSystem>();
				}
				sporeFX.Play();
			}
			else
			{
				if (!(sporeTime > 0.25f) || !(sporeTime < 0.75f))
				{
					break;
				}
				Vector3 localPosition6 = base.transform.localPosition;
				for (int num22 = GameCharacter.allChars.Count - 1; num22 >= 0; num22--)
				{
					if (GameCharacter.allChars[num22].Type == Character.Player && !GameCharacter.allChars[num22].IsDead)
					{
						Vector3 localPosition7 = GameCharacter.allChars[num22].transform.localPosition;
						float num23 = localPosition7.x - localPosition6.x;
						float num24 = localPosition7.z - localPosition6.z;
						if (num23 * num23 + num24 * num24 < 2.640625f)
						{
							GameCharacter.allChars[num22].Infect(Virus.SuperSlow, 2.5f);
							break;
						}
					}
				}
			}
			break;
		case 2:
		case 4:
		{
			Vector3 localPosition8 = base.transform.localPosition;
			bool num25;
			if (level == 4)
			{
				if (!(localPosition8.y >= 0f))
				{
					break;
				}
				num25 = fireTime < 0.125f;
			}
			else
			{
				num25 = localPosition8.y > 0f;
			}
			if (!num25)
			{
				break;
			}
			bool flag = localPosition8.y > 0f;
			localPosition8.y -= 25f * Time.deltaTime;
			if (localPosition8.y < 0f)
			{
				localPosition8.y = 0f;
			}
			if (localPosition8.y == 0f && flag)
			{
				GameSound.StartSFX("skillCrash");
			}
			if (localPosition8.y < 1f)
			{
				float num26 = ((level != 4) ? 0.5625f : 3.0625f);
				List<GameItem> list3 = GameItem.List();
				for (int num27 = list3.Count - 1; num27 >= 0; num27--)
				{
					if (!list3[num27].IsDeactived)
					{
						Vector3 localPosition9 = list3[num27].transform.localPosition;
						float num28 = localPosition9.x - localPosition8.x;
						float num29 = localPosition9.z - localPosition8.z;
						if (num28 * num28 + num29 * num29 < num26)
						{
							list3[num27].Deactive();
							break;
						}
					}
				}
				List<GameBomb> list4 = GameBomb.List();
				for (int num30 = list4.Count - 1; num30 >= 0; num30--)
				{
					if (!list4[num30].IsExploded && !list4[num30].IsHeld)
					{
						Vector3 localPosition10 = list4[num30].transform.localPosition;
						float num31 = localPosition10.x - localPosition8.x;
						float num32 = localPosition10.z - localPosition8.z;
						if (num31 * num31 + num32 * num32 < num26)
						{
							list4[num30].Explode();
							break;
						}
					}
				}
				for (int num33 = GameCharacter.allChars.Count - 1; num33 >= 0; num33--)
				{
					if (GameCharacter.allChars[num33].Type == Character.Player)
					{
						Vector3 localPosition11 = GameCharacter.allChars[num33].transform.localPosition;
						float num34 = localPosition11.x - localPosition8.x;
						float num35 = localPosition11.z - localPosition8.z;
						if (num34 * num34 + num35 * num35 < num26)
						{
							GameCharacter.allChars[num33].Dead();
							break;
						}
					}
				}
				fireTime += Time.deltaTime;
			}
			if (localPosition8.y == 0f)
			{
				if (level == 2)
				{
					base.gameObject.GetComponentInChildren<ParticleSystem>().Play();
				}
				else if (base.GetComponent<Collider>().enabled)
				{
					base.GetComponent<Collider>().enabled = false;
					ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
					ParticleSystem[] array3 = componentsInChildren;
					foreach (ParticleSystem particleSystem in array3)
					{
						if (particleSystem.loop)
						{
							particleSystem.Stop();
						}
						else
						{
							particleSystem.Play();
						}
					}
					GameShadow.RemoveObject(base.gameObject);
					Object.Destroy(model);
					Object.Destroy(base.gameObject, 2.5f);
				}
			}
			base.transform.localPosition = localPosition8;
			break;
		}
		case 5:
		{
			Vector3 localPosition = base.transform.localPosition;
			Direction direction = (Direction)model.transform.localRotation.eulerAngles.y;
			if (Mathf.RoundToInt(base.transform.localScale.x * 10000f) == 150000)
			{
				float num6 = 3.3333333f * Time.deltaTime;
				switch (direction)
				{
				case Direction.Left:
					if (localPosition.x == -8f)
					{
						plasmaTime = 0f;
						base.Dead();
						break;
					}
					localPosition.x -= num6;
					if (localPosition.x < -8f)
					{
						localPosition.x = -8f;
					}
					break;
				case Direction.Down:
					if (localPosition.z == -5f)
					{
						plasmaTime = 0f;
						base.Dead();
						break;
					}
					localPosition.z -= num6;
					if (localPosition.z < -5f)
					{
						localPosition.z = -5f;
					}
					break;
				case Direction.Right:
					if (localPosition.x == 8f)
					{
						plasmaTime = 0f;
						base.Dead();
						break;
					}
					localPosition.x += num6;
					if (localPosition.x > 8f)
					{
						localPosition.x = 8f;
					}
					break;
				case Direction.Up:
					if (localPosition.z == 5f)
					{
						plasmaTime = 0f;
						base.Dead();
						break;
					}
					localPosition.z += num6;
					if (localPosition.z > 5f)
					{
						localPosition.z = 5f;
					}
					break;
				}
				localPosition.y = 0f;
				base.transform.localPosition = localPosition;
			}
			else if (base.transform.localScale.x == 10f)
			{
				if (moveTar.y != 0f)
				{
					moveTar.x = Mathf.Round(localPosition.x);
					moveTar.y = 0f;
					moveTar.z = Mathf.Round(localPosition.z);
					Direction direction2 = direction;
					if (direction2 != Direction.Up)
					{
						if (direction2 != Direction.Right)
						{
							if (direction2 != Direction.Down)
							{
								if (direction2 == Direction.Left)
								{
									do
									{
										moveTar.x -= 1f;
									}
									while ((int)moveTar.x % 2 != 0);
								}
							}
							else
							{
								do
								{
									moveTar.z -= 1f;
								}
								while ((int)moveTar.z % 2 == 0);
							}
						}
						else
						{
							do
							{
								moveTar.x += 1f;
							}
							while ((int)moveTar.x % 2 != 0);
						}
					}
					else
					{
						do
						{
							moveTar.z += 1f;
						}
						while ((int)moveTar.z % 2 == 0);
					}
				}
				float num7 = 3.3333333f * Time.deltaTime;
				Vector3 vector = moveTar - localPosition;
				if (vector.x * vector.x + vector.z * vector.z > num7 * num7)
				{
					switch (direction)
					{
					case Direction.Left:
						localPosition.x -= num7;
						break;
					case Direction.Down:
						localPosition.z -= num7;
						break;
					case Direction.Right:
						localPosition.x += num7;
						break;
					case Direction.Up:
						localPosition.z += num7;
						break;
					}
				}
				else
				{
					localPosition = moveTar;
					Vector3[] array2 = new Vector3[4]
					{
						new Vector3(-2f, 0f, 0f),
						new Vector3(0f, 0f, -2f),
						new Vector3(2f, 0f, 0f),
						new Vector3(0f, 0f, 2f)
					};
					for (int m = 0; m < 4; m++)
					{
						array2[m] += moveTar;
					}
					for (int num8 = GameCharacter.allChars.Count - 1; num8 >= 0; num8--)
					{
						if (GameCharacter.allChars[num8].Type == Character.Player)
						{
							Vector3 localPosition2 = GameCharacter.allChars[num8].transform.localPosition;
							float num9 = (localPosition2 - moveTar).sqrMagnitude;
							int num10 = -1;
							for (int n = 0; n < 4; n++)
							{
								if (!(Mathf.Abs(array2[n].x) > 8f) && !(Mathf.Abs(array2[n].z) > 5f))
								{
									float sqrMagnitude = (localPosition2 - array2[n]).sqrMagnitude;
									if (num9 > sqrMagnitude)
									{
										num9 = sqrMagnitude;
										num10 = n;
									}
								}
							}
							if (num10 != -1)
							{
								moveTar = array2[num10];
								switch (num10)
								{
								case 0:
									direction = Direction.Left;
									break;
								case 1:
									direction = Direction.Down;
									break;
								case 2:
									direction = Direction.Right;
									break;
								case 3:
									direction = Direction.Up;
									break;
								}
								model.transform.localRotation = Quaternion.Euler(0f, (float)direction, 0f);
							}
							else
							{
								plasmaTime = 0f;
								base.Dead();
							}
							break;
						}
					}
				}
				localPosition.y = 0f;
				base.transform.localPosition = localPosition;
			}
			if (!(localPosition.y < 1f))
			{
				break;
			}
			float num11 = 0.25f + 1.25f * base.transform.localScale.x / 10f;
			float num12 = num11 * num11;
			for (int num13 = GameCharacter.allChars.Count - 1; num13 >= 0; num13--)
			{
				if (GameCharacter.allChars[num13].Type == Character.Player && !GameCharacter.allChars[num13].IsDead)
				{
					Vector3 localPosition3 = GameCharacter.allChars[num13].transform.localPosition;
					float num14 = localPosition3.x - localPosition.x;
					float num15 = localPosition3.z - localPosition.z;
					if (num14 * num14 + num15 * num15 < num12)
					{
						GameCharacter.allChars[num13].Dead();
					}
				}
			}
			List<GameBomb> list = GameBomb.List();
			for (int num16 = list.Count - 1; num16 >= 0; num16--)
			{
				if (!list[num16].IsExploded && !list[num16].IsHeld)
				{
					Vector3 localPosition4 = list[num16].transform.localPosition;
					float num17 = localPosition4.x - localPosition.x;
					float num18 = localPosition4.z - localPosition.z;
					if (num17 * num17 + num18 * num18 < num12)
					{
						list[num16].Explode();
					}
				}
			}
			List<GameItem> list2 = GameItem.List();
			for (int num19 = list2.Count - 1; num19 >= 0; num19--)
			{
				if (!list2[num19].IsDeactived)
				{
					Vector3 localPosition5 = list2[num19].transform.localPosition;
					float num20 = localPosition5.x - localPosition.x;
					float num21 = localPosition5.z - localPosition.z;
					if (num20 * num20 + num21 * num21 < num12)
					{
						list2[num19].Deactive();
					}
				}
			}
			break;
		}
		case 3:
			break;
		}
	}
}
