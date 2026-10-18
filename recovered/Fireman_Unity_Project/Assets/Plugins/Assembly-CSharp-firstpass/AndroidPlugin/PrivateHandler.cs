using System.Collections.Generic;
using UnityEngine;

namespace AndroidPlugin
{
	internal class PrivateHandler : MonoBehaviour
	{
		public static EventHandler handler;

		public void Mmusia_onAsk4RateClickYes(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Mmusia_onAsk4RateClickYes : " + param);
			}
			if (handler != null)
			{
				handler.Mmusia_onAsk4RateClickYes();
			}
		}

		public void Mmusia_onAsk4RateClickNeverAgain(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Mmusia_onAsk4RateClickNeverAgain : " + param);
			}
			if (handler != null)
			{
				handler.Mmusia_onAsk4RateClickNeverAgain();
			}
		}

		public void Mmusia_onAsk4RateClickLater(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Mmusia_onAsk4RateClickLater : " + param);
			}
			if (handler != null)
			{
				handler.Mmusia_onAsk4RateClickLater();
			}
		}

		public void Mmusia_AppOfTheDayReady(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Mmusia_AppOfTheDayReady : " + param);
			}
			if (handler != null)
			{
				handler.Mmusia_AppOfTheDayReady(param);
			}
		}

		public void GameServices_onSignInFailed(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onSignInFailed : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onSignInFailed();
			}
		}

		public void GameServices_onSignInSucceeded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onSignInSucceeded : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onSignInSucceeded();
			}
		}

		public void GameServices_onSignInCanceled(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onSignInCanceled : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onSignInCanceled();
			}
		}

		public void GameServices_onStateConflict(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onStateConflict : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int stateKey = rawStreamReader.ReadInt32();
				string resolvedVersion = rawStreamReader.ReadString();
				byte[] localData = rawStreamReader.ReadByteArray();
				byte[] serverData = rawStreamReader.ReadByteArray();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onStateConflict(stateKey, resolvedVersion, localData, serverData);
				}
			}
		}

		public void GameServices_onStateLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onStateLoaded : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int statusCode = rawStreamReader.ReadInt32();
				int stateKey = rawStreamReader.ReadInt32();
				byte[] data = rawStreamReader.ReadByteArray();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onStateLoaded(statusCode, stateKey, data);
				}
			}
		}

		public void GameServices_onAchievementsLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onAchievementsLoaded : " + param);
			}
			List<GameServices.Achievements.Achievement> list = new List<GameServices.Achievements.Achievement>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int statusCode = rawStreamReader.ReadInt32();
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadAchievement());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onAchievementsLoaded(statusCode, list);
				}
			}
		}

		public void Inapp_onBillingSupported(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Inapp_onBillingSupported : " + param);
			}
			if (handler != null)
			{
				handler.Inapp_onBillingSupported(param);
			}
		}

		public void Inapp_onProductDetails(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Inapp_onSkuDetails : " + param);
			}
			List<Inapp.ProductDetails> list = new List<Inapp.ProductDetails>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadSkuDetails());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.Inapp_onProductDetails(list);
				}
			}
		}

		public void Inapp_onPurchaseDetails(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Inapp_onPurcaseDetails : " + param);
			}
			List<Inapp.PurchaseDetails> list = new List<Inapp.PurchaseDetails>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadPurchaseDetails());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.Inapp_onPurchaseDetails(list);
				}
			}
		}

		public void Inapp_onPurchase(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Inapp_onPurchase : " + param);
			}
			Inapp.PurchaseDetails purchaseDetails = null;
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				purchaseDetails = rawStreamReader.ReadPurchaseDetails();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.Inapp_onPurchase(purchaseDetails);
				}
			}
		}

		public void Adskit_onInterstitialAdLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onInterstitialAdLoaded : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onInterstitialAdLoaded(int.Parse(param));
			}
		}

		public void Adskit_onInterstitialAdClosed(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onInterstitialAdClosed : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onInterstitialAdClosed(int.Parse(param));
			}
		}

		public void Adskit_onVideoLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoLoaded : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoLoaded(int.Parse(param));
			}
		}

		public void Adskit_onVideoClosed(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoClosed : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoClosed(int.Parse(param));
			}
		}

		public void Adskit_onVideoReward(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoReward : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoReward(int.Parse(param));
			}
		}

		public void Adskit_onVideoAdFailedToLoad(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoAdFailedToLoad : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoAdFailedToLoad(int.Parse(param));
			}
		}

		public void Adskit_onVideoAdLeftApplication(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoAdLeftApplication : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoAdLeftApplication(int.Parse(param));
			}
		}

		public void Adskit_onVideoAdOpened(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoAdOpened : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoAdOpened(int.Parse(param));
			}
		}

		public void Adskit_onVideoStarted(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Adskit_onVideoStarted : " + param);
			}
			if (handler != null)
			{
				handler.Adskit_onVideoStarted(int.Parse(param));
			}
		}

		public void Core_onAlertsLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> Core_onAlertsLoaded : " + param);
			}
			List<Alerts.Alert> list = new List<Alerts.Alert>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadAlert());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.Core_onAlertsLoaded(list);
				}
			}
		}

		public void GameServices_onQuestEventResult(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onQuestEventResult : " + param);
			}
			List<GameServices.Events.Event> list = new List<GameServices.Events.Event>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int statusCode = 0;
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadEvent());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onQuestEventResult(statusCode, list);
				}
			}
		}

		public void GameServices_onQuestCompleted(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onQuestCompleted : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				GameServices.Quests.Quest quest = rawStreamReader.ReadQuest();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onQuestCompleted(quest);
				}
			}
		}

		public void GameServices_onQuestsClicked(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onQuestsClicked : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				GameServices.Quests.Quest quest = rawStreamReader.ReadQuest();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onQuestsClicked(quest);
				}
			}
		}

		public void GameServices_onQuestsLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onQuestsLoaded : " + param);
			}
			List<GameServices.Quests.Quest> list = new List<GameServices.Quests.Quest>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int statusCode = rawStreamReader.ReadInt32();
				int num = rawStreamReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadQuest());
				}
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onQuestsLoaded(statusCode, list);
				}
			}
		}

		public void GameServices_onSnapshotLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onSnapshotLoaded : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				byte[] data = rawStreamReader.ReadByteArray();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onSnapshotLoaded(data);
				}
			}
		}

		public void GameServices_onRequestRemoved(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onRequestRemoved : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onRequestRemoved(param);
			}
		}

		public void GameServices_onRequestReceived(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onRequestReceived : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				GameServices.Requests.GameRequest request = rawStreamReader.ReadGameRequest();
				rawStreamReader.Close();
				if (handler != null)
				{
					handler.GameServices_onRequestReceived(request);
				}
			}
		}

		public void GameServices_onRequestSent(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onRequestSent : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onRequestSent(int.Parse(param));
			}
		}

		public void GameServices_onRequestUpdated(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onRequestUpdated : " + param);
			}
			List<GameServices.Requests.GameRequest> list = new List<GameServices.Requests.GameRequest>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int num = rawStreamReader.ReadInt16();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadGameRequest());
				}
			}
			if (handler != null)
			{
				handler.GameServices_onRequestUpdated(list);
			}
		}

		public void GameServices_onLeaderboardsScoresLoaded(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onLeaderboardsScoresLoaded : " + param);
			}
			List<GameServices.Leaderboards.Score> list = new List<GameServices.Leaderboards.Score>();
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				int statusCode = rawStreamReader.ReadInt32();
				int num = rawStreamReader.ReadInt16();
				for (int i = 0; i < num; i++)
				{
					list.Add(rawStreamReader.ReadScore());
				}
				if (handler != null)
				{
					handler.GameServices_onLeaderboardsScoresLoaded(statusCode, list);
				}
			}
		}

		public void GameServices_onNearbyMessage(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onNearbyMessage : " + param);
			}
			using (RawStreamReader rawStreamReader = new RawStreamReader(param))
			{
				byte[] data = rawStreamReader.ReadByteArray();
				if (handler != null)
				{
					handler.GameServices_onNearbyMessage(data);
				}
			}
		}

		public void GameServices_onNearbyStatusChanged(string param)
		{
			if (Core.DEBUG)
			{
				Debug.Log(">> GameServices_onNearbyStatusChanged : " + param);
			}
			if (handler != null)
			{
				handler.GameServices_onNearbyStatusChanged((GameServices.Nearby.Status)int.Parse(param));
			}
		}
	}
}
