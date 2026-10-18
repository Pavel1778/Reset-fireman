using UnityEngine;

public class ResizeFontTranslation : MonoBehaviour
{
	public float JapaneseResize = 100f;

	public float KoreanResize = 100f;

	public float ChineseResize = 100f;

	public float PTBRResize = 100f;

	public float GermanResize = 100f;

	public float RussianResize = 100f;

	public float SpanishResize = 100f;

	public float CzechResize = 100f;

	public float ItalianResize = 100f;

	public float VietnameseResize = 100f;

	public float YJapanese = -1.111111f;

	public float YKorean = -1.111111f;

	public float YChinese = -1.111111f;

	public void Start()
	{
		if (TextManager.Lang == "ja")
		{
			Rescale(JapaneseResize);
			Repos(YJapanese);
		}
		else if (TextManager.Lang == "ko")
		{
			Rescale(KoreanResize);
			Repos(YKorean);
		}
		else if (TextManager.Lang == "zhcn")
		{
			Rescale(ChineseResize);
			Repos(YChinese);
		}
		else if (TextManager.Lang == "ptbr")
		{
			Rescale(PTBRResize);
		}
		else if (TextManager.Lang == "de")
		{
			Rescale(GermanResize);
		}
		else if (TextManager.Lang == "es")
		{
			Rescale(SpanishResize);
		}
		else if (TextManager.Lang == "cs")
		{
			Rescale(CzechResize);
		}
		else if (TextManager.Lang == "it")
		{
			Rescale(ItalianResize);
		}
		else if (TextManager.Lang == "ru")
		{
			Rescale(RussianResize);
		}
		else if (TextManager.Lang == "vi")
		{
			Rescale(VietnameseResize);
		}
	}

	private void Repos(float pos)
	{
		if (pos != -1.111111f)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.y = pos;
			base.transform.localPosition = localPosition;
		}
	}

	private void Rescale(float factor)
	{
		if (factor != 100f)
		{
			Vector3 localScale = base.transform.localScale;
			float num = factor * 0.01f;
			localScale.x *= num;
			localScale.y *= num;
			base.transform.localScale = localScale;
		}
	}
}
