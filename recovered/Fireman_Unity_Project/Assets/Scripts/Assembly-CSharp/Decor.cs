using UnityEngine;

public class Decor : MonoBehaviour
{
	public bool mClipped = true;

	public int tagID;

	public float relX;

	public float relY;

	public UISprite sprite;

	public bool Clipped
	{
		get
		{
			return mClipped;
		}
		set
		{
			if (mClipped != value)
			{
				mClipped = value;
				sprite.enabled = value;
				base.gameObject.SetActive(value);
				sprite.gameObject.SetActive(value);
			}
		}
	}

	public void SetData(int pLayer, int pAngle, int pHFlip, int pVFlip, float pOx, float pOy, float pOz)
	{
		Vector3 localPosition = base.transform.localPosition;
		sprite.atlas = GameManager.getAtlas(pLayer);
		sprite.spriteName = "decor" + pLayer;
		sprite.depth = -(int)(pOz * 100f);
		base.transform.localPosition = new Vector3(localPosition.x + pOx, localPosition.y - pOy, localPosition.z + pOz);
		if (pAngle != 0 || pHFlip != 0 || pVFlip != 0)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(pVFlip, pHFlip, -pAngle));
		}
		relX = base.transform.position.x;
		relY = base.transform.position.y;
	}

	public void SetAngleAndFlip(int pAngle, int pHFlip, int pVFlip)
	{
		if (pAngle != 0 || pHFlip != 0 || pVFlip != 0)
		{
			base.transform.localRotation = Quaternion.Euler(new Vector3(pVFlip, pHFlip, -pAngle));
		}
	}

	public void SetLayer(int pLayer)
	{
		sprite.atlas = GameManager.getAtlas(pLayer);
		sprite.spriteName = "decor" + pLayer;
	}
}
