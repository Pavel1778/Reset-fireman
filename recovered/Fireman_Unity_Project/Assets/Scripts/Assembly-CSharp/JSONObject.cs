using System;
using System.Collections.Generic;
using UnityEngine;

public class JSONObject
{
	public enum Type
	{
		NULL = 0,
		STRING = 1,
		NUMBER = 2,
		OBJECT = 3,
		ARRAY = 4,
		BOOL = 5
	}

	public delegate void AddJSONConents(JSONObject self);

	public delegate void FieldNotFound(string name);

	public delegate void GetFieldResponse(JSONObject obj);

	private const int MAX_DEPTH = 1000;

	private const string INFINITY = "\"INFINITY\"";

	private const string NEGINFINITY = "\"NEGINFINITY\"";

	private const string NaN = "\"NaN\"";

	public static char[] WHITESPACE = new char[4] { ' ', '\r', '\n', '\t' };

	public JSONObject parent;

	public Type type;

	public List<JSONObject> list;

	public List<string> keys;

	public string str;

	public float n;

	public bool b;

	public bool isContainer
	{
		get
		{
			return type == Type.ARRAY || type == Type.OBJECT;
		}
	}

	public int Count
	{
		get
		{
			if (list == null)
			{
				return -1;
			}
			return list.Count;
		}
	}

	public float f
	{
		get
		{
			return n;
		}
	}

	public static JSONObject nullJO
	{
		get
		{
			return new JSONObject(Type.NULL);
		}
	}

	public static JSONObject obj
	{
		get
		{
			return new JSONObject(Type.OBJECT);
		}
	}

	public static JSONObject arr
	{
		get
		{
			return new JSONObject(Type.ARRAY);
		}
	}

	public bool IsNumber
	{
		get
		{
			return type == Type.NUMBER;
		}
	}

	public bool IsNull
	{
		get
		{
			return type == Type.NULL;
		}
	}

	public bool IsString
	{
		get
		{
			return type == Type.STRING;
		}
	}

	public bool IsBool
	{
		get
		{
			return type == Type.BOOL;
		}
	}

	public bool IsArray
	{
		get
		{
			return type == Type.ARRAY;
		}
	}

	public bool IsObject
	{
		get
		{
			return type == Type.OBJECT;
		}
	}

	public JSONObject this[int index]
	{
		get
		{
			if (list.Count > index)
			{
				return list[index];
			}
			return null;
		}
		set
		{
			if (list.Count > index)
			{
				list[index] = value;
			}
		}
	}

	public JSONObject this[string index]
	{
		get
		{
			return GetField(index);
		}
		set
		{
			SetField(index, value);
		}
	}

	public JSONObject(Type t)
	{
		type = t;
		switch (t)
		{
		case Type.ARRAY:
			list = new List<JSONObject>();
			break;
		case Type.OBJECT:
			list = new List<JSONObject>();
			keys = new List<string>();
			break;
		}
	}

	public JSONObject(bool b)
	{
		type = Type.BOOL;
		this.b = b;
	}

	public JSONObject(float f)
	{
		type = Type.NUMBER;
		n = f;
	}

	public JSONObject(Dictionary<string, string> dic)
	{
		type = Type.OBJECT;
		keys = new List<string>();
		list = new List<JSONObject>();
		foreach (KeyValuePair<string, string> item in dic)
		{
			keys.Add(item.Key);
			list.Add(new JSONObject
			{
				type = Type.STRING,
				str = item.Value
			});
		}
	}

	public JSONObject(Dictionary<string, JSONObject> dic)
	{
		type = Type.OBJECT;
		keys = new List<string>();
		list = new List<JSONObject>();
		foreach (KeyValuePair<string, JSONObject> item in dic)
		{
			keys.Add(item.Key);
			list.Add(item.Value);
		}
	}

	public JSONObject(AddJSONConents content)
	{
		content(this);
	}

	public JSONObject(JSONObject[] objs)
	{
		type = Type.ARRAY;
		list = new List<JSONObject>(objs);
	}

	public JSONObject()
	{
	}

	public JSONObject(string str, bool strict = false)
	{
		if (str != null)
		{
			str = str.Trim(WHITESPACE);
			if (strict && str[0] != '[' && str[0] != '{')
			{
				type = Type.NULL;
				Debug.LogWarning("Improper (strict) JSON formatting.  First character must be [ or {");
			}
			else
			{
				if (str.Length > 0)
				{
					switch (str)
					{
					case "true":
						type = Type.BOOL;
						b = true;
						return;
					case "false":
						type = Type.BOOL;
						b = false;
						return;
					case "null":
						type = Type.NULL;
						return;
					case "\"INFINITY\"":
						type = Type.NUMBER;
						n = float.PositiveInfinity;
						return;
					case "\"NEGINFINITY\"":
						type = Type.NUMBER;
						n = float.NegativeInfinity;
						return;
					case "\"NaN\"":
						type = Type.NUMBER;
						n = float.NaN;
						return;
					}
					if (str[0] == '"')
					{
						type = Type.STRING;
						this.str = str.Substring(1, str.Length - 2);
						return;
					}
					try
					{
						n = Convert.ToSingle(str);
						type = Type.NUMBER;
						return;
					}
					catch (FormatException)
					{
						int num = 1;
						int num2 = 0;
						switch (str[num2])
						{
						case '{':
							type = Type.OBJECT;
							keys = new List<string>();
							list = new List<JSONObject>();
							break;
						case '[':
							type = Type.ARRAY;
							list = new List<JSONObject>();
							break;
						default:
							type = Type.NULL;
							Debug.LogWarning("improper JSON formatting:" + str);
							return;
						}
						string item = string.Empty;
						bool flag = false;
						bool flag2 = false;
						int num3 = 0;
						while (++num2 < str.Length)
						{
							if (Array.IndexOf(WHITESPACE, str[num2]) > -1)
							{
								continue;
							}
							if (str[num2] == '"')
							{
								if (flag)
								{
									if (!flag2 && num3 == 0 && type == Type.OBJECT)
									{
										item = str.Substring(num + 1, num2 - num - 1);
									}
									flag = false;
								}
								else
								{
									if (num3 == 0 && type == Type.OBJECT)
									{
										num = num2;
									}
									flag = true;
								}
							}
							if (flag)
							{
								continue;
							}
							if (type == Type.OBJECT && num3 == 0 && str[num2] == ':')
							{
								num = num2 + 1;
								flag2 = true;
							}
							if (str[num2] == '[' || str[num2] == '{')
							{
								num3++;
							}
							else if (str[num2] == ']' || str[num2] == '}')
							{
								num3--;
							}
							if ((str[num2] != ',' || num3 != 0) && num3 >= 0)
							{
								continue;
							}
							flag2 = false;
							string text = str.Substring(num, num2 - num).Trim(WHITESPACE);
							if (text.Length > 0)
							{
								if (type == Type.OBJECT)
								{
									keys.Add(item);
								}
								list.Add(new JSONObject(text));
							}
							num = num2 + 1;
						}
						return;
					}
				}
				type = Type.NULL;
			}
		}
		else
		{
			type = Type.NULL;
		}
	}

	public static JSONObject StringObject(string val)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.type = Type.STRING;
		jSONObject.str = val;
		return jSONObject;
	}

	public void Absorb(JSONObject obj)
	{
		list.AddRange(obj.list);
		keys.AddRange(obj.keys);
		str = obj.str;
		n = obj.n;
		b = obj.b;
		type = obj.type;
	}

	public void Add(bool val)
	{
		Add(new JSONObject(val));
	}

	public void Add(float val)
	{
		Add(new JSONObject(val));
	}

	public void Add(int val)
	{
		Add(new JSONObject(val));
	}

	public void Add(string str)
	{
		Add(StringObject(str));
	}

	public void Add(AddJSONConents content)
	{
		Add(new JSONObject(content));
	}

	public void Add(JSONObject obj)
	{
		if (!obj)
		{
			return;
		}
		if (type != Type.ARRAY)
		{
			type = Type.ARRAY;
			if (list == null)
			{
				list = new List<JSONObject>();
			}
		}
		list.Add(obj);
	}

	public void AddField(string name, bool val)
	{
		AddField(name, new JSONObject(val));
	}

	public void AddField(string name, float val)
	{
		AddField(name, new JSONObject(val));
	}

	public void AddField(string name, int val)
	{
		AddField(name, new JSONObject(val));
	}

	public void AddField(string name, AddJSONConents content)
	{
		AddField(name, new JSONObject(content));
	}

	public void AddField(string name, string val)
	{
		AddField(name, StringObject(val));
	}

	public void AddField(string name, JSONObject obj)
	{
		if (!obj)
		{
			return;
		}
		if (type != Type.OBJECT)
		{
			keys = new List<string>();
			if (type == Type.ARRAY)
			{
				for (int i = 0; i < list.Count; i++)
				{
					keys.Add(i + string.Empty);
				}
			}
			else if (list == null)
			{
				list = new List<JSONObject>();
			}
			type = Type.OBJECT;
		}
		keys.Add(name);
		list.Add(obj);
	}

	public void SetField(string name, bool val)
	{
		SetField(name, new JSONObject(val));
	}

	public void SetField(string name, float val)
	{
		SetField(name, new JSONObject(val));
	}

	public void SetField(string name, int val)
	{
		SetField(name, new JSONObject(val));
	}

	public void SetField(string name, JSONObject obj)
	{
		if (HasField(name))
		{
			list.Remove(this[name]);
			keys.Remove(name);
		}
		AddField(name, obj);
	}

	public void RemoveField(string name)
	{
		if (keys.IndexOf(name) > -1)
		{
			list.RemoveAt(keys.IndexOf(name));
			keys.Remove(name);
		}
	}

	public void GetField(ref bool field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				field = list[num].b;
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public void GetField(ref float field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				field = list[num].n;
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public void GetField(ref int field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				field = (int)list[num].n;
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public void GetField(ref uint field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				field = (uint)list[num].n;
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public void GetField(ref string field, string name, FieldNotFound fail = null)
	{
		if (type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				field = list[num].str;
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public void GetField(string name, GetFieldResponse response, FieldNotFound fail = null)
	{
		if (response != null && type == Type.OBJECT)
		{
			int num = keys.IndexOf(name);
			if (num >= 0)
			{
				response(list[num]);
				return;
			}
		}
		if (fail != null)
		{
			fail(name);
		}
	}

	public JSONObject GetField(string name)
	{
		if (type == Type.OBJECT)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				if (keys[i] == name)
				{
					return list[i];
				}
			}
		}
		return null;
	}

	public bool HasFields(string[] names)
	{
		foreach (string item in names)
		{
			if (!keys.Contains(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasField(string name)
	{
		if (type == Type.OBJECT)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				if (keys[i] == name)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void Clear()
	{
		type = Type.NULL;
		if (list != null)
		{
			list.Clear();
		}
		if (keys != null)
		{
			keys.Clear();
		}
		str = string.Empty;
		n = 0f;
		b = false;
	}

	public JSONObject Copy()
	{
		return new JSONObject(print());
	}

	public void Merge(JSONObject obj)
	{
		MergeRecur(this, obj);
	}

	private static void MergeRecur(JSONObject left, JSONObject right)
	{
		if (left.type == Type.NULL)
		{
			left.Absorb(right);
		}
		else if (left.type == Type.OBJECT && right.type == Type.OBJECT)
		{
			for (int i = 0; i < right.list.Count; i++)
			{
				string text = right.keys[i];
				if (right[i].isContainer)
				{
					if (left.HasField(text))
					{
						MergeRecur(left[text], right[i]);
					}
					else
					{
						left.AddField(text, right[i]);
					}
				}
				else if (left.HasField(text))
				{
					left.SetField(text, right[i]);
				}
				else
				{
					left.AddField(text, right[i]);
				}
			}
		}
		else
		{
			if (left.type != Type.ARRAY || right.type != Type.ARRAY)
			{
				return;
			}
			if (right.Count > left.Count)
			{
				Debug.LogError("Cannot merge arrays when right object has more elements");
				return;
			}
			for (int j = 0; j < right.list.Count; j++)
			{
				if (left[j].type == right[j].type)
				{
					if (left[j].isContainer)
					{
						MergeRecur(left[j], right[j]);
					}
					else
					{
						left[j] = right[j];
					}
				}
			}
		}
	}

	public string print(bool pretty = false)
	{
		return print(0, pretty);
	}

	public string print(int depth, bool pretty = false)
	{
		if (depth++ > 1000)
		{
			Debug.Log("reached max depth!");
			return string.Empty;
		}
		string text = string.Empty;
		switch (type)
		{
		case Type.STRING:
			text = "\"" + str + "\"";
			break;
		case Type.NUMBER:
			text = ((!float.IsInfinity(n)) ? ((!float.IsNegativeInfinity(n)) ? ((!float.IsNaN(n)) ? (text + n) : "\"NaN\"") : "\"NEGINFINITY\"") : "\"INFINITY\"");
			break;
		case Type.OBJECT:
			text = "{";
			if (list.Count > 0)
			{
				if (pretty)
				{
					text += "\n";
				}
				for (int i = 0; i < list.Count; i++)
				{
					string text2 = keys[i];
					JSONObject jSONObject = list[i];
					if (!jSONObject)
					{
						continue;
					}
					if (pretty)
					{
						for (int j = 0; j < depth; j++)
						{
							text += "\t";
						}
					}
					text = text + "\"" + text2 + "\":";
					text = text + jSONObject.print(depth, pretty) + ",";
					if (pretty)
					{
						text += "\n";
					}
				}
				if (pretty)
				{
					text = text.Substring(0, text.Length - 1);
				}
				text = text.Substring(0, text.Length - 1);
			}
			if (pretty && list.Count > 0)
			{
				text += "\n";
				for (int k = 0; k < depth - 1; k++)
				{
					text += "\t";
				}
			}
			text += "}";
			break;
		case Type.ARRAY:
			text = "[";
			if (list.Count > 0)
			{
				if (pretty)
				{
					text += "\n";
				}
				foreach (JSONObject item in list)
				{
					if (!item)
					{
						continue;
					}
					if (pretty)
					{
						for (int l = 0; l < depth; l++)
						{
							text += "\t";
						}
					}
					text = text + item.print(depth, pretty) + ",";
					if (pretty)
					{
						text += "\n";
					}
				}
				if (pretty)
				{
					text = text.Substring(0, text.Length - 1);
				}
				text = text.Substring(0, text.Length - 1);
			}
			if (pretty && list.Count > 0)
			{
				text += "\n";
				for (int m = 0; m < depth - 1; m++)
				{
					text += "\t";
				}
			}
			text += "]";
			break;
		case Type.BOOL:
			text = ((!b) ? "false" : "true");
			break;
		case Type.NULL:
			text = "null";
			break;
		}
		return text;
	}

	public override string ToString()
	{
		return print();
	}

	public string ToString(bool pretty)
	{
		return print(pretty);
	}

	public Dictionary<string, string> ToDictionary()
	{
		if (type == Type.OBJECT)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < list.Count; i++)
			{
				JSONObject jSONObject = list[i];
				switch (jSONObject.type)
				{
				case Type.STRING:
					dictionary.Add(keys[i], jSONObject.str);
					break;
				case Type.NUMBER:
					dictionary.Add(keys[i], jSONObject.n + string.Empty);
					break;
				case Type.BOOL:
					dictionary.Add(keys[i], jSONObject.b + string.Empty);
					break;
				default:
					Debug.LogWarning("Omitting object: " + keys[i] + " in dictionary conversion");
					break;
				}
			}
			return dictionary;
		}
		Debug.LogWarning("Tried to turn non-Object JSONObject into a dictionary");
		return null;
	}

	public static implicit operator WWWForm(JSONObject obj)
	{
		WWWForm wWWForm = new WWWForm();
		for (int i = 0; i < obj.list.Count; i++)
		{
			string fieldName = i + string.Empty;
			if (obj.type == Type.OBJECT)
			{
				fieldName = obj.keys[i];
			}
			string text = obj.list[i].ToString();
			if (obj.list[i].type == Type.STRING)
			{
				text = text.Replace("\"", string.Empty);
			}
			wWWForm.AddField(fieldName, text);
		}
		return wWWForm;
	}

	public static implicit operator bool(JSONObject o)
	{
		return o != null;
	}
}
