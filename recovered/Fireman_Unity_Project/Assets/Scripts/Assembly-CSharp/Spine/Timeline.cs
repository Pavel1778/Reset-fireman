namespace Spine
{
	public interface Timeline
	{
		void Apply(Skeleton skeleton, float time, float alpha);
	}
}
