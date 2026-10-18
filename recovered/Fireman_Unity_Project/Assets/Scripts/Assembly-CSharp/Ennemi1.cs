using UnityEngine;

public class Ennemi1 : BaseEntity
{
	public float life;

	public Fire mAnim;

	public bool giveNothing;

	private float timeReLife;

	private float timeBlinkRallume;

	private bool isBlinked;

	private Ennemi1 top;

	private Ennemi1 bottom;

	public override int BaseType
	{
		get
		{
			return 1;
		}
	}

	public void setEnnemi(Vector3 pos)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localPosition = new Vector3(pos.x, pos.y, -700f + pos.y / 10000f);
		base.transform.localScale = Vector3.one;
		giveNothing = false;
		life = 1f;
		if (Random.value > 0.5f)
		{
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		}
		mAnim.SetFire();
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		if (Cull())
		{
			if (timeReLife > 0f && mAnim.isAshes)
			{
				timeReLife -= Time.deltaTime;
				if (timeReLife <= 0f)
				{
					Fire(false);
				}
			}
			mAnim.GetComponent<Renderer>().enabled = false;
			return;
		}
		mAnim.GetComponent<Renderer>().enabled = true;
		if (mAnim.GetComponent<Renderer>().enabled)
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = GameManager.Instance.Player1.transform.localPosition;
			int num = Common.Absi(localPosition.x - localPosition2.x);
			int num2 = Common.Absi(localPosition.y - localPosition2.y);
			if (num < 40 && num2 < 30 && life > 0f)
			{
				GameManager.Instance.Player1.laMort();
			}
			else if (num < 30 && num2 < 30)
			{
				kill(false);
			}
			if (GameManager.Instance.mTruc != null)
			{
				localPosition2 = GameManager.Instance.mTruc.transform.localPosition;
				num = Common.Absi(localPosition.x - localPosition2.x);
				num2 = Common.Absi(localPosition.y - localPosition2.y);
				if (num < 30 && num2 < 30 && life <= 0f && GameManager.Instance.mTruc.GetComponent<Rigidbody>().velocity.y < -0.01f)
				{
					kill(true);
				}
			}
		}
		if (!(timeReLife > 0f))
		{
			return;
		}
		timeReLife -= Time.deltaTime;
		if (timeReLife <= 0f)
		{
			Fire(true);
		}
		else
		{
			if (!(timeReLife <= 1.5f))
			{
				return;
			}
			timeBlinkRallume -= Time.deltaTime;
			if (!(timeBlinkRallume < 0f))
			{
				return;
			}
			isBlinked = !isBlinked;
			timeBlinkRallume = 0.1f;
			if (isBlinked)
			{
				if (mAnim.isAshes)
				{
					mAnim.SetFire();
				}
			}
			else if (!mAnim.isAshes)
			{
				mAnim.SetAsh();
			}
		}
	}

	private void kill(bool pUsePet)
	{
		if (!giveNothing)
		{
			GameManager.GiveBonus(base.transform.position, Config.bonuses.ashes, pUsePet);
			GameManager.Instance.flamesExtinguish++;
		}
		Sounds.snd_mob1_kill.Play();
		Object.Destroy(base.gameObject);
	}

	public void Fire(bool pSound)
	{
		mAnim.SetFire();
		timeReLife = 0f;
		life = 1f;
	}

	public void Ash(bool pSound)
	{
		if (pSound)
		{
			Sounds.snd_mob1_extinguish.Play();
		}
		mAnim.SetAsh();
		timeReLife = 10f;
		life = 0f;
	}

	public void Extinguish()
	{
		if (!(bottom != null))
		{
			if (top != null)
			{
				top.InnerExtinguish();
			}
			Ash(false);
		}
	}

	private void InnerExtinguish()
	{
		if (top != null)
		{
			top.InnerExtinguish();
		}
		if (bottom != null)
		{
			bottom.top = null;
		}
		Object.Destroy(base.gameObject);
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (top != null)
		{
			top.OnGoutteHit(goutte);
		}
		else
		{
			if (!(timeReLife <= 0f))
			{
				return;
			}
			if (life > 0f)
			{
				life -= 0.2f;
				if (life <= 0f)
				{
					life = 0f;
					if (bottom == null)
					{
						Ash(true);
					}
					else
					{
						bottom.top = null;
						Object.Destroy(base.gameObject);
					}
				}
			}
			goutte.ruisselle();
		}
	}

	public override void OnActivate()
	{
		Transform transform = base.transform;
		float x = transform.localPosition.x;
		float y = transform.localPosition.y;
		ListOf<BaseEntity> ennemies = GameManager.ennemies;
		for (int i = 0; i < ennemies.Count; i++)
		{
			BaseEntity baseEntity = ennemies[i];
			if (!(baseEntity != null) || !(baseEntity != this) || baseEntity.BaseType != 1)
			{
				continue;
			}
			Transform transform2 = baseEntity.transform;
			float x2 = transform2.localPosition.x;
			float y2 = transform2.localPosition.y;
			if (Common.Absf(x - x2) < 10f)
			{
				if (y - y2 > -60f && y - y2 < 0f)
				{
					top = (Ennemi1)baseEntity;
				}
				else if (y - y2 < 60f && y - y2 > 0f)
				{
					bottom = (Ennemi1)baseEntity;
				}
			}
		}
	}
}
