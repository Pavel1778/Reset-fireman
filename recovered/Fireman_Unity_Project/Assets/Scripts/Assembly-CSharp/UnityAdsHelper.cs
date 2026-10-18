using System;
using AndroidPlugin;
using UnityEngine.Advertisements;

public static class UnityAdsHelper
{
	private static string AppID = "29163";

	private static string ZoneID = "rewardedVideoZone";

	private static bool Testing;

	public static bool IsSupported
	{
		get
		{
			if (Core.IsGoogleTV())
			{
				return false;
			}
			return Advertisement.isSupported;
		}
	}

	public static bool IsReady
	{
		get
		{
			if (Core.IsGoogleTV())
			{
				return false;
			}
			return Advertisement.IsReady(ZoneID);
		}
	}

	public static Action OnAdSkipped { get; set; }

	public static Action OnAdFailed { get; set; }

	public static Action OnAdFinished { get; set; }

	public static void Initialize()
	{
		if (IsSupported)
		{
			Advertisement.Initialize(AppID, Testing);
		}
	}

	public static bool Show()
	{
		if (!Advertisement.isShowing && Advertisement.IsReady(ZoneID))
		{
			ShowOptions showOptions = new ShowOptions();
			showOptions.resultCallback = ShowCallBack;
			Advertisement.Show(ZoneID, showOptions);
			return true;
		}
		return false;
	}

	private static void ShowCallBack(ShowResult r)
	{
		switch (r)
		{
		case ShowResult.Finished:
			if (OnAdFinished != null)
			{
				OnAdFinished();
			}
			break;
		case ShowResult.Failed:
			if (OnAdFailed != null)
			{
				OnAdFailed();
			}
			break;
		case ShowResult.Skipped:
			if (OnAdSkipped != null)
			{
				OnAdSkipped();
			}
			break;
		}
	}
}
