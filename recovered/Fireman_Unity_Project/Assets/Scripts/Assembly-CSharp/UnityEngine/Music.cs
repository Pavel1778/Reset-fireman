namespace UnityEngine
{
	public class Music : AudioClipEx
	{
		public Music(string pName)
			: base(pName)
		{
		}

		public override void Play()
		{
			AudioPlayerEx.playMusic(this, 0f);
		}

		public override void Stop()
		{
			AudioPlayerEx.stopMusic(this);
		}

		public void PlayFade(float pFade)
		{
			AudioPlayerEx.playFade2Music(this, pFade);
		}
	}
}
