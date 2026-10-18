using UnityEngine;

public class AppOfTheDay : MonoBehaviour
{
	public UITexture spriteButton;

	public static AppOfTheDayU5 Instance
	{
		get
		{
			return AppOfTheDayU5.Instance;
		}
	}

	private void OnEnable()
	{
		AppOfTheDayU5.Instance.addOnLoaded(SetTexture);
	}

	private void OnDisable()
	{
		AppOfTheDayU5.Instance.onloaded -= SetTexture;
	}

	public void SetTexture(Texture t)
	{
		spriteButton.enabled = true;
		spriteButton.mainTexture = t;
		spriteButton.color = Color.white;
	}

	public void AppOfTheDayReady(string AppOfDayParams)
	{
		AppOfTheDayU5.Instance.AppOfTheDayReady(AppOfDayParams);
	}

	public void OnClick()
	{
		AppOfTheDayU5.Instance.Clicked();
	}
}
