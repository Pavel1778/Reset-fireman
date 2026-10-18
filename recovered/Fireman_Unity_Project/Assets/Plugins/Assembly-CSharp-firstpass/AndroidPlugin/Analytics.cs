using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class Analytics
	{
		private const string CN = "up/Analytics";

		public static bool track(string page)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Analytics");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "track", "(Ljava/lang/String;)Z");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(page);
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, array);
		}

		public static bool trackAndDispatch(string page)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Analytics");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "trackAndDispatch", "(Ljava/lang/String;)Z");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(page);
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, array);
		}

		public static void trackStat(int index, string value)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Analytics");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "trackStat", "(ILjava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].i = index;
			array[1].l = AndroidJNI.NewStringUTF(value);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void trackDim(int index, string value)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Analytics");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "trackDim", "(ILjava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].i = index;
			array[1].l = AndroidJNI.NewStringUTF(value);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}
	}
}
