using System;
using UnityEngine;

public class Projectile : BaseSpawnable
{
	private float gravity = 600f;

	private float life;

	private Vector2 vel;

	public GameObject mAnim;

	private bool destroyMe;

	public void setProjectile(Vector3 pos, float angle, float force)
	{
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localPosition = new Vector3(pos.x, pos.y, -500f);
		base.transform.localScale = Vector3.one;
		vel = new Vector2(Mathf.Cos(angle * ((float)Math.PI / 180f)) * force, Mathf.Sin(angle * ((float)Math.PI / 180f)) * force);
		destroyMe = false;
		life = 3f;
	}

	private void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING || base.Deleted)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y += (0f - gravity) * deltaTime * deltaTime / 2f + vel.y * deltaTime;
		vel.y += (0f - gravity) * deltaTime;
		localPosition.x += vel.x * deltaTime;
		base.transform.localPosition = localPosition;
		base.transform.eulerAngles = new Vector3(0f, 0f, 57.29578f * Mathf.Atan2(vel.y, vel.x));
		life -= Time.deltaTime;
		if (life < 0f)
		{
			base.Deleted = true;
			return;
		}
		if (Mathf.Abs(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x) < 30f && Mathf.Abs(base.transform.localPosition.y - GameManager.Instance.Player1.transform.localPosition.y) < 30f)
		{
			GameManager.Instance.Player1.laMort();
			base.Deleted = true;
		}
		if (destroyMe)
		{
			base.Deleted = true;
		}
	}

	public void OnTriggerEnter(Collider c)
	{
		destroyMe = true;
	}
}
