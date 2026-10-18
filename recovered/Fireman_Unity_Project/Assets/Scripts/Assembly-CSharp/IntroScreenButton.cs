using UnityEngine;

public class IntroScreenButton : MonoBehaviour
{
	public enum IntroScreenButtonType
	{
		None = 0,
		PlayNow = 1,
		ListItem = 2
	}

	public UILabel label;

	public UITexture texture;

	public string urlClick = string.Empty;

	public string pname = string.Empty;

	public MagmaIntroScreen.preHome.preHomeItem itemFromApi;

	public IntroScreenButtonType introScreenButtonType;

	private void OnClick()
	{
		Debug.Log("click ! ");
		switch (introScreenButtonType)
		{
		case IntroScreenButtonType.PlayNow:
			MagmaIntroScreen.Instance.loadNextScene();
			break;
		case IntroScreenButtonType.ListItem:
			if (urlClick != string.Empty)
			{
				Application.OpenURL(urlClick);
				MagmaIntroScreen.Instance.preHomeClick(itemFromApi);
			}
			break;
		}
	}

	public void cleanImageAndLabelInList()
	{
		label.text = string.Empty;
		urlClick = string.Empty;
		texture.mainTexture = null;
	}

	public void setImageAndLabelInList(byte[] b, string text, string urlMarket)
	{
		label.text = text;
		if (b != null)
		{
			Texture2D texture2D = new Texture2D(75, 75, TextureFormat.ARGB32, false);
			texture2D.LoadImage(b);
			texture.mainTexture = texture2D;
		}
		urlClick = urlMarket;
	}
}
