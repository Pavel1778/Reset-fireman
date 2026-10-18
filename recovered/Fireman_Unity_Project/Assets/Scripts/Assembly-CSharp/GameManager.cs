using AndroidPlugin;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static Clock clock = null;

	public static GameManager instance = null;

	public static UIAtlas[] ATLAS = null;

	public static GameObject uiRoot = null;

	public static float uiScaleX = 1f;

	public static float uiScaleY = 1f;

	public static int tailleZoneX = 2;

	public static int tailleZoneY = 1;

	public static Camera mainCamera = null;

	public static ListOf<ISwitchable> switchables = null;

	public static ListOf<UpDown> platforms = null;

	public static ListOf<BlocSwitch> switches = null;

	public static ListOf<BaseEntity> ennemies = null;

	public static ListOf<BaseEntity> activeEnnemies = null;

	public static ListOf<Pompier> pompiers = null;

	private static Pompier pompier = null;

	private UIAtlas mAtlasDecor1;

	private UIAtlas mAtlasDecor2;

	private UIAtlas mAtlasDecor3;

	private UIAtlas mAtlasDecorSD;

	public static BaseSpawner<Goutte> gouttes;

	public static BaseSpawner<Coin> coins;

	public static BaseSpawner<Tank> tanks;

	public static BaseSpawner<EffetExp> fxexp;

	public static BaseSpawner<Projectile> projectiles;

	public static Decor[] decors;

	public static readonly byte[] DECORS_ATLAS = new byte[109]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 0, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 0, 0, 1, 1, 0, 0, 1, 1,
		0, 1, 0, 0, 255, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2
	};

	public int tailleBlocs = 50;

	public float ratioEnnemiSpeed = 0.8f;

	public float relPosBtnJumpX = 0.92f;

	public float relPosBtnJumpY = 0.12f;

	public float relPosBtnShootX = 0.76f;

	public float relPosBtnShootY = 0.12f;

	public static int RUNNING = 0;

	public static int PAUSED = 1;

	public static int FINISH = 2;

	public static int IDLE = 3;

	public static int END = 4;

	private int gameStatus = RUNNING;

	public UIAtlas AtlasFire;

	[Header("-- PANELS")]
	public UIPanel panelGame;

	public UIPanel panelPause;

	public UIPanel panelFinish;

	public UIPanel panelGameUI;

	public UIPanel panelCongratulation;

	public GameObject spriteTransition;

	public float ratioTransition;

	public bool sensTransition;

	private int nextScreen;

	[Header("-- PREFABS")]
	public GameObject prefabRelicAnim;

	public GameObject prefabPoufAnim;

	public GameObject prefabFlashAnim;

	public GameObject prefabMagicPoufAnim;

	public GameObject prefabTalky;

	public GameObject prefabGoutte;

	public GameObject prefabProjectile;

	public GameObject prefabCoin;

	public GameObject prefabTank;

	public GameObject prefabEffetExp;

	public GameObject prefabTruc;

	public GameObject prefabDecor;

	public GameObject prefabPompier;

	public GameObject prefabExit;

	public GameObject prefabPic;

	public GameObject prefabTeleporter;

	public GameObject prefabUpDown;

	public GameObject prefabVictim;

	public GameObject prefabBorne;

	public GameObject prefabEnnemi2;

	public GameObject prefabEnnemi3;

	public GameObject prefabEnnemi4;

	public GameObject prefabEnnemi5;

	public GameObject prefabEnnemi6;

	public GameObject prefabEventBloc;

	public GameObject prefabBackground;

	public GameObject prefabHiddenObject;

	private Texture bgDesert;

	private Texture bgJungle;

	private Texture bgMountain;

	public GameObject mBando;

	public PhysicMaterial defaultMaterial;

	public GameObject mTalky;

	public GameObject mTruc;

	public GameObject mDudeA;

	public GameObject mDudeB;

	public GameObject mDudeC;

	public GameObject mClock;

	public GameObject mPointeur;

	public GameObject mContent1;

	public GameObject mContent2;

	public GameObject mItemRang;

	public bool savedDudeA;

	public bool savedDudeB;

	public bool savedDudeC;

	public int life;

	[Header("-- SPRITES")]
	public UISprite uiDudesA;

	public UISprite uiDudesB;

	public UISprite uiDudesC;

	public UISprite uiLife1;

	public UISprite uiLife2;

	public UISprite uiLife3;

	public UISprite uiLife4;

	public UISprite uiLife5;

	public UISprite uiLife6;

	public UISprite uiPadBackground;

	public UISprite uiPad;

	public UISprite uiLeft;

	public UISprite uiRight;

	public UISprite uiShoot;

	public UISprite uiJump;

	public GameObject uiPause;

	[Header("-- DIALOGS")]
	public GameObject dialogAskShop;

	public GameObject dialogAnotherLife;

	public GameObject btnControl;

	public UISprite waterBarre;

	public UILabel labelPieceFinish;

	public float padInitX;

	public float padInitY;

	public float padX;

	public float padY;

	public int pieceOwned;

	public int nbrLife;

	private int rangAtStart;

	private int gameXp;

	public UISprite spriteVictimFinishA;

	public UISprite spriteVictimFinishAf;

	public UISprite spriteVictimFinishB;

	public UISprite spriteVictimFinishBf;

	public UISprite spriteVictimFinishC;

	public UISprite spriteVictimFinishCf;

	public UISprite spriteFadeFinish;

	public float litre;

	public int flamesExtinguish;

	public int flameboyExtinguish;

	public int slimeExtinguish;

	public int bomberExtinguish;

	public int throwerExtinguish;

	public int spiderExtinguish;

	public int gameOverCount;

	private static float camX;

	private static float camY;

	private static float xMin;

	private static float yMin;

	private static float xMax;

	private static float yMax;

	private static int nSiz;

	private static float xPos;

	private static float yPos;

	private static Vector2 nCam;

	private static Decor nDec;

	public Camera cameraUI;

	public Pompier Player1
	{
		get
		{
			return pompier;
		}
		set
		{
			pompier = value;
		}
	}

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int Status
	{
		get
		{
			return gameStatus;
		}
		set
		{
			if (value == PAUSED)
			{
				if (!VarsManager.Instance.hideAllPub())
				{
					PluginScript.Instance.showPub(1);
				}
				PluginScript.Instance.track("ingame/pause");
				panelPause.gameObject.SetActiveRecursively(true);
				dialogAskShop.gameObject.SetActiveRecursively(false);
				hideControls();
				GameObject gameObject = GameObject.Find("lblLevel");
				if (gameObject != null)
				{
					UILabel component = gameObject.GetComponent<UILabel>();
					if (component != null)
					{
						component.text = "Level " + VarsManager.Instance.worldToLoad + "." + VarsManager.Instance.levelToLoad;
					}
				}
			}
			else if (value == FINISH)
			{
				panelFinish.gameObject.SetActiveRecursively(true);
				panelGameUI.gameObject.SetActiveRecursively(false);
				hideControls();
			}
			else if (value == END)
			{
				panelGameUI.gameObject.SetActiveRecursively(false);
				panelFinish.gameObject.SetActiveRecursively(false);
				panelCongratulation.gameObject.SetActiveRecursively(true);
			}
			else
			{
				PluginScript.Instance.hidePub();
				panelGameUI.gameObject.SetActiveRecursively(true);
				panelPause.gameObject.SetActiveRecursively(false);
				panelFinish.gameObject.SetActiveRecursively(false);
				showControls();
			}
			gameStatus = value;
		}
	}

	public void Awake()
	{
		if (instance != null)
		{
			Object.DestroyImmediate(base.gameObject);
		}
		else
		{
			instance = this;
		}
	}

	public void OnApplicationPause()
	{
		if (gameStatus == RUNNING)
		{
			Status = PAUSED;
		}
	}

	public void LateUpdate()
	{
		if (decors == null)
		{
			return;
		}
		nCam = mainCamera.transform.position;
		xMin = (float)(-tailleZoneX) + nCam.x;
		yMin = (float)(-tailleZoneY) + nCam.y;
		xMax = (float)tailleZoneX + nCam.x;
		yMax = (float)tailleZoneY + nCam.y;
		nSiz = decors.Length;
		while (nSiz > 0)
		{
			nSiz--;
			nDec = decors[nSiz];
			xPos = nDec.relX;
			yPos = nDec.relY;
			if (xPos < xMin || yPos < yMin || xPos > xMax || yPos > yMax)
			{
				nDec.Clipped = false;
			}
			else
			{
				nDec.Clipped = true;
			}
		}
	}

	public static bool AddEnnemie(BaseEntity e)
	{
		if (ennemies.IndexOf(e) == -1)
		{
			ennemies.Add(e);
			return true;
		}
		return false;
	}

	public static UIAtlas getAtlas(int index)
	{
		if (index < 0 || index > DECORS_ATLAS.Length)
		{
			return ATLAS[0];
		}
		int num = DECORS_ATLAS[index];
		if (num == 255)
		{
			num = VarsManager.Instance.worldToLoad - 1;
		}
		return ATLAS[num];
	}

	public void Start()
	{
		UnityAdsHelper.Initialize();
		hideDialogAnotherLife();
		if (VarsManager.Instance.worldToLoad == 3)
		{
			Musics.music_ingame1.Release();
			Musics.music_ingame2.PlayFade(0.2f);
		}
		else
		{
			Musics.music_ingame2.Release();
			Musics.music_ingame1.PlayFade(0.2f);
		}
		if (Core.IsGoogleTV() || Core.IsLexibox())
		{
			int num = Mathf.FloorToInt(UIRoot.GetPixelSizeAdjustment(panelFinish.gameObject) * ((float)Screen.width * 0.5f));
			mContent1.transform.localPosition = new Vector3(num, 0f, 0f);
			mContent2.transform.localPosition = new Vector3(num, 0f, 0f);
		}
		mainCamera = Camera.main;
		if (Screen.width > 800)
		{
			if (mAtlasDecor1 == null)
			{
				mAtlasDecor1 = (UIAtlas)Resources.Load("Atlas/AtlasDecor", typeof(UIAtlas));
			}
			if (mAtlasDecor2 == null)
			{
				mAtlasDecor2 = (UIAtlas)Resources.Load("Atlas/AtlasDecor2", typeof(UIAtlas));
			}
			if (mAtlasDecor3 == null)
			{
				mAtlasDecor3 = (UIAtlas)Resources.Load("Atlas/AtlasDecor3", typeof(UIAtlas));
			}
			ATLAS = new UIAtlas[3] { mAtlasDecor1, mAtlasDecor2, mAtlasDecor3 };
		}
		else
		{
			if (mAtlasDecorSD == null)
			{
				mAtlasDecorSD = (UIAtlas)Resources.Load("Atlas/AtlasDecorSD", typeof(UIAtlas));
			}
			ATLAS = new UIAtlas[3] { mAtlasDecorSD, mAtlasDecorSD, mAtlasDecorSD };
		}
		sensTransition = false;
		ratioTransition = 1f;
		gameOverCount = 0;
		uiRoot = GameObject.Find("UI Root (2D)");
		uiScaleX = 1f / uiRoot.transform.localScale.x;
		uiScaleY = 1f / uiRoot.transform.localScale.y;
		if (AppConfig.Build == AppConfig.BuildEnum.Debug)
		{
			clock = new Clock(mClock);
		}
		else
		{
			GameObject gameObject = mClock;
			if (gameObject != null)
			{
				Object.DestroyImmediate(gameObject);
			}
		}
		pompiers = new ListOf<Pompier>();
		activeEnnemies = new ListOf<BaseEntity>();
		ennemies = new ListOf<BaseEntity>();
		switches = new ListOf<BlocSwitch>();
		platforms = new ListOf<UpDown>();
		switchables = new ListOf<ISwitchable>();
		gouttes = new BaseSpawner<Goutte>(100, prefabGoutte);
		coins = new BaseSpawner<Coin>(50, prefabCoin);
		tanks = new BaseSpawner<Tank>(50, prefabTank);
		fxexp = new BaseSpawner<EffetExp>(50, prefabEffetExp);
		projectiles = new BaseSpawner<Projectile>(50, prefabProjectile);
		Victim.cptDudes = 0;
		bool flag = false;
		if (!((!(VarsManager.Instance != null) || VarsManager.Instance.levelToLoad == 0) ? LevelLoader.Initialize(1, 1) : LevelLoader.Initialize(VarsManager.Instance.worldToLoad, VarsManager.Instance.levelToLoad)))
		{
		}
		setBackground();
		Status = RUNNING;
		if (VarsManager.Instance != null)
		{
			nbrLife = VarsManager.Instance.getObjetShopStep("upgrade_life") + 3;
		}
		life = nbrLife;
		updateLifeUI();
		savedDudeA = false;
		savedDudeB = false;
		savedDudeC = false;
		updateDudesUI();
		Vector3 position = Player1.transform.position;
		mainCamera.transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
		gameXp = VarsManager.Instance.getXp();
		rangAtStart = VarsManager.Instance.rang;
		gameXp = 0;
		if (VarsManager.Instance.getPetSkin() != 0)
		{
			GameObject gameObject2 = Object.Instantiate(Instance.prefabTruc);
			gameObject2.GetComponent<Truc>().setTruc(VarsManager.Instance.getPetSkin(), true);
		}
		litre = VarsManager.Instance.listAchievs[14].current_step;
		flamesExtinguish = VarsManager.Instance.listAchievs[25].current_step;
		flameboyExtinguish = VarsManager.Instance.listAchievs[26].current_step;
		slimeExtinguish = VarsManager.Instance.listAchievs[27].current_step;
		bomberExtinguish = VarsManager.Instance.listAchievs[28].current_step;
		throwerExtinguish = VarsManager.Instance.listAchievs[29].current_step;
		spiderExtinguish = VarsManager.Instance.listAchievs[30].current_step;
		mPointeur.GetComponent<PointeurDude>().cible = mDudeA;
		VarsManager.Instance.loadMenu = false;
		VarsManager.Instance.loadGame = false;
		VarsManager.Instance.returnToGame = false;
		PluginScript.Instance.track("ingame/level/" + VarsManager.GameMode + "/" + int2String0000(VarsManager.Instance.worldToLoad) + "/" + int2String0000(VarsManager.Instance.levelToLoad));
		showControls();
	}

	private void showControls()
	{
		if (AppConfig.Platform != AppConfig.PlatformEnum.AndroidOuya && !Core.IsGoogleTV() && !Core.IsLexibox())
		{
			uiPadBackground.gameObject.SetActive(!VarsManager.Instance.useArrows);
			uiPad.gameObject.SetActive(!VarsManager.Instance.useArrows);
			uiLeft.gameObject.SetActive(VarsManager.Instance.useArrows);
			uiRight.gameObject.SetActive(VarsManager.Instance.useArrows);
			uiShoot.gameObject.SetActive(true);
			uiJump.gameObject.SetActive(true);
			uiPause.SetActive(true);
		}
		else
		{
			hideControls();
		}
	}

	private void hideControls()
	{
		btnControl.gameObject.SetActive(false);
		uiPadBackground.gameObject.SetActive(false);
		uiPad.gameObject.SetActive(false);
		uiLeft.gameObject.SetActive(false);
		uiRight.gameObject.SetActive(false);
		uiShoot.gameObject.SetActive(false);
		uiJump.gameObject.SetActive(false);
		uiPause.SetActive(false);
	}

	public static void setBackground()
	{
		GameObject gameObject = Object.Instantiate(Instance.prefabBackground);
		gameObject.transform.parent = instance.panelGame.transform;
		gameObject.GetComponent<UIStretch>().uiCamera = Camera.main;
		gameObject.GetComponent<UIStretch>().relativeSize.x = Camera.main.orthographicSize;
		gameObject.GetComponent<UIStretch>().relativeSize.y = Camera.main.orthographicSize;
		gameObject.GetComponent<UITexture>().mainTexture = loadBackground(VarsManager.Instance.worldToLoad);
		LevelLoader.add_Talky();
	}

	private static Texture loadBackground(int index)
	{
		Texture result = null;
		if (index == 1)
		{
			instance.bgJungle = loadIfNull("Bg_Jungle", instance.bgJungle);
			result = instance.bgJungle;
		}
		else
		{
			instance.bgJungle = null;
		}
		if (index == 2)
		{
			instance.bgDesert = loadIfNull("Bg_Desert", instance.bgDesert);
			result = instance.bgDesert;
		}
		else
		{
			instance.bgDesert = null;
		}
		if (index == 3)
		{
			instance.bgMountain = loadIfNull("Bg_Mountain", instance.bgMountain);
			result = instance.bgMountain;
		}
		else
		{
			instance.bgMountain = null;
		}
		return result;
	}

	private static Texture loadIfNull(string name, Texture tex)
	{
		return (!(tex != null)) ? ((Texture)Resources.Load("Textures/" + name)) : tex;
	}

	public static Goutte newGoutte(float pX, float pY, float pVelX, float pVelY, float pLife)
	{
		Goutte goutte = gouttes.Allocate();
		goutte.setGoutte(new Vector3(pX, pY, 0f), new Vector2(pVelX, pVelY), pLife);
		return goutte;
	}

	public static Projectile newProjectile(float pX, float pY, float pAngle, float pForce)
	{
		Projectile projectile = projectiles.Allocate();
		projectile.setProjectile(new Vector3(pX, pY, 0f), pAngle, pForce);
		return projectile;
	}

	public static Coin newCoin(float pX, float pY, float pVelX, float pVelY, int pValue)
	{
		Coin coin = coins.Allocate();
		coin.setCoin(new Vector3(pX, pY, 0f), new Vector2(pVelX, pVelY), pValue);
		return coin;
	}

	public static Tank newTank(float pX, float pY, float pVelX, float pVelY, int pValue)
	{
		Tank tank = tanks.Allocate();
		tank.setTank(new Vector3(pX, pY, 0f), pValue);
		return tank;
	}

	public static EffetExp newEffetExp(float pX, float pY, string pText)
	{
		EffetExp effetExp = fxexp.Allocate();
		effetExp.setEffetExp(new Vector3(pX, pY, 0f), pText);
		return effetExp;
	}

	public static void GiveBonus(Vector3 pPos, Config.BonusInfo pBonus, bool pUsePet)
	{
		int coin_rnd = pBonus.coin_rnd;
		int num = 0;
		if (coin_rnd > 0)
		{
			if (coin_rnd == 100)
			{
				num = Common.Rndi(pBonus.coin_min, pBonus.coin_max);
			}
			else
			{
				int num2 = Common.Rndi(100);
				if (coin_rnd >= num2)
				{
					num = Common.Rndi(pBonus.coin_min, pBonus.coin_max);
				}
			}
			if (num > 0)
			{
				addCoins(pPos, num);
			}
		}
		num = pBonus.exp;
		if (num > 0)
		{
			addExps(instance.Player1.transform.position, (!pUsePet) ? num : (num / 2));
		}
	}

	private static void addExps(Vector3 pos, int num)
	{
		if (num > 0)
		{
			Vector3 position = instance.Player1.transform.position;
			if (VarsManager.Instance.getObjetShopStep("upgrade_xpx2") == 1 || VarsManager.Instance.getObjetShopStep("upgrade_piecesxp") == 1)
			{
				num *= 2;
			}
			newEffetExp(position.x, position.y, "+ " + num + " XP");
			instance.gameXp += num;
		}
	}

	private static void addCoins(Vector3 pos, int num)
	{
		if (num > 0)
		{
			int num2 = 1;
			if (VarsManager.Instance.getObjetShopStep("upgrade_piecex2") == 1 || VarsManager.Instance.getObjetShopStep("upgrade_piecesxp") == 1)
			{
				num2 *= 2;
			}
			for (int i = 0; i < num; i++)
			{
				newCoin(pos.x, pos.y, -0.5f + Random.value * 1f, 2.5f + Random.value * 1f, num2);
			}
		}
	}

	public void Update()
	{
		if (AppConfig.Build == AppConfig.BuildEnum.Debug)
		{
			clock.Paused = Status == PAUSED;
			clock.Update();
		}
		if (gameStatus == RUNNING)
		{
			gouttes.Update();
			coins.Update();
			tanks.Update();
			fxexp.Update();
			projectiles.Update();
			activeEnnemies.Clear();
			for (int i = 0; i < ennemies.Count; i++)
			{
				BaseEntity baseEntity = ennemies[i];
				if (baseEntity != null && !baseEntity.Culled)
				{
					activeEnnemies.Add(ennemies[i]);
				}
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Status = PAUSED;
				return;
			}
		}
		else if (gameStatus == FINISH)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				btnHome();
				return;
			}
		}
		else if (gameStatus == PAUSED && Input.GetKeyDown(KeyCode.Escape))
		{
			Status = RUNNING;
			return;
		}
		if (AppConfig.Platform != AppConfig.PlatformEnum.AndroidOuya)
		{
			if (VarsManager.Instance.useArrows)
			{
				uiLeft.enabled = true;
				uiRight.enabled = true;
				uiPad.enabled = false;
				uiPadBackground.enabled = false;
			}
			else
			{
				uiLeft.enabled = false;
				uiRight.enabled = false;
				if (padInitX == -1f && padInitY == -1f)
				{
					uiPad.enabled = false;
					uiPadBackground.enabled = false;
				}
				else
				{
					uiPad.enabled = true;
					uiPadBackground.enabled = true;
				}
				uiPadBackground.GetComponent<UIAnchor>().relativeOffset.x = padInitX;
				uiPadBackground.GetComponent<UIAnchor>().relativeOffset.y = padInitY;
				uiPad.GetComponent<UIAnchor>().relativeOffset.x = padX;
				uiPad.GetComponent<UIAnchor>().relativeOffset.y = padY;
			}
		}
		if (sensTransition)
		{
			ratioTransition += 3f * Time.deltaTime;
			if (ratioTransition > 1f)
			{
				ratioTransition = 1f;
				if (nextScreen == 0)
				{
					Start();
				}
				else if (nextScreen == 1)
				{
					VarsManager.Instance.loadMenu = true;
					Application.LoadLevel("SceneLoading");
				}
				else if (nextScreen == 2)
				{
					Status = END;
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
			spriteTransition.SetActiveRecursively(true);
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
			spriteTransition.SetActiveRecursively(false);
		}
	}

	public void updateDudesUI()
	{
		if (VarsManager.Instance.levelToLoad <= 3 && VarsManager.Instance.worldToLoad == 1)
		{
			if (savedDudeA)
			{
				uiDudesB.spriteName = "05_Pnj_Filled";
				uiDudesB.alpha = 1f;
			}
			else
			{
				uiDudesB.spriteName = "05_Pnj_Empty";
				uiDudesB.alpha = 0.5f;
			}
			uiDudesA.alpha = 0f;
			uiDudesC.alpha = 0f;
			return;
		}
		if (savedDudeA)
		{
			uiDudesA.spriteName = "05_Pnj_Filled";
			uiDudesA.alpha = 1f;
		}
		else
		{
			uiDudesA.spriteName = "05_Pnj_Empty";
			uiDudesA.alpha = 0.5f;
		}
		if (savedDudeB)
		{
			uiDudesB.spriteName = "06_Pnj_Filled";
			uiDudesB.alpha = 1f;
		}
		else
		{
			uiDudesB.spriteName = "06_Pnj_Empty";
			uiDudesB.alpha = 0.5f;
		}
		if (savedDudeC)
		{
			uiDudesC.spriteName = "07_Pnj_Filled";
			uiDudesC.alpha = 1f;
		}
		else
		{
			uiDudesC.spriteName = "07_Pnj_Empty";
			uiDudesC.alpha = 0.5f;
		}
	}

	public void updateLifeUI()
	{
		if (life >= 1)
		{
			uiLife1.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 1)
		{
			uiLife1.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife1.alpha = 0f;
		}
		if (life >= 2)
		{
			uiLife2.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 2)
		{
			uiLife2.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife2.alpha = 0f;
		}
		if (life >= 3)
		{
			uiLife3.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 3)
		{
			uiLife3.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife3.alpha = 0f;
		}
		if (life >= 4)
		{
			uiLife4.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 4)
		{
			uiLife4.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife4.alpha = 0f;
		}
		if (life >= 5)
		{
			uiLife5.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 5)
		{
			uiLife5.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife5.alpha = 0f;
		}
		if (life >= 6)
		{
			uiLife6.spriteName = "02_Heart_Filled";
		}
		else if (nbrLife >= 6)
		{
			uiLife6.spriteName = "02_Heart_Empty";
		}
		else
		{
			uiLife6.alpha = 0f;
		}
	}

	public void finishLevel()
	{
		Player1.finish();
		Player1.stopWalk();
		int num = 0;
		if (savedDudeA)
		{
			num++;
		}
		if (savedDudeB)
		{
			num++;
		}
		if (savedDudeC)
		{
			num++;
		}
		VarsManager.Instance.setStars(VarsManager.GameMode, VarsManager.Instance.worldToLoad, VarsManager.Instance.levelToLoad, savedDudeA, savedDudeB, savedDudeC, false);
		VarsManager.Instance.addCoins(pieceOwned);
		int progression = VarsManager.Instance.getProgression();
		int num2 = progression / 1000;
		int num3 = progression - num2 * 1000;
		if (num2 == VarsManager.Instance.worldToLoad && VarsManager.Instance.levelToLoad >= num3)
		{
			num3++;
			if (num3 > 18)
			{
				num3 = 1;
				num2++;
				if (num2 > 3)
				{
					num2 = 3;
					num3 = 18;
				}
			}
			VarsManager.Instance.setProgression(VarsManager.GameMode, num2, num3);
		}
		labelPieceFinish.text = string.Empty + VarsManager.Instance.getCoins();
		int num4 = VarsManager.Instance.computeRang(gameXp + VarsManager.Instance.getXp());
		if (rangAtStart < num4)
		{
			mBando.GetComponent<Bando>().showBando("Rank" + num4, TextManager.GetText("res_new_rank") + " : " + VarsManager.Instance.tabRang[num4].titre, true);
			Sounds.snd_ig_notif.Play();
			VarsManager.Instance.setAchievement(num4 + 3, 1, true);
			rangAtStart = num4;
		}
		VarsManager.Instance.addXp(gameXp);
		Status = FINISH;
		spriteFadeFinish.GetComponent<TweenAlpha>().Reset();
		spriteFadeFinish.GetComponent<TweenAlpha>().delay = 0f;
		spriteFadeFinish.GetComponent<TweenAlpha>().duration = 0.1f;
		spriteFadeFinish.GetComponent<TweenAlpha>().enabled = true;
		if (VarsManager.Instance.levelToLoad <= 3 && VarsManager.Instance.worldToLoad == 1)
		{
			spriteVictimFinishB.GetComponent<TweenScale>().Reset();
			spriteVictimFinishB.GetComponent<TweenScale>().delay = 1f;
			spriteVictimFinishB.GetComponent<TweenScale>().duration = 0.3f;
			spriteVictimFinishB.GetComponent<TweenScale>().enabled = true;
			spriteVictimFinishB.spriteName = "05_Pnj_Filled";
			spriteVictimFinishBf.spriteName = "05_Pnj_Empty";
			if (savedDudeA)
			{
				spriteVictimFinishB.gameObject.SetActiveRecursively(true);
			}
			else
			{
				spriteVictimFinishB.gameObject.SetActiveRecursively(false);
			}
			spriteVictimFinishA.gameObject.SetActiveRecursively(false);
			spriteVictimFinishAf.gameObject.SetActiveRecursively(false);
			spriteVictimFinishC.gameObject.SetActiveRecursively(false);
			spriteVictimFinishCf.gameObject.SetActiveRecursively(false);
		}
		else
		{
			spriteVictimFinishA.GetComponent<TweenScale>().Reset();
			spriteVictimFinishA.GetComponent<TweenScale>().delay = 0.5f;
			spriteVictimFinishA.GetComponent<TweenScale>().duration = 0.3f;
			spriteVictimFinishA.GetComponent<TweenScale>().enabled = true;
			spriteVictimFinishB.GetComponent<TweenScale>().Reset();
			spriteVictimFinishB.GetComponent<TweenScale>().delay = 1f;
			spriteVictimFinishB.GetComponent<TweenScale>().duration = 0.3f;
			spriteVictimFinishB.GetComponent<TweenScale>().enabled = true;
			spriteVictimFinishC.GetComponent<TweenScale>().Reset();
			spriteVictimFinishC.GetComponent<TweenScale>().delay = 1.5f;
			spriteVictimFinishC.GetComponent<TweenScale>().duration = 0.3f;
			spriteVictimFinishC.GetComponent<TweenScale>().enabled = true;
			spriteVictimFinishA.spriteName = "05_Pnj_Filled";
			spriteVictimFinishAf.spriteName = "05_Pnj_Empty";
			spriteVictimFinishB.spriteName = "06_Pnj_Filled";
			spriteVictimFinishBf.spriteName = "06_Pnj_Empty";
			spriteVictimFinishC.spriteName = "07_Pnj_Filled";
			spriteVictimFinishCf.spriteName = "07_Pnj_Empty";
			if (savedDudeA)
			{
				spriteVictimFinishA.gameObject.SetActiveRecursively(true);
			}
			else
			{
				spriteVictimFinishA.gameObject.SetActiveRecursively(false);
			}
			if (savedDudeB)
			{
				spriteVictimFinishB.gameObject.SetActiveRecursively(true);
			}
			else
			{
				spriteVictimFinishB.gameObject.SetActiveRecursively(false);
			}
			if (savedDudeC)
			{
				spriteVictimFinishC.gameObject.SetActiveRecursively(true);
			}
			else
			{
				spriteVictimFinishC.gameObject.SetActiveRecursively(false);
			}
		}
		if (VarsManager.Instance.levelToLoad <= 8)
		{
			VarsManager.Instance.setAchievement(3, VarsManager.Instance.levelToLoad, false);
		}
		VarsManager.Instance.setAchievement(14, (int)litre, false);
		VarsManager.Instance.setAchievement(15, (int)litre, false);
		VarsManager.Instance.setAchievement(16, (int)litre, false);
		if (num == 3)
		{
			VarsManager.Instance.setAchievement(17, 1, false);
		}
		if (num == 0)
		{
			VarsManager.Instance.setAchievement(18, 1, false);
		}
		if (life == nbrLife && VarsManager.Instance.levelToLoad > 3)
		{
			VarsManager.Instance.setAchievement(19, 1, false);
		}
		if (VarsManager.Instance.levelToLoad == 18 && VarsManager.Instance.worldToLoad == 1)
		{
			VarsManager.Instance.setAchievement(20, 1, false);
		}
		if (VarsManager.Instance.levelToLoad == 18 && VarsManager.Instance.worldToLoad == 2)
		{
			VarsManager.Instance.setAchievement(21, 1, false);
		}
		VarsManager.Instance.setAchievement(25, flamesExtinguish, false);
		VarsManager.Instance.setAchievement(26, flameboyExtinguish, false);
		VarsManager.Instance.setAchievement(27, slimeExtinguish, false);
		VarsManager.Instance.setAchievement(28, bomberExtinguish, false);
		VarsManager.Instance.setAchievement(29, throwerExtinguish, false);
		VarsManager.Instance.setAchievement(30, spiderExtinguish, false);
		PluginScript.Instance.hidePub();
		if (!VarsManager.Instance.hideAllPub())
		{
			PluginScript.Instance.showPub(1);
		}
		Sounds.snd_ig_goodjob.Play();
		AudioPlayerEx.stopMusic(0.2f);
	}

	public void restart()
	{
		if (dialogAnotherLife.activeSelf || sensTransition)
		{
			return;
		}
		life--;
		updateLifeUI();
		if (life <= 0)
		{
			PluginScript.Instance.track("ingame/gameover/" + int2String0000(VarsManager.Instance.worldToLoad) + "/" + int2String0000(VarsManager.Instance.levelToLoad));
			VarsManager.Instance.GameOverCount++;
			if (VarsManager.Instance.GameOverCount == 3)
			{
				PluginScript.Instance.track("ingame/shopgameover");
				Invoke("showDialogShop", 1f);
				VarsManager.Instance.GameOverCount = 0;
				nextScreen = 0;
				sensTransition = true;
			}
			else if (!showDialogAnotherLife())
			{
				sensTransition = true;
				nextScreen = 0;
			}
		}
		else
		{
			Player1.returnToBorne();
		}
	}

	public void btnNext()
	{
		if (gameStatus == IDLE)
		{
			return;
		}
		gameStatus = IDLE;
		PluginScript.Instance.hidePub();
		Sounds.snd_ui_play.Play();
		VarsManager.Instance.GameOverCount = 0;
		if (VarsManager.Instance.levelToLoad > 18)
		{
			VarsManager.Instance.goToLevelSelector = true;
			VarsManager.Instance.loadMenu = true;
			Application.LoadLevel("SceneLoading");
			return;
		}
		VarsManager.Instance.levelToLoad++;
		if (VarsManager.Instance.levelToLoad > 18)
		{
			if (VarsManager.Instance.worldToLoad == 3)
			{
				nextScreen = 2;
				sensTransition = true;
				VarsManager.Instance.tryItHarder = true;
				return;
			}
			VarsManager.Instance.levelToLoad = 1;
			VarsManager.Instance.worldToLoad++;
		}
		if (Ask4RateScreen.next())
		{
			sensTransition = false;
			nextScreen = 0;
		}
		else
		{
			nextScreen = 0;
			sensTransition = true;
		}
	}

	public void btnPause()
	{
		if (gameStatus != IDLE)
		{
			gameStatus = IDLE;
			Sounds.snd_ui_select.Play();
			PluginScript.Instance.hidePub();
			Status = PAUSED;
		}
	}

	public void btnResume()
	{
		if (gameStatus != IDLE)
		{
			gameStatus = IDLE;
			Sounds.snd_ui_play.Play();
			PluginScript.Instance.hidePub();
			Status = RUNNING;
		}
	}

	public void btnHome()
	{
		if (gameStatus != IDLE)
		{
			gameStatus = IDLE;
			PluginScript.Instance.track("ingame/home");
			PluginScript.Instance.hidePub();
			Sounds.snd_ui_select.Play();
			nextScreen = 1;
			sensTransition = true;
		}
	}

	public void btnRetry()
	{
		if (gameStatus != IDLE)
		{
			gameStatus = IDLE;
			PluginScript.Instance.track("ingame/level/" + VarsManager.GameMode + "/" + int2String0000(VarsManager.Instance.worldToLoad) + "/" + int2String0000(VarsManager.Instance.levelToLoad));
			PluginScript.Instance.hidePub();
			Sounds.snd_ui_select.Play();
			nextScreen = 0;
			sensTransition = true;
			VarsManager.Instance.GameOverCount = 0;
		}
	}

	public void btnShop()
	{
		if (gameStatus != IDLE)
		{
			gameStatus = IDLE;
			PluginScript.Instance.hidePub();
			PluginScript.Instance.track("ingame/shop");
			Sounds.snd_ui_select.Play();
			if (panelFinish.gameObject.active && VarsManager.Instance.levelToLoad < 18)
			{
				VarsManager.Instance.levelToLoad++;
			}
			VarsManager.Instance.loadMenu = true;
			VarsManager.Instance.goToShop = true;
			VarsManager.Instance.returnToGame = true;
			Application.LoadLevel("SceneLoading");
		}
	}

	public void btnRate()
	{
		PluginScript.Instance.track("ingame/rate");
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.magmamobile.game.Fireman");
	}

	public void changeControls()
	{
		Sounds.snd_ui_select.Play();
		if (VarsManager.Instance.useArrows)
		{
			PluginScript.Instance.track("ingame/option/joystick");
			VarsManager.Instance.setUseArrows(false);
			btnControl.GetComponentInChildren<UILabel>().text = "< " + TextManager.GetText("res_joystick") + ">";
		}
		else
		{
			PluginScript.Instance.track("ingame/option/arrows");
			VarsManager.Instance.setUseArrows(true);
			btnControl.GetComponentInChildren<UILabel>().text = "< " + TextManager.GetText("res_arrows") + ">";
		}
	}

	public void sonPetGoodJobA()
	{
		Sounds.snd_ig_saved1.Play();
	}

	public void sonPetGoodJobB()
	{
		Sounds.snd_ig_saved2.Play();
	}

	public void sonPetGoodJobC()
	{
		Sounds.snd_ig_saved3.Play();
	}

	public void btnNoDialogShop()
	{
		dialogAskShop.gameObject.SetActiveRecursively(false);
	}

	public void btnYesDialogShop()
	{
		VarsManager.Instance.loadMenu = true;
		VarsManager.Instance.goToShop = true;
		VarsManager.Instance.returnToGame = true;
		Application.LoadLevel("SceneLoading");
	}

	public void showDialogShop()
	{
		dialogAskShop.gameObject.SetActiveRecursively(true);
	}

	public static string int2String0000(int num)
	{
		string text = "0000";
		string text2 = num + string.Empty;
		return text.Substring(0, text.Length - text2.Length) + text2;
	}

	private bool showDialogAnotherLife()
	{
		if (UnityAdsHelper.IsReady)
		{
			PluginScript.Instance.track("ingame/another_life_video");
			dialogAnotherLife.gameObject.SetActiveRecursively(true);
			return true;
		}
		return false;
	}

	private void hideDialogAnotherLife()
	{
		dialogAnotherLife.gameObject.SetActiveRecursively(false);
	}

	public void btnDialogAnotherLife_Yes()
	{
		if (gameStatus != PAUSED)
		{
			PluginScript.Instance.track("ingame/another_life_video/yes");
			UnityAdsHelper.OnAdFailed = OnVideoAdsFail;
			UnityAdsHelper.OnAdSkipped = OnVideoAdsFail;
			UnityAdsHelper.OnAdFinished = OnVideoAdsOK;
			UnityAdsHelper.Show();
		}
	}

	public void btnDialogAnotherLife_No()
	{
		if (gameStatus != PAUSED)
		{
			hideDialogAnotherLife();
			PluginScript.Instance.track("ingame/another_life_video/no");
			nextScreen = 0;
			sensTransition = true;
		}
	}

	private void OnVideoAdsOK()
	{
		PluginScript.Instance.track("ingame/another_life_video/ok");
		VarsManager.Instance.GameOverCount = 0;
		life = 1;
		updateLifeUI();
		Player1.returnToBorne();
		hideDialogAnotherLife();
	}

	private void OnVideoAdsFail()
	{
		PluginScript.Instance.track("ingame/another_life_video/fail");
		nextScreen = 0;
		sensTransition = true;
		hideDialogAnotherLife();
	}
}
