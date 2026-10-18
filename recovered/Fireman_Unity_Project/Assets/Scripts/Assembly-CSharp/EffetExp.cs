using UnityEngine;

public class EffetExp : BaseSpawnable
{
	public UILabel mLabel;

	private float alpha;

	private bool dead;

	public void setEffetExp(Vector3 pos, string text)
	{
		mLabel.text = text;
		Transform transform = base.transform;
		transform.parent = GameManager.Instance.panelGame.transform;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		transform.position = pos;
		alpha = 2f;
		dead = false;
	}

	public void Start()
	{
		setEffetExp(base.transform.position, mLabel.text);
	}

	public void Update()
	{
		if (dead)
		{
			return;
		}
		Transform transform = base.transform;
		float deltaTime = Time.deltaTime;
		Vector3 position = transform.position;
		position.y += 0.1f * deltaTime;
		transform.position = position;
		if (alpha > 0f)
		{
			alpha -= 2f * deltaTime;
			if (alpha < 0f)
			{
				base.Deleted = true;
				alpha = 0f;
				dead = true;
			}
		}
	}
}
