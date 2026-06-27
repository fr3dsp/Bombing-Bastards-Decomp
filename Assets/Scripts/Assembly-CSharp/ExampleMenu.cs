using TNet;
using UnityEngine;

[ExecuteInEditMode]
public class ExampleMenu : MonoBehaviour
{
	private const float buttonWidth = 150f;

	private const float buttonHeight = 40f;

	public int serverTcpPort = 5127;

	public string mainMenu = "Example Menu";

	public string[] examples;

	public GUIStyle button;

	public GUIStyle text;

	public GUIStyle textLeft;

	public GUIStyle input;

	private string mAddress = "127.0.0.1";

	private string mMessage = string.Empty;

	private float mAlpha;

	private void Start()
	{
		if (Application.isPlaying)
		{
			Tools.ResolveIPs(null);
			Screen.sleepTimeout = -1;
			TNManager.StartUDP(Random.Range(10000, 50000));
		}
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			float to = ((TNLobbyClient.knownServers.list.size != 0) ? 1f : 0f);
			mAlpha = UnityTools.SpringLerp(mAlpha, to, 8f, Time.deltaTime);
		}
	}

	private void OnGUI()
	{
		if (!TNManager.isConnected)
		{
			DrawConnectMenu();
		}
		else
		{
			if (!Application.isPlaying || Application.loadedLevelName == mainMenu)
			{
				DrawSelectionMenu();
			}
			else if (TNManager.isInChannel)
			{
				DrawExampleMenu();
			}
			DrawDisconnectButton();
		}
		DrawDebugInfo();
	}

	private void DrawConnectMenu()
	{
		Rect rect = new Rect((float)Screen.width * 0.5f - 100f - mAlpha * 120f, (float)Screen.height * 0.5f - 100f, 200f, 220f);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(UnityTools.PadRect(rect, 8f), string.Empty);
		GUI.color = Color.white;
		GUILayout.BeginArea(rect);
		GUILayout.Label("Server Address", text);
		mAddress = GUILayout.TextField(mAddress, input, GUILayout.Width(200f));
		if (GUILayout.Button("Connect", button))
		{
			TNManager.Connect(mAddress);
			mMessage = "Connecting...";
		}
		if (TNServerInstance.isActive)
		{
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Stop the Server", button))
			{
				TNServerInstance.Stop("server.dat");
				mMessage = "Server stopped";
			}
		}
		else
		{
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Start a Local Server", button))
			{
				int udpPort = Random.Range(10000, 40000);
				TNLobbyClient component = GetComponent<TNLobbyClient>();
				if (component == null)
				{
					TNServerInstance.Start(serverTcpPort, udpPort, "server.dat");
				}
				else
				{
					TNServerInstance.Type type = ((component is TNUdpLobbyClient) ? TNServerInstance.Type.Udp : TNServerInstance.Type.Tcp);
					TNServerInstance.Start(serverTcpPort, udpPort, component.remotePort, "server.dat", type);
				}
				mMessage = "Server started";
			}
		}
		GUI.backgroundColor = Color.white;
		if (!string.IsNullOrEmpty(mMessage))
		{
			GUILayout.Label(mMessage, text);
		}
		GUILayout.EndArea();
		if (mAlpha > 0.01f)
		{
			rect.x += ((float)Screen.width - rect.xMin - rect.xMax) * mAlpha;
			DrawServerList(rect);
		}
	}

	private void OnNetworkConnect(bool success, string message)
	{
		mMessage = message;
	}

	private void DrawSelectionMenu()
	{
		int num = examples.Length;
		Rect position = new Rect((float)Screen.width * 0.5f - 75f, (float)Screen.height * 0.5f - 20f * (float)num, 150f, 40f);
		for (int i = 0; i < num; i++)
		{
			string levelName = examples[i];
			if (GUI.Button(position, levelName, button))
			{
				TNManager.JoinChannel(i + 1, levelName);
			}
			position.y += 40f;
		}
		position.y += 20f;
	}

	private void DrawExampleMenu()
	{
		Rect position = new Rect(0f, (float)Screen.height - 40f, 150f, 40f);
		if (GUI.Button(position, "Main Menu", button))
		{
			TNManager.LeaveChannel();
		}
	}

	private void OnNetworkLeaveChannel()
	{
		Application.LoadLevel(mainMenu);
	}

	private void DrawDisconnectButton()
	{
		Rect position = new Rect((float)Screen.width - 150f, (float)Screen.height - 40f, 150f, 40f);
		if (GUI.Button(position, "Disconnect", button))
		{
			TNManager.Disconnect();
		}
	}

	private void DrawDebugInfo()
	{
		GUILayout.Label("LAN: " + Tools.localAddress.ToString(), textLeft);
		if (Application.isPlaying)
		{
			if (Tools.isExternalIPReliable)
			{
				GUILayout.Label("WAN: " + Tools.externalAddress, textLeft);
			}
			else
			{
				GUILayout.Label("WAN: Resolving...", textLeft);
			}
			if (TNManager.isConnected)
			{
				GUILayout.Label("Ping: " + TNManager.ping + " (" + ((!TNManager.canUseUDP) ? "TCP" : "TCP+UDP") + ")", textLeft);
			}
		}
	}

	private void DrawServerList(Rect rect)
	{
		GUI.color = new Color(1f, 1f, 1f, mAlpha * mAlpha * 0.5f);
		GUI.Box(UnityTools.PadRect(rect, 8f), string.Empty);
		GUI.color = new Color(1f, 1f, 1f, mAlpha * mAlpha);
		GUILayout.BeginArea(rect);
		GUILayout.Label("LAN Server List", text);
		List<ServerList.Entry> list = TNLobbyClient.knownServers.list;
		for (int i = 0; i < list.size; i++)
		{
			ServerList.Entry entry = list[i];
			if (GUILayout.Button(entry.internalAddress.ToString(), button))
			{
				TNManager.Connect(entry.internalAddress, entry.internalAddress);
				mMessage = "Connecting...";
			}
		}
		GUILayout.EndArea();
		GUI.color = Color.white;
	}
}
