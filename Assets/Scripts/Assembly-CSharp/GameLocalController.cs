using UnityEngine;

public class GameLocalController : GameController
{
	private int id;

	private bool isPress;

	public GameLocalController(int id)
	{
		this.id = id;
	}

	public bool IsConnected()
	{
		return id >= 0 && id <= 4;
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

	private bool ProbeButton(int joy, int button)
	{
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[joy, 1], out value) || GameSetting.playerLayout[joy, 0] == 1)
		{
			value = GameSetting.defaultLayout[joy];
		}
		if (SpecialKey(value.game[button, 0], 1) || SpecialKey(value.game[button, 1], 1))
		{
			return true;
		}
		if ((value.gameKey[button, 0] != KeyCode.None || value.gameKey[button, 1] != KeyCode.None) && (Input.GetKeyDown(value.gameKey[button, 0]) || Input.GetKeyDown(value.gameKey[button, 1])))
		{
			return true;
		}
		if (value.game[button, 0].Contains("Axis"))
		{
			float axis = Input.GetAxis(value.game[button, 0].Substring(1));
			if (value.game[button, 0].IndexOf('-') != -1 && axis < -0.5f)
			{
				return true;
			}
			if (value.game[button, 0].IndexOf('+') != -1 && axis > 0.5f)
			{
				return true;
			}
		}
		if (value.game[button, 1].Contains("Axis"))
		{
			float axis2 = Input.GetAxis(value.game[button, 1].Substring(1));
			if (value.game[button, 1].IndexOf('-') != -1 && axis2 < -0.5f)
			{
				return true;
			}
			if (value.game[button, 1].IndexOf('+') != -1 && axis2 > 0.5f)
			{
				return true;
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

	public float GetHorizontal()
	{
		return ProbeAxis(id, 2, 3);
	}

	public float GetVertical()
	{
		return ProbeAxis(id, 0, 1);
	}

	public bool DoBombDrop()
	{
		return ProbeButton(id, 4);
	}

	public bool DoBombPick()
	{
		return ProbeButton(id, 5);
	}

	public bool DoBombRemote()
	{
		return ProbeButton(id, 6);
	}

	public bool ReleaseBombRemote()
	{
		if (id == 4 && Input.GetKeyUp(KeyCode.Escape))
		{
			Debug.Log("release remote bomb : " + id);
			return true;
		}
		return false;
	}

	public bool DoSelectMenu()
	{
		if (id == 0 && Input.GetKeyDown(KeyCode.Escape))
		{
			return true;
		}
		return ProbeButton(id, 7);
	}

	public bool DoStartMenu()
	{
		return false;
	}

	public bool DoActiveButton()
	{
		return Input.GetKeyDown(KeyCode.A);
	}

	public bool DoMenuButton()
	{
		if (id == 0 && Input.GetButtonDown("Fire1"))
		{
			return true;
		}
		return ProbeButton(id, 4);
	}

	public bool DoCheatButton()
	{
		if (id == 0)
		{
			return Input.GetKeyDown(KeyCode.F1);
		}
		if (Input.GetKeyDown("joystick " + id + " button 9"))
		{
			return true;
		}
		return false;
	}

	public Vector3 GetPointerPosition()
	{
		return Vector3.zero;
	}

	public bool DoEnterPress(string name)
	{
		if (id == 0 && Input.GetMouseButtonDown(0))
		{
			return true;
		}
		if (name.Equals("mouse"))
		{
			return false;
		}
		if (ProbeButton(id, 4) && !isPress)
		{
			return true;
		}
		return false;
	}

	public bool DoEnterHold(string name)
	{
		if (id == 0 && Input.GetMouseButton(0))
		{
			return true;
		}
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[id, 1], out value) || GameSetting.playerLayout[id, 0] == 1)
		{
			value = GameSetting.defaultLayout[id];
		}
		if (SpecialKey(value.game[4, 0], 2) || SpecialKey(value.game[4, 1], 2))
		{
			return true;
		}
		if ((value.gameKey[4, 0] != KeyCode.None || value.gameKey[4, 1] != KeyCode.None) && (Input.GetKey(value.gameKey[4, 0]) || Input.GetKey(value.gameKey[4, 1])))
		{
			return true;
		}
		if (value.game[4, 0].Contains("Axis"))
		{
			float axis = Input.GetAxis(value.game[4, 0].Substring(1));
			if (value.game[4, 0].IndexOf('-') != -1 && axis < -0.5f)
			{
				return true;
			}
			if (value.game[4, 0].IndexOf('+') != -1 && axis > 0.5f)
			{
				return true;
			}
		}
		if (value.game[4, 1].Contains("Axis"))
		{
			float axis2 = Input.GetAxis(value.game[4, 1].Substring(1));
			if (value.game[4, 1].IndexOf('-') != -1 && axis2 < -0.5f)
			{
				return true;
			}
			if (value.game[4, 1].IndexOf('+') != -1 && axis2 > 0.5f)
			{
				return true;
			}
		}
		return false;
	}

	public bool DoEnterRelease(string name)
	{
		if (id == 0 && Input.GetMouseButtonUp(0))
		{
			return true;
		}
		JoyLayout value;
		if (!GameSetting.presetContainer.TryGetValue(GameSetting.playerLayout[id, 1], out value) || GameSetting.playerLayout[id, 0] == 1)
		{
			value = GameSetting.defaultLayout[id];
		}
		if (SpecialKey(value.game[4, 0], 3) || SpecialKey(value.game[4, 1], 3))
		{
			return true;
		}
		if ((value.gameKey[4, 0] != KeyCode.None || value.gameKey[4, 1] != KeyCode.None) && (Input.GetKeyUp(value.gameKey[4, 0]) || Input.GetKeyUp(value.gameKey[4, 1])))
		{
			return true;
		}
		if (!isPress)
		{
			return false;
		}
		if (value.game[4, 0].Contains("Axis"))
		{
			float axis = Input.GetAxis(value.game[4, 0].Substring(1));
			if (value.game[4, 0].IndexOf('-') != -1 && axis > -0.5f)
			{
				return true;
			}
			if (value.game[4, 0].IndexOf('+') != -1 && axis < 0.5f)
			{
				return true;
			}
		}
		if (value.game[4, 1].Contains("Axis"))
		{
			float axis2 = Input.GetAxis(value.game[4, 1].Substring(1));
			if (value.game[4, 1].IndexOf('-') != -1 && axis2 > -0.5f)
			{
				return true;
			}
			if (value.game[4, 1].IndexOf('+') != -1 && axis2 < 0.5f)
			{
				return false;
			}
		}
		return false;
	}

	public bool DoPadPress(int jid)
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

	public bool DoPadRelease(int rid)
	{
		if (rid == -1)
		{
			return !isPress;
		}
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
}
