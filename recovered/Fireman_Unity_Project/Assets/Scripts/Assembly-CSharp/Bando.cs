using System;
using System.Collections.Generic;
using UnityEngine;

public class Bando : MonoBehaviour
{
	[Serializable]
	public class Affichage
	{
		public string icon;

		public string texte;

		public bool invert;

		public float time;

		public Affichage(string icon, string texte, bool invert)
		{
			this.icon = icon;
			this.texte = texte;
			this.invert = invert;
			time = 3f;
		}
	}

	public UISprite spriteIco;

	public UISprite spriteFond;

	public UILabel label;

	public List<Affichage> listAffichages;

	public void showBando(string icon, string texte, bool invert)
	{
		if (listAffichages.Count == 0 || !listAffichages[0].texte.Equals(texte))
		{
			listAffichages.Add(new Affichage(icon, texte, invert));
			setAffichage();
		}
	}

	private void setAffichage()
	{
		label.text = listAffichages[0].texte;
		spriteIco.spriteName = listAffichages[0].icon;
		if (listAffichages[0].invert)
		{
			GetComponent<UIAnchor>().side = UIAnchor.Side.TopLeft;
			spriteIco.transform.localPosition = new Vector3(-75f, spriteIco.transform.localPosition.y, spriteIco.transform.localPosition.z);
			spriteFond.transform.localScale = new Vector3(-550f, spriteFond.transform.localScale.y, spriteFond.transform.localScale.z);
			label.pivot = UIWidget.Pivot.Right;
			label.transform.localPosition = new Vector3(-135f, label.transform.localPosition.y, label.transform.localPosition.z);
		}
		else
		{
			GetComponent<UIAnchor>().side = UIAnchor.Side.TopRight;
			spriteIco.transform.localPosition = new Vector3(75f, spriteIco.transform.localPosition.y, spriteIco.transform.localPosition.z);
			spriteFond.transform.localScale = new Vector3(550f, spriteFond.transform.localScale.y, spriteFond.transform.localScale.z);
			label.pivot = UIWidget.Pivot.Left;
			label.transform.localPosition = new Vector3(135f, label.transform.localPosition.y, label.transform.localPosition.z);
		}
	}

	private void Update()
	{
		if (listAffichages.Count <= 0)
		{
			return;
		}
		if (listAffichages[0].time <= 0f)
		{
			if (listAffichages.Count > 1)
			{
				listAffichages.RemoveAt(0);
				setAffichage();
			}
			else if (!listAffichages[0].invert && base.gameObject.GetComponent<UIAnchor>().relativeOffset.x < 0f)
			{
				base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = base.gameObject.GetComponent<UIAnchor>().relativeOffset.x + 1f * Time.deltaTime;
				if (base.gameObject.GetComponent<UIAnchor>().relativeOffset.x > 0f)
				{
					base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = 0f;
					listAffichages.RemoveAt(0);
				}
			}
			else if (listAffichages[0].invert && base.gameObject.GetComponent<UIAnchor>().relativeOffset.x > 0f)
			{
				base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = base.gameObject.GetComponent<UIAnchor>().relativeOffset.x - 1f * Time.deltaTime;
				if (base.gameObject.GetComponent<UIAnchor>().relativeOffset.x < 0f)
				{
					base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = 0f;
					listAffichages.RemoveAt(0);
				}
			}
		}
		else
		{
			if (!(listAffichages[0].time > 0f))
			{
				return;
			}
			listAffichages[0].time -= Time.deltaTime;
			if (!listAffichages[0].invert && base.gameObject.GetComponent<UIAnchor>().relativeOffset.x > -0.4f)
			{
				base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = base.gameObject.GetComponent<UIAnchor>().relativeOffset.x - 1f * Time.deltaTime;
				if (base.gameObject.GetComponent<UIAnchor>().relativeOffset.x < -0.4f)
				{
					base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = -0.4f;
				}
			}
			else if (listAffichages[0].invert && base.gameObject.GetComponent<UIAnchor>().relativeOffset.x < 0.4f)
			{
				base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = base.gameObject.GetComponent<UIAnchor>().relativeOffset.x + 1f * Time.deltaTime;
				if (base.gameObject.GetComponent<UIAnchor>().relativeOffset.x > 0.4f)
				{
					base.gameObject.GetComponent<UIAnchor>().relativeOffset.x = 0.4f;
				}
			}
		}
	}
}
