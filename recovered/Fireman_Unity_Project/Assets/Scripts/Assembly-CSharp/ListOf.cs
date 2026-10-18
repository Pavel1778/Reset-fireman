using System;

public class ListOf<T> where T : class
{
	private T[] _array;

	private int _buffer;

	private int _total;

	private int _count;

	public int Count
	{
		get
		{
			return _count;
		}
	}

	public T this[int i]
	{
		get
		{
			if (i < 0 || i >= _count)
			{
				return (T)null;
			}
			return _array[i];
		}
		set
		{
			if (i >= 0 && i < _count)
			{
				_array[i] = value;
			}
		}
	}

	public ListOf()
	{
		_buffer = 32;
	}

	public ListOf(int s)
	{
		_buffer = s;
	}

	public void Clear()
	{
		_count = 0;
		_total = 0;
	}

	public void Add(T pItem)
	{
		if (_count >= _total)
		{
			T[] array = new T[_total + _buffer];
			for (int i = 0; i < _total; i++)
			{
				array[i] = _array[i];
			}
			_total += _buffer;
			_array = array;
		}
		_array[_count] = pItem;
		_count++;
	}

	public void AddAll(ListOf<T> pItems)
	{
		if (pItems != null)
		{
			for (int i = 0; i < pItems.Count; i++)
			{
				Add(pItems[i]);
			}
		}
	}

	public int IndexOf(T e)
	{
		T[] array = _array;
		for (int i = 0; i < _count; i++)
		{
			if (array[i] == e)
			{
				return i;
			}
		}
		return -1;
	}

	public T Remove(T pItem)
	{
		return Remove(IndexOf(pItem));
	}

	public T Remove(int pIndex)
	{
		if (pIndex < 0 || pIndex >= _count)
		{
			return (T)null;
		}
		_count--;
		T result = _array[pIndex];
		if (_count > 0)
		{
			for (int i = pIndex; i < _count; i++)
			{
				_array[i] = _array[i + 1];
			}
		}
		return result;
	}

	public T[] ToArray()
	{
		T[] array = new T[_count];
		Array.Copy(_array, array, _count);
		return array;
	}
}
