using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utils
{
	public static string ToHash(this string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		byte[] buffer = Array.ConvertAll(str.ToCharArray(), (char x) => (byte)x);
		byte[] array = new MD5CryptoServiceProvider().ComputeHash(buffer);
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public static string toHex(this string input)
	{
		string text = "0123456789ABCDEF";
		string text2 = string.Empty;
		foreach (char c in input)
		{
			int num = (256 + c) % 256;
			text2 += text[num / 16] + text[num % 16];
		}
		return text2;
	}

	public static Type GetTypeByName(string name)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.FullName == name)
				{
					return type;
				}
			}
		}
		return null;
	}

	public static List<Type> GetSubTypes(Type t)
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(t))
				{
					list.Add(type);
				}
			}
		}
		return list;
	}

	public static T Child<T>(this GameObject t, string name) where T : MonoBehaviour
	{
		return new List<T>(t.GetComponentsInChildren<T>(true)).Find((T u) => u.gameObject.name == name);
	}

	public static T GetComponentInChildren<T>(this GameObject t, bool includeInactive) where T : MonoBehaviour
	{
		return (T)t.GetComponentsInChildren(typeof(T), includeInactive)[0];
	}

	public static List<O> map<T, O>(this List<T> t, Converter<T, O> converter)
	{
		if (converter == null)
		{
			throw new ArgumentNullException("converter");
		}
		List<O> list = new List<O>(t.Count);
		for (int i = 0; i < t.Count; i++)
		{
			list.Add(converter(t[i]));
		}
		return list;
	}

	public static O fold<T, O>(this List<T> t, Func<T, O, O> f, O acc)
	{
		if (f == null)
		{
			throw new ArgumentNullException("converter");
		}
		for (int i = 0; i < t.Count; i++)
		{
			acc = f(t[i], acc);
		}
		return acc;
	}

	public static T fold1<T>(this List<T> t, Func<T, T, T> f)
	{
		if (f == null)
		{
			throw new ArgumentNullException("converter");
		}
		T val = t[0];
		for (int i = 1; i < t.Count; i++)
		{
			val = f(t[i], val);
		}
		return val;
	}

	public static int Levenstein(this string s, string t)
	{
		if (t == null)
		{
			return 0;
		}
		if (s == null)
		{
			return 0;
		}
		int length = s.Length;
		int length2 = t.Length;
		int[,] array = new int[length + 1, length2 + 1];
		if (length == 0)
		{
			return length2;
		}
		if (length2 == 0)
		{
			return length;
		}
		for (int i = 1; i <= length; i++)
		{
			for (int j = 1; j <= length2; j++)
			{
				int num = ((t[j - 1] != s[i - 1]) ? 1 : 0);
				array[i, j] = Math.Min(Math.Min(array[i - 1, j] + 1, array[i, j - 1] + 1), array[i - 1, j - 1] + num);
			}
		}
		return array[length, length2];
	}

	public static int[] TriangleStrip(int vertices)
	{
		int[] array = new int[(vertices - 2) * 3];
		int num = 0;
		for (int i = 0; i < vertices - 2; i++)
		{
			if ((i & 1) == 0)
			{
				array[num] = i;
				num++;
				array[num] = i + 1;
				num++;
				array[num] = i + 2;
				num++;
			}
			else
			{
				array[num] = i;
				num++;
				array[num] = i + 2;
				num++;
				array[num] = i + 1;
				num++;
			}
		}
		return array;
	}

	public static void shuffle<T>(this T[] tab)
	{
		for (int i = 0; i < tab.Length; i++)
		{
			int num = UnityEngine.Random.Range(i, tab.Length);
			T val = tab[i];
			tab[i] = tab[num];
			tab[num] = val;
		}
	}

	public static T[] shuffleClone<T>(this T[] tab)
	{
		tab = new List<T>(tab).ToArray();
		tab.shuffle();
		return tab;
	}

	public static void shuffle<T>(this List<T> tab)
	{
		for (int i = 0; i < tab.Count; i++)
		{
			int index = UnityEngine.Random.Range(i, tab.Count);
			T value = tab[i];
			tab[i] = tab[index];
			tab[index] = value;
		}
	}

	public static List<T> shuffleClone<T>(this List<T> tab)
	{
		tab = new List<T>(tab);
		tab.shuffle();
		return tab;
	}
}
