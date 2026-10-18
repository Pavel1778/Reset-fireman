public class AppConfig
{
	public enum BuildEnum
	{
		Release = 0,
		Debug = 1
	}

	public enum GameModeEnum
	{
		Auto = 0,
		Normal = 1,
		Hardcore = 2,
		Extreme = 4
	}

	public enum PlatformEnum
	{
		None = 0,
		AndroidGoogle = 1001,
		AndroidAmazon = 1002,
		AndroidOuya = 1003,
		AndroidiDTGV = 1004,
		AndroidAmazonTV = 1005,
		iOS = 2001,
		MacOS = 2002,
		WindowPhone8 = 3001,
		Windows8 = 3002
	}

	public enum PrefEnum
	{
		Keep = 0,
		Clear = 1
	}

	public enum CheatEnum
	{
		None = 0,
		UnlockLevels = 1
	}

	public static BuildEnum Build
	{
		get
		{
			return BuildEnum.Release;
		}
	}

	public static GameModeEnum GameMode
	{
		get
		{
			return GameModeEnum.Auto;
		}
	}

	public static PlatformEnum Platform
	{
		get
		{
			return PlatformEnum.AndroidGoogle;
		}
	}

	public static PrefEnum Preferences
	{
		get
		{
			return PrefEnum.Keep;
		}
	}

	public static CheatEnum Cheats
	{
		get
		{
			return CheatEnum.None;
		}
	}
}
