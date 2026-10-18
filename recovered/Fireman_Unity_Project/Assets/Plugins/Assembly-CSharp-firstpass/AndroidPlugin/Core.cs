using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class Core
	{
		private const string CN = "up/Core";

		private const string AEV = "AndroidEventHandler";

		public static bool DEBUG;

		private static bool _IsLexibox;

		private static bool _IsLexiboxInit;

		private static bool _IsFunTab2;

		private static bool _IsFunTabInit;

		private static bool _IsGoogleTV;

		private static bool _IsGoogleTVInit;

		private static bool _IsAmazon;

		private static bool _IsAmazonInit;

		public static string LibVersion
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getLibVersion", "()Ljava/lang/String;");
				return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static string PackageName
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getPackageName", "()Ljava/lang/String;");
				return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static float ScreenDensity
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getScreenDensity", "()F");
				return AndroidJNI.CallStaticFloatMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static int AndroidSDKVersion
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getAndroidSDKVersion", "()I");
				return AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static string AppVersionName
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getAppVersionName", "()Ljava/lang/String;");
				return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static int AppVersionCode
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getAppVersionCode", "()I");
				return AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static string deviceUniqueIdentifier
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getUniqueID", "()Ljava/lang/String;");
				return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static bool UseNearby
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "UseNearby", "()Z;");
				return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static void Initialize(EventHandler handler)
		{
			DEBUG = getDebug();
			PrivateHandler.handler = handler;
			CreateEventHandler();
		}

		private static GameObject CreateEventHandler()
		{
			GameObject gameObject = GameObject.Find("AndroidEventHandler");
			if (gameObject == null)
			{
				gameObject = new GameObject("AndroidEventHandler");
				gameObject.AddComponent<PrivateHandler>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return gameObject;
		}

		public static bool IsLexibox()
		{
			if (!_IsLexiboxInit)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "IsLexibox", "()Z");
				_IsLexibox = AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
				_IsLexiboxInit = true;
			}
			return _IsLexibox;
		}

		public static bool IsFunTab2()
		{
			if (!_IsFunTabInit)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "IsFunTab2", "()Z");
				_IsFunTab2 = AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
				_IsFunTabInit = true;
			}
			return _IsFunTab2;
		}

		public static bool IsGoogleTV()
		{
			if (!_IsGoogleTVInit)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "IsGoogleTV", "()Z");
				_IsGoogleTV = AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
				_IsGoogleTVInit = true;
			}
			return _IsGoogleTV;
		}

		public static bool IsAmazon()
		{
			if (!_IsAmazonInit)
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Core");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "IsAmazon", "()Z");
				_IsAmazon = AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
				_IsAmazonInit = true;
			}
			return _IsAmazon;
		}

		public static bool HasFacebookApp()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "HasFacebookApp", "()Z");
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static float dpi(float value)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getDPI", "(F)F");
			jvalue[] array = new jvalue[1];
			array[0].f = value;
			return AndroidJNI.CallStaticFloatMethod(clazz, staticMethodID, array);
		}

		public static void SetFullScreenAsync()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "setFullScreenAsync", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void showGooglePlayGamePage()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showGooglePlayGamePage", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		[Obsolete("Use Unity code instead")]
		public static void showBrowser(string url)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showBrowser", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(url);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void email(string mailto, string subject, string body)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "email", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[3];
			array[0].l = AndroidJNI.NewStringUTF(mailto);
			array[1].l = AndroidJNI.NewStringUTF(subject);
			array[2].l = AndroidJNI.NewStringUTF(body);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void share(string msg, string subject, string title, string facebookUrl)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "share", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[4];
			array[0].l = AndroidJNI.NewStringUTF(msg);
			array[1].l = AndroidJNI.NewStringUTF(subject);
			array[2].l = AndroidJNI.NewStringUTF(title);
			array[3].l = AndroidJNI.NewStringUTF(facebookUrl);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void sharePicture(string msg, string subject, string title, byte[] png)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "sharePicture", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;[B)V");
			jvalue[] array = new jvalue[4];
			array[0].l = AndroidJNI.NewStringUTF(msg);
			array[1].l = AndroidJNI.NewStringUTF(subject);
			array[2].l = AndroidJNI.NewStringUTF(title);
			array[3].l = AndroidJNI.ToByteArray(png);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		[Obsolete("Use Unity code instead")]
		public static string kryp(string text)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "kryp", "(Ljava/lang/String;)Ljava/lang/String;");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(text);
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, array);
		}

		[Obsolete("Use Unity code instead")]
		public static string dkryp(string text)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "dkryp", "(Ljava/lang/String;)Ljava/lang/String;");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(text);
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, array);
		}

		public static void ShowToast(string text)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "showToast", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(text);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static string getGLExtensions()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getGLExtensions", "()Ljava/lang/String;");
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static int getLibBuildDate()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getLibBuildDate", "()I");
			return AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static bool getVerbose()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getVerbose", "()Z");
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static string getCountry()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getCountry", "()Ljava/lang/String;");
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static string getLanguage()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getLanguage", "()Ljava/lang/String;");
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static string getLocale()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getLocale", "()Ljava/lang/String;");
			return AndroidJNI.CallStaticStringMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static bool getDebug()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getDebug", "()Z");
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static bool getThrowError()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Core");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getThrowError", "()Z");
			return AndroidJNI.CallStaticBooleanMethod(clazz, staticMethodID, new jvalue[0]);
		}

		[Obsolete("Use \"LibVersion\" instead")]
		public static string getLibVersion()
		{
			return LibVersion;
		}

		[Obsolete("Use \"PackageName\" instead")]
		public static string getPackageName()
		{
			return PackageName;
		}

		[Obsolete("Use \"ScreenDensity\" instead")]
		public static float getScreenDensity()
		{
			return ScreenDensity;
		}

		[Obsolete("Use \"AndroidSDKVersion\" instead")]
		public static int getAndroidSDKVersion()
		{
			return AndroidSDKVersion;
		}

		[Obsolete("Use \"AppVersionName\" instead")]
		public static string getAppVersionName()
		{
			return AppVersionName;
		}

		[Obsolete("Use \"AppVersionCode\" instead")]
		public static int getAppVersionCode()
		{
			return AppVersionCode;
		}

		[Obsolete("Use \"deviceUniqueIdentifier\" instead")]
		public static string getUniqueID()
		{
			return deviceUniqueIdentifier;
		}
	}
}
