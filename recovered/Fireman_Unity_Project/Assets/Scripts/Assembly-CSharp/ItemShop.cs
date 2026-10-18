using System.Collections.Generic;
using UnityEngine;

public class ItemShop
{
	public string id;

	public string title;

	public string desc;

	public int categorie;

	public int step;

	public int stepMax;

	public int[] rangRequired;

	public int prixPiece;

	public string prixInApp;

	public string spriteName;

	public string id_googleplay;

	public string id_itunes;

	public string id_wp8store;

	public bool IsInApp
	{
		get
		{
			return !string.IsNullOrEmpty(id_googleplay);
		}
	}

	public ItemShop(string id, int cat, int[] rangRequired, int prixPiece, string title, string desc, int stepMax, string spriteName, string ID_GooglePlay, string ID_iTunes, string ID_WP8Store)
	{
		prixInApp = string.Empty;
		this.id = id;
		categorie = cat;
		this.prixPiece = prixPiece;
		this.rangRequired = rangRequired;
		id_googleplay = ID_GooglePlay;
		id_itunes = ID_iTunes;
		id_wp8store = ID_WP8Store;
		this.stepMax = stepMax;
		this.spriteName = spriteName;
		this.title = title;
		this.desc = ((!string.IsNullOrEmpty(desc)) ? desc : title);
		prixInApp = string.Empty;
		step = VarsManager.Instance.getObjetShopStep(id);
	}

	public static void unlockAll()
	{
		List<ItemShop> listItemShop = MenusManager.Instance.listItemShop;
		for (int i = 0; i < listItemShop.Count; i++)
		{
			ItemShop itemShop = listItemShop[i];
			while (itemShop.unlock())
			{
			}
		}
	}

	public bool unlock()
	{
		if (step < stepMax && step != -1)
		{
			step++;
			VarsManager.Instance.setObjetShopStep(id, step);
			return true;
		}
		return false;
	}

	public void buy(GameObject btn)
	{
		if (step >= stepMax || step == -1)
		{
			return;
		}
		if (VarsManager.Instance.rang >= rangRequired[step])
		{
			if (btn.name.Equals("ButtonInApp") && !prixInApp.Equals("-1"))
			{
				InAppManager.Instance.BuyInApp(this);
			}
			if (!btn.name.Equals("ButtonPiece") || prixPiece == -1)
			{
				return;
			}
			if (VarsManager.Instance.getCoins() < prixPiece * (step + 1))
			{
				MenusManager.Instance.mBando.GetComponent<Bando>().showBando("coin02", TextManager.GetText("res_not_enough_money"), true);
				Sounds.snd_ui_shop_nok.Play();
				return;
			}
			VarsManager.Instance.removeCoins(prixPiece * (step + 1));
			unlock();
			Sounds.snd_ui_shop_ok.Play();
			if (id.Equals("upgrade_xpx2") || id.Equals("upgrade_piecex2"))
			{
				VarsManager.Instance.setObjetShopStep("upgrade_piecesxp", -1);
			}
			if (id.Equals("upgrade_piecesxp"))
			{
				VarsManager.Instance.setObjetShopStep("upgrade_xpx2", 1);
				VarsManager.Instance.setObjetShopStep("upgrade_piecex2", 1);
			}
			if (id.Equals("upgrade_all"))
			{
				unlockAll();
			}
			MenusManager.Instance.updateUIShop();
		}
		else
		{
			Sounds.snd_ui_shop_nok.Play();
			MenusManager.Instance.mBando.GetComponent<Bando>().showBando("Rank" + (rangRequired[step] + 1), "Rank " + VarsManager.Instance.tabRang[rangRequired[step]].titre + " " + TextManager.GetText("res_required"), true);
		}
	}

	public void updateStep()
	{
		step = VarsManager.Instance.getObjetShopStep(id);
	}
}
