using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public UISprite sprite;

	public int tpX;

	public int tpY;

	public void setTeleporter(int x, int y)
	{
		tpX = x;
		tpY = y;
	}

	public void setSprite()
	{
		if (VarsManager.Instance.worldToLoad == 2)
		{
			sprite.spriteName = "Teleporter2";
		}
		else if (VarsManager.Instance.worldToLoad == 3)
		{
			sprite.spriteName = "Teleporter3";
		}
		else
		{
			sprite.spriteName = "Teleporter1";
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
		if (Common.Absi(position.x - position2.x) < 20 && Common.Absi(position.y - position2.y) < 20)
		{
			GameManager.Instance.Player1.tpHere(tpX, tpY);
		}
	}
}
