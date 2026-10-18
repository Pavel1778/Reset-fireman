using System;
using System.Collections.Generic;

[Serializable]
public class ApiBase
{
	public List<ItemNews> news;

	public List<ItemMoreGames> moregames;

	public int HasNewNews;

	public int HasNewUpdates;

	public int HasNewVersionAvailable;

	public List<Promo> promo;

	public int appodayId;

	public string appodayName;

	public string appodayUrl;

	public string appodayIconUrl;
}
