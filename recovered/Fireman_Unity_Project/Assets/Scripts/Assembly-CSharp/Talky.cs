using UnityEngine;

public class Talky : MonoBehaviour
{
	public static readonly string[] ONOMATOPEES = new string[3] { "Yes !", "Oh !", "ZZz" };

	public GameObject mAnim;

	public UILabel mLabel;

	private Vector3 cible;

	private int charIndex;

	private string leTexte;

	private float timeChar;

	private float timeHide;

	private int id;

	public void setTalky()
	{
		GameManager.Instance.mTalky = base.gameObject;
		cible = new Vector3(GameManager.mainCamera.transform.position.x + 0.5f, GameManager.mainCamera.transform.position.y + 1f, 0f);
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localPosition = new Vector3(0f, 0f, -1000f);
		base.transform.localScale = Vector2.one;
		mAnim.GetComponent<Renderer>().enabled = false;
		mLabel.enabled = false;
		leTexte = string.Empty;
		id = -1;
	}

	public void showTalky(Vector3 cible, int i)
	{
		timeHide = 1f;
		if (i != id)
		{
			id = i;
			charIndex = 0;
			cible = new Vector3(cible.x + 0.07f, cible.y + 0.05f, cible.z);
			if (id == -1)
			{
				leTexte = string.Empty;
			}
			else if (id >= 0)
			{
				leTexte = TextManager.GetText("res_talky_" + id);
			}
			else
			{
				leTexte = ONOMATOPEES[-id - 2];
			}
		}
		mAnim.GetComponent<Renderer>().enabled = true;
	}

	private void Update()
	{
		float num = 0.08f;
		if (timeHide <= 0f)
		{
			num = 0.04f;
		}
		base.transform.position = new Vector3(base.transform.position.x + (cible.x + 0.05f - base.transform.position.x) * num, base.transform.position.y + (cible.y + 0.3f - base.transform.position.y) * num, base.transform.position.z);
		if (base.transform.position.y > Camera.main.transform.position.y + 0.99f)
		{
			mAnim.GetComponent<Renderer>().enabled = false;
		}
		if (timeHide > 0f)
		{
			timeChar -= Time.deltaTime;
			timeHide -= Time.deltaTime;
			cible = new Vector3(GameManager.Instance.Player1.transform.position.x, GameManager.Instance.Player1.transform.position.y, base.transform.position.z);
			if (timeChar < 0f)
			{
				timeChar = 0.01f;
				if (charIndex < leTexte.Length)
				{
					if (!Sounds.snd_ig_talky.isPlaying)
					{
						Sounds.snd_ig_talky.Play();
					}
					charIndex++;
				}
				else
				{
					Sounds.snd_ig_talky.Stop();
				}
			}
			mLabel.enabled = true;
			mLabel.text = leTexte.Substring(0, charIndex);
		}
		else
		{
			mLabel.enabled = false;
			id = -1;
			charIndex = 0;
			cible = new Vector3(Camera.main.transform.position.x + 1f, Camera.main.transform.position.y + 1f, base.transform.position.z);
		}
	}
}
