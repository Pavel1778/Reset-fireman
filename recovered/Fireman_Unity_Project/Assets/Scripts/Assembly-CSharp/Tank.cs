using System;
using UnityEngine;

public class Tank : BaseSpawnable
{
	public GameObject mAnim;

	public bool isStatic;

	private SkeletonAnimation spine;

	private float timeLife;

	private float timeBlink;

	public void setTank(Vector3 pPos, int pValue)
	{
		spine = mAnim.GetComponent<SkeletonAnimation>();
		spine.skeletonDataAsset = SpineData.Instance.dataTank;
		spine.Initialize();
		spine.state.SetAnimation("animation", true);
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.position = pPos;
		timeLife = 5f;
	}

	public void Start()
	{
		setTank(base.transform.position, 1);
	}

	public void Update()
	{
		if (base.Deleted)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = GameManager.mainCamera.transform.position;
		if (Mathf.Abs(position.x - position2.x) > (float)GameManager.tailleZoneX || Mathf.Abs(position.y - position2.y) > (float)GameManager.tailleZoneY)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
			return;
		}
		mAnim.GetComponent<Renderer>().enabled = true;
		Pompier player = GameManager.Instance.Player1;
		position2 = player.transform.localPosition;
		position = base.transform.localPosition;
		int num = Common.Absi(position.x - position2.x);
		int num2 = Common.Absi(position.y - position2.y);
		if (num < 40 && num2 < 40 && !player.IsFullWater())
		{
			base.Deleted = true;
			Sounds.snd_ig_checkpoint.Play();
			for (int i = 0; i < 4; i++)
			{
				int num3 = 45 + 22 * i;
				float num4 = UnityEngine.Random.value * 0.3f + 0.7f;
				GameManager.newGoutte(position.x, position.y, Mathf.Cos((float)Math.PI / 180f * (float)num3) * 1f * num4, Mathf.Sin((float)Math.PI / 180f * (float)num3) * 2.5f * num4, 1f);
			}
			player.refill(0.5f);
		}
		float deltaTime = Time.deltaTime;
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
}
