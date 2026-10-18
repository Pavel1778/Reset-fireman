using UnityEngine;

public class ButtonBar : MonoBehaviour
{
	private Transform[] _buttons;

	private Transform _texture;

	private int _threshold;

	public void Start()
	{
		_buttons = new Transform[6]
		{
			Common.FindTransform("SmallButtonGameService"),
			Common.FindTransform("SmallButtonAOTD"),
			Common.FindTransform("SmallButtonFacebook"),
			Common.FindTransform("SmallButtonShare"),
			Common.FindTransform("SmallButtonSettings"),
			Common.FindTransform("SmallButtonNewsLetter")
		};
	}

	public void Update()
	{
		if (_threshold == 0)
		{
			_threshold = 60;
			_texture = Common.FindTransform("TextureBackground");
			Vector3 localScale = _texture.localScale;
			Transform transform = base.transform;
			int num = _buttons.Length;
			int enabledButtons = GetEnabledButtons();
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				Transform transform2 = _buttons[i];
				if (transform2 != null && transform2.gameObject.activeSelf)
				{
					Vector3 localPosition = transform.localPosition;
					float x = localScale.x / (float)(enabledButtons + 1) * (float)(num2 + 1) - localScale.x * 0.5f;
					float num3 = localScale.y * 0.5f;
					transform2.localPosition = new Vector3(x, 0f - num3 + 70f, localPosition.z);
					num2++;
				}
			}
		}
		else
		{
			_threshold--;
		}
	}

	private int GetEnabledButtons()
	{
		int num = 0;
		for (int i = 0; i < _buttons.Length; i++)
		{
			Transform transform = _buttons[i];
			if (transform != null && _buttons[i].gameObject.activeSelf)
			{
				num++;
			}
		}
		return num;
	}
}
