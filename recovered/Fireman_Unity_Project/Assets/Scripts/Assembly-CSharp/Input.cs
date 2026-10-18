using AndroidPlugin;
using UnityEngine;

public class Input
{
	private static InputBase input;

	public static Vector3 mousePosition
	{
		get
		{
			return input.mousePosition;
		}
	}

	public static string inputString
	{
		get
		{
			return input.inputString;
		}
	}

	public static string compositionString
	{
		get
		{
			return input.compositionString;
		}
	}

	public static Vector2 compositionCursorPos
	{
		get
		{
			return input.compositionCursorPos;
		}
		set
		{
			input.compositionCursorPos = value;
		}
	}

	public static IMECompositionMode imeCompositionMode
	{
		get
		{
			return input.imeCompositionMode;
		}
		set
		{
			input.imeCompositionMode = value;
		}
	}

	public static int touchCount
	{
		get
		{
			return input.touchCount;
		}
	}

	static Input()
	{
		if (AppConfig.Platform == AppConfig.PlatformEnum.AndroidOuya)
		{
			input = new InputOuya();
		}
		else if (Core.IsGoogleTV())
		{
			input = new InputGoogleTV();
		}
		else if (Core.IsLexibox())
		{
			input = new InputLexibox();
		}
		else
		{
			input = new InputBase();
		}
	}

	public static void Update()
	{
		input.Update();
	}

	public static float GetAxis(string axisName)
	{
		return input.GetAxis(axisName);
	}

	public static float GetAxisRaw(string axisName)
	{
		return input.GetAxisRaw(axisName);
	}

	public static string[] GetJoystickNames()
	{
		return input.GetJoystickNames();
	}

	public static Touch GetTouch(int index)
	{
		return input.GetTouch(index);
	}

	public static bool GetKey(KeyCode Key)
	{
		return input.GetKey(Key);
	}

	public static bool GetKeyUp(KeyCode Key)
	{
		return input.GetKeyUp(Key);
	}

	public static bool GetKeyDown(KeyCode Key)
	{
		return input.GetKeyDown(Key);
	}

	public static bool GetMouseButton(int Button)
	{
		return input.GetMouseButton(Button);
	}

	public static bool GetMouseButtonUp(int Button)
	{
		return input.GetMouseButtonUp(Button);
	}

	public static bool GetMouseButtonDown(int Button)
	{
		return input.GetMouseButtonDown(Button);
	}
}
