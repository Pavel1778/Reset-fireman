using UnityEngine;

[ExecuteInEditMode]
public class AutoBackground : MonoBehaviour
{
	protected Color _color = Color.white;

	protected Material _material;

	protected Vector3[] _vertices;

	protected Vector3[] _normals;

	protected Vector2[] _uvs;

	protected int[] _tris;

	protected Color[] _cols;

	protected Mesh _mesh;

	protected MeshFilter _mf;

	protected MeshRenderer _mr;

	protected bool _update = true;

	protected int _width;

	protected int _height;

	protected float _vsx;

	protected float _vex;

	protected float _vsy;

	protected float _vey;

	protected float _forcedWidth;

	protected float _forcedHeight;

	[SerializeField]
	protected Camera _camera;

	[SerializeField]
	protected Shader _shader;

	[SerializeField]
	protected Texture2D _texture;

	[SerializeField]
	protected UIRoot _uiroot;

	public Camera CurrentCamera
	{
		get
		{
			return _camera;
		}
		set
		{
			if (!(_camera == value))
			{
				_camera = value;
				_update = true;
			}
		}
	}

	public UIRoot CurrentUIRoot
	{
		get
		{
			return _uiroot;
		}
		set
		{
			if (!(_uiroot == value))
			{
				_uiroot = value;
				_update = true;
			}
		}
	}

	public Texture2D CurrentTexture
	{
		get
		{
			return _texture;
		}
		set
		{
			if (!(_texture == value))
			{
				_texture = value;
				_update = true;
			}
		}
	}

	public Shader CurrentShader
	{
		get
		{
			return _shader;
		}
		set
		{
			if (!(_shader == value))
			{
				_shader = value;
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

	public void Start()
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
		_mr.receiveShadows = false;
		_mr.castShadows = false;
		_update = true;
	}

	public void Invalidate()
	{
		_update = true;
	}

	public void Update()
	{
		if (_update)
		{
			Apply();
			_update = false;
		}
	}

	public void Apply()
	{
		if (_camera != null && _texture != null && _shader != null && _uiroot != null)
		{
			_material = new Material(_shader);
			_material.SetTexture("_MainTex", _texture);
			createMesh(1f, 1f);
		}
	}

	private void createMesh(float w, float h)
	{
		Texture texture = _texture;
		_width = texture.width;
		_height = texture.height;
		float x = 0f;
		float y = 0f;
		float x2 = 1f;
		float y2 = 1f;
		w = 1f;
		h = 1f;
		float num = w * 0.5f;
		_vsx = 0f - num;
		_vex = num;
		float num2 = h * 0.5f;
		_vsy = 0f - num2;
		_vey = num2;
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
		_mr.sharedMaterial = _material;
		_mf.sharedMesh = _mesh;
		_update = false;
	}
}
