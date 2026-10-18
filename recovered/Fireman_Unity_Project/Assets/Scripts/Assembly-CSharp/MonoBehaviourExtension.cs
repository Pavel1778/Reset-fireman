using UnityEngine;

public static class MonoBehaviourExtension
{
	public static T New<T>(this MonoBehaviour sender, string name, Transform parent, Vector3 position) where T : MonoBehaviour
	{
		GameObject gameObject = new GameObject(name);
		Transform transform = gameObject.transform;
		transform.parent = parent;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		transform.localPosition = position;
		return gameObject.AddComponent<T>();
	}
}
