using UnityEngine;

public class GameSound : MonoBehaviour
{
	private static GameSound sound;

	public string bgmPath;

	public string sfxPath;

	public string vovPath;

	private static bool[] activeBG;

	private static bool[] enableBG;

	private static float[] subVolBG;

	private static float volumeBG;

	private static AudioSource[] bgm;

	private static AudioSource[] sfx;

	private static AudioSource[] vov;

	private static float volumeSFX = 1f;

	private static float volumeVOV = 1f;

	private static float lastDeltaTime;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		sound = this;
		Object[] array = Resources.LoadAll(bgmPath, typeof(AudioClip));
		Object[] array2 = Resources.LoadAll(sfxPath, typeof(AudioClip));
		Object[] array3 = Resources.LoadAll(vovPath, typeof(AudioClip));
		activeBG = new bool[array.Length];
		enableBG = new bool[array.Length];
		subVolBG = new float[array.Length];
		volumeBG = 1f;
		string[] array4 = new string[5] { "col_01", "cow_01", "hurry_01", "levelIntro_01", "tutorial_00.0" };
		bgm = new AudioSource[array.Length];
		sfx = new AudioSource[array2.Length];
		vov = new AudioSource[array3.Length + array4.Length];
		for (int i = 0; i < array.Length; i++)
		{
			bgm[i] = base.gameObject.AddComponent<AudioSource>();
			bgm[i].clip = array[i] as AudioClip;
			bgm[i].loop = true;
			bgm[i].playOnAwake = false;
		}
		for (int j = 0; j < array2.Length; j++)
		{
			sfx[j] = base.gameObject.AddComponent<AudioSource>();
			sfx[j].clip = array2[j] as AudioClip;
			sfx[j].loop = false;
			sfx[j].playOnAwake = false;
		}
		for (int k = 0; k < array3.Length; k++)
		{
			vov[k] = base.gameObject.AddComponent<AudioSource>();
			vov[k].clip = array3[k] as AudioClip;
			vov[k].loop = false;
			vov[k].playOnAwake = false;
		}
		for (int l = 0; l < 5; l++)
		{
			int num = array3.Length + l;
			vov[num] = base.gameObject.AddComponent<AudioSource>();
			vov[num].clip = Resources.Load(vovPath + "X/" + array4[l]) as AudioClip;
			vov[num].loop = false;
			vov[num].playOnAwake = false;
		}
	}

	private void OnDestroy()
	{
		if (sound == this)
		{
			sound = null;
		}
	}

	private void Update()
	{
		if (Time.timeScale == 1f)
		{
			lastDeltaTime = Mathf.Max(Time.deltaTime, 1f / 60f);
		}
		if (!Global.isFocus)
		{
			return;
		}
		for (int i = 0; i < bgm.Length; i++)
		{
			if (enableBG[i])
			{
				if (subVolBG[i] < 1f)
				{
					if (Time.timeScale == 1f)
					{
						subVolBG[i] += 1f;
					}
					else
					{
						subVolBG[i] += 1f;
					}
					if (subVolBG[i] > 1f)
					{
						subVolBG[i] = 1f;
					}
					bgm[i].volume = volumeBG * subVolBG[i];
					if (activeBG[i] && !bgm[i].isPlaying)
					{
						bgm[i].Play();
					}
				}
			}
			else
			{
				if (!(subVolBG[i] > 0f))
				{
					continue;
				}
				if (Time.timeScale == 1f)
				{
					subVolBG[i] -= Time.deltaTime;
				}
				else
				{
					subVolBG[i] -= lastDeltaTime;
				}
				if (subVolBG[i] < 0f)
				{
					subVolBG[i] = 0f;
				}
				bgm[i].volume = volumeBG * subVolBG[i];
				if (subVolBG[i] == 0f)
				{
					if (activeBG[i])
					{
						bgm[i].Pause();
					}
					else
					{
						bgm[i].Stop();
					}
				}
			}
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		Global.isFocus = focusStatus;
		if (focusStatus)
		{
			for (int i = 0; i < bgm.Length; i++)
			{
				if (enableBG[i] && bgm[i].isPlaying)
				{
					bgm[i].volume = volumeBG * subVolBG[i];
				}
			}
			AudioSource[] array = sfx;
			foreach (AudioSource audioSource in array)
			{
				audioSource.volume = volumeSFX;
			}
			AudioSource[] array2 = vov;
			foreach (AudioSource audioSource2 in array2)
			{
				audioSource2.volume = volumeVOV;
			}
			return;
		}
		for (int l = 0; l < bgm.Length; l++)
		{
			if (bgm[l].isPlaying)
			{
				bgm[l].volume = 0f;
			}
		}
		AudioSource[] array3 = sfx;
		foreach (AudioSource audioSource3 in array3)
		{
			audioSource3.volume = 0f;
		}
		AudioSource[] array4 = vov;
		foreach (AudioSource audioSource4 in array4)
		{
			audioSource4.volume = 0f;
		}
	}

	public static void StartBGM(string name)
	{
		if (sound == null)
		{
			return;
		}
		for (int i = 0; i < bgm.Length; i++)
		{
			if (bgm[i].clip.name == name)
			{
				activeBG[i] = true;
				enableBG[i] = true;
				break;
			}
		}
	}

	public static void StopBGM(string name = null, bool pause = false)
	{
		if (sound == null)
		{
			return;
		}
		if (name == null)
		{
			for (int i = 0; i < bgm.Length; i++)
			{
				activeBG[i] = false;
				enableBG[i] = false;
			}
			return;
		}
		for (int j = 0; j < bgm.Length; j++)
		{
			if (bgm[j].clip.name == name)
			{
				enableBG[j] = false;
				if (!pause)
				{
					activeBG[j] = false;
				}
				break;
			}
		}
	}

	public static void StartSFX(string name, bool loop = false)
	{
		if (!Global.isFocus || sound == null)
		{
			return;
		}
		AudioSource[] array = sfx;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name)
			{
				audioSource.loop = loop;
				audioSource.Play();
				break;
			}
		}
	}

	public static void StopSFX(string name = null)
	{
		if (sound == null)
		{
			return;
		}
		if (name == null)
		{
			AudioSource[] array = sfx;
			foreach (AudioSource audioSource in array)
			{
				audioSource.Stop();
			}
			return;
		}
		AudioSource[] array2 = sfx;
		foreach (AudioSource audioSource2 in array2)
		{
			if (audioSource2.clip.name == name)
			{
				audioSource2.Stop();
				break;
			}
		}
	}

	public static void StartVOV(string name)
	{
		if (sound == null)
		{
			return;
		}
		int num = 0;
		if (name.StartsWith("col_"))
		{
			num = -5;
		}
		else if (name.StartsWith("cow_"))
		{
			num = -4;
		}
		else if (name.StartsWith("hurry_"))
		{
			num = -3;
		}
		else if (name.StartsWith("levelIntro_"))
		{
			num = -2;
		}
		else if (name.StartsWith("tutorial_"))
		{
			num = -1;
		}
		if (num == 0)
		{
			AudioSource[] array = vov;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource.clip.name == name)
				{
					audioSource.Play();
					break;
				}
			}
		}
		else
		{
			num += vov.Length;
			if (vov[num].clip.name == name)
			{
				vov[num].Play();
				return;
			}
			vov[num].clip = Resources.Load(sound.vovPath + "X/" + name) as AudioClip;
			vov[num].Play();
		}
	}

	public static void StopVOV(string name = null)
	{
		if (sound == null)
		{
			return;
		}
		if (name == null)
		{
			AudioSource[] array = vov;
			foreach (AudioSource audioSource in array)
			{
				audioSource.Stop();
			}
			return;
		}
		AudioSource[] array2 = vov;
		foreach (AudioSource audioSource2 in array2)
		{
			if (audioSource2.clip.name == name)
			{
				audioSource2.Stop();
				break;
			}
		}
	}

	public static bool IsPlayingBGM(string name = null)
	{
		if (sound == null)
		{
			return false;
		}
		if (name == null)
		{
			AudioSource[] array = bgm;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource.isPlaying)
				{
					return true;
				}
			}
		}
		else
		{
			AudioSource[] array2 = bgm;
			foreach (AudioSource audioSource2 in array2)
			{
				if (audioSource2.clip.name == name && audioSource2.isPlaying)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsPlayingSFX(string name = null)
	{
		if (sound == null)
		{
			return false;
		}
		if (name == null)
		{
			AudioSource[] array = sfx;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource.isPlaying)
				{
					return true;
				}
			}
		}
		else
		{
			AudioSource[] array2 = sfx;
			foreach (AudioSource audioSource2 in array2)
			{
				if (audioSource2.clip.name == name && audioSource2.isPlaying)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsPlayingVOV(string name = null)
	{
		if (sound == null)
		{
			return false;
		}
		if (name == null)
		{
			AudioSource[] array = vov;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource.isPlaying)
				{
					return true;
				}
			}
		}
		else
		{
			AudioSource[] array2 = vov;
			foreach (AudioSource audioSource2 in array2)
			{
				if (audioSource2.clip.name == name && audioSource2.isPlaying)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string[] GetCurrentBGM()
	{
		if (sound == null)
		{
			return null;
		}
		int num = 0;
		for (int i = 0; i < bgm.Length; i++)
		{
			if (enableBG[i] && activeBG[i])
			{
				num++;
			}
		}
		string[] array = new string[num];
		for (int j = 0; j < bgm.Length; j++)
		{
			if (enableBG[j])
			{
				array[--num] = bgm[j].clip.name;
			}
		}
		return array;
	}

	public static string[] GetCurrentSFX()
	{
		if (sound == null)
		{
			return null;
		}
		int num = 0;
		AudioSource[] array = sfx;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.isPlaying)
			{
				num++;
			}
		}
		string[] array2 = new string[num];
		AudioSource[] array3 = sfx;
		foreach (AudioSource audioSource2 in array3)
		{
			if (audioSource2.isPlaying)
			{
				array2[--num] = audioSource2.clip.name;
			}
		}
		return array2;
	}

	public static string[] GetCurrentVOV()
	{
		if (sound == null)
		{
			return null;
		}
		int num = 0;
		AudioSource[] array = vov;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.isPlaying)
			{
				num++;
			}
		}
		string[] array2 = new string[num];
		AudioSource[] array3 = vov;
		foreach (AudioSource audioSource2 in array3)
		{
			if (audioSource2.isPlaying)
			{
				array2[--num] = audioSource2.clip.name;
			}
		}
		return array2;
	}

	public static void SetVolumeBGM(float vol)
	{
		volumeBG = vol;
		if (!(sound == null))
		{
			AudioSource[] array = bgm;
			foreach (AudioSource audioSource in array)
			{
				audioSource.volume = vol;
			}
		}
	}

	public static void SetVolumeSFX(float vol)
	{
		volumeSFX = vol;
		if (!(sound == null))
		{
			AudioSource[] array = sfx;
			foreach (AudioSource audioSource in array)
			{
				audioSource.volume = vol;
			}
		}
	}

	public static void SetVolumeVOV(float vol)
	{
		volumeVOV = vol;
		if (!(sound == null))
		{
			AudioSource[] array = vov;
			foreach (AudioSource audioSource in array)
			{
				audioSource.volume = vol;
			}
		}
	}

	public static float GetTimeBGM(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = bgm;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name && audioSource.isPlaying)
			{
				return audioSource.time;
			}
		}
		return 0f;
	}

	public static float GetTimeSFX(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = sfx;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name && audioSource.isPlaying)
			{
				return audioSource.time;
			}
		}
		return 0f;
	}

	public static float GetTimeVOV(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = vov;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name && audioSource.isPlaying)
			{
				return audioSource.time;
			}
		}
		return 0f;
	}

	public static float GetLengthBGM(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = bgm;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name)
			{
				return audioSource.clip.length;
			}
		}
		return 0f;
	}

	public static float GetLengthSFX(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = sfx;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name)
			{
				return audioSource.clip.length;
			}
		}
		return 0f;
	}

	public static float GetLengthVOV(string name)
	{
		if (sound == null)
		{
			return 0f;
		}
		AudioSource[] array = vov;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.clip.name == name)
			{
				return audioSource.clip.length;
			}
		}
		return 0f;
	}

	public static void PreloadVOV(string name, AudioClip clip = null)
	{
		int num = 0;
		if (name.StartsWith("col_"))
		{
			num = -5;
		}
		else if (name.StartsWith("cow_"))
		{
			num = -4;
		}
		else if (name.StartsWith("hurry_"))
		{
			num = -3;
		}
		else if (name.StartsWith("levelIntro_"))
		{
			num = -2;
		}
		else if (name.StartsWith("tutorial_"))
		{
			num = -1;
		}
		if (num == 0)
		{
			return;
		}
		num += vov.Length;
		if (vov[num].clip.name != name)
		{
			if (clip == null)
			{
				clip = Resources.Load(sound.vovPath + "X/" + name) as AudioClip;
			}
			vov[num].clip = clip;
		}
	}
}
