using UnityEngine;

public class Ennemi5 : BaseEntity
{
	public FootCollider footCollider;

	public GameObject mAnim;

	private SkeletonAnimation spine;

	private float timeMort;

	private float life;

	private float timeShoot;

	private float rndDist;

	private float flip;

	public void setEnnemi(Vector3 pos)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localPosition = pos;
		base.transform.localScale = Vector3.one;
		flip = 1f;
		life = 1f;
		spine = mAnim.GetComponent<SkeletonAnimation>();
		spine.skeletonDataAsset = SpineData.Instance.dataEnnemi5;
		spine.Initialize();
		spine.state.SetAnimation("walk", true);
		rndDist = 0.9f + Random.value * 0.2f;
	}

	private void Start()
	{
		setEnnemi(base.transform.localPosition);
	}

	private void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		if (Mathf.Abs(base.transform.position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
			return;
		}
		mAnim.GetComponent<Renderer>().enabled = true;
		mAnim.transform.localScale = new Vector3(1f * flip, 1f, 1f);
		collisionPompier();
		if (timeMort > 0f)
		{
			timeMort += Time.deltaTime;
		}
		if (timeMort >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
		if (timeShoot > 0f)
		{
			timeShoot -= Time.deltaTime;
		}
		if (spine.state.Animation.Name.Equals("shoot") && spine.state.Time >= spine.state.Animation.Duration)
		{
			spine.state.AddAnimation("walk", true);
		}
	}

	public void collisionPompier()
	{
		Pompier player = GameManager.instance.Player1;
		float num = GameManager.Instance.ratioEnnemiSpeed * 0.6f;
		float num2 = base.transform.localPosition.x - player.transform.localPosition.x;
		float num3 = base.transform.localPosition.y - player.transform.localPosition.y;
		float num4 = Mathf.Sqrt(num2 * num2 + num3 * num3);
		if (Common.Absi(num2) < 45 && Common.Absi(num3) < 45)
		{
			player.laMort();
			return;
		}
		if (num4 < 300f * rndDist)
		{
			if (GameManager.Instance.Player1.transform.position.x < base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = -1f;
			}
			if (GameManager.Instance.Player1.transform.position.x > base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f - num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = 1f;
			}
		}
		else if (num4 > 310f * rndDist && num4 < 400f * rndDist)
		{
			if (GameManager.Instance.Player1.transform.position.x < base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f - num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = 1f;
			}
			if (GameManager.Instance.Player1.transform.position.x > base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = -1f;
			}
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		if (!(num4 < 310f * rndDist) || !(num4 > 100f * rndDist))
		{
			return;
		}
		if (GameManager.Instance.Player1.transform.position.x < base.transform.position.x)
		{
			flip = 1f;
		}
		if (GameManager.Instance.Player1.transform.position.x > base.transform.position.x)
		{
			flip = -1f;
		}
		if (timeShoot <= 0f)
		{
			timeShoot = 1f;
			spine.state.AddAnimation("shoot", false);
			Sounds.snd_mob5_shoot.Play();
			if (base.transform.position.x < GameManager.Instance.Player1.transform.position.x)
			{
				GameManager.newProjectile(base.transform.localPosition.x, base.transform.localPosition.y, 45f, 470f);
			}
			else
			{
				GameManager.newProjectile(base.transform.localPosition.x, base.transform.localPosition.y, 135f, 470f);
			}
		}
	}

	public override void Die()
	{
		Sounds.snd_mob5_kill.Play();
		GameManager.GiveBonus(base.transform.position, Config.bonuses.thrower, false);
		GameManager.Instance.throwerExtinguish++;
		Object.Destroy(base.gameObject);
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (life > 0f)
		{
			life -= 0.1f;
			if (life < 0f)
			{
				life = 0f;
				Die();
			}
			if (goutte.GetComponent<Goutte>().vel.x > 0f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
			}
			else
			{
				GetComponent<Rigidbody>().velocity = new Vector3(-1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
			}
			goutte.ruisselle();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		col(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		col(collision);
	}

	private void col(Collision collision)
	{
		bool flag = false;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			if (collision.contacts[i].point.y >= base.transform.position.y)
			{
				flag = true;
			}
		}
		if (flag && collision.impactForceSum.x != 0f && footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 3f, GetComponent<Rigidbody>().velocity.z);
		}
	}
}
