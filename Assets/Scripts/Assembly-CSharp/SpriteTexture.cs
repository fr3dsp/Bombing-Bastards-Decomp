using UnityEngine;

public class SpriteTexture : MonoBehaviour
{
	public bool toggle = true;

	public bool play = true;

	public int colCount = 4;

	public int rowCount = 4;

	public int colNumber;

	public int rowNumber;

	public int totalCells = 4;

	public float speed = 10f;

	public int animCount;

	public int showIndex;

	private Vector2 offset;

	private int count;

	private float animtime;

	private void Update()
	{
		if (play)
		{
			if (toggle)
			{
				toggle = false;
				ResetAnim();
			}
			PlayAnim(base.gameObject, colCount, rowCount, rowNumber, colNumber, totalCells, speed);
		}
		else if (toggle)
		{
			SetSpriteAnimation(base.gameObject, colCount, rowCount, rowNumber, colNumber, totalCells, speed);
		}
	}

	public void SetSpriteAnimation(GameObject obj, int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, float speed)
	{
		int num = (int)(Time.time * speed);
		num = (showIndex = num % totalCells);
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
		Vector2 vector = new Vector2(num4, num5);
		obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vector);
		obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

	public void PlayAnim(GameObject obj, int colCount, int rowCount, int rowNumber, int colNumber, int totalCells, float speed)
	{
		animtime += Time.deltaTime;
		int num = (int)(animtime * speed);
		num %= totalCells;
		if (count < animCount || animCount == 0)
		{
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
			Vector2 vector = new Vector2(num4, num5);
			obj.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vector);
			obj.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
			if (num != showIndex)
			{
				showIndex = num;
				if (showIndex == totalCells - 1)
				{
					count++;
				}
			}
		}
		else if (count == animCount)
		{
			play = false;
		}
	}

	public void SetCount(int newcount)
	{
		animCount = newcount;
	}

	public void ResetAnim()
	{
		count = 0;
		animtime = 0f;
	}
}
