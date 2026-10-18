using UnityEngine;

public class Coin : BaseSpawnable
{
	public GameObject mAnim;

	public bool isStatic;

	public FootCollider footCollider;

	private SkeletonAnimation spine;

	private float timeCanTake;

	private float rebond;

	private float timeLife;

	private float timeBlink;

	private int coins;

	public void setCoin(Vector3 pPos, Vector2 pVel, int pValue)
	{
		spine = mAnim.GetComponent<SkeletonAnimation>();
		spine.skeletonDataAsset = SpineData.Instance.dataCoin;
		spine.Initialize();
		spine.state.SetAnimation("animation", true);
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.position = pPos;
		GetComponent<Rigidbody>().velocity = pVel;
		GetComponent<Rigidbody>().useGravity = true;
		coins = pValue;
		rebond = 2f;
		timeCanTake = 0.3f;
		timeLife = 5f;
	}

	public void Start()
	{
		setCoin(base.transform.position, GetComponent<Rigidbody>().velocity, 1);
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = GameManager.mainCamera.transform.position;
		if (Mathf.Abs(position.x - position2.x) > (float)GameManager.tailleZoneX || Mathf.Abs(position.y - position2.y) > (float)GameManager.tailleZoneY)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
			return;
		}
		mAnim.GetComponent<Renderer>().enabled = true;
		position = base.transform.localPosition;
		position2 = GameManager.Instance.Player1.transform.localPosition;
		int num = Common.Absi(position.x - position2.x);
		int num2 = Common.Absi(position.y - position2.y);
		if (num < 40 && num2 < 40 && timeCanTake < 0f)
		{
			Sounds.snd_ig_coin.Play();
			timeCanTake = 0f;
			GameManager.Instance.pieceOwned += coins;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			mAnim.transform.localPosition = new Vector3(mAnim.transform.localPosition.x, mAnim.transform.localPosition.y, -1200f);
			base.Deleted = true;
		}
		float deltaTime = Time.deltaTime;
		if (timeCanTake > 0f)
		{
			timeCanTake -= deltaTime;
		}
		timeBlink += 15f * deltaTime;
		if (timeBlink > 1f)
		{
			timeBlink = -1f;
		}
		if (isStatic)
		{
			return;
		}
		timeLife -= deltaTime;
		if (timeLife <= 1.5f)
		{
			if (timeBlink >= 0f)
			{
				mAnim.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				mAnim.GetComponent<Renderer>().enabled = false;
			}
		}
		if (timeLife <= 0f)
		{
			base.Deleted = true;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		bool flag = false;
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			if (collision.contacts[i].point.y < base.transform.position.y)
			{
				flag = true;
			}
		}
		if (flag && collision.impactForceSum.y > 0f && footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, rebond, GetComponent<Rigidbody>().velocity.z);
			rebond *= 0.6f;
		}
	}
}
