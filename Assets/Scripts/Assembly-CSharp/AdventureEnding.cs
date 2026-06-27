using UnityEngine;

public class AdventureEnding : MonoBehaviour
{
	private Material eff;

	private Material eye;

	private float eyeClose;

	private Material mFade;

	private int iFade;

	private float fFade;

	private string vovName;

	private void Awake()
	{
		Transform transform = base.transform.Find("Ending");
		transform.GetComponent<Animation>()["idle"].speed = 0.8f;
		eff = transform.Find("title").GetComponent<Renderer>().material;
		eye = transform.Find("head").GetComponent<Renderer>().material;
		mFade = base.transform.Find("fade").GetComponent<Renderer>().material;
		iFade = -1;
		fFade = 0.5f;
		mFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fFade));
		vovName = "levelIntro_clear";
		GameSound.PreloadVOV(vovName);
	}

	private void Start()
	{
		GameSound.StopBGM("BossFight");
		GameSound.StartBGM("Menu");
	}

	private void OnDestroy()
	{
		GameSound.StopVOV();
	}

	private void Update()
	{
		if (iFade == 0 && fFade == 0.5f)
		{
			Application.LoadLevel("Adventure.Result");
			return;
		}
		float num;
		for (num = (Time.time - (float)(int)Time.time) * 3f; num >= 1f; num -= 1f)
		{
		}
		eff.SetTextureOffset("_MainTex", new Vector2(0f, (!(num < 0.5f)) ? 0.5f : 0f));
		if (eyeClose > 0f)
		{
			eyeClose -= Time.deltaTime;
			if (eyeClose <= 0f)
			{
				eyeClose = -1f;
			}
			eye.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f));
		}
		else if (eyeClose < 0f)
		{
			eyeClose += Time.deltaTime;
			if (eyeClose > 0f)
			{
				eyeClose = 0f;
			}
			eye.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
		}
		else if (Random.value > 0.99f)
		{
			eyeClose = 0.2f;
		}
		if (iFade != 0)
		{
			fFade += (float)iFade * Time.deltaTime / 2f;
			if (iFade > 0)
			{
				if (fFade >= 0.5f)
				{
					fFade = 0.5f;
					iFade = 0;
				}
			}
			else if (fFade <= 0f)
			{
				fFade = 0f;
				iFade = 0;
			}
			mFade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, fFade));
		}
		else if (vovName != null && Global.IsVoiceOn)
		{
			GameSound.StartVOV(vovName);
			vovName = null;
		}
		for (int i = 0; i < 5; i++)
		{
			GameController controller = GameInput.GetController(i);
			if (controller.IsConnected() && controller.DoEnterHold("Fire1"))
			{
				iFade = 1;
				break;
			}
		}
	}
}
