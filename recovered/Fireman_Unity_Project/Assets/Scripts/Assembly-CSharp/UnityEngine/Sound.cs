namespace UnityEngine
{
	public class Sound : AudioClipEx
	{
		public Sound(string pName)
			: base(pName)
		{
		}

		public override void Play()
		{
			AudioPlayerEx.playSound(this);
		}

		public override void Stop()
		{
			AudioPlayerEx.stopSound(this);
		}
	}
}
