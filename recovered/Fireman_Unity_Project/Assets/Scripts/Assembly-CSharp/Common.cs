using System;
using UnityEngine;

public class Common
{
	public static void PrintStackTrace()
	{
		Debug.Log(Environment.StackTrace);
	}

	public static int Absi(float value)
	{
		return (int)((!(value < 0f)) ? value : (0f - value));
	}

	public static int Absi(int value)
	{
		return (value >= 0) ? value : (-value);
	}

	public static float Absf(float value)
	{
		return (!(value < 0f)) ? value : (0f - value);
	}

	public static int Rndi(int min, int max)
	{
		return min + Int(UnityEngine.Random.value * (float)(max - min));
	}

	public static float Rndf(float min, float max)
	{
		return min + UnityEngine.Random.value * (max - min);
	}

	public static int Rndi(int max)
	{
		return Int(UnityEngine.Random.value * (float)max);
	}

	public static float Rndf(float max)
	{
		return UnityEngine.Random.value * max;
	}

	public static float minf(float a, float b)
	{
		return (!(a < b)) ? b : a;
	}

	public static float maxf(float a, float b)
	{
		return (!(a > b)) ? b : a;
	}

	public static float minmaxf(float a, float vmin, float vmax)
	{
		return maxf(minf(a, vmin), vmax);
	}

	public static float clipf(float a, float vmin, float vmax)
	{
		if (a < vmin)
		{
			a = vmin;
		}
		if (a > vmax)
		{
			a = vmax;
		}
		return a;
	}

	public static T FindObject<T>(string name) where T : Component
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	public static Transform FindTransform(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.transform;
	}

	public static int Int(float v)
	{
		int num = (int)v;
		float num2 = v - (float)num;
		if (num2 != 0f)
		{
			if (v > 0f)
			{
				return (!(num2 >= 0.5f)) ? num : (num + 1);
			}
			return (!(num2 <= 0.5f)) ? num : (num - 1);
		}
		return num;
	}

	public static int getNextPowerOfTwo(int n)
	{
		n--;
		n |= n >> 1;
		n |= n >> 2;
		n |= n >> 4;
		n |= n >> 8;
		n |= n >> 16;
		n++;
		return (n <= 0) ? 1 : n;
	}

	public static int RoundTo(float v, float nearTo)
	{
		return Int(v / nearTo) * (int)nearTo;
	}

	public static long getTick()
	{
		return DateTime.Now.Ticks / 10000;
	}

	public static float getCycleInterpolation(float input, float cycles)
	{
		return Mathf.Abs(Mathf.Sin(cycles * (float)Math.PI * input));
	}

	public static float getBackAndForthInterpolation(float input)
	{
		return (!(input < 0.5f)) ? (2f - input - input) : (input + input);
	}

	public static float getAnticipateInterpolation(float input, float tension)
	{
		return input * input * ((tension + 1f) * input - tension);
	}

	public static float getOvershootInterpolation(float input, float tension)
	{
		input--;
		return input * input * ((tension + 1f) * input + tension) + 1f;
	}

	public static float getAccelerateInterpolation(float input, float factor)
	{
		if (factor == 1f)
		{
			return input * input;
		}
		return Mathf.Pow(input, 2f * factor);
	}

	public static float getDecelerateInterpolation(float input, float factor)
	{
		if (factor == 1f)
		{
			return 1f - (1f - input) * (1f - input);
		}
		return 1f - Mathf.Pow(1f - input, 2f * factor);
	}

	public static float getShakeInterpolation(float input, float wave)
	{
		float num = wave / 4f;
		float num2 = 2f / wave;
		if (input < wave)
		{
			if (input < num)
			{
				return 0.5f + input * num2;
			}
			if (input < num * 3f)
			{
				return 1.5f - input * num2;
			}
			return input * num2 - 1.5f;
		}
		return 0.5f;
	}

	public static bool ApproximatelyEquals(float v1, float v2, float epsilon)
	{
		float num = Mathf.Abs(v1 - v2);
		return num <= epsilon;
	}

	public static bool ApproximatelyEquals(float v1, float v2)
	{
		float num = Mathf.Abs(v1 - v2);
		return num <= 0.01f;
	}

	public static float lerpAnim(int from, int to, int tick, int maxtick)
	{
		return (float)from + (float)(to - from) * getDecelerateInterpolation((float)tick / (float)maxtick, 1f);
	}

	public static float lerpLinear(float from, float to, float factor)
	{
		return from + (to - from) * factor;
	}

	public static float lerpAnticipate(float from, float to, float factor)
	{
		return from + (to - from) * getAnticipateInterpolation(factor, 2f);
	}

	public static float lerpOvershoot(float from, float to, float factor)
	{
		return from + (to - from) * getOvershootInterpolation(factor, 2f);
	}

	public static float lerpDecelerate(float from, float to, float factor)
	{
		return from + (to - from) * getDecelerateInterpolation(factor, 1f);
	}

	public static float lerpAccelerate(float from, float to, float factor)
	{
		return from + (to - from) * getAccelerateInterpolation(factor, 1f);
	}

	public static float lerpCycle(float from, float to, float factor, float cycles)
	{
		return from + (to - from) * getCycleInterpolation(factor, cycles);
	}

	public static float lerpBackAndForth(float from, float to, float factor)
	{
		return from + (to - from) * getBackAndForthInterpolation(factor);
	}

	public static float lerpShake(float from, float to, float factor, float wave)
	{
		return from + (to - from) * getShakeInterpolation(factor, wave);
	}
}
