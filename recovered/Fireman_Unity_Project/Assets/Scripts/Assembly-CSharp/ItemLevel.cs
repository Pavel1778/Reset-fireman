using UnityEngine;

public class ItemLevel : MonoBehaviour
{
	private UIButtonMessage message;

	public UISprite spriteStar1;

	public UISprite spriteStar2;

	public UISprite spriteStar3;

	public UISprite background;

	public UILabel label;

	public int idWorld;

	public int idLevel;

	public float alpha;

	public float scale;

	public bool locked;

	public void setItemLevel(int idWorld, int idLevel, int progression)
	{
		locked = false;
		GetComponent<Collider>().enabled = true;
		label.gameObject.SetActiveRecursively(true);
		background.gameObject.SetActiveRecursively(true);
		spriteStar1.gameObject.SetActiveRecursively(true);
		spriteStar2.gameObject.SetActiveRecursively(true);
		spriteStar3.gameObject.SetActiveRecursively(true);
		if (message == null)
		{
			message = GetComponent<UIButtonMessage>();
		}
		message.target = MenusManager.Instance.gameObject;
		string stars = VarsManager.Instance.getStars(VarsManager.GameMode, idWorld, idLevel);
		if (stars[0] == '1')
		{
			spriteStar1.spriteName = "PetHead01";
		}
		else
		{
			spriteStar1.spriteName = "pititeBouBouleB";
		}
		if (stars[1] == '1')
		{
			spriteStar2.spriteName = "PetHead02";
		}
		else
		{
			spriteStar2.spriteName = "pititeBouBouleB";
		}
		if (stars[2] == '1')
		{
			spriteStar3.spriteName = "PetHead03";
		}
		else
		{
			spriteStar3.spriteName = "pititeBouBouleB";
		}
		if (idLevel <= 3 && idWorld == 1)
		{
			if (stars[0] == '1')
			{
				spriteStar2.spriteName = "PetHead01";
			}
			else
			{
				spriteStar2.spriteName = "pititeBouBouleB";
			}
			spriteStar1.gameObject.SetActiveRecursively(false);
			spriteStar3.gameObject.SetActiveRecursively(false);
		}
		this.idWorld = idWorld;
		this.idLevel = idLevel;
		label.text = string.Empty + idLevel;
		background.spriteName = "LevelSelector_02_Post-it" + (int)(1f + Random.value * 3f);
		if ((progression == -1 || progression < idLevel) && progression != 0 && idLevel <= 18)
		{
			background.spriteName = "LevelSelector_02_Post-it0";
			locked = true;
		}
		scale = 2f;
		alpha = 0f;
		if (idLevel > 18 && ((idLevel < 24 && !VarsManager.Instance.getObjetCache(idWorld, idLevel - 19) && VarsManager.Instance.getObjetShopStep("extralevel" + idWorld) != 1) || (idLevel == 24 && VarsManager.Instance.getObjetShopStep("extralevel" + idWorld) != 1)))
		{
			GetComponent<Collider>().enabled = false;
			label.gameObject.SetActiveRecursively(false);
			background.gameObject.SetActiveRecursively(false);
			spriteStar1.gameObject.SetActiveRecursively(false);
			spriteStar2.gameObject.SetActiveRecursively(false);
			spriteStar3.gameObject.SetActiveRecursively(false);
		}
	}
}
