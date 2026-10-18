using UnityEngine;

public class BlocSwitch : Decor, ISwitchable
{
	public BoxCollider mainCollider;

	public float timeToLive;

	public float remainTime;

	public bool initialState;

	public bool switched;

	public int Ident
	{
		get
		{
			return tagID;
		}
	}

	public bool IsON
	{
		get
		{
			return switched;
		}
	}

	public bool IsOFF
	{
		get
		{
			return !switched;
		}
	}

	public void setup(int pAngle, int pHFlip, int pVFlip, float pOx, float pOy, float pOz, int pTagID, int pTtl, bool pEnabled)
	{
		GameObject gameObject = new GameObject("Sprite");
		Transform transform = gameObject.transform;
		transform.parent = base.gameObject.transform;
		transform.localPosition = new Vector3(0f, -8f, 0f);
		transform.localScale = new Vector3(34f, 34f, 1f);
		transform.localRotation = Quaternion.identity;
		gameObject.layer = 8;
		sprite = gameObject.gameObject.AddComponent<UISprite>();
		sprite.atlas = GameManager.Instance.AtlasFire;
		sprite.spriteName = ((!switched) ? "switch_off" : "switch_on");
		mainCollider = base.gameObject.AddComponent<BoxCollider>();
		mainCollider.center = new Vector3(0f, -16f, 0f);
		mainCollider.size = new Vector3(30f, 17f, 50f);
		Vector3 localPosition = base.transform.localPosition;
		base.transform.localPosition = new Vector3(localPosition.x + pOx, localPosition.y - pOy, localPosition.z + pOz);
		if (pAngle != 0 || pHFlip != 0 || pVFlip != 0)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(pVFlip, pHFlip, -pAngle));
		}
		remainTime = 0f;
		tagID = pTagID;
		initialState = switched;
		switched = pEnabled;
		timeToLive = (float)pTtl / 1000f;
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		if (mainCollider.bounds.Intersects(GameManager.Instance.Player1.mainCollider.bounds))
		{
			SwitchON();
		}
		if (!(timeToLive > 0f) || switched == initialState)
		{
			return;
		}
		remainTime -= Time.deltaTime;
		if (remainTime <= 0f)
		{
			if (switched)
			{
				SwitchOFF();
			}
			else
			{
				SwitchON();
			}
		}
	}

	public virtual void OnGoutteHit(Goutte e)
	{
		if (SwitchON())
		{
			e.Deleted = true;
			e.hit = true;
		}
	}

	public bool Toggle()
	{
		if (switched)
		{
			SwitchOFF();
		}
		else
		{
			SwitchON();
		}
		return switched;
	}

	public bool SwitchON()
	{
		if (!switched)
		{
			Sounds.snd_ig_switch.Play();
			remainTime = timeToLive;
			switched = true;
			sprite.spriteName = "switch_on";
			ListOf<ISwitchable> switchables = GameManager.switchables;
			for (int i = 0; i < switchables.Count; i++)
			{
				ISwitchable switchable = switchables[i];
				if (switchable != this && switchable.Ident == tagID)
				{
					switchable.SwitchON();
				}
			}
			return true;
		}
		return false;
	}

	public bool SwitchOFF()
	{
		if (switched)
		{
			Sounds.snd_ig_switch.Play();
			remainTime = timeToLive;
			switched = false;
			sprite.spriteName = "switch_off";
			ListOf<ISwitchable> switchables = GameManager.switchables;
			for (int i = 0; i < switchables.Count; i++)
			{
				ISwitchable switchable = switchables[i];
				if (switchable != this && switchable.Ident == tagID)
				{
					switchable.SwitchOFF();
				}
			}
			return true;
		}
		return false;
	}
}
