using System;
using AndroidPlugin;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	public UILabel coins;

	public int[] prices;

	private static InAppManager instance;

	public bool inAppReady;

	private static bool paid;

	private static bool removeAdd;

	public static InAppManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Start()
	{
		prices = new int[11];
		inAppReady = false;
		inAppReady = Inapp.CurrentState == Inapp.State.Ready;
		if (Core.IsGoogleTV())
		{
			inAppReady = false;
		}
		checkL();
	}

	private void Awake()
	{
		instance = this;
	}

	public void Refresh()
	{
	}

	public void checkInAppPrice()
	{
		if (string.IsNullOrEmpty(PluginScript.Instance.listInApp[0].price))
		{
			PluginScript.Instance.retrieveInfoInApp();
		}
		else
		{
			inAppReady = true;
		}
	}

	public void OnInAppPrice(string identifier, string price)
	{
		inAppReady = true;
	}

	public void BuyInApp(ItemShop item)
	{
		if (inAppReady)
		{
			PluginScript.Instance.track("InApp/tryBuy/" + item.id);
			string text = TextManager.GetText(item.desc);
			string text2 = TextManager.GetText(item.title);
			PluginScript.Instance.showBuyFeature(item.id_googleplay, text2, text);
		}
	}

	public void OnInAppBuyRefunded(string identifier)
	{
		PluginScript.Instance.track("InApp/buyRefunded/" + identifier);
	}

	public void OnInAppBuyNOK(string identifier)
	{
		PluginScript.Instance.track("InApp/buyNOK/" + identifier);
	}

	public void OnInAppBuyOK(string identifier)
	{
		PluginScript.Instance.track("InApp/buyOK/" + identifier);
		PluginScript.Instance.hidePub();
		PaidAndroidInApp();
		switch (identifier.ToLower())
		{
		case "com.magmamobile.game.fireman.extralevel1":
			VarsManager.Instance.setObjetShopStep("extralevel1", 1);
			break;
		case "com.magmamobile.game.fireman.extralevel2":
			VarsManager.Instance.setObjetShopStep("extralevel2", 1);
			break;
		case "com.magmamobile.game.fireman.extralevel3":
			VarsManager.Instance.setObjetShopStep("extralevel3", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin1":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin1", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin2":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin2", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin3":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin3", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin4":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin4", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin5":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin5", 1);
			break;
		case "com.magmamobile.game.fireman.extraskin6":
			VarsManager.Instance.setObjetShopStep("custom_fireman_skin6", 1);
			break;
		case "com.magmamobile.game.fireman.pet1":
			VarsManager.Instance.setObjetShopStep("custom_pet1", 1);
			break;
		case "com.magmamobile.game.fireman.pet2":
			VarsManager.Instance.setObjetShopStep("custom_pet2", 1);
			break;
		case "com.magmamobile.game.fireman.pet3":
			VarsManager.Instance.setObjetShopStep("custom_pet3", 1);
			break;
		case "com.magmamobile.game.fireman.pet4":
			VarsManager.Instance.setObjetShopStep("custom_pet4", 1);
			break;
		case "com.magmamobile.game.fireman.pet5":
			VarsManager.Instance.setObjetShopStep("custom_pet5", 1);
			break;
		case "com.magmamobile.game.fireman.pet6":
			VarsManager.Instance.setObjetShopStep("custom_pet6", 1);
			break;
		case "com.magmamobile.game.fireman.pet7":
			VarsManager.Instance.setObjetShopStep("custom_pet7", 1);
			break;
		case "com.magmamobile.game.fireman.pet8":
			VarsManager.Instance.setObjetShopStep("custom_pet8", 1);
			break;
		case "com.magmamobile.game.fireman.doublemoney":
			VarsManager.Instance.setObjetShopStep("upgrade_piecex2", 1);
			VarsManager.Instance.setObjetShopStep("upgrade_piecesxp", -1);
			break;
		case "com.magmamobile.game.fireman.doublexp":
			VarsManager.Instance.setObjetShopStep("upgrade_xpx2", 1);
			VarsManager.Instance.setObjetShopStep("upgrade_piecesxp", -1);
			break;
		case "com.magmamobile.game.fireman.doublexpmoney":
			VarsManager.Instance.setObjetShopStep("upgrade_piecesxp", 1);
			VarsManager.Instance.setObjetShopStep("upgrade_piecex2", 1);
			VarsManager.Instance.setObjetShopStep("upgrade_xpx2", 1);
			break;
		case "com.magmamobile.game.fireman.all":
			ItemShop.unlockAll();
			break;
		}
		MenusManager.Instance.updateUIShop();
		Sounds.snd_ui_shop_ok.Play();
	}

	public void setL()
	{
		if (paid)
		{
			PlayerPrefs.SetString("l", PluginScript.Instance.getK("removeads"));
		}
		else
		{
			PlayerPrefs.SetString("l", string.Empty);
		}
		PlayerPrefs.Save();
	}

	public void checkL()
	{
		string text = PlayerPrefs.GetString("l", string.Empty);
		if (text != string.Empty)
		{
			if (PluginScript.Instance.getDK(text) == "removeads")
			{
				paid = true;
				removeAdd = true;
			}
		}
		else
		{
			removeAdd = false;
		}
	}

	public void PaidAndroidInApp()
	{
		paid = true;
		setL();
		checkL();
	}

	public void PaidIOSInApp()
	{
		paid = true;
		setL();
		checkL();
	}

	public void RefundedAndroidInApp()
	{
		paid = false;
		setL();
		checkL();
	}

	public bool shouldRemoveAdd()
	{
		return removeAdd;
	}

	public static string Encrypt(string strIn, string strKey)
	{
		string text = string.Empty;
		for (int i = 0; i < strIn.Length; i++)
		{
			text += string.Format("{0:0000}", strIn[i] ^ strKey[i % strKey.Length]);
		}
		return text;
	}

	public static string getMagicKey()
	{
		string text = PlayerPrefs.GetString("sesame", string.Empty);
		if (text == string.Empty)
		{
			text = UnityEngine.Random.Range(1000000, 2000000).ToString();
			PlayerPrefs.SetString("sesame", text);
			PlayerPrefs.Save();
		}
		return text;
	}

	public static string Decrypt(string strIn, string strKey)
	{
		string text = string.Empty;
		for (int i = 0; i < strIn.Length; i += 4)
		{
			byte b = Convert.ToByte(strIn.Substring(i, 4));
			text += (char)(b ^ strKey[i / 4 % strKey.Length]);
		}
		return text;
	}
}
