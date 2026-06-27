using TNet;
using UnityEngine;

public class GamePlayer : GameCharacter
{
	private float speedMod = 1f;

	protected override void Init(int param)
	{
	}

	protected override float ModSpeed(float rawSpeed)
	{
		if (Global.pirated)
		{
			if (Global.Mode == GameMode.Adventure)
			{
				return speedMod * rawSpeed * ((float)(30 - Global.advStage) / 30f * 0.8f + 0.2f);
			}
			if (Global.Mode == GameMode.LocalBattle)
			{
				int num = 0;
				for (int num2 = Global.cupAmount.Length - 1; num2 >= 0; num2--)
				{
					if (num < Global.cupAmount[num2])
					{
						num = Global.cupAmount[num2];
					}
				}
				num += 5 - Global.winAmount;
				return speedMod * rawSpeed * (1f - (float)num / 5f);
			}
		}
		return speedMod * rawSpeed;
	}

	protected override void ActionUpdate()
	{
		if (isGhost)
		{
			GameController controller = GameInput.GetController((Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
			if (controller.IsConnected())
			{
				int num = Global.Map.CountBomb(this, true);
				if (controller.DoBombDrop() && num < 1 && GamePointer.isHit((Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID))
				{
					shootStep = 1;
				}
			}
		}
		else
		{
			if (dead)
			{
				return;
			}
			float num2 = 0f;
			float num3 = 0f;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (Global.Mode == GameMode.Adventure)
			{
				for (int i = 0; i < 5; i++)
				{
					GameController controller2 = GameInput.GetController(i);
					num2 += controller2.GetHorizontal();
					num3 += controller2.GetVertical();
					flag |= controller2.DoBombDrop();
					flag2 |= controller2.DoBombPick();
					flag3 |= controller2.DoBombRemote();
				}
				if (num2 < -1f)
				{
					num2 = -1f;
				}
				else if (num2 > 1f)
				{
					num2 = 1f;
				}
				if (num3 < -1f)
				{
					num3 = -1f;
				}
				else if (num3 > 1f)
				{
					num3 = 1f;
				}
			}
			else
			{
				GameController controller3 = GameInput.GetController((Global.Mode != GameMode.OnlineBattle) ? controllerID : battleID);
				num2 = controller3.GetHorizontal();
				num3 = controller3.GetVertical();
				flag = controller3.DoBombDrop();
				flag2 = controller3.DoBombPick();
				flag3 = controller3.DoBombRemote();
				if (Global.Mode == GameMode.OnlineBattle && Global.onlinePlayerID == controllerID && GameRemoteController.IgnorePlayerAction)
				{
					num2 = 0f;
					num3 = 0f;
				}
			}
			Vector3 localPosition = base.transform.localPosition;
			speedMod = Mathf.Sqrt(num2 * num2 + num3 * num3);
			if (speedMod > 1f)
			{
				speedMod = 1f;
			}
			int num4;
			int num5;
			if (Mathf.Abs(num2) > Mathf.Abs(num3) * 2f)
			{
				num4 = ((num2 > 0f) ? 1 : (-1));
				num5 = 0;
			}
			else if (Mathf.Abs(num3) > Mathf.Abs(num2) * 2f)
			{
				num4 = 0;
				num5 = ((num3 > 0f) ? 1 : (-1));
			}
			else
			{
				num4 = ((num2 > 0f) ? 1 : ((num2 < 0f) ? (-1) : 0));
				num5 = ((num3 > 0f) ? 1 : ((num3 < 0f) ? (-1) : 0));
			}
			int num6 = num4;
			int num7 = num5;
			int num8 = 0;
			int num9 = 0;
			bool flag4;
			if (num4 == 0 || num5 == 0)
			{
				if (num4 != 0)
				{
					int num10 = Mathf.RoundToInt(localPosition.x);
					if (num10 % 2 != 0 || Mathf.Abs((float)num10 - localPosition.x) > 1f / 6f || (num4 > 0 && localPosition.x > 7f) || (num4 < 0 && localPosition.x < -7f))
					{
						flag4 = true;
					}
					else
					{
						int num11 = Mathf.RoundToInt(localPosition.z);
						if (Mathf.Abs((float)num11 - localPosition.z) > 1f / 6f)
						{
							num8 = num10;
							num9 = num11;
							num8 = ((num4 <= 0) ? (num8 - 1) : (num8 + 1));
							if (num11 % 2 != 0)
							{
								num7 = ((!((float)num11 > localPosition.z)) ? (num7 + 1) : (num7 - 1));
							}
							else if ((float)num11 > localPosition.z)
							{
								num7++;
								num9--;
							}
							else
							{
								num7--;
								num9++;
							}
							flag4 = false;
						}
						else
						{
							flag4 = true;
						}
					}
				}
				else if (num5 != 0)
				{
					int num12 = Mathf.RoundToInt(localPosition.z);
					if (num12 % 2 == 0 || Mathf.Abs((float)num12 - localPosition.z) > 1f / 6f || (num5 < 0 && localPosition.z > 4f) || (num5 > 0 && localPosition.z < -4f))
					{
						flag4 = true;
					}
					else
					{
						int num13 = Mathf.RoundToInt(localPosition.x);
						if (Mathf.Abs((float)num13 - localPosition.x) > 1f / 6f)
						{
							num8 = num13;
							num9 = num12;
							num9 = ((num5 >= 0) ? (num9 - 1) : (num9 + 1));
							if (num13 % 2 == 0)
							{
								num6 = ((!((float)num13 > localPosition.x)) ? (num6 - 1) : (num6 + 1));
							}
							else if ((float)num13 > localPosition.x)
							{
								num6--;
								num8--;
							}
							else
							{
								num6++;
								num8++;
							}
							flag4 = false;
						}
						else
						{
							flag4 = true;
						}
					}
				}
				else
				{
					flag4 = false;
				}
			}
			else
			{
				flag4 = true;
			}
			if (!flag4)
			{
				if (Global.Map.IsBlock(num8, num9))
				{
					flag4 = true;
				}
				else
				{
					List<GameBomb> list = GameBomb.List();
					for (int num14 = list.Count - 1; num14 >= 0; num14--)
					{
						Vector3 localPosition2 = list[num14].transform.localPosition;
						if (localPosition2.y < 0.5f && Mathf.Abs(localPosition2.x - (float)num8) < 0.25f && Mathf.Abs(localPosition2.z - (float)num9) < 0.25f)
						{
							flag4 = true;
							break;
						}
					}
					if (!flag4)
					{
						if (num6 > 0)
						{
							MoveRight();
						}
						else if (num6 < 0)
						{
							MoveLeft();
						}
						if (num7 < 0)
						{
							MoveUp();
						}
						else if (num7 > 0)
						{
							MoveDown();
						}
					}
				}
			}
			if (flag4)
			{
				if (num4 > 0)
				{
					MoveRight();
				}
				else if (num4 < 0)
				{
					MoveLeft();
				}
				if (num5 < 0)
				{
					MoveUp();
				}
				else if (num5 > 0)
				{
					MoveDown();
				}
			}
			if (Global.Mode != GameMode.OnlineBattle || TNManager.isHosting)
			{
				if (flag)
				{
					DropBomb();
				}
				if (flag2)
				{
					PickBomb();
				}
				if (flag3)
				{
					RemoteBomb();
				}
			}
		}
	}

	[RFC(30)]
	private void GamePlayerUpdatePhysic(int matchID, Vector3 localPosition, int modelRotate, float modelScale, Vector3 ghostRotate, float ghostScale, Vector2 externalForce)
	{
		if (Global.onlineMatchID == matchID)
		{
			base.transform.localPosition = localPosition;
			model.transform.localRotation = Quaternion.Euler(0f, modelRotate, 0f);
			model.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
			ghostModel.transform.localRotation = Quaternion.Euler(ghostRotate);
			ghostModel.transform.localScale = new Vector3(ghostScale, ghostScale, ghostScale);
			base.externalForce = externalForce;
		}
	}

	[RFC(31)]
	private void GamePlayerGetItem(int matchID, int item)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementGetItem((Item)item);
			Debug.Log("GamePlayerGetItem");
		}
	}

	[RFC(32)]
	private void GamePlayerInfect(int matchID, int type, float duration)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementInfect((Virus)type, duration);
			Debug.Log("GamePlayerInfect");
		}
	}

	[RFC(33)]
	private void GamePlayerDropBomb(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementDropBomb();
			Debug.Log("GamePlayerDropBomb");
		}
	}

	[RFC(34)]
	private void GamePlayerPickBomb(int matchID, int bombID)
	{
		if (Global.onlineMatchID != matchID)
		{
			return;
		}
		foreach (GameBomb item in GameBomb.List())
		{
			if (item.GetComponent<TNObject>().uid == (uint)bombID)
			{
				ImplementPickBomb(item);
				Debug.Log("GamePlayerPickBomb");
				break;
			}
		}
	}

	[RFC(35)]
	private void GamePlayerThrowBomb(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementThrowBomb();
			Debug.Log("GamePlayerThrowBomb");
		}
	}

	[RFC(36)]
	private void GamePlayerRemoteBomb(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementRemoteBomb();
			Debug.Log("GamePlayerRemoteBomb");
		}
	}

	[RFC(37)]
	private void GamePlayerStun(int matchID)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementStun();
			Debug.Log("GamePlayerStun");
		}
	}

	[RFC(38)]
	private void GamePlayerDead(int matchID, int killerID)
	{
		if (Global.onlineMatchID == matchID)
		{
			ImplementDead(killerID);
			Debug.Log("GamePlayerDead");
		}
	}
}
