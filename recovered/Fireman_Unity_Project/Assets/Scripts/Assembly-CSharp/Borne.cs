using System;
using UnityEngine;

public class Borne : MonoBehaviour
{
	public UISprite sprite;

	private float nextRefill;

	public void setBorne(Vector3 p)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = Vector3.one;
		base.transform.position = p;
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Transform transform = base.transform;
		Transform transform2 = GameManager.mainCamera.transform;
		if (Mathf.Abs(transform.position.x - transform2.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(transform.position.y - transform2.position.y) > (float)GameManager.tailleZoneY)
		{
			sprite.enabled = false;
			return;
		}
		sprite.enabled = true;
		if (nextRefill > 0f)
		{
			nextRefill -= Time.deltaTime;
			if (nextRefill < 0f)
			{
				nextRefill = 0f;
			}
		}
		ListOf<Pompier> pompiers = GameManager.pompiers;
		for (int i = 0; i < pompiers.Count; i++)
		{
			Pompier pompier = pompiers[i];
			if (Common.Absi(transform.localPosition.x - pompier.transform.localPosition.x) >= 40 || Common.Absi(transform.localPosition.y - pompier.transform.localPosition.y) >= 40)
			{
				continue;
			}
			if ((nextRefill <= 0f && !pompier.IsFullWater()) || pompier.setLastBorne(base.transform.localPosition))
			{
				nextRefill = 1f;
				Sounds.snd_ig_checkpoint.Play();
				for (int j = 0; j < 10; j++)
				{
					int num = 45 + j * 9;
					float num2 = UnityEngine.Random.value * 0.3f + 0.7f;
					GameManager.newGoutte(transform.localPosition.x, transform.localPosition.y, Mathf.Cos((float)Math.PI / 180f * (float)num) * 1f * num2, Mathf.Sin((float)Math.PI / 180f * (float)num) * 2.5f * num2, 1f);
				}
			}
			pompier.refill();
		}
	}
}
