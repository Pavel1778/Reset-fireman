using System.Collections.Generic;
using System.Text;
using AndroidPlugin;
using UnityEngine;

public class MenusManager : UIButtonClickReceiver
{
	public GameObject mBando;

	public UIGrid uiGridWorld;

	public UIGrid uiGridAchiev;

	public UILabel lblDifficulty;

	public UIButton btnDifficulty;

	public UIPanel draggablePanelWorld;

	public UIPanel draggablePanelLostAndFound;

	public GameObject prefabItemShop;

	public GameObject prefabItemWorld;

	public GameObject prefabItemAchievement;

	public GameObject panelHome;

	public GameObject panelDialogHome;

	[HideInInspector]
	public GameObject panelGame;

	public GameObject panelHarder;

	public GameObject panelChangeLog;

	public GameObject panelWorldSelect;

	public GameObject panelShop;

	public GameObject panelQuit;

	public GameObject panelSettings;

	public GameObject panelSettingsElements;

	public GameObject panelCredit;

	public GameObject LogoExtreme;

	public GameObject LogoHardcore;

	public GameObject LogoFireman;

	public GameObject spriteTransition;

	public float ratioTransition;

	public bool sensTransition;

	public GameObject panelQG;

	public GameObject panelQGSkins;

	public GameObject panelQGAchievements;

	public GameObject panelQGPets;

	public GameObject panelQGLostAndFound;

	public GameObject panelQGRangs;

	public SkeletonAnimation qgAnimSkin;

	public SkeletonAnimation qgAnimPet;

	public SkeletonAnimation shopAnimSkin;

	public SkeletonAnimation shopAnimPet;

	public GameObject buttonQGSkins;

	public GameObject buttonQGAchievements;

	public GameObject buttonQGPets;

	public GameObject buttonQGLostAndFound;

	public GameObject buttonQGRangs;

	public UISprite spriteRang;

	public UILabel labelPiece;

	public Camera mCam;

	private float orthoSize;

	public GameObject currentPanel;

	public GameObject nextPanel;

	public GameObject buttonGameServices;

	public GameObject buttonAchievGooglePlus;

	public GameObject buttonNews;

	private int shopCategorie;

	public List<ItemShop> listItemShop = new List<ItemShop>();

	private int selectedItemShop;

	public UISprite spriteItemShop;

	public GameObject buttonShopLeft;

	public GameObject buttonShopRight;

	public UISprite spriteTabLevels;

	public UISprite spriteTabCustoms;

	public UISprite spriteTabUpgrades;

	public UILabel labelNomItemShop;

	public UILabel labelPrixInApp;

	public UILabel labelPrixPiece;

	public GameObject buttonPrixInApp;

	public GameObject buttonPrixPiece;

	public UISprite spriteGaugeBase;

	public UISprite spriteGaugeLvl1;

	public UISprite spriteGaugeLvl2;

	public UISprite spriteGaugeLvl3;

	public UISprite spriteShopCheck;

	public UISprite spritePetCache1;

	public UISprite spritePetCache2;

	public UISprite spritePetCache3;

	public UISprite spritePetCache4;

	public UISprite spritePetCache5;

	public UISprite spritePetCache6;

	public UILabel labelSelectWorldName;

	public UISprite spriteQGPetCheck;

	public GameObject buttonQGPetChoose;

	public GameObject boutonQGPetBuyIt;

	private static MenusManager instance;

	private float touchSelectX;

	private float touchSelectY;

	public int selectedWorld;

	public GameObject buttonWorldLeft;

	public GameObject buttonWorldRight;

	public GameObject buttonWorldQGUp;

	public GameObject buttonWorldQGDown;

	public GameObject ButtonMoreGames;

	public GameObject SmallButtonNews;

	public GameObject ButtonShare;

	public GameObject ButtonFacebook;

	public GameObject ButtonAppOfDay;

	public GameObject ButtonNewsletter;

	public GameObject buttonQGSkinHeadLeft;

	public GameObject buttonQGSkinHeadRight;

	public GameObject buttonQGSkinJacketLeft;

	public GameObject buttonQGSkinJacketRight;

	public GameObject buttonQGPetLeft;

	public GameObject buttonQGPetRight;

	public UISprite spriteNoPets;

	public Texture textureWorld1;

	public Texture textureWorld2;

	public Texture textureWorld3;

	public UITexture textureFondWorld;

	private int selectedPet;

	public UISprite chkMusic;

	public UISprite chkSound;

	private float timeBlink;

	private float timeSpeakDocShop;

	private float timeSpeakMouthDocShop;

	private int cptSpeakMouthDocShop = 1;

	public UISprite spriteMouthDoc;

	public UISprite spriteDocArm;

	public UISprite spriteDocArm2;

	private string textBulleShop;

	private float ratioTextBulleShop;

	private int textBulleShopIndex;

	private int harderModeToUnlock;

	public GameObject mContentQuit;

	private ItemWorld[] itemsWorld;

	public static MenusManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void Awake()
	{
		instance = this;
	}

	public void Start()
	{
		if (!Core.IsGoogleTV() && !Core.IsLexibox())
		{
			Adskit.preloadInterstitialAd(0);
		}
		textureWorld3 = (Texture)Resources.Load("Textures/LevelSelector_01_Map3");
		InAppManager.Instance.checkInAppPrice();
		panelGame = new GameObject("PanelGame");
		Musics.music_menu.PlayFade(1f);
		VarsManager.Instance.genRank();
		shopCategorie = 0;
		genAllItemShop();
		genAllItemAchiev();
		genAllItemWorld();
		uiGridWorld.cellWidth = (float)Screen.height * 4f;
		prefabItemWorld.GetComponent<BoxCollider>().size = new Vector3((float)Screen.height * 4f, prefabItemWorld.GetComponent<BoxCollider>().size.y, 0f);
		updateUIShop();
		updateUIWorld();
		selectedPet = VarsManager.Instance.getPetSkin();
		UpdateDifficultyLabel();
		sensTransition = false;
		ratioTransition = 1f;
		VarsManager.Instance.loadMenu = false;
		VarsManager.Instance.loadGame = false;
		if (VarsManager.Instance.goToLevelSelector)
		{
			currentPanel = panelWorldSelect;
			btnShowWorldSelect();
			VarsManager.Instance.goToLevelSelector = false;
		}
		else if (VarsManager.Instance.goToShop)
		{
			showShop();
		}
		else
		{
			showHome();
		}
		PluginScript.Instance.showPromo();
		PluginScript.Instance.track("menu");
		autoShowChangeLog();
		if (Core.IsGoogleTV() || Core.IsLexibox())
		{
			PluginScript.Instance.pub = false;
		}
	}

	public void autoShowChangeLog()
	{
		string appVersionName = Core.AppVersionName;
		string text = PlayerPrefs.GetString("versionName", "1.0.0");
		if (text != appVersionName && appVersionName != "1.0.0")
		{
			if (!VarsManager.Instance.firstUse)
			{
				showChangeLog();
			}
			PlayerPrefs.SetString("versionName", appVersionName);
			PlayerPrefs.Save();
		}
	}

	public void SetGameModeLogo()
	{
		UITexture component = LogoExtreme.GetComponent<UITexture>();
		component.enabled = VarsManager.GameMode == 4;
		UITexture component2 = LogoHardcore.GetComponent<UITexture>();
		component2.enabled = VarsManager.GameMode == 2;
	}

	public void btnBack()
	{
		if (Ask4RateScreen.next())
		{
			return;
		}
		if (currentPanel == panelHome)
		{
			showQuit();
		}
		else if (currentPanel == panelQuit)
		{
			showHome();
		}
		else if (currentPanel == panelHarder)
		{
			showHome();
		}
		else if (currentPanel == panelChangeLog)
		{
			showHome();
		}
		else if (currentPanel == panelShop)
		{
			if (VarsManager.Instance.returnToGame)
			{
				VarsManager.Instance.returnToGame = false;
				VarsManager.Instance.loadGame = true;
				Application.LoadLevel("SceneLoading");
			}
			else
			{
				nextPanel = panelHome;
				sensTransition = true;
				Musics.music_menu.PlayFade(0.2f);
			}
		}
		else if (currentPanel == panelWorldSelect || currentPanel == panelQG)
		{
			nextPanel = panelHome;
			sensTransition = true;
		}
		else if (currentPanel == panelSettings)
		{
			if (panelCredit.active)
			{
				panelCredit.SetActiveRecursively(false);
				panelSettingsElements.SetActiveRecursively(true);
			}
			else
			{
				nextPanel = panelHome;
				sensTransition = true;
			}
		}
		Sounds.snd_ui_select.Play();
	}

	private void Update()
	{
		if (VarsManager.Instance.tryItHarder)
		{
			VarsManager.Instance.tryItHarder = false;
			showHarder();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			btnBack();
		}
		if (textBulleShopIndex < textBulleShop.Length)
		{
			ratioTextBulleShop -= Time.deltaTime;
			if (ratioTextBulleShop < 0f)
			{
				ratioTextBulleShop = 0.01f;
				textBulleShopIndex++;
			}
		}
		if (TextManager.Lang == "ru" || TextManager.Lang == "ja" || TextManager.Lang == "ko" || TextManager.Lang == "zhcn" || TextManager.Lang == "th" || TextManager.Lang == "el")
		{
			labelNomItemShop.color = Color.white;
		}
		labelNomItemShop.text = textBulleShop.Substring(0, textBulleShopIndex);
		timeBlink += 4f * Time.deltaTime;
		if (timeBlink > 1f)
		{
			timeBlink = -1f;
		}
		if (listItemShop[selectedItemShop].stepMax == 3)
		{
			spriteGaugeBase.gameObject.active = true;
			int num = listItemShop[selectedItemShop].step;
			if (timeBlink >= 0f)
			{
				num++;
			}
			if (num >= 1)
			{
				spriteGaugeLvl1.gameObject.active = true;
			}
			else
			{
				spriteGaugeLvl1.gameObject.active = false;
			}
			if (num >= 2)
			{
				spriteGaugeLvl2.gameObject.active = true;
			}
			else
			{
				spriteGaugeLvl2.gameObject.active = false;
			}
			if (num >= 3)
			{
				spriteGaugeLvl3.gameObject.active = true;
			}
			else
			{
				spriteGaugeLvl3.gameObject.active = false;
			}
		}
		else
		{
			spriteGaugeBase.gameObject.active = false;
			spriteGaugeLvl1.gameObject.active = false;
			spriteGaugeLvl2.gameObject.active = false;
			spriteGaugeLvl3.gameObject.active = false;
			if (listItemShop[selectedItemShop].step >= 1)
			{
				spriteShopCheck.gameObject.active = true;
			}
			else
			{
				spriteShopCheck.gameObject.active = false;
			}
		}
		if (timeSpeakDocShop >= 0f && currentPanel == panelShop)
		{
			spriteDocArm.gameObject.active = true;
			spriteDocArm2.gameObject.active = false;
			if (!Sounds.snd_ui_shop_talk01.isPlaying && !Sounds.snd_ui_shop_talk02.isPlaying && !Sounds.snd_ui_shop_talk03.isPlaying && !Sounds.snd_ui_shop_talk04.isPlaying)
			{
				int num2 = (int)(Random.value * 4f) + 1;
				if (num2 == 1)
				{
					Sounds.snd_ui_shop_talk01.Play();
				}
				if (num2 == 2)
				{
					Sounds.snd_ui_shop_talk02.Play();
				}
				if (num2 == 3)
				{
					Sounds.snd_ui_shop_talk03.Play();
				}
				if (num2 == 4)
				{
					Sounds.snd_ui_shop_talk04.Play();
				}
			}
			timeSpeakDocShop -= Time.deltaTime;
			if (timeSpeakMouthDocShop >= 0f)
			{
				timeSpeakMouthDocShop -= Time.deltaTime;
				if (timeSpeakMouthDocShop <= 0f)
				{
					spriteMouthDoc.spriteName = "Shop_04_DocMouth" + cptSpeakMouthDocShop;
					int num3;
					for (num3 = (int)(Random.value * 5f); num3 == cptSpeakMouthDocShop; num3 = (int)(Random.value * 5f))
					{
					}
					cptSpeakMouthDocShop = num3;
					timeSpeakMouthDocShop = 0.07f;
				}
			}
			if (timeSpeakDocShop <= 0f)
			{
				spriteMouthDoc.spriteName = "Shop_04_DocMouth4";
			}
		}
		else
		{
			spriteDocArm.gameObject.active = false;
			spriteDocArm2.gameObject.active = true;
			Sounds.snd_ui_shop_talk01.Stop();
			Sounds.snd_ui_shop_talk02.Stop();
			Sounds.snd_ui_shop_talk03.Stop();
			Sounds.snd_ui_shop_talk04.Stop();
		}
		mCam.transform.position = currentPanel.transform.position;
		if (currentPanel == panelWorldSelect || currentPanel == panelQG)
		{
			bool flag = false;
			bool flag2 = false;
			Vector2 vector = Vector2.zero;
			flag2 = Input.touchCount > 0;
			if (flag2)
			{
				vector = Input.GetTouch(0).position;
			}
			if (flag2)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					touchSelectX = vector.x;
					touchSelectY = vector.y;
				}
				else if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					float num4 = vector.x / (float)Screen.width - touchSelectX / (float)Screen.width;
					float num5 = vector.y / (float)Screen.height - touchSelectY / (float)Screen.height;
					if (num4 < -0.06f)
					{
						selectedWorld++;
						flag = true;
					}
					if (num4 > 0.06f)
					{
						selectedWorld--;
						flag = true;
					}
					if (num5 < -0.07f)
					{
						selectedWorld--;
						flag = true;
					}
					if (num5 > 0.07f)
					{
						selectedWorld++;
						flag = true;
					}
					if (flag)
					{
						updateUIWorld();
					}
				}
			}
			if (currentPanel == panelWorldSelect)
			{
				draggablePanelWorld.transform.localPosition = new Vector3(draggablePanelWorld.transform.localPosition.x + ((float)(-selectedWorld * Screen.height) * 4f - draggablePanelWorld.transform.localPosition.x) * 0.2f, draggablePanelWorld.transform.localPosition.y, draggablePanelWorld.transform.localPosition.z);
			}
			else if (currentPanel == panelQG)
			{
				draggablePanelLostAndFound.transform.localPosition = new Vector3(draggablePanelLostAndFound.transform.localPosition.x, draggablePanelLostAndFound.transform.localPosition.y + ((float)(-selectedWorld) * -510f - draggablePanelLostAndFound.transform.localPosition.y) * 0.2f, draggablePanelLostAndFound.transform.localPosition.z);
			}
		}
		if (sensTransition)
		{
			ratioTransition += 3f * Time.deltaTime;
			if (ratioTransition > 1f)
			{
				ratioTransition = 1f;
				if (nextPanel == panelShop)
				{
					showShop();
					sensTransition = false;
				}
				else if (nextPanel == panelHome)
				{
					showHome();
					sensTransition = false;
				}
				else if (nextPanel == panelQG)
				{
					showQG();
					sensTransition = false;
				}
				else if (nextPanel == panelWorldSelect)
				{
					showWorldSelect();
					sensTransition = false;
				}
				else if (nextPanel == panelGame)
				{
					VarsManager.Instance.loadGame = true;
					VarsManager.Instance.GameOverCount = 0;
					Application.LoadLevel("SceneLoading");
				}
				else if (nextPanel == panelSettings)
				{
					showSettings();
					sensTransition = false;
				}
			}
		}
		else
		{
			ratioTransition -= 3f * Time.deltaTime;
			if (ratioTransition < 0f)
			{
				ratioTransition = 0f;
			}
		}
		if (ratioTransition != 0f)
		{
			spriteTransition.active = true;
			if (sensTransition)
			{
				if (spriteTransition.GetComponent<UISprite>().pivot != UIWidget.Pivot.Right)
				{
					spriteTransition.GetComponent<UISprite>().pivot = UIWidget.Pivot.Right;
				}
				if (spriteTransition.GetComponent<UIAnchor>().side != UIAnchor.Side.Right)
				{
					spriteTransition.GetComponent<UIAnchor>().side = UIAnchor.Side.Right;
				}
				spriteTransition.GetComponent<UIAnchor>().relativeOffset.x = 0.05f;
				spriteTransition.GetComponent<UIStretch>().relativeSize.x = ratioTransition * 5f;
			}
			else
			{
				if (spriteTransition.GetComponent<UISprite>().pivot != UIWidget.Pivot.Right)
				{
					spriteTransition.GetComponent<UISprite>().pivot = UIWidget.Pivot.Right;
				}
				if (spriteTransition.GetComponent<UIAnchor>().side != UIAnchor.Side.Left)
				{
					spriteTransition.GetComponent<UIAnchor>().side = UIAnchor.Side.Left;
				}
				spriteTransition.GetComponent<UIAnchor>().relativeOffset.x = -0.05f;
				spriteTransition.GetComponent<UIStretch>().relativeSize.x = (0f - ratioTransition) * 5f;
			}
		}
		else
		{
			spriteTransition.active = false;
		}
	}

	public void hidePanels()
	{
		if (currentPanel != panelHome)
		{
			panelHome.SetActiveRecursively(false);
		}
		if (currentPanel != panelWorldSelect)
		{
			panelWorldSelect.SetActiveRecursively(false);
		}
		if (currentPanel != panelShop)
		{
			panelShop.SetActiveRecursively(false);
		}
		if (currentPanel != panelQuit)
		{
			panelQuit.SetActiveRecursively(false);
		}
		if (currentPanel != panelQG)
		{
			panelQG.SetActiveRecursively(false);
		}
		if (currentPanel != panelSettings)
		{
			panelSettings.SetActiveRecursively(false);
		}
		if (currentPanel != panelHarder)
		{
			panelHarder.SetActiveRecursively(false);
		}
		if (currentPanel != panelChangeLog)
		{
			panelChangeLog.SetActiveRecursively(false);
		}
	}

	public void btnShowWorldSelect()
	{
		if (VarsManager.Instance.getProgression() == 1001)
		{
			VarsManager.Instance.levelToLoad = 1;
			VarsManager.Instance.worldToLoad = 1;
			PluginScript.Instance.hidePub();
			nextPanel = panelGame;
			sensTransition = true;
		}
		else
		{
			nextPanel = panelWorldSelect;
			sensTransition = true;
		}
		Sounds.snd_ui_play.Play();
	}

	public void showWorldSelect()
	{
		PluginScript.Instance.track("menu/worlds");
		VarsManager.Instance.goToLevelSelector = false;
		if (!VarsManager.Instance.hideAllPub())
		{
			PluginScript.Instance.showPub(0);
		}
		currentPanel = panelWorldSelect;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
		genAllItemWorld();
		selectedWorld = VarsManager.Instance.getProgression() / 1000 - 1;
		updateUIWorld();
		mCam.transform.position = currentPanel.transform.position;
	}

	public void btnShowShop()
	{
		Sounds.snd_ui_select.Play();
		nextPanel = panelShop;
		sensTransition = true;
	}

	private void UpdateShopItems()
	{
		bool flag = InAppEnabled();
		genAllItemShop();
		if (VarsManager.Instance.selectItemShop == -1)
		{
			for (int i = 0; i < listItemShop.Count; i++)
			{
				ItemShop itemShop = listItemShop[i];
				if (itemShop.categorie == shopCategorie)
				{
					if (flag)
					{
						selectedItemShop = i;
						break;
					}
					if (!itemShop.IsInApp || itemShop.prixPiece != -1)
					{
						selectedItemShop = i;
						break;
					}
				}
			}
		}
		else
		{
			selectedItemShop = VarsManager.Instance.selectItemShop;
		}
		updateUIShop();
	}

	public void btnTabWorlds()
	{
		Sounds.snd_ui_select.Play();
		if (shopCategorie != 0)
		{
			shopCategorie = 0;
			UpdateShopItems();
		}
	}

	public void btnTabCustom()
	{
		Sounds.snd_ui_select.Play();
		if (shopCategorie != 1)
		{
			shopCategorie = 1;
			UpdateShopItems();
		}
	}

	public void btnTabUpgrades()
	{
		Sounds.snd_ui_select.Play();
		if (shopCategorie != 2)
		{
			shopCategorie = 2;
			UpdateShopItems();
		}
	}

	public void showShop()
	{
		PluginScript.Instance.track("menu/shop");
		VarsManager.Instance.goToShop = false;
		PluginScript.Instance.hidePub();
		currentPanel = panelShop;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
		if (VarsManager.Instance.goToShopTabCustom)
		{
			shopCategorie = 1;
		}
		else
		{
			shopCategorie = 2;
		}
		VarsManager.Instance.goToShopTabCustom = false;
		UpdateShopItems();
		VarsManager.Instance.selectItemShop = -1;
		Musics.music_shop.PlayFade(0.2f);
	}

	public void btnShowHome()
	{
		nextPanel = panelHome;
		sensTransition = true;
		Sounds.snd_ui_select.Play();
	}

	public void showHome()
	{
		PluginScript.Instance.hidePub();
		currentPanel = panelHome;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
		panelDialogHome.SetActiveRecursively(false);
		if (GooglePlayServiceEnabled())
		{
			buttonAchievGooglePlus.GetComponentInChildren<UISprite>().spriteName = ((!GameServices.isSignedIn()) ? "GooglePlayGame_Off" : "GooglePlayGame_On");
			buttonGameServices.GetComponentInChildren<UISprite>().spriteName = "googleplus";
			buttonGameServices.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		}
		else
		{
			buttonGameServices.SetActiveRecursively(false);
			buttonAchievGooglePlus.SetActiveRecursively(false);
		}
		if (AppConfig.Platform == AppConfig.PlatformEnum.AndroidiDTGV)
		{
			GameObject.Find("SmallButtonAOTD").SetActiveRecursively(false);
			ButtonMoreGames.SetActiveRecursively(false);
			ButtonShare.SetActiveRecursively(false);
			ButtonFacebook.SetActiveRecursively(false);
			ButtonAppOfDay.SetActiveRecursively(false);
			ButtonNewsletter.SetActiveRecursively(false);
		}
		else if (Core.IsGoogleTV() || Core.IsLexibox())
		{
			GameObject.Find("SmallButtonAOTD").SetActiveRecursively(false);
			ButtonMoreGames.SetActiveRecursively(false);
			ButtonFacebook.SetActiveRecursively(false);
			ButtonAppOfDay.SetActiveRecursively(false);
			ButtonNewsletter.SetActiveRecursively(false);
			ButtonShare.SetActiveRecursively(false);
			buttonGameServices.SetActiveRecursively(false);
			buttonAchievGooglePlus.SetActiveRecursively(false);
		}
		if (Core.IsAmazon())
		{
			GameObject.Find("SmallButtonAOTD").SetActiveRecursively(false);
			ButtonAppOfDay.SetActiveRecursively(false);
			ButtonMoreGames.SetActiveRecursively(false);
			buttonGameServices.SetActiveRecursively(false);
			buttonAchievGooglePlus.SetActiveRecursively(false);
			if (!Core.IsGoogleTV())
			{
				ButtonFacebook.SetActiveRecursively(true);
				ButtonNewsletter.SetActiveRecursively(true);
				ButtonShare.SetActiveRecursively(true);
			}
		}
		SetGameModeLogo();
	}

	public void switchButtonGameServicesAndroid()
	{
		buttonAchievGooglePlus.GetComponentInChildren<UISprite>().spriteName = ((!GameServices.isSignedIn()) ? "GooglePlayGame_Off" : "GooglePlayGame_On");
	}

	public void switchButtonGameServicesAndroidToLogged()
	{
		buttonAchievGooglePlus.GetComponentInChildren<UISprite>().spriteName = "GooglePlayGame_On";
	}

	public void btnShowSettings()
	{
		nextPanel = panelSettings;
		sensTransition = true;
		Sounds.snd_ui_select.Play();
	}

	public void showSettings()
	{
		PluginScript.Instance.hidePub();
		if (!VarsManager.Instance.hideAllPub())
		{
			PluginScript.Instance.showPub(0);
		}
		currentPanel = panelSettings;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
		panelCredit.SetActiveRecursively(false);
		if (AudioPlayerEx.MusicEnabled)
		{
			chkMusic.spriteName = "CheckBox_Checked";
		}
		else
		{
			chkMusic.spriteName = "CheckBox_Base";
		}
		if (AudioPlayerEx.SoundEnabled)
		{
			chkSound.spriteName = "CheckBox_Checked";
		}
		else
		{
			chkSound.spriteName = "CheckBox_Base";
		}
		if (!InAppEnabled())
		{
			GameObject.Find("ButtonRestorePurchase").SetActiveRecursively(false);
		}
		UpdateDifficultyLabel();
	}

	public void showChangeLog()
	{
		PluginScript.Instance.track("menu/changelog");
		PluginScript.Instance.hidePub();
		currentPanel = panelChangeLog;
		hidePanels();
		panelChangeLog.SetActiveRecursively(true);
		if (AppConfig.Platform == AppConfig.PlatformEnum.AndroidiDTGV)
		{
			SmallButtonNews.SetActiveRecursively(false);
		}
		else if (Core.IsGoogleTV() || Core.IsLexibox() || Core.IsAmazon())
		{
			SmallButtonNews.SetActiveRecursively(false);
		}
		GameObject gameObject = GameObject.Find("LabelChangeLog");
		if (gameObject != null)
		{
			UILabel component = gameObject.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = ((TextAsset)Resources.Load("ChangeLogs/android", typeof(TextAsset))).text;
			}
		}
	}

	public void showHarder()
	{
		harderModeToUnlock = VarsManager.HasHarderMode();
		if (harderModeToUnlock > 0)
		{
			PluginScript.Instance.track("menu/harder");
			PluginScript.Instance.hidePub();
			currentPanel = panelHarder;
			hidePanels();
			currentPanel.SetActiveRecursively(true);
		}
	}

	public void showQuit()
	{
		if (Core.IsGoogleTV() || Core.IsLexibox())
		{
			int num = Mathf.FloorToInt(UIRoot.GetPixelSizeAdjustment(panelQuit.gameObject) * ((float)Screen.width * 0.5f));
			mContentQuit.transform.localPosition = new Vector3(num, 0f, 0f);
		}
		PluginScript.Instance.track("menu/quit");
		PluginScript.Instance.hidePub();
		if (!VarsManager.Instance.hideAllPub())
		{
			PluginScript.Instance.showPub(1);
		}
		currentPanel = panelQuit;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
	}

	public void btnShowQG()
	{
		nextPanel = panelQG;
		sensTransition = true;
	}

	public void showQG()
	{
		PluginScript.Instance.track("menu/qg");
		PluginScript.Instance.hidePub();
		UICamera.selectedObject = null;
		currentPanel = panelQG;
		hidePanels();
		currentPanel.SetActiveRecursively(true);
		hidePanelsQG();
		btnQGSkins();
	}

	public void btnShowLevel(GameObject button)
	{
		ItemLevel component = button.GetComponent<ItemLevel>();
		if (!(component == null))
		{
			if (!component.locked || VarsManager.Instance.masterOfTheKeys)
			{
				Sounds.snd_ui_postit.Play();
				VarsManager.Instance.levelToLoad = component.idLevel;
				VarsManager.Instance.worldToLoad = component.idWorld;
				PluginScript.Instance.hidePub();
				nextPanel = panelGame;
				sensTransition = true;
			}
			else
			{
				ShowBando("Padlock", TextManager.GetText("res_level_locked"), true);
			}
		}
	}

	public void ShowBando(string icon, string text, bool invert)
	{
		mBando.GetComponent<Bando>().showBando(icon, text, invert);
	}

	private void genAllItemWorld()
	{
		if (itemsWorld == null)
		{
			itemsWorld = new ItemWorld[3];
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject = Object.Instantiate(prefabItemWorld);
				Transform transform = gameObject.transform;
				transform.parent = uiGridWorld.transform;
				transform.localScale = Vector2.one;
				transform.localRotation = Quaternion.identity;
				transform.localPosition = new Vector3(i * Screen.height * 4, 0f, 0f);
				itemsWorld[i] = gameObject.GetComponent<ItemWorld>();
			}
		}
		for (int j = 0; j < 3; j++)
		{
			itemsWorld[j].setItemWorld2(j + 1);
		}
	}

	public void genAllItemAchiev()
	{
		if (uiGridAchiev.transform.childCount == 0)
		{
			for (int i = 0; i < VarsManager.Instance.listAchievs.Count; i++)
			{
				if (!VarsManager.Instance.listAchievs[i].disabled)
				{
					genItemAchiev(i);
				}
			}
			uiGridAchiev.repositionNow = true;
		}
		else
		{
			for (int j = 0; j < uiGridAchiev.transform.childCount; j++)
			{
				uiGridAchiev.transform.GetChild(j).gameObject.GetComponent<ItemAchiev>().setItemAchiev(-1);
			}
		}
	}

	private void genItemAchiev(int id)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(prefabItemAchievement, Vector3.zero, Quaternion.identity);
		gameObject.transform.parent = uiGridAchiev.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.GetComponent<ItemAchiev>().setItemAchiev(id);
	}

	public void dragWorldG()
	{
		selectedWorld--;
		Sounds.snd_ui_select.Play();
		updateUIWorld();
	}

	public void dragWorldD()
	{
		selectedWorld++;
		Sounds.snd_ui_select.Play();
		updateUIWorld();
	}

	private void updateUIWorld()
	{
		if (selectedWorld < 0)
		{
			selectedWorld = 0;
		}
		else if (selectedWorld > 2)
		{
			selectedWorld = 2;
		}
		if (currentPanel == panelWorldSelect)
		{
			if (selectedWorld == 0)
			{
				textureFondWorld.mainTexture = Instance.textureWorld1;
				Sounds.snd_ambiant_jungle.Play();
			}
			else if (selectedWorld == 1)
			{
				textureFondWorld.mainTexture = Instance.textureWorld2;
				Sounds.snd_ambiant_desert.Play();
			}
			else if (selectedWorld == 2)
			{
				textureFondWorld.mainTexture = Instance.textureWorld3;
				Sounds.snd_ambiant_mountain.Play();
			}
			if (selectedWorld == 0)
			{
				buttonWorldLeft.SetActiveRecursively(false);
			}
			else
			{
				buttonWorldLeft.SetActiveRecursively(true);
			}
			if (selectedWorld == 2)
			{
				buttonWorldRight.SetActiveRecursively(false);
			}
			else
			{
				buttonWorldRight.SetActiveRecursively(true);
			}
			int num = selectedWorld + 1;
			if (!VarsManager.Instance.getObjetCache(num, 0))
			{
				spritePetCache1.gameObject.active = true;
			}
			else
			{
				spritePetCache1.gameObject.active = false;
			}
			if (!VarsManager.Instance.getObjetCache(num, 1))
			{
				spritePetCache2.gameObject.active = true;
			}
			else
			{
				spritePetCache2.gameObject.active = false;
			}
			if (!VarsManager.Instance.getObjetCache(num, 2))
			{
				spritePetCache3.gameObject.active = true;
			}
			else
			{
				spritePetCache3.gameObject.active = false;
			}
			if (!VarsManager.Instance.getObjetCache(num, 3))
			{
				spritePetCache4.gameObject.active = true;
			}
			else
			{
				spritePetCache4.gameObject.active = false;
			}
			if (!VarsManager.Instance.getObjetCache(num, 4))
			{
				spritePetCache5.gameObject.active = true;
			}
			else
			{
				spritePetCache5.gameObject.active = false;
			}
			if (VarsManager.Instance.getObjetShopStep("extralevel" + num) == 1)
			{
				spritePetCache1.gameObject.active = false;
				spritePetCache2.gameObject.active = false;
				spritePetCache3.gameObject.active = false;
				spritePetCache4.gameObject.active = false;
				spritePetCache5.gameObject.active = false;
				spritePetCache6.gameObject.active = false;
			}
			else
			{
				spritePetCache6.gameObject.active = true;
			}
			labelSelectWorldName.text = TextManager.GetText("res_title_monde" + num);
		}
		else if (currentPanel == panelQG && panelQGLostAndFound.active)
		{
			if (selectedWorld == 0)
			{
				buttonWorldQGUp.SetActiveRecursively(false);
			}
			else
			{
				buttonWorldQGUp.SetActiveRecursively(true);
			}
			if (selectedWorld == 2)
			{
				buttonWorldQGDown.SetActiveRecursively(false);
			}
			else
			{
				buttonWorldQGDown.SetActiveRecursively(true);
			}
		}
		for (int i = 0; i < 3; i++)
		{
			uiGridWorld.transform.GetChild(i).gameObject.SetActive(false);
		}
		if (Core.IsGoogleTV() || Core.IsLexibox())
		{
			buttonWorldLeft.GetComponent<UIButtonKeysExt>().selectOnDisable = uiGridWorld.transform.GetChild(selectedWorld).GetComponent<ItemWorld>().uiGridLevel.transform.GetChild(0).GetComponent<UIButtonKeysExt>();
			buttonWorldRight.GetComponent<UIButtonKeysExt>().selectOnDisable = uiGridWorld.transform.GetChild(selectedWorld).GetComponent<ItemWorld>().uiGridLevel.transform.GetChild(0).GetComponent<UIButtonKeysExt>();
		}
		uiGridWorld.transform.GetChild(selectedWorld).transform.localPosition = new Vector3(4 * Screen.height * selectedWorld, 0f, 0f);
		uiGridWorld.transform.GetChild(selectedWorld).gameObject.SetActive(true);
	}

	private bool GooglePlayServiceEnabled()
	{
		return AppConfig.Platform == AppConfig.PlatformEnum.AndroidGoogle;
	}

	private void genAllItemShop()
	{
		listItemShop.Clear();
		addIS("extralevel1", 0, new int[1], 1000, "res_shop_extralevel1", null, 1, "PostalCard_Jungle", "com.magmamobile.game.fireman.extralevel1", "com.magmamobile.game.fireman.ExtraLevel1", string.Empty);
		addIS("extralevel2", 0, new int[1], 1000, "res_shop_extralevel2", null, 1, "PostalCard_Desert", "com.magmamobile.game.fireman.extralevel2", "com.magmamobile.game.fireman.ExtraLevel2", string.Empty);
		addIS("extralevel3", 0, new int[1], 1000, "res_shop_world3_desc", null, 1, "PostalCard_Mountain", "com.magmamobile.game.fireman.extralevel3", "com.magmamobile.game.fireman.ExtraLevel3", string.Empty);
		addIS("custom_fireman_skin1", 1, new int[1], 500, "res_inapp2_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin1", "com.magmamobile.game.fireman.ExtraSkin1", string.Empty);
		addIS("custom_fireman_skin2", 1, new int[1], 550, "res_inapp3_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin2", "com.magmamobile.game.fireman.ExtraSkin2", string.Empty);
		addIS("custom_fireman_skin3", 1, new int[1], 550, "res_shop_skin3_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin3", "com.magmamobile.game.fireman.Extraskin3", string.Empty);
		addIS("custom_fireman_skin4", 1, new int[1], 550, "res_shop_skin4_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin4", "com.magmamobile.game.fireman.ExtraSkin4", string.Empty);
		addIS("custom_fireman_skin5", 1, new int[1], 550, "res_shop_skin5_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin5", "com.magmamobile.game.fireman.ExtraSkin5", string.Empty);
		addIS("custom_fireman_skin6", 1, new int[1], 550, "res_shop_skin6_desc", null, 1, "spinePompier", "com.magmamobile.game.fireman.extraskin6", "com.magmamobile.game.fireman.ExtraSkin6", string.Empty);
		addIS("custom_pet1", 1, new int[1], 200, "res_inapp8_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet2", "com.magmamobile.game.fireman.Pet2", string.Empty);
		addIS("custom_pet2", 1, new int[1], 250, "res_inapp7_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet1", "com.magmamobile.game.fireman.Pet1", string.Empty);
		addIS("custom_pet3", 1, new int[1], 250, "res_inapp9_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet3", "com.magmamobile.game.fireman.Pet3", string.Empty);
		addIS("custom_pet4", 1, new int[1], 250, "res_shop_pet4_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet4", "com.magmamobile.game.fireman.Pet4", string.Empty);
		addIS("custom_pet5", 1, new int[1], 250, "res_shop_pet5_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet5", "com.magmamobile.game.fireman.Pet5", string.Empty);
		addIS("custom_pet6", 1, new int[1], 250, "res_shop_pet6_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet6", "com.magmamobile.game.fireman.Pet6", string.Empty);
		addIS("custom_pet7", 1, new int[1], 250, "res_shop_pet7_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet7", "com.magmamobile.game.fireman.Pet7", string.Empty);
		addIS("custom_pet8", 1, new int[1], 250, "res_shop_pet8_desc", null, 1, "spinePet", "com.magmamobile.game.fireman.pet8", "com.magmamobile.game.fireman.Pet8", string.Empty);
		addIS("upgrade_all", 2, new int[1], -1, "res_shop_all", null, 1, "Present", "com.magmamobile.game.fireman.all", "com.magmamobile.game.fireman.All", string.Empty);
		addIS("upgrade_canon", 2, new int[3] { 1, 4, 7 }, 90, "res_shop_range", null, 3, "Blaster", null, null, null);
		addIS("upgrade_jetpack", 2, new int[3] { 2, 5, 8 }, 100, "res_shop_reservoir", null, 3, "Jetpack", null, null, null);
		addIS("upgrade_life", 2, new int[3] { 3, 6, 9 }, 125, "res_shop_life", null, 3, "Bonus_Life", null, null, null);
		addIS("upgrade_xpx2", 2, new int[1], -1, "res_shop_xpx2", null, 1, "Bonus_Xp", "com.magmamobile.game.fireman.doublexp", "com.magmamobile.game.fireman.DoubleXP", string.Empty);
		addIS("upgrade_piecex2", 2, new int[1], -1, "res_shop_piecex2", null, 1, "Bonus_Coins", "com.magmamobile.game.fireman.doublemoney", "com.magmamobile.game.fireman.DoubleMoney", string.Empty);
		addIS("upgrade_piecesxp", 2, new int[1], -1, "res_shop_maxipack", null, 1, "Bonus_CoinsXp", "com.magmamobile.game.fireman.doublexpmoney", "com.magmamobile.game.fireman.DoubleXP", string.Empty);
	}

	private void addIS(string id, int cat, int[] rangRequired, int prixPiece, string title, string desc, int stepMax, string spriteName, string id_googleplay, string id_itunes, string id_wp8store)
	{
		listItemShop.Add(new ItemShop(id, cat, rangRequired, prixPiece, title, desc, stepMax, spriteName, id_googleplay, id_itunes, id_wp8store));
	}

	private bool InAppEnabled()
	{
		return (AppConfig.Platform == AppConfig.PlatformEnum.AndroidGoogle || AppConfig.Platform == AppConfig.PlatformEnum.iOS || AppConfig.Platform == AppConfig.PlatformEnum.WindowPhone8) && !Core.IsAmazon() && !Core.IsLexibox() && !Core.IsGoogleTV();
	}

	public void btnBuyInApp(GameObject g)
	{
		listItemShop[selectedItemShop].buy(g);
	}

	public void btnBuyPiece(GameObject g)
	{
		listItemShop[selectedItemShop].buy(g);
	}

	private void prevShopItem()
	{
		selectedItemShop--;
		if (selectedItemShop < 0)
		{
			selectedItemShop = listItemShop.Count - 1;
		}
	}

	private void nextShopItem()
	{
		selectedItemShop++;
		if (selectedItemShop > listItemShop.Count - 1)
		{
			selectedItemShop = 0;
		}
	}

	public void btnShopLeft()
	{
		prevShopItem();
		if (!InAppEnabled())
		{
			while (listItemShop[selectedItemShop].IsInApp && listItemShop[selectedItemShop].prixPiece == -1)
			{
				prevShopItem();
			}
		}
		Sounds.snd_ui_select.Play();
		updateUIShop();
	}

	public void btnShopRight()
	{
		nextShopItem();
		if (!InAppEnabled())
		{
			while (listItemShop[selectedItemShop].IsInApp && listItemShop[selectedItemShop].prixPiece == -1)
			{
				nextShopItem();
			}
		}
		Sounds.snd_ui_select.Play();
		updateUIShop();
	}

	public void updateUIShop()
	{
		if (listItemShop[selectedItemShop].categorie != shopCategorie)
		{
			shopCategorie = listItemShop[selectedItemShop].categorie;
		}
		for (int i = 0; i < listItemShop.Count; i++)
		{
			listItemShop[i].updateStep();
		}
		if (shopCategorie == 0)
		{
			spriteTabLevels.color = new Color(1f, 1f, 1f, 1f);
			spriteTabCustoms.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			spriteTabUpgrades.color = new Color(0.7f, 0.7f, 0.7f, 1f);
		}
		else if (shopCategorie == 1)
		{
			spriteTabLevels.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			spriteTabCustoms.color = new Color(1f, 1f, 1f, 1f);
			spriteTabUpgrades.color = new Color(0.7f, 0.7f, 0.7f, 1f);
		}
		else if (shopCategorie == 2)
		{
			spriteTabLevels.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			spriteTabCustoms.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			spriteTabUpgrades.color = new Color(1f, 1f, 1f, 1f);
		}
		spriteShopCheck.gameObject.active = false;
		int num = listItemShop[selectedItemShop].step;
		if (num >= listItemShop[selectedItemShop].stepMax)
		{
			num = listItemShop[selectedItemShop].stepMax - 1;
		}
		textBulleShop = TextManager.GetText(listItemShop[selectedItemShop].desc).Replace("%s", string.Empty + (num + 1));
		textBulleShopIndex = 0;
		timeSpeakDocShop = (float)labelNomItemShop.text.Length / 10f;
		Sounds.snd_ui_shop_talk01.Stop();
		Sounds.snd_ui_shop_talk02.Stop();
		Sounds.snd_ui_shop_talk03.Stop();
		Sounds.snd_ui_shop_talk04.Stop();
		if (listItemShop[selectedItemShop].IsInApp && InAppEnabled())
		{
			buttonPrixInApp.gameObject.SetActiveRecursively(true);
			labelPrixInApp.gameObject.active = true;
			labelPrixInApp.text = string.Empty + listItemShop[selectedItemShop].prixInApp;
			if (labelPrixInApp.text.Length == 0)
			{
				labelPrixInApp.text = TextManager.GetText("res_buy");
			}
		}
		else
		{
			buttonPrixInApp.gameObject.SetActiveRecursively(false);
		}
		if (listItemShop[selectedItemShop].prixPiece != -1)
		{
			buttonPrixPiece.gameObject.SetActiveRecursively(true);
			labelPrixPiece.gameObject.active = true;
			labelPrixPiece.text = string.Empty + listItemShop[selectedItemShop].prixPiece * (listItemShop[selectedItemShop].step + 1);
		}
		else
		{
			buttonPrixPiece.gameObject.SetActiveRecursively(false);
		}
		if (listItemShop[selectedItemShop].step == listItemShop[selectedItemShop].stepMax)
		{
			buttonPrixPiece.gameObject.SetActiveRecursively(false);
			buttonPrixInApp.gameObject.SetActiveRecursively(false);
		}
		if (!buttonPrixPiece.gameObject.active && buttonPrixInApp.gameObject.active)
		{
			buttonPrixInApp.GetComponent<UIAnchor>().relativeOffset.y = 0.07f;
		}
		if (buttonPrixPiece.gameObject.active && !buttonPrixInApp.gameObject.active)
		{
			buttonPrixPiece.GetComponent<UIAnchor>().relativeOffset.y = 0.07f;
		}
		if (buttonPrixPiece.gameObject.active && buttonPrixInApp.gameObject.active)
		{
			buttonPrixPiece.GetComponent<UIAnchor>().relativeOffset.y = 0.14f;
			buttonPrixInApp.GetComponent<UIAnchor>().relativeOffset.y = 0f;
		}
		if (listItemShop[selectedItemShop].step < listItemShop[selectedItemShop].stepMax && listItemShop[selectedItemShop].step != -1)
		{
			if (VarsManager.Instance.rang < listItemShop[selectedItemShop].rangRequired[listItemShop[selectedItemShop].step])
			{
				if (buttonPrixPiece.active)
				{
					buttonPrixPiece.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_Gris";
				}
				if (buttonPrixInApp.active)
				{
					buttonPrixInApp.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_Gris";
				}
			}
			else
			{
				if (buttonPrixPiece.active)
				{
					buttonPrixPiece.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_CoinsPrice";
				}
				if (buttonPrixInApp.active)
				{
					buttonPrixInApp.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_ReelPrice";
				}
			}
		}
		else if (listItemShop[selectedItemShop].step == -1)
		{
			if (buttonPrixPiece.active)
			{
				buttonPrixPiece.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_Gris";
			}
			if (buttonPrixInApp.active)
			{
				buttonPrixInApp.GetComponentInChildren<UISprite>().spriteName = "Shop_Button_Gris";
			}
		}
		labelPiece.text = string.Empty + VarsManager.Instance.getCoins();
		if (listItemShop[selectedItemShop].spriteName.Equals("spinePompier"))
		{
			if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin1"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 1, 1);
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin2"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 2, 2);
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin3"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 3, 3);
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin4"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 4, 4);
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin5"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 5, 5);
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_fireman_skin6"))
			{
				VarsManager.Instance.commonPompierSkinUpdater(shopAnimSkin, 6, 6);
			}
			spriteItemShop.alpha = 0f;
			shopAnimPet.GetComponent<Renderer>().enabled = false;
			shopAnimSkin.GetComponent<Renderer>().enabled = true;
		}
		else if (listItemShop[selectedItemShop].spriteName.Equals("spinePet"))
		{
			shopAnimPet.GetComponent<Renderer>().enabled = true;
			shopAnimPet.skeleton.SetToSetupPose();
			int index = 0;
			if (listItemShop[selectedItemShop].id.Equals("custom_pet1"))
			{
				index = 0;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet2"))
			{
				index = 1;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet3"))
			{
				index = 2;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet4"))
			{
				index = 3;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet5"))
			{
				index = 4;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet6"))
			{
				index = 5;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet7"))
			{
				index = 6;
			}
			else if (listItemShop[selectedItemShop].id.Equals("custom_pet8"))
			{
				index = 7;
			}
			Pets.Setup(shopAnimPet, index);
			spriteItemShop.alpha = 0f;
			shopAnimSkin.GetComponent<Renderer>().enabled = false;
			shopAnimPet.skeleton.SetSlotsToSetupPose();
		}
		else
		{
			shopAnimSkin.GetComponent<Renderer>().enabled = false;
			shopAnimPet.GetComponent<Renderer>().enabled = false;
			string text = listItemShop[selectedItemShop].spriteName;
			int num2 = listItemShop[selectedItemShop].step;
			if (num2 >= listItemShop[selectedItemShop].stepMax)
			{
				num2 = listItemShop[selectedItemShop].stepMax - 1;
			}
			if (text.Equals("Jetpack"))
			{
				text += num2 + 1;
			}
			if (text.Equals("Blaster"))
			{
				text += num2 + 1;
			}
			if (text.Equals("Bonus_Life"))
			{
				text += num2 + 1;
			}
			spriteItemShop.spriteName = text;
			spriteItemShop.alpha = 1f;
			spriteItemShop.MakePixelPerfect();
		}
	}

	public void btnQGSkins()
	{
		if (!panelQGSkins.active)
		{
			hidePanelsQG();
			panelQGSkins.SetActiveRecursively(true);
			Sounds.snd_ui_select.Play();
			updateSkinPompier(0, 0);
			buttonQGSkins.GetComponentInChildren<UISprite>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			buttonQGAchievements.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGPets.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGLostAndFound.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGRangs.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
		}
	}

	public void btnQGAchievements()
	{
		if (!panelQGAchievements.active)
		{
			hidePanelsQG();
			panelQGAchievements.SetActiveRecursively(true);
			Sounds.snd_ui_select.Play();
			buttonQGSkins.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGAchievements.GetComponentInChildren<UISprite>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			buttonQGPets.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGLostAndFound.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGRangs.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			genAllItemAchiev();
		}
	}

	public void btnQGPets()
	{
		if (!panelQGPets.active)
		{
			hidePanelsQG();
			panelQGPets.SetActiveRecursively(true);
			if (VarsManager.Instance.getPetSkin() == 0)
			{
				selectedPet = 1;
			}
			Sounds.snd_ui_select.Play();
			updateQGPet();
			buttonQGSkins.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGAchievements.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGPets.GetComponentInChildren<UISprite>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			buttonQGLostAndFound.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGRangs.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
		}
	}

	public void btnQGLostAndFound()
	{
		if (!panelQGLostAndFound.active)
		{
			hidePanelsQG();
			panelQGLostAndFound.SetActiveRecursively(true);
			Sounds.snd_ui_select.Play();
			buttonQGSkins.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGAchievements.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGPets.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGLostAndFound.GetComponentInChildren<UISprite>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
			buttonQGRangs.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			updateUIWorld();
		}
	}

	public void btnQGRangs()
	{
		PluginScript.Instance.track("menu/qg/ranks");
		if (!panelQGRangs.active)
		{
			hidePanelsQG();
			panelQGRangs.SetActiveRecursively(true);
			Sounds.snd_ui_select.Play();
			buttonQGSkins.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGAchievements.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGPets.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGLostAndFound.GetComponentInChildren<UISprite>().color = new Color(1f, 1f, 1f, 1f);
			buttonQGRangs.GetComponentInChildren<UISprite>().color = new Color(0.8f, 0.8f, 0.8f, 1f);
		}
	}

	public void hidePanelsQG()
	{
		panelQGAchievements.SetActiveRecursively(false);
		panelQGLostAndFound.SetActiveRecursively(false);
		panelQGPets.SetActiveRecursively(false);
		panelQGSkins.SetActiveRecursively(false);
		panelQGRangs.SetActiveRecursively(false);
	}

	public void btnQGSkinHeadLeft()
	{
		Sounds.snd_ui_select.Play();
		updateSkinPompier(-1, 0);
	}

	public void btnQGSkinHeadRight()
	{
		Sounds.snd_ui_select.Play();
		updateSkinPompier(1, 0);
	}

	public void btnQGSkinJacketLeft()
	{
		Sounds.snd_ui_select.Play();
		updateSkinPompier(0, -1);
	}

	public void btnQGSkinJacketRight()
	{
		Sounds.snd_ui_select.Play();
		updateSkinPompier(0, 1);
	}

	public int maxUnlockedSkin()
	{
		int num = 0;
		for (int i = 1; i < 7; i++)
		{
			if (VarsManager.Instance.getObjetShopStep("custom_fireman_skin" + i) > 0)
			{
				num++;
			}
		}
		return num;
	}

	private void updateSkinPompier(int changeHead, int changeJacket)
	{
		int num = VarsManager.Instance.getSkinHead();
		int num2 = VarsManager.Instance.getSkinJacket();
		if (num + changeHead <= 6 && num + changeHead >= 0)
		{
			num += changeHead;
		}
		while (num != 0 && VarsManager.Instance.getObjetShopStep("custom_fireman_skin" + num) != 1 && changeHead != 0)
		{
			num += changeHead;
			if (num > 6)
			{
				num = 0;
				break;
			}
			if (num < 0)
			{
				num = 0;
				break;
			}
		}
		VarsManager.Instance.setSkinHead(num);
		if (num2 + changeJacket <= 6 && num2 + changeJacket >= 0)
		{
			num2 += changeJacket;
		}
		while (num2 != 0 && VarsManager.Instance.getObjetShopStep("custom_fireman_skin" + num2) != 1 && changeJacket != 0)
		{
			num2 += changeJacket;
			if (num2 > 6)
			{
				num2 = 0;
				break;
			}
			if (num2 < 0)
			{
				num2 = 0;
				break;
			}
		}
		VarsManager.Instance.setSkinJacket(num2);
		int num3 = maxUnlockedSkin();
		if (num <= 0)
		{
			buttonQGSkinHeadLeft.gameObject.SetActive(false);
		}
		else
		{
			buttonQGSkinHeadLeft.gameObject.SetActive(true);
		}
		if (num >= num3)
		{
			buttonQGSkinHeadRight.gameObject.SetActive(false);
		}
		else
		{
			buttonQGSkinHeadRight.gameObject.SetActive(true);
		}
		if (num2 <= 0)
		{
			buttonQGSkinJacketLeft.gameObject.SetActive(false);
		}
		else
		{
			buttonQGSkinJacketLeft.gameObject.SetActive(true);
		}
		if (num2 >= num3)
		{
			buttonQGSkinJacketRight.gameObject.SetActive(false);
		}
		else
		{
			buttonQGSkinJacketRight.gameObject.SetActive(true);
		}
		VarsManager.Instance.commonPompierSkinUpdater(qgAnimSkin, num, num2);
	}

	public void btnQGPetLeft()
	{
		if (selectedPet > 0)
		{
			selectedPet--;
		}
		Sounds.snd_ui_select.Play();
		updateQGPet();
	}

	public void btnQGPetRight()
	{
		if (selectedPet < 8)
		{
			selectedPet++;
		}
		Sounds.snd_ui_select.Play();
		updateQGPet();
	}

	private void updateQGPet()
	{
		if (selectedPet == 0)
		{
			buttonQGPetLeft.gameObject.SetActive(false);
		}
		else
		{
			buttonQGPetLeft.gameObject.SetActive(true);
		}
		if (selectedPet == 8)
		{
			buttonQGPetRight.gameObject.SetActive(false);
		}
		else
		{
			buttonQGPetRight.gameObject.SetActive(true);
		}
		int petSkin = VarsManager.Instance.getPetSkin();
		if (petSkin == selectedPet)
		{
			spriteQGPetCheck.gameObject.active = true;
		}
		else
		{
			spriteQGPetCheck.gameObject.active = false;
		}
		if (VarsManager.Instance.getObjetShopStep("custom_pet" + selectedPet) == 0 && VarsManager.Instance.getPetSkin() != selectedPet && selectedPet != 0)
		{
			boutonQGPetBuyIt.gameObject.SetActiveRecursively(true);
			buttonQGPetChoose.gameObject.SetActiveRecursively(false);
		}
		else
		{
			boutonQGPetBuyIt.gameObject.SetActiveRecursively(false);
			if (petSkin != selectedPet)
			{
				buttonQGPetChoose.gameObject.SetActiveRecursively(true);
			}
			else
			{
				buttonQGPetChoose.gameObject.SetActiveRecursively(false);
			}
		}
		if (selectedPet == 0)
		{
			spriteNoPets.gameObject.active = true;
			qgAnimPet.GetComponent<Renderer>().enabled = false;
			spriteQGPetCheck.gameObject.active = false;
			boutonQGPetBuyIt.gameObject.SetActiveRecursively(false);
		}
		else
		{
			spriteNoPets.gameObject.active = false;
			qgAnimPet.GetComponent<Renderer>().enabled = true;
			Pets.Setup(qgAnimPet, selectedPet - 1);
		}
	}

	public void btnChoosePet()
	{
		VarsManager.Instance.setPetSkin(selectedPet);
		Sounds.snd_ui_select.Play();
		updateQGPet();
	}

	public void btnGoShopQGPet()
	{
		PluginScript.Instance.track("menu/qg/pets");
		VarsManager.Instance.goToShopTabCustom = true;
		Sounds.snd_ui_select.Play();
		VarsManager.Instance.selectItemShop = 8 + selectedPet;
		btnShowShop();
	}

	public void OnClickBtnNewsLetter()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/newsletter");
		Application.OpenURL("http://m.magmamobile.com/redir.aspx?quid=nl&os=Android&source=Fireman");
	}

	public void OnClickBtnGameService()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/gameservices");
		if (!GameServices.isSignedIn())
		{
			PluginScript.Instance.GPGSLogin();
			return;
		}
		UICamera.selectedObject = null;
		panelDialogHome.SetActiveRecursively(true);
		panelDialogHome.GetComponentInChildren<UILabel>().text = TextManager.GetText("res_ask_logout_gp");
	}

	public void OnClickBtnAchievGooglePlus()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/gameservicesachievandroid");
		if (!GameServices.isSignedIn())
		{
			UICamera.selectedObject = null;
			panelDialogHome.SetActiveRecursively(true);
			panelDialogHome.GetComponentInChildren<UILabel>().text = TextManager.GetText("res_ask_login_gp");
		}
		else
		{
			PluginScript.Instance.showAchievements();
		}
	}

	public void btnNoDialogHome()
	{
		PluginScript.Instance.track("menu/playgamesno");
		panelDialogHome.SetActiveRecursively(false);
	}

	public void btnYesDialogHome()
	{
		PluginScript.Instance.track("menu/playgamesyes");
		panelDialogHome.SetActiveRecursively(false);
		if (!GameServices.isSignedIn())
		{
			PluginScript.Instance.GPGSLogin();
			return;
		}
		PluginScript.Instance.GPGSLogout();
		buttonAchievGooglePlus.GetComponentInChildren<UISprite>().spriteName = "GooglePlayGame_Off";
	}

	public void OnClickBtnFacebook()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/facebook");
		if (Core.HasFacebookApp())
		{
			Application.OpenURL("fb://page/121554497873951");
		}
		else
		{
			Application.OpenURL("https://m.facebook.com/121554497873951");
		}
		VarsManager.Instance.setAchievement(0, 1, false);
	}

	public void OnClickBtnShare()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/share");
		string text = TextManager.GetText("res_share");
		text = text.Replace("%1s", TextManager.GetText("app_name"));
		text = text.Replace("%2s", "http://goo.gl/rRWgFF");
		PluginScript.Instance.share(text, TextManager.GetText("app_name"), TextManager.GetText("app_name"), "http://m.magmamobile.com/FireMan");
		VarsManager.Instance.setAchievement(1, 1, false);
	}

	public void btnYesChangeLog()
	{
		Sounds.snd_ui_select.Play();
		showHome();
	}

	public void btnYesHarder()
	{
		Sounds.snd_mob2_kill.Play();
		VarsManager.UnlockGameMode(harderModeToUnlock);
		VarsManager.GameMode = harderModeToUnlock;
		genAllItemWorld();
		showHome();
	}

	public void btnYesQuit()
	{
		Sounds.snd_ui_select.Play();
		Application.Quit();
	}

	public void btnNoQuit()
	{
		Sounds.snd_ui_select.Play();
		showHome();
	}

	public void OnClickBtnSettings()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/settings");
		nextPanel = panelSettings;
		sensTransition = true;
	}

	public void OnClickCredit()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/credits");
		panelCredit.SetActiveRecursively(true);
		panelSettingsElements.SetActiveRecursively(false);
		MakeCreditsAnimation();
	}

	public void OnClickBtnMoreGames()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("home/moregames");
		PluginScript.Instance.showMoreGames();
		VarsManager.Instance.setAchievement(2, 1, false);
		Interstial.Show();
	}

	public void OnClickRestorePurchases()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/restorepurchases");
		PluginScript.Instance.restorePurchases();
	}

	public void OnClickBtnNewsChangeLog()
	{
		Sounds.snd_ui_select.Play();
		PluginScript.Instance.track("menu/news");
		PluginScript.Instance.showNews();
	}

	public void OnClickBtnNews()
	{
		Sounds.snd_ui_select.Play();
		showChangeLog();
	}

	public void OnClickPrivacy()
	{
		PluginScript.Instance.track("settings/privacypolicy");
		Application.OpenURL("http://www.magmamobile.com/privacypolicy");
	}

	public void OnToggleSound()
	{
		Sounds.snd_ui_select.Play();
		AudioPlayerEx.toggleSound();
		if (AudioPlayerEx.SoundEnabled)
		{
			PluginScript.Instance.track("settings/sound/on");
			chkSound.spriteName = "CheckBox_Checked";
		}
		else
		{
			PluginScript.Instance.track("settings/sound/off");
			chkSound.spriteName = "CheckBox_Base";
		}
	}

	public void OnToggleMusic()
	{
		Sounds.snd_ui_select.Play();
		AudioPlayerEx.toggleMusic();
		if (AudioPlayerEx.MusicEnabled)
		{
			PluginScript.Instance.track("settings/music/on");
			chkMusic.spriteName = "CheckBox_Checked";
		}
		else
		{
			PluginScript.Instance.track("settings/music/off");
			chkMusic.spriteName = "CheckBox_Base";
		}
	}

	public void OnToggleMode()
	{
		if (VarsManager.GameMode == 1 && VarsManager.IsGameModeAvailable(2))
		{
			VarsManager.GameMode = 2;
		}
		else if (VarsManager.GameMode == 2 && VarsManager.IsGameModeAvailable(4))
		{
			VarsManager.GameMode = 4;
		}
		else if (VarsManager.GameMode == 2 && !VarsManager.IsGameModeAvailable(4))
		{
			VarsManager.GameMode = 1;
		}
		else if (VarsManager.GameMode == 4)
		{
			VarsManager.GameMode = 1;
		}
		UpdateDifficultyLabel();
	}

	private void UpdateDifficultyLabel()
	{
		if (VarsManager.HasOtherGameMode())
		{
			if (VarsManager.GameMode == 4)
			{
				lblDifficulty.text = TextManager.GetText("res_difficulty") + ": Extreme";
			}
			else if (VarsManager.GameMode == 2)
			{
				lblDifficulty.text = TextManager.GetText("res_difficulty") + ": Hardcore";
			}
			else
			{
				lblDifficulty.text = TextManager.GetText("res_difficulty") + ": Normal";
			}
			btnDifficulty.gameObject.SetActive(true);
			lblDifficulty.enabled = true;
		}
		else
		{
			btnDifficulty.gameObject.SetActive(false);
			lblDifficulty.enabled = false;
		}
	}

	public override void OnButtonClick(UIButtonClick sender)
	{
		if (sender.name == "ButtonPrivacyPolicy")
		{
			OnClickPrivacy();
		}
		else if (sender.name == "ButtonRestorePurchase")
		{
			OnClickRestorePurchases();
		}
		else if (sender.name == "ButtonCredit")
		{
			OnClickCredit();
		}
		else if (sender.name == "ButtonMusic")
		{
			OnToggleMusic();
		}
		else if (sender.name == "ButtonSound")
		{
			OnToggleSound();
		}
		else if (sender.name == "ButtonDifficulty")
		{
			OnToggleMode();
		}
	}

	private void MakeCreditsAnimation()
	{
		StringBuilder stringBuilder = new StringBuilder();
		UILabel component = GameObject.Find("LabelCredits").GetComponent<UILabel>();
		TweenPosition component2 = component.gameObject.GetComponent<TweenPosition>();
		component2.to = new Vector3(0f, 2400f, 0f);
		component2.Reset();
		stringBuilder.Append("[FFDC00]STAFF\n\n");
		stringBuilder.Append("[FFDC00]Magma Mobile CEO\n[FFFFFF]");
		stringBuilder.Append("Nicolas SOREL\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Magma Mobile CTO\n[FFFFFF]");
		stringBuilder.Append("Emmanuel ROBLES\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Team Leader\n[FFFFFF]");
		stringBuilder.Append("Loïc KEMPF\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Developer\n[FFFFFF]");
		stringBuilder.Append("Clément SLEMPKES\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Sound Designer\n[FFFFFF]");
		stringBuilder.Append("Carl MALHERBE\n");
		stringBuilder.Append("Jérémie GUIGUE\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Game Artist\n[FFFFFF]");
		stringBuilder.Append("Sandra FERRAGU\n");
		stringBuilder.Append("Elise URBAN\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Level Designer\n[FFFFFF]");
		stringBuilder.Append("Tatiana GRANGE\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]Game Testers\n[FFFFFF]");
		stringBuilder.Append("Maxime AUDOUIN\n");
		stringBuilder.Append("Laëtitia BAÏSE\n");
		stringBuilder.Append("Olivier BOUCHEZ\n");
		stringBuilder.Append("Christophe MEZERETTE\n");
		stringBuilder.Append("Franck MOLTO\n");
		stringBuilder.Append("Thierry PERRAULT\n");
		stringBuilder.Append("Arthur WENDLING\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]DEV TOOLS\n[FFFFFF]");
		stringBuilder.Append("Visual Basic 6\n");
		stringBuilder.Append("Photoshop CS 4\n");
		stringBuilder.Append("Unity 4\n");
		stringBuilder.Append("Spine\n");
		stringBuilder.Append("\n");
		stringBuilder.Append("[FFDC00]SPECIAL THANKS\n[FFFFFF]");
		stringBuilder.Append("To all the Bubble Blast\nrescuers\n\n");
		stringBuilder.Append("To all the Firefighters\naround the world\n\n");
		stringBuilder.Append("And special thanks to\n\n.:: YOU ::.\n");
		stringBuilder.Append("\n");
		component.text = stringBuilder.ToString();
	}
}
