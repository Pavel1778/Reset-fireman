using UnityEngine;

public class FPSManager
{
	public static float updateInterval = 0.5f;

	private static float accum;

	private static int frames;

	private static float timeleft;

	private static UILabel guiText;

	public static void Start()
	{
		guiText = null;
		timeleft = updateInterval;
	}

	public static void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			string text = Time.deltaTime.ToString("f5");
			guiText.text = text;
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
