using System;
using UnityEngine;

namespace AndroidPlugin
{
	public class Mmusia
	{
		private const string CN = "up/Mmusia";

		[Obsolete("Use Unity code instead")]
		public static void showAbout(string title, string html)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showAbout", "(Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].l = AndroidJNI.NewStringUTF(title);
			array[1].l = AndroidJNI.NewStringUTF(html);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void showMoreGames()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showMoreGames", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void showOtherGames()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showOtherGames", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void showNews()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showNews", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void sendAppOfTheDay()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "sendAppOfTheDay", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void logAppOfDayClick(string idstr)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "logAppOfDayClick", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(idstr);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		[Obsolete("Use Unity code instead")]
		public static void showAsk4Rate()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showAsk4Rate", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		[Obsolete("Use Unity code instead")]
		public static void startAsk4Rate()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Mmusia");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "startAsk4Rate", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}
	}
}
