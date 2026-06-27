using UnityEngine;

public class GameInput
{
	public const int ControllerCount = 5;

	private static GameLocalController[] ctrls;

	private static GameRemoteController[] remotes;

	public static void Init()
	{
		if (ctrls == null)
		{
			ctrls = new GameLocalController[5];
			Debug.Log("platform: UNITY_STANDALONE_WIN");
			for (int i = 0; i < 5; i++)
			{
				ctrls[i] = new GameLocalController(i);
			}
		}
	}

	public static GameController GetController(int i = 0)
	{
		if (remotes != null && Global.Mode == GameMode.OnlineBattle)
		{
			if (i < 0 || i >= 8)
			{
				return null;
			}
			return remotes[i];
		}
		if (i < 0 || i >= 5)
		{
			return null;
		}
		return ctrls[i];
	}

	public static void InitRemoteSystem(int[] playerIDs)
	{
		GameRemoteController.IgnorePlayerAction = false;
		if (remotes == null)
		{
			remotes = new GameRemoteController[8];
			for (int i = 0; i < 8; i++)
			{
				GameObject gameObject = new GameObject("GameInput.Remote.Slot" + i);
				gameObject.AddComponent<TNObject>().uid = (uint)(20000 + i);
				remotes[i] = gameObject.AddComponent<GameRemoteController>();
				remotes[i].id = playerIDs[i];
				Object.DontDestroyOnLoad(gameObject);
			}
		}
		else
		{
			for (int j = 0; j < 8; j++)
			{
				remotes[j].id = playerIDs[j];
				remotes[j].Reset();
			}
		}
	}

	public static void DestroyRemoteSystem()
	{
		if (remotes != null)
		{
			for (int i = 0; i < 8; i++)
			{
				Object.DestroyImmediate(remotes[i].gameObject);
			}
			remotes = null;
		}
	}
}
