using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class IntroScreenImageCacheManager : MonoBehaviour
{
	private static IntroScreenImageCacheManager instance;

	public static IntroScreenImageCacheManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
			Debug.Log("Destroy ImageCacheManager instance !");
		}
		else
		{
			instance = this;
		}
	}

	public string getFilename(string url)
	{
		return Txt2Alphanumeric(url);
	}

	public bool isUrlInCache(string url)
	{
		string filename = getFilename(url);
		if (filename == string.Empty)
		{
			return false;
		}
		return File.Exists(Application.persistentDataPath + "/imgcache/" + filename);
	}

	public void saveImageCache(string url, byte[] b)
	{
		if (b.Length == 0)
		{
			Debug.LogError("Photo vide");
			return;
		}
		if (!Directory.Exists(Application.persistentDataPath + "/imgcache/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/imgcache/");
		}
		WriteFully(Application.persistentDataPath + "/imgcache/" + getFilename(url), b);
	}

	public void getImageCache(IntroScreenButton obj, string url, GameObject targetNotify, string callbackFunction, string appName, string urlMarket)
	{
		StartCoroutine(getImageCacheCoroutine(obj, url, targetNotify, callbackFunction, appName, urlMarket));
	}

	private IEnumerator getImageCacheCoroutine(IntroScreenButton obj, string url, GameObject targetNotify, string callbackFunction, string appName, string urlMarket)
	{
		byte[] b = null;
		if (string.IsNullOrEmpty(url))
		{
			b = new byte[0];
		}
		if (isUrlInCache(url))
		{
			string fname = getFilename(url);
			if (fname == string.Empty)
			{
				b = new byte[0];
			}
			FileStream fs = File.OpenRead(Application.persistentDataPath + "/imgcache/" + fname);
			b = ReadFully(fs);
			if (targetNotify != null)
			{
				targetNotify.SendMessage(callbackFunction, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			WWW www = new WWW(url);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				b = www.texture.EncodeToPNG();
				saveImageCache(url, b);
			}
			else
			{
				Debug.LogError("Erreur réseau, pas d'image !");
				b = new byte[0];
			}
		}
		if (b.Length > 10)
		{
			obj.setImageAndLabelInList(b, appName, urlMarket);
		}
		else
		{
			obj.setImageAndLabelInList(null, appName, urlMarket);
		}
	}

	public static byte[] ReadFully(Stream input)
	{
		byte[] array = new byte[16384];
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int count;
			while ((count = input.Read(array, 0, array.Length)) > 0)
			{
				memoryStream.Write(array, 0, count);
			}
			return memoryStream.ToArray();
		}
	}

	public static void WriteFully(string path, byte[] bytes)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			fileStream.Write(bytes, 0, bytes.Length);
		}
	}

	public static string Txt2Alphanumeric(string Text)
	{
		if (Text == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		text = Text.Trim().ToLower();
		for (int i = 0; i <= text.Length - 1; i++)
		{
			char value = text[i];
			if ("abcdefghijklmnopqrstuvwxyz1234567890".Contains(value.ToString()))
			{
				stringBuilder.Append(value);
			}
		}
		return stringBuilder.ToString();
	}
}
