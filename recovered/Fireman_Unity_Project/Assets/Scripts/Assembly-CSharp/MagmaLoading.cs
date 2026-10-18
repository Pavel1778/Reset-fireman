using System.Collections;
using AndroidPlugin;
using UnityEngine;

public class MagmaLoading : MonoBehaviour
{
	public GameObject logoTexture;

	public GameObject backTopTexture;

	public GameObject backBottomTexture;

	public Camera cam;

	private int SceneToLoadIndex = 2;

	private int MainGameSceneIndex = 2;

	private int IntroSceneIndex = 1;

	private int displayIntroSceneAfterXLaunch = 1;

	private AsyncOperation asyncOp;

	private bool startedFadeOut;

	public float DURATION;

	private string tmpScreenFingerPrint = string.Empty;

	private void Awake()
	{
		checkScreenRotation();
	}

	private void Start()
	{
		if (getlaunchCount() >= displayIntroSceneAfterXLaunch)
		{
			SceneToLoadIndex = IntroSceneIndex;
		}
		else
		{
			SceneToLoadIndex = MainGameSceneIndex;
		}
		if (Core.IsGoogleTV() || Core.IsLexibox() || Core.IsAmazon())
		{
			SceneToLoadIndex = MainGameSceneIndex;
		}
		launchCountIncrement();
		StartCoroutine_Auto(startFade(true));
	}

	private void launchCountIncrement()
	{
		PlayerPrefs.SetInt("MMUSIA_PreHomeCount", getlaunchCount() + 1);
		PlayerPrefs.Save();
	}

	private int getlaunchCount()
	{
		return PlayerPrefs.GetInt("MMUSIA_PreHomeCount", 0);
	}

	private void Update()
	{
		checkScreenRotation();
		if (asyncOp != null && asyncOp.isDone && !startedFadeOut)
		{
			startedFadeOut = true;
			StartCoroutine_Auto(startFade(false));
		}
	}

	public IEnumerator startFade(bool fadeIn)
	{
		float timeStart = Time.time;
		while (Time.time < timeStart + DURATION)
		{
			float percent = (Time.time - timeStart) / DURATION;
			backTopTexture.GetComponent<UITexture>().alpha = ((!fadeIn) ? (1f - percent) : percent);
			logoTexture.GetComponent<UITexture>().alpha = ((!fadeIn) ? (1f - percent) : percent);
			backBottomTexture.GetComponent<UITexture>().alpha = ((!fadeIn) ? (1f - percent) : percent);
			yield return null;
		}
		cam.clearFlags = CameraClearFlags.Depth;
		if (fadeIn)
		{
			asyncOp = Application.LoadLevelAdditiveAsync(SceneToLoadIndex);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void checkScreenRotation()
	{
		if (tmpScreenFingerPrint != Screen.width + "/" + Screen.height)
		{
			float pixelSizeAdjustment = GetComponent<UIRoot>().pixelSizeAdjustment;
			float num = Screen.width;
			float num2 = num * pixelSizeAdjustment / 742f * 145f;
			if (Screen.width > Screen.height)
			{
				logoTexture.transform.localScale = new Vector3((int)(num * pixelSizeAdjustment), (int)num2, 10f);
			}
			else
			{
				logoTexture.transform.localScale = new Vector3((int)(num * pixelSizeAdjustment), (int)num2, 10f);
			}
		}
	}
}
