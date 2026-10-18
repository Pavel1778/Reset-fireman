using System;
using UnityEngine;

namespace AndroidPlugin
{
	internal static class AndroidJNIUtils
	{
		public static IntPtr NewStringArray(string[] values)
		{
			if (values != null)
			{
				IntPtr intPtr = AndroidJNI.NewObjectArray(values.Length, AndroidJNI.FindClass("java/lang/String"), IntPtr.Zero);
				for (int i = 0; i < values.Length; i++)
				{
					AndroidJNI.SetObjectArrayElement(intPtr, i, AndroidJNI.NewStringUTF(values[i]));
				}
				return intPtr;
			}
			return IntPtr.Zero;
		}
	}
}
