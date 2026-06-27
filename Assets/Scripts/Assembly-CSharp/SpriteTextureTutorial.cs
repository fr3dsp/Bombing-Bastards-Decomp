using UnityEngine;

public class SpriteTextureTutorial : MonoBehaviour
{
	public int total_sprite = 1;

	public float speed = 1f;

	public int typeAnimation;

	public bool alpha;

	public float alpha_speed = 1f;

	public float alpha_max = 1f;

	public float alpha_min;

	private bool alpha_down = true;

	private int[] dr_sprite_animation_array = new int[42]
	{
		0, 0, 0, 0, 0, 0, 1, 2, 3, 4,
		3, 2, 1, 0, 0, 1, 0, 0, 0, 1,
		0, 1, 0, 0, 0, 0, 0, 1, 0, 1,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0
	};

	private int[] dr_sprite_effect_animation_array = new int[4] { 0, 1, 2, 1 };

	private int[] mask_point_animation_array = new int[10] { 0, 1, 2, 3, 4, 5, 4, 3, 2, 1 };

	public float starTimeSpace = 2f;

	private float starCurrentTime;

	private bool starAnimation;

	private void Awake()
	{
	}

	private void Update()
	{
		switch (typeAnimation)
		{
		case 0:
			runDrSpriteAnimation();
			break;
		case 1:
			runDrSpriteEffectAnimation();
			break;
		case 2:
			runArrowAnimation();
			break;
		case 3:
			runMaskAnimation();
			break;
		case 4:
			runStarEffect();
			break;
		}
	}

	public void runStarEffect()
	{
		if (starCurrentTime >= starTimeSpace)
		{
			starAnimation = true;
			starCurrentTime = 0f;
		}
		if (starAnimation)
		{
			if (alpha_down)
			{
				Color color = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
				color.a -= Time.deltaTime * alpha_speed;
				if (color.a >= alpha_max)
				{
					color.a = alpha_max;
				}
				if (color.a <= alpha_min)
				{
					color.a = alpha_min;
					alpha_down = false;
					starAnimation = false;
				}
				base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			}
			else
			{
				Color color2 = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
				color2.a += Time.deltaTime * alpha_speed;
				if (color2.a <= alpha_min)
				{
					color2.a = alpha_min;
				}
				if (color2.a >= alpha_max)
				{
					color2.a = alpha_max;
					alpha_down = true;
				}
				base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color2);
			}
		}
		else
		{
			starCurrentTime += Time.deltaTime;
		}
	}

	public void runDrSpriteAnimation()
	{
		float num = Time.time * speed;
		num -= (float)(int)num;
		float num2 = (float)dr_sprite_animation_array[(int)(num * (float)dr_sprite_animation_array.Length)] / (float)total_sprite;
		float y = 0.5f;
		num2 *= 2f;
		if (num2 >= 1f)
		{
			num2 -= 1f;
			y = 0f;
		}
		Vector2 offset = new Vector2(num2, y);
		base.gameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
	}

	public void runDrSpriteEffectAnimation()
	{
		float num = Time.time * speed;
		num -= (float)(int)num;
		float x = (float)dr_sprite_effect_animation_array[(int)(num * (float)dr_sprite_effect_animation_array.Length)] / (float)total_sprite;
		Vector2 offset = new Vector2(x, 0f);
		base.gameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		if (!alpha)
		{
			return;
		}
		if (alpha_down)
		{
			Color color = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
			color.a -= Time.deltaTime * alpha_speed;
			if (color.a >= alpha_max)
			{
				color.a = alpha_max;
			}
			if (color.a <= alpha_min)
			{
				color.a = alpha_min;
				alpha_down = false;
			}
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
		}
		else
		{
			Color color2 = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
			color2.a += Time.deltaTime * alpha_speed;
			if (color2.a <= alpha_min)
			{
				color2.a = alpha_min;
			}
			if (color2.a >= alpha_max)
			{
				color2.a = alpha_max;
				alpha_down = true;
			}
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color2);
		}
	}

	public void runArrowAnimation()
	{
		if (!alpha)
		{
			return;
		}
		if (alpha_down)
		{
			Color color = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
			color.a -= Time.deltaTime * alpha_speed;
			if (color.a <= alpha_min)
			{
				color.a = alpha_min;
				alpha_down = false;
			}
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color);
		}
		else
		{
			Color color2 = base.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
			color2.a += Time.deltaTime * alpha_speed;
			if (color2.a >= alpha_max)
			{
				color2.a = alpha_max;
				alpha_down = true;
			}
			base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", color2);
		}
	}

	public void runMaskAnimation()
	{
		float num = Time.time * speed;
		num -= (float)(int)num;
		float num2 = (float)mask_point_animation_array[(int)(num * (float)mask_point_animation_array.Length)] / (float)total_sprite;
		float y = 0.5f;
		num2 *= 2f;
		if (num2 >= 1f)
		{
			num2 -= 1f;
			y = 0f;
		}
		Vector2 offset = new Vector2(num2, y);
		base.gameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
	}
}
