using UnityEngine;

public class InputBase
{
	public virtual Vector3 mousePosition
	{
		get
		{
			return UnityEngine.Input.mousePosition;
		}
	}

	public virtual string inputString
	{
		get
		{
			return UnityEngine.Input.inputString;
		}
	}

	public virtual string compositionString
	{
		get
		{
			return UnityEngine.Input.compositionString;
		}
	}

	public virtual Vector2 compositionCursorPos
	{
		get
		{
			return UnityEngine.Input.compositionCursorPos;
		}
		set
		{
			UnityEngine.Input.compositionCursorPos = value;
		}
	}

	public virtual IMECompositionMode imeCompositionMode
	{
		get
		{
			return UnityEngine.Input.imeCompositionMode;
		}
		set
		{
			UnityEngine.Input.imeCompositionMode = value;
		}
	}

	public virtual int touchCount
	{
		get
		{
			return UnityEngine.Input.touchCount;
		}
	}

	public virtual void Update()
	{
	}

	public virtual float GetAxis(string axisName)
	{
		return UnityEngine.Input.GetAxis(axisName);
	}

	public virtual float GetAxisRaw(string axisName)
	{
		return UnityEngine.Input.GetAxisRaw(axisName);
	}

	public virtual string[] GetJoystickNames()
	{
		return UnityEngine.Input.GetJoystickNames();
	}

	public virtual Touch GetTouch(int index)
	{
		return UnityEngine.Input.GetTouch(index);
	}

	public virtual bool GetKeyDown(KeyCode Key)
	{
		return UnityEngine.Input.GetKeyDown(Key);
	}

	public virtual bool GetKeyUp(KeyCode Key)
	{
		return UnityEngine.Input.GetKeyUp(Key);
	}

	public virtual bool GetKey(KeyCode Key)
	{
		return UnityEngine.Input.GetKey(Key);
	}

	public virtual bool GetMouseButton(int Button)
	{
		return UnityEngine.Input.GetMouseButton(Button);
	}

	public virtual bool GetMouseButtonUp(int Button)
	{
		return UnityEngine.Input.GetMouseButtonUp(Button);
	}

	public virtual bool GetMouseButtonDown(int Button)
	{
		return UnityEngine.Input.GetMouseButtonDown(Button);
	}
}
