namespace Nix.Text
{
	public class Utils
	{
		public static string JoinStringArray(string[] arr, string separator)
		{
			return string.Join(separator, arr);
		}

		public static string int2String00(int num)
		{
			if (num > 99)
			{
				return num.ToString();
			}
			string text = "00";
			string text2 = num + string.Empty;
			return text.Substring(0, text.Length - text2.Length) + text2;
		}

		public static string int2String000(int num)
		{
			if (num > 999)
			{
				return num.ToString();
			}
			string text = "000";
			string text2 = num + string.Empty;
			return text.Substring(0, text.Length - text2.Length) + text2;
		}

		public static string int2String0000(int num)
		{
			if (num > 9999)
			{
				return num.ToString();
			}
			string text = "0000";
			string text2 = num + string.Empty;
			return text.Substring(0, text.Length - text2.Length) + text2;
		}
	}
}
