using UnityEngine;

public class InputLexibox : InputBase
{
	private static bool _leftArrow;

	private static bool _rightArrow;

	private static bool _upArrow;

	private static bool _downArrow;

	private static bool _lastLeftArrow;

	private static bool _lastRightArrow;

	private static bool _lastUpArrow;

	private static bool _lastDownArrow;

	public override void Update()
	{
		_lastLeftArrow = _leftArrow;
		_lastRightArrow = _rightArrow;
		_lastUpArrow = _upArrow;
		_lastDownArrow = _downArrow;
		_leftArrow = UnityEngine.Input.GetAxis("Horizontal") < 0f;
		_rightArrow = UnityEngine.Input.GetAxis("Horizontal") > 0f;
		_upArrow = UnityEngine.Input.GetAxis("Vertical") > 0f;
		_downArrow = UnityEngine.Input.GetAxis("Vertical") < 0f;
	}

	public override bool GetKeyDown(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return _leftArrow && !_lastLeftArrow;
		case KeyCode.RightArrow:
			return _rightArrow && !_lastRightArrow;
		case KeyCode.UpArrow:
			return _upArrow && !_lastUpArrow;
		case KeyCode.DownArrow:
			return _downArrow && !_lastDownArrow;
		case KeyCode.X:
			return UnityEngine.Input.GetKeyDown(KeyCode.JoystickButton0);
		case KeyCode.W:
			return UnityEngine.Input.GetKeyDown(KeyCode.JoystickButton2);
		default:
			return UnityEngine.Input.GetKeyDown(Key);
		}
	}

	public override bool GetKeyUp(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return !_leftArrow && _lastLeftArrow;
		case KeyCode.RightArrow:
			return !_rightArrow && _lastRightArrow;
		case KeyCode.UpArrow:
			return !_upArrow && _lastUpArrow;
		case KeyCode.DownArrow:
			return !_downArrow && _lastDownArrow;
		case KeyCode.X:
			return UnityEngine.Input.GetKeyUp(KeyCode.JoystickButton0);
		case KeyCode.W:
			return UnityEngine.Input.GetKeyUp(KeyCode.JoystickButton2);
		default:
			return UnityEngine.Input.GetKeyUp(Key);
		}
	}

	public override bool GetKey(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return _leftArrow;
		case KeyCode.RightArrow:
			return _rightArrow;
		case KeyCode.UpArrow:
			return _upArrow;
		case KeyCode.DownArrow:
			return _downArrow;
		case KeyCode.X:
			return UnityEngine.Input.GetKey(KeyCode.JoystickButton0);
		case KeyCode.W:
			return UnityEngine.Input.GetKey(KeyCode.JoystickButton2);
		default:
			return UnityEngine.Input.GetKey(Key);
		}
	}
}
