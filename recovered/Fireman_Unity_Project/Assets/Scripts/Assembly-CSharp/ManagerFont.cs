using System.Collections.Generic;
using UnityEngine;

public class ManagerFont : MonoBehaviour
{
	public List<UIFont> listFontRef;

	public List<FontReplace> listFontReplacement;

	private void Awake()
	{
		for (int i = 0; i < listFontReplacement.Count; i++)
		{
			if (i == listFontReplacement.Count - 1)
			{
				for (int j = 0; j < listFontRef.Count; j++)
				{
					listFontRef[j].replacement = listFontReplacement[i].listFont[j];
				}
			}
			else if (listFontReplacement[i].lng == TextManager.Lang)
			{
				for (int k = 0; k < listFontRef.Count; k++)
				{
					listFontRef[k].replacement = listFontReplacement[i].listFont[k];
				}
				break;
			}
		}
	}
}
