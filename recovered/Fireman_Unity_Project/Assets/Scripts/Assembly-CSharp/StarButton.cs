using UnityEngine;

public class StarButton : MonoBehaviour
{
	public void OnMouseDown()
	{
		Ask4RateScreen.Instance.OnStarClick(this);
	}
}
