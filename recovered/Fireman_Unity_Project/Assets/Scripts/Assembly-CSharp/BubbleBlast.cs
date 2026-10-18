using UnityEngine;

public class BubbleBlast : BaseEntity
{
	private FootCollider footCollider;

	private Sprite sprite;

	private float life;

	private int step;

	public void Awake()
	{
		life = 10f;
		base.gameObject.layer = 10;
		sprite = this.New<Sprite>("anim", base.gameObject.transform, new Vector3(0f, 0f, -710f));
		sprite.Atlas = GameManager.Instance.AtlasFire;
		sprite.SpriteName = "bubbleblast";
		sprite.ForcedHeight = 50f;
		sprite.ForcedWidth = 50f;
		BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
		boxCollider.material = GameManager.instance.defaultMaterial;
		boxCollider.size = new Vector3(40f, 40f, 50f);
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.constraints = (RigidbodyConstraints)120;
		rigidbody.useGravity = true;
		rigidbody.drag = 2f;
		rigidbody.angularDrag = 0f;
		GameObject gameObject = new GameObject("FootCollider");
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.layer = 10;
		gameObject.transform.localPosition = new Vector3(0f, -16f, 0f);
		boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.material = GameManager.instance.defaultMaterial;
		boxCollider.size = new Vector3(10f, 10f, 50f);
		boxCollider.isTrigger = true;
		footCollider = gameObject.AddComponent<FootCollider>();
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		if (Mathf.Abs(base.transform.position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			sprite.enabled = false;
			return;
		}
		sprite.enabled = true;
		if (footCollider.isTouch && life > 0f)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, 2f, 0f);
		}
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		float x = ((!(goutte.vel.x > 0f)) ? (-1.7f) : 1.7f);
		GetComponent<Rigidbody>().AddForce(x, 0f, 0f);
		if (life > 0f)
		{
			life -= 0.1f;
			if (life <= 0f)
			{
				life = 0f;
				Die();
			}
			float num = Common.lerpLinear(1.3f, 1f, life / 10f);
			base.transform.localScale = new Vector3(num, num, 1f);
		}
		goutte.ruisselle();
	}

	private void MakeBubble(float pX, float pY)
	{
		GameObject gameObject = new GameObject("BBB");
		BubbleBlastBubble bubbleBlastBubble = gameObject.AddComponent<BubbleBlastBubble>();
		gameObject.transform.parent = GameManager.instance.panelGame.transform;
		gameObject.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, -710f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		bubbleBlastBubble.angle = Random.Range(0f, 360f);
		bubbleBlastBubble.velX = pX * 200f;
		bubbleBlastBubble.velY = pY * 200f;
	}

	private void Explode()
	{
		float num = 750f;
		num *= num;
		Transform transform = base.transform;
		ListOf<BaseEntity> ennemies = GameManager.ennemies;
		for (int i = 0; i < ennemies.Count; i++)
		{
			BaseEntity baseEntity = ennemies[i];
			if (baseEntity != null && baseEntity != this)
			{
				Transform transform2 = baseEntity.transform;
				float num2 = transform2.localPosition.x - transform.localPosition.x;
				float num3 = transform2.localPosition.y - transform.localPosition.y;
				if (num2 * num2 + num3 * num3 < num && baseEntity.BaseType == 1)
				{
					((Ennemi1)baseEntity).Extinguish();
				}
			}
		}
	}

	public override void Die()
	{
		Sounds.snd_mob2_detect.Play();
		GameManager.GiveBonus(base.transform.position, Config.bonuses.bubbleblast, false);
		Object.Instantiate(GameManager.Instance.prefabPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, -2f), Quaternion.identity);
		MakeBubble(-1f, 0f);
		MakeBubble(1f, 0f);
		MakeBubble(0f, -1f);
		MakeBubble(0f, 1f);
		Explode();
		Object.Destroy(base.gameObject);
		if (TagID > 0)
		{
			VarsManager.setSecret(TagID, false, true);
		}
	}
}
