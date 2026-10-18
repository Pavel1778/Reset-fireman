using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class GameServices
	{
		public static class Leaderboards
		{
			public enum TimeSpan
			{
				TIME_SPAN_DAILY = 0,
				TIME_SPAN_WEEKLY = 1,
				TIME_SPAN_ALL_TIME = 2
			}

			public enum Collection
			{
				COLLECTION_PUBLIC = 0,
				COLLECTION_SOCIAL = 1
			}

			public class Score
			{
				public string displayRank;

				public string displayScore;

				public string scoreHolderDisplayName;

				public string scoreHolderHiResImageUrl;

				public string scoreHolderIconImageUrl;

				public string scoreTag;

				public long timestampMillis;

				public bool mine;
			}

			public static void submitScore(string itemid, long score)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "leaderboards_submitScore", "(Ljava/lang/String;J)V");
				jvalue[] array = new jvalue[2];
				array[0].l = AndroidJNI.NewStringUTF(itemid);
				array[1].j = score;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void showAll()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "leaderboards_showAll", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}

			public static void show(string leaderboardId)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "leaderboards_show", "(Ljava/lang/String;)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.NewStringUTF(leaderboardId);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void loadPlayerCenteredScores(string leaderboardId, TimeSpan span, Collection leaderboardCollection, int maxResults)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "leaderboards_loadPlayerCenteredScores", "(Ljava/lang/String;III)V");
				jvalue[] array = new jvalue[4];
				array[0].l = AndroidJNI.NewStringUTF(leaderboardId);
				array[1].i = (int)span;
				array[2].i = (int)leaderboardCollection;
				array[3].i = maxResults;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class Achievements
		{
			public class Achievement
			{
				public enum Type
				{
					TYPE_STANDARD = 0,
					TYPE_INCREMENTAL = 1
				}

				public enum State
				{
					STATE_UNLOCKED = 0,
					STATE_REVEALED = 1,
					STATE_HIDDEN = 2
				}

				public string id;

				public string name;

				public string description;

				public Type type;

				public State state;

				public int totalSteps;

				public int currentSteps;
			}

			public static void unlock(string itemid)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "achievements_unlock", "(Ljava/lang/String;)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.NewStringUTF(itemid);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void increment(string itemid, int numSteps)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "achievements_increment", "(Ljava/lang/String;I)V");
				jvalue[] array = new jvalue[2];
				array[0].l = AndroidJNI.NewStringUTF(itemid);
				array[1].i = numSteps;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void load()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "achievements_load", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}

			public static void show()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "achievements_show", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static class Quests
		{
			public class Select
			{
				public const int SELECT_UPCOMING = 1;

				public const int SELECT_OPEN = 2;

				public const int SELECT_ACCEPTED = 3;

				public const int SELECT_COMPLETED = 4;

				public const int SELECT_EXPIRED = 5;

				public const int SELECT_FAILED = 6;

				public const int SELECT_COMPLETED_UNCLAIMED = 101;

				public const int SELECT_ENDING_SOON = 102;

				public static readonly int[] SELECT_ALL_QUESTS = new int[8] { 3, 4, 101, 102, 5, 6, 2, 1 };
			}

			public class Milestone
			{
				public enum State
				{
					STATE_UNKNOWN = 0,
					STATE_NOT_STARTED = 1,
					STATE_NOT_COMPLETED = 2,
					STATE_COMPLETED_NOT_CLAIMED = 3,
					STATE_CLAIMED = 4
				}

				public string milestoneId;

				public string eventId;

				public State state;

				public long targetProgress;

				public long currentProgress;

				public byte[] reward;
			}

			public class Quest
			{
				public enum State
				{
					STATE_UNKNOWN = 0,
					STATE_UPCOMING = 1,
					STATE_OPEN = 2,
					STATE_ACCEPTED = 3,
					STATE_COMPLETED = 4,
					STATE_EXPIRED = 5
				}

				public string id;

				public string name;

				public string description;

				public int type;

				public State state;

				public bool isEndingSoon;

				public Milestone currentMilestone;

				public long startTimeStamp;

				public long endTimeStamp;

				public long acceptedTimeStamp;
			}

			public static void claim(string questId, string milestoneId)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "quests_claim", "(Ljava/lang/String;Ljava/lang/String;)V");
				jvalue[] array = new jvalue[2];
				array[0].l = AndroidJNI.NewStringUTF(questId);
				array[1].l = AndroidJNI.NewStringUTF(milestoneId);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void show(int[] request)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "quests_show", "([I)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.ToIntArray(request);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void load(int[] request)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "quests_load", "([I)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.ToIntArray(request);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class Nearby
		{
			public enum Status
			{
				STATE_ERROR = 0,
				STATE_IDLE = 1023,
				STATE_READY = 1024,
				STATE_ADVERTISING = 1025,
				STATE_DISCOVERING = 1026,
				STATE_CONNECTED = 1027
			}

			public static Status state
			{
				get
				{
					AndroidJNI.AttachCurrentThread();
					IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
					IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "Nearby_getState", "()I");
					return (Status)AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
				}
			}

			public static void ping()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "Nearby_ping", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}

			public static void startAdvertising()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "Nearby_startAdvertising", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}

			public static void startDiscovery()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "Nearby_startDiscovery", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}

			public static void sendMessage(byte[] data)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "Nearby_sendMessage", "([B)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.ToByteArray(data);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class Events
		{
			public class Event
			{
				public string id;

				public string name;

				public string description;

				public long value;
			}

			public static void increment(string eventId, int incrementAmount)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "events_increment", "(Ljava/lang/String;I)V");
				jvalue[] array = new jvalue[2];
				array[0].l = AndroidJNI.NewStringUTF(eventId);
				array[1].i = incrementAmount;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void load(bool forceReload)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "events_load", "(Z)V");
				jvalue[] array = new jvalue[1];
				array[0].z = forceReload;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void loadByIds(bool forceReload, string[] eventIds)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "events_loadByIds", "(Z[Ljava/lang/String;)V");
				jvalue[] array = new jvalue[2];
				array[0].z = forceReload;
				array[1].l = AndroidJNIUtils.NewStringArray(eventIds);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class AppState
		{
			[Obsolete("Use \"Snapshots\" instead")]
			public static void resolve(int stateKey, string resolvedVersion, byte[] mergedData)
			{
			}

			[Obsolete("Use \"Snapshots\" instead")]
			public static void load(int keyState)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "appState_migration_load", "(I)V");
				jvalue[] array = new jvalue[1];
				array[0].i = keyState;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			[Obsolete("Use \"Snapshots\" instead")]
			public static void updateImmediate(int keyState, byte[] data)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "appState_migration_updateImmediate", "(I[B)V");
				jvalue[] array = new jvalue[2];
				array[0].i = keyState;
				array[1].l = AndroidJNI.ToByteArray(data);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class Snapshots
		{
			public static void open(string fileName)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "snapshots_open", "(Ljava/lang/String;)V");
				jvalue[] array = new jvalue[1];
				array[0].l = AndroidJNI.NewStringUTF(fileName);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void commitAndClose(long playedTimeMillis, string description, byte[] bitmap, byte[] data)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "snapshots_commitAndClose", "(JLjava/lang/String;[B[B)V");
				jvalue[] array = new jvalue[4];
				array[0].j = playedTimeMillis;
				array[1].l = AndroidJNI.NewStringUTF(description);
				array[2].l = ((bitmap != null) ? AndroidJNI.ToByteArray(bitmap) : ((IntPtr)0));
				array[3].l = ((data != null) ? AndroidJNI.ToByteArray(data) : ((IntPtr)0));
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void select(string title, bool allowAddButton, bool allowDelete)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "snapshots_select", "(Ljava/lang/String;ZZ)V");
				jvalue[] array = new jvalue[3];
				array[0].l = AndroidJNI.NewStringUTF(title);
				array[1].z = allowAddButton;
				array[2].z = allowDelete;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static class Requests
		{
			public enum Type
			{
				TYPE_GIFT = 1,
				TYPE_WISH = 2,
				TYPE_ALL = 65535
			}

			public enum Status
			{
				STATUS_PENDING = 0,
				STATUS_ACCEPTED = 1
			}

			public class GameRequest
			{
				public Status status;

				public Type type;

				public string requestId;

				public long creationTimestamp;

				public long expirationTimestamp;

				public byte[] data;

				public bool mine;
			}

			public static void showSend(Type type, byte[] payload, int lifetime, byte[] icon, string description)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "requets_showSend", "(I[BI[BLjava/lang/String;)V");
				jvalue[] array = new jvalue[5];
				array[0].i = (int)type;
				array[1].l = ((payload != null) ? AndroidJNI.ToByteArray(payload) : ((IntPtr)0));
				array[2].i = lifetime;
				array[3].l = ((icon != null) ? AndroidJNI.ToByteArray(icon) : ((IntPtr)0));
				array[4].l = AndroidJNI.NewStringUTF(description);
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}

			public static void showInbox()
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "requests_showInbox", "()V");
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static bool isSignedIn()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "isSignedIn", "()Z");
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void signIn()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "signIn", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void signOut()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/GameServices");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "signOut", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}
	}
}
