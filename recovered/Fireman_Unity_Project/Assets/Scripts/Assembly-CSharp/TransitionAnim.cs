using UnityEngine;

public class TransitionAnim : MonoBehaviour
{
	private GUI_Sprite s;

	private float duration = 2f;

	private bool reverse;

	private bool playing;

	private bool dirty = true;

	private float vFactor;

	private float vFrom;

	private float vTo;

	public bool IsPlaying
	{
		get
		{
			return playing;
		}
	}

	public void Start()
	{
		s = GetComponent<GUI_Sprite>();
	}

	public void Update()
	{
		if (dirty)
		{
			base.transform.localScale = new Vector3(vFrom + vFactor * (vTo - vFrom), 750f, 1f);
			dirty = false;
		}
	}

	public void FixedUpdate()
	{
		if (!playing)
		{
			return;
		}
		dirty = true;
		vFactor += Time.deltaTime * duration;
		if (vFactor > 1f)
		{
			if (reverse)
			{
				vFactor = 1f;
				playing = false;
			}
			else
			{
				vFactor = 1f;
				playing = false;
			}
		}
	}

	public void Hide()
	{
		dirty = false;
		playing = false;
		s.transform.localPosition = new Vector3(-550f, 0f, -10f);
		s.transform.localScale = new Vector3(0f, 750f, 1f);
	}

	public void Show()
	{
		dirty = false;
		playing = false;
		s.transform.localPosition = new Vector3(-550f, 0f, -10f);
		s.transform.localScale = new Vector3(-6000f, 750f, 1f);
	}

	public void FadeOut()
	{
		vFrom = 0f;
		vTo = 6000f;
		vFactor = 0f;
		playing = true;
		reverse = false;
		dirty = false;
		s.transform.localPosition = new Vector3(550f, 0f, -10f);
		s.transform.localScale = new Vector3(vFrom, 750f, 1f);
		s.Apply();
		base.gameObject.SetActiveRecursively(true);
	}

	public void FadeIn()
	{
		vFrom = -6000f;
		vTo = 0f;
		vFactor = 0f;
		playing = true;
		reverse = true;
		dirty = false;
		s.transform.localPosition = new Vector3(-550f, 0f, -10f);
		s.transform.localScale = new Vector3(vFrom, 750f, 1f);
		s.Apply();
		base.gameObject.SetActiveRecursively(true);
	}
}
