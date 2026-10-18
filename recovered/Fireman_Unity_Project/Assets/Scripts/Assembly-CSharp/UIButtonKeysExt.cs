using AndroidPlugin;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Keys")]
[RequireComponent(typeof(Collider))]
public class UIButtonKeysExt : UIButtonKeys
{
	public bool isMenu;

	public bool backToLast;

	public UIButtonKeysExt last;

	public bool isScrollView;

	public UIButtonKeysExt selectOnLeftBackup;

	public UIButtonKeysExt selectOnRightBackup;

	public UIButtonKeysExt selectOnDownBackup;

	public UIButtonKeysExt selectOnUpBackup;

	public UIButtonKeysExt selectOnDisable;

	public bool release;

	public static bool isPressed;

	private void Awake()
	{
		if (!Core.IsGoogleTV() && !Core.IsLexibox())
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.enabled = true;
		if (startsSelected)
		{
			if (!NGUITools.GetActive(UICamera.selectedObject))
			{
				UICamera.selectedObject = base.gameObject;
				UICamera.Notify(base.gameObject, "OnHover", true);
			}
			else
			{
				UICamera.Notify(base.gameObject, "OnHover", true);
			}
		}
		else
		{
			UICamera.Notify(base.gameObject, "OnHover", false);
		}
	}

	private void OnEnable()
	{
		base.enabled = true;
		if (startsSelected)
		{
			if (!NGUITools.GetActive(UICamera.selectedObject))
			{
				UICamera.selectedObject = base.gameObject;
				UICamera.Notify(base.gameObject, "OnHover", true);
			}
			else
			{
				UICamera.Notify(base.gameObject, "OnHover", true);
			}
		}
		else
		{
			UICamera.Notify(base.gameObject, "OnHover", false);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (isPressed || !base.enabled || !NGUITools.GetActive(base.gameObject))
		{
			return;
		}
		switch (key)
		{
		case KeyCode.LeftArrow:
			if (!(selectOnLeft != null))
			{
				break;
			}
			UICamera.selectedObject = selectOnLeft.gameObject;
			if (!selectOnLeft.gameObject.activeSelf)
			{
				if (selectOnLeftBackup != null)
				{
					if (selectOnLeftBackup.gameObject.activeSelf)
					{
						UICamera.selectedObject = selectOnLeftBackup.gameObject;
					}
					else
					{
						UICamera.selectedObject = base.gameObject;
					}
				}
				else
				{
					UICamera.selectedObject = base.gameObject;
				}
			}
			if (selectOnLeft.gameObject.GetComponent<UIButtonKeysExt>().backToLast)
			{
				selectOnLeft.gameObject.GetComponent<UIButtonKeysExt>().selectOnRight = base.gameObject.GetComponent<UIButtonKeysExt>();
			}
			break;
		case KeyCode.RightArrow:
			if (!(selectOnRight != null))
			{
				break;
			}
			UICamera.selectedObject = selectOnRight.gameObject;
			if (!selectOnRight.gameObject.activeSelf && selectOnRightBackup != null)
			{
				if (selectOnRightBackup.gameObject.activeSelf)
				{
					UICamera.selectedObject = selectOnRightBackup.gameObject;
				}
				else
				{
					UICamera.selectedObject = base.gameObject;
				}
			}
			if (selectOnRight.gameObject.GetComponent<UIButtonKeysExt>().backToLast)
			{
				selectOnRight.gameObject.GetComponent<UIButtonKeysExt>().selectOnLeft = base.gameObject.GetComponent<UIButtonKeysExt>();
			}
			break;
		case KeyCode.UpArrow:
			if (isScrollView)
			{
				if (GetComponent<UIDraggablePanel>().transform.localPosition.y > 280f)
				{
					GetComponent<UIDraggablePanel>().MoveRelative(new Vector3(0f, -140f, 0f));
				}
			}
			else if (selectOnUp != null)
			{
				UICamera.selectedObject = selectOnUp.gameObject;
				if (!selectOnUp.gameObject.activeSelf && selectOnUpBackup != null)
				{
					UICamera.selectedObject = selectOnUpBackup.gameObject;
					UICamera.Notify(UICamera.selectedObject, "OnHover", true);
				}
				if (selectOnUp.gameObject.GetComponent<UIButtonKeysExt>().backToLast)
				{
					selectOnUp.gameObject.GetComponent<UIButtonKeysExt>().selectOnDown = base.gameObject.GetComponent<UIButtonKeysExt>();
				}
			}
			break;
		case KeyCode.DownArrow:
			if (isScrollView)
			{
				if (GetComponent<UIDraggablePanel>().transform.localPosition.y < (float)(GetComponent<UIDraggablePanel>().transform.GetChild(0).transform.childCount * 140))
				{
					GetComponent<UIDraggablePanel>().MoveRelative(new Vector3(0f, 140f, 0f));
				}
			}
			else
			{
				if (!(selectOnDown != null))
				{
					break;
				}
				UICamera.selectedObject = selectOnDown.gameObject;
				if (!selectOnDown.gameObject.activeSelf)
				{
					if (selectOnDownBackup != null && selectOnDownBackup.gameObject.activeSelf)
					{
						UICamera.selectedObject = selectOnDownBackup.gameObject;
					}
					else
					{
						UICamera.selectedObject = base.gameObject;
					}
				}
				if (selectOnDown.gameObject.GetComponent<UIButtonKeysExt>().backToLast)
				{
					selectOnDown.gameObject.GetComponent<UIButtonKeysExt>().selectOnUp = base.gameObject.GetComponent<UIButtonKeysExt>();
				}
			}
			break;
		case KeyCode.Tab:
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				if (selectOnLeft != null)
				{
					UICamera.selectedObject = selectOnLeft.gameObject;
				}
				else if (selectOnUp != null)
				{
					UICamera.selectedObject = selectOnUp.gameObject;
				}
				else if (selectOnDown != null)
				{
					UICamera.selectedObject = selectOnDown.gameObject;
				}
				else if (selectOnRight != null)
				{
					UICamera.selectedObject = selectOnRight.gameObject;
				}
			}
			else if (selectOnRight != null)
			{
				UICamera.selectedObject = selectOnRight.gameObject;
			}
			else if (selectOnDown != null)
			{
				UICamera.selectedObject = selectOnDown.gameObject;
			}
			else if (selectOnUp != null)
			{
				UICamera.selectedObject = selectOnUp.gameObject;
			}
			else if (selectOnLeft != null)
			{
				UICamera.selectedObject = selectOnLeft.gameObject;
			}
			break;
		}
	}

	private void OnClick()
	{
		if (base.enabled && selectOnClick != null && isMenu)
		{
			UICamera.selectedObject = selectOnClick.gameObject;
		}
		else if (release)
		{
			UICamera.selectedObject = selectOnDisable.gameObject;
		}
	}

	private void OnPress(bool down)
	{
		isPressed = down;
	}

	private void OnDisable()
	{
		if (base.enabled && selectOnDisable != null && selectOnDisable.enabled)
		{
			UICamera.selectedObject = selectOnDisable.gameObject;
		}
	}
}
