using UnityEngine;

public class ItemWorld : MonoBehaviour
{
	private ItemLevel[] itemLevel;

	public UIGrid uiGridLevel;

	public GameObject prefabItemLevel;

	public int idWorld;

	public void setItemWorld2(int idWorld)
	{
		if (itemLevel == null)
		{
			GetComponent<Collider>().enabled = false;
			itemLevel = new ItemLevel[24];
			for (int i = 0; i < 24; i++)
			{
				GameObject gameObject = Object.Instantiate(prefabItemLevel);
				Transform transform = gameObject.transform;
				transform.parent = uiGridLevel.transform;
				transform.localScale = Vector3.one;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				gameObject.name = idWorld + " - " + i;
				itemLevel[i] = gameObject.GetComponent<ItemLevel>();
			}
			for (int j = 0; j < 24; j++)
			{
				UIButtonKeysExt component = uiGridLevel.transform.GetChild(j).GetComponent<UIButtonKeysExt>();
				if (j == 0)
				{
					component.startsSelected = true;
				}
				if (j < 18)
				{
					component.selectOnDown = uiGridLevel.transform.GetChild(j + 6).GetComponent<UIButtonKeysExt>();
				}
				if (j > 5)
				{
					component.selectOnUp = uiGridLevel.transform.GetChild(j - 6).GetComponent<UIButtonKeysExt>();
				}
				if (j < 23)
				{
					if ((j + 1) % 6 == 0)
					{
						if (idWorld < 3)
						{
							component.selectOnRight = MenusManager.Instance.buttonWorldRight.GetComponent<UIButtonKeysExt>();
						}
					}
					else
					{
						component.selectOnRight = uiGridLevel.transform.GetChild(j + 1).GetComponent<UIButtonKeysExt>();
					}
					if (j % 6 == 0)
					{
						if (idWorld > 1)
						{
							component.selectOnLeft = MenusManager.Instance.buttonWorldLeft.GetComponent<UIButtonKeysExt>();
						}
						component.selectOnRight = uiGridLevel.transform.GetChild(j + 1).GetComponent<UIButtonKeysExt>();
					}
				}
				if (j > 0)
				{
					component.selectOnLeft = uiGridLevel.transform.GetChild(j - 1).GetComponent<UIButtonKeysExt>();
				}
				if (j > 17 && ((j < 23 && !VarsManager.Instance.getObjetCache(idWorld, j - 18) && VarsManager.Instance.getObjetShopStep("extralevel" + idWorld) != 1) || (j == 23 && VarsManager.Instance.getObjetShopStep("extralevel" + idWorld) != 1)))
				{
					component.selectOnUp.GetComponent<UIButtonKeysExt>().selectOnDown = null;
				}
			}
		}
		this.idWorld = idWorld;
		int progression = VarsManager.Instance.getProgression();
		int num = progression / 1000;
		int progression2 = progression - num * 1000;
		if (num < idWorld)
		{
			progression2 = -1;
		}
		else if (num != idWorld)
		{
			progression2 = 0;
		}
		for (int k = 0; k < 24; k++)
		{
			itemLevel[k].setItemLevel(idWorld, k + 1, progression2);
		}
		uiGridLevel.Reposition();
	}
}
