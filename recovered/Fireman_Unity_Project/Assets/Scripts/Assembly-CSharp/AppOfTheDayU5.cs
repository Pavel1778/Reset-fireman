using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AndroidPlugin;
using UnityEngine;

public class AppOfTheDayU5 : MonoBehaviour
{
	private class AppOfTheDayItem
	{
		public string Icon;

		public string PackageName;

		public string AppName;

		public string AppStoreID;

		public int Weight;

		public int WeightStart;

		public int WeightEnd;

		public AppOfTheDayItem(string icon, string packageName, string appName, string appStoreID, int weigth)
		{
			Icon = icon;
			PackageName = packageName;
			AppName = appName;
			AppStoreID = appStoreID;
			Weight = weigth;
		}
	}

	private static AppOfTheDayU5 instance;

	private Texture texture;

	public GameObject target;

	public string callback;

	private int getTryCount;

	private string url = string.Empty;

	private string url2click = string.Empty;

	private string idAppOfDay = string.Empty;

	public static AppOfTheDayU5 Instance
	{
		get
		{
			if (instance == null)
			{
				AppOfTheDayU5[] array = UnityEngine.Object.FindObjectsOfType<AppOfTheDayU5>();
				AppOfTheDayU5[] array2 = array;
				foreach (AppOfTheDayU5 appOfTheDayU in array2)
				{
					if (!appOfTheDayU.transform.parent)
					{
						instance = appOfTheDayU;
					}
				}
			}
			if (object.ReferenceEquals(instance, null))
			{
				GameObject gameObject = new GameObject();
				instance = gameObject.AddComponent<AppOfTheDayU5>();
				gameObject.name = "App Of Day Manager";
			}
			UnityEngine.Object.DontDestroyOnLoad(instance);
			return instance;
		}
	}

	public event Action<Texture> onloaded = (Texture t) =>
	{
	};

	public void addOnLoaded(Action<Texture> on)
	{
		onloaded = (Action<Texture>)Delegate.Combine(onloaded, on);
		if ((bool)texture)
		{
			on(texture);
		}
	}

	private void Start()
	{
		if (Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else if ((bool)base.gameObject)
		{
			AppOfTheDayReady("NOAPPOFDAY");
		}
	}

	public void AppOfTheDayReady(string AppOfDayParams)
	{
		if (AppOfDayParams == "NOAPPOFDAY")
		{
			StartCoroutine(askAppOfDayAgain());
			return;
		}
		string[] array = AppOfDayParams.Split("§".ToCharArray());
		if (array.Length == 4)
		{
			url = array[1];
			idAppOfDay = array[2];
			url2click = array[3];
			StartCoroutine(loadAppOfTheDayIcon(url));
		}
	}

	private void getIconByFile()
	{
		getTryCount++;
		if (getTryCount > 2)
		{
			return;
		}
		List<AppOfTheDayItem> list = new List<AppOfTheDayItem>();
		if (File.Exists(Application.persistentDataPath + "/appofdayios"))
		{
			string text = File.ReadAllText(Application.persistentDataPath + "/appofdayios");
			string[] array = text.Split("\n".ToCharArray());
			if (array.Length < 3)
			{
				File.Delete(Application.persistentDataPath + "/appofdayios");
				getIconByFile();
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != string.Empty)
				{
					string[] array2 = array[i].Split(";".ToCharArray());
					if (array2.Length == 5)
					{
						list.Add(new AppOfTheDayItem(array2[3], array2[0], array2[1], array2[2], int.Parse(array2[4])));
					}
				}
			}
			if (list.Count == 0)
			{
				File.Delete(Application.persistentDataPath + "/appofdayios");
				getIconByFile();
				return;
			}
		}
		else
		{
			TextAsset textAsset = (TextAsset)Resources.Load("appofdayios", typeof(TextAsset));
			string[] array3 = textAsset.text.Split("\n".ToCharArray());
			for (int j = 0; j < array3.Length; j++)
			{
				if (array3[j] != string.Empty)
				{
					string[] array4 = array3[j].Split(";".ToCharArray());
					if (array4.Length == 5)
					{
						list.Add(new AppOfTheDayItem(array4[3], array4[0], array4[1], array4[2], int.Parse(array4[4])));
					}
				}
			}
		}
		string currentPackage = PluginScript.Instance.getPName();
		list = list.FindAll((AppOfTheDayItem x) => x.PackageName != currentPackage);
		int num = 0;
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			list[num2].WeightStart = num;
			num += list[num2].Weight;
			list[num2].WeightEnd = num;
		}
		AppOfTheDayItem appOfTheDayItem = null;
		int num3 = UnityEngine.Random.Range(0, num) + 1;
		for (int num4 = 0; num4 < list.Count; num4++)
		{
			if (list[num4].WeightStart <= num3 && list[num4].WeightEnd >= num3)
			{
				appOfTheDayItem = list[num4];
				break;
			}
		}
		if (appOfTheDayItem == null)
		{
			Debug.LogWarning("App Of The Day Empty with Weight Method !");
			appOfTheDayItem = list[UnityEngine.Random.Range(0, list.Count)];
		}
		url2click = "http://itunes.apple.com/app/id" + appOfTheDayItem.AppStoreID;
		StartCoroutine(loadAppOfTheDayIcon(appOfTheDayItem.Icon));
		StartCoroutine(loadAppListFromWeb());
	}

	private IEnumerator loadAppListFromWeb()
	{
		WWW www = new WWW("http://api.magmamobile.com/ios/appofdayios.txt");
		yield return www;
		if (www.error == null)
		{
			File.WriteAllText(contents: www.text, path: Application.persistentDataPath + "/appofdayios");
		}
	}

	private IEnumerator askAppOfDayAgain()
	{
		yield return new WaitForSeconds(1f);
		getTryCount++;
		if (getTryCount > 20)
		{
			getTryCount = 0;
		}
		else
		{
			Mmusia.sendAppOfTheDay();
		}
	}

	private void OnEnable()
	{
		getTryCount = 0;
		AppOfTheDayReady("NOAPPOFDAY");
	}

	public void Clicked()
	{
		if (url2click != string.Empty && url2click != "null")
		{
			Mmusia.logAppOfDayClick(idAppOfDay);
			Application.OpenURL(url2click);
		}
	}

	private IEnumerator loadAppOfTheDayIcon(string url)
	{
		string path = Application.persistentDataPath + "/imgcache/";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		path += url.ToHash();
		if (File.Exists(path))
		{
			byte[] b = File.ReadAllBytes(path);
			Texture2D t = new Texture2D(2, 2, TextureFormat.ARGB32, false);
			t.LoadImage(b);
			onloaded(t);
			texture = t;
			yield break;
		}
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null && www.texture != null)
		{
			File.WriteAllBytes(path, www.bytes);
			onloaded(www.texture);
			texture = www.texture;
			if (target != null && !string.IsNullOrEmpty(callback))
			{
				target.SendMessage(callback, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
