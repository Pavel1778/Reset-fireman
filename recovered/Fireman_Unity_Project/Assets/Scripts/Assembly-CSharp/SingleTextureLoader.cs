using Spine;
using UnityEngine;

public class SingleTextureLoader : TextureLoader
{
	private Material material;

	public SingleTextureLoader(Material material)
	{
		this.material = material;
	}

	public void Load(AtlasPage page, string path)
	{
		page.width = material.mainTexture.width;
		page.height = material.mainTexture.height;
	}

	public void Unload(object texture)
	{
	}
}
