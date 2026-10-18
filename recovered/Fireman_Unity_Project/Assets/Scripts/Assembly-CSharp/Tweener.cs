using UnityEngine;

public class Tweener
{
	private bool vAdd;

	private bool vLoop;

	private bool vMoving;

	private bool vEnabled;

	private float vTarget1;

	private float vTarget2;

	private float vTarget;

	private float vSpeed;

	private float vValue;

	public Tweener(float pFrom, float pTo, float pSpeed, bool pEnabled, bool pLoop)
	{
		if (pEnabled)
		{
			SetTarget(pFrom, pTo);
		}
		else
		{
			vMoving = false;
			vValue = pFrom;
			vTarget1 = pFrom;
			vTarget2 = pTo;
		}
		vEnabled = pEnabled;
		vSpeed = pSpeed;
		vLoop = pLoop;
	}

	public void Activate()
	{
		vEnabled = true;
		if (!vMoving)
		{
			SetTarget(vTarget1, vTarget2);
		}
	}

	public void Disable()
	{
		vEnabled = false;
	}

	public float GetValue()
	{
		return vValue;
	}

	public float GetSens()
	{
		if (vMoving)
		{
			return ((!vAdd) ? (-1f) : 1f) * vSpeed;
		}
		return 0f;
	}

	private void SetTarget(float pFrom, float pTo)
	{
		vTarget1 = pTo;
		vTarget2 = pFrom;
		vTarget = pTo;
		vValue = pFrom;
		vMoving = true;
		if (pFrom < pTo)
		{
			vAdd = true;
		}
		else
		{
			vAdd = false;
		}
	}

	public void Update()
	{
		Update(Time.deltaTime);
	}

	public void Update(float dt)
	{
		if (!vMoving)
		{
			return;
		}
		float num = dt * vSpeed;
		if (vAdd)
		{
			vValue += num;
			if (vValue >= vTarget)
			{
				vValue = vTarget;
				vMoving = false;
				if (vLoop && vEnabled)
				{
					SetTarget(vTarget1, vTarget2);
				}
			}
			return;
		}
		vValue -= num;
		if (vValue <= vTarget)
		{
			vValue = vTarget;
			vMoving = false;
			if (vLoop && vEnabled)
			{
				SetTarget(vTarget1, vTarget2);
			}
		}
	}
}
