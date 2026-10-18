using UnityEngine;

public class UILabelMMUSIA : MonoBehaviour
{
	private void Start()
	{
		GetComponent<UILabel>().text = MMUSIAStrings.Instance.getWord(GetComponent<UILabel>().text);
	}
}
