using UnityEngine;

public class UIButtonClick : MonoBehaviour
{
	public UIButtonClickReceiver ClickReceiver;

	public void OnClick()
	{
		Sounds.snd_ui_select.Play();
		if (ClickReceiver != null)
		{
			ClickReceiver.OnButtonClick(this);
		}
	}
}
