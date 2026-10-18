using UnityEngine;

public class BaseSpawnable : MonoBehaviour
{
	private bool _deleted;

	public bool Deleted
	{
		get
		{
			return _deleted;
		}
		set
		{
			if (value != _deleted)
			{
				_deleted = value;
				base.enabled = !value;
				base.gameObject.SetActiveRecursively(!value);
			}
		}
	}
}
