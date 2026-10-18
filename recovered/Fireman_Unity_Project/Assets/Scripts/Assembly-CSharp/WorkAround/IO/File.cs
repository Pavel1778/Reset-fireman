using System.IO;

namespace WorkAround.IO
{
	public class File
	{
		public static byte[] ReadAllBytes(string path)
		{
			return System.IO.File.ReadAllBytes(path);
		}

		public static void WriteAllBytes(string path, byte[] contents)
		{
			System.IO.File.WriteAllBytes(path, contents);
		}

		public static string ReadAllText(string path)
		{
			return System.IO.File.ReadAllText(path);
		}

		public static void WriteAllText(string path, string contents)
		{
			System.IO.File.WriteAllText(path, contents);
		}
	}
}
