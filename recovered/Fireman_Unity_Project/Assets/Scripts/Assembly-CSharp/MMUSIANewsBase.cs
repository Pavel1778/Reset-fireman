using System.Collections;
using UnityEngine;

public class MMUSIANewsBase : MonoBehaviour
{
	public UIPanel PanelItems;

	public UIPanel PanelUI;

	public GameObject ScrollBarBackGround;

	public GameObject ScrollBarForeGround;

	public GameObject LoadingObj;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			mmusia.Instance.clean(true);
			mmusia.Instance.closeMMUSIAItem();
			mmusia.Instance.broadcastCloseList();
		}
	}

	public void MMUSIA_ResizeMeImFamous()
	{
		if (Screen.width > Screen.height)
		{
			PanelItems.transform.localPosition = new Vector3(0f, 90f, 0f);
			PanelItems.clipRange = new Vector4(0f, -100f, 960f, 300f);
			ScrollBarBackGround.transform.localScale = new Vector3(14f, 300f, 0f);
			ScrollBarForeGround.transform.localScale = new Vector3(16f, 302f, 1f);
		}
		else
		{
			PanelItems.transform.localPosition = new Vector3(0f, 300f, 0f);
			PanelItems.clipRange = new Vector4(0f, -300f, 960f, 700f);
			ScrollBarBackGround.transform.localScale = new Vector3(14f, 600f, 0f);
			ScrollBarForeGround.transform.localScale = new Vector3(16f, 602f, 1f);
		}
	}

	public void MMUSIA_JsonApiLoaded()
	{
		StartCoroutine(reloadMe());
	}

	private IEnumerator reloadMe()
	{
		yield return new WaitForSeconds(0.1f);
		mmusia.Instance.uirootMmusia.DisplayNews();
	}
}
