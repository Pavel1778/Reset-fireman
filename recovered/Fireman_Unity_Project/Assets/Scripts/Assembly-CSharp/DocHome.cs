using UnityEngine;

public class DocHome : MonoBehaviour
{
	private float timeCoucou;

	private SkeletonAnimation anim;

	private void Awake()
	{
		anim = GetComponent<SkeletonAnimation>();
		anim.skeletonDataAsset = SpineData.Instance.dataDoc;
		anim.Initialize();
	}

	private void Start()
	{
		timeCoucou = 0f;
		anim.state.SetAnimation("idle", true);
	}

	private void OnEnable()
	{
		anim.state.SetAnimation("idle", true);
	}

	private void Update()
	{
		if (timeCoucou < 0f)
		{
			anim.state.SetAnimation("coucou", false);
			anim.state.AddAnimation("idle", true);
			timeCoucou = Random.value * 4f;
		}
		if (anim.state.IsComplete())
		{
			timeCoucou -= Time.deltaTime;
		}
	}
}
