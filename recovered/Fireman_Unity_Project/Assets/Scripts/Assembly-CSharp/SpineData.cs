using UnityEngine;

public class SpineData : MonoBehaviour
{
	private static SpineData instance;

	public SkeletonDataAsset dataDoc;

	public SkeletonDataAsset koalaHome;

	public SkeletonDataAsset dataCoin;

	public SkeletonDataAsset dataTalky;

	public SkeletonDataAsset dataTank;

	public SkeletonDataAsset dataPet1;

	public SkeletonDataAsset dataPet2;

	public SkeletonDataAsset dataPet3;

	public SkeletonDataAsset dataPet4;

	public SkeletonDataAsset dataPet5;

	public SkeletonDataAsset dataPet6;

	public SkeletonDataAsset dataPet7;

	public SkeletonDataAsset dataPet8;

	public SkeletonDataAsset dataEnnemi1;

	public SkeletonDataAsset dataEnnemi2;

	public SkeletonDataAsset dataEnnemi3;

	public SkeletonDataAsset dataEnnemi5;

	public SkeletonDataAsset dataEnnemi6;

	public SkeletonDataAsset dataFireman;

	public SkeletonDataAsset dataProjectile;

	public static SpineData Instance
	{
		get
		{
			return instance;
		}
	}

	public void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
