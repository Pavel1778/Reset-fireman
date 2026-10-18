using UnityEngine;

public class FootCollider : MonoBehaviour
{
	public bool isTouch;

	public void OnTriggerEnter(Collider c)
	{
		isTouch = true;
	}

	public void OnTriggerStay(Collider c)
	{
		isTouch = true;
	}

	public void OnTriggerExit(Collider c)
	{
		isTouch = false;
	}
}
