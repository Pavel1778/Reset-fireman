public class Config
{
	public class BonusInfo
	{
		public int coin_rnd;

		public int coin_max;

		public int coin_min;

		public int exp;

		public BonusInfo(int pCoin_Rnd, int pCoin_Min, int pCoin_Max, int pExp)
		{
			coin_rnd = pCoin_Rnd;
			coin_min = pCoin_Min;
			coin_max = pCoin_Max;
			exp = pExp;
		}
	}

	public class BonusDef
	{
		public readonly BonusInfo ashes = new BonusInfo(2, 0, 1, 5);

		public readonly BonusInfo crawler = new BonusInfo(100, 1, 2, 10);

		public readonly BonusInfo runner = new BonusInfo(100, 1, 2, 15);

		public readonly BonusInfo bomber = new BonusInfo(100, 1, 3, 20);

		public readonly BonusInfo thrower = new BonusInfo(100, 2, 3, 25);

		public readonly BonusInfo spider = new BonusInfo(100, 2, 3, 30);

		public readonly BonusInfo victim = new BonusInfo(0, 0, 0, 20);

		public readonly BonusInfo relic = new BonusInfo(0, 0, 0, 100);

		public readonly BonusInfo bubbleblast = new BonusInfo(100, 6, 12, 50);

		public readonly BonusInfo boss1 = new BonusInfo(100, 6, 12, 100);
	}

	public static readonly BonusDef bonuses = new BonusDef();
}
