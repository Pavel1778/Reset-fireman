using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
	public Transform casque;

	private float a;

	public void Start()
	{
		if (VarsManager.Instance.loadGame)
		{
			SceneManager.LoadSceneAsync("ScenePlay3");
		}
		else if (VarsManager.Instance.loadMenu)
		{
			SceneManager.LoadSceneAsync("SceneMenus");
		}
	}

	public void Update()
	{
		a -= 180f * Time.deltaTime;
		if (a < -360f)
		{
			a += 360f;
		}
		casque.localEulerAngles = new Vector3(0f, 0f, a);
	}
}
