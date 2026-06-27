using TNet;
using UnityEngine;

public class GameAI : GameCharacter
{
	private enum Mode
	{
		Offend = 0,
		Defend = 1
	}

	private Mode mode;

	private int level = 1;

	private Vector3 gotoPoint = Vector3.down;

	private Direction gotoDir = Direction.None;

	private Vector3 runAwayFrom = Vector3.down;

	private bool IsSmartNotConfuse
	{
		get
		{
			return level > 4 && IsInfected(Virus.Confusing);
		}
	}

	protected override void Init(int param)
	{
		level = param;
	}

	protected override float ModSpeed(float rawSpeed)
	{
		return rawSpeed;
	}

	protected override void ActionUpdate()
	{
		if (isGhost)
		{
			int num = Global.Map.CountBomb(this);
			if (num < 1 && shootStep == 0)
			{
				shootStep = 1;
			}
		}
		else
		{
			if (dead)
			{
				return;
			}
			Vector3 localPosition = base.transform.localPosition;
			if (gotoPoint.y == 0f)
			{
				int x = (int)gotoPoint.x;
				int y = (int)gotoPoint.z;
				if (!CanStand(x, y))
				{
					gotoPoint = Vector3.down;
				}
				else
				{
					Vector3 vector = gotoPoint - localPosition;
					float num2 = (float)level * 0.05f;
					if (vector.sqrMagnitude > 2.25f)
					{
						gotoPoint.y = -1f;
					}
					else if (IsSmartNotConfuse)
					{
						switch (gotoDir)
						{
						case Direction.Left:
							if (vector.x <= 0f - num2 && Mathf.Abs(vector.z) < 0.5f)
							{
								MoveRight();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Down:
							if (vector.z <= 0f - num2 && Mathf.Abs(vector.x) < 0.5f)
							{
								MoveUp();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Right:
							if (vector.x >= num2 && Mathf.Abs(vector.z) < 0.5f)
							{
								MoveLeft();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Up:
							if (vector.z >= num2 && Mathf.Abs(vector.x) < 0.5f)
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
							if (vector.x <= 0f - num2 && Mathf.Abs(vector.z) < 0.5f)
							{
								MoveLeft();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Down:
							if (vector.z <= 0f - num2 && Mathf.Abs(vector.x) < 0.5f)
							{
								MoveDown();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Right:
							if (vector.x >= num2 && Mathf.Abs(vector.z) < 0.5f)
							{
								MoveRight();
							}
							else
							{
								gotoPoint.y = -1f;
							}
							break;
						case Direction.Up:
							if (vector.z >= num2 && Mathf.Abs(vector.x) < 0.5f)
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
			if (gotoPoint.y != 0f)
			{
				switch (type)
				{
				case Character.AI:
					RunActionAsAI(localPosition);
					break;
				case Character.Monster:
					RunActionAsMonster(localPosition);
					break;
				}
			}
		}
	}

	private void RunActionAsAI(Vector3 p)
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
		Direction[] array3 = new Direction[4]
		{
			Direction.Left,
			Direction.Down,
			Direction.Right,
			Direction.Up
		};
		float timeFuse = -1f;
		GameBomb gameBomb = IsSafeZone(num, num2, ref timeFuse);
		float num3 = 1000f;
		GameCharacter gameCharacter = null;
		for (int num4 = GameCharacter.allChars.Count - 1; num4 >= 0; num4--)
		{
			if (GameCharacter.allChars[num4] != this && !GameCharacter.allChars[num4].IsDead)
			{
				float sqrMagnitude = (GameCharacter.allChars[num4].transform.localPosition - p).sqrMagnitude;
				if (num3 > sqrMagnitude)
				{
					num3 = sqrMagnitude;
					gameCharacter = GameCharacter.allChars[num4];
				}
			}
		}
		int num5 = int.MaxValue;
		int num6 = int.MaxValue;
		int num7 = -1;
		if (gameCharacter != null)
		{
			Vector3 localPosition = gameCharacter.transform.localPosition;
			num5 = Mathf.RoundToInt(localPosition.x);
			num6 = Mathf.RoundToInt(localPosition.z);
			if (num5 == num && num6 == num2)
			{
				num7 = 4;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					if (array[i] == num5 && array2[i] == num6)
					{
						num7 = i;
						break;
					}
				}
			}
		}
		else
		{
			mode = Mode.Defend;
		}
		if (mode == Mode.Offend)
		{
			if (gameBomb == null)
			{
				RemoteBomb();
				bool[] array4 = new bool[4];
				float[] array5 = new float[4];
				bool[] array6 = new bool[4];
				GameBomb[] array7 = new GameBomb[4];
				float[] array8 = new float[4];
				float[] array9 = new float[4];
				for (int j = 0; j < 4; j++)
				{
					array4[j] = CanStand(array[j], array2[j]);
					array7[j] = ((!array4[j]) ? null : IsSafeZone(array[j], array2[j], ref array8[j]));
					array6[j] = !array4[j] || Global.Map.IsOnFire(array[j], array2[j]);
					array5[j] = ((!array6[j]) ? ((!(array7[j] == null)) ? array8[j] : 10f) : (-1f));
					array9[j] = GetMinFuse(array[j], array2[j], array7[j]);
				}
				int num8 = 0;
				for (int k = 0; k < 4; k++)
				{
					if (array5[k] == 10f || (array5[k] > array9[k] && k == num7))
					{
						num8++;
					}
				}
				if (num8 == 1)
				{
					bool flag = true;
					for (int l = 0; l < 4; l++)
					{
						if (array4[l] && array6[l])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						int num9 = -1;
						for (int m = 0; m < 4; m++)
						{
							if (array5[m] == 10f || (array5[m] > array9[m] && m == num7))
							{
								num9 = m;
								break;
							}
						}
						if (level >= 3)
						{
							if (!IsDeadEnd(num, num2, array[num9], array2[num9]))
							{
								runAwayFrom = new Vector3(num, 0f, num2);
								switch (num9)
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
								if (bombMaxN == 1 || bombMaxN - Global.Map.CountBomb(this) > 1)
								{
									DropBomb();
								}
							}
						}
						else
						{
							int num10 = num5 - num;
							int num11 = num6 - num2;
							int num12 = num5 - array[num9];
							int num13 = num6 - array2[num9];
							float num14 = num10 * num10 + num11 * num11;
							float num15 = num12 * num12 + num13 * num13;
							if (num14 < num15)
							{
								if (bombMaxN == 1 || bombMaxN - Global.Map.CountBomb(this) > 1)
								{
									DropBomb();
								}
								else
								{
									SnapTo(num, num2, p);
								}
							}
							else
							{
								runAwayFrom = new Vector3(num, 0f, num2);
								switch (num9)
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
						}
					}
					else
					{
						SnapTo(num, num2, p);
					}
				}
				else if (num8 > 1)
				{
					if (num7 >= 0 && num7 < 4)
					{
						bool[] array10 = new bool[4];
						bool flag2 = true;
						for (int n = 0; n < 4; n++)
						{
							array10[n] = !array6[n] && !IsDeadEnd(num, num2, array[n], array2[n]);
							if (array10[n])
							{
								flag2 = false;
							}
						}
						if (!flag2 && DropBomb())
						{
							if (Global.Map.CountBomb(this) < bombMaxN && !IsDeadEnd(num, num2, array[num7], array2[num7]))
							{
								switch (num7)
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
						}
						else
						{
							num7 = -1;
						}
					}
					else if (num7 == 4)
					{
						float num16 = -2f;
						int num17 = -1;
						for (int num18 = 0; num18 < 4; num18++)
						{
							if (array5[num18] > array9[num18] && !IsDeadEnd(num, num2, array[num18], array2[num18]) && (num16 < array9[num18] || (num16 == array9[num18] && Random.value < 0.2f)))
							{
								num16 = array9[num18];
								num17 = num18;
							}
						}
						switch (num17)
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
						case -1:
							num7 = -1;
							break;
						}
					}
					if (num7 == -1)
					{
						List<GameItem> list = GameItem.List();
						float num19 = 1000f;
						int num20 = -1;
						for (int num21 = list.Count - 1; num21 >= 0; num21--)
						{
							Vector3 localPosition2 = list[num21].transform.localPosition;
							if (localPosition2.y == 0f)
							{
								switch (list[num21].Type)
								{
								default:
								{
									int num22 = (int)localPosition2.x - num;
									int num23 = (int)localPosition2.z - num2;
									float num24 = num22 * num22 + num23 * num23;
									if (num19 > num24)
									{
										num19 = num24;
										num20 = num21;
									}
									break;
								}
								case Item.FireDown:
								case Item.BombDown:
								case Item.SpeedDown:
								case Item.Virus:
									break;
								}
							}
						}
						int num25 = Global.Map.CountBomb(this);
						float[] array11 = new float[4];
						int num26 = num5 - num;
						int num27 = num6 - num2;
						float num28 = num26 * num26 + num27 * num27;
						float num29 = num28;
						int num30 = -1;
						if (num20 != -1 && num19 < num29)
						{
							num29 = num19;
							Vector3 localPosition3 = list[num20].transform.localPosition;
							int num31 = (int)localPosition3.x;
							int num32 = (int)localPosition3.z;
							for (int num33 = 0; num33 < 4; num33++)
							{
								if ((runAwayFrom.y != 0f || runAwayFrom.x != (float)array[num33] || runAwayFrom.z != (float)array2[num33]) && array5[num33] > array9[num33] && (!IsDeadEnd(num, num2, array[num33], array2[num33]) || num25 < bombMaxN))
								{
									num26 = num31 - array[num33];
									num27 = num32 - array2[num33];
									array11[num33] = num26 * num26 + num27 * num27;
									if (num29 > array11[num33] || (num28 > array11[num33] && Random.value < 0.1f))
									{
										num29 = array11[num33];
										num30 = num33;
									}
								}
							}
							num7 = -1;
						}
						else
						{
							for (int num34 = 0; num34 < 4; num34++)
							{
								if ((runAwayFrom.y != 0f || runAwayFrom.x != (float)array[num34] || runAwayFrom.z != (float)array2[num34]) && array5[num34] > array9[num34] && (!IsDeadEnd(num, num2, array[num34], array2[num34]) || num25 < bombMaxN))
								{
									num26 = num5 - array[num34];
									num27 = num6 - array2[num34];
									array11[num34] = num26 * num26 + num27 * num27;
									if (num29 > array11[num34] || (num28 > array11[num34] && Random.value < 0.9f))
									{
										num29 = array11[num34];
										num30 = num34;
									}
								}
							}
						}
						if (num30 != -1)
						{
							if (num7 != -1 && num5 == array[num30] && num6 == array2[num30])
							{
								SnapTo(num, num2, p);
							}
							else if (Global.Map.CountBomb(this) < bombMaxN)
							{
								switch (num30)
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
								runAwayFrom.y = -1f;
							}
							else
							{
								SnapTo(num, num2, p);
							}
						}
						else
						{
							bool[] array12 = new bool[4];
							bool flag3 = true;
							for (int num35 = 0; num35 < 4; num35++)
							{
								array12[num35] = !array6[num35] && !IsDeadEnd(num, num2, array[num35], array2[num35]);
								if (array12[num35])
								{
									flag3 = false;
								}
							}
							if (flag3)
							{
								if (runAwayFrom.y == -1f)
								{
									runAwayFrom.y = -1f;
									float num36 = -2f;
									int num37 = -1;
									for (int num38 = 0; num38 < 4; num38++)
									{
										if (array5[num38] > array9[num38] && (num36 < array9[num38] || (num36 == array9[num38] && Random.value < 0.2f)))
										{
											num36 = array9[num38];
											num37 = num38;
										}
									}
									switch (num37)
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
							}
							else if (bombMaxN == 1 || bombMaxN - Global.Map.CountBomb(this) > 1)
							{
								DropBomb();
							}
							else
							{
								SnapTo(num, num2, p);
							}
						}
					}
				}
			}
			else
			{
				mode = Mode.Defend;
			}
		}
		if (mode != Mode.Defend)
		{
			return;
		}
		if (gameBomb == null)
		{
			if (SnapTo(num, num2, p))
			{
				mode = Mode.Offend;
			}
			return;
		}
		runAwayFrom.y = -1f;
		bool[] array13 = new bool[4];
		float[] array14 = new float[4];
		bool[] array15 = new bool[4];
		GameBomb[] array16 = new GameBomb[4];
		float[] array17 = new float[4];
		float[] array18 = new float[4];
		for (int num39 = 0; num39 < 4; num39++)
		{
			array13[num39] = CanStand(array[num39], array2[num39]);
			array16[num39] = ((!array13[num39]) ? null : IsSafeZone(array[num39], array2[num39], ref array17[num39]));
			array15[num39] = !array13[num39] || Global.Map.IsOnFire(array[num39], array2[num39]);
			array14[num39] = (array15[num39] ? (-1f) : ((!(array16[num39] == null)) ? array17[num39] : 10f));
			array18[num39] = GetMinFuse(array[num39], array2[num39], array16[num39]);
			if (array14[num39] != -1f && array14[num39] < timeFuse)
			{
				array14[num39] = -1f;
			}
		}
		bool[] array19 = new bool[4];
		bool flag4 = true;
		for (int num40 = 0; num40 < 4; num40++)
		{
			array19[num40] = !array15[num40] && !IsDeadEnd(num, num2, array[num40], array2[num40]);
			if (array19[num40])
			{
				flag4 = false;
			}
		}
		int num41 = 0;
		for (int num42 = 0; num42 < 4; num42++)
		{
			if (array14[num42] > array18[num42])
			{
				num41++;
			}
		}
		if (num7 >= 0 && num7 < 4)
		{
			int num43 = 0;
			for (int num44 = 0; num44 < 4; num44++)
			{
				if (array19[num44])
				{
					num43++;
				}
			}
			if (num43 > 0 && DropBomb())
			{
				return;
			}
		}
		switch (num41)
		{
		case 0:
			SnapTo(num, num2, p);
			break;
		default:
			if (!flag4)
			{
				int num45 = -1;
				float num46 = timeFuse;
				bool flag5 = timeFuse > GetMinFuse(num, num2, gameBomb);
				if (flag5)
				{
					bool flag6 = false;
					for (int num47 = 0; num47 < 4; num47++)
					{
						if (array19[num47] && num46 <= array14[num47])
						{
							flag6 = true;
							break;
						}
					}
					if (flag6)
					{
						do
						{
							for (int num48 = 0; num48 < 4; num48++)
							{
								if (array19[num48] && (num46 < array14[num48] || (num46 == array14[num48] && Random.value < 0.25f)))
								{
									num46 = array14[num48];
									num45 = num48;
								}
							}
						}
						while (num45 == -1);
					}
					if (array3[(num45 + 2) % 4] == gotoDir)
					{
						flag5 = false;
					}
				}
				if (!flag5 || num45 == -1)
				{
					bool flag7 = false;
					for (int num49 = 0; num49 < 4; num49++)
					{
						if (num46 < array14[num49] || (num46 == array14[num49] && array19[num49]))
						{
							flag7 = true;
							break;
						}
					}
					if (flag7)
					{
						do
						{
							for (int num50 = 0; num50 < 4; num50++)
							{
								if (num46 < array14[num50] || (num46 == array14[num50] && array19[num50] && Random.value < 0.1f))
								{
									num46 = array14[num50];
									num45 = num50;
								}
							}
						}
						while (num45 == -1);
					}
				}
				switch (num45)
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
				break;
			}
			goto case 1;
		case 1:
		{
			bool flag8 = true;
			for (int num51 = 0; num51 < 4; num51++)
			{
				if (array13[num51] && array15[num51])
				{
					flag8 = false;
					break;
				}
			}
			if (flag8)
			{
				if (array14[0] >= timeFuse && array19[0])
				{
					SmartMoveLeft(num, num2);
					break;
				}
				if (array14[1] >= timeFuse && array19[1])
				{
					SmartMoveDown(num, num2);
					break;
				}
				if (array14[2] >= timeFuse && array19[2])
				{
					SmartMoveRight(num, num2);
					break;
				}
				if (array14[3] >= timeFuse && array19[3])
				{
					SmartMoveUp(num, num2);
					break;
				}
				if (array14[0] > timeFuse)
				{
					SmartMoveLeft(num, num2);
					break;
				}
				if (array14[1] > timeFuse)
				{
					SmartMoveDown(num, num2);
					break;
				}
				if (array14[2] > timeFuse)
				{
					SmartMoveRight(num, num2);
					break;
				}
				if (array14[3] > timeFuse)
				{
					SmartMoveUp(num, num2);
					break;
				}
				Vector3 localPosition4 = gameBomb.transform.localPosition;
				int num52 = Mathf.RoundToInt(localPosition4.x);
				int num53 = Mathf.RoundToInt(localPosition4.z);
				if (num52 != num || num53 != num2)
				{
					if (num53 == num2)
					{
						if (num52 > num && !array15[0])
						{
							SmartMoveLeft(num, num2);
						}
						else if (num52 < num && !array15[2])
						{
							SmartMoveRight(num, num2);
						}
					}
					else if (num53 > num2 && !array15[1])
					{
						SmartMoveDown(num, num2);
					}
					else if (num53 < num2 && !array15[3])
					{
						SmartMoveUp(num, num2);
					}
				}
				else if (flag4)
				{
					if (array14[0] != -1f)
					{
						SmartMoveLeft(num, num2);
					}
					else if (array14[1] != -1f)
					{
						SmartMoveDown(num, num2);
					}
					else if (array14[2] != -1f)
					{
						SmartMoveRight(num, num2);
					}
					else if (array14[3] != -1f)
					{
						SmartMoveUp(num, num2);
					}
				}
				else if (array14[0] != -1f && array19[0])
				{
					SmartMoveLeft(num, num2);
				}
				else if (array14[1] != -1f && array19[1])
				{
					SmartMoveDown(num, num2);
				}
				else if (array14[2] != -1f && array19[2])
				{
					SmartMoveRight(num, num2);
				}
				else if (array14[3] != -1f && array19[3])
				{
					SmartMoveUp(num, num2);
				}
				else
				{
					SnapTo(num, num2, p);
				}
			}
			else
			{
				SnapTo(num, num2, p);
			}
			break;
		}
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

	private GameBomb IsSafeZone(int x, int y, ref float timeFuse, int stackCount = 0, GameBomb skip = null)
	{
		List<GameBomb> list = GameBomb.List();
		if (stackCount > 2)
		{
			return null;
		}
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
				GameBomb gameBomb2 = IsSafeZone(num3, num4, ref timeFuse2, stackCount + 1, list[num2]);
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

	private bool CanStand(int x, int y)
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
				if (objects[num].name.StartsWith("Bomb") && !HasItem(Item.BombPass))
				{
					return false;
				}
			}
		}
		List<GameGround> list = GameGround.List();
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if (list[num2].Type == Ground.TimeOut)
			{
				Vector3 localPosition = list[num2].transform.localPosition;
				if (localPosition.x == (float)x && localPosition.z == (float)y)
				{
					return false;
				}
			}
		}
		if (Global.Level == 2)
		{
			for (int num3 = list.Count - 1; num3 >= 0; num3--)
			{
				if (list[num3].Type == Ground.Crack && list[num3].GetComponent<Collider>() != null)
				{
					Vector3 localPosition2 = list[num3].transform.localPosition;
					if (localPosition2.x == (float)x && localPosition2.z == (float)y)
					{
						return false;
					}
				}
			}
		}
		else if (Global.Level == 4)
		{
			for (int num4 = list.Count - 1; num4 >= 0; num4--)
			{
				if (list[num4].Type == Ground.Explode)
				{
					Vector3 localPosition3 = list[num4].transform.localPosition;
					if (localPosition3.x == (float)x && localPosition3.z == (float)y)
					{
						return list[num4].transform.Find("Volcano").GetComponent<Renderer>().material.GetTextureOffset("_MainTex").y < 0.5f && list[num4].GetLifeTime() < 6f;
					}
				}
			}
		}
		else if (Global.Level == 5)
		{
			for (int num5 = list.Count - 1; num5 >= 0; num5--)
			{
				if (list[num5].Type == Ground.Pull)
				{
					Vector3 localPosition4 = list[num5].transform.localPosition;
					float num6 = list[num5].transform.localScale.x / 20f;
					float num7 = localPosition4.x - (float)x;
					float num8 = localPosition4.z - (float)y;
					if (num7 * num7 + num8 * num8 < num6 * num6)
					{
						return false;
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
