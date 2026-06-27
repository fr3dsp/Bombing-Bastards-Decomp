using System.Collections;
using UnityEngine;

public class NoiseAnim : MonoBehaviour
{
	public int type;

	public float waitTime = 0.5f;

	public float offsetX = 1f;

	public float offsetY = 1f;

	public bool ignoreX;

	public bool ignoreY;

	public float speedX = 1f;

	public float speedY = 1f;

	public string map = "_BumpMap";

	private IEnumerator Start()
	{
		float customtime = waitTime;
		yield return StartCoroutine("UpdateTexture", customtime);
	}

	private IEnumerator UpdateTexture(float customtime)
	{
		while (true)
		{
			if (type == 0)
			{
				Vector2 curOffset = base.GetComponent<Renderer>().material.GetTextureOffset(map);
				float toffsetX = ((!ignoreX) ? (offsetX * Random.value) : curOffset.x);
				float toffsetY = ((!ignoreY) ? (offsetY * Random.value) : curOffset.y);
				Vector2 offset = new Vector2(toffsetX, toffsetY);
				base.GetComponent<Renderer>().material.SetTextureOffset(map, offset);
			}
			else if (type == 1)
			{
				Vector2 curOffset2 = base.GetComponent<Renderer>().material.GetTextureOffset(map);
				float stepX = ((!ignoreX) ? (Time.time * speedX) : curOffset2.x);
				float stepY = ((!ignoreY) ? (Time.time * speedY) : curOffset2.y);
				Vector2 offset2 = new Vector2(stepX, stepY);
				base.GetComponent<Renderer>().material.SetTextureOffset(map, offset2);
			}
			else if (type == -1)
			{
				break;
			}
			while (type == -2)
			{
				Vector2 curOffset3 = base.GetComponent<Renderer>().material.GetTextureOffset(map);
				float stepX2 = ((!ignoreX) ? 0f : curOffset3.x);
				float stepY2 = ((!ignoreY) ? 0f : curOffset3.y);
				base.GetComponent<Renderer>().material.SetTextureOffset(map, new Vector2(stepX2, stepY2));
				yield return new WaitForFixedUpdate();
			}
			yield return new WaitForSeconds(waitTime);
		}
		Vector2 curOffset4 = base.GetComponent<Renderer>().material.GetTextureOffset(map);
		float stepX3 = ((!ignoreX) ? 0f : curOffset4.x);
		float stepY3 = ((!ignoreY) ? 0f : curOffset4.y);
		base.GetComponent<Renderer>().material.SetTextureOffset(map, new Vector2(stepX3, stepY3));
		GetComponent<NoiseAnim>().enabled = false;
	}

	private void Update()
	{
	}
}
