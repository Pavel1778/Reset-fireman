using UnityEngine;

[ExecuteInEditMode]
public class GUI_Texture : MonoBehaviour
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
	protected bool _useCollider;

	[SerializeField]
	protected bool _isOpaque;

	[SerializeField]
	protected bool _visible = true;

	[SerializeField]
	protected Texture2D _texture;

	[SerializeField]
	protected float _width = 1f;

	[SerializeField]
	protected float _height = 1f;

	[SerializeField]
	protected Color _color = Color.white;

	protected Material _material;

	protected bool _update = true;

	protected float _vsx;

	protected float _vex;

	protected float _vsy;

	protected float _vey;

	private static Shader _SharedShader;

	public Texture2D Texture
	{
		get
		{
			return _texture;
		}
		set
		{
			if (value != _texture)
			{
				_texture = value;
				_update = true;
			}
		}
	}

	public bool UseCollider
	{
		get
		{
			return _useCollider;
		}
		set
		{
			if (value != _useCollider)
			{
				_useCollider = value;
				_update = true;
			}
		}
	}

	public bool IsOpaque
	{
		get
		{
			return _isOpaque;
		}
		set
		{
			if (value != _isOpaque)
			{
				_isOpaque = value;
				_update = true;
			}
		}
	}

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

	public float InnerWidth
	{
		get
		{
			return _width;
		}
		set
		{
			if (value != _width)
			{
				_width = value;
				_update = true;
			}
		}
	}

	public float InnerHeight
	{
		get
		{
			return _height;
		}
		set
		{
			if (value != _height)
			{
				_height = value;
				_update = true;
			}
		}
	}

	public bool HasUpdate
	{
		get
		{
			return _update;
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

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (value != _visible)
			{
				_visible = value;
				_update = true;
			}
		}
	}

	public virtual void Reset()
	{
		_update = true;
	}

	public virtual void Start()
	{
		if (_SharedShader == null)
		{
			_SharedShader = Resources.Load<Shader>("Shaders\\Sprite");
		}
		_material = new Material(_SharedShader);
	}

	public virtual void Awake()
	{
		_update = true;
	}

	public virtual void Update()
	{
		if (_update)
		{
			_update = false;
			Apply();
		}
	}

	public void invalidate()
	{
		_update = true;
	}

	public void PixelPerfect()
	{
		if (!(_texture == null))
		{
			_width = _texture.width;
			_height = _texture.height;
			Apply();
		}
	}

	public void Apply()
	{
		if (_visible != base.gameObject.activeSelf)
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
		GameObject gameObject = base.gameObject;
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.castShadows = false;
			meshRenderer.receiveShadows = false;
		}
		if (_material == null)
		{
			return;
		}
		meshRenderer.sharedMaterial = _material;
		meshRenderer.sharedMaterial.SetTexture("_MainTex", _texture);
		Texture mainTexture = _material.mainTexture;
		if (mainTexture == null)
		{
			return;
		}
		float width = _width;
		float height = _height;
		float x = 0f;
		float y = 0f;
		float x2 = 1f;
		float y2 = 1f;
		Mesh sharedMesh = meshFilter.sharedMesh;
		if (sharedMesh == null)
		{
			meshFilter.mesh = new Mesh();
			sharedMesh = meshFilter.sharedMesh;
		}
		sharedMesh.Clear();
		if (_alignHorizontal == AlignHorizontalEnum.AlignCenter)
		{
			float num = width / 2f;
			_vsx = 0f - num;
			_vex = num;
		}
		if (_alignHorizontal == AlignHorizontalEnum.AlignRight)
		{
			_vsx = 0f - width;
			_vex = 0f;
		}
		if (_alignHorizontal == AlignHorizontalEnum.AlignLeft)
		{
			_vsx = 0f;
			_vex = width;
		}
		if (_alignVertical == AlignVerticalEnum.AlignMiddle)
		{
			float num2 = height / 2f;
			_vsy = 0f - num2;
			_vey = num2;
		}
		if (_alignVertical == AlignVerticalEnum.AlignBottom)
		{
			_vsy = 0f - height;
			_vey = 0f;
		}
		if (_alignVertical == AlignVerticalEnum.AlignTop)
		{
			_vsy = 0f;
			_vey = height;
		}
		sharedMesh.vertices = new Vector3[4]
		{
			new Vector3(_vsx, _vey, 0f),
			new Vector3(_vex, _vey, 0f),
			new Vector3(_vex, _vsy, 0f),
			new Vector3(_vsx, _vsy, 0f)
		};
		sharedMesh.normals = new Vector3[4]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		sharedMesh.uv = new Vector2[4]
		{
			new Vector2(x, y2),
			new Vector2(x2, y2),
			new Vector2(x2, y),
			new Vector2(x, y)
		};
		sharedMesh.colors = new Color[4] { _color, _color, _color, _color };
		sharedMesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		sharedMesh.RecalculateBounds();
		sharedMesh.Optimize();
		if (_useCollider)
		{
			BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.size = new Vector3(_width, _height, 1f);
			boxCollider.isTrigger = true;
		}
		else
		{
			Object.DestroyImmediate(gameObject.GetComponent<BoxCollider>());
		}
	}
}
