using UnityEngine;

public class Ennemi6 : BaseEntity
{
	private const int ACTION_CLIMBING = 0;

	private const int ACTION_FALLING = 1;

	private const int ACTION_IDLE_DOWN = 2;

	private const int ACTION_IDLE_UP = 3;

	private const int ACTION_FOLLOWING_DOWN = 4;

	private const int ACTION_FOLLOWING_UP = 5;

	private const float SPEED_CRAWLING = 0.5f;

	private const float SPEED_CLIMBING = 0.5f;

	private const int RAY_THRESHOLD = 6;

	public GameObject mAnim;

	public FootCollider footCollider;

	public UISprite spriteString;

	private int[] rayTick;

	private int action;

	private float speed_crawling;

	private float speed_climbing;

	private float crawlingTime;

	private float followMinTime;

	private float stringTop;

	private float stringBottom;

	private float life;

	private float flip;

	private SkeletonAnimation skeleton;

	private float startX;

	private float maxDistX;

	private RaycastHit hit;

	private Ray ray;

	public void setEnnemi(Vector3 pos)
	{
		ray = new Ray(Vector3.zero, Vector3.zero);
		hit = default(RaycastHit);
		rayTick = new int[3];
		spriteString.transform.localScale = new Vector3(spriteString.transform.localScale.x, 1f, 1f);
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = pos;
		startX = pos.x;
		maxDistX = 100f;
		flip = 1f;
		life = 1f;
		followMinTime = Random.Range(0.5f, 3f);
		action = 1;
		GetComponent<Rigidbody>().useGravity = true;
		skeleton = mAnim.GetComponent<SkeletonAnimation>();
		skeleton.skeletonDataAsset = SpineData.Instance.dataEnnemi6;
		skeleton.Initialize();
		skeleton.state.SetAnimation("fall", true);
	}

	public override void OnActivate()
	{
		Climb(true);
	}

	private void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		if (Mathf.Abs(base.transform.position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
			spriteString.enabled = false;
		}
		else
		{
			mAnim.GetComponent<Renderer>().enabled = true;
			spriteString.enabled = true;
		}
		Pompier player = GameManager.Instance.Player1;
		if (Common.Absi(base.transform.localPosition.x - player.transform.localPosition.x) < 45 && Common.Absi(base.transform.localPosition.y - player.transform.localPosition.y) < 45)
		{
			player.laMort();
			return;
		}
		float num = player.transform.localPosition.x - base.transform.localPosition.x;
		float num2 = player.transform.localPosition.y - base.transform.localPosition.y;
		float num3 = Mathf.Abs(num);
		float num4 = Mathf.Abs(num2);
		if (footCollider.isTouch)
		{
			if (action == 1)
			{
				if (num3 < 200f || followMinTime > 0f)
				{
					Follow_Down();
				}
				else
				{
					Idle_Down();
				}
			}
		}
		else if (GetComponent<Rigidbody>().useGravity)
		{
			action = 1;
		}
		if (action == 0)
		{
			if (!CheckAndFall(num, num2, num3, num4, player))
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f, 0.5f, 0f);
				stringBottom = base.transform.localPosition.y;
				spriteString.transform.localScale = new Vector3(spriteString.transform.localScale.x, 0f - (stringBottom - stringTop), 1f);
			}
		}
		else if (action == 4)
		{
			if ((num3 > 200f || num4 > 100f) && followMinTime > 0f)
			{
				Idle_Down();
				return;
			}
			followMinTime -= Time.deltaTime;
			if (num3 > 25f)
			{
				flip = Mathf.Sign(num);
			}
			GetComponent<Rigidbody>().velocity = new Vector3(0.5f * flip, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else if (action == 5)
		{
			if (CheckAndFall(num, num2, num3, num4, player))
			{
				return;
			}
			if ((num3 > 250f || num4 > 350f) && followMinTime > 0f)
			{
				Idle_Up();
				return;
			}
			if (!HasAnchor())
			{
				Fall();
				return;
			}
			followMinTime -= Time.deltaTime;
			if (num3 > 25f)
			{
				flip = Mathf.Sign(num);
			}
			GetComponent<Rigidbody>().velocity = new Vector3(0.5f * flip, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else if (action == 2)
		{
			if (num3 < 200f && num4 < 100f)
			{
				Follow_Down();
				return;
			}
			if (crawlingTime <= 0f)
			{
				Climb(false);
				return;
			}
			crawlingTime -= Time.deltaTime;
			if (base.transform.localPosition.x - startX > maxDistX)
			{
				flip = -1f;
			}
			else if (base.transform.localPosition.x - startX < 0f - maxDistX)
			{
				flip = 1f;
			}
			GetComponent<Rigidbody>().velocity = new Vector3(0.5f * flip, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else if (action == 3)
		{
			if (num2 <= 0f && num3 < 250f && num4 < 350f)
			{
				Follow_Up();
			}
			else
			{
				CheckAndFall(num, num2, num3, num4, player);
			}
		}
	}

	private bool CheckAndFall(float dx, float dy, float rx, float ry, Pompier fireman)
	{
		if (dy <= 0f && rx < 100f && ry < 350f && CanFall(fireman))
		{
			Fall();
			flip = Mathf.Sign(dx);
			GetComponent<Rigidbody>().velocity = new Vector3(0.5f * flip, 0f, 0f);
			return true;
		}
		return false;
	}

	private bool canRay(int index)
	{
		rayTick[index] = (rayTick[index] + 1) % 6;
		return rayTick[index] == 0;
	}

	private void Idle_Up()
	{
		skeleton.state.SetAnimation("walk", true);
		action = 3;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		followMinTime = 0f;
	}

	private void Idle_Down()
	{
		skeleton.state.SetAnimation("walk", true);
		action = 2;
		GetComponent<Rigidbody>().useGravity = true;
		flip = ((!(Random.value >= 0.5f)) ? (-1f) : 1f);
		crawlingTime = 3f;
		followMinTime = 0f;
	}

	private void Follow_Down()
	{
		skeleton.state.SetAnimation("walk", true);
		action = 4;
		GetComponent<Rigidbody>().useGravity = true;
	}

	private void Follow_Up()
	{
		skeleton.state.SetAnimation("walk", true);
		action = 5;
		GetComponent<Rigidbody>().useGravity = false;
	}

	private bool Climb(bool force)
	{
		if (canRay(0) || force)
		{
			ray.origin = base.transform.position;
			ray.direction = Vector3.up;
			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 512))
			{
				stringBottom = base.transform.localPosition.y;
				stringTop = hit.transform.localPosition.y - 20f;
				spriteString.transform.localScale = new Vector3(spriteString.transform.localScale.x, 0f - (stringBottom - stringTop), 1f);
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().useGravity = false;
				action = 0;
				followMinTime = 0f;
				skeleton.state.SetAnimation("walk", true);
				return true;
			}
		}
		return false;
	}

	private bool CanFall(Pompier fireman)
	{
		bool result = false;
		if (canRay(1))
		{
			ray.origin = base.transform.position;
			ray.direction = (fireman.transform.position - ray.origin).normalized;
			int layer = footCollider.gameObject.layer;
			footCollider.gameObject.layer = 2;
			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 1536) && hit.transform.gameObject == fireman.gameObject)
			{
				result = true;
			}
			footCollider.gameObject.layer = layer;
		}
		return result;
	}

	private bool HasAnchor()
	{
		if (canRay(2))
		{
			ray.origin = base.transform.position;
			ray.direction = Vector3.up;
			if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 512))
			{
				return hit.distance < 0.1f;
			}
			return false;
		}
		return true;
	}

	private void Fall()
	{
		Sounds.snd_mob6_detect.Play();
		spriteString.transform.localScale = new Vector3(spriteString.transform.localScale.x, 1f, 1f);
		followMinTime = Random.Range(0.5f, 3f);
		action = 1;
		GetComponent<Rigidbody>().useGravity = true;
		skeleton.state.SetAnimation("fall", true);
	}

	public override void Die()
	{
		Sounds.snd_mob6_kill.Play();
		GameManager.GiveBonus(base.transform.position, Config.bonuses.spider, false);
		GameManager.Instance.spiderExtinguish++;
		Object.Destroy(base.gameObject);
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (!(life > 0f))
		{
			return;
		}
		life -= 0.1f;
		if (life < 0f)
		{
			Die();
		}
		if (action != 0 && action != 3 && action != 5)
		{
			if (goutte.GetComponent<Goutte>().vel.x > 0f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
			}
			else
			{
				GetComponent<Rigidbody>().velocity = new Vector3(-1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
			}
		}
		goutte.ruisselle();
	}

	private void OnCollisionEnter(Collision collision)
	{
		col(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		col(collision);
	}

	private void col(Collision c)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		float num = 0f;
		for (int i = 0; i < c.contacts.Length; i++)
		{
			if (Common.ApproximatelyEquals(c.contacts[i].normal.y, -1f))
			{
				num = c.contacts[i].point.y - 20f;
				flag = true;
			}
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, 1f))
			{
				flag3 = true;
			}
			if (Common.ApproximatelyEquals(c.contacts[i].normal.x, -1f))
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			if (action == 2)
			{
				flip = -1f;
			}
			if (action == 4)
			{
				Climb(false);
			}
		}
		if (flag3)
		{
			if (action == 2)
			{
				flip = 1f;
			}
			if (action == 4)
			{
				Climb(false);
			}
		}
		if (flag && action == 0)
		{
			stringTop = num;
			Idle_Up();
		}
	}
}
