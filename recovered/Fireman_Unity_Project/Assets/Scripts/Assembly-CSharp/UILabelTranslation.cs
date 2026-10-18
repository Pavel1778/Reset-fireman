using UnityEngine;

public class UILabelTranslation : MonoBehaviour
{
	public bool translateMe = true;

	public bool upperCase;

	private bool translated;

	public void Start()
	{
		if (translated || !translateMe)
		{
			return;
		}
		UILabel component = GetComponent<UILabel>();
		if (component == null)
		{
			return;
		}
		string text = component.text;
		text = TextManager.GetText(text);
		if (text != null)
		{
			if (upperCase)
			{
				text = text.ToUpper();
			}
			component.text = text;
		}
		translated = true;
	}
}
