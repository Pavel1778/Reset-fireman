using System.Collections.Generic;
using Spine;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SkeletonComponent : MonoBehaviour
{
	public SkeletonDataAsset skeletonDataAsset;

	public Skeleton skeleton;

	public string initialSkinName;

	public float timeScale = 1f;

	private Mesh mesh;

	private Vector3[] vertices;

	private Color[] colors;

	private Vector2[] uvs;

	private int[] triangles;

	private float[] vertexPositions = new float[8];

	public virtual void Clear()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (component != null)
		{
			component.mesh = null;
		}
		if (mesh != null)
		{
			Object.DestroyImmediate(mesh);
			mesh = null;
		}
		if (GetComponent<Renderer>() != null)
		{
			GetComponent<Renderer>().sharedMaterial = null;
		}
		if (skeleton != null)
		{
			skeleton = null;
		}
	}

	public virtual void Initialize()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component == null)
		{
			component = base.gameObject.AddComponent<MeshRenderer>();
			component.receiveShadows = false;
			component.castShadows = false;
		}
		mesh = new Mesh();
		meshFilter.mesh = mesh;
		mesh.name = "Skeleton Mesh";
		mesh.hideFlags = HideFlags.HideAndDontSave;
		vertices = new Vector3[0];
		skeleton = new Skeleton(skeletonDataAsset.GetSkeletonData(false));
		if (initialSkinName != null && initialSkinName.Length > 0)
		{
			skeleton.SetSkin(initialSkinName);
			skeleton.SetSlotsToSetupPose();
		}
	}

	public virtual void UpdateSkeleton()
	{
		skeleton.Update(Time.deltaTime * timeScale);
		skeleton.UpdateWorldTransform();
	}

	public virtual void Update()
	{
		if (skeletonDataAsset == null || skeletonDataAsset.GetSkeletonData(false) == null)
		{
			Clear();
			return;
		}
		if (skeleton == null || skeleton.Data != skeletonDataAsset.GetSkeletonData(false))
		{
			Initialize();
		}
		UpdateSkeleton();
		int num = 0;
		List<Slot> drawOrder = skeleton.DrawOrder;
		int i = 0;
		for (int count = drawOrder.Count; i < count; i++)
		{
			Slot slot = drawOrder[i];
			Attachment attachment = slot.Attachment;
			if (attachment is RegionAttachment)
			{
				num++;
			}
		}
		if (num > vertices.Length / 4)
		{
			vertices = new Vector3[num * 4];
			colors = new Color[num * 4];
			uvs = new Vector2[num * 4];
			triangles = new int[num * 6];
			mesh.Clear();
			int j = 0;
			for (int num2 = num; j < num2; j++)
			{
				int num3 = j * 6;
				int num4 = j * 4;
				triangles[num3] = num4;
				triangles[num3 + 1] = num4 + 2;
				triangles[num3 + 2] = num4 + 1;
				triangles[num3 + 3] = num4 + 2;
				triangles[num3 + 4] = num4 + 3;
				triangles[num3 + 5] = num4 + 1;
			}
		}
		else
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			int k = num * 4;
			for (int num5 = vertices.Length; k < num5; k++)
			{
				vertices[k] = vector;
			}
		}
		float[] array = vertexPositions;
		int num6 = 0;
		Color color = default(Color);
		int l = 0;
		for (int count2 = drawOrder.Count; l < count2; l++)
		{
			Slot slot2 = drawOrder[l];
			Attachment attachment2 = slot2.Attachment;
			if (attachment2 is RegionAttachment)
			{
				RegionAttachment regionAttachment = (RegionAttachment)attachment2;
				regionAttachment.ComputeVertices(skeleton.X, skeleton.Y, slot2.Bone, array);
				int num7 = num6 * 4;
				vertices[num7] = new Vector3(array[0], array[1], 0f);
				vertices[num7 + 1] = new Vector3(array[6], array[7], 0f);
				vertices[num7 + 2] = new Vector3(array[2], array[3], 0f);
				vertices[num7 + 3] = new Vector3(array[4], array[5], 0f);
				color.a = skeleton.A * slot2.A;
				color.r = skeleton.R * slot2.R * color.a;
				color.g = skeleton.G * slot2.G * color.a;
				color.b = skeleton.B * slot2.B * color.a;
				colors[num7] = color;
				colors[num7 + 1] = color;
				colors[num7 + 2] = color;
				colors[num7 + 3] = color;
				float[] uVs = regionAttachment.UVs;
				uvs[num7] = new Vector2(uVs[0], 1f - uVs[1]);
				uvs[num7 + 1] = new Vector2(uVs[6], 1f - uVs[7]);
				uvs[num7 + 2] = new Vector2(uVs[2], 1f - uVs[3]);
				uvs[num7 + 3] = new Vector2(uVs[4], 1f - uVs[5]);
				num6++;
			}
		}
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		GetComponent<Renderer>().sharedMaterial = skeletonDataAsset.atlasAsset.material;
	}

	public virtual void OnEnable()
	{
		Update();
	}

	public virtual void OnDisable()
	{
		if (Application.isEditor)
		{
			Clear();
		}
	}

	public virtual void Reset()
	{
		Update();
	}
}
