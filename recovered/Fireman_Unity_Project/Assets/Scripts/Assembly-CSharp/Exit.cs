using UnityEngine;

public class Exit : MonoBehaviour
{
	public UISprite sprite;

	private float dist;

	public void setSprite()
	{
		if (VarsManager.Instance.worldToLoad == 2)
		{
			sprite.spriteName = "Exit2";
		}
		else if (VarsManager.Instance.worldToLoad == 3)
		{
			sprite.spriteName = "Exit3";
		}
		else
		{
			sprite.spriteName = "Exit1";
		}
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = GameManager.mainCamera.transform.position;
		if (Mathf.Abs(position.x - position2.x) > (float)GameManager.tailleZoneX || Mathf.Abs(position.y - position2.y) > (float)GameManager.tailleZoneY)
		{
			sprite.enabled = false;
			return;
		}
		sprite.enabled = true;
		position = base.transform.localPosition;
		position2 = GameManager.Instance.Player1.transform.localPosition;
		if (Mathf.Abs(position.x - position2.x) < 20f && Mathf.Abs(position.y - position2.y) < 20f)
		{
			GameManager.Instance.finishLevel();
		}
	}
}
