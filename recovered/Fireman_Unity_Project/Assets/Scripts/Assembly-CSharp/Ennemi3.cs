using UnityEngine;

public class Ennemi3 : BaseEntity
{
	private float timeSpawnFeu;

	public UISprite spriteFleche;

	private float flip;

	public GameObject mAnim;

	private float life;

	private float timeMort;

	private bool dead;

	private float startX;

	public float moveX;

	private SkeletonAnimation spine;

	private float timeRallume;

	private float timeBlinkRallume;

	public void setEnnemi(Vector3 pos, float mX)
	{
		startX = base.transform.localPosition.x;
		moveX = mX;
		flip = 1f;
		life = 1f;
		spine = mAnim.GetComponent<SkeletonAnimation>();
		spine.skeletonDataAsset = SpineData.Instance.dataEnnemi3;
		spine.Initialize();
		spine.state.SetAnimation("walk", true);
		spriteFleche.gameObject.SetActiveRecursively(false);
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			return;
		}
		if (moveX == 0f)
		{
			moveX = 2f;
		}
		GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		Vector3 position = GameManager.mainCamera.transform.position;
		if (Mathf.Abs(base.transform.position.x - position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - position.y) > (float)GameManager.tailleZoneY)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
			return;
		}
		mAnim.GetComponent<Renderer>().enabled = true;
		timeSpawnFeu -= Time.deltaTime;
		if (timeSpawnFeu <= 0f && life > 0f && !dead)
		{
			bool flag = false;
			float num = Common.RoundTo(base.transform.localPosition.x - 2f, 50f);
			float num2 = Common.RoundTo(base.transform.localPosition.y + 2f, 50f);
			ListOf<BaseEntity> ennemies = GameManager.ennemies;
			for (int i = 0; i < ennemies.Count; i++)
			{
				BaseEntity baseEntity = ennemies[i];
				if (baseEntity != null && baseEntity.BaseType == 1)
				{
					Vector3 localPosition = baseEntity.transform.localPosition;
					if (Common.Absi(localPosition.x - num) < 50 && Common.Absi(localPosition.y - num2) < 50)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				timeSpawnFeu = 0.1f;
				GameObject gameObject = Fire.create().gameObject;
				gameObject.transform.localPosition = new Vector3(num, num2, 0f);
				Ennemi1 component = gameObject.GetComponent<Ennemi1>();
				component.giveNothing = true;
				GameManager.AddEnnemie(component);
				Sounds.snd_mob3_detect.Play();
			}
		}
		if (moveX > 0f)
		{
			if (base.transform.localPosition.x > startX + moveX * (float)GameManager.Instance.tailleBlocs)
			{
				flip = -1f;
			}
			if (base.transform.localPosition.x < startX)
			{
				flip = 1f;
			}
		}
		else if (moveX < 0f)
		{
			if (base.transform.localPosition.x < startX + moveX * (float)GameManager.Instance.tailleBlocs)
			{
				flip = 1f;
			}
			if (base.transform.localPosition.x > startX)
			{
				flip = -1f;
			}
		}
		if (timeMort == 0f)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(GameManager.Instance.ratioEnnemiSpeed * 0.5f * flip, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		mAnim.transform.localScale = new Vector3(1f * (0f - flip), 1f, 1f);
		Pompier player = GameManager.Instance.Player1;
		Vector3 localPosition2 = player.transform.localPosition;
		if (Mathf.Abs(base.transform.localPosition.x - localPosition2.x) < 50f && Mathf.Abs(base.transform.localPosition.y - localPosition2.y) < 50f && life > 0f)
		{
			player.laMort();
		}
		if (Mathf.Abs(base.transform.localPosition.x - localPosition2.x) < 50f && Mathf.Abs(base.transform.localPosition.y - localPosition2.y) < 30f && life <= 0f && player.GetComponent<Rigidbody>().velocity.y < -0.2f && timeMort == 0f)
		{
			spine.state.SetAnimation("ecraser", false);
			Sounds.snd_mob3_kill.Play();
			spriteFleche.gameObject.SetActiveRecursively(false);
			if (timeMort == 0f)
			{
				timeMort = 0.01f;
			}
			player.GetComponent<Rigidbody>().velocity = new Vector3(player.GetComponent<Rigidbody>().velocity.x, 2f, 0f);
		}
		if (timeMort > 0f)
		{
			timeMort += Time.deltaTime;
		}
		if (!dead && timeMort >= 1f)
		{
			Die();
		}
		timeBlinkRallume -= 15f * Time.deltaTime;
		while (timeBlinkRallume < -1f)
		{
			timeBlinkRallume = 1f;
		}
		if (dead || !(life <= 0f) || timeMort != 0f)
		{
			return;
		}
		timeRallume += Time.deltaTime;
		if (timeRallume > 7f)
		{
			spine.state.SetAnimation("walk", true);
			spriteFleche.gameObject.SetActiveRecursively(false);
			life = 1f;
			timeRallume = 0f;
		}
		if (!(timeRallume > 6f) || !(timeRallume < 7f))
		{
			return;
		}
		if (timeBlinkRallume > 0f)
		{
			if (!spine.state.Animation.Name.Equals("walk"))
			{
				spine.state.SetAnimation("walk", true);
			}
		}
		else if (!spine.state.Animation.Name.Equals("walk-eteint"))
		{
			spine.state.SetAnimation("walk-eteint", true);
		}
	}

	public override void Die()
	{
		GameManager.GiveBonus(base.transform.position, Config.bonuses.crawler, false);
		GameManager.Instance.slimeExtinguish++;
		Object.Destroy(base.gameObject);
		dead = true;
	}

	public override void OnGoutteHit(Goutte goutte)
	{
		if (life > 0f)
		{
			life -= 0.1f;
			if (life <= 0f)
			{
				life = 0f;
				spine.state.SetAnimation("walk-eteint", true);
				spriteFleche.gameObject.SetActiveRecursively(true);
			}
		}
		if (goutte.vel.x > 0f)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(-1.5f, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		goutte.ruisselle();
	}
}
