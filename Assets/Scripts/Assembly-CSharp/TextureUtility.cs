using UnityEngine;

public class TextureUtility
{
	public static void SetTextureAnimation(GameObject obj, float colSpeed, float rowSpeed, bool animUV, float colStep, float rowStep, float showSizeX, float showSizeY, bool animAlpha, float alphaStart, float alphaEnd, float alphaDuration)
	{
		if (animUV)
		{
			float x = Time.time * colSpeed;
			float y = Time.time * rowSpeed;
			Vector2 offset = new Vector2(x, y);
			Vector2 scale = new Vector2(showSizeX, showSizeY);
			obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
			obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		}
		if (animAlpha)
		{
			float a = Mathf.PingPong(Time.time * alphaDuration, alphaEnd) + alphaStart;
			Color color = obj.GetComponent<Renderer>().material.GetColor("_TintColor");
			obj.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(color.r, color.g, color.b, a));
		}
	}

	public static bool SetSpriteAnimation(GameObject obj, int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, float speed)
	{
		int num = (int)(Time.time * speed);
		num %= totalCells;
		float x = 1f / (float)colCount;
		float y = 1f / (float)rowCount;
		Vector2 scale = new Vector2(x, y);
		int num2 = num % colCount;
		int num3 = num / colCount;
		float num4 = (float)(num2 + colNumber) * scale.x;
		float num5 = 1f - scale.y - (float)(num3 + rowNumber) * scale.y;
		for (; num4 < 0f; num4 += 1f)
		{
		}
		for (; num5 < 0f; num5 += 1f)
		{
		}
		while (num4 >= 1f)
		{
			num4 -= 1f;
		}
		while (num5 >= 1f)
		{
			num5 -= 1f;
		}
		Vector2 offset = new Vector2(num4, num5);
		obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
		Debug.Log("frame index of:" + obj.name + " = " + num);
		if (num == totalCells - 1)
		{
			return true;
		}
		return false;
	}

	public static void SetSpriteIndex(GameObject obj, int colCount, int rowCount, int index)
	{
		index %= colCount * rowCount;
		float x = 1f / (float)colCount;
		float y = 1f / (float)rowCount;
		Vector2 scale = new Vector2(x, y);
		int num = index % colCount;
		int num2 = index / colCount;
		float num3 = (float)num * scale.x;
		float num4 = 1f - scale.y - (float)num2 * scale.y;
		for (; num3 < 0f; num3 += 1f)
		{
		}
		for (; num4 < 0f; num4 += 1f)
		{
		}
		while (num3 >= 1f)
		{
			num3 -= 1f;
		}
		while (num4 >= 1f)
		{
			num4 -= 1f;
		}
		Vector2 offset = new Vector2(num3, num4);
		obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

	public static void clearAnim(GameObject obj, int colCount, int rowCount)
	{
		float x = 1f / (float)colCount;
		float y = 1f / (float)rowCount;
		Vector2 scale = new Vector2(x, y);
		Vector2 offset = new Vector2(x, y);
		obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}
}
