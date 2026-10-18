using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

public class MMUSIAImgCacheManager : MonoBehaviour
{
	private static MMUSIAImgCacheManager instance;

	public GameObject target;

	public string callback;

	public static MMUSIAImgCacheManager Instance
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
		return File.Exists(Application.persistentDataPath + "/" + filename);
	}

	public void getImage(string url, UITexture s)
	{
		getImage(url, s, false);
	}

	public void getImage(string url, UITexture s, bool appOfDay)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		Texture2D texture2D = new Texture2D(72, 72, TextureFormat.ARGB32, false);
		if (isUrlInCache(url))
		{
			string filename = getFilename(url);
			if (!(filename == string.Empty))
			{
				FileStream input = File.OpenRead(Application.persistentDataPath + "/" + filename);
				byte[] data = ReadFully(input);
				texture2D.LoadImage(data);
				s.mainTexture = texture2D;
				if (appOfDay && target != null && target.activeSelf)
				{
					target.SendMessage(callback, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else
		{
			StartCoroutine(loadIcon(url, s, appOfDay));
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

	public IEnumerator loadIcon(string url, UITexture s, bool appOfDay)
	{
		if (string.IsNullOrEmpty(url))
		{
			yield return 0;
		}
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			if (s != null)
			{
				s.mainTexture = www.texture;
				WriteFully(Application.persistentDataPath + "/" + getFilename(url), www.texture.EncodeToPNG());
				if (appOfDay && target != null)
				{
					target.SendMessage(callback, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else
		{
			s.gameObject.SetActive(false);
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
