using TNet;
using UnityEngine;

public class GameRemoteController : MonoBehaviour, GameController
{
	public static bool IgnorePlayerAction;

	private List<bool[]> cache;

	private bool isPress;

	public int id = -1;

	private Vector3 getPointerPosition;

	private float getHorizontal;

	private float getVertical;

	private bool doBombDrop;

	private bool doBombPick;

	private bool doBombRemote;

	private bool doSelectMenu;

	private bool doStartMenu;

	private bool doMenuButton;

	private bool doCheatButton;

	private bool doEnterPress;

	private bool doEnterHold;

	private bool doEnterRelease;

	private bool lastSend;

	private bool willSend;

	public bool IsConnected()
	{
		return id != -1;
	}

	public Vector3 GetPointerPosition()
	{
		return getPointerPosition;
	}

	public float GetHorizontal()
	{
		return getHorizontal;
	}

	public float GetVertical()
	{
		return getVertical;
	}

	public bool DoBombDrop()
	{
		return doBombDrop;
	}

	public bool DoBombPick()
	{
		return doBombPick;
	}

	public bool DoBombRemote()
	{
		return doBombRemote;
	}

	public bool ReleaseBombRemote()
	{
		return false;
	}

	public bool DoSelectMenu()
	{
		return doSelectMenu;
	}

	public bool DoStartMenu()
	{
		return doStartMenu;
	}

	public bool DoActiveButton()
	{
		return false;
	}

	public bool DoMenuButton()
	{
		return doMenuButton;
	}

	public bool DoCheatButton()
	{
		return doCheatButton;
	}

	public bool DoEnterPress(string name)
	{
		return doEnterPress;
	}

	public bool DoEnterHold(string name)
	{
		return doEnterHold;
	}

	public bool DoEnterRelease(string name)
	{
		return doEnterRelease;
	}

	public bool DoPadPress(int id)
	{
		if (!Input.GetMouseButton(0))
		{
			isPress = true;
		}
		JoyLayout joyLayout = GameSetting.presetContainer[GameSetting.playerLayout[id, 1]];
		if ((joyLayout.gameKey[4, 0] != KeyCode.None || joyLayout.gameKey[4, 1] != KeyCode.None) && (Input.GetKey(joyLayout.gameKey[4, 0]) || Input.GetKey(joyLayout.gameKey[4, 1])))
		{
			isPress = false;
		}
		return false;
	}

	public bool DoPadRelease(int id)
	{
		if (isPress)
		{
			isPress = false;
		}
		return false;
	}

	public bool IsMouseMove()
	{
		float num = 0f;
		num = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
		if (num > 0.5f || num < -0.5f)
		{
			return true;
		}
		return false;
	}

	public void Reset()
	{
		cache.Clear();
	}

	private void Awake()
	{
		cache = new List<bool[]>();
	}

	private void Update()
	{
		if (id == Global.onlinePlayerID)
		{
			getPointerPosition = Input.mousePosition;
			getHorizontal = 0f;
			getVertical = 0f;
			doBombDrop = false;
			doBombPick = false;
			doBombRemote = false;
			doSelectMenu = false;
			doStartMenu = false;
			doMenuButton = false;
			doCheatButton = false;
			doEnterPress = false;
			doEnterHold = false;
			doEnterRelease = false;
			for (int i = 0; i < 5; i++)
			{
				getHorizontal += ProbeAxis(i, 2, 3);
				getVertical += ProbeAxis(i, 0, 1);
				doBombDrop |= ProbeButton(i, 4, 0);
				doBombPick |= ProbeButton(i, 5, 0);
				doBombRemote |= ProbeButton(i, 6, 0);
				if (i == 0)
				{
					doSelectMenu |= Input.GetKeyDown(KeyCode.Escape);
				}
				doSelectMenu |= ProbeButton(i, 7, 0);
				if (i == 0)
				{
					doStartMenu |= Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
				}
				else
				{
					doStartMenu |= Input.GetKeyDown("joystick " + i + " button 6");
				}
				if (i == 0)
				{
					doMenuButton |= Input.GetButtonDown("Fire1");
				}
				else
				{
					doMenuButton |= Input.GetKeyDown("joystick " + i + " button 0");
				}
				if (i == 0)
				{
					doCheatButton |= Input.GetKeyDown(KeyCode.F1);
				}
				else
				{
					doCheatButton |= Input.GetKeyDown("joystick " + i + " button 14");
				}
				if (i == 0)
				{
					doEnterPress |= Input.GetMouseButtonDown(0);
				}
				doEnterPress |= ProbeButton(i, 4, 0);
				if (i == 0)
				{
					doEnterHold |= Input.GetMouseButton(0);
				}
				doEnterHold |= ProbeButton(i, 4, 1);
				if (i == 0)
				{
					doEnterRelease |= Input.GetMouseButtonUp(0);
				}
				doEnterRelease |= ProbeButton(i, 4, 2);
			}
			if (getHorizontal < -1f)
			{
				getHorizontal = -1f;
			}
			else if (getHorizontal > 1f)
			{
				getHorizontal = 1f;
			}
			if (getVertical < -1f)
			{
				getVertical = -1f;
			}
			else if (getVertical > 1f)
			{
				getVertical = 1f;
			}
			float num = getHorizontal;
			float num2 = getVertical;
			if (IgnorePlayerAction)
			{
				getHorizontal = 0f;
				getVertical = 0f;
				doBombDrop = false;
				doBombPick = false;
				doBombRemote = false;
			}
			GetComponent<TNObject>().SendQuickly(90, Target.Others, getPointerPosition, getHorizontal, getVertical);
			lastSend = willSend;
			willSend = doBombDrop || doBombPick || doBombRemote;
			if (lastSend || willSend)
			{
				bool[] array = new bool[3] { doBombDrop, doBombPick, doBombRemote };
				GetComponent<TNObject>().Send(91, Target.Others, array);
			}
			if (IgnorePlayerAction)
			{
				getHorizontal = num;
				getVertical = num2;
			}
			return;
		}
		doBombDrop = false;
		doBombPick = false;
		doBombRemote = false;
		doSelectMenu = false;
		doStartMenu = false;
		doMenuButton = false;
		doCheatButton = false;
		doEnterPress = false;
		doEnterRelease = false;
		while (cache.Count > 0)
		{
			bool[] array2 = cache[0];
			bool flag = false;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j])
				{
					flag = true;
					break;
				}
			}
			cache.RemoveAt(0);
			if (flag)
			{
				int num3 = 0;
				doBombDrop = array2[num3++];
				doBombPick = array2[num3++];
				doBombRemote = array2[num3++];
				break;
			}
		}
	}

	private float ProbeAxis(int joy, int na, int pa)
	{
		float num = 0f;
		float num2 = 0f;
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[joy, 1], out value) || GameSetting.playerLayout[joy, 0] == 1)
		{
			value = GameSetting.defaultLayout[joy];
		}
		if (SpecialKey(value.game[na, 0], 2) || SpecialKey(value.game[na, 1], 2))
		{
			return -1f;
		}
		if (SpecialKey(value.game[pa, 0], 2) || SpecialKey(value.game[pa, 1], 2))
		{
			return 1f;
		}
		if (value.gameKey[na, 0] != KeyCode.None || value.gameKey[na, 1] != KeyCode.None)
		{
			if (Input.GetKey(value.gameKey[na, 0]) || Input.GetKey(value.gameKey[na, 1]))
			{
				return -1f;
			}
			if ((value.gameKey[pa, 0] != KeyCode.None || value.gameKey[pa, 1] != KeyCode.None) && (Input.GetKey(value.gameKey[pa, 0]) || Input.GetKey(value.gameKey[pa, 1])))
			{
				return 1f;
			}
		}
		if (value.game[na, 0].Contains("Axis"))
		{
			num = Input.GetAxis(value.game[na, 0].Substring(1));
			if (value.game[na, 0].IndexOf('-') != -1 && num < -0.2f)
			{
				return num;
			}
			if (value.game[na, 0].IndexOf('+') != -1 && num > 0.2f)
			{
				return 0f - num;
			}
		}
		if (value.game[na, 1].Contains("Axis"))
		{
			num2 = Input.GetAxis(value.game[na, 1].Substring(1));
			if (value.game[na, 1].IndexOf('-') != -1 && num2 < -0.2f)
			{
				return num2;
			}
			if (value.game[na, 1].IndexOf('+') != -1 && num2 > 0.2f)
			{
				return 0f - num2;
			}
		}
		if (value.game[pa, 0].Contains("Axis"))
		{
			num = Input.GetAxis(value.game[pa, 0].Substring(1));
			if (value.game[pa, 0].IndexOf('-') != -1 && num < -0.2f)
			{
				return 0f - num;
			}
			if (value.game[pa, 0].IndexOf('+') != -1 && num > 0.2f)
			{
				return num;
			}
		}
		if (value.game[pa, 1].Contains("Axis"))
		{
			num2 = Input.GetAxis(value.game[pa, 1].Substring(1));
			if (value.game[pa, 1].IndexOf('-') != -1 && num2 < -0.2f)
			{
				return 0f - num2;
			}
			if (value.game[pa, 1].IndexOf('+') != -1 && num2 > 0.2f)
			{
				return num2;
			}
		}
		return 0f;
	}

	private bool ProbeButton(int joy, int button, int mode)
	{
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[joy, 1], out value) || GameSetting.playerLayout[joy, 0] == 1)
		{
			value = GameSetting.defaultLayout[joy];
		}
		switch (mode)
		{
		case 0:
			if (SpecialKey(value.game[button, 0], 1) || SpecialKey(value.game[button, 1], 1))
			{
				return true;
			}
			if ((value.gameKey[button, 0] != KeyCode.None || value.gameKey[button, 1] != KeyCode.None) && (Input.GetKeyDown(value.gameKey[button, 0]) || Input.GetKeyDown(value.gameKey[button, 1])))
			{
				return true;
			}
			break;
		case 1:
			if (SpecialKey(value.game[button, 0], 2) || SpecialKey(value.game[button, 1], 2))
			{
				return true;
			}
			if ((value.gameKey[button, 0] != KeyCode.None || value.gameKey[button, 1] != KeyCode.None) && (Input.GetKey(value.gameKey[button, 0]) || Input.GetKey(value.gameKey[button, 1])))
			{
				return true;
			}
			break;
		case 2:
			if (SpecialKey(value.game[button, 0], 3) || SpecialKey(value.game[button, 1], 3))
			{
				return true;
			}
			if ((value.gameKey[button, 0] != KeyCode.None || value.gameKey[button, 1] != KeyCode.None) && (Input.GetKeyUp(value.gameKey[button, 0]) || Input.GetKeyUp(value.gameKey[button, 1])))
			{
				return true;
			}
			break;
		}
		if (!isPress && mode == 2)
		{
			return false;
		}
		if (value.game[button, 0].Contains("Axis"))
		{
			float axis = Input.GetAxis(value.game[button, 0].Substring(1));
			if (mode < 2)
			{
				if (value.game[button, 0].IndexOf('-') != -1 && axis < -0.5f)
				{
					return true;
				}
				if (value.game[button, 0].IndexOf('+') != -1 && axis > 0.5f)
				{
					return true;
				}
			}
			else if (mode == 2)
			{
				if (value.game[button, 0].IndexOf('-') != -1 && axis > -0.5f)
				{
					return true;
				}
				if (value.game[button, 0].IndexOf('+') != -1 && axis < 0.5f)
				{
					return true;
				}
			}
		}
		if (value.game[button, 1].Contains("Axis"))
		{
			float axis2 = Input.GetAxis(value.game[button, 1].Substring(1));
			if (mode < 2)
			{
				if (value.game[button, 1].IndexOf('-') != -1 && axis2 < -0.5f)
				{
					return true;
				}
				if (value.game[button, 1].IndexOf('+') != -1 && axis2 > 0.5f)
				{
					return true;
				}
			}
			else if (mode == 2)
			{
				if (value.game[button, 1].IndexOf('-') != -1 && axis2 > -0.5f)
				{
					return true;
				}
				if (value.game[button, 1].IndexOf('+') != -1 && axis2 < 0.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool SpecialKey(string key, int mode)
	{
		switch (mode)
		{
		case 1:
			switch (key)
			{
			case "Control":
				if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))
				{
					return true;
				}
				break;
			case "Alt":
				if (Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyDown(KeyCode.LeftAlt))
				{
					return true;
				}
				break;
			case "Shift":
				if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
				{
					return true;
				}
				break;
			default:
				return false;
			}
			break;
		case 2:
			switch (key)
			{
			case "Control":
				if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
				{
					return true;
				}
				break;
			case "Alt":
				if (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt))
				{
					return true;
				}
				break;
			case "Shift":
				if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
				{
					return true;
				}
				break;
			default:
				return false;
			}
			break;
		case 3:
			switch (key)
			{
			case "Control":
				if (Input.GetKeyUp(KeyCode.RightControl) || Input.GetKeyUp(KeyCode.LeftControl))
				{
					return true;
				}
				break;
			case "Alt":
				if (Input.GetKeyUp(KeyCode.RightAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
				{
					return true;
				}
				break;
			case "Shift":
				if (Input.GetKeyUp(KeyCode.RightShift) || Input.GetKeyUp(KeyCode.LeftShift))
				{
					return true;
				}
				break;
			default:
				return false;
			}
			break;
		}
		return false;
	}

	[RFC(90)]
	private void GameRemoteControllerUpdateMovement(Vector3 pointerPosition, float horizontal, float vertical)
	{
		getPointerPosition = pointerPosition;
		getHorizontal = horizontal;
		getVertical = vertical;
	}

	[RFC(91)]
	private void GameRemoteControllerUpdateAction(bool[] data)
	{
		cache.Add(data);
	}
}
