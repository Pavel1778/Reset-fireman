using UnityEngine;

public class MMUSIAPromoDialogBase : MonoBehaviour
{
	public UIPanel PanelItems;

	public UIPanel PanelUI;

	public UILabel Title;

	public UILabel Description;

	public UITexture Icon;

	public void setDialogDisplay()
	{
		Title.text = mmusia.Instance.promoInfo.title;
		Description.text = mmusia.Instance.promoInfo.description;
		mmusia.Instance.loadIcon(mmusia.Instance.promoInfo.iconUrl, Icon);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			mmusia.Instance.clean(true);
			mmusia.Instance.closeMMUSIAItem();
			mmusia.Instance.broadcastCloseList();
		}
	}
}
