using UnityEngine;

public class SoundMusicControl : MonoBehaviour
{
	public static SoundMusicControl ctrl;

	private float vol;

	private void Awake()
	{
		ctrl = this;
	}

	private void OnDestroy()
	{
		ctrl = null;
	}

	private void Update()
	{
		if (!Global.isFocus || !Global.IsMusicOn)
		{
			return;
		}
		if (GameSound.IsPlayingVOV())
		{
			vol -= Time.deltaTime;
			if (vol < 0.5f)
			{
				vol = 0.5f;
			}
		}
		else
		{
			vol += Time.deltaTime;
			if (vol > 1f)
			{
				vol = 1f;
			}
		}
		GameSound.SetVolumeBGM(vol);
	}
}
