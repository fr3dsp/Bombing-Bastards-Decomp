using UnityEngine;

public class AnimateTexture : MonoBehaviour
{
	public bool animUV;

	public float colSpeed = 0.1f;

	public float rowSpeed = 0.1f;

	public float showSizeX = 0.1f;

	public float showSizeY = 0.1f;

	public bool animAlpha;

	public float alphaStart = 0.5f;

	public float alphaEnd;

	public float alphaDuration = 0.6f;

	private Vector2 offset;

	private void Update()
	{
		SetSpriteAnimation(animUV, colSpeed, rowSpeed, showSizeX, showSizeY, animAlpha, alphaStart, alphaEnd, alphaDuration);
	}

	private void SetSpriteAnimation(bool animUV, float colStep, float rowStep, float showSizeX, float showSizeY, bool animAlpha, float alphaStart, float alphaEnd, float alphaDuration)
	{
		if (animUV)
		{
			float num = Time.time * colSpeed;
			float num2 = Time.time * rowSpeed;
			while (num >= 1f)
			{
				num -= 1f;
			}
			while (num2 >= 1f)
			{
				num2 -= 1f;
			}
			for (; num < 0f; num += 1f)
			{
			}
			for (; num2 < 0f; num2 += 1f)
			{
			}
			Vector2 vector = new Vector2(num, num2);
			Vector2 scale = new Vector2(showSizeX, showSizeY);
			base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vector);
			base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		}
		if (animAlpha)
		{
			float a = Mathf.PingPong(Time.time * alphaDuration, alphaEnd) + alphaStart;
			Color color = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, a));
		}
	}
}
