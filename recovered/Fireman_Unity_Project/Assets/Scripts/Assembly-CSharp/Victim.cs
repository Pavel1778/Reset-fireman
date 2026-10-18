using UnityEngine;

public class Victim : BaseEntity
{
	public static int cptDudes;

	public UISprite sprite;

	public UISprite spriteFleche;

	public int typeDude;

	public bool isVisible;

	public float initScale;

	public float scale;

	private float dist;

	public void setVictim()
	{
		typeDude = cptDudes;
		cptDudes++;
		sprite.spriteName = "Pnj" + (typeDude + 1);
		if (typeDude == 0)
		{
			GameManager.Instance.mDudeA = base.gameObject;
		}
		if (typeDude == 1)
		{
			GameManager.Instance.mDudeB = base.gameObject;
		}
		if (typeDude == 2)
		{
			GameManager.Instance.mDudeC = base.gameObject;
		}
		scale = 1f;
		initScale = sprite.transform.localScale.x;
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		Vector3 position = base.transform.position;
		if (Mathf.Abs(position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			sprite.enabled = false;
			spriteFleche.enabled = false;
			isVisible = false;
			return;
		}
		sprite.enabled = true;
		spriteFleche.enabled = true;
		isVisible = true;
		if (scale > 0f && scale != 1f)
		{
			scale -= 2f * Time.deltaTime;
			if (scale < 0f)
			{
				scale = 0f;
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = GameManager.Instance.Player1.transform.localPosition;
			int num = Common.Absi(localPosition.x - localPosition2.x);
			int num2 = Common.Absi(localPosition.y - localPosition2.y);
			if (num < 40 && num2 < 40)
			{
				Sounds.snd_ig_survivor.Play();
				if (typeDude == 0)
				{
					GameManager.Instance.savedDudeA = true;
				}
				if (typeDude == 1)
				{
					GameManager.Instance.savedDudeB = true;
				}
				if (typeDude == 2)
				{
					GameManager.Instance.savedDudeC = true;
				}
				Object.Instantiate(GameManager.Instance.prefabFlashAnim, new Vector3(position.x, position.y, -0.5f), Quaternion.identity);
				GameManager.GiveBonus(position, Config.bonuses.victim, false);
				GameManager.Instance.updateDudesUI();
				spriteFleche.enabled = false;
				scale = 0.999f;
			}
		}
		sprite.transform.localScale = new Vector3(initScale * scale, initScale * scale, 1f);
	}
}
