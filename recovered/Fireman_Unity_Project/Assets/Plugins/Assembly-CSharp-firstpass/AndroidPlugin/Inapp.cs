using System;
using UnityEngine;

namespace AndroidPlugin
{
	public static class Inapp
	{
		public enum State
		{
			Uninitialized = 0,
			Ready = 1,
			NotAvailable = 2
		}

		public class ProductDetails
		{
			public string productId;

			public string type;

			public string price;

			public string price_amount_micros;

			public string price_currency_code;

			public string title;

			public string description;
		}

		public class PurchaseDetails
		{
			public string autoRenewing;

			public string orderId;

			public string packageName;

			public string productId;

			public string purchaseTime;

			public string purchaseState;

			public string developerPayload;

			public string purchaseToken;

			public bool verified;
		}

		private const string CN = "up/Inapp";

		public static State CurrentState
		{
			get
			{
				AndroidJNI.AttachCurrentThread();
				IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
				IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "getState", "()I");
				return (State)AndroidJNI.CallStaticIntMethod(clazz, staticMethodID, new jvalue[0]);
			}
		}

		public static void addProduct(string productId)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "addProduct", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(productId);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void loadProductDetails()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "loadProductDetails", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void loadPurchaseDetails()
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "loadPurchaseDetails", "()V");
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, new jvalue[0]);
		}

		public static void purchaseItem(string productId, string developerPayload)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "purchaseItem", "(Ljava/lang/String;Ljava/lang/String;)V");
			jvalue[] array = new jvalue[2];
			array[0].l = AndroidJNI.NewStringUTF(productId);
			array[1].l = AndroidJNI.NewStringUTF(developerPayload);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}

		public static void consumePurchase(string token)
		{
			AndroidJNI.AttachCurrentThread();
			IntPtr clazz = AndroidJNI.FindClass("up/Inapp");
			IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(clazz, "consumePurchase", "(Ljava/lang/String;)V");
			jvalue[] array = new jvalue[1];
			array[0].l = AndroidJNI.NewStringUTF(token);
			AndroidJNI.CallStaticVoidMethod(clazz, staticMethodID, array);
		}
	}
}
