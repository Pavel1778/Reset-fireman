using UnityEngine;

public class Fire : MonoBehaviour
{
	private class Anim
	{
		public Frame[] frames;
	}

	private class Frame
	{
		public Mesh mesh;

		public float time;

		public Frame(UIAtlas.Sprite sp, int OfsY)
		{
			mesh = createMesh(sp, OfsY);
			time = 0.08f;
		}
	}

	private static Material material;

	private static Anim animFire;

	private static Anim animAsh;

	private static bool init;

	public bool isAshes;

	private float nextFrame;

	private int frameCount;

	private Frame[] frames;

	private int frame;

	private MeshFilter mf;

	private MeshRenderer mr;

	public static Fire create()
	{
		GameObject gameObject = new GameObject("Fire");
		Fire result = gameObject.AddComponent<Fire>();
		gameObject.AddComponent<Ennemi1>();
		return result;
	}

	public void Start()
	{
		if (!init)
		{
			init = true;
			UIAtlas atlasFire = GameManager.Instance.AtlasFire;
			Texture2D texture = (Texture2D)atlasFire.texture;
			material = atlasFire.spriteMaterial;
			material.SetTexture("_MainTex", texture);
			animFire = new Anim();
			animFire.frames = new Frame[6];
			for (int i = 1; i <= animFire.frames.Length; i++)
			{
				animFire.frames[i - 1] = new Frame(atlasFire.GetSprite("Flamme0" + i), 20);
			}
			animAsh = new Anim();
			animAsh.frames = new Frame[5];
			for (int j = 1; j <= animAsh.frames.Length; j++)
			{
				animAsh.frames[j - 1] = new Frame(atlasFire.GetSprite("Cendre0" + j), 15);
			}
		}
		mf = base.gameObject.GetComponent<MeshFilter>();
		if (mf == null)
		{
			mf = base.gameObject.AddComponent<MeshFilter>();
		}
		mr = base.gameObject.GetComponent<MeshRenderer>();
		if (mr == null)
		{
			mr = base.gameObject.AddComponent<MeshRenderer>();
		}
		mr.sharedMaterial = material;
		SetFire();
		Ennemi1 component = GetComponent<Ennemi1>();
		component.mAnim = this;
		component.setEnnemi(base.transform.localPosition);
	}

	public void SetAsh()
	{
		frameCount = animAsh.frames.Length;
		nextFrame = Random.value * 0.08f;
		this.frame = Random.Range(0, animAsh.frames.Length);
		frames = animAsh.frames;
		Frame frame = frames[this.frame];
		mf.sharedMesh = frame.mesh;
		isAshes = true;
	}

	public void SetFire()
	{
		frameCount = animFire.frames.Length;
		nextFrame = Random.value * 0.08f;
		this.frame = Random.Range(0, animFire.frames.Length);
		frames = animFire.frames;
		Frame frame = frames[this.frame];
		mf.sharedMesh = frame.mesh;
		isAshes = false;
	}

	private void Update()
	{
		nextFrame -= Time.deltaTime;
		if (nextFrame <= 0f)
		{
			this.frame = (this.frame + 1) % frameCount;
			Frame frame = frames[this.frame];
			mf.sharedMesh = frame.mesh;
			nextFrame = frame.time;
		}
	}

	private static Mesh createMesh(UIAtlas.Sprite u, float OfsY)
	{
		Texture mainTexture = material.mainTexture;
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		int num7 = (int)u.inner.width;
		int num8 = (int)u.inner.height;
		int num9 = num7;
		int num10 = num8;
		int num11 = (int)u.inner.x;
		int num12 = (int)u.inner.y;
		float x = (float)num11 / (float)mainTexture.width;
		float y = 1f - (float)num12 / (float)mainTexture.height;
		float x2 = (float)(num11 + num9) / (float)mainTexture.width;
		float y2 = 1f - (float)(num12 + num10) / (float)mainTexture.height;
		num9 = (int)((float)((num <= 0) ? num9 : num) / 1.5f);
		num10 = (int)((float)((num2 <= 0) ? num10 : num2) / 1.5f);
		Mesh mesh = new Mesh();
		float num13 = num9 >> 1;
		num3 = 0f - num13;
		num4 = num13;
		float num14 = num10 >> 1;
		num5 = 0f - num14;
		num6 = num14;
		mesh.vertices = new Vector3[4]
		{
			new Vector3(num3, num6 + OfsY),
			new Vector3(num4, num6 + OfsY),
			new Vector3(num4, num5 + OfsY),
			new Vector3(num3, num5 + OfsY)
		};
		mesh.normals = new Vector3[4]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f)
		};
		mesh.uv = new Vector2[4]
		{
			new Vector2(x, y),
			new Vector2(x2, y),
			new Vector2(x2, y2),
			new Vector2(x, y2)
		};
		mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		mesh.RecalculateBounds();
		mesh.Optimize();
		return mesh;
	}
}
