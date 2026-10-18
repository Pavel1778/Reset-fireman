using UnityEngine;

public class KoalaHome : MonoBehaviour
{
	private bool started;

	private UIAnchor anchor;

	private SkeletonAnimation anim;

	public void Awake()
	{
		anchor = GetComponent<UIAnchor>();
		anim = GetComponent<SkeletonAnimation>();
	}

	public void Start()
	{
		started = true;
		anim.state.SetAnimation("down", true);
	}

	public void OnEnable()
	{
		if (started)
		{
			if (anchor.relativeOffset.y == -0.3f)
			{
				anim.state.SetAnimation("idle", true);
			}
			else
			{
				anim.state.SetAnimation("down", true);
			}
		}
	}

	public void Update()
	{
		if (anchor.relativeOffset.y > -0.3f)
		{
			anchor.relativeOffset.y -= 0.08f * Time.deltaTime;
		}
		else if (anchor.relativeOffset.y < -0.3f)
		{
			anim.state.SetAnimation("idle", true);
			anchor.relativeOffset.y = -0.3f;
		}
	}
}
