using UnityEngine;

public class BaseSpawner<T> where T : BaseSpawnable
{
	private int _capacity;

	private GameObject _prefab;

	private int _last;

	private T[] _col;

	private GameObject _root;

	public int Capacity
	{
		get
		{
			return _capacity;
		}
	}

	public int Count
	{
		get
		{
			return _last;
		}
	}

	public GameObject Prefab
	{
		get
		{
			return _prefab;
		}
	}

	public GameObject Root
	{
		get
		{
			return _root;
		}
	}

	public BaseSpawner(int capacity, GameObject prefab)
	{
		GameObject gameObject = GameObject.Find("Spawners");
		if (gameObject == null)
		{
			gameObject = new GameObject("Spawners");
		}
		_root = new GameObject(string.Concat("Spawner<", typeof(T), ">"));
		_root.transform.parent = gameObject.transform;
		_last = 0;
		_prefab = prefab;
		_capacity = capacity;
		_col = new T[capacity];
		for (int i = 0; (float)i < (float)capacity * 0.1f; i++)
		{
			_col[i] = newObject();
		}
	}

	private T newObject()
	{
		GameObject gameObject = Object.Instantiate(_prefab);
		T component = gameObject.GetComponent<T>();
		gameObject.transform.parent = _root.transform;
		gameObject.name = _prefab.name;
		component.Deleted = true;
		return component;
	}

	public T Allocate()
	{
		if (_last < _capacity)
		{
			T val = _col[_last];
			if (val == null)
			{
				val = newObject();
				_col[_last] = val;
			}
			val.Deleted = false;
			_last++;
			return val;
		}
		T result = _col[0];
		result.Deleted = false;
		return result;
	}

	public void Update()
	{
		int num = 0;
		T[] col = _col;
		while (num < _last)
		{
			T val = col[num];
			if (val.Deleted)
			{
				_last--;
				if (_last > 0)
				{
					col[num] = col[_last];
					col[_last] = val;
				}
			}
			else
			{
				num++;
			}
		}
	}
}
