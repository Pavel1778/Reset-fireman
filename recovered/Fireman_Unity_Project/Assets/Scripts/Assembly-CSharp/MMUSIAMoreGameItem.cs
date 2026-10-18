using UnityEngine;

public class MMUSIAMoreGameItem : MonoBehaviour
{
	public UILabel title;

	public UILabel descr;

	public UILabel date;

	public UITexture icone;

	[HideInInspector]
	public int id;

	[HideInInspector]
	public string url;

	[HideInInspector]
	public string pname;

	private void OnClick()
	{
		mmusia.Instance.moreGamesDownload(this);
	}
}
