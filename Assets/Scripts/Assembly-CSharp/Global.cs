using UnityEngine;

public class Global
{
	public static float pointerSpd = 180f;

	public static int ctrlID = -1;

	public static bool isFirstStart;

	public static GameMap Map;

	public static GameMode Mode;

	public static bool isFocus;

	public static int advMaxP = 5;

	public static int advPlanet = 0;

	public static int advUnlock = 0;

	public static int advStage;

	public static bool advClear = true;

	public static float advRemainTime;

	public static int[] advStar = new int[31];

	public static CharSlot[] localPlayerSlot;

	public static string[] localPlayerName = new string[5] { "Player 1", "Player 2", "Player 3", "Player 4", "Player 5" };

	public static bool onlineWAN;

	public static int onlineMatchID;

	public static int onlinePlayerSlot = -1;

	public static int onlinePlayerID = -1;

	public static string onlinePlayerName = "Guest";

	public static int[] onlinePlayerIDs = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };

	public static string[] onlinePlayerNames = new string[8];

	public static bool[] onlinePlayerOns = new bool[8];

	public static int winAmount = 3;

	public static int timeAmount = 3;

	public static bool ghost = true;

	public static bool itemDestroy = true;

	public static bool mapShrink = true;

	public static bool virus = true;

	public static int winIndex = 2;

	public static int timeIndex = 2;

	public static int mapID = 1;

	public static int winID;

	public static int[] cupAmount = new int[8];

	private static bool sndOn = true;

	private static bool musOn = true;

	private static bool vovOn = true;

	private static bool itvOn = true;

	private static bool intOn = false;

	private static bool fullscreen = true;

	private static int resolution = 0;

	private static int quality = 2;

	public static KeyCode[] validKeyCodes;

	public static int tcpPort = 15127;

	public static int udpPort = 15128;

	public static int lobbyPort = 15129;

	public static string lobbyIP = "54.88.59.83";

	public static string version = "1.3.6";

	public static bool pirated = false;

	public static int Stage
	{
		get
		{
			return (advStage % 6 != 0) ? (advStage % 6) : 6;
		}
	}

	public static int UnlockStage
	{
		get
		{
			return ((advUnlock + 1) % 6 != 0) ? ((advUnlock + 1) % 6) : 6;
		}
	}

	public static int Level
	{
		get
		{
			return (Mode != GameMode.Adventure) ? mapID : ((advStage - 1) / 6 + 1);
		}
	}

	public static bool IsBossStage
	{
		get
		{
			return Mode == GameMode.Adventure && (advStage - 1) % 6 == 5;
		}
	}

	public static bool IsSoundOn
	{
		get
		{
			return sndOn;
		}
	}

	public static bool IsMusicOn
	{
		get
		{
			return musOn;
		}
	}

	public static bool IsVoiceOn
	{
		get
		{
			return vovOn;
		}
	}

	public static bool IsItemVOn
	{
		get
		{
			return itvOn;
		}
	}

	public static bool IsIntroOn
	{
		get
		{
			return intOn;
		}
	}

	public static bool FullScreen
	{
		get
		{
			return fullscreen;
		}
		set
		{
			fullscreen = value;
		}
	}

	public static int Resolution
	{
		get
		{
			return resolution;
		}
		set
		{
			resolution = value;
		}
	}

	public static int Quality
	{
		get
		{
			return quality;
		}
		set
		{
			quality = value;
		}
	}

	public static void ClearBattleData()
	{
		for (int i = 0; i < 8; i++)
		{
			cupAmount[i] = 0;
		}
	}

	public static void SetSound(bool on)
	{
		sndOn = on;
		GameSound.SetVolumeSFX(on ? 1 : 0);
	}

	public static void SetMusic(bool on)
	{
		musOn = on;
		GameSound.SetVolumeBGM(on ? 1 : 0);
	}

	public static void SetVoice(bool on)
	{
		vovOn = on;
	}

	public static void SetItemV(bool on)
	{
		itvOn = on;
	}

	public static void SetIntro(bool on)
	{
		intOn = on;
	}
}
