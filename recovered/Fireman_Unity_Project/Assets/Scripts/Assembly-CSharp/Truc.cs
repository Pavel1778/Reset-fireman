using UnityEngine;

public class Truc : MonoBehaviour
{
	public FootCollider footCollider;

	public GameObject mAnim;

	private float timeMicroJump;

	private bool follow;

	private Vector3 posTampon;

	private int verifTp;

	private SkeletonAnimation spine;

	public void setTruc(int skin, bool follow)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		GameManager.Instance.mTruc = base.gameObject;
		spine = mAnim.GetComponent<SkeletonAnimation>();
		Pets.Setup(spine, skin - 1);
		spine.state.SetAnimation("idle", true);
		this.follow = follow;
	}

	public void Update()
	{
		if (verifTp == 0 && footCollider.isTouch)
		{
			verifTp = 1;
			mAnim.GetComponent<Renderer>().enabled = true;
		}
		if (verifTp < 0)
		{
			verifTp++;
		}
		timeMicroJump -= Time.deltaTime;
		if (timeMicroJump < 0f && footCollider.isTouch && GetComponent<Rigidbody>().velocity.x != 0f)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 1.2f, GetComponent<Rigidbody>().velocity.z);
			timeMicroJump = Random.value * 3f;
		}
		posTampon = new Vector3(posTampon.x + (base.transform.position.x - posTampon.x) * 0.65f, posTampon.y + (base.transform.position.y - posTampon.y) * 0.65f, posTampon.z);
		bool flag = false;
		Vector3 localPosition = base.transform.localPosition;
		ListOf<BaseEntity> activeEnnemies = GameManager.activeEnnemies;
		for (int i = 0; i < activeEnnemies.Count; i++)
		{
			BaseEntity baseEntity = activeEnnemies[i];
			if (!(baseEntity != null) || baseEntity.BaseType != 1 || !(baseEntity.GetComponent<Ennemi1>().life <= 0f))
			{
				continue;
			}
			Vector3 localPosition2 = baseEntity.gameObject.transform.localPosition;
			if (!(Mathf.Abs(localPosition.x - localPosition2.x) < 300f) || !(Mathf.Abs(localPosition.y - localPosition2.y) < 50f))
			{
				continue;
			}
			if (!flag && Mathf.Abs(localPosition.x - localPosition2.x) < 40f && GetComponent<Rigidbody>().velocity.y < 0.1f && GetComponent<Rigidbody>().velocity.y > -0.1f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 1.2f, GetComponent<Rigidbody>().velocity.z);
			}
			if (Mathf.Abs(localPosition.x - localPosition2.x) > 20f)
			{
				if (localPosition.x < localPosition2.x)
				{
					GetComponent<Rigidbody>().velocity = new Vector3(0.8f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				}
				else
				{
					GetComponent<Rigidbody>().velocity = new Vector3(-0.8f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				}
			}
			else
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			}
			flag = true;
		}
		if (!flag)
		{
			collisionPompier();
		}
		if (GetComponent<Rigidbody>().velocity.x == 0f && GetComponent<Rigidbody>().velocity.y < 0.1f)
		{
			if (!spine.state.Animation.Name.Equals("idle"))
			{
				spine.state.SetAnimation("idle", true);
			}
		}
		else if (!spine.state.Animation.Name.Equals("animation"))
		{
			spine.state.SetAnimation("animation", true);
		}
		if (GetComponent<Rigidbody>().velocity.x < 0f)
		{
			mAnim.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else if (GetComponent<Rigidbody>().velocity.x > 0f)
		{
			mAnim.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void collisionPompier()
	{
		GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		if (follow)
		{
			if ((Mathf.Abs(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x) > 500f || Mathf.Abs(base.transform.localPosition.y - GameManager.Instance.Player1.transform.localPosition.y) > 400f) && GameManager.Instance.Player1.canTpTruc())
			{
				base.transform.position = new Vector3(GameManager.Instance.Player1.transform.position.x + 0.2f, GameManager.Instance.Player1.transform.position.y, GameManager.Instance.Player1.transform.position.z);
				verifTp = -5;
				mAnim.GetComponent<Renderer>().enabled = false;
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
			else if (Mathf.Abs(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x) > 70f)
			{
				if (GameManager.Instance.Player1.transform.position.x < base.transform.position.x)
				{
					GetComponent<Rigidbody>().velocity = new Vector3(-0.8f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				}
				if (GameManager.Instance.Player1.transform.position.x > base.transform.position.x)
				{
					GetComponent<Rigidbody>().velocity = new Vector3(0.8f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				}
			}
			else if (Mathf.Abs(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x) < 55f)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			}
		}
		else if (Mathf.Abs(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x) < 50f && Mathf.Abs(base.transform.localPosition.y - GameManager.Instance.Player1.transform.localPosition.y) < 50f)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z);
			follow = true;
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
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, Random.value * 2f + 1.5f, GetComponent<Rigidbody>().velocity.z);
		}
	}
}
