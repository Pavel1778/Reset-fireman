using System.Collections.Generic;

namespace AndroidPlugin
{
	public abstract class EventHandler
	{
		public virtual void Mmusia_onAsk4RateClickYes()
		{
		}

		public virtual void Mmusia_onAsk4RateClickNeverAgain()
		{
		}

		public virtual void Mmusia_onAsk4RateClickLater()
		{
		}

		public virtual void Mmusia_AppOfTheDayReady(string key)
		{
		}

		public virtual void Inapp_onBillingSupported(string yesNo)
		{
		}

		public virtual void Inapp_onProductDetails(List<Inapp.ProductDetails> data)
		{
		}

		public virtual void Inapp_onPurchaseDetails(List<Inapp.PurchaseDetails> data)
		{
		}

		public virtual void Inapp_onPurchase(Inapp.PurchaseDetails data)
		{
		}

		public virtual void Adskit_onInterstitialAdLoaded(int index)
		{
		}

		public virtual void Adskit_onInterstitialAdClosed(int index)
		{
		}

		public virtual void Adskit_onVideoLoaded(int index)
		{
		}

		public virtual void Adskit_onVideoClosed(int index)
		{
		}

		public virtual void Adskit_onVideoReward(int index)
		{
		}

		public virtual void Adskit_onVideoAdFailedToLoad(int index)
		{
		}

		public virtual void Adskit_onVideoAdLeftApplication(int index)
		{
		}

		public virtual void Adskit_onVideoAdOpened(int index)
		{
		}

		public virtual void Adskit_onVideoStarted(int index)
		{
		}

		public virtual void GameServices_onSignInFailed()
		{
		}

		public virtual void GameServices_onSignInSucceeded()
		{
		}

		public virtual void GameServices_onSignInCanceled()
		{
		}

		public virtual void GameServices_onStateConflict(int stateKey, string resolvedVersion, byte[] localData, byte[] serverData)
		{
		}

		public virtual void GameServices_onStateLoaded(int statusCode, int stateKey, byte[] data)
		{
		}

		public virtual void GameServices_onAchievementsLoaded(int statusCode, List<GameServices.Achievements.Achievement> data)
		{
		}

		public virtual void GameServices_onQuestCompleted(GameServices.Quests.Quest quest)
		{
		}

		public virtual void GameServices_onQuestEventResult(int statusCode, List<GameServices.Events.Event> data)
		{
		}

		public virtual void GameServices_onQuestsLoaded(int statusCode, List<GameServices.Quests.Quest> data)
		{
		}

		public virtual void GameServices_onQuestsClicked(GameServices.Quests.Quest quest)
		{
		}

		public virtual void GameServices_onSnapshotLoaded(byte[] data)
		{
		}

		public virtual void GameServices_onRequestRemoved(string requestId)
		{
		}

		public virtual void GameServices_onRequestReceived(GameServices.Requests.GameRequest request)
		{
		}

		public virtual void GameServices_onRequestSent(int result)
		{
		}

		public virtual void GameServices_onRequestUpdated(List<GameServices.Requests.GameRequest> requests)
		{
		}

		public virtual void GameServices_onLeaderboardsScoresLoaded(int statusCode, List<GameServices.Leaderboards.Score> scores)
		{
		}

		public virtual void Core_onAlertsLoaded(List<Alerts.Alert> alerts)
		{
		}

		public virtual void GameServices_onNearbyMessage(byte[] data)
		{
		}

		public virtual void GameServices_onNearbyStatusChanged(GameServices.Nearby.Status status)
		{
		}
	}
}
