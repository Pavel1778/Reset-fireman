using System;
using System.IO;
using System.Text;

namespace AndroidPlugin
{
	internal class RawStreamReader : MemoryStream
	{
		public RawStreamReader(byte[] input)
			: base(input)
		{
		}

		public RawStreamReader(string base64)
			: base(Convert.FromBase64String(base64))
		{
		}

		public bool ReadBoolean()
		{
			return ReadByte() != 0;
		}

		public int Read()
		{
			return ReadByte();
		}

		public short ReadInt16()
		{
			int num = ReadByte() & 0xFF;
			int num2 = ReadByte() << 8;
			return (short)(num | num2);
		}

		public int ReadInt32()
		{
			int num = ReadByte() & 0xFF;
			int num2 = ReadByte() << 8;
			int num3 = ReadByte() << 16;
			int num4 = ReadByte() << 24;
			return num | num2 | num3 | num4;
		}

		public long ReadInt64()
		{
			int num = ReadByte() & 0xFF;
			int num2 = ReadByte() << 8;
			int num3 = ReadByte() << 16;
			int num4 = ReadByte() << 24;
			int num5 = ReadByte() & 0xFF;
			int num6 = ReadByte() << 8;
			int num7 = ReadByte() << 16;
			int num8 = ReadByte() << 24;
			uint num9 = (uint)(num | num2 | num3 | num4);
			uint num10 = (uint)(num5 | num6 | num7 | num8);
			return (long)(((ulong)num10 << 32) | num9);
		}

		public ulong ReadUInt64()
		{
			int num = ReadByte() & 0xFF;
			int num2 = ReadByte() << 8;
			int num3 = ReadByte() << 16;
			int num4 = ReadByte() << 24;
			int num5 = ReadByte() & 0xFF;
			int num6 = ReadByte() << 8;
			int num7 = ReadByte() << 16;
			int num8 = ReadByte() << 24;
			uint num9 = (uint)(num | num2 | num3 | num4);
			uint num10 = (uint)(num5 | num6 | num7 | num8);
			return ((ulong)num10 << 32) | num9;
		}

		public string ReadString()
		{
			int num = ReadInt32();
			if (num > 0)
			{
				byte[] array = new byte[num];
				Read(array, 0, num);
				return Encoding.UTF8.GetString(array);
			}
			return string.Empty;
		}

		public byte[] ReadByteArray()
		{
			int size = ReadInt32();
			return ReadBytes(size);
		}

		public byte[] ReadBytes(int size)
		{
			if (size > 0)
			{
				byte[] array = new byte[size];
				Read(array, 0, size);
				return array;
			}
			return null;
		}

		public Alerts.Alert ReadAlert()
		{
			Alerts.Alert alert = new Alerts.Alert();
			alert.time = ReadInt64();
			alert.title = ReadString();
			alert.content = ReadString();
			alert.trackPage = ReadString();
			return alert;
		}

		public Inapp.ProductDetails ReadSkuDetails()
		{
			Inapp.ProductDetails productDetails = new Inapp.ProductDetails();
			productDetails.productId = ReadString();
			productDetails.type = ReadString();
			productDetails.price = ReadString();
			productDetails.price_amount_micros = ReadString();
			productDetails.price_currency_code = ReadString();
			productDetails.title = ReadString();
			productDetails.description = ReadString();
			return productDetails;
		}

		public Inapp.PurchaseDetails ReadPurchaseDetails()
		{
			Inapp.PurchaseDetails purchaseDetails = new Inapp.PurchaseDetails();
			purchaseDetails.autoRenewing = ReadString();
			purchaseDetails.orderId = ReadString();
			purchaseDetails.packageName = ReadString();
			purchaseDetails.productId = ReadString();
			purchaseDetails.purchaseTime = ReadString();
			purchaseDetails.purchaseState = ReadString();
			purchaseDetails.developerPayload = ReadString();
			purchaseDetails.purchaseToken = ReadString();
			purchaseDetails.verified = ReadBoolean();
			return purchaseDetails;
		}

		public GameServices.Achievements.Achievement ReadAchievement()
		{
			GameServices.Achievements.Achievement achievement = new GameServices.Achievements.Achievement();
			achievement.id = ReadString();
			achievement.name = ReadString();
			achievement.description = ReadString();
			achievement.type = (GameServices.Achievements.Achievement.Type)ReadInt32();
			achievement.state = (GameServices.Achievements.Achievement.State)ReadInt32();
			if (achievement.type == GameServices.Achievements.Achievement.Type.TYPE_INCREMENTAL)
			{
				achievement.totalSteps = ReadInt32();
				achievement.currentSteps = ReadInt32();
			}
			return achievement;
		}

		public GameServices.Events.Event ReadEvent()
		{
			GameServices.Events.Event obj = new GameServices.Events.Event();
			obj.id = ReadString();
			obj.name = ReadString();
			obj.description = ReadString();
			obj.value = ReadInt64();
			return obj;
		}

		public GameServices.Requests.GameRequest ReadGameRequest()
		{
			GameServices.Requests.GameRequest gameRequest = new GameServices.Requests.GameRequest();
			gameRequest.status = (GameServices.Requests.Status)ReadInt32();
			gameRequest.type = (GameServices.Requests.Type)ReadInt32();
			gameRequest.requestId = ReadString();
			gameRequest.creationTimestamp = ReadInt64();
			gameRequest.expirationTimestamp = ReadInt64();
			gameRequest.data = ReadByteArray();
			gameRequest.mine = ReadBoolean();
			return gameRequest;
		}

		public GameServices.Leaderboards.Score ReadScore()
		{
			GameServices.Leaderboards.Score score = new GameServices.Leaderboards.Score();
			score.displayRank = ReadString();
			score.displayScore = ReadString();
			score.scoreHolderDisplayName = ReadString();
			score.scoreHolderHiResImageUrl = ReadString();
			score.scoreHolderIconImageUrl = ReadString();
			score.scoreTag = ReadString();
			score.timestampMillis = ReadInt64();
			score.mine = ReadBoolean();
			return score;
		}

		public GameServices.Quests.Quest ReadQuest()
		{
			GameServices.Quests.Quest quest = new GameServices.Quests.Quest();
			quest.id = ReadString();
			quest.name = ReadString();
			quest.description = ReadString();
			quest.type = ReadInt32();
			quest.state = (GameServices.Quests.Quest.State)ReadInt32();
			quest.isEndingSoon = ReadBoolean();
			quest.startTimeStamp = ReadInt64();
			quest.endTimeStamp = ReadInt64();
			quest.acceptedTimeStamp = ReadInt64();
			GameServices.Quests.Milestone milestone = new GameServices.Quests.Milestone();
			milestone.milestoneId = ReadString();
			milestone.eventId = ReadString();
			milestone.state = (GameServices.Quests.Milestone.State)ReadInt32();
			milestone.targetProgress = ReadInt64();
			milestone.currentProgress = ReadInt64();
			milestone.reward = ReadByteArray();
			quest.currentMilestone = milestone;
			return quest;
		}
	}
}
