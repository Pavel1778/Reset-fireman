using System.Collections.Generic;
using UnityEngine;

public class MMUSIAStrings
{
	public class MMUSIAWord
	{
		public string enWord = string.Empty;

		public string frWord = string.Empty;

		public MMUSIAWord(string en, string fr)
		{
			enWord = en;
			frWord = fr;
		}
	}

	private static MMUSIAStrings instance;

	private Dictionary<string, MMUSIAWord> wordList = new Dictionary<string, MMUSIAWord>();

	public static MMUSIAStrings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new MMUSIAStrings();
			}
			return instance;
		}
	}

	public MMUSIAStrings()
	{
		wordList.Clear();
		MMUSIAWord value = new MMUSIAWord("Close", "Fermer");
		wordList.Add("Close", value);
		value = new MMUSIAWord("Download", "Télécharger");
		wordList.Add("Download", value);
		value = new MMUSIAWord("Play", "Jouer");
		wordList.Add("Play", value);
		value = new MMUSIAWord("Loading...", "Chargement...");
		wordList.Add("Loading...", value);
	}

	public string getWord(string word)
	{
		if (wordList.ContainsKey(word))
		{
			MMUSIAWord mMUSIAWord = wordList[word];
			SystemLanguage systemLanguage = Application.systemLanguage;
			if (systemLanguage == SystemLanguage.French)
			{
				return mMUSIAWord.frWord;
			}
			return mMUSIAWord.enWord;
		}
		return "NOT SET !!! : " + word;
	}
}
