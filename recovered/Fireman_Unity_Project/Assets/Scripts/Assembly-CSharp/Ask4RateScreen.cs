using System;
using AndroidPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ask4RateScreen : MonoBehaviour
{
	public static Ask4RateScreen Instance;

	[Header("-- Stars")]
	public StarButton star1;

	public StarButton star2;

	public StarButton star3;

	public StarButton star4;

	public StarButton star5;

	[Header("-- Pictures")]
	public Texture2D image1;

	public Texture2D image2;

	public Texture2D image3;

	public Texture2D image4;

	public Texture2D image5;

	[Header("-- Labels")]
	public UILabel bubbleText;

	public UILabel submitText;

	public UILabel titleText;

	[Header("-- Misc")]
	public Texture2D starImage;

	public Texture2D emptyImage;

	public GameObject btnSubmit;

	public TransitionAnim transition;

	public GameObject starburst;

	public GUI_Texture backg;

	public GUI_Texture pompier;

	public GameObject pointer;

	private bool quit;

	private Texture2D[] images;

	private StarButton[] stars;

	private Color[] shadow;

	private int[] ratings;

	private int rating;

	private string[] bubbleTextes;

	private string[] submitTextes;

	private static bool NeverAsk
	{
		get
		{
			return PlayerPrefs.GetInt("Ask4Rate_NeverAsk", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("Ask4Rate_NeverAsk", value ? 1 : 0);
		}
	}

	private static int CallCount
	{
		get
		{
			return PlayerPrefs.GetInt("Ask4Rate_Call", 0);
		}
		set
		{
			PlayerPrefs.SetInt("Ask4Rate_Call", value);
		}
	}

	private static int CallToShow
	{
		get
		{
			return 15;
		}
	}

	public void Start()
	{
		Track("ask4Rate/start");
		Instance = this;
		Musics.music_shop.PlayFade(0.2f);
		btnSubmit.SetActive(false);
		bubbleTextes = new string[5] { "not_enjoy_playing", "not_enjoy_playing", "not_enjoy_playing", "not_fully_satisfied", "you_relly_like_it" };
		submitTextes = new string[5] { "send_email", "send_email", "send_email", "send_email", "rate_on_store" };
		stars = new StarButton[5] { star5, star4, star3, star2, star1 };
		images = new Texture2D[5] { image5, image4, image3, image2, image1 };
		shadow = new Color[5]
		{
			new Color(1f, 1f, 1f, 1f),
			new Color(0.7f, 0.7f, 0.7f, 1f),
			new Color(0.5f, 0.5f, 0.5f, 1f),
			new Color(0.3f, 0.3f, 0.3f, 1f),
			new Color(0.1f, 0.1f, 0.1f, 1f)
		};
		ratings = new int[5] { 5, 4, 3, 2, 1 };
		transition.FadeIn();
		OnStarClick(null);
	}

	public void OnSubmitClick()
	{
		Track("ask4Rate/submit/" + rating);
		Sounds.snd_ui_play.Play();
		if (rating < 5)
		{
			NeverAsk = true;
			OnSendMail();
		}
		else
		{
			NeverAsk = true;
			OnRate();
		}
		ReturnToGame();
	}

	private static void OnSendMail()
	{
		Core.email("supportapp@magmamobile.com", "About Fireman", string.Empty);
	}

	private static void OnRate()
	{
		Core.showGooglePlayGamePage();
	}

	private static bool CanShow()
	{
		if (Core.IsGoogleTV() || Core.IsLexibox() || Core.IsAmazon() || Core.IsFunTab2())
		{
			return false;
		}
		return true;
	}

	public void OnBackButton()
	{
		if (!quit)
		{
			Sounds.snd_ui_play.Play();
			transition.FadeOut();
			quit = true;
		}
	}

	public void Update()
	{
		if (quit)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackButton();
			}
			if (!transition.IsPlaying)
			{
				Track("ask4Rate/quit");
				ReturnToGame();
			}
		}
	}

	private void ReturnToGame()
	{
		VarsManager.Instance.loadGame = true;
		VarsManager.Instance.loadMenu = false;
		SceneManager.LoadScene("SceneLoading");
	}

	public void OnStarClick(StarButton o)
	{
		if (o != null)
		{
			Track("ask4Rate/click/" + o.name);
		}
		rating = 0;
		bool flag = false;
		for (int i = 0; i < stars.Length; i++)
		{
			if (o == stars[i])
			{
				rating = ratings[i];
				pompier.Texture = images[i];
				backg.Color = shadow[i];
				flag = true;
			}
			GUI_Texture component = stars[i].GetComponent<GUI_Texture>();
			component.Texture = ((!flag) ? emptyImage : starImage);
		}
		if (rating > 0)
		{
			pointer.SetActive(false);
			bubbleText.ChangeText(TextManager.GetText(bubbleTextes[rating - 1]));
			submitText.ChangeText(TextManager.GetText(submitTextes[rating - 1]));
		}
		else
		{
			bubbleText.ChangeText(TextManager.GetText("how_do_you_like"));
		}
		titleText.ChangeText(TextManager.GetText("rate the game"));
		starburst.SetActive(rating == 5);
		btnSubmit.SetActive(rating >= 1);
		if (rating == 5)
		{
			Sounds.snd_ig_notif.Play();
		}
		else
		{
			Sounds.snd_ui_select.Play();
		}
	}

	public static bool next()
	{
		if (NeverAsk || !CanShow())
		{
			return false;
		}
		CallCount++;
		if (CallCount >= CallToShow)
		{
			CallCount = 0;
			SceneManager.LoadScene("SceneAsk4Rate");
			return true;
		}
		return false;
	}

	private void Track(string p)
	{
		try
		{
			PluginScript.Instance.track(p);
		}
		catch (Exception)
		{
		}
	}
}
