using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AndroidPlugin;
using Ionic.Zlib;
using UnityEngine;

public class mmusia : MonoBehaviour
{
	public class PromoInfo
	{
		public string title;

		public string description;

		public string iconUrl;
	}

	public UIRootMMUSIABase uiRootPrefab;

	private bool ready;

	public ApiBase api;

	public bool notifAllowed = true;

	public string RefererComplement = string.Empty;

	public bool reset_prefs;

	public string PackageName;

	public int versionCode = 1;

	[HideInInspector]
	public bool newsLoaded;

	[HideInInspector]
	public UIRootMMUSIABase uirootMmusia;

	public MMUSIAAppOfDayBase MMUSIAappOfDay;

	public GameObject target;

	public string targetCallback;

	private static mmusia instance;

	public static string apiUrl = "http://api.magmamobile.com/api/mmusia.ashx";

	private int trycount;

	[HideInInspector]
	public bool moreGamesLoaded;

	private int promo_id;

	private string promo_url;

	public PromoInfo promoInfo;

	public static mmusia Instance
	{
		get
		{
			return instance;
		}
	}

	private void Start()
	{
	}

	private IEnumerator DelayedMMUSIAStartForDebug()
	{
		yield return new WaitForSeconds(5f);
		StartMMUSIA();
	}

	public void clean(bool forCameraActivation)
	{
		if (uirootMmusia != null)
		{
			uirootMmusia.CameraRoot.GetComponent<Camera>().depth = -1f;
			uirootMmusia.clean();
		}
	}

	public void closeMMUSIAItem()
	{
		if (target != null && !string.IsNullOrEmpty(targetCallback))
		{
			target.SendMessage(targetCallback, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void broadcastOpenList()
	{
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage("MMUSIAListOpen", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void broadcastCloseList()
	{
		GameObject[] array = (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage("MMUSIAListClose", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void StartMMUSIA()
	{
		loadUrlParam(0, false);
		launchCountIncrement();
	}

	public void loadUIRoot()
	{
		uirootMmusia = UnityEngine.Object.Instantiate(uiRootPrefab);
		UnityEngine.Object.DontDestroyOnLoad(uirootMmusia);
		uirootMmusia.name = "UIRootMMUSIABase";
		uirootMmusia.CameraRoot.GetComponent<Camera>().depth = 100f;
	}

	public void checkUIRoot()
	{
		if (uirootMmusia == null)
		{
			loadUIRoot();
		}
		uirootMmusia.CameraRoot.GetComponent<Camera>().depth = 100f;
	}

	public void KillMeImFamous()
	{
		UnityEngine.Object.Destroy(this);
		UnityEngine.Object.Destroy(base.gameObject);
		instance = null;
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
	}

	public static bool isPackageNameInDevice(string pname)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			IntPtr clazz = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
			IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "currentActivity", "Landroid/app/Activity;");
			IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(clazz, staticFieldID);
			IntPtr clazz2 = AndroidJNI.FindClass("com/magmamobile/mmusiaunity/Plugin");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz2, "packages", "(Landroid/content/Context;Ljava/lang/String;)Z");
			jvalue[] array = new jvalue[2]
			{
				default(jvalue),
				default(jvalue)
			};
			array[0].l = staticObjectField;
			array[1].l = AndroidJNI.NewStringUTF(pname);
			return AndroidJNI.CallStaticBooleanMethod(clazz2, staticMethodID, array);
		}
		return false;
	}

	public static int getSDK()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return 15;
		}
		return 4;
	}

	public static string getAppPackageName()
	{
		if (string.IsNullOrEmpty(Instance.PackageName))
		{
			throw new Exception("NO PACKAGENAME SET IN MMUSIA UNITY MANAGER !!!");
		}
		return Instance.PackageName;
	}

	public static string getDeviceID()
	{
		return Core.deviceUniqueIdentifier;
	}

	public static string getLanguage()
	{
		return (!SystemLanguage.French.Equals(Application.systemLanguage)) ? "en" : "fr";
	}

	public int getAppVersionCode()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return 1;
		}
		return versionCode;
	}

	public static string getModelNumber()
	{
		return SystemInfo.deviceModel;
	}

	public static string getScreenSize()
	{
		return string.Empty + Screen.width + "x" + Screen.height;
	}

	public static string getMMUSIAVersion()
	{
		return "4";
	}

	public void loadUrlParam(int LatestNewsId, bool forceUpdate)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", getDeviceID());
		wWWForm.AddField("lng", getLanguage());
		wWWForm.AddField("lid", LatestNewsId + string.Empty);
		wWWForm.AddField("ver", getAppVersionCode() + string.Empty);
		wWWForm.AddField("pn", getAppPackageName());
		wWWForm.AddField("pm", getModelNumber());
		wWWForm.AddField("sw", getScreenSize());
		wWWForm.AddField("sv", getSDK());
		wWWForm.AddField("mmver", getMMUSIAVersion());
		if (forceUpdate)
		{
			wWWForm.AddField("gu", "1");
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip");
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(WaitForRequest(www));
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		if (www.error == null && www.isDone)
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
			JSONObject jso = new JSONObject(str);
			loadResultJSONObject(jso);
			if (MMUSIAappOfDay != null)
			{
				loadIcon(api.appodayIconUrl, MMUSIAappOfDay.GetComponent<UITexture>(), true);
			}
			ready = true;
			if (File.Exists(Application.persistentDataPath + "/mmusiajsoncache"))
			{
				File.Delete(Application.persistentDataPath + "/mmusiajsoncache");
			}
			File.WriteAllText(Application.persistentDataPath + "/mmusiajsoncache", str);
		}
		else if (trycount <= 2)
		{
			trycount++;
			yield return new WaitForSeconds(1f);
			loadUrlParam(0, false);
		}
		else if (File.Exists(Application.persistentDataPath + "/mmusiajsoncache"))
		{
			string str2 = null;
			str2 = File.ReadAllText(Application.persistentDataPath + "/mmusiajsoncache");
			if (str2 == null)
			{
				str2 = www.text;
			}
			JSONObject jso2 = new JSONObject(str2);
			loadResultJSONObject(jso2);
			if (MMUSIAappOfDay != null)
			{
				loadIcon(api.appodayIconUrl, MMUSIAappOfDay.GetComponent<UITexture>(), true);
			}
			ready = true;
		}
	}

	public void AppOfDayIfAvailable()
	{
		if (api == null || string.IsNullOrEmpty(api.appodayUrl))
		{
			return;
		}
		if (MMUSIAappOfDay == null)
		{
			MMUSIAAppOfDayBase mMUSIAAppOfDayBase = (MMUSIAAppOfDayBase)UnityEngine.Object.FindObjectOfType(typeof(MMUSIAAppOfDayBase));
			if (mMUSIAAppOfDayBase != null)
			{
				MMUSIAappOfDay = mMUSIAAppOfDayBase;
			}
		}
		if (MMUSIAappOfDay != null)
		{
			loadIcon(api.appodayIconUrl, MMUSIAappOfDay.GetComponent<UITexture>(), true);
		}
	}

	private void loadResultJSONObject(JSONObject js)
	{
		api = new ApiBase();
		api.news = new List<ItemNews>();
		api.moregames = new List<ItemMoreGames>();
		api.promo = new List<Promo>();
		for (int i = 0; i < js.GetField("news").list.Count; i++)
		{
			try
			{
				ItemNews itemNews = new ItemNews();
				itemNews.NewsID = (int)js.GetField("news").list[i].GetField("NewsID").n;
				itemNews.NewsLanguage = js.GetField("news").list[i].GetField("NewsLanguage").str;
				itemNews.NewsTitle = js.GetField("news").list[i].GetField("NewsTitle").str;
				itemNews.NewsDesc = js.GetField("news").list[i].GetField("NewsDesc").str;
				itemNews.NewsDate = DateTime.Parse(js.GetField("news").list[i].GetField("NewsDate").str);
				itemNews.NewsUrlLink = js.GetField("news").list[i].GetField("NewsUrlLink").str;
				itemNews.NewsUrlMarket = js.GetField("news").list[i].GetField("NewsUrlMarket").str;
				if (itemNews.NewsUrlMarket.Contains("?"))
				{
					itemNews.NewsUrlMarket = itemNews.NewsUrlMarket + "&-" + RefererComplement;
				}
				else
				{
					itemNews.NewsUrlMarket = itemNews.NewsUrlMarket + "?-" + RefererComplement;
				}
				itemNews.imgUrl = js.GetField("news").list[i].GetField("NewsImgUrl").str;
				api.news.Add(itemNews);
			}
			catch (Exception)
			{
			}
		}
		for (int j = 0; j < js.GetField("moregames").list.Count; j++)
		{
			try
			{
				ItemMoreGames itemMoreGames = new ItemMoreGames();
				itemMoreGames.title = js.GetField("moregames").list[j].GetField("title").str;
				itemMoreGames.urlImg = js.GetField("moregames").list[j].GetField("urlImg").str;
				itemMoreGames.urlMarket = js.GetField("moregames").list[j].GetField("urlMarket").str;
				itemMoreGames.pname = js.GetField("moregames").list[j].GetField("pname").str;
				itemMoreGames.free = (int)js.GetField("moregames").list[j].GetField("free").n;
				api.moregames.Add(itemMoreGames);
			}
			catch (Exception)
			{
			}
		}
		api.HasNewNews = (int)js.GetField("HasNewNews").n;
		api.HasNewUpdates = (int)js.GetField("HasNewUpdates").n;
		api.HasNewVersionAvailable = (int)js.GetField("newVersionAvailable").n;
		Promo promo = new Promo();
		promo.promoId = (int)js.GetField("promoId").n;
		promo.promoTitle = js.GetField("promoTitle").str;
		promo.promoDesc = js.GetField("promoDesc").str;
		promo.promoUrl = js.GetField("promoUrl").str;
		promo.promoIconUrl = js.GetField("promoIconUrl").str;
		promo.promoPName = js.GetField("promoPName").str;
		api.promo.Add(promo);
		promo = new Promo();
		promo.promoId = (int)js.GetField("promoId2").n;
		promo.promoTitle = js.GetField("promoTitle2").str;
		promo.promoDesc = js.GetField("promoDesc2").str;
		promo.promoUrl = js.GetField("promoUrl2").str;
		promo.promoIconUrl = js.GetField("promoIconUrl2").str;
		promo.promoPName = js.GetField("promoPName2").str;
		api.promo.Add(promo);
		promo = new Promo();
		promo.promoId = (int)js.GetField("promoId3").n;
		promo.promoTitle = js.GetField("promoTitle3").str;
		promo.promoDesc = js.GetField("promoDesc3").str;
		promo.promoUrl = js.GetField("promoUrl3").str;
		promo.promoIconUrl = js.GetField("promoIconUrl3").str;
		promo.promoPName = js.GetField("promoPName3").str;
		api.promo.Add(promo);
		api.appodayId = (int)js.GetField("appodayId").n;
		api.appodayName = js.GetField("appodayName").str;
		api.appodayUrl = js.GetField("appodayUrl").str;
		api.appodayIconUrl = js.GetField("appodayIconUrl").str;
		if (uirootMmusia != null)
		{
			uirootMmusia.BroadcastMsgMMUSIA("MMUSIA_JsonApiLoaded");
		}
	}

	private void launchCountIncrement()
	{
		PlayerPrefs.SetInt("MMUSIA_launchCount", getlaunchCount() + 1);
		PlayerPrefs.Save();
	}

	private int getlaunchCount()
	{
		return PlayerPrefs.GetInt("MMUSIA_launchCount", 0);
	}

	private int getLatestPromoIDPref1()
	{
		return PlayerPrefs.GetInt("MMUSIA_LatestPromoIDPref1");
	}

	private int getLatestPromoIDPref2()
	{
		return PlayerPrefs.GetInt("MMUSIA_LatestPromoIDPref2");
	}

	private int getLatestPromoIDPref3()
	{
		return PlayerPrefs.GetInt("MMUSIA_LatestPromoIDPref3");
	}

	private void setLatestPromoIDPref1(int id)
	{
		PlayerPrefs.SetInt("MMUSIA_LatestPromoIDPref1", id);
		PlayerPrefs.Save();
	}

	private void setLatestPromoIDPref2(int id)
	{
		PlayerPrefs.SetInt("MMUSIA_LatestPromoIDPref2", id);
		PlayerPrefs.Save();
	}

	private void setLatestPromoIDPref3(int id)
	{
		PlayerPrefs.SetInt("MMUSIA_LatestPromoIDPref3", id);
		PlayerPrefs.Save();
	}

	private void resetPromoCount()
	{
		PlayerPrefs.SetInt("MMUSIA_promoCount", 0);
		PlayerPrefs.Save();
	}

	private void promoCountIncrement()
	{
		PlayerPrefs.SetInt("MMUSIA_promoCount", getPromoCount() + 1);
		PlayerPrefs.Save();
	}

	private int getPromoCount()
	{
		return PlayerPrefs.GetInt("MMUSIA_promoCount");
	}

	public void resetALL()
	{
		setLatestPromoIDPref1(0);
		setLatestPromoIDPref2(0);
		setLatestPromoIDPref3(0);
		resetPromoCount();
		PlayerPrefs.SetInt("MMUSIA_launchCount", 0);
		PlayerPrefs.Save();
	}

	public void loadIcon(string url, UITexture s)
	{
		loadIcon(url, s, false);
	}

	public void loadIcon(string url, UITexture s, bool appOfDay)
	{
		MMUSIAImgCacheManager.Instance.getImage(url, s, appOfDay);
	}

	public void showNews()
	{
		if (api != null && !newsLoaded)
		{
			newsLoaded = true;
			checkUIRoot();
			uirootMmusia.DisplayNews();
		}
	}

	public void showMoreGames()
	{
		if (api != null && !moreGamesLoaded)
		{
			moreGamesLoaded = true;
			checkUIRoot();
			uirootMmusia.DisplayMoreGames();
		}
	}

	public void notifPromoDialogThread(int id)
	{
		id--;
		promoInfo = new PromoInfo();
		promoInfo.title = api.promo[id].promoTitle;
		promoInfo.description = api.promo[id].promoDesc;
		promoInfo.iconUrl = api.promo[id].promoIconUrl;
		promo_id = api.promo[id].promoId;
		promo_url = api.promo[id].promoUrl;
		StartCoroutine(showNotifLate());
	}

	private IEnumerator showNotifLate()
	{
		yield return new WaitForSeconds(0.1f);
		checkUIRoot();
		uirootMmusia.DisplayPromoDialog();
	}

	private void Update()
	{
		if (reset_prefs)
		{
			reset_prefs = false;
			resetALL();
		}
		if (!ready)
		{
			return;
		}
		ready = false;
		if (api.HasNewNews != 1 || notifAllowed)
		{
		}
		if (api.HasNewUpdates != 1 || notifAllowed)
		{
		}
		if (!notifAllowed || getlaunchCount() <= 1)
		{
			return;
		}
		bool flag = false;
		if (api.promo[0].promoId > 0 && api.promo[0].promoId != getLatestPromoIDPref1())
		{
			if (!isPackageNameInDevice(api.promo[0].promoPName))
			{
				resetPromoCount();
				flag = true;
				notifPromoDialogThread(1);
				setLatestPromoIDPref1(api.promo[0].promoId);
				setLatestPromoIDPref2(0);
				setLatestPromoIDPref3(0);
			}
			else
			{
				resetPromoCount();
				setLatestPromoIDPref1(api.promo[0].promoId);
				flag = false;
			}
		}
		if (api.promo[1].promoId > 0 && !flag && getPromoCount() % 3 == 0 && api.promo[1].promoId != getLatestPromoIDPref2())
		{
			if (!isPackageNameInDevice(api.promo[1].promoPName))
			{
				resetPromoCount();
				flag = true;
				notifPromoDialogThread(2);
				setLatestPromoIDPref2(api.promo[1].promoId);
			}
			else
			{
				resetPromoCount();
				setLatestPromoIDPref2(api.promo[1].promoId);
				flag = false;
			}
		}
		if (api.promo[2].promoId > 0 && !flag && getPromoCount() % 3 == 0 && api.promo[2].promoId != getLatestPromoIDPref3())
		{
			if (!isPackageNameInDevice(api.promo[2].promoPName))
			{
				resetPromoCount();
				flag = true;
				notifPromoDialogThread(3);
				setLatestPromoIDPref3(api.promo[2].promoId);
			}
			else
			{
				resetPromoCount();
				setLatestPromoIDPref3(api.promo[2].promoId);
				flag = false;
			}
		}
		promoCountIncrement();
	}

	private IEnumerator ignoreResponse(WWW www)
	{
		yield return www;
	}

	public void DialogDownload()
	{
		Application.OpenURL(promo_url);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", getDeviceID());
		wWWForm.AddField("prid", promo_id);
		wWWForm.AddField("pn", getAppPackageName());
		wWWForm.AddField("a", "clickpromo");
		wWWForm.AddField("dlg", "1");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip, deflate");
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(ignoreResponse(www));
	}

	public void moreGamesDownload(MMUSIAMoreGameItem ns)
	{
		Application.OpenURL(ns.url);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", getDeviceID());
		wWWForm.AddField("pn", getAppPackageName());
		wWWForm.AddField("pn2", ns.pname);
		wWWForm.AddField("a", "moregame");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip, deflate");
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(ignoreResponse(www));
	}

	public void newsDownload(MMUSIANewsItem ns)
	{
		int id = ns.id;
		Application.OpenURL(ns.url);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("di", getDeviceID());
		wWWForm.AddField("nid", id);
		wWWForm.AddField("a", "click");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Accept-Encoding", "gzip, deflate");
		WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
		StartCoroutine(ignoreResponse(www));
	}

	public void onClickAppofDay()
	{
		string appodayUrl = api.appodayUrl;
		if (!string.IsNullOrEmpty(appodayUrl))
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("di", getDeviceID());
			wWWForm.AddField("aodid", api.appodayId);
			wWWForm.AddField("pn", getAppPackageName());
			wWWForm.AddField("a", "appoday");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Accept-Encoding", "gzip, deflate");
			WWW www = new WWW(apiUrl, wWWForm.data, dictionary);
			StartCoroutine(ignoreResponse(www));
			Application.OpenURL(appodayUrl);
		}
	}
}
