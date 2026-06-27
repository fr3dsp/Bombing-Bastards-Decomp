using System;
using System.Net.NetworkInformation;
using TNet;
using UnityEngine;

public class OnlineBattleMenu : MonoBehaviour
{
	public GUISkin skin;

	private bool mouseOver;

	private GameObject togButton;

	private GameObject player;

	private float pointerSpeed = Global.pointerSpd;

	private float pTime;

	private int pointerInit;

	private bool isPress;

	private TextMesh nameMesh;

	private string nameWord;

	private string nameWordBak;

	private bool nameGUI;

	private bool nameFocus;

	private TextMesh[] nameShadow;

	private Material namePen;

	private bool joyGetFocus;

	private Material fadeMat;

	private float fadeAlpha;

	private int fadeSign;

	private bool goBack;

	private WWW version;

	private TextMesh verText;

	private void Awake()
	{
		Transform transform = base.transform.Find("GUI");
		Transform transform2 = transform.Find("panel");
		player = transform2.Find("p_pointer").gameObject;
		fadeMat = transform2.Find("fade_bg").GetComponent<Renderer>().material;
		nameMesh = transform2.Find("field_user_name").GetComponent<TextMesh>();
		nameWord = Global.onlinePlayerName;
		nameWordBak = nameWord;
		nameGUI = nameWord == "Guest";
		nameFocus = nameGUI;
		nameShadow = nameMesh.GetComponentsInChildren<TextMesh>();
		namePen = nameMesh.transform.Find("ui_pen").GetComponent<Renderer>().material;
		for (int num = nameShadow.Length - 1; num >= 0; num--)
		{
			if (nameShadow[num] == nameMesh)
			{
				nameShadow[num] = null;
				break;
			}
		}
		nameMesh.text = nameWord;
		TextMesh[] array = nameShadow;
		foreach (TextMesh textMesh in array)
		{
			if (textMesh != null)
			{
				textMesh.text = nameMesh.text;
			}
		}
		nameMesh.gameObject.SetActive(!nameGUI);
		verText = transform2.Find("txt_info").GetComponent<TextMesh>();
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Online.ServerList");
		if (gameObject != null)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
		fadeAlpha = 1f;
		fadeSign = -1;
		string text = "windows";
		string macAddress = GetMacAddress();
		string text2 = "http://174.129.230.82/BombingBastards/version/version.php?";
		if (macAddress != null)
		{
			text2 = text2 + "mac=" + macAddress + "&";
		}
		text2 = text2 + "ver=" + Global.version + "&";
		string text3 = text2;
		text2 = text3 + "plt=" + text + "&t=" + Time.time;
		version = new WWW(text2);
	}

	private void playerInput()
	{
		bool flag = fadeAlpha > 0f;
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 position = player.transform.GetChild(0).position;
			RaycastHit hitInfo;
			if (!flag && ((Physics.Raycast(ray, out hitInfo) && pointerInit == 0) || Physics.Raycast(position, Vector3.forward, out hitInfo)))
			{
				if (hitInfo.transform.name == "bg")
				{
					if (togButton != null)
					{
						TextureUtility.SetSpriteIndex(togButton, 4, 1, (!togButton.GetComponent<Collider>().enabled) ? 3 : 0);
						togButton = null;
						mouseOver = false;
						if (controller.DoEnterPress("mouse"))
						{
							joyGetFocus = true;
						}
					}
				}
				else if (hitInfo.transform.name.StartsWith("field_"))
				{
					if (togButton != null)
					{
						TextureUtility.SetSpriteIndex(togButton, 4, 1, (!togButton.GetComponent<Collider>().enabled) ? 3 : 0);
						togButton = null;
						mouseOver = false;
					}
					if (controller.DoEnterPress("mouse"))
					{
						nameMesh.gameObject.SetActive(false);
						nameGUI = true;
						nameFocus = true;
					}
				}
				else
				{
					if (!hitInfo.transform.name.StartsWith("btn_"))
					{
						continue;
					}
					if (togButton != null && togButton != hitInfo.transform.gameObject)
					{
						mouseOver = false;
						TextureUtility.SetSpriteIndex(togButton, 4, 1, (!togButton.GetComponent<Collider>().enabled) ? 3 : 0);
					}
					togButton = hitInfo.transform.gameObject;
					if (!mouseOver)
					{
						mouseOver = true;
						if (!GameSound.IsPlayingSFX("mouseOver"))
						{
							GameSound.StartSFX("mouseOver");
						}
					}
					if (!isPress)
					{
						TextureUtility.SetSpriteIndex(togButton, 4, 1, togButton.GetComponent<Collider>().enabled ? 1 : 3);
					}
					if (controller.DoEnterHold("x"))
					{
						controller.DoPadPress(i);
						TextureUtility.SetSpriteIndex(togButton, 4, 1, (!togButton.GetComponent<Collider>().enabled) ? 3 : 2);
						isPress = true;
						joyGetFocus = true;
					}
					else if (controller.DoEnterRelease("x"))
					{
						controller.DoPadRelease(i);
						GameSound.StartSFX("menuSelect");
						Debug.Log("Do action " + togButton.name);
						if (togButton.name == "btn_back")
						{
							goBack = true;
							fadeSign = 1;
						}
						else if (togButton.name == "btn_lan")
						{
							Global.onlineWAN = false;
							goBack = false;
							fadeSign = 1;
						}
						else if (togButton.name == "btn_wan")
						{
							Global.onlineWAN = true;
							goBack = false;
							fadeSign = 1;
						}
						GameSave.Save();
					}
				}
			}
			else if (togButton != null)
			{
				TextureUtility.SetSpriteIndex(togButton, 4, 1, (!togButton.GetComponent<Collider>().enabled) ? 3 : 0);
				controller.DoPadRelease(i);
				isPress = false;
				togButton = null;
				mouseOver = false;
			}
			else
			{
				if (controller.DoEnterPress("x"))
				{
					joyGetFocus = true;
				}
				controller.DoPadRelease(i);
			}
		}
	}

	private void Update()
	{
		if (version == null)
		{
			if (verText != null)
			{
				Color color = verText.color;
				color.a -= Time.deltaTime;
				if (color.a < 0f)
				{
					color.a = 0f;
				}
				verText.color = color;
				if (color.a == 0f)
				{
					verText = null;
				}
			}
		}
		else if (version.isDone)
		{
			if (version.error == null || version.error.Length == 0)
			{
				int num = ParseVersion(version.text.Trim());
				int num2 = ParseVersion(Global.version);
				Debug.Log("minVer=" + num + " & curVer=" + num2);
				if (num2 >= num)
				{
					Transform transform = base.transform.Find("GUI/panel");
					GameObject gameObject = transform.Find("btn_lan").gameObject;
					GameObject gameObject2 = transform.Find("btn_wan").gameObject;
					TextureUtility.SetSpriteIndex(gameObject, 4, 1, 0);
					TextureUtility.SetSpriteIndex(gameObject2, 4, 1, 0);
					gameObject.GetComponent<Collider>().enabled = true;
					gameObject2.GetComponent<Collider>().enabled = true;
				}
				else
				{
					verText.color = new Color(1f, 0f, 0f, 0.75f);
					verText.text = "Your version of the game is outdated.\nPlease update.";
					verText = null;
				}
			}
			else
			{
				string text = "Error: " + version.error;
				int num3 = text.Length;
				if (num3 > 45)
				{
					int num4 = Mathf.CeilToInt((float)num3 / 45f);
					int num5 = Mathf.CeilToInt((float)num3 / (float)num4);
					int num6 = 0;
					int num7 = 0;
					char[] array = text.ToCharArray();
					for (int i = 0; i < array.Length; i++)
					{
						if (num6 >= num5 && char.IsWhiteSpace(array[i]))
						{
							if (num7 < num6)
							{
								num7 = num6;
							}
							array[i] = '\n';
							num6 = 0;
						}
						num6++;
					}
					num3 = num7;
					text = new string(array);
				}
				if (num3 > 30)
				{
					verText.characterSize = 1.5f - (float)(num3 - 30) * 0.02f;
				}
				verText.color = new Color(1f, 0f, 0f, 0.75f);
				verText.text = text;
				verText = null;
			}
			version = null;
		}
		else
		{
			verText.color = new Color(1f, 1f, 1f, 0.125f + Mathf.PingPong(Time.time, 0.75f));
		}
		namePen.SetColor("_TintColor", new Color(0.371f, 0.56799996f, 0.52f, 0.125f + Mathf.PingPong(Time.time, 0.25f)));
		Color color2 = ((!(nameShadow[0] == null)) ? nameShadow[0] : nameShadow[1]).color;
		color2.a = Mathf.PingPong(Time.time * 45f, 0.4f) + 0.35f;
		for (int num8 = nameShadow.Length - 1; num8 >= 0; num8--)
		{
			if (nameShadow[num8] != null)
			{
				nameShadow[num8].color = color2;
			}
		}
		if (fadeAlpha == 1f && fadeSign == 0)
		{
			if (goBack)
			{
				Loading.LoadScene("MainMenu");
				return;
			}
			UdpProtocol.useMulticasting = false;
			Application.LoadLevel("OnlineBattle.ServerList");
			return;
		}
		playerInput();
		pointerControl();
		if (fadeSign < 0)
		{
			fadeAlpha -= Time.deltaTime * 4f;
			if (fadeAlpha <= 0f)
			{
				fadeAlpha = 0f;
				fadeSign = 0;
			}
			fadeMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fadeAlpha / 2f));
		}
		else if (fadeSign > 0)
		{
			fadeAlpha += Time.deltaTime * 4f;
			if (fadeAlpha >= 1f)
			{
				fadeAlpha = 1f;
				fadeSign = 0;
			}
			fadeMat.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fadeAlpha / 2f));
		}
	}

	private void OnGUI()
	{
		float num = (float)Screen.height / 1080f;
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.Scale(new Vector3(num, num, 1f));
		GUISkin gUISkin = GUI.skin;
		GUI.skin = skin;
		bool flag = nameGUI;
		GUI.SetNextControlName("nameField");
		if (nameGUI)
		{
			nameWord = GUI.TextField(new Rect((float)Screen.width / num / 2f - 340f, 373f, 680f, 60f), nameWord, 8);
		}
		else
		{
			GUI.TextField(new Rect((float)Screen.width / num / 2f - 340f, 373f, -1f, 60f), string.Empty);
		}
		if (nameFocus)
		{
			nameFocus = false;
			nameWordBak = nameWord;
			GUI.FocusControl("nameField");
			return;
		}
		if (GUI.GetNameOfFocusedControl() == "nameField")
		{
			Event current = Event.current;
			if (current.type == EventType.KeyUp)
			{
				if (current.keyCode == KeyCode.Escape)
				{
					nameGUI = false;
					nameWord = nameWordBak;
				}
				else if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
				{
					nameGUI = false;
					nameWord = OnlineBattleServerList.ValidatePlayerName(nameWord);
				}
			}
		}
		else
		{
			nameGUI = false;
		}
		if ((!flag || nameGUI) && !joyGetFocus)
		{
			return;
		}
		GUI.FocusControl(null);
		if (nameWord.Length == 0)
		{
			nameMesh.text = "Guest";
		}
		else
		{
			nameMesh.text = OnlineBattleServerList.ValidatePlayerName(nameWord);
			nameWord = nameMesh.text;
		}
		TextMesh[] array = nameShadow;
		foreach (TextMesh textMesh in array)
		{
			if (textMesh != null)
			{
				textMesh.text = nameMesh.text;
			}
		}
		nameMesh.gameObject.SetActive(true);
		joyGetFocus = false;
		Global.onlinePlayerName = nameMesh.text;
	}

	private void pointerControl()
	{
		if (pointerInit == 1)
		{
			pTime += Time.deltaTime;
		}
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (controller.IsMouseMove())
			{
				pTime = 100f;
				Cursor.visible = true;
			}
			StickPointer(controller, i);
		}
	}

	private void StickPointer(GameController ctrl, int p)
	{
		if (pTime >= 10f)
		{
			pointerInit = 0;
			pTime = 0f;
			player.transform.localPosition = new Vector3(-500f, 0f, -100f);
		}
		for (int i = 0; i < 2; i++)
		{
			Vector3 localPosition = player.transform.localPosition;
			if (i == 1)
			{
				float num = ctrl.GetHorizontal() * 1.25f;
				if (pointerInit == 0 && num != 0f && localPosition.x < -181f)
				{
					player.transform.localPosition = new Vector3(0f, 0f, -100f);
					pointerInit = 1;
					Cursor.visible = false;
					break;
				}
				if (num < 0f)
				{
					if (localPosition.x >= -181f)
					{
						player.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
						pTime = 0f;
					}
				}
				else if (num > 0f && localPosition.x <= 195f)
				{
					player.transform.localPosition = new Vector3(localPosition.x + Time.deltaTime * pointerSpeed * num, localPosition.y, localPosition.z);
					pTime = 0f;
				}
				continue;
			}
			float num2 = ctrl.GetVertical() * 1.25f;
			if (pointerInit == 0 && num2 != 0f && localPosition.x < -181f)
			{
				player.transform.localPosition = new Vector3(0f, 0f, -100f);
				pointerInit = 1;
				Cursor.visible = false;
				break;
			}
			if (num2 < 0f)
			{
				if (localPosition.y <= 98f)
				{
					player.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
					pTime = 0f;
				}
			}
			else if (num2 > 0f && localPosition.y >= -115f)
			{
				player.transform.localPosition = new Vector3(localPosition.x, localPosition.y - Time.deltaTime * pointerSpeed * num2, localPosition.z);
				pTime = 0f;
			}
		}
	}

	private int ParseVersion(string version)
	{
		string[] array = version.Split('.');
		int num = 0;
		for (int i = 0; i < array.Length && i <= 2; i++)
		{
			int result;
			if (int.TryParse(array[i], out result))
			{
				num *= 100;
				num += result;
				continue;
			}
			return -1;
		}
		for (int j = array.Length; j < 3; j++)
		{
			num *= 100;
		}
		return num;
	}

	private string GetMacAddress()
	{
		try
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			if (allNetworkInterfaces == null)
			{
				return SystemInfo.deviceUniqueIdentifier;
			}
			NetworkInterface[] array = allNetworkInterfaces;
			foreach (NetworkInterface networkInterface in array)
			{
				NetworkInterfaceType networkInterfaceType = networkInterface.NetworkInterfaceType;
				if (networkInterfaceType != NetworkInterfaceType.Loopback && networkInterfaceType != NetworkInterfaceType.Tunnel)
				{
					return networkInterface.GetPhysicalAddress().ToString();
				}
			}
		}
		catch (Exception)
		{
		}
		return SystemInfo.deviceUniqueIdentifier;
	}
}
