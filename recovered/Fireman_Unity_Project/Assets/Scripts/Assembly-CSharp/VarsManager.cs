using System;
using System.Collections.Generic;
using System.Text;
using AndroidPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VarsManager : MonoBehaviour
{
	[Serializable]
	public class Achiev
	{
		public string sprite_name;

		public int id_title;

		public int id_description;

		public int max_steps;

		public int current_step;

		public bool disabled;

		public Achiev(string sprite_name, int id_title, int id_description, int max_steps, int current_step)
		{
			this.sprite_name = sprite_name;
			this.id_title = id_title;
			this.id_description = id_description;
			this.max_steps = max_steps;
			this.current_step = current_step;
		}
	}

	public const int MODE_FLAG_NORMAL = 1;

	public const int MODE_FLAG_HARDCORE = 2;

	public const int MODE_FLAG_EXTREME = 4;

	public const int NBR_SKIN_POMPIER = 7;

	public const int NBR_SKIN_PET = 8;

	public const int NB_MONDE = 3;

	public const int NB_LEVEL = 24;

	private static VarsManager instance;

	private static int mGameMode = 1;

	private static int mGameModeUnlocked = 1;

	public bool masterOfTheKeys;

	public bool twoPlayer;

	public bool tryItHarder;

	public bool firstUse;

	public bool vibration;

	public bool useArrows;

	public int rang;

	public Rang[] tabRang;

	public int worldToLoad;

	public int levelToLoad;

	public bool loadGame;

	public bool loadMenu;

	public bool goToLevelSelector;

	public bool goToShop;

	public int selectItemShop = -1;

	public bool goToShopTabCustom;

	public bool returnToGame;

	public bool cloudToSend;

	public int GameOverCount;

	public List<Achiev> listAchievs;

	public static int GameMode
	{
		get
		{
			return mGameMode;
		}
		set
		{
			if (mGameMode != value)
			{
				mGameMode = value;
				PlayerPrefs.SetInt("modeCurrent", mGameMode);
				PlayerPrefs.Save();
			}
		}
	}

	public static VarsManager Instance
	{
		get
		{
			return instance;
		}
	}

	private static void loadTestLevel()
	{
		if (AppConfig.GameMode != AppConfig.GameModeEnum.Auto)
		{
			mGameMode = (int)AppConfig.GameMode;
		}
		if (AppConfig.Build == AppConfig.BuildEnum.Debug)
		{
			Instance.levelToLoad = -1;
			Instance.worldToLoad = 1;
			SceneManager.LoadScene("ScenePlay3");
		}
		if (AppConfig.Preferences == AppConfig.PrefEnum.Clear)
		{
			PlayerPrefs.DeleteAll();
		}
		if (AppConfig.Cheats == AppConfig.CheatEnum.UnlockLevels)
		{
			Instance.setProgression(1, 3, 18);
			Instance.setProgression(2, 3, 18);
			Instance.setProgression(4, 3, 18);
			Instance.setCoins(10000);
			UnlockGameMode(7);
		}
	}

	public void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(instance);
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/SpineData");
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = gameObject.name;
	}

	public void Start()
	{
		loadAchievements();
		firstUse = PlayerPrefs.GetInt("firstUse", 1) == 1;
		vibration = PlayerPrefs.GetInt("vibration", 1) == 1;
		useArrows = PlayerPrefs.GetInt("useArrows", 1) == 1;
		mGameMode = PlayerPrefs.GetInt("modeCurrent", 1);
		mGameModeUnlocked = PlayerPrefs.GetInt("modeUnlocked", 1);
		if (firstUse)
		{
			PlayerPrefs.SetInt("firstUse", 0);
			PlayerPrefs.Save();
		}
		GameServices.AppState.load(0);
		selectItemShop = -1;
		loadTestLevel();
	}

	public void Update()
	{
		Input.Update();
	}

	public static void setGameModeTemporally(int v)
	{
		if (mGameMode != v)
		{
			mGameMode = v;
		}
	}

	public static bool HasOtherGameMode()
	{
		return mGameModeUnlocked > 1;
	}

	public static bool IsGameModeAvailable(int mode)
	{
		return (mGameModeUnlocked & mode) == mode;
	}

	public static void UnlockGameMode(int mode)
	{
		mGameModeUnlocked |= mode;
		PlayerPrefs.SetInt("modeUnlocked", mGameModeUnlocked);
		PluginScript.Instance.saveIntForCloud("modeUnlocked", mGameModeUnlocked);
		PlayerPrefs.Save();
	}

	public static int HasHarderMode()
	{
		if (mGameModeUnlocked == 1)
		{
			return 2;
		}
		if (mGameModeUnlocked == 3)
		{
			return 4;
		}
		return 0;
	}

	private void loadAchievements()
	{
		for (int i = 0; i < 32; i++)
		{
			listAchievs.Add(new Achiev("Achievement1", i + 1, i + 1, 1, 0));
		}
		listAchievs[3].max_steps = 8;
		listAchievs[14].max_steps = 1000;
		listAchievs[15].max_steps = 5000;
		listAchievs[16].max_steps = 10000;
		listAchievs[24].max_steps = 15;
		listAchievs[25].max_steps = 100;
		listAchievs[26].max_steps = 100;
		listAchievs[27].max_steps = 100;
		listAchievs[28].max_steps = 100;
		listAchievs[29].max_steps = 100;
		listAchievs[30].max_steps = 100;
		listAchievs[31].disabled = true;
		for (int j = 0; j < listAchievs.Count; j++)
		{
			listAchievs[j].current_step = PlayerPrefs.GetInt("achiev" + j);
		}
	}

	public void setAchievement(int id, int step, bool forceAndDontShow)
	{
		if (step <= listAchievs[id].current_step && !forceAndDontShow)
		{
			return;
		}
		listAchievs[id].current_step = step;
		PlayerPrefs.SetInt("achiev" + id, step);
		if (listAchievs[id].current_step >= listAchievs[id].max_steps)
		{
			if (!forceAndDontShow && listAchievs[id].current_step == listAchievs[id].max_steps)
			{
				Sounds.snd_ig_notif.Play();
				if (GameManager.Instance != null)
				{
					GameManager.Instance.mBando.GetComponent<Bando>().showBando("Achievement1", TextManager.GetText("res_achiev" + listAchievs[id].id_title + "_title"), true);
				}
				else if (MenusManager.Instance != null)
				{
					MenusManager.Instance.mBando.GetComponent<Bando>().showBando("Achievement1", TextManager.GetText("res_achiev" + listAchievs[id].id_title + "_title"), true);
				}
			}
			PluginScript.Instance.submitAchievementAndroid(id);
		}
		PluginScript.Instance.saveIntForCloud("achiev" + id, step);
	}

	public void genRank()
	{
		tabRang = new Rang[11]
		{
			new Rang(0, TextManager.GetText("res_rank_0"), 0),
			new Rang(1, TextManager.GetText("res_rank_1"), 100),
			new Rang(2, TextManager.GetText("res_rank_2"), 300),
			new Rang(3, TextManager.GetText("res_rank_3"), 500),
			new Rang(4, TextManager.GetText("res_rank_4"), 1000),
			new Rang(5, TextManager.GetText("res_rank_5"), 1500),
			new Rang(6, TextManager.GetText("res_rank_6"), 2000),
			new Rang(7, TextManager.GetText("res_rank_7"), 3000),
			new Rang(8, TextManager.GetText("res_rank_8"), 5000),
			new Rang(9, TextManager.GetText("res_rank_9"), 7000),
			new Rang(10, TextManager.GetText("res_rank_10"), 12000)
		};
		rang = computeRang(getXp());
	}

	public void addXp(int xp)
	{
		int num = 0;
		int num2 = PlayerPrefs.GetInt("xp", 0);
		for (int i = 0; i < tabRang.Length; i++)
		{
			num += tabRang[i].xpRequired;
		}
		num2 = ((num2 + xp <= num) ? (num2 + xp) : num);
		PlayerPrefs.SetInt("xp", num2);
		rang = computeRang(num2);
		PluginScript.Instance.saveIntForCloud("xp", getXp());
	}

	public void setXp(int xp)
	{
		int num = 0;
		for (int i = 0; i < tabRang.Length; i++)
		{
			num += tabRang[i].xpRequired;
		}
		if (xp > num)
		{
			xp = num;
		}
		PlayerPrefs.SetInt("xp", xp);
		rang = computeRang(xp);
		PluginScript.Instance.saveIntForCloud("xp", getXp());
	}

	public int getXp()
	{
		return PlayerPrefs.GetInt("xp", 0);
	}

	public int getXpNeeded(int withThisXp)
	{
		int num = -1;
		int i;
		for (i = 0; i <= withThisXp; i += tabRang[num].xpRequired)
		{
			if (num >= tabRang.Length - 1)
			{
				break;
			}
			num++;
		}
		if (num == -1)
		{
			num = tabRang.Length - 1;
		}
		i -= tabRang[num].xpRequired;
		return withThisXp - i;
	}

	public int computeRang(int withThisXp)
	{
		int num = -1;
		for (int i = 0; i <= withThisXp; i += tabRang[num].xpRequired)
		{
			if (num >= tabRang.Length)
			{
				break;
			}
			num++;
			if (num == tabRang.Length)
			{
				break;
			}
		}
		num--;
		if (num == -2)
		{
			num = tabRang.Length - 1;
		}
		return num;
	}

	public int getCoins()
	{
		return PlayerPrefs.GetInt("piece", 0);
	}

	public int getObjetShopStep(string id)
	{
		return PlayerPrefs.GetInt("objetShop_" + id, 0);
	}

	public void setObjetShopStep(string id, int val)
	{
		PlayerPrefs.SetInt("objetShop_" + id, val);
		PluginScript.Instance.saveIntForCloud("objetShop_" + id, val);
		if (id.StartsWith("custom_fireman_skin"))
		{
			int num = int.Parse(id.Substring(id.Length - 1));
			Instance.setSkinHead(num);
			Instance.setSkinJacket(num);
		}
		if (id.StartsWith("custom_pet"))
		{
			int petSkin = int.Parse(id.Substring(id.Length - 1));
			Instance.setPetSkin(petSkin);
		}
	}

	public void addCoins(int coins)
	{
		PlayerPrefs.SetInt("piece", PlayerPrefs.GetInt("piece", 0) + coins);
		PluginScript.Instance.saveIntForCloud("piece", PlayerPrefs.GetInt("piece", 0));
	}

	public void setCoins(int coins)
	{
		PlayerPrefs.SetInt("piece", coins);
		PluginScript.Instance.saveIntForCloud("piece", PlayerPrefs.GetInt("piece", 0));
	}

	public void removeCoins(int coins)
	{
		PlayerPrefs.SetInt("piece", PlayerPrefs.GetInt("piece", 0) - coins);
	}

	public int getProgression()
	{
		return PlayerPrefs.GetInt("progression" + mGameMode, 1001);
	}

	public void setProgression(int mode, int world, int level)
	{
		string key = "progression" + mode;
		int num = world * 1000 + level;
		PlayerPrefs.SetInt(key, num);
		PluginScript.Instance.saveIntForCloud(key, num);
	}

	public string getStars(int mode, int world, int level)
	{
		return PlayerPrefs.GetString("stringStars" + (mode * 100000 + world * 1000 + level), "000");
	}

	public void setStars(int mode, int world, int level, bool a, bool b, bool c, bool force)
	{
		string key = "stringStars" + (mode * 100000 + world * 1000 + level);
		string empty = string.Empty;
		int num = 0;
		int num2 = 0;
		if (a)
		{
			empty += "1";
			num2++;
		}
		else
		{
			empty += "0";
		}
		if (b)
		{
			empty += "1";
			num2++;
		}
		else
		{
			empty += "0";
		}
		if (c)
		{
			empty += "1";
			num2++;
		}
		else
		{
			empty += "0";
		}
		if (PlayerPrefs.HasKey(key))
		{
			string text = PlayerPrefs.GetString(key);
			if (text[0] == '1')
			{
				num++;
			}
			if (text[1] == '1')
			{
				num++;
			}
			if (text[2] == '1')
			{
				num++;
			}
			if (num2 > num || force)
			{
				PlayerPrefs.SetString(key, empty);
				PluginScript.Instance.saveStringForCloud(key, empty);
			}
		}
		else
		{
			PlayerPrefs.SetString(key, empty);
			PluginScript.Instance.saveStringForCloud(key, empty);
		}
	}

	public int getPetSkin()
	{
		return PlayerPrefs.GetInt("petSkin", 0);
	}

	public void setPetSkin(int val)
	{
		PlayerPrefs.SetInt("petSkin", val);
	}

	public int getSkinHead()
	{
		return PlayerPrefs.GetInt("skinHead");
	}

	public void setSkinHead(int val)
	{
		PlayerPrefs.SetInt("skinHead", val);
	}

	public int getSkinJacket()
	{
		return PlayerPrefs.GetInt("skinJacket");
	}

	public void setSkinJacket(int val)
	{
		PlayerPrefs.SetInt("skinJacket", val);
	}

	public static bool getSecret(int id)
	{
		return id != 0 && PlayerPrefs.GetInt("secret" + id, 0) == 1;
	}

	public static void setSecret(int id, bool unlocked, bool saveToCloud)
	{
		if (id != 0)
		{
			PlayerPrefs.SetInt("secret" + id, unlocked ? 1 : 0);
			if (saveToCloud)
			{
				PluginScript.Instance.saveIntForCloud("secret" + id, unlocked ? 1 : 0);
			}
		}
	}

	public bool getObjetCache(int world, int id)
	{
		return PlayerPrefs.GetInt("lvlcache_" + world + "_" + id, 0) == 1;
	}

	public void setObjetCache(int world, int id)
	{
		PlayerPrefs.SetInt("lvlcache_" + world + "_" + id, 1);
		PluginScript.Instance.saveIntForCloud("lvlcache_" + world + "_" + id, 1);
		setAchievement(23, 100, false);
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				num += PlayerPrefs.GetInt("lvlcache_" + i + "_" + j, 0);
			}
		}
		setAchievement(24, num, false);
	}

	public void setUseArrows(bool val)
	{
		int value = 0;
		if (val)
		{
			value = 1;
		}
		PlayerPrefs.SetInt("useArrows", value);
		useArrows = val;
	}

	public void commonPompierSkinUpdater(SkeletonAnimation anim, int skinHead, int skinJacket)
	{
		anim.skeleton.SetToSetupPose();
		Skins.All[skinHead].SetHead(anim.skeleton);
		Skins.All[skinJacket].SetBody(anim.skeleton);
		Skins.All[0].SetJetPack(anim.skeleton, getObjetShopStep("upgrade_jetpack"));
	}

	public void onCloudChange(string keyvalue)
	{
		int num = 0;
		string[] array = keyvalue.Split('/');
		string text = array[0];
		switch (array[0])
		{
		case "piece":
			num = int.Parse(array[1]);
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
			return;
		case "xp":
			num = int.Parse(array[1]);
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
			return;
		case "modeUnlocked":
			num = int.Parse(array[1]);
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
			return;
		}
		num = int.Parse(array[1]);
		if (text.StartsWith("objetShop_"))
		{
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
		}
		if (text.StartsWith("achiev"))
		{
			if (num > PlayerPrefs.GetInt(text))
			{
				setAchievement(int.Parse(text.Substring(6)), num, true);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
		}
		if (text.StartsWith("lvlcache_"))
		{
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
		}
		if (text.StartsWith("progression"))
		{
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
		}
		if (text.StartsWith("stringStars"))
		{
			int num2 = 0;
			int num3 = 0;
			string text2 = PlayerPrefs.GetString(text, "000");
			if (array[1][0] == '1')
			{
				num3++;
			}
			if (array[1][1] == '1')
			{
				num3++;
			}
			if (array[1][2] == '1')
			{
				num3++;
			}
			if (text2[0] == '1')
			{
				num2++;
			}
			if (text2[1] == '1')
			{
				num2++;
			}
			if (text2[2] == '1')
			{
				num2++;
			}
			if (num3 > num2)
			{
				PlayerPrefs.SetString(text, array[1]);
			}
			else if (num3 < num2)
			{
				cloudToSend = true;
			}
		}
		if (text.StartsWith("secret"))
		{
			if (num > PlayerPrefs.GetInt(text))
			{
				PlayerPrefs.SetInt(text, num);
			}
			else if (num < PlayerPrefs.GetInt(text))
			{
				cloudToSend = true;
			}
		}
	}

	private void onCloudFinish()
	{
		if (!cloudToSend)
		{
			return;
		}
		PluginScript.Instance.saveIntForCloud("piece", getCoins());
		PluginScript.Instance.saveIntForCloud("progression", getProgression());
		PluginScript.Instance.saveIntForCloud("xp", getXp());
		PluginScript.Instance.saveIntForCloud("modeUnlocked", getXp());
		for (int i = 0; i < MenusManager.Instance.listItemShop.Count; i++)
		{
			PluginScript.Instance.saveIntForCloud("objetShop_" + MenusManager.Instance.listItemShop[i].id, getObjetShopStep("objetShop_" + MenusManager.Instance.listItemShop[i].id));
		}
		for (int j = 0; j < 16; j++)
		{
			PluginScript.Instance.saveIntForCloud("secret" + j, PlayerPrefs.GetInt("secret" + j, 0));
		}
		for (int k = 0; k < listAchievs.Count; k++)
		{
			PluginScript.Instance.saveIntForCloud("achiev" + k, listAchievs[k].current_step);
		}
		int[] array = new int[3] { 1, 2, 4 };
		for (int l = 0; l < array.Length; l++)
		{
			for (int m = 1; m <= 3; m++)
			{
				int num = 1;
				while (num <= 24)
				{
					PluginScript.Instance.saveStringForCloud("stringStars" + (array[l] * 100000 + m * 1000 + num), getStars(l, m, num));
					m++;
				}
			}
		}
		for (int n = 1; n <= 3; n++)
		{
			int num2 = 0;
			while (num2 < 5)
			{
				int val = 0;
				if (getObjetCache(n, num2))
				{
					val = 1;
				}
				PluginScript.Instance.saveIntForCloud("lvlcache_" + n + "_" + num2, val);
				n++;
			}
		}
		cloudToSend = false;
	}

	public byte[] createDataAndroidCloud()
	{
		byte[] array = new byte[4096];
		int num = 0;
		byte[] bytes = BitConverter.GetBytes(getCoins());
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		bytes = BitConverter.GetBytes(getProgression());
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		bytes = BitConverter.GetBytes(getXp());
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		bytes = BitConverter.GetBytes(MenusManager.Instance.listItemShop.Count);
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		for (int i = 0; i < MenusManager.Instance.listItemShop.Count; i++)
		{
			bytes = BitConverter.GetBytes(getObjetShopStep(MenusManager.Instance.listItemShop[i].id));
			Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
		}
		bytes = BitConverter.GetBytes(listAchievs.Count);
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		for (int j = 0; j < listAchievs.Count; j++)
		{
			bytes = BitConverter.GetBytes(listAchievs[j].current_step);
			Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
			num += bytes.Length;
		}
		bytes = BitConverter.GetBytes(3);
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		bytes = BitConverter.GetBytes(3);
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		bytes = BitConverter.GetBytes(24);
		Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
		num += bytes.Length;
		int[] array2 = new int[3] { 1, 2, 4 };
		for (int k = 0; k < array2.Length; k++)
		{
			for (int l = 1; l <= 3; l++)
			{
				for (int m = 1; m <= 24; m++)
				{
					bytes = Encoding.ASCII.GetBytes(getStars(array2[k], l, m));
					Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
					num += bytes.Length;
				}
			}
		}
		for (int n = 1; n <= 3; n++)
		{
			for (int num2 = 0; num2 < 5; num2++)
			{
				int value = 0;
				if (getObjetCache(n, num2))
				{
					value = 1;
				}
				bytes = BitConverter.GetBytes(value);
				Buffer.BlockCopy(bytes, 0, array, num, bytes.Length);
				num += bytes.Length;
			}
		}
		return array;
	}

	public void mergeDataAndroidCloud(byte[] data)
	{
		PluginScript.Instance.dontCloud = true;
		try
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			num = BitConverter.ToInt32(data, num2);
			if (num > getCoins())
			{
				setCoins(num);
			}
			num2 += 4;
			num = BitConverter.ToInt32(data, num2);
			int num7 = num / 1000;
			int level = num - num7 * 1000;
			if (num > getProgression())
			{
				setProgression(1, num7, level);
			}
			num2 += 4;
			num = BitConverter.ToInt32(data, num2);
			if (num > getProgression())
			{
				setXp(num);
			}
			num2 += 4;
			num3 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			for (int i = 0; i < num3; i++)
			{
				num = BitConverter.ToInt32(data, num2);
				if (num > getObjetShopStep(MenusManager.Instance.listItemShop[i].id))
				{
					setObjetShopStep(MenusManager.Instance.listItemShop[i].id, num);
				}
				num2 += 4;
			}
			num3 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			for (int j = 0; j < num3; j++)
			{
				num = BitConverter.ToInt32(data, num2);
				if (num > listAchievs[j].current_step)
				{
					setAchievement(j, num, true);
				}
				num2 += 4;
			}
			num6 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			num4 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			num5 = BitConverter.ToInt32(data, num2);
			num2 += 4;
			int[] array = new int[3] { 1, 2, 4 };
			for (int k = 1; k <= num6; k++)
			{
				for (int l = 1; l <= num4; l++)
				{
					for (int m = 1; m <= num5; m++)
					{
						int num8 = 0;
						int num9 = 0;
						string stars = getStars(array[k], l, m);
						string text = Encoding.ASCII.GetString(data, num2, 3);
						if (text[0] == '1')
						{
							num9++;
						}
						if (text[1] == '1')
						{
							num9++;
						}
						if (text[2] == '1')
						{
							num9++;
						}
						if (stars[0] == '1')
						{
							num8++;
						}
						if (stars[1] == '1')
						{
							num8++;
						}
						if (stars[2] == '1')
						{
							num8++;
						}
						if (num9 > num8)
						{
							setStars(array[k], l, m, text[0] == '1', text[1] == '1', text[2] == '1', false);
						}
						num2 += 3;
					}
				}
			}
			for (int n = 1; n <= num4; n++)
			{
				for (int num10 = 0; num10 < 5; num10++)
				{
					if (!getObjetCache(n, num10) && BitConverter.ToInt32(data, num2) == 1)
					{
						setObjetCache(n, num10);
					}
					num2 += 4;
				}
			}
		}
		catch (Exception)
		{
		}
		PluginScript.Instance.dontCloud = false;
	}

	public bool hideAllPub()
	{
		return InAppManager.Instance.shouldRemoveAdd();
	}
}
