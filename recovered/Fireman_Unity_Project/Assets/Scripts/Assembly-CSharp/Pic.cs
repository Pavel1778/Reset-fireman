using UnityEngine;

public class Pic : Decor
{
	public void setPic()
	{
		int num = 60;
		if (VarsManager.Instance.worldToLoad == 2)
		{
			num = 61;
		}
		else if (VarsManager.Instance.worldToLoad == 3)
		{
			num = 80;
		}
		sprite.atlas = GameManager.getAtlas(num);
		sprite.spriteName = "decor" + num;
	}

	public void Update()
	{
		if (GameManager.Instance.Status == GameManager.RUNNING)
		{
			Vector2 vector = base.transform.localPosition;
			Vector2 vector2 = GameManager.Instance.Player1.transform.localPosition;
			if (Common.Absi(vector.x - vector2.x) < 40 && Common.Absi(vector.y - vector2.y) < 40)
			{
				GameManager.Instance.Player1.laMort();
			}
		}
	}
}
