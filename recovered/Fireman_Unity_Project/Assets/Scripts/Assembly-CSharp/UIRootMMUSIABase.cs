using UnityEngine;

public class UIRootMMUSIABase : MonoBehaviour
{
	public UICamera CameraRoot;

	public UIAnchor Anchor;

	public UIPanel Panel;

	public GameObject MMUSIAPromoDialog;

	public GameObject MMUSIAMoreGames;

	public GameObject MMUSIANews;

	public GameObject moreGamesPrefab;

	public GameObject newsPrefab;

	private string ScreenWidthStr = string.Empty;

	private void Start()
	{
		UICamera.fallThrough = null;
	}

	private void Update()
	{
		if (ScreenWidthStr == string.Empty || ScreenWidthStr != Screen.width.ToString() + Screen.height)
		{
			ScreenWidthStr = Screen.width.ToString() + Screen.height;
			if (Screen.width > Screen.height)
			{
				GetComponent<UIRoot>().manualHeight = 480;
				GetComponent<UIRoot>().minimumHeight = 480;
				GetComponent<UIRoot>().maximumHeight = 480;
			}
			else
			{
				GetComponent<UIRoot>().manualHeight = 960;
				GetComponent<UIRoot>().minimumHeight = 960;
				GetComponent<UIRoot>().maximumHeight = 960;
			}
			BroadcastMsgMMUSIA("MMUSIA_ResizeMeImFamous");
		}
	}

	public void BroadcastMsgMMUSIA(string msg)
	{
		BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver);
	}

	public void DisplayMoreGames()
	{
		clean();
		checkUIRoot();
		MMUSIAMoreGamesBase component = NGUITools.AddChild(Panel.gameObject, MMUSIAMoreGames).GetComponent<MMUSIAMoreGamesBase>();
		component.name = "MMUSIAMoreGames";
		if (mmusia.Instance.api.moregames.Count > 0)
		{
			Object.Destroy(component.LoadingObj);
		}
		ScreenWidthStr = string.Empty;
		Vector3 zero = Vector3.zero;
		foreach (ItemMoreGames moregame in mmusia.Instance.api.moregames)
		{
			GameObject gameObject = Object.Instantiate(moreGamesPrefab);
			gameObject.transform.parent = component.PanelItems.transform;
			gameObject.transform.localPosition = zero;
			gameObject.transform.localScale = Vector3.one;
			MMUSIAMoreGameItem component2 = gameObject.GetComponent<MMUSIAMoreGameItem>();
			component2.title.text = moregame.title;
			component2.pname = moregame.pname;
			component2.descr.text = ((moregame.free != 0) ? "FREE" : string.Empty);
			component2.url = moregame.urlMarket;
			mmusia.Instance.loadIcon(moregame.urlImg, component2.icone);
			zero += Vector3.down * 100f;
		}
		mmusia.Instance.broadcastOpenList();
	}

	public void DisplayNews()
	{
		clean();
		checkUIRoot();
		MMUSIANewsBase component = NGUITools.AddChild(Panel.gameObject, MMUSIANews).GetComponent<MMUSIANewsBase>();
		component.name = "MMUSIANews";
		if (mmusia.Instance.api.news.Count > 0)
		{
			Object.Destroy(component.LoadingObj);
		}
		ScreenWidthStr = string.Empty;
		Vector3 zero = Vector3.zero;
		foreach (ItemNews item in mmusia.Instance.api.news)
		{
			GameObject gameObject = Object.Instantiate(newsPrefab);
			gameObject.transform.parent = component.PanelItems.transform;
			gameObject.transform.localPosition = zero;
			gameObject.transform.localScale = Vector3.one;
			MMUSIANewsItem component2 = gameObject.GetComponent<MMUSIANewsItem>();
			component2.title.text = item.NewsTitle;
			component2.descr.text = item.NewsDesc;
			component2.id = item.NewsID;
			component2.url = item.NewsUrlLink;
			if (component2.url == string.Empty)
			{
				component2.url = item.NewsUrlMarket;
			}
			mmusia.Instance.loadIcon(item.imgUrl, component2.icone);
			zero += Vector3.down * 100f;
		}
		mmusia.Instance.broadcastOpenList();
	}

	public void DisplayPromoDialog()
	{
		clean();
		checkUIRoot();
		MMUSIAPromoDialogBase component = NGUITools.AddChild(Panel.gameObject, MMUSIAPromoDialog).GetComponent<MMUSIAPromoDialogBase>();
		component.name = "MMUSIAPromoDialog";
		ScreenWidthStr = string.Empty;
		component.setDialogDisplay();
		component.PanelUI = Panel;
		mmusia.Instance.broadcastOpenList();
	}

	public void checkUIRoot()
	{
		if (mmusia.Instance.uirootMmusia == null)
		{
			mmusia.Instance.loadUIRoot();
		}
	}

	public void clean()
	{
		if (!(mmusia.Instance.uirootMmusia == null))
		{
			int childCount = Panel.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(Panel.transform.GetChild(i).gameObject);
			}
			mmusia.Instance.moreGamesLoaded = false;
			mmusia.Instance.newsLoaded = false;
		}
	}
}
