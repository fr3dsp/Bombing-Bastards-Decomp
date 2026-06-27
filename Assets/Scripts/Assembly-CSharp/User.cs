using ManagedSteam;
using UnityEngine;

public class User : MonoBehaviour
{
	private IUser user;

	public static string Path { get; private set; }

	private void Start()
	{
		if (Steamworks.SteamInterface == null)
		{
			Debug.LogError("SteamInterface startup failed!");
			return;
		}
		user = Steamworks.SteamInterface.User;
		Path = user.GetUserDataFolder().Path;
	}

	private void Update()
	{
	}
}
