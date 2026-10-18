using UnityEngine;

public class BubbleBlastBubble : Sprite
{
	public float velX;

	public float velY;

	public float life;

	public float angle;

	public override void Awake()
	{
		base.Awake();
		base.Atlas = GameManager.Instance.AtlasFire;
		base.SpriteName = "bubbleblast";
		base.ForcedHeight = 25f;
		base.ForcedWidth = 25f;
		life = 3f;
	}

	public override void Update()
	{
		life -= Time.deltaTime;
		if (life > 0f)
		{
			base.Alpha = life / 3f;
			angle += 30f * Time.deltaTime;
			base.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
			base.transform.localPosition = new Vector3(base.transform.localPosition.x + velX * Time.deltaTime, base.transform.localPosition.y + velY * Time.deltaTime, base.transform.localPosition.z);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		if (_update)
		{
			Apply();
		}
	}
}
