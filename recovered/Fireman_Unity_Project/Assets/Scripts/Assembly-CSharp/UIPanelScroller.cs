using UnityEngine;

public class UIPanelScroller : MonoBehaviour
{
	public GameObject child;

	private bool hook;

	private float lastY;

	public void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (hook)
			{
				float y = Input.mousePosition.y;
				child.transform.localPosition += new Vector3(0f, y - lastY, 0f);
				float num = NGUIMath.CalculateRelativeWidgetBounds(child.transform).size.y * child.transform.localScale.y;
				if (child.transform.localPosition.y > -250f + num)
				{
					child.transform.localPosition = new Vector3(child.transform.localPosition.x, -250f + num, child.transform.localPosition.z);
				}
				if (child.transform.localPosition.y < 250f)
				{
					child.transform.localPosition = new Vector3(child.transform.localPosition.x, 250f, child.transform.localPosition.z);
				}
				lastY = y;
			}
			else
			{
				Camera component = GameObject.Find("Camera").GetComponent<Camera>();
				Ray ray = component.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject == base.gameObject)
				{
					lastY = Input.mousePosition.y;
					hook = true;
				}
			}
		}
		else
		{
			hook = false;
		}
	}
}
