using UnityEngine;

public class ItemRang : MonoBehaviour
{
	public UISprite spriteIcone;

	public UISprite spriteBarre;

	public UISprite spriteBarreBase;

	public UILabel labelRang;

	public UILabel labelXp;

	public int xpToAdd;

	public bool addXp;

	public void OnEnable()
	{
		updateUI();
	}

	public void Update()
	{
		if (addXp)
		{
			VarsManager.Instance.addXp(xpToAdd);
			updateUI();
			addXp = false;
		}
	}

	public void updateUI()
	{
		if (!(VarsManager.Instance == null))
		{
			int xp = VarsManager.Instance.getXp();
			if (VarsManager.Instance.rang > 0)
			{
				spriteIcone.gameObject.SetActiveRecursively(true);
				spriteIcone.spriteName = "Rank" + VarsManager.Instance.rang;
			}
			else
			{
				spriteIcone.gameObject.SetActiveRecursively(false);
			}
			labelRang.text = VarsManager.Instance.tabRang[VarsManager.Instance.rang].titre;
			if (VarsManager.Instance.rang < VarsManager.Instance.tabRang.Length)
			{
				int xpNeeded = VarsManager.Instance.getXpNeeded(xp);
				int num = ((VarsManager.Instance.rang + 1 >= VarsManager.Instance.tabRang.Length) ? VarsManager.Instance.tabRang[VarsManager.Instance.rang].xpRequired : VarsManager.Instance.tabRang[VarsManager.Instance.rang + 1].xpRequired);
				labelXp.text = xpNeeded + "/" + num;
				spriteBarre.fillAmount = 0.1f + (float)xpNeeded / (float)num * 0.8f;
			}
			else
			{
				labelXp.text = "yeah!";
			}
		}
	}
}
