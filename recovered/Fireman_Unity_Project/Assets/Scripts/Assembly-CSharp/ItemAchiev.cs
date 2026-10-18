using UnityEngine;

public class ItemAchiev : MonoBehaviour
{
	public UISprite spriteIcone;

	public UILabel labelTitre;

	public UILabel labelDescription;

	public UILabel labelPourcentage;

	public int id;

	public void setItemAchiev(int i)
	{
		if (i != -1)
		{
			id = i;
		}
		spriteIcone.spriteName = VarsManager.Instance.listAchievs[id].sprite_name;
		labelTitre.text = TextManager.GetText("res_achiev" + VarsManager.Instance.listAchievs[id].id_title + "_title");
		labelDescription.text = TextManager.GetText("res_achiev" + VarsManager.Instance.listAchievs[id].id_description + "_desc");
		float num = (float)VarsManager.Instance.listAchievs[id].current_step / (float)VarsManager.Instance.listAchievs[id].max_steps;
		if (num > 1f)
		{
			num = 1f;
		}
		labelPourcentage.text = (int)(num * 100f) + "%";
		if (num == 0f)
		{
			labelPourcentage.alpha = 0f;
		}
		else
		{
			labelPourcentage.alpha = 1f;
		}
		if (num != 1f)
		{
			spriteIcone.spriteName = "AchievementGris";
			labelTitre.color = new Color(0.85f, 0.85f, 0.85f, 1f);
			labelDescription.color = new Color(0.8f, 0.8f, 0.8f, 1f);
		}
		else
		{
			labelTitre.color = new Color(1f, 1f, 1f, 1f);
			labelDescription.color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
