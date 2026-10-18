using UnityEngine;

public class Boss1 : BaseEntity
{
	private FootCollider footCollider;

	private SkeletonAnimation sprite;

	private float life;

	private int step;

	private float sens;

	private float hurt;

	private float timeFire;

	private float timeShoot;

	private float timeShrapnel;

	private int fireSpawnCount;

	private RaycastHit hit;

	private Ray ray;

	public void Awake()
	{
		ColX = 50;
		ColY = 50;
		sens = -1f;
		life = 50f;
		base.gameObject.layer = 10;
		fireSpawnCount = 3;
		timeShoot = Random.value * 3f + 3f;
		timeFire = Random.value * 4f + 4f;
		timeShrapnel = Random.value * 20f + 10f;
		sprite = this.New<SkeletonAnimation>("anim", base.gameObject.transform, new Vector3(10f, -20f, -710f));
		sprite.skeletonDataAsset = SpineData.Instance.dataEnnemi2;
		sprite.Initialize();
		sprite.state.SetAnimation("walk", true);
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.material = GameManager.instance.defaultMaterial;
		boxCollider.size = new Vector3(35f, 35f, 50f);
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.constraints = (RigidbodyConstraints)120;
		rigidbody.useGravity = true;
		rigidbody.drag = 2f;
		rigidbody.angularDrag = 0f;
		GameObject gameObject = new GameObject("FootCollider");
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = new Vector3(0f, -16f, 0f);
		gameObject.layer = 10;
		boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.material = GameManager.instance.defaultMaterial;
		boxCollider.size = new Vector3(30f, 6f, 50f);
		boxCollider.isTrigger = true;
		footCollider = gameObject.AddComponent<FootCollider>();
		ray = new Ray(Vector3.zero, Vector3.zero);
		hit = default(RaycastHit);
	}

	public void Start()
	{
		base.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		if (Mathf.Abs(base.transform.position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			sprite.GetComponent<Renderer>().enabled = false;
			return;
		}
		sprite.GetComponent<Renderer>().enabled = true;
		if (timeShoot > 0f)
		{
			timeShoot -= Time.deltaTime;
		}
		else
		{
			timeShoot = Random.value * 3f + 3f;
			SpawnShoot();
		}
		if (timeShrapnel > 0f)
		{
			timeShrapnel -= Time.deltaTime;
		}
		else
		{
			timeShrapnel = Random.value * 20f + 10f;
			SpawnShrapnel();
		}
		if (timeFire > 0f)
		{
			timeFire -= Time.deltaTime;
		}
		else if (fireSpawnCount > 0)
		{
			if (footCollider.isTouch && HasFloor() && SpawnFire())
			{
				fireSpawnCount--;
				timeFire = 0.5f;
			}
		}
		else
		{
			timeFire = Random.value * 4f + 4f;
			fireSpawnCount = 3;
		}
		if (hurt > 0f)
		{
			hurt -= Time.deltaTime;
			if (hurt < 0f)
			{
				hurt = 0f;
			}
		}
		float num = GameManager.Instance.ratioEnnemiSpeed * 0.4f;
		if (hurt > 0f)
		{
			num *= 0.6f;
		}
		GetComponent<Rigidbody>().velocity = new Vector3(num * sens, GetComponent<Rigidbody>().velocity.y, 0f);
		Transform transform2 = GameManager.instance.Player1.transform;
		if (Common.Absi(base.transform.localPosition.x - transform2.localPosition.x) < ColX && Common.Absi(base.transform.localPosition.y - transform2.localPosition.y) < ColY)
		{
			GameManager.instance.Player1.laMort();
		}
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		hurt = 0.3f;
		if (life > 0f)
		{
			life -= 0.1f;
			if (life <= 0f)
			{
				life = 0f;
				Die();
			}
		}
		if (goutte.vel.x < 0f)
		{
			GotoLeft();
		}
		else
		{
			GotoRight();
		}
		goutte.GetComponent<Goutte>().ruisselle();
	}

	public override void Die()
	{
		Sounds.snd_mob5_kill.Play();
		GameManager.GiveBonus(base.transform.position, Config.bonuses.boss1, false);
		Object.Instantiate(GameManager.Instance.prefabPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, -2f), Quaternion.identity);
		Object.Destroy(base.gameObject);
		if (TagID > 0)
		{
			VarsManager.setSecret(TagID, true, true);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		col(collision);
	}

	public void OnCollisionStay(Collision collision)
	{
		col(collision);
	}

	private bool HasFloor()
	{
		ray.origin = base.transform.position;
		ray.direction = Vector3.down;
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 512))
		{
			float num = base.transform.localPosition.y - hit.transform.localPosition.y;
			return num < 70f;
		}
		return false;
	}

	private void col(Collision c)
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < c.contacts.Length; i++)
		{
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, 1f))
			{
				flag2 = true;
			}
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, -1f))
			{
				flag = true;
			}
		}
		if (flag && footCollider.isTouch)
		{
			if (base.transform.localPosition.x > GameManager.Instance.Player1.transform.localPosition.x)
			{
				GotoRight();
			}
			else
			{
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 2.88f, 0f);
			}
		}
		if (flag2 && footCollider.isTouch)
		{
			if (base.transform.localPosition.x < GameManager.Instance.Player1.transform.localPosition.x)
			{
				GotoLeft();
			}
			else
			{
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 2.88f, 0f);
			}
		}
	}

	private void GotoLeft()
	{
		if (sens != 1f)
		{
			sens = 1f;
			sprite.transform.localScale = new Vector3(-1f, 1f, 1f);
			sprite.transform.localPosition = new Vector3(-10f, -20f, -710f);
		}
	}

	private void GotoRight()
	{
		if (sens != -1f)
		{
			sens = -1f;
			sprite.transform.localScale = new Vector3(1f, 1f, 1f);
			sprite.transform.localPosition = new Vector3(10f, -20f, -710f);
		}
	}

	public bool SpawnFire()
	{
		float num = Common.RoundTo(base.transform.localPosition.x - 2f, 50f);
		float num2 = Common.RoundTo(base.transform.localPosition.y + 2f, 50f);
		ListOf<BaseEntity> ennemies = GameManager.ennemies;
		for (int i = 0; i < ennemies.Count; i++)
		{
			BaseEntity baseEntity = ennemies[i];
			if (baseEntity != null && baseEntity.BaseType == 1 && Common.Absi(baseEntity.transform.localPosition.x - num) < 50 && Common.Absi(baseEntity.transform.localPosition.y - num2) < 50)
			{
				return false;
			}
		}
		GameObject gameObject = Fire.create().gameObject;
		gameObject.transform.localPosition = new Vector3(num, num2, 0f);
		Ennemi1 component = gameObject.GetComponent<Ennemi1>();
		component.giveNothing = true;
		GameManager.AddEnnemie(component);
		Sounds.snd_mob3_detect.Play();
		return true;
	}

	public void SpawnShrapnel()
	{
		Sounds.snd_mob5_shoot.Play();
		Transform transform = GameManager.Instance.Player1.transform;
		float x = transform.localPosition.x - base.transform.localPosition.x;
		float y = transform.localPosition.y - base.transform.localPosition.y;
		float num = Mathf.Atan2(y, x);
		for (float num2 = 0f; num2 < 6.28f; num2 += 0.785f)
		{
			LevelLoader.add_Projectile2(base.transform.localPosition, new Vector3(Mathf.Cos(num + num2) * 300f, Mathf.Sin(num + num2) * 300f));
		}
	}

	public void SpawnShoot()
	{
		Object.Instantiate(GameManager.Instance.prefabPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, 0f), Quaternion.identity);
		Sounds.snd_mob5_shoot.Play();
		GameManager.newProjectile(base.transform.localPosition.x, base.transform.localPosition.y, 45f, 470f);
		GameManager.newProjectile(base.transform.localPosition.x, base.transform.localPosition.y, 135f, 470f);
	}
}
