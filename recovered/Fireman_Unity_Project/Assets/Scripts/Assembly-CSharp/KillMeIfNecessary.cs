using AndroidPlugin;
using UnityEngine;

public class KillMeIfNecessary : MonoBehaviour
{
	public bool KillMeIfIOS;

	public bool KillMeIfAndroid;

	public bool KillMeIfWP8;

	public bool KillMeIfWin8;

	public bool KillMeIfMacOS;

	public bool KillMeIfAndroidTV;

	private void Awake()
	{
		if (KillMeIfIOS)
		{
		}
		if (KillMeIfAndroid)
		{
			Object.DestroyImmediate(base.gameObject);
		}
		if (KillMeIfWP8)
		{
		}
		if (KillMeIfWin8)
		{
		}
		if (KillMeIfMacOS)
		{
		}
		if (KillMeIfAndroidTV && (Core.IsGoogleTV() || Core.IsLexibox()))
		{
			base.gameObject.SetActive(false);
		}
	}
}
