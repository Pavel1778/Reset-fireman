using UnityEngine;

public class BaseEntity : MonoBehaviour
{
	public bool Culled;

	public int ColX = 25;

	public int ColY = 25;

	public int TagID;

	public virtual int BaseType
	{
		get
		{
			return 0;
		}
	}

	public bool Cull()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = GameManager.mainCamera.transform.position;
		if (Common.Absf(position.x - position2.x) > (float)GameManager.tailleZoneX || Common.Absf(position.y - position2.y) > (float)GameManager.tailleZoneY)
		{
			Culled = true;
		}
		else
		{
			Culled = false;
		}
		return Culled;
	}

	public virtual void OnGoutteHit(Goutte e)
	{
	}

	public virtual void OnActivate()
	{
	}

	public virtual void Die()
	{
	}
}
