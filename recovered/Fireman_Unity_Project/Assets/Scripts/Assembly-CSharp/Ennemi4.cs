using UnityEngine;

public class Ennemi4 : BaseEntity
{
	public FootCollider footCollider;

	public GameObject mAnim;

	private bool exploded;

	private float flip;

	private float timeBlink;

	private float timeExplode;

	private bool detect;

	private float life;

	private SkeletonAnimation spine;

	public void setEnnemi(Vector3 pos)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localPosition = pos;
		base.transform.localScale = Vector3.one;
		flip = 1f;
		spine = mAnim.GetComponent<SkeletonAnimation>();
		spine.skeletonDataAsset = SpineData.Instance.dataEnnemi2;
		spine.Initialize();
		spine.state.SetAnimation("walk", true);
		life = 1f;
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
		timeBlink += 15f * Time.deltaTime;
		if (timeBlink > 1f)
		{
			timeBlink = -1f;
		}
		mAnim.transform.localScale = new Vector3(1f * flip, 1f, 1f);
		collisionPompier();
		if (timeExplode > 0f)
		{
			if (timeBlink >= 0f)
			{
				mAnim.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				mAnim.GetComponent<Renderer>().enabled = false;
			}
			timeExplode += Time.deltaTime;
		}
		if (exploded)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void collisionPompier()
	{
		float num = GameManager.Instance.ratioEnnemiSpeed * 0.7f;
		Pompier player = GameManager.instance.Player1;
		if (Common.Absi(base.transform.localPosition.x - player.transform.localPosition.x) < 45 && Common.Absi(base.transform.localPosition.y - player.transform.localPosition.y) < 45)
		{
			player.laMort();
			return;
		}
		if (timeExplode >= 1f)
		{
			for (int i = 0; i < 7; i++)
			{
				GameManager.newProjectile(base.transform.localPosition.x, base.transform.localPosition.y, 10 + i * 22, 375f + Random.value * 250f);
			}
			exploded = true;
			Sounds.snd_mob4_kill.Play();
		}
		if (Mathf.Abs(base.transform.localPosition.x - player.transform.localPosition.x) < 700f && Mathf.Abs(base.transform.localPosition.x - player.transform.localPosition.x) > 10f && Mathf.Abs(base.transform.localPosition.y - player.transform.localPosition.y) < 600f && !exploded)
		{
			if (!detect)
			{
				if (!Sounds.snd_mob4_detect.isPlaying)
				{
					Sounds.snd_mob4_detect.Play();
				}
				detect = true;
			}
			if (Mathf.Abs(base.transform.localPosition.x - player.transform.localPosition.x) < 250f && Mathf.Abs(base.transform.localPosition.y - player.transform.localPosition.y) < 100f && timeExplode == 0f)
			{
				timeExplode = 0.001f;
			}
			if (player.transform.position.x < base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f - num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = 1f;
			}
			if (player.transform.position.x > base.transform.position.x)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(num, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				flip = -1f;
			}
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			detect = false;
		}
	}

	public override void Die()
	{
		Sounds.snd_mob5_kill.Play();
		GameManager.GiveBonus(base.transform.position, Config.bonuses.bomber, false);
		GameManager.Instance.bomberExtinguish++;
		Object.Destroy(base.gameObject);
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (life > 0f)
		{
			life -= 0.05f;
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
