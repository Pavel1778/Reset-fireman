using System;
using System.Collections.Generic;
using AndroidPlugin;
using UnityEngine;

public class PluginScript : MonoBehaviour
{
	public delegate void PluginInAppDelegate(bool buy, InAppType type, string idInApp);

	public delegate void PluginInAppPriceDelegate(string idInApp, string price);

	public delegate void PluginCloudDelegate(string keyvalue);

	public delegate void PluginFinishCloudDelegate();

	private static PluginScript instance;

	private bool mmusiab = true;

	private bool inApp = true;

	private bool tracker = true;

	public bool dontCloud;

	public bool gameCenter;

	public bool pub;

	public bool cloud;

	public List<InApp> listInApp = new List<InApp>();

	public List<Pub> listPub = new List<Pub>();

	public List<PubAndroid> listPubAndroid = new List<PubAndroid>();

	public string idenTrackerIOS;

	public string idenTrackerAndroid;

	public string idApp;

	public string InMobiAppId;

	private GameObject targetSIA;

	private string callbackSIA;

	private string pathSIA;

	public List<Achievement> listAchievements = new List<Achievement>();

	public List<Leaderboard> listLeaderboards = new List<Leaderboard>();

	public string GoogleApplicationId;

	public bool GooglePlayGamesLogged;

	public static PluginScript Instance
	{
		get
		{
			return instance;
		}
	}

	public event PluginInAppDelegate InAppTransaction;

	public event PluginInAppPriceDelegate InAppPrice;

	public event PluginCloudDelegate CloudChange;

	public event PluginFinishCloudDelegate CloudFinish;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		setDebug(true);
		Core.Initialize(new MyEventHandler());
		Adskit.UseHeader = true;
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin1");
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin2");
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin3");
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin4");
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin5");
		Inapp.addProduct("com.magmamobile.game.fireman.extraskin6");
		Inapp.addProduct("com.magmamobile.game.fireman.pet1");
		Inapp.addProduct("com.magmamobile.game.fireman.pet2");
		Inapp.addProduct("com.magmamobile.game.fireman.pet3");
		Inapp.addProduct("com.magmamobile.game.fireman.pet4");
		Inapp.addProduct("com.magmamobile.game.fireman.pet5");
		Inapp.addProduct("com.magmamobile.game.fireman.pet6");
		Inapp.addProduct("com.magmamobile.game.fireman.pet7");
		Inapp.addProduct("com.magmamobile.game.fireman.pet8");
		Inapp.addProduct("com.magmamobile.game.fireman.extralevel1");
		Inapp.addProduct("com.magmamobile.game.fireman.extralevel2");
		Inapp.addProduct("com.magmamobile.game.fireman.extralevel3");
		Inapp.addProduct("com.magmamobile.game.fireman.doublexp");
		Inapp.addProduct("com.magmamobile.game.fireman.doublemoney");
		Inapp.addProduct("com.magmamobile.game.fireman.doublexpmoney");
		Inapp.addProduct("com.magmamobile.game.fireman.all");
		if (mmusiab)
		{
			initMMusia();
		}
		if (tracker)
		{
		}
		if (gameCenter)
		{
			initGameCenter();
		}
		if (pub)
		{
			listPubAndroid.Clear();
			if (isTablet())
			{
				newPub("ca-app-pub-6807707624701585/9662873016", AdType.BANNER, (AdPos)40);
			}
			else
			{
				newPub("ca-app-pub-6807707624701585/8186139817", AdType.BANNER, (AdPos)40);
			}
			newPub("ca-app-pub-6807707624701585/2139606217", AdType.SQUARE, (AdPos)20);
			foreach (PubAndroid item in listPubAndroid)
			{
				initPub(item.identifier, item.type, item.posX | item.posY);
			}
			Interstial.Initialize();
		}
		if (string.IsNullOrEmpty(idApp))
		{
			Debug.LogWarning("idApp vide, le ask4rate ne fonctionnera pas");
		}
		changeDefaultPub(0);
	}

	public void newPub(string identifier, AdType type, AdPos pos)
	{
		PubAndroid pubAndroid = new PubAndroid();
		pubAndroid.identifier = identifier;
		pubAndroid.type = type;
		pubAndroid.posX = pos;
		listPubAndroid.Add(pubAndroid);
	}

	public static bool isTablet()
	{
		try
		{
			float num = (float)Screen.width / Screen.dpi;
			float num2 = (float)Screen.height / Screen.dpi;
			double num3 = Mathf.Sqrt(num * num + num2 * num2);
			return num3 >= 6.0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public void setDebug(bool value)
	{
	}

	public void resetCloud()
	{
	}

	public void initMMusia()
	{
		if (mmusiab)
		{
		}
	}

	public void showMoreGames()
	{
		if (mmusiab)
		{
			Mmusia.showMoreGames();
		}
	}

	public void showOtherGames()
	{
		if (mmusiab)
		{
			Mmusia.showOtherGames();
		}
	}

	public void showNews()
	{
		if (mmusiab)
		{
			Mmusia.showNews();
		}
	}

	public void showPromo()
	{
		if (mmusiab)
		{
		}
	}

	public void startAsk4Rate()
	{
		Mmusia.startAsk4Rate();
	}

	public void showAsk4Rate()
	{
		Mmusia.showAsk4Rate();
	}

	public void rate()
	{
		Core.showGooglePlayGamePage();
	}

	public void initPub(string adKey, AdType adType, AdPos adPos)
	{
		if (pub && AppConfig.Platform != AppConfig.PlatformEnum.AndroidOuya && !Core.IsFunTab2())
		{
			if (adType.Equals(AdType.BANNER))
			{
				Adskit.addAdView(adKey, Adskit.AdProvider.ADMOBMEDIATION, Adskit.AdType.BANNER, (Adskit.AdPos)40);
			}
			else
			{
				Adskit.addAdView(adKey, Adskit.AdProvider.ADMOBMEDIATION, Adskit.AdType.SQUARE, (Adskit.AdPos)20);
			}
		}
	}

	public void showPub(int idPub)
	{
		if (pub && AppConfig.Platform != AppConfig.PlatformEnum.AndroidOuya && !Core.IsFunTab2())
		{
			Adskit.showAds(idPub);
		}
	}

	public void hidePub()
	{
		if (pub && AppConfig.Platform != AppConfig.PlatformEnum.AndroidOuya && !Core.IsFunTab2())
		{
			for (int i = 0; i < listPubAndroid.Count; i++)
			{
				Adskit.hideAds(i);
			}
		}
	}

	public void movePub(AdPos adPos)
	{
		if (pub)
		{
		}
	}

	public void changeDefaultPub(int idPub)
	{
		if (pub)
		{
		}
	}

	public void initTracker(string idTracker)
	{
		if (tracker)
		{
		}
	}

	public void track(string track)
	{
		if (tracker)
		{
			Analytics.trackAndDispatch(track);
		}
	}

	public void initGameCenter()
	{
		if (gameCenter)
		{
		}
	}

	public void resetAchievement()
	{
		if (gameCenter)
		{
		}
	}

	public void submitAchievementIOS(int idAchievement, int percent)
	{
		if (gameCenter)
		{
		}
	}

	public void submitAchievementAndroid(int idAchievement)
	{
		if (gameCenter && GameServices.isSignedIn())
		{
			GameServices.Achievements.unlock(listAchievements[idAchievement].idAndroid);
		}
	}

	public void submitAchievementAndroidIncrement(int idAchievement, int value)
	{
		if (gameCenter && GameServices.isSignedIn())
		{
		}
	}

	public int getAchievementIncrementalActualNumber(int idAchievement)
	{
		if (!gameCenter)
		{
			return -1;
		}
		return -1;
	}

	public void GPGSLogin()
	{
		GameServices.signIn();
	}

	public void GPGSLogout()
	{
		GameServices.signOut();
	}

	public void submitScore(int idLeaderboard, int score)
	{
		if (gameCenter)
		{
			GameServices.Leaderboards.submitScore(listLeaderboards[idLeaderboard].idAndroid, score);
		}
	}

	public void showLeaderboardAchievements()
	{
		if (gameCenter)
		{
		}
	}

	public void showAchievements()
	{
		GameServices.Achievements.show();
	}

	public void showLeaderboards()
	{
		GameServices.Leaderboards.showAll();
	}

	public bool isGameCenterAvailable()
	{
		if (!gameCenter)
		{
			return false;
		}
		return false;
	}

	public void initInApp()
	{
		if (inApp)
		{
		}
	}

	public void addInApp(string inAppIden)
	{
		if (inApp)
		{
		}
	}

	public void startInApp()
	{
		if (inApp)
		{
		}
	}

	public void retrieveInfoInApp()
	{
		if (inApp)
		{
		}
	}

	public void showBuyFeature(string featureID, string title, string desc)
	{
		if (inApp)
		{
			Inapp.purchaseItem(featureID.ToLower(), string.Empty);
		}
	}

	public void restorePurchases()
	{
		if (inApp)
		{
			Inapp.loadPurchaseDetails();
		}
	}

	public void share(string msg, string subject, string title, string facebookurl)
	{
		Core.share(msg, subject, title, facebookurl);
	}

	public void saveImageInAlbum(string path, GameObject target, string callback)
	{
		targetSIA = target;
		callbackSIA = callback;
		pathSIA = path;
	}

	private void OnFinishSaveImageInAlbum(string n)
	{
		if (targetSIA != null)
		{
			targetSIA.SendMessage(callbackSIA, pathSIA, SendMessageOptions.DontRequireReceiver);
		}
		targetSIA = null;
	}

	public void showWeb(string title, string url)
	{
		Application.OpenURL(url);
	}

	public void initCloud()
	{
		if (cloud)
		{
		}
	}

	public void saveBoolForCloud(string key, bool val)
	{
		if (cloud && !dontCloud)
		{
			GameServices.AppState.updateImmediate(0, VarsManager.Instance.createDataAndroidCloud());
		}
	}

	public void saveIntForCloud(string key, int val)
	{
		if (cloud && !dontCloud)
		{
			GameServices.AppState.updateImmediate(0, VarsManager.Instance.createDataAndroidCloud());
		}
	}

	public void saveStringForCloud(string key, string val)
	{
		if (cloud && !dontCloud)
		{
			GameServices.AppState.updateImmediate(0, VarsManager.Instance.createDataAndroidCloud());
		}
	}

	public void saveBytesForCloud(string key, byte[] data, int length)
	{
		if (cloud && !dontCloud)
		{
			GameServices.AppState.updateImmediate(0, VarsManager.Instance.createDataAndroidCloud());
		}
	}

	public string getPName()
	{
		return Core.PackageName;
	}

	public void getTypeInApp(string str, out InAppType type, out string identifier)
	{
		string[] array = str.Split('/');
		type = (InAppType)int.Parse(array[0]);
		identifier = array[1];
	}

	public void transactionOk(string idInApp)
	{
		if (InAppTransaction != null)
		{
			string empty = string.Empty;
			empty = idInApp;
			InAppType type = InAppType.InAppBuyed;
			InAppTransaction(true, type, empty);
		}
	}

	public void transactionNok(string idInApp)
	{
		if (InAppTransaction != null)
		{
			string empty = string.Empty;
			empty = idInApp;
			InAppType type = InAppType.InAppFailed;
			InAppTransaction(false, type, empty);
		}
	}

	public void retrievePrice(string idInApp)
	{
		string[] array = idInApp.Split('/');
		foreach (InApp item in listInApp)
		{
			if (item.identifier == array[0])
			{
				item.price = array[1];
				if (InAppPrice != null)
				{
					InAppPrice(array[0], array[1]);
				}
				break;
			}
		}
	}

	public void changeCloudValue(string keyvalue)
	{
		if (CloudChange != null)
		{
			CloudChange(keyvalue);
		}
	}

	public void finishChangeCloud(string keyvalue)
	{
		if (CloudFinish != null)
		{
			CloudFinish();
		}
	}

	public string getK(string str)
	{
		return Core.kryp(str);
	}

	public string getDK(string str)
	{
		return Core.dkryp(str);
	}

	public void openItunes(string s)
	{
		Application.OpenURL(s);
	}

	public void Mmusia_AppOfTheDayReady(string key)
	{
		if ((bool)AppOfTheDayU5.Instance)
		{
			AppOfTheDayU5.Instance.AppOfTheDayReady(key);
		}
	}
}
