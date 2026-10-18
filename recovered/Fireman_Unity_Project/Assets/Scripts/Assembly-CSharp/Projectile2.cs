using UnityEngine;

public class Projectile2 : MonoBehaviour
{
	private SkeletonAnimation sprite;

	private float angle;

	private float life;

	public Vector2 vel;

	public void Awake()
	{
		sprite = this.New<SkeletonAnimation>("anim", base.gameObject.transform, new Vector3(0f, 0f, -710f));
		sprite.skeletonDataAsset = SpineData.Instance.dataProjectile;
		sprite.Initialize();
		life = 1.5f;
		base.gameObject.layer = 10;
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.x += vel.x * deltaTime;
		localPosition.y += vel.y * deltaTime;
		angle += 500f * deltaTime;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
		base.transform.localPosition = localPosition;
		life -= deltaTime;
		if (life <= 0f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Vector3 localPosition2 = GameManager.Instance.Player1.transform.localPosition;
		if (Common.Absi(localPosition.x - localPosition2.x) < 30 && Common.Absi(localPosition.y - localPosition2.y) < 30)
		{
			GameManager.Instance.Player1.laMort();
			Object.Destroy(base.gameObject);
		}
	}
}
