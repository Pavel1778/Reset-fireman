using System;
using System.IO;
using Spine;
using UnityEngine;

public class SkeletonDataAsset : ScriptableObject
{
	public AtlasAsset atlasAsset;

	public TextAsset skeletonJSON;

	public float scale = 1f;

	public string[] fromAnimation;

	public string[] toAnimation;

	public float[] duration;

	private SkeletonData skeletonData;

	private AnimationStateData stateData;

	public void Clear()
	{
		skeletonData = null;
		stateData = null;
	}

	public SkeletonData GetSkeletonData(bool quiet)
	{
		if (atlasAsset == null)
		{
			if (!quiet)
			{
				Debug.LogWarning("Atlas not set for skeleton data asset: " + base.name, this);
			}
			Clear();
			return null;
		}
		if (skeletonJSON == null)
		{
			if (!quiet)
			{
				Debug.LogWarning("Skeleton JSON file not set for skeleton data asset: " + base.name, this);
			}
			Clear();
			return null;
		}
		Atlas atlas = atlasAsset.GetAtlas();
		if (atlas == null)
		{
			Clear();
			return null;
		}
		if (skeletonData != null)
		{
			return skeletonData;
		}
		SkeletonJson skeletonJson = new SkeletonJson(atlas);
		skeletonJson.Scale = scale;
		try
		{
			skeletonData = skeletonJson.ReadSkeletonData(new StringReader(skeletonJSON.text));
		}
		catch (Exception ex)
		{
			if (!quiet)
			{
				Debug.Log("Error reading skeleton JSON file for skeleton data asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
			}
			return null;
		}
		stateData = new AnimationStateData(skeletonData);
		int i = 0;
		for (int num = fromAnimation.Length; i < num; i++)
		{
			if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
			{
				stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
			}
		}
		return skeletonData;
	}

	public AnimationStateData GetAnimationStateData()
	{
		if (stateData != null)
		{
			return stateData;
		}
		GetSkeletonData(false);
		return stateData;
	}
}
