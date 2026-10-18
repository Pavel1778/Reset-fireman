namespace Nix.Maths
{
	public class Utils
	{
		public static float getPercent(int max, int val)
		{
			return getPercent(max, val, 0f);
		}

		public static float getPercent(float max, float val)
		{
			return getPercent(max, val, 0f);
		}

		public static float getPercent(int max, int val, float defaultValue)
		{
			return getPercent((float)max, (float)val, defaultValue);
		}

		public static float getPercent(float max, float val, float defaultValue)
		{
			if (max > 0f)
			{
				return val / max * 100f;
			}
			return defaultValue;
		}
	}
}
