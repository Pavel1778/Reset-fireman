public class Pets
{
	private static SkeletonDataAsset[] All
	{
		get
		{
			return new SkeletonDataAsset[8]
			{
				SpineData.Instance.dataPet1,
				SpineData.Instance.dataPet2,
				SpineData.Instance.dataPet3,
				SpineData.Instance.dataPet4,
				SpineData.Instance.dataPet5,
				SpineData.Instance.dataPet6,
				SpineData.Instance.dataPet7,
				SpineData.Instance.dataPet8
			};
		}
	}

	public static void Setup(SkeletonAnimation anim, int index)
	{
		if (!(anim == null) && index >= 0 && index < All.Length)
		{
			anim.skeletonDataAsset = All[index];
			anim.Initialize();
			anim.skeleton.SetSlotsToSetupPose();
			anim.skeleton.SetToSetupPose();
		}
	}
}
