using UnityEngine;

public class LevelLoader
{
	private const int TYPE_TILE = 0;

	private const int TYPE_UPDOWN = 1;

	private const int TYPE_HYDRANT = 2;

	private const int TYPE_COIN = 3;

	private const int TYPE_BOMBER = 4;

	private const int TYPE_CRAWLER = 5;

	private const int TYPE_FIREMAN = 6;

	private const int TYPE_FLAMMER = 7;

	private const int TYPE_RUNNER = 8;

	private const int TYPE_SPIDER = 9;

	private const int TYPE_THROWER = 10;

	private const int TYPE_VICTIM = 11;

	private const int TYPE_EXIT = 12;

	private const int TYPE_HURT = 13;

	private const int TYPE_TELEPORT = 14;

	private const int TYPE_TRIGGER = 15;

	private const int TYPE_PET = 16;

	private const int TYPE_HIDDENOBJECT = 17;

	private const int TYPE_BUBBLEBLAST = 18;

	private const int TYPE_BOSS1 = 19;

	private const int TYPE_SWITCH = 20;

	private const int TYPE_TANK = 21;

	private const int TYPE_CUSTOM = 99;

	private const int TYPE_BOX = -1;

	private const int TYPE_BG = -2;

	private static readonly int[] FLIP_H = new int[5] { 0, 180, 0, 180, 0 };

	private static readonly int[] FLIP_V = new int[5] { 0, 0, 180, 180, 0 };

	private static ListOf<Decor> decors;

	private static GameObject root;

	private static int count;

	public static bool Initialize(int idW, int idL)
	{
		bool flag = false;
		root = GameObject.Find("PanelGame").gameObject;
		clearLevel(root);
		if (idW == -1 || idL == -1)
		{
			return loadLevel("testLevel");
		}
		return loadLevel("level_" + idW + "_" + string.Format("{0:00}", idL));
	}

	private static void clearLevel(GameObject pRoot)
	{
		int num = pRoot.transform.childCount;
		int num2 = 0;
		count = 0;
		GameManager.decors = null;
		GameManager.switchables.Clear();
		GameManager.switches.Clear();
		GameManager.platforms.Clear();
		GameManager.ennemies.Clear();
		GameManager.pompiers.Clear();
		while (pRoot.transform.childCount > 0)
		{
			GameObject gameObject = pRoot.transform.GetChild(num2).gameObject;
			if (!"BG".Equals(gameObject.name) && !"talky".Equals(gameObject.name))
			{
				Object.DestroyImmediate(gameObject);
			}
			else
			{
				num2++;
			}
			num--;
			if (num <= 0)
			{
				break;
			}
		}
	}

	private static GameObject gp(int tag)
	{
		switch (tag)
		{
		case 0:
			return Object.Instantiate(GameManager.Instance.prefabDecor);
		case 1:
			return Object.Instantiate(GameManager.Instance.prefabUpDown);
		case 2:
			return Object.Instantiate(GameManager.Instance.prefabBorne);
		case 3:
			return GameManager.newCoin(0f, 0f, 0f, 0f, 1).gameObject;
		case 21:
			return GameManager.newTank(0f, 0f, 0f, 0f, 1).gameObject;
		case 4:
			return Object.Instantiate(GameManager.Instance.prefabEnnemi4);
		case 5:
			return Object.Instantiate(GameManager.Instance.prefabEnnemi3);
		case 6:
			return Object.Instantiate(GameManager.Instance.prefabPompier);
		case 7:
			return Fire.create().gameObject;
		case 8:
			return Object.Instantiate(GameManager.Instance.prefabEnnemi2);
		case 9:
			return Object.Instantiate(GameManager.Instance.prefabEnnemi6);
		case 10:
			return Object.Instantiate(GameManager.Instance.prefabEnnemi5);
		case 11:
			return Object.Instantiate(GameManager.Instance.prefabVictim);
		case 12:
			return Object.Instantiate(GameManager.Instance.prefabExit);
		case 13:
			return Object.Instantiate(GameManager.Instance.prefabPic);
		case 14:
			return Object.Instantiate(GameManager.Instance.prefabTeleporter);
		case 15:
			return Object.Instantiate(GameManager.Instance.prefabEventBloc);
		case 16:
			return Object.Instantiate(GameManager.Instance.prefabTruc);
		case -2:
			return Object.Instantiate(GameManager.Instance.prefabBackground);
		case 99:
			return Object.Instantiate(GameManager.Instance.prefabDecor);
		case 17:
			return Object.Instantiate(GameManager.Instance.prefabHiddenObject);
		case -1:
			return new GameObject("Wall");
		default:
			return null;
		}
	}

	public static int scaleX(int v)
	{
		return v * 50 - 500;
	}

	public static int scaleY(int v)
	{
		return 300 - v * 50;
	}

	private static GameObject nm(GameObject obj, int col, int row, int z)
	{
		if (obj == null)
		{
			return null;
		}
		obj.transform.parent = root.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = new Vector3(scaleX(col), scaleY(row), z);
		return obj;
	}

	private static void rTile(ByteArrayReader j)
	{
		count++;
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.ReadByte();
		int pAngle = j.Read_Angle(f);
		int num2 = j.Read_Flip(f);
		int num3 = j.Read_OfsX(f);
		int num4 = j.Read_OfsY(f);
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(0), col, row, 0);
			Decor component = gameObject.GetComponent<Decor>();
			component.SetData(num - 1, pAngle, FLIP_H[num2], FLIP_V[num2], num3, num4, 0f - (float)count / 100f);
			decors.Add(component);
		}
	}

	private static void rHurt(ByteArrayReader j)
	{
		count++;
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.ReadByte();
		int pAngle = j.Read_Angle(f);
		int num2 = j.Read_Flip(f);
		int num3 = j.Read_OfsX(f);
		int num4 = j.Read_OfsY(f);
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(13), col, row, 0);
			Pic component = gameObject.GetComponent<Pic>();
			component.SetData(num - 1, pAngle, FLIP_H[num2], FLIP_V[num2], num3, num4, 0f - (float)count / 100f);
			component.setPic();
			decors.Add(component);
		}
	}

	private static void rUpDown(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.ReadInt16();
		int num2 = j.ReadInt16();
		int num3 = j.ReadByte();
		int pAngle = j.Read_Angle(f);
		int num4 = j.Read_Flip(f);
		int num5 = j.Read_OfsX(f);
		int num6 = j.Read_OfsY(f);
		int pTagID = j.Read_Tag(f);
		int f2 = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(1), col, row, 0);
			UpDown component = gameObject.GetComponent<UpDown>();
			component.SetData(num3 - 1, pAngle, FLIP_H[num4], FLIP_V[num4], num5, num6, 0f);
			component.Setup(num, num2, check_Enabled(f2), check_IsLoop(f2), pTagID);
			GameManager.switchables.Add(component);
			GameManager.platforms.Add(component);
		}
	}

	private static void rHiddenObject(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int objetCache = j.ReadInt16();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(17), col, row, 0);
			ObjetCache component = gameObject.GetComponent<ObjetCache>();
			component.setObjetCache(objetCache);
		}
	}

	private static void rHydrant(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(2), col, row, 0);
		}
	}

	private static void rCoin(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(3), col, row, 0);
			Coin component = gameObject.GetComponent<Coin>();
			component.isStatic = true;
		}
	}

	private static void rTank(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(21), col, row, 0);
			Tank component = gameObject.GetComponent<Tank>();
			component.isStatic = true;
		}
	}

	private static void rFlammer(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(7), col, row, -1);
			Ennemi1 component = gameObject.GetComponent<Ennemi1>();
			GameManager.AddEnnemie(component);
		}
	}

	private static void rRunner(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(8), col, row, 0);
			Ennemi2 component = gameObject.GetComponent<Ennemi2>();
			component.setEnnemi(gameObject.transform.localPosition);
			GameManager.AddEnnemie(component);
		}
	}

	private static void rCrawler(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.ReadInt16();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(5), col, row, -1);
			Ennemi3 component = gameObject.GetComponent<Ennemi3>();
			component.setEnnemi(Vector3.zero, num);
			GameManager.AddEnnemie(component);
		}
	}

	private static void rBomber(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(4), col, row, -1);
			Ennemi4 component = gameObject.GetComponent<Ennemi4>();
			component.setEnnemi(gameObject.transform.localPosition);
			GameManager.AddEnnemie(component);
		}
	}

	private static void rThrower(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(10), col, row, -1);
			Ennemi5 component = gameObject.GetComponent<Ennemi5>();
			component.setEnnemi(gameObject.transform.localPosition);
			GameManager.AddEnnemie(component);
		}
	}

	private static void rSpider(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(9), col, row, -1);
			Ennemi6 component = gameObject.GetComponent<Ennemi6>();
			component.setEnnemi(gameObject.transform.localPosition);
			GameManager.AddEnnemie(component);
		}
	}

	private static void rFireman(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(6), col, row, -1);
			Pompier component = gameObject.GetComponent<Pompier>();
			component.playerIndex = GameManager.pompiers.Count;
			component.setPompier(gameObject.transform.localPosition);
			GameManager.pompiers.Add(component);
		}
	}

	private static void rVictim(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(11), col, row, -1);
			Victim component = gameObject.GetComponent<Victim>();
			component.setVictim();
		}
	}

	private static void rExit(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(12), col, row, 0);
			Exit component = gameObject.GetComponent<Exit>();
			component.setSprite();
		}
	}

	private static void rTeleport(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int x = j.ReadInt16();
		int y = j.ReadInt16();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(14), col, row, 0);
			Teleporter component = gameObject.GetComponent<Teleporter>();
			component.setTeleporter(x, y);
			component.setSprite();
		}
	}

	private static void rTrigger(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		int eventID = j.ReadInt16();
		int num = j.ReadInt16();
		if (check_Mode(f))
		{
			GameObject gameObject = nm(gp(15), col, row, 0);
			EventBloc component = gameObject.GetComponent<EventBloc>();
			component.eventID = eventID;
		}
	}

	private static void rPet(ByteArrayReader j)
	{
		int col = j.ReadInt16();
		int row = j.ReadInt16();
		int f = j.ReadByte();
		if (check_Mode(f) && VarsManager.Instance.getPetSkin() == 0)
		{
			GameObject gameObject = nm(gp(16), col, row, -1);
			GameManager.Instance.mTruc = gameObject;
			Truc component = gameObject.GetComponent<Truc>();
			component.setTruc((int)(Random.value * 3f) + 1, false);
		}
	}

	private static void rBubbleBlast(ByteArrayReader j)
	{
		int v = j.ReadInt16();
		int v2 = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.Read_Tag(f);
		if (check_Mode(f) && !VarsManager.getSecret(num))
		{
			add_BubbleBlast(new Vector3(scaleX(v), scaleY(v2)), num);
		}
	}

	private static void rBoss1(ByteArrayReader j)
	{
		int v = j.ReadInt16();
		int v2 = j.ReadInt16();
		int f = j.ReadByte();
		int num = j.Read_Tag(f);
		if (check_Mode(f) && !VarsManager.getSecret(num))
		{
			add_Boss1(new Vector3(scaleX(v), scaleY(v2)), num);
		}
	}

	private static void rSwitch(ByteArrayReader j)
	{
		int v = j.ReadInt16();
		int v2 = j.ReadInt16();
		int f = j.ReadByte();
		int pAngle = j.Read_Angle(f);
		int num = j.Read_Flip(f);
		int num2 = j.Read_OfsX(f);
		int num3 = j.Read_OfsY(f);
		int pTagID = j.Read_Tag(f);
		int f2 = j.ReadByte();
		bool pEnabled = check_Enabled(f2);
		int pTtl = (check_IsLoop(f2) ? j.ReadInt32() : 0);
		if (check_Mode(f))
		{
			BlocSwitch blocSwitch = add_Switch(new Vector3(scaleX(v), scaleY(v2)));
			blocSwitch.setup(pAngle, FLIP_H[num], FLIP_V[num], num2, num3, 0f, pTagID, pTtl, pEnabled);
		}
	}

	private static void rBox(ByteArrayReader j)
	{
		int num = j.ReadInt16();
		int num2 = j.ReadInt16();
		int num3 = j.ReadInt16();
		int num4 = j.ReadInt16();
		GameObject gameObject = nm(gp(-1), num, num2, 0);
		gameObject.layer = 9;
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		int num5 = (num3 - num) * 50;
		int num6 = (num4 - num2) * 50;
		int num7 = num5 >> 1;
		int num8 = num6 >> 1;
		boxCollider.center = new Vector3(num7 - 25, -num8 + 25, 0f);
		boxCollider.size = new Vector3(num5 + 1, num6 + 1, 200f);
		boxCollider.material = GameManager.Instance.defaultMaterial;
		boxCollider.enabled = true;
	}

	private static bool check_Enabled(int f)
	{
		return (f & 1) == 1;
	}

	private static bool check_IsLoop(int f)
	{
		return (f & 2) == 2;
	}

	private static bool check_Angle(int f)
	{
		return (f & 1) == 1;
	}

	private static bool check_Flip(int f)
	{
		return (f & 2) == 2;
	}

	private static bool check_OfsX(int f)
	{
		return (f & 4) == 4;
	}

	private static bool check_OfsY(int f)
	{
		return (f & 8) == 8;
	}

	private static bool check_Mode(int f)
	{
		return ((f / 16) & 7 & VarsManager.GameMode) != 0;
	}

	private static bool check_Tag(int f)
	{
		return (f & 0x80) == 128;
	}

	private static bool loadLevel(string assetName)
	{
		TextAsset textAsset = (TextAsset)Resources.Load("Levels/" + assetName, typeof(TextAsset));
		if (textAsset == null)
		{
			return false;
		}
		byte[] bytes = textAsset.bytes;
		if (bytes == null)
		{
			return false;
		}
		decors = new ListOf<Decor>();
		ByteArrayReader byteArrayReader = new ByteArrayReader(bytes);
		int num = byteArrayReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			switch ((int)byteArrayReader.ReadByte())
			{
			case 0:
				rTile(byteArrayReader);
				break;
			case 1:
				rUpDown(byteArrayReader);
				break;
			case 2:
				rHydrant(byteArrayReader);
				break;
			case 3:
				rCoin(byteArrayReader);
				break;
			case 4:
				rBomber(byteArrayReader);
				break;
			case 5:
				rCrawler(byteArrayReader);
				break;
			case 6:
				rFireman(byteArrayReader);
				break;
			case 7:
				rFlammer(byteArrayReader);
				break;
			case 8:
				rRunner(byteArrayReader);
				break;
			case 9:
				rSpider(byteArrayReader);
				break;
			case 10:
				rThrower(byteArrayReader);
				break;
			case 11:
				rVictim(byteArrayReader);
				break;
			case 12:
				rExit(byteArrayReader);
				break;
			case 13:
				rHurt(byteArrayReader);
				break;
			case 14:
				rTeleport(byteArrayReader);
				break;
			case 15:
				rTrigger(byteArrayReader);
				break;
			case 16:
				rPet(byteArrayReader);
				break;
			case 17:
				rHiddenObject(byteArrayReader);
				break;
			case 18:
				rBubbleBlast(byteArrayReader);
				break;
			case 19:
				rBoss1(byteArrayReader);
				break;
			case 20:
				rSwitch(byteArrayReader);
				break;
			case 21:
				rTank(byteArrayReader);
				break;
			}
		}
		num = byteArrayReader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			rBox(byteArrayReader);
		}
		GameManager.decors = decors.ToArray();
		if (GameManager.pompiers.Count > 0)
		{
			GameManager.Instance.Player1 = GameManager.pompiers[0];
			if (VarsManager.Instance.twoPlayer)
			{
				add_Pompier(GameManager.Instance.Player1.transform.localPosition);
			}
		}
		for (int k = 0; k < GameManager.ennemies.Count; k++)
		{
			GameManager.ennemies[k].OnActivate();
		}
		decors = null;
		return true;
	}

	public static T CreateObject<T>(string n, Vector3 p, GameObject prefab) where T : MonoBehaviour
	{
		GameObject gameObject = Object.Instantiate(prefab);
		Transform transform = gameObject.transform;
		transform.parent = GameManager.instance.panelGame.transform;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		transform.localPosition = p;
		return gameObject.GetComponent<T>();
	}

	public static T CreateObject<T>(string n, Vector3 p) where T : MonoBehaviour
	{
		GameObject gameObject = new GameObject(n);
		Transform transform = gameObject.transform;
		transform.parent = GameManager.instance.panelGame.transform;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		transform.localPosition = p;
		return gameObject.AddComponent<T>();
	}

	public static BubbleBlast add_BubbleBlast(Vector3 p, int tag)
	{
		BubbleBlast bubbleBlast = CreateObject<BubbleBlast>("BubbleBlast", p);
		GameManager.ennemies.Add(bubbleBlast);
		bubbleBlast.TagID = tag;
		return bubbleBlast;
	}

	public static Boss1 add_Boss1(Vector3 p, int tag)
	{
		Boss1 boss = CreateObject<Boss1>("Boss1", p);
		GameManager.ennemies.Add(boss);
		boss.TagID = tag;
		return boss;
	}

	public static BlocSwitch add_Switch(Vector3 p)
	{
		BlocSwitch blocSwitch = CreateObject<BlocSwitch>("BlocSwitch", p);
		blocSwitch.gameObject.layer = 8;
		GameManager.switchables.Add(blocSwitch);
		GameManager.switches.Add(blocSwitch);
		return blocSwitch;
	}

	public static Projectile2 add_Projectile2(Vector3 p, Vector3 v)
	{
		Projectile2 projectile = CreateObject<Projectile2>("Projectile2", p);
		projectile.vel = v;
		return projectile;
	}

	public static Talky add_Talky()
	{
		GameObject gameObject = Object.Instantiate(GameManager.Instance.prefabTalky);
		Talky component = gameObject.GetComponent<Talky>();
		component.setTalky();
		return component;
	}

	public static Pompier add_Pompier(Vector3 p)
	{
		Pompier pompier = CreateObject<Pompier>("Pompier", p, GameManager.Instance.prefabPompier);
		pompier.playerIndex = GameManager.pompiers.Count;
		pompier.setPompier(pompier.transform.localPosition);
		GameManager.pompiers.Add(pompier);
		return pompier;
	}
}
