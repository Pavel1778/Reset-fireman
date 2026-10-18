using UnityEngine;

[ExecuteInEditMode]
public class Sprite : MonoBehaviour
{
	public enum AlignVerticalEnum
	{
		AlignTop = 0,
		AlignBottom = 1,
		AlignMiddle = 2
	}

	public enum AlignHorizontalEnum
	{
		AlignLeft = 0,
		AlignRight = 1,
		AlignCenter = 2
	}

	[SerializeField]
	protected AlignHorizontalEnum _alignHorizontal = AlignHorizontalEnum.AlignCenter;

	[SerializeField]
	protected AlignVerticalEnum _alignVertical = AlignVerticalEnum.AlignMiddle;

	[SerializeField]
	protected Color _color = Color.white;

	[SerializeField]
	protected bool _visible = true;

	[SerializeField]
	protected float _forcedWidth;

	[SerializeField]
	protected float _forcedHeight;

	[SerializeField]
	protected UIAtlas _atlas;

	[SerializeField]
	protected string _spriteName;

	protected Vector3[] _vertices;

	protected Vector3[] _normals;

	protected Vector2[] _uvs;

	protected int[] _tris;

	protected Color[] _cols;

	protected UIAtlas.Sprite _frame;

	protected bool _update = true;

	protected int _width;

	protected int _height;

	protected float _vsx;

	protected float _vex;

	protected float _vsy;

	protected float _vey;

	protected Mesh _mesh;

	protected MeshFilter _mf;

	protected MeshRenderer _mr;

	public AlignHorizontalEnum AlignHorizontal
	{
		get
		{
			return _alignHorizontal;
		}
		set
		{
			if (value != _alignHorizontal)
			{
				_alignHorizontal = value;
				_update = true;
			}
		}
	}

	public AlignVerticalEnum AlignVertical
	{
		get
		{
			return _alignVertical;
		}
		set
		{
			if (value != _alignVertical)
			{
				_alignVertical = value;
				_update = true;
			}
		}
	}

	public UIAtlas Atlas
	{
		get
		{
			return _atlas;
		}
		set
		{
			if (_atlas != value)
			{
				_atlas = value;
				_update = true;
			}
		}
	}

	public string SpriteName
	{
		get
		{
			return _spriteName;
		}
		set
		{
			if (_spriteName != value)
			{
				_spriteName = value;
				_update = true;
			}
		}
	}

	public Color Color
	{
		get
		{
			return _color;
		}
		set
		{
			if (value != _color)
			{
				_color = value;
				_update = true;
			}
		}
	}

	public float Alpha
	{
		get
		{
			return _color.a;
		}
		set
		{
			if (_color.a != value)
			{
				_color.a = value;
				_color.r = value;
				_color.g = value;
				_color.b = value;
				_update = true;
			}
		}
	}

	public float ForcedWidth
	{
		get
		{
			return _forcedWidth;
		}
		set
		{
			if (value != _forcedWidth)
			{
				_forcedWidth = value;
				_update = true;
			}
		}
	}

	public float ForcedHeight
	{
		get
		{
			return _forcedHeight;
		}
		set
		{
			if (value != _forcedHeight)
			{
				_forcedHeight = value;
				_update = true;
			}
		}
	}

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible == value)
			{
				return;
			}
			_visible = value;
			if (base.gameObject.activeSelf != _visible)
			{
				if (!_visible)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					base.gameObject.SetActive(true);
				}
			}
		}
	}

	public int Width
	{
		get
		{
			return _width;
		}
	}

	public int Height
	{
		get
		{
			return _height;
		}
	}

	public virtual void Awake()
	{
		_tris = new int[6] { 0, 1, 2, 2, 3, 0 };
		_vertices = new Vector3[4];
		_uvs = new Vector2[4];
		_cols = new Color[4];
		_normals = new Vector3[4]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		_mesh = new Mesh();
		_mf = base.gameObject.GetComponent<MeshFilter>();
		_mr = base.gameObject.GetComponent<MeshRenderer>();
		if (_mf == null)
		{
			_mf = base.gameObject.AddComponent<MeshFilter>();
		}
		if (_mr == null)
		{
			_mr = base.gameObject.AddComponent<MeshRenderer>();
		}
		_update = true;
	}

	public virtual void Start()
	{
		_update = true;
	}

	public virtual void Update()
	{
		if (_update)
		{
			Apply();
		}
	}

	public void Apply()
	{
		createMesh();
	}

	private void createMesh()
	{
		if (_atlas == null)
		{
			return;
		}
		UIAtlas.Sprite sprite = _atlas.GetSprite(_spriteName);
		if (sprite != null)
		{
			Texture texture = _atlas.texture;
			_width = (int)sprite.inner.width;
			_height = (int)sprite.inner.height;
			int width = _width;
			int height = _height;
			int num = (int)sprite.inner.x;
			int num2 = (int)sprite.inner.y;
			float x = (float)num / (float)texture.width;
			float y = 1f - (float)num2 / (float)texture.height;
			float x2 = (float)(num + width) / (float)texture.width;
			float y2 = 1f - (float)(num2 + height) / (float)texture.height;
			width = (int)((!(_forcedWidth > 0f)) ? ((float)width) : _forcedWidth);
			height = (int)((!(_forcedHeight > 0f)) ? ((float)height) : _forcedHeight);
			if (_alignHorizontal == AlignHorizontalEnum.AlignCenter)
			{
				float num3 = width >> 1;
				_vsx = 0f - num3;
				_vex = num3;
			}
			else if (_alignHorizontal == AlignHorizontalEnum.AlignRight)
			{
				_vsx = -width;
				_vex = 0f;
			}
			else if (_alignHorizontal == AlignHorizontalEnum.AlignLeft)
			{
				_vsx = 0f;
				_vex = width;
			}
			if (_alignVertical == AlignVerticalEnum.AlignMiddle)
			{
				float num4 = height >> 1;
				_vsy = 0f - num4;
				_vey = num4;
			}
			else if (_alignVertical == AlignVerticalEnum.AlignBottom)
			{
				_vsy = -height;
				_vey = 0f;
			}
			else if (_alignVertical == AlignVerticalEnum.AlignTop)
			{
				_vsy = 0f;
				_vey = height;
			}
			_vertices[0] = new Vector3(_vsx, _vey);
			_vertices[1] = new Vector3(_vex, _vey);
			_vertices[2] = new Vector3(_vex, _vsy);
			_vertices[3] = new Vector3(_vsx, _vsy);
			_uvs[0] = new Vector2(x, y);
			_uvs[1] = new Vector2(x2, y);
			_uvs[2] = new Vector2(x2, y2);
			_uvs[3] = new Vector2(x, y2);
			_cols[0] = _color;
			_cols[1] = _color;
			_cols[2] = _color;
			_cols[3] = _color;
			_mesh.Clear();
			_mesh.vertices = _vertices;
			_mesh.normals = _normals;
			_mesh.uv = _uvs;
			_mesh.triangles = _tris;
			_mesh.colors = _cols;
			_mr.sharedMaterial = _atlas.spriteMaterial;
			_mf.sharedMesh = _mesh;
			_update = false;
		}
	}

	public bool Hit(float x, float y)
	{
		Vector3 localPosition = base.transform.localPosition;
		return x >= _vsx + localPosition.x && y >= _vsy + localPosition.y && x <= _vex + localPosition.x && y <= _vey + localPosition.y;
	}

	public bool Intersect(Sprite r)
	{
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localPosition2 = r.transform.localPosition;
		return localPosition.x + _vex >= localPosition2.x + r._vsx && localPosition.x + _vsx <= localPosition2.x + r._vex && localPosition.y + _vey >= localPosition2.y + r._vsy && localPosition.y + _vsy <= localPosition2.y + r._vey;
	}
}
