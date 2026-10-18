using AndroidPlugin;

public class Interstial
{
	private static bool isInitialized;

	private static bool isReady;

	public static void Initialize()
	{
		isInitialized = true;
		Adskit.addInterstitialAd("ca-app-pub-6807707624701585/3969719014");
	}

	public static void Preload()
	{
		if (isInitialized && !VarsManager.Instance.hideAllPub() && !isReady)
		{
			Adskit.preloadInterstitialAd(0);
		}
	}

	public static void Show()
	{
		if (isInitialized && !VarsManager.Instance.hideAllPub() && isReady)
		{
			Adskit.showInterstitialAd(0);
		}
	}

	public static void NotifyClosed()
	{
		isReady = false;
		Preload();
	}

	public static void NotifyLoaded()
	{
		isReady = true;
	}
}
