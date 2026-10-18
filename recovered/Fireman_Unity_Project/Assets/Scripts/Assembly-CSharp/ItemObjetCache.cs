using UnityEngine;

[ExecuteInEditMode]
public class ItemObjetCache : MonoBehaviour
{
	public int idWorld;

	public int idObjet;

	private int lidWorld = -1;

	private int lidObjet = -1;

	private UISprite sprite;

	public void Update()
	{
		if (idObjet != lidObjet || idWorld != lidWorld)
		{
			lidObjet = idObjet;
			lidWorld = idWorld;
			sprite = GetComponent<UISprite>();
			sprite.spriteName = "objetCache_" + idWorld + "_" + idObjet;
			if (Application.isPlaying && !VarsManager.Instance.getObjetCache(idWorld, idObjet))
			{
				sprite.color = new Color(0f, 0f, 0f, 1f);
			}
		}
	}
}
