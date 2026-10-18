using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class Alerts
	{
		public class Alert
		{
			public long time;

			public string title;

			public string content;

			public string trackPage;
		}

		private const string CN = "up/Core";

		public static long currentTimeMillis()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - dateTime).TotalMilliseconds;
		}

		public static void add(long time, string title, string content, string trackPage)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addAlert", "(JLjava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[4];
			array[0].j = time;
			array[1].l = AndroidJNI.NewStringUTF(title);
			array[2].l = AndroidJNI.NewStringUTF(content);
			array[3].l = AndroidJNI.NewStringUTF(trackPage);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void clear()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "clearAlerts", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void load()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "loadAlerts", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}
	}
}
