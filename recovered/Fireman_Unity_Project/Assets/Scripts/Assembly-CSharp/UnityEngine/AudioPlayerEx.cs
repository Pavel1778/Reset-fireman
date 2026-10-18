namespace UnityEngine
{
	public class AudioPlayerEx
	{
		private static bool soundEnabled;

		private static bool musicEnabled;

		private static GameObject gameObject;

		private static AudioSource[] listAudio;

		private static bool fade2Music;

		private static bool fadeStop;

		private static bool fadePlay;

		private static bool fadePause;

		private static float timeCur;

		private static float timeFade;

		public static bool SoundEnabled
		{
			get
			{
				return soundEnabled;
			}
			set
			{
				soundEnabled = value;
				PlayerPrefs.SetInt("sound", soundEnabled ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		public static bool MusicEnabled
		{
			get
			{
				return musicEnabled;
			}
			set
			{
				musicEnabled = value;
				PlayerPrefs.SetInt("music", musicEnabled ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		private AudioPlayerEx()
		{
		}

		public static void Initialize(GameObject pGameObject)
		{
			gameObject = pGameObject;
			soundEnabled = PlayerPrefs.GetInt("sound", 1) == 1;
			musicEnabled = PlayerPrefs.GetInt("music", 1) == 1;
			listAudio = new AudioSource[10];
			for (int i = 0; i < listAudio.Length; i++)
			{
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.mute = false;
				audioSource.bypassEffects = false;
				audioSource.bypassListenerEffects = false;
				audioSource.bypassReverbZones = false;
				audioSource.loop = false;
				audioSource.playOnAwake = true;
				audioSource.priority = 128;
				audioSource.volume = 1f;
				audioSource.pitch = 1f;
				listAudio[i] = audioSource;
			}
		}

		public static bool isThisSoundPlaying(AudioClipEx clip)
		{
			if (!soundEnabled)
			{
				return false;
			}
			AudioSource[] array = listAudio;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource != null && audioSource.isPlaying && audioSource.clip.name.Equals(clip.name))
				{
					return true;
				}
			}
			return false;
		}

		public static void playSound(AudioClipEx clip)
		{
			if (!soundEnabled)
			{
				return;
			}
			AudioSource[] array = listAudio;
			for (int i = 2; i < array.Length; i++)
			{
				AudioSource audioSource = array[i];
				if (audioSource != null && !audioSource.isPlaying)
				{
					audioSource.clip = clip.clip;
					audioSource.loop = false;
					audioSource.volume = 1f;
					audioSource.Play();
					break;
				}
			}
		}

		public static void stopSound(AudioClipEx clip)
		{
			AudioSource[] array = listAudio;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource != null && audioSource.isPlaying && audioSource.clip.name.Equals(clip.name))
				{
					audioSource.Stop();
				}
			}
		}

		public static void playMusic(AudioClipEx clip, float fade = 0f)
		{
			if (musicEnabled)
			{
				if (fade == 0f)
				{
					listAudio[0].volume = 1f;
					listAudio[0].clip = clip.clip;
					listAudio[0].Play();
					fadePlay = false;
					fadeStop = false;
				}
				else
				{
					timeCur = 0f;
					timeFade = fade;
					fadePlay = true;
					listAudio[0].volume = 0f;
					listAudio[0].clip = clip.clip;
					listAudio[0].Play();
				}
			}
		}

		public static void playFade2Music(AudioClipEx clip, float fade = 0f)
		{
			if (musicEnabled)
			{
				timeCur = 0f;
				timeFade = fade;
				fade2Music = true;
				listAudio[1].volume = 0f;
				listAudio[1].clip = clip.clip;
				listAudio[1].loop = true;
				listAudio[1].Play();
			}
		}

		public static void stopMusic(AudioClipEx clip)
		{
			AudioSource[] array = listAudio;
			foreach (AudioSource audioSource in array)
			{
				if (audioSource != null && audioSource.isPlaying && audioSource.clip.name.Equals(clip.name))
				{
					audioSource.Stop();
				}
			}
		}

		public static void stopMusic(float fade)
		{
			if (fade > 0f)
			{
				fadeStop = true;
				timeFade = fade;
			}
			else if (listAudio[0].clip != null)
			{
				listAudio[0].Stop();
			}
		}

		public static void pauseMusic(float fade = 0f)
		{
			if (musicEnabled && listAudio[0].clip != null)
			{
				if (fade == 0f)
				{
					listAudio[0].Pause();
					fadePause = false;
				}
				else
				{
					timeCur = 0f;
					timeFade = fade;
					fadePause = true;
				}
			}
		}

		public static void resumeMusic(float fade = 0f)
		{
			if (musicEnabled && listAudio[0].clip != null)
			{
				if (fade == 0f)
				{
					listAudio[0].volume = 1f;
					listAudio[0].Play();
					return;
				}
				timeCur = 0f;
				timeFade = fade;
				fadePlay = true;
				listAudio[0].volume = 0f;
				listAudio[0].Play();
			}
		}

		public static void FixedUpdate()
		{
			if (fade2Music)
			{
				timeCur += Time.fixedDeltaTime;
				float num = ((!(timeCur > timeFade)) ? (timeCur / timeFade) : 1f);
				listAudio[0].volume = 1f - num;
				listAudio[1].volume = num;
				if (num == 1f)
				{
					listAudio[0].Stop();
					AudioSource audioSource = listAudio[0];
					listAudio[0] = listAudio[1];
					listAudio[1] = audioSource;
					fade2Music = false;
				}
			}
			if (fadeStop)
			{
				timeCur += Time.fixedDeltaTime;
				float num = ((!(timeCur > timeFade)) ? (1f - timeCur / timeFade) : 0f);
				listAudio[0].volume = num;
				if (num == 0f)
				{
					listAudio[0].Stop();
					fadeStop = false;
				}
			}
			if (fadePause)
			{
				timeCur += Time.fixedDeltaTime;
				float num = ((!(timeCur > timeFade)) ? (1f - timeCur / timeFade) : 0f);
				listAudio[0].volume = num;
				if (num == 0f)
				{
					listAudio[0].Pause();
					fadePause = false;
				}
			}
			if (fadePlay)
			{
				timeCur += Time.fixedDeltaTime;
				float num = ((!(timeCur > timeFade)) ? (timeCur / timeFade) : 1f);
				listAudio[0].volume = num;
				if (num == 1f)
				{
					fadePlay = false;
				}
			}
		}

		public static void toggleSound()
		{
			if (soundEnabled)
			{
				SoundEnabled = false;
			}
			else
			{
				SoundEnabled = true;
			}
		}

		public static void toggleMusic()
		{
			if (musicEnabled)
			{
				MusicEnabled = false;
				stopMusic(0f);
				return;
			}
			MusicEnabled = true;
			if (MenusManager.Instance != null)
			{
				Musics.music_menu.PlayFade(1f);
			}
			else if (GameManager.Instance != null)
			{
				if (VarsManager.Instance.worldToLoad == 3)
				{
					Musics.music_ingame2.PlayFade(1f);
				}
				else
				{
					Musics.music_ingame1.PlayFade(1f);
				}
			}
		}
	}
}
