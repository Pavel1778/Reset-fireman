using System.Collections;
using System.IO;
using AndroidPlugin;
using UnityEngine;

public static class TextManager
{
	private static Hashtable _default;

	private static Hashtable _current;

	private static string _lang;

	public static string Lang
	{
		get
		{
			return _lang;
		}
	}

	static TextManager()
	{
		SetCurrentLanguage();
		_default = LoadLanguage("string-en");
		_current = LoadLanguage("string-" + _lang);
	}

	public static string GetText(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return string.Empty;
		}
		string text = null;
		key = key.Replace("\n", "\\n");
		text = Get(_current, key);
		if (text == null)
		{
			text = Get(_default, key);
		}
		return text;
	}

	private static string Get(Hashtable table, string key)
	{
		if (table == null || string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (table.ContainsKey(key))
		{
			string text = (string)table[key];
			if (!string.IsNullOrEmpty(text))
			{
				return text.Replace("\\n", "\n");
			}
			return text;
		}
		return null;
	}

	private static Hashtable LoadLanguage(string filename)
	{
		Hashtable hashtable = new Hashtable();
		if (string.IsNullOrEmpty(filename))
		{
			return hashtable;
		}
		TextAsset textAsset = (TextAsset)Resources.Load("Languages/" + filename, typeof(TextAsset));
		if (textAsset == null)
		{
			Debug.LogError("[TextManager] " + filename + " file not found.");
			return hashtable;
		}
		StringReader stringReader = new StringReader(textAsset.text);
		string text = null;
		string text2 = null;
		bool flag = false;
		string text3;
		while ((text3 = stringReader.ReadLine()) != null)
		{
			if (!flag)
			{
				text3 = text3.Trim();
			}
			if (text3.StartsWith("msgid \""))
			{
				text = text3.Substring(7, text3.Length - 8);
			}
			else if (text3.StartsWith("msgstr \""))
			{
				if (text3.EndsWith("\"") && !text3.EndsWith("\\\""))
				{
					text2 = text3.Substring(8, text3.Length - 9);
					flag = false;
				}
				else
				{
					text2 = text3.Substring(8, text3.Length - 8);
					flag = true;
				}
			}
			else if (flag)
			{
				if (text3.EndsWith("\"") && !text3.EndsWith("\\\""))
				{
					text2 += text3.Substring(0, text3.Length - 1);
					flag = false;
				}
				else
				{
					text2 += text3.Substring(0, text3.Length);
					flag = true;
				}
			}
			else if (text != null && text2 != null)
			{
				flag = false;
				if (!hashtable.ContainsKey(text))
				{
					hashtable.Add(text, text2);
				}
				text = (text2 = null);
			}
		}
		stringReader.Close();
		return hashtable;
	}

	private static void SetCurrentLanguage()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Czech:
			_lang = "cs";
			break;
		case SystemLanguage.Danish:
			_lang = "da";
			break;
		case SystemLanguage.German:
			_lang = "de";
			break;
		case SystemLanguage.Spanish:
			_lang = "es";
			break;
		case SystemLanguage.Finnish:
			_lang = "fi";
			break;
		case SystemLanguage.Hungarian:
			_lang = "hu";
			break;
		case SystemLanguage.Indonesian:
			_lang = "id";
			break;
		case SystemLanguage.Italian:
			_lang = "it";
			break;
		case SystemLanguage.Japanese:
			_lang = "ja";
			break;
		case SystemLanguage.Korean:
			_lang = "ko";
			break;
		case SystemLanguage.Dutch:
			_lang = "nl";
			break;
		case SystemLanguage.Greek:
			_lang = "el";
			break;
		case SystemLanguage.Polish:
			_lang = "pl";
			break;
		case SystemLanguage.Portuguese:
			if ("pt-rBR".Equals(Core.getLocale()))
			{
				_lang = "ptbr";
			}
			else
			{
				_lang = "ptpt";
			}
			break;
		case SystemLanguage.Romanian:
			_lang = "ro";
			break;
		case SystemLanguage.Russian:
			_lang = "ru";
			break;
		case SystemLanguage.Swedish:
			_lang = "sv";
			break;
		case SystemLanguage.Slovak:
			_lang = "sk";
			break;
		case SystemLanguage.Turkish:
			_lang = "tr";
			break;
		case SystemLanguage.Chinese:
			if ("zh-rCN".Equals(Core.getLocale()))
			{
				_lang = "zhcn";
			}
			else
			{
				_lang = "zhtw";
			}
			break;
		case SystemLanguage.English:
			if (Core.IsLexibox())
			{
				_lang = "en-lexibox";
			}
			else if (Core.IsGoogleTV())
			{
				_lang = "en-gtv";
			}
			else
			{
				_lang = "en";
			}
			break;
		case SystemLanguage.French:
			_lang = "fr";
			break;
		case SystemLanguage.Thai:
			_lang = "th";
			break;
		case SystemLanguage.Vietnamese:
			_lang = "vi";
			break;
		default:
			_lang = "en";
			break;
		}
	}
}
