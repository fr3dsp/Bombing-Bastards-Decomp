using System;
using UnityEngine;

public class Init : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = 60;
		GameInput.Init();
		GameSound.SetVolumeBGM(1f);
		GameSound.StartBGM("Menu");
		if (GameObject.Find("Achievement") == null)
		{
			Resolution currentResolution = Screen.currentResolution;
			Screen.SetResolution(currentResolution.width, currentResolution.height, true);
		}
		Global.isFirstStart = true;
		GameSetting.Init();
		if (Global.validKeyCodes == null)
		{
			Global.validKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
		}
	}

	private void Start()
	{
		if (GameObject.Find("Achievement") == null)
		{
			Global.pirated = true;
			GameSave.Load();
		}
	}

	private void Update()
	{
		Application.LoadLevel("MainMenu");
	}
}
