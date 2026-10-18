using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AndroidPlugin;
using Ionic.Zlib;
using Nix.Maths;
using Nix.Text;
using UnityEngine;
using WorkAround.IO;

public class MagmaIntroScreen : MonoBehaviour
{
	[Serializable]
	public class preHome
	{
		[Serializable]
		public class preHomeItem
		{
			public string appid = "0";

			public string name = string.Empty;

			public string pname = string.Empty;

			public string url = string.Empty;

			public string iconurl = string.Empty;
		}

		public List<preHomeItem> items = new List<preHomeItem>();
	}

	private static MagmaIntroScreen instance;

	[Header("Settings")]
	public string PackageName;

	[Header("Touch and i break your legs")]
	public GameObject prefabButtonForList;

	public GameObject wrapContent;

	public GameObject uiScrollView;

	public UISlider uiSlider;

	public UILabel labelProgress;

	[SerializeField]
	public preHome apiPreHome = new preHome();

	private bool clickedGameItem;

	public static string apiUrl;

	private int trycount;

	private float deltaScroll;

	private string tmpScreenFingerPrint = string.Empty;

	private bool jsonIsLoaded;

	private AsyncOperation async;

	private bool AsyncSceneReady;

	public static MagmaIntroScreen Instance
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
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(base.gameObject);
			Debug.Log("Destroy MainManager instance !");
		}
		else
		{
			instance = this;
		}
	}

	private void Start()
	{
		apiUrl = "http://api.magmamobile.com/api/mmusiaph.ashx";
		if (PackageName == "0" || PackageName == "NOTSET" || PackageName == string.Empty)
		{
			Debug.LogError("PACKAGENAME NOT SET IN INTROSCREEN !!!");
		}
		StartMMUSIA();
		StartCoroutine(loadCoroutine());
		PluginScript.Instance.track("IntroScreen/");
	}

	public void loadNextScene()
	{
		async.allowSceneActivation = true;
		if (clickedGameItem)
		{
			PluginScript.Instance.track("IntroScreen/PlayButton/ClickedItem");
		}
		else
		{
			PluginScript.Instance.track("IntroScreen/PlayButton/NOTClickedItem");
		}
	}

	public void StartMMUSIA()
	{
		loadUrlParam();
	}

	public void loadUrlParam()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", Core.deviceUniqueIdentifier);
		wWWForm.AddField("lng", (!SystemLanguage.French.Equals(Application.systemLanguage)) ? "en" : "fr");
		wWWForm.AddField("pn", getAppPackageName());
		wWWForm.AddField("os", getPlatformString());
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip");
		Debug.Log("loading url : " + apiUrl);
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(WaitForRequest(www));
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		if (System.IO.File.Exists(Application.persistentDataPath + "/mmusiaprehomejsoncache") && System.IO.File.GetLastWriteTime(Application.persistentDataPath + "/mmusiaprehomejsoncache").AddMinutes(15.0) > DateTime.Now)
		{
			Debug.Log("Recup du json dans le cache !");
			Debug.Log(System.IO.File.GetLastWriteTime(Application.persistentDataPath + "/mmusiaprehomejsoncache"));
			Debug.Log(System.IO.File.GetLastWriteTime(Application.persistentDataPath + "/mmusiaprehomejsoncache").AddMinutes(15.0));
			Debug.Log(DateTime.Now);
			loadJsonInApiObj(WorkAround.IO.File.ReadAllText(Application.persistentDataPath + "/mmusiaprehomejsoncache"));
			jsonReady();
			yield break;
		}
		yield return www;
		if (string.IsNullOrEmpty(www.error) && www.isDone)
		{
			string str = null;
			if (www.responseHeaders.ContainsKey("CONTENT-ENCODING"))
			{
				string encoding = www.responseHeaders["CONTENT-ENCODING"];
				if (encoding.Contains("gzip"))
				{
					str = GZipStream.UncompressString(www.bytes);
				}
			}
			if (str == null)
			{
				str = www.text;
			}
			Debug.Log(str);
			loadJsonInApiObj(str);
			if (System.IO.File.Exists(Application.persistentDataPath + "/mmusiaprehomejsoncache"))
			{
				System.IO.File.Delete(Application.persistentDataPath + "/mmusiaprehomejsoncache");
			}
			WorkAround.IO.File.WriteAllText(Application.persistentDataPath + "/mmusiaprehomejsoncache", str);
			jsonReady();
			yield break;
		}
		Debug.LogError(www.error);
		if (trycount <= 2)
		{
			trycount++;
			yield return new WaitForSeconds(0.5f);
			loadUrlParam();
		}
		else if (System.IO.File.Exists(Application.persistentDataPath + "/mmusiaprehomejsoncache"))
		{
			string str2 = null;
			str2 = WorkAround.IO.File.ReadAllText(Application.persistentDataPath + "/mmusiaprehomejsoncache");
			if (str2 == null)
			{
				str2 = www.text;
			}
			loadJsonInApiObj(str2);
			jsonReady();
		}
	}

	public void loadJsonInApiObj(string str)
	{
		Dictionary<string, object> dictionary = Json.MagmaJson.Deserialize(str) as Dictionary<string, object>;
		if (dictionary == null || !dictionary.ContainsKey("OK") || !dictionary.ContainsKey("items") || !(dictionary["OK"].ToString() == "1"))
		{
			return;
		}
		Debug.Log("OK");
		Debug.Log(((List<object>)dictionary["items"]).Count);
		List<object> list = (List<object>)dictionary["items"];
		foreach (Dictionary<string, object> item in list)
		{
			if (item.ContainsKey("appid") && item.ContainsKey("name") && item.ContainsKey("pname") && item.ContainsKey("url") && item.ContainsKey("iconurl"))
			{
				preHome.preHomeItem preHomeItem = new preHome.preHomeItem();
				preHomeItem.appid = item["appid"].ToString();
				preHomeItem.name = item["name"].ToString();
				preHomeItem.pname = item["pname"].ToString();
				preHomeItem.url = item["url"].ToString();
				preHomeItem.iconurl = item["iconurl"].ToString();
				apiPreHome.items.Add(preHomeItem);
			}
		}
	}

	public string getPlatformString()
	{
		return "android";
	}

	public string getAppPackageName()
	{
		if (string.IsNullOrEmpty(Instance.PackageName))
		{
			throw new UnityException("NO PACKAGENAME SET IN MMUSIA PREHOME UNITY MANAGER !!!");
		}
		return Instance.PackageName;
	}

	private void Update()
	{
		if (jsonIsLoaded)
		{
			uiScrollView.GetComponent<UIDraggablePanel>().MoveRelative(new Vector3(-10f * Time.deltaTime, 0f, 0f));
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			loadNextScene();
		}
		checkScreenRotation();
	}

	public void checkScreenRotation()
	{
		if (tmpScreenFingerPrint != Screen.width + "/" + Screen.height)
		{
			uiScrollView.GetComponent<UIDraggablePanel>().ResetPosition();
			float pixelSizeAdjustment = GetComponent<UIRoot>().pixelSizeAdjustment;
			tmpScreenFingerPrint = Screen.width + "/" + Screen.height;
			uiScrollView.transform.localPosition = new Vector3(0f, 220f, 0f);
			if (Screen.width > Screen.height)
			{
				uiScrollView.GetComponent<UIPanel>().clipRange = new Vector4(0f, 0f, (float)Screen.width * pixelSizeAdjustment * 0.9f, 150f);
			}
			else
			{
				uiScrollView.GetComponent<UIPanel>().clipRange = new Vector4(0f, 0f, (float)Screen.width * pixelSizeAdjustment * 0.9f, 150f);
			}
			wrapContent.GetComponent<UIGrid>().repositionNow = true;
			wrapContent.GetComponent<UIGrid>().Reposition();
			uiScrollView.GetComponent<UIDraggablePanel>().ResetPosition();
		}
	}

	public void preHomeClick(preHome.preHomeItem ns)
	{
		clickedGameItem = true;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", Core.deviceUniqueIdentifier);
		wWWForm.AddField("a", "cph");
		wWWForm.AddField("lng", (!SystemLanguage.French.Equals(Application.systemLanguage)) ? "en" : "fr");
		wWWForm.AddField("pn", getAppPackageName());
		wWWForm.AddField("pn2", ns.pname);
		wWWForm.AddField("os", getPlatformString());
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip, deflate");
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(ignoreResponse(www));
		PluginScript.Instance.track("IntroScreen/Click/" + ns.pname);
	}

	private IEnumerator ignoreResponse(WWW www)
	{
		yield return www;
		yield return www;
		if (string.IsNullOrEmpty(www.error) && www.isDone)
		{
			Debug.Log("Click PreHome OK !");
		}
		else
		{
			Debug.LogError("Click PreHome ERROR !!! : " + www.error);
		}
	}

	private void jsonReady()
	{
		Debug.Log("Received !");
		for (int i = 0; i < apiPreHome.items.Count; i++)
		{
			GameObject gameObject = NGUITools.AddChild(wrapContent.gameObject, prefabButtonForList);
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -100f);
			gameObject.GetComponent<IntroScreenButton>().cleanImageAndLabelInList();
			gameObject.GetComponent<IntroScreenButton>().pname = apiPreHome.items[i].name;
			gameObject.GetComponent<IntroScreenButton>().itemFromApi = apiPreHome.items[i];
			gameObject.name = Nix.Text.Utils.int2String0000(i) + "button";
			StartCoroutine(getAppIcon(0.01f, gameObject, apiPreHome.items[i].iconurl, apiPreHome.items[i].name, apiPreHome.items[i].url));
		}
		wrapContent.GetComponent<UIGrid>().repositionNow = true;
		wrapContent.GetComponent<UIGrid>().Reposition();
		uiScrollView.GetComponent<UIDraggablePanel>().ResetPosition();
		jsonIsLoaded = true;
	}

	private IEnumerator getAppIcon(float waitTime, GameObject go, string url, string appName, string urlMarket)
	{
		yield return new WaitForSeconds(waitTime);
		IntroScreenImageCacheManager.Instance.getImageCache(go.GetComponent<IntroScreenButton>(), url, null, string.Empty, appName, urlMarket);
	}

	private IEnumerator loadCoroutine()
	{
		Debug.Log("Loading starts");
		async = Application.LoadLevelAsync(2);
		async.priority = 0;
		async.allowSceneActivation = false;
		while (!async.isDone)
		{
			Debug.Log("Loading Progress : " + async.progress);
			if (async.progress == 0.9f)
			{
				Debug.Log("Loading done - ACTIVATE SCENE");
				AsyncSceneReady = true;
				yield return new WaitForSeconds(3f);
				uiSlider.sliderValue = 1f;
				labelProgress.text = "100%";
			}
			else
			{
				uiSlider.sliderValue = async.progress;
				labelProgress.text = string.Format("{0:0}", Nix.Maths.Utils.getPercent(1f, async.progress, 0f)) + "%";
			}
			yield return 0;
		}
	}
}
