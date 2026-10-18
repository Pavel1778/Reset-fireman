using UnityEngine;

public class Ennemi2 : BaseEntity
{
	public SkeletonAnimation mAnim;

	private BoxCollider mCollider;

	public FootCollider footCollider;

	private float life;

	public bool isInLife;

	private bool detect;

	private float rndDist;

	private float flip;

	private float lflip;

	private bool arena;

	public void setEnnemi(Vector3 p)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = p;
		life = 5f;
		isInLife = true;
		arena = false;
		flip = -1f;
		lflip = 0f;
		rndDist = 0.9f + Random.value * 0.2f;
		mAnim.skeletonDataAsset = SpineData.Instance.dataEnnemi1;
		mAnim.Initialize();
		mAnim.state.SetAnimation("walk", true);
		mCollider = GetComponent<BoxCollider>();
	}

	public void Start()
	{
		setEnnemi(base.transform.localPosition);
	}

	public void Update()
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
		if (footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		if (life < 5f)
		{
			life += Time.deltaTime;
			if (life >= 5f)
			{
				life = 5f;
				isInLife = true;
				mAnim.GetComponent<SkeletonAnimation>().state.SetAnimation("walk", true);
			}
		}
		if (arena)
		{
			behaviour_arena();
		}
		else
		{
			behaviour_story();
		}
		behaviour_arena();
		mAnim.transform.localScale = new Vector3(1f * flip, 1f, 1f);
		if (lflip != flip)
		{
			Vector3 center = mCollider.center;
			if (flip > 0f)
			{
				mCollider.center = new Vector3(-9f, center.y, center.z);
			}
			else
			{
				mCollider.center = new Vector3(9f, center.y, center.z);
			}
			lflip = flip;
		}
	}

	public override void Die()
	{
		GameManager.GiveBonus(base.transform.position, Config.bonuses.runner, false);
		GameManager.Instance.flameboyExtinguish++;
		Object.Destroy(base.gameObject);
	}

	private void behaviour_arena()
	{
		Pompier player = GameManager.Instance.Player1;
		Vector3 localPosition = player.transform.localPosition;
		Vector3 position = player.transform.position;
		float num = GameManager.Instance.ratioEnnemiSpeed * 0.3f;
		if (Common.Absi(base.transform.localPosition.x - localPosition.x) < 45 && Common.Absi(base.transform.localPosition.y - localPosition.y) < 45)
		{
			if (isInLife)
			{
				player.laMort();
			}
			else
			{
				Die();
			}
			return;
		}
		if (isInLife)
		{
			GetComponent<Rigidbody>().velocity = new Vector3((0f - num) * flip, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		if (!isInLife && position.y < base.transform.position.y + 0.4f && footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 2f, GetComponent<Rigidbody>().velocity.z);
		}
	}

	private void behaviour_story()
	{
		Pompier player = GameManager.Instance.Player1;
		Vector3 localPosition = player.transform.localPosition;
		Vector3 position = player.transform.position;
		float num = -1f;
		float num2 = GameManager.Instance.ratioEnnemiSpeed * 0.3f;
		if (isInLife)
		{
			num = 1f;
			num2 = 0.5f * rndDist;
		}
		if (Common.Absi(base.transform.localPosition.x - localPosition.x) < 45 && Common.Absi(base.transform.localPosition.y - localPosition.y) < 45)
		{
			if (isInLife)
			{
				player.laMort();
			}
			else
			{
				Die();
			}
		}
		else if (Mathf.Abs(base.transform.localPosition.x - localPosition.x) < 500f * rndDist && Mathf.Abs(base.transform.localPosition.x - localPosition.x) > 5f)
		{
			if (!detect && isInLife)
			{
				if (!Sounds.snd_mob2_detect.isPlaying)
				{
					Sounds.snd_mob2_detect.Play();
				}
				detect = true;
				if (footCollider.isTouch)
				{
					GetComponent<Rigidbody>().velocity = new Vector3(0f, 1f, GetComponent<Rigidbody>().velocity.z);
				}
			}
			if (localPosition.x < base.transform.localPosition.x && GetComponent<Rigidbody>().velocity.x > -1f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3((0f - num2) * num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				if (isInLife)
				{
					flip = 1f;
				}
				else
				{
					flip = -1f;
				}
			}
			if (localPosition.x > base.transform.localPosition.x && GetComponent<Rigidbody>().velocity.x < 1f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(num2 * num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				if (isInLife)
				{
					flip = -1f;
				}
				else
				{
					flip = 1f;
				}
			}
			if (isInLife && position.y > base.transform.position.y + 0.4f && footCollider.isTouch)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 2.88f, GetComponent<Rigidbody>().velocity.z);
			}
			if (!isInLife && position.y < base.transform.position.y + 0.4f && footCollider.isTouch)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 2.4f, GetComponent<Rigidbody>().velocity.z);
			}
		}
		else
		{
			detect = false;
		}
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (life > 0f)
		{
			life -= 0.4f;
			if (life < 0f)
			{
				life = 0f;
				isInLife = false;
				if (!Sounds.snd_mob2_kill.isPlaying)
				{
					Sounds.snd_mob2_kill.Play();
				}
				mAnim.GetComponent<SkeletonAnimation>().state.SetAnimation("hurt", true);
			}
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

	public float getLife()
	{
		return life;
	}

	public void OnCollisionEnter(Collision c)
	{
		if (arena)
		{
			col_arena(c);
		}
		else
		{
			col_story(c);
		}
	}

	public void OnCollisionStay(Collision c)
	{
		if (arena)
		{
			col_arena(c);
		}
		else
		{
			col_story(c);
		}
	}

	private void col_story(Collision c)
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < c.contacts.Length; i++)
		{
			if (c.contacts[i].point.y >= base.transform.position.y)
			{
				flag = true;
				break;
			}
		}
		if (flag && c.impactForceSum.x != 0f && footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, 2.5f, GetComponent<Rigidbody>().velocity.z);
		}
	}

	private void col_arena(Collision c)
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < c.contacts.Length; i++)
		{
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, 1f) && footCollider.isTouch)
			{
				flag2 = true;
			}
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, -1f) && footCollider.isTouch)
			{
				flag = true;
			}
		}
		if (flag)
		{
			flip = 1f;
		}
		if (flag2)
		{
			flip = -1f;
		}
	}
}
