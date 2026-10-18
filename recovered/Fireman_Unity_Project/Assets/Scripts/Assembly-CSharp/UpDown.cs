using UnityEngine;

public class UpDown : Decor, ISwitchable
{
	private bool pompierIsOn;

	private float startX;

	private float startY;

	private Tweener moveX;

	private Tweener moveY;

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
			return false;
		}
	}

	public bool IsOFF
	{
		get
		{
			return false;
		}
	}

	public void Setup(float pDeltaX, float pDeltaY, bool pEnabled, bool pLoop, int pTagID)
	{
		Vector3 localPosition = base.transform.localPosition;
		moveX = createValue(localPosition.x, localPosition.x + pDeltaX * (float)GameManager.Instance.tailleBlocs, 70f, pEnabled, pLoop);
		moveY = createValue(localPosition.y, localPosition.y + pDeltaY * (float)GameManager.Instance.tailleBlocs, 70f, pEnabled, pLoop);
		tagID = pTagID;
		startX = localPosition.x;
		startY = localPosition.y;
	}

	private static Tweener createValue(float pFrom, float pTo, float pSpeed, bool pEnabled, bool pLoop)
	{
		if (pFrom == pTo)
		{
			return null;
		}
		return new Tweener(pFrom, pTo, pSpeed, pEnabled, pLoop);
	}

	public void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject == GameManager.instance.Player1.gameObject && (collision.contacts.Length == 0 || collision.contacts[0].normal.y < 0f))
		{
			pompierIsOn = true;
		}
	}

	public void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject == GameManager.instance.Player1.gameObject)
		{
			pompierIsOn = false;
			GameManager.Instance.Player1.upDownMoveX = 0f;
		}
	}

	public bool Toggle()
	{
		return false;
	}

	public bool SwitchON()
	{
		if (moveX != null)
		{
			moveX.Activate();
		}
		if (moveY != null)
		{
			moveY.Activate();
		}
		return false;
	}

	public bool SwitchOFF()
	{
		if (moveX != null)
		{
			moveX.Disable();
		}
		if (moveY != null)
		{
			moveY.Disable();
		}
		return false;
	}

	public void Update()
	{
		if (GameManager.Instance.Status == GameManager.RUNNING)
		{
			float value;
			float upDownMoveX;
			if (moveX != null)
			{
				moveX.Update();
				value = moveX.GetValue();
				upDownMoveX = moveX.GetSens();
			}
			else
			{
				value = startX;
				upDownMoveX = 0f;
			}
			float value2;
			if (moveY != null)
			{
				moveY.Update();
				value2 = moveY.GetValue();
			}
			else
			{
				value2 = startY;
			}
			base.transform.localPosition = new Vector3(value, value2);
			if (pompierIsOn)
			{
				GameManager.Instance.Player1.upDownMoveX = upDownMoveX;
			}
		}
	}
}
