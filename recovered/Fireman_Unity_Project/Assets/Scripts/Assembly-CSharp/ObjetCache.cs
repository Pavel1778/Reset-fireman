using UnityEngine;

public class ObjetCache : MonoBehaviour
{
	public UISprite sprite;

	public int idObjet;

	public void setObjetCache(int id)
	{
		idObjet = id;
		sprite.spriteName = "objetCache_" + VarsManager.Instance.worldToLoad + "_" + id;
		if (VarsManager.Instance.getObjetCache(VarsManager.Instance.worldToLoad, id))
		{
			Object.Destroy(base.gameObject);
		}
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
		int num = Common.Absi(base.transform.localPosition.x - GameManager.Instance.Player1.transform.localPosition.x);
		int num2 = Common.Absi(base.transform.localPosition.y - GameManager.Instance.Player1.transform.localPosition.y);
		if ((float)num < 20f && (float)num2 < 20f)
		{
			GameManager.Instance.mBando.GetComponent<Bando>().showBando("Achievement1", TextManager.GetText((VarsManager.Instance.worldToLoad != 1) ? "res_shop_extralevel2" : "res_shop_extralevel1"), true);
			Sounds.snd_ig_hidden.Play();
			GameManager.GiveBonus(base.transform.position, Config.bonuses.relic, false);
			VarsManager.Instance.setObjetCache(VarsManager.Instance.worldToLoad, idObjet);
			Object.Instantiate(GameManager.Instance.prefabRelicAnim, new Vector3(base.transform.position.x, base.transform.position.y, -4f), Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
	}
}
