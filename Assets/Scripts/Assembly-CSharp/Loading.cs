using System;
using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour
{
	private static string nextScene;

	public TextAsset proTips;

	public int randomTime = 1;

	public float gearSpeed = 10f;

	public float fxSpeed = 12f;

	private int direction = 1;

	private Material fade;

	private Transform logo;

	private Transform gear;

	private Transform gearFX;

	private float progess;

	private AsyncOperation asyncOp;

	public static void LoadScene(string sceneName)
	{
		nextScene = sceneName;
		Application.LoadLevel("Loading");
	}

	private void Awake()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1.7777778f;
		Transform transform = GameObject.Find("banner").transform;
		Vector3 localPosition = transform.localPosition;
		localPosition.x *= (1f + num / num2) / 2f;
		transform.localPosition = localPosition;
		fade = GameObject.Find("fade").GetComponent<Renderer>().material;
		logo = transform.Find("logo");
		gear = logo.Find("bannergear");
		gearFX = logo.Find("bannereffect");
		fade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
		string[] array = proTips.text.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
		transform.Find("tip").GetComponent<TextMesh>().text = array[0] + array[UnityEngine.Random.Range(1, array.Length)];
	}

	private IEnumerator Start()
	{
		progess = 0f;
		if (Application.HasProLicense())
		{
			asyncOp = Application.LoadLevelAsync(nextScene);
			if (Global.Mode == GameMode.OnlineBattle && nextScene == "Adventure.GameMode")
			{
				TNManager.client.SetTimeout(30);
			}
		}
		yield return asyncOp;
	}

	private void Update()
	{
		if (Application.HasProLicense())
		{
			progess = asyncOp.progress;
		}
		else
		{
			if (progess == 1f)
			{
				Application.LoadLevel(nextScene);
			}
			progess += Time.deltaTime / 2.5f;
			if (progess > 1f)
			{
				progess = 1f;
			}
		}
		float num = 1f;
		if (progess < 0.2f)
		{
			num = progess * 5f;
		}
		else if (progess > 0.8f)
		{
			num = (1f - progess) * 5f;
		}
		fade.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, (1f - num) / 2f));
		if ((int)(Time.time * 1000f) % randomTime == 0)
		{
			direction = UnityEngine.Random.Range(-1, 2);
		}
		gear.Rotate(0f, 0f, gearSpeed * Time.deltaTime * (float)direction);
		gearFX.Rotate(0f, 0f, fxSpeed * Time.deltaTime);
	}
}
