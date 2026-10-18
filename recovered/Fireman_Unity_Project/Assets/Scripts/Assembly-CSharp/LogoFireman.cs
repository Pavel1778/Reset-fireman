using UnityEngine;

public class LogoFireman : MonoBehaviour
{
	private const int COUNT1 = 8;

	private const int COUNT2 = 12;

	private const float WAIT = 5f;

	private int mCount;

	private float mTime;

	public void Start()
	{
		mTime = 0f;
		mCount = 0;
	}

	public void OnEnable()
	{
		mTime = 0f;
		mCount = 0;
	}

	public void OnDisable()
	{
		mTime = 0f;
		mCount = 0;
	}

	public void Update()
	{
		if (mCount == 8)
		{
			mTime += Time.deltaTime;
			if (mTime >= 5f)
			{
				if (VarsManager.GameMode == 1)
				{
					VarsManager.GameMode = 2;
				}
				else if (VarsManager.GameMode == 2)
				{
					VarsManager.GameMode = 4;
				}
				else if (VarsManager.GameMode == 4)
				{
					VarsManager.GameMode = 1;
				}
				Sounds.snd_mob2_kill.Play();
				MenusManager.Instance.ShowBando("LevelSelector_02_Post-it0", "Yeah Baby !", true);
				MenusManager.Instance.SetGameModeLogo();
				mCount++;
			}
		}
		else if (mCount == 12)
		{
			mTime += Time.deltaTime;
			if (mTime >= 5f)
			{
				VarsManager.Instance.masterOfTheKeys = true;
				Sounds.snd_mob2_kill.Play();
				MenusManager.Instance.ShowBando("LevelSelector_02_Post-it0", "Easy Baby !", true);
				mCount++;
			}
		}
	}

	public void OnClick()
	{
		Sounds.snd_ig_teleport.Play();
		mTime = 0f;
		mCount++;
	}
}
