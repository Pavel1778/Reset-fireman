using UnityEngine;

public class MoreGamesFromLabel : MonoBehaviour
{
	private void Awake()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			GetComponent<UILabel>().text = "More Games from";
			break;
		case SystemLanguage.French:
			GetComponent<UILabel>().text = "Plus de jeux de";
			break;
		case SystemLanguage.Spanish:
			GetComponent<UILabel>().text = "Mas juegos de";
			break;
		case SystemLanguage.Japanese:
			GetComponent<UILabel>().text = "他のゲーム";
			break;
		case SystemLanguage.Chinese:
			GetComponent<UILabel>().text = "更多游戏";
			break;
		case SystemLanguage.Korean:
			GetComponent<UILabel>().text = "더 많은 게임";
			break;
		case SystemLanguage.Portuguese:
			GetComponent<UILabel>().text = "Mais Jogos de";
			break;
		case SystemLanguage.Lithuanian:
			GetComponent<UILabel>().text = "Daugiau žaidimų iš";
			break;
		case SystemLanguage.Italian:
			GetComponent<UILabel>().text = "Altri Giochi da";
			break;
		case SystemLanguage.Czech:
			GetComponent<UILabel>().text = "Další hry z";
			break;
		case SystemLanguage.German:
			GetComponent<UILabel>().text = "Mehr Spiele aus";
			break;
		case SystemLanguage.Polish:
			GetComponent<UILabel>().text = "Więcej gier z";
			break;
		case SystemLanguage.Norwegian:
			GetComponent<UILabel>().text = "Flere spill fra";
			break;
		case SystemLanguage.Russian:
			GetComponent<UILabel>().text = "Больше игр из";
			break;
		case SystemLanguage.SerboCroatian:
			GetComponent<UILabel>().text = "Више од игре";
			break;
		case SystemLanguage.Danish:
			GetComponent<UILabel>().text = "Flere spil fra";
			break;
		case SystemLanguage.Romanian:
			GetComponent<UILabel>().text = "Mai multe jocuri de la";
			break;
		case SystemLanguage.Swedish:
			GetComponent<UILabel>().text = "Fler spel från";
			break;
		case SystemLanguage.Turkish:
			GetComponent<UILabel>().text = "Den fazla oyun";
			break;
		case SystemLanguage.Finnish:
			GetComponent<UILabel>().text = "Lisää pelejä";
			break;
		case SystemLanguage.Indonesian:
			GetComponent<UILabel>().text = "Lebih dari games";
			break;
		case SystemLanguage.Greek:
			GetComponent<UILabel>().text = "Περισσότερα παιχνίδια από";
			break;
		case SystemLanguage.Thai:
			GetComponent<UILabel>().text = "เกมอ\u0e37\u0e48น ๆ";
			break;
		default:
			GetComponent<UILabel>().text = "More Games from";
			break;
		}
	}
}
