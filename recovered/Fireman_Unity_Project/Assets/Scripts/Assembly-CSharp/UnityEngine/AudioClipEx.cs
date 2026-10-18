using System;

namespace UnityEngine
{
	public abstract class AudioClipEx
	{
		private bool _loaded;

		private string _name;

		private AudioClip _clip;

		public string name
		{
			get
			{
				return _name;
			}
		}

		public bool isLoaded
		{
			get
			{
				return _loaded;
			}
		}

		public bool isPlaying
		{
			get
			{
				return AudioPlayerEx.isThisSoundPlaying(this);
			}
		}

		public AudioClip clip
		{
			get
			{
				if (!_loaded)
				{
					_clip = (AudioClip)Resources.Load("Sfx/" + _name, typeof(AudioClip));
					_loaded = true;
				}
				return _clip;
			}
		}

		private AudioClipEx()
		{
		}

		public AudioClipEx(string pName)
		{
			_name = pName;
			_loaded = false;
		}

		public void Release()
		{
			if (_loaded)
			{
				_loaded = false;
				_clip = null;
				GC.Collect();
			}
		}

		public abstract void Play();

		public abstract void Stop();
	}
}
