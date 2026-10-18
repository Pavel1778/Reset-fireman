using UnityEngine;

public class PointeurDude : MonoBehaviour
{
	public Camera cam;

	public GameObject cible;

	public UISprite sprite;

	public float alpha;

	private int threshold;

	private float minDist;

	private bool near;

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		threshold = (threshold + 1) % 4;
		if (threshold != 0)
		{
			return;
		}
		cible = null;
		near = false;
		minDist = float.MaxValue;
		Transform p = GameManager.instance.Player1.transform;
		IsNear(p, GameManager.Instance.mDudeA, 0);
		IsNear(p, GameManager.Instance.mDudeB, 1);
		IsNear(p, GameManager.Instance.mDudeC, 2);
		if (cible != null)
		{
			Vector3 vector = GameManager.mainCamera.WorldToScreenPoint(cible.transform.position);
			float num = vector.x;
			float num2 = vector.y;
			if (num < (float)cam.pixelWidth * 0.05f)
			{
				num = (float)cam.pixelWidth * 0.05f;
			}
			if (num > (float)cam.pixelWidth * 0.95f)
			{
				num = (float)cam.pixelWidth * 0.95f;
			}
			if (num2 < (float)cam.pixelHeight * 0.05f)
			{
				num2 = (float)cam.pixelHeight * 0.05f;
			}
			if (num2 > (float)cam.pixelHeight * 0.95f)
			{
				num2 = (float)cam.pixelHeight * 0.95f;
			}
			vector = new Vector3(num, num2, 0f);
			base.transform.position = cam.ScreenToWorldPoint(vector);
			sprite.transform.rotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(cible.transform.position.y - GameManager.Instance.Player1.transform.position.y, cible.transform.position.x - GameManager.Instance.Player1.transform.position.x));
			if (near)
			{
				alpha = 0f;
			}
			else
			{
				alpha += 2f * Time.deltaTime;
				if (alpha > 1f)
				{
					alpha = 1f;
				}
			}
		}
		else
		{
			alpha -= 2f * Time.deltaTime;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
		sprite.alpha = alpha;
	}

	private void IsNear(Transform p, GameObject c, int i)
	{
		if (c == null)
		{
			return;
		}
		Transform transform = c.transform;
		float num = p.localPosition.x - transform.localPosition.x;
		float num2 = p.localPosition.y - transform.localPosition.y;
		float num3 = num * num + num2 * num2;
		if (num3 < minDist)
		{
			minDist = num3;
			cible = transform.gameObject;
			near = Common.Absi(num2) < 350 && Common.Absi(num) < 600;
			switch (i)
			{
			case 0:
				sprite.spriteName = "Arrow_pet1";
				break;
			case 1:
				sprite.spriteName = "Arrow_pet2";
				break;
			case 2:
				sprite.spriteName = "Arrow_pet3";
				break;
			}
		}
	}
}
