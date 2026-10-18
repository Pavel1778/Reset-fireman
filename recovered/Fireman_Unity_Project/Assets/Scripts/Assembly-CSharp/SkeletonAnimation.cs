using Spine;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class SkeletonAnimation : SkeletonComponent
{
	public bool useAnimationName;

	public string animationName;

	public bool loop;

	public Spine.AnimationState state;

	public override void Initialize()
	{
		base.Initialize();
		state = new Spine.AnimationState(skeletonDataAsset.GetAnimationStateData());
	}

	public override void UpdateSkeleton()
	{
		if (useAnimationName)
		{
			if (animationName == null || animationName.Length == 0)
			{
				if (state.Animation != null)
				{
					state.ClearAnimation();
				}
			}
			else if (state.Animation == null || animationName != state.Animation.Name)
			{
				Spine.Animation animation = skeleton.Data.FindAnimation(animationName);
				if (animation != null)
				{
					state.SetAnimation(animation, loop);
				}
			}
			state.Loop = loop;
		}
		state.Update(Time.deltaTime * timeScale);
		state.Apply(skeleton);
		base.UpdateSkeleton();
	}
}
