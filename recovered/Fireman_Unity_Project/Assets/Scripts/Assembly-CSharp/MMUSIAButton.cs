using UnityEngine;

public class MMUSIAButton : MonoBehaviour
{
	public enum enumMMUSIAButonType
	{
		None = 0,
		MoreGames = 1,
		AppOfDay = 2,
		MMUSIANews = 3,
		OpenFromPromoDialog = 4,
		Close = 5,
		MagmaMobileLogo = 6,
		TEST = 7
	}

	public enumMMUSIAButonType MMUSIAButtonType;

	private void OnClick()
	{
		switch (MMUSIAButtonType)
		{
		case enumMMUSIAButonType.MoreGames:
			mmusia.Instance.checkUIRoot();
			mmusia.Instance.showMoreGames();
			break;
		case enumMMUSIAButonType.MMUSIANews:
			mmusia.Instance.checkUIRoot();
			mmusia.Instance.showNews();
			break;
		case enumMMUSIAButonType.AppOfDay:
			mmusia.Instance.onClickAppofDay();
			break;
		case enumMMUSIAButonType.OpenFromPromoDialog:
			mmusia.Instance.DialogDownload();
			mmusia.Instance.clean(true);
			mmusia.Instance.closeMMUSIAItem();
			break;
		case enumMMUSIAButonType.Close:
			mmusia.Instance.clean(true);
			mmusia.Instance.closeMMUSIAItem();
			mmusia.Instance.broadcastCloseList();
			break;
		case enumMMUSIAButonType.MagmaMobileLogo:
			Application.OpenURL("http://m.magmamobile.com/?os=android");
			break;
		case enumMMUSIAButonType.TEST:
			break;
		}
	}
}
