using UnityEngine;

public class InputNearby : InputBase
{
	public override void Update()
	{
		for (int i = 0; i < MyEventHandler.keyStates.Length; i++)
		{
			MyEventHandler.keyLast[i] = MyEventHandler.keyStates[i];
		}
	}

	public override bool GetKeyDown(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return MyEventHandler.keyStates[0] && !MyEventHandler.keyLast[0];
		case KeyCode.UpArrow:
			return MyEventHandler.keyStates[1] && !MyEventHandler.keyLast[1];
		case KeyCode.RightArrow:
			return MyEventHandler.keyStates[2] && !MyEventHandler.keyLast[2];
		case KeyCode.DownArrow:
			return MyEventHandler.keyStates[3] && !MyEventHandler.keyLast[3];
		case KeyCode.X:
			return MyEventHandler.keyStates[4] && !MyEventHandler.keyLast[4];
		case KeyCode.W:
			return MyEventHandler.keyStates[5] && !MyEventHandler.keyLast[5];
		case KeyCode.Escape:
			return MyEventHandler.keyStates[6] && !MyEventHandler.keyLast[6];
		default:
			return UnityEngine.Input.GetKeyDown(Key);
		}
	}

	public override bool GetKeyUp(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return !MyEventHandler.keyStates[0] && MyEventHandler.keyLast[0];
		case KeyCode.UpArrow:
			return !MyEventHandler.keyStates[1] && MyEventHandler.keyLast[1];
		case KeyCode.RightArrow:
			return !MyEventHandler.keyStates[2] && MyEventHandler.keyLast[2];
		case KeyCode.DownArrow:
			return !MyEventHandler.keyStates[3] && MyEventHandler.keyLast[3];
		case KeyCode.X:
			return !MyEventHandler.keyStates[4] && MyEventHandler.keyLast[4];
		case KeyCode.W:
			return !MyEventHandler.keyStates[5] && MyEventHandler.keyLast[5];
		case KeyCode.Escape:
			return !MyEventHandler.keyStates[6] && MyEventHandler.keyLast[6];
		default:
			return UnityEngine.Input.GetKeyUp(Key);
		}
	}

	public override bool GetKey(KeyCode Key)
	{
		switch (Key)
		{
		case KeyCode.LeftArrow:
			return MyEventHandler.keyStates[0];
		case KeyCode.UpArrow:
			return MyEventHandler.keyStates[1];
		case KeyCode.RightArrow:
			return MyEventHandler.keyStates[2];
		case KeyCode.DownArrow:
			return MyEventHandler.keyStates[3];
		case KeyCode.X:
			return MyEventHandler.keyStates[4];
		case KeyCode.W:
			return MyEventHandler.keyStates[5];
		case KeyCode.Escape:
			return MyEventHandler.keyStates[6];
		default:
			return UnityEngine.Input.GetKey(Key);
		}
	}
}
