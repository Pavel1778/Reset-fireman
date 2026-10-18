using System;
using System.Collections.Generic;
using AndroidPlugin;
using UnityEngine;

public class MyEventHandler : AndroidPlugin.EventHandler
{
	public static bool[] keyStates = new bool[8];

	public static bool[] keyLast = new bool[8];

	public override void Mmusia_AppOfTheDayReady(string key)
	{
		GameObject gameObject = GameObject.Find("NGUIButtonAppOfTheDayAndroid");
		if (gameObject != null)
		{
			gameObject.GetComponent<AppOfTheDay>().AppOfTheDayReady(key);
		}
	}

	public override void GameServices_onSignInFailed()
	{
		try
		{
			MenusManager.Instance.switchButtonGameServicesAndroid();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public override void GameServices_onSignInSucceeded()
	{
		try
		{
			MenusManager.Instance.switchButtonGameServicesAndroidToLogged();
			GameServices.AppState.load(0);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public override void GameServices_onStateConflict(int stateKey, string resolvedVersion, byte[] localData, byte[] serverData)
	{
		VarsManager.Instance.mergeDataAndroidCloud(serverData);
	}

	public override void GameServices_onStateLoaded(int statusCode, int stateKey, byte[] data)
	{
		VarsManager.Instance.mergeDataAndroidCloud(data);
	}

	public override void Inapp_onBillingSupported(string yesNo)
	{
		InAppManager.Instance.inAppReady = yesNo == "yes";
	}

	public override void Inapp_onPurchaseDetails(List<Inapp.PurchaseDetails> data)
	{
		foreach (Inapp.PurchaseDetails datum in data)
		{
			Inapp_onPurchase(datum);
		}
	}

	public override void Inapp_onPurchase(Inapp.PurchaseDetails data)
	{
		if (data.purchaseState == "0")
		{
			InAppManager.Instance.OnInAppBuyOK(data.productId);
		}
		else if (data.purchaseState == "2")
		{
			InAppManager.Instance.OnInAppBuyRefunded(data.productId);
		}
		else
		{
			InAppManager.Instance.OnInAppBuyNOK(data.productId);
		}
	}

	public override void Adskit_onInterstitialAdClosed(int index)
	{
		Interstial.NotifyClosed();
	}

	public override void Adskit_onInterstitialAdLoaded(int index)
	{
		Interstial.NotifyLoaded();
	}

	public override void GameServices_onNearbyStatusChanged(GameServices.Nearby.Status status)
	{
	}

	public override void GameServices_onNearbyMessage(byte[] data)
	{
		if (data != null && data.Length >= 1)
		{
			switch (data[0])
			{
			case 1:
				keyStates[0] = true;
				break;
			case 2:
				keyStates[1] = true;
				break;
			case 3:
				keyStates[2] = true;
				break;
			case 4:
				keyStates[3] = true;
				break;
			case 5:
				keyStates[4] = true;
				break;
			case 6:
				keyStates[5] = true;
				break;
			case 7:
				keyStates[6] = true;
				break;
			case 8:
				keyStates[0] = false;
				break;
			case 9:
				keyStates[1] = false;
				break;
			case 10:
				keyStates[2] = false;
				break;
			case 11:
				keyStates[3] = false;
				break;
			case 12:
				keyStates[4] = false;
				break;
			case 13:
				keyStates[5] = false;
				break;
			case 14:
				keyStates[6] = false;
				break;
			}
		}
	}
}
