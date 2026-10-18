using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class Adskit
	{
		public enum AdVideoProvider
		{
			NONE = 0,
			DEBUG = 1,
			ADMOBMEDIATION = 2,
			UNITYADS = 3,
			FACEBOOK = 4
		}

		public enum AdProvider
		{
			NONE = 0,
			DEBUG = 1,
			ADMOBMEDIATION = 2,
			MOPUB = 3,
			AMAZON = 4,
			FACEBOOK = 5
		}

		public enum AdType
		{
			BANNER = 1,
			SQUARE = 2,
			LEADERBOARD = 3,
			LARGEBANNER = 4,
			SMARTBANNER = 5
		}

		public enum AdPos
		{
			LEFT = 1,
			TOP = 2,
			RIGHT = 4,
			BOTTOM = 8,
			CENTER_VERTICAL = 0x10,
			CENTER_HORIZONTAL = 0x20,
			CENTER = 0x40
		}

		private const string CN = "up/Adskit";

		public static bool UseHeader
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getUseHeader", "()Z");
				return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
			}
			set
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setUseHeader", "(Z)V");
				jvalue[] array = new jvalue[1];
				array[0].z = value;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static bool UseExpand
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getUseExpand", "()Z");
				return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
			}
			set
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setUseExpand", "(Z)V");
				jvalue[] array = new jvalue[1];
				array[0].z = value;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static uint ExpandColor
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getExpandColor", "()I");
				return (uint)AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
			}
			set
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setExpandColor", "(I)V");
				jvalue[] array = new jvalue[1];
				array[0].i = (int)value;
				AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
			}
		}

		public static int getAdWidth(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getAdWidth", "(I)I");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			return AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, array);
		}

		public static int getAdHeight(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getAdHeight", "(I)I");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			return AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, array);
		}

		public static void addAdView(string pID, AdProvider pProvider, AdType pType, AdPos pAlign)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addAdView", "(Ljava/lang/String;III)V");
			jvalue[] array = new jvalue[4];
			array[0].l = AndroidJNI.NewStringUTF(pID);
			array[1].i = (int)pProvider;
			array[2].i = (int)pType;
			array[3].i = (int)pAlign;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void addAdView(string pID, AdProvider pProvider, AdType pType, int pX, int pY)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addAdView", "(Ljava/lang/String;IIII)V");
			jvalue[] array = new jvalue[5];
			array[0].l = AndroidJNI.NewStringUTF(pID);
			array[1].i = (int)pProvider;
			array[2].i = (int)pType;
			array[3].i = pX;
			array[4].i = pY;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void setKeywords(int index, string[] keywords)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setKeywords", "(ILjava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].i = index;
			array[1].l = AndroidJNI.NewStringUTF(string.Join(",", keywords));
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void setInterstitialKeywords(int index, string[] keywords)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setInterstitialKeywords", "(ILjava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].i = index;
			array[1].l = AndroidJNI.NewStringUTF(string.Join(",", keywords));
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void showAds(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showAds", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void hideAds(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "hideAds", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void moveAds(int index, AdPos align)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "moveAds", "(II)V");
			jvalue[] array = new jvalue[2];
			array[0].i = index;
			array[1].i = (int)align;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void requeryAds(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "requeryAds", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void addInterstitialAd(string adUnitId)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addInterstitialAd", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(adUnitId);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void preloadInterstitialAd(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "preloadInterstitialAd", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void showInterstitialAd(int index)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showInterstitialAd", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = index;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void setupVideo(string pAdColonyAppID, string pAdColonyZoneID)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setupVideo", "(Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].l = AndroidJNI.NewStringUTF(pAdColonyAppID);
			array[1].l = AndroidJNI.NewStringUTF(pAdColonyZoneID);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void addVideo(AdVideoProvider pType, string pAdUnitID)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addVideo", "(ILjava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].i = (int)pType;
			array[1].l = AndroidJNI.NewStringUTF(pAdUnitID);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void preloadVideo(int pIndex)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "preloadVideo", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = pIndex;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void showVideo(int pIndex)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showVideo", "(I)V");
			jvalue[] array = new jvalue[1];
			array[0].i = pIndex;
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static bool isVideoLoaded(int pIndex)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Adskit");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "isVideoLoaded", "(I)Z");
			jvalue[] array = new jvalue[1];
			array[0].i = pIndex;
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, array);
		}
	}
}
