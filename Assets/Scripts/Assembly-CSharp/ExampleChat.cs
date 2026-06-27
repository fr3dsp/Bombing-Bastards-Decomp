using TNet;
using UnityEngine;

[ExecuteInEditMode]
public class ExampleChat : TNBehaviour
{
	private struct ChatEntry
	{
		public string text;

		public Color color;
	}

	private Rect mRect;

	private string mName = "Guest";

	private string mInput = string.Empty;

	private List<ChatEntry> mChatEntries = new List<ChatEntry>();

	private void AddToChat(string text, Color color)
	{
		ChatEntry item = new ChatEntry
		{
			text = text,
			color = color
		};
		mChatEntries.Add(item);
	}

	private void OnNetworkJoinChannel(bool success, string error)
	{
		mName = TNManager.playerName;
		string text = "Players here: ";
		List<Player> players = TNManager.players;
		for (int i = 0; i < players.size; i++)
		{
			if (i > 0)
			{
				text += ", ";
			}
			text += players[i].name;
			if (players[i].id == TNManager.playerID)
			{
				text += " (you)";
			}
		}
		AddToChat(text, Color.black);
	}

	private void OnNetworkPlayerJoin(Player p)
	{
		AddToChat(p.name + " has joined the channel.", Color.black);
	}

	private void OnNetworkPlayerLeave(Player p)
	{
		AddToChat(p.name + " has left the channel.", Color.black);
	}

	private void OnNetworkPlayerRenamed(Player p, string previous)
	{
		AddToChat(previous + " is now known as " + p.name, Color.black);
	}

	[RFC]
	private void OnChat(int playerID, string text)
	{
		Player player = TNManager.GetPlayer(playerID);
		Color color = ((player.id != TNManager.playerID) ? Color.white : Color.green);
		AddToChat("[" + player.name + "]: " + text, color);
	}

	private void Send()
	{
		if (!string.IsNullOrEmpty(mInput))
		{
			base.tno.Send("OnChat", Target.All, TNManager.playerID, mInput);
			mInput = string.Empty;
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		GUI.Box(new Rect((float)Screen.width * 0.5f - 270f, (float)Screen.height * 0.5f - 200f, 540f, 410f), string.Empty);
		GUI.Label(new Rect(num - 140f, num2 - 170f, 80f, 24f), "Nickname");
		GUI.SetNextControlName("Nickname");
		mName = GUI.TextField(new Rect(num - 70f, num2 - 170f, 120f, 24f), mName);
		if (GUI.Button(new Rect(num + 60f, num2 - 170f, 80f, 24f), "Change"))
		{
			TNManager.playerName = mName;
		}
		GUI.SetNextControlName("Chat Window");
		mRect = new Rect(num - 200f, num2 - 120f, 400f, 300f);
		GUI.Window(0, mRect, OnGUIWindow, "Chat Window");
		if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)
		{
			string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
			if (nameOfFocusedControl == "Nickname")
			{
				TNManager.playerName = mName;
				GUI.FocusControl("Chat Window");
			}
			else if (nameOfFocusedControl == "Chat Input")
			{
				Send();
				GUI.FocusControl("Chat Window");
			}
			else
			{
				GUI.FocusControl("Chat Input");
			}
		}
	}

	private void OnGUIWindow(int id)
	{
		GUI.SetNextControlName("Chat Input");
		mInput = GUI.TextField(new Rect(6f, mRect.height - 30f, 328f, 24f), mInput);
		if (GUI.Button(new Rect(334f, mRect.height - 31f, 60f, 26f), "Send"))
		{
			Send();
			GUI.FocusControl("Chat Window");
		}
		GUI.BeginGroup(new Rect(2f, 20f, 382f, 254f));
		Rect position = new Rect(4f, 244f, 382f, 300f);
		int num = mChatEntries.size;
		while (num > 0)
		{
			ChatEntry chatEntry = mChatEntries[--num];
			position.y -= GUI.skin.label.CalcHeight(new GUIContent(chatEntry.text), 382f);
			GUI.color = chatEntry.color;
			GUI.Label(position, chatEntry.text, GUI.skin.label);
			if (position.y < 0f)
			{
				break;
			}
		}
		GUI.color = Color.white;
		GUI.EndGroup();
	}
}
