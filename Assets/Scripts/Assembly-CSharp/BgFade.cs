using UnityEngine;

public class BgFade : MonoBehaviour
{
	public bool fadeIn = true;

	public float fadeSpeed = 1f;

	public static bool fadeOut;

	public static string gotoScene = "MainMenu";

	private float fadeTime;

	private void Start()
	{
		fadeTime = 0f;
		fadeIn = true;
	}

	private void Update()
	{
		if (fadeIn)
		{
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0.5f, 0f, fadeTime += Time.deltaTime * fadeSpeed)));
			if (base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0f)
			{
				fadeTime = 0f;
				fadeIn = false;
			}
		}
		else if (fadeOut)
		{
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, Mathf.Lerp(0f, 0.5f, fadeTime += Time.deltaTime * fadeSpeed)));
			if (base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor").a == 0.5f)
			{
				fadeTime = 0f;
				fadeOut = false;
				Application.LoadLevel(gotoScene);
			}
		}
	}
}
