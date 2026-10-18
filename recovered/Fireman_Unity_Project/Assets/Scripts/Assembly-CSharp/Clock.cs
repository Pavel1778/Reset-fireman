using UnityEngine;

public class Clock
{
	private UILabel _label;

	private bool _paused;

	private float _value;

	private int _lastValue = -1;

	public float Value
	{
		get
		{
			return _value;
		}
	}

	public bool Paused
	{
		get
		{
			return _paused;
		}
		set
		{
			_paused = value;
		}
	}

	public UILabel Label
	{
		get
		{
			return _label;
		}
		set
		{
			_label = value;
			UpdateUI();
		}
	}

	public Clock(GameObject g)
	{
		if (!(g == null))
		{
			_label = g.GetComponent<UILabel>();
		}
	}

	public void Update()
	{
		if (!_paused)
		{
			_value += Time.deltaTime;
			UpdateUI();
		}
	}

	public void Reset()
	{
		_lastValue = -1;
		_value = 0f;
	}

	public void UpdateUI()
	{
		if (!(_label == null))
		{
			int num = (int)_value;
			if (num != _lastValue)
			{
				_lastValue = num;
				int num2 = num / 60;
				int num3 = num % 60;
				string text = "time\n" + ((num2 >= 10) ? string.Empty : "0") + num2 + ":" + ((num3 >= 10) ? string.Empty : "0") + num3;
				_label.text = text;
			}
		}
	}
}
