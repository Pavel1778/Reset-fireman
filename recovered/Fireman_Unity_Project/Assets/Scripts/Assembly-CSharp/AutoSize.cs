using UnityEngine;

public class AutoSize : MonoBehaviour
{
	public UIRoot uiroot;

	public void Awake()
	{
	}

	public void Update()
	{
		Vector3 localScale = uiroot.transform.localScale;
		float num = (float)uiroot.manualHeight / (float)Screen.height * (float)Screen.width;
		base.transform.localScale = new Vector3(localScale.x * num, localScale.y * (float)uiroot.manualHeight, localScale.z);
	}
}
