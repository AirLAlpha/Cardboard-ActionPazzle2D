/**********************************************
 * 
 *  EasingFunctions.cs 
 *  イージング関数をまとめて使用できる処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/13
 * 
 **********************************************/
using UnityEngine;

public static class EasingFunctions
{
	public static float EaseInSine(float x)
	{
		return 1 - Mathf.Cos((x * Mathf.PI) / 2);
	}

	public static float EaseInCubic(float x)
	{
		return x * x * x;
	}

	public static float EaseInQuint(float x)
	{
		return x * x * x * x * x;
	}

	public static float EaseInCirc(float x)
	{
		return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
	}

	public static float EaseInElastic(float x)
	{
		const float c4 = (2 * Mathf.PI) / 3;

		return x == 0
			? 0
			: x == 1
			? 1
			: -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * c4);
	}

	public static float EaseOutSine(float x)
	{
		return Mathf.Sin((x * Mathf.PI) / 2);
	}

	public static float EaseOutCubic(float x)
	{
		return 1 - Mathf.Pow(1 - x, 3);
	}

	public static float EaseOutQuint(float x)
	{
		return 1 - Mathf.Pow(1 - x, 5);
	}

	public static float EaseOutCirc(float x)
	{
		return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
	}

	public static float EaseOutElastic(float x)
	{
		const float c4 = (2 * Mathf.PI) / 3;

		return x == 0
			? 0
			: x == 1
			? 1
			: Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
	}

	public static float EaseInOutSine(float x)
	{
		return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
	}

	public static float EaseInOutCubic(float x)
	{
		return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
	}

	public static float EaseInOutQuint(float x)
	{
		return x < 0.5f ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
	}

	public static float EaseInOutCirc(float x)
	{
		return x < 0.5f
			? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
			: (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
	}

	public static float EaseInOutElastic(float x)
	{
		const float c5 = (2 * Mathf.PI) / 4.5f;

		return x == 0
			? 0
			: x == 1
			? 1
			: x < 0.5f
			? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
			: (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
	}

	public static float EaseInQuad(float x)
	{
		return x * x;
	}

	public static float EaseInQuart(float x)
	{
		return x * x * x * x;
	}

	public static float EaseInExpo(float x)
	{
		return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
	}

	public static float EaseInBack(float x)
	{
		const float c1 = 1.70158f;
		const float c3 = c1 + 1;

		return c3 * x * x * x - c1 * x * x;
	}

	public static float EaseInBounce(float x)
	{
		return 1 - EaseOutBounce(1 - x);
	}

	public static float EaseOutQuad(float x)
	{
		return 1 - (1 - x) * (1 - x);
	}

	public static float EaseOutQuart(float x)
	{
		return 1 - Mathf.Pow(1 - x, 4);
	}

	public static float EaseOutExpo(float x)
	{
		return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
	}

	public static float EaseOutBack(float x)
	{
		const float c1 = 1.70158f;
		const float c3 = c1 + 1;

		return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
	}

	public static float EaseOutBounce(float x)
	{
		const float n1 = 7.5625f;
		const float d1 = 2.75f;

		if (x < 1 / d1)
		{
			return n1 * x * x;
		}
		else if (x < 2 / d1)
		{
			return n1 * (x -= 1.5f / d1) * x + 0.75f;
		}
		else if (x < 2.5f / d1)
		{
			return n1 * (x -= 2.25f / d1) * x + 0.9375f;
		}
		else
		{
			return n1 * (x -= 2.625f / d1) * x + 0.984375f;
		}
	}

	public static float EaseInOutQuad(float x)
	{
		return x < 0.5f ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
	}

	public static float EaseInOutQuart(float x)
	{
		return x < 0.5f ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
	}

	public static float EaseInOutExpo(float x)
	{
		return x == 0
			? 0
			: x == 1
			? 1
			: x < 0.5f ? Mathf.Pow(2, 20 * x - 10) / 2
			: (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
	}

	public static float EaseInOutBack(float x)
	{
		const float c1 = 1.70158f;
		const float c2 = c1 * 1.525f;

		return x < 0.5f
			? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
			: (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
	}

	public static float EaseInOutBounce(float x)
	{
		return x < 0.5f
			? (1 - EaseOutBounce(1 - 2 * x)) / 2
			: (1 + EaseOutBounce(2 * x - 1)) / 2;
	}
}
