using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static SoundManager ins;

	private SoundManager()
	{
	}

	private void Awake()
	{
		if (ins != null && ins != this)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject);
		}
		else
		{
			ins = this;
			Object.DontDestroyOnLoad(this);
			AudioPlayerEx.Initialize(base.gameObject);
		}
	}

	private void FixedUpdate()
	{
		AudioPlayerEx.FixedUpdate();
	}
}
