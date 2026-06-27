using ManagedSteam;
using ManagedSteam.CallbackStructures;
using ManagedSteam.SteamTypes;
using UnityEngine;

public class Stats : MonoBehaviour
{
	private void Start()
	{
		if (Steamworks.SteamInterface == null)
		{
			Debug.LogError("SteamInterface startup failed!");
			return;
		}
		IStats stats = Steamworks.SteamInterface.Stats;
		stats.UserStatsReceived += UserStatsReceived;
		stats.UserStatsStored += UserStatsStored;
		stats.RequestCurrentStats();
	}

	private void UserStatsReceived(UserStatsReceived value)
	{
		Debug.Log("value.GameID: " + value.GameID.ToString() + "; Steamworks.SteamInterface.AppID: " + Steamworks.SteamInterface.AppID.ToString());
		Debug.Log("value.Result: " + value.Result);
		if (!(value.GameID == new GameID(Steamworks.SteamInterface.AppID.AsUInt64)))
		{
			return;
		}
		if (value.Result != Result.OK)
		{
			Debug.LogError("Failed to download stats.");
			return;
		}
		IStats stats = Steamworks.SteamInterface.Stats;
		int data;
		if (!stats.GetStat("TestStatInt", out data))
		{
			Debug.LogWarning("Failed to read TestStatInt");
		}
		float data2;
		if (!stats.GetStat("TestStatFloat", out data2))
		{
			Debug.LogWarning("Failed to read TestStatFloat");
		}
		Debug.Log("TestStatInt = " + data);
		Debug.Log("TestStatFloat = " + data2);
		data++;
		data2 += 0.5f;
		if (!stats.SetStat("TestStatInt", data))
		{
			Debug.LogWarning("Failed to write TestStatInt");
		}
		if (!stats.SetStat("TestStatFloat", data2))
		{
			Debug.LogWarning("Failed to write TestStatFloat");
		}
		stats.StoreStats();
	}

	private void UserStatsStored(UserStatsStored value)
	{
		if (value.GameID == new GameID(Steamworks.SteamInterface.AppID.AsUInt64))
		{
			if (value.Result == Result.OK)
			{
				Debug.Log("Stats saved to the server successfully.");
			}
			else
			{
				Debug.LogWarning("Failed to save stats to the server.");
			}
		}
	}
}
