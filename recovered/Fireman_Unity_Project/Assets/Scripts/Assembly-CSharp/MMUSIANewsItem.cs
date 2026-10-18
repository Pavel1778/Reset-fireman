using UnityEngine;

public class MMUSIANewsItem : MonoBehaviour
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
		mmusia.Instance.newsDownload(this);
	}
}
