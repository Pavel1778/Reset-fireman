using UnityEngine;

public class Goutte : BaseSpawnable
{
	private Bounds bounds;

	private float life;

	public Sprite spriteGoutte;

	public Vector3 vel;

	public bool hit;

	public void setGoutte(Vector3 pos, Vector2 v, float life)
	{
		spriteGoutte = base.gameObject.GetComponent<Sprite>();
		spriteGoutte.Atlas = GameManager.ATLAS[VarsManager.Instance.worldToLoad - 1];
		spriteGoutte.Alpha = 1f - Random.value * 0.4f;
		spriteGoutte.ForcedWidth = 22f;
		spriteGoutte.ForcedHeight = 11f;
		vel = v;
		float num = 1f + Random.value * 1f;
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = new Vector3(num, num, 1f);
		base.transform.localPosition = new Vector3(pos.x, pos.y, -800f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(v.y, v.x));
		this.life = life;
		hit = false;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		bounds = new Bounds(Vector3.zero, new Vector3(8f * num * localToWorldMatrix.m00, 8f * num * localToWorldMatrix.m11, 50f * localToWorldMatrix.m22));
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		float deltaTime = Time.deltaTime;
		vel.x += Physics.gravity.x * deltaTime;
		vel.y += Physics.gravity.y * deltaTime;
		localPosition.x += vel.x * GameManager.uiScaleX * deltaTime;
		localPosition.y += vel.y * GameManager.uiScaleY * deltaTime;
		base.transform.localPosition = localPosition;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(vel.y, vel.x));
		Transform transform = GameManager.mainCamera.transform;
		Vector3 position = base.transform.position;
		Vector3 vector = position - transform.position;
		int tailleZoneX = GameManager.tailleZoneX;
		int tailleZoneY = GameManager.tailleZoneY;
		if (vector.x < (float)(-tailleZoneX) || vector.y < (float)(-tailleZoneY) || vector.x > (float)tailleZoneX || vector.y > (float)tailleZoneY)
		{
			base.Deleted = true;
			return;
		}
		spriteGoutte.Alpha = 0.9f * life;
		if (!hit)
		{
			ListOf<BaseEntity> activeEnnemies = GameManager.activeEnnemies;
			for (int i = 0; i < activeEnnemies.Count; i++)
			{
				BaseEntity baseEntity = activeEnnemies[i];
				Vector3 localPosition2 = baseEntity.gameObject.transform.localPosition;
				if (Common.Absi(localPosition.x - localPosition2.x) < baseEntity.ColX + 8 && Common.Absi(localPosition.y - localPosition2.y) < baseEntity.ColY + 8)
				{
					baseEntity.OnGoutteHit(this);
					return;
				}
			}
		}
		Vector3 center = position;
		center.z = 0f;
		bounds.center = center;
		if (hit)
		{
			return;
		}
		ListOf<BlocSwitch> switches = GameManager.switches;
		for (int j = 0; j < switches.Count; j++)
		{
			BlocSwitch blocSwitch = switches[j];
			BoxCollider mainCollider = blocSwitch.mainCollider;
			if (bounds.Intersects(mainCollider.bounds))
			{
				blocSwitch.OnGoutteHit(this);
				break;
			}
		}
	}

	public void ruisselle()
	{
		if (!hit)
		{
			float num = Random.value * 0.5f;
			base.transform.localScale = new Vector3(base.transform.localScale.x - num, base.transform.localScale.y - num, 1f);
			vel = new Vector2(vel.x / 8f, vel.y / 2f - 40f);
			life = 0.99f;
			hit = true;
		}
		else
		{
			vel = new Vector2(vel.x / 8f, vel.y / 2f - 40f);
		}
		base.Deleted = true;
	}
}
