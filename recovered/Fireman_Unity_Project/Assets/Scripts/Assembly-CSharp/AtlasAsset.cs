using System;
using System.IO;
using Spine;
using UnityEngine;

public class AtlasAsset : ScriptableObject
{
	public TextAsset atlasFile;

	public Material material;

	private Atlas atlas;

	public void Clear()
	{
		atlas = null;
	}

	public Atlas GetAtlas()
	{
		if (atlasFile == null)
		{
			Debug.LogWarning("Atlas file not set for atlas asset: " + base.name, this);
			Clear();
			return null;
		}
		if (material == null)
		{
			Debug.LogWarning("Material not set for atlas asset: " + base.name, this);
			Clear();
			return null;
		}
		if (atlas != null)
		{
			return atlas;
		}
		try
		{
			atlas = new Atlas(new StringReader(atlasFile.text), string.Empty, new SingleTextureLoader(material));
			return atlas;
		}
		catch (Exception ex)
		{
			Debug.Log("Error reading atlas file for atlas asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
			return null;
		}
	}
}
