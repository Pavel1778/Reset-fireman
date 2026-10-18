using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	private static Spawner instance;

	private int preloadMin = 15;

	private List<Queue<GameObject>> queue = new List<Queue<GameObject>>();

	private List<GameObject> listPrefab = new List<GameObject>();

	private static Spawner Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject gameObject = new GameObject("Default Spawner");
				instance = (Spawner)gameObject.AddComponent(typeof(Spawner));
				Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public static bool isInitPrefab(GameObject prefab)
	{
		return Instance.isInitPrefab2(prefab);
	}

	public static void InitPrefab(GameObject prefab)
	{
		Instance.initPrefab2(prefab);
	}

	public static void InitPrefab(GameObject prefab, int nb)
	{
		Instance.initPrefab2(prefab, nb);
	}

	public void initPrefab2(GameObject prefab)
	{
		initPrefab2(prefab, preloadMin);
	}

	public bool isInitPrefab2(GameObject prefab)
	{
		return listPrefab.Find((GameObject a) => a.name == prefab.name) != null;
	}

	public void initPrefab2(GameObject prefab, int nb)
	{
		int num = 0;
		for (num = 0; num < listPrefab.Count; num++)
		{
			if (listPrefab[num].name == prefab.name)
			{
				Debug.LogWarning("Prefab deja initialisé");
				return;
			}
		}
		queue.Add(new Queue<GameObject>());
		for (int i = 0; i < nb; i++)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.name = prefab.name;
			gameObject.SetActiveRecursively(false);
			gameObject.transform.parent = base.transform;
			queue[queue.Count - 1].Enqueue(gameObject);
		}
		listPrefab.Add(prefab);
	}

	public static void DestroyPrefab(GameObject prefab)
	{
		Instance.destroyPrefab2(prefab);
	}

	public void destroyPrefab2(GameObject prefab)
	{
		int num = 0;
		for (num = 0; num < listPrefab.Count && !(listPrefab[num].name == prefab.name); num++)
		{
		}
		if (num >= listPrefab.Count)
		{
			Debug.LogError("Prefab inexistant");
			return;
		}
		foreach (GameObject item in queue[num])
		{
			Object.Destroy(item);
		}
		listPrefab.RemoveAt(num);
		queue.RemoveAt(num);
	}

	public static void DestroyAllPrefab()
	{
		Instance.destroyAllPrefab2();
	}

	public void destroyAllPrefab2()
	{
		foreach (Queue<GameObject> item in queue)
		{
			foreach (GameObject item2 in item)
			{
				Object.Destroy(item2);
			}
		}
		queue.Clear();
		listPrefab.Clear();
	}

	public static GameObject Instantiate(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		return Instance.instantiate2(prefab, pos, rot);
	}

	public static GameObject Instantiate(GameObject prefab, Vector3 pos)
	{
		return Instance.instantiate2(prefab, pos, Quaternion.identity);
	}

	public static GameObject Instantiate(GameObject prefab)
	{
		return Instance.instantiate2(prefab, Vector3.zero, Quaternion.identity);
	}

	public GameObject instantiate2(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		int num = 0;
		for (num = 0; num < listPrefab.Count && !(listPrefab[num].name == prefab.name); num++)
		{
		}
		if (num >= listPrefab.Count)
		{
			Debug.LogError("Prefab inexistant");
			return null;
		}
		if (queue[num].Count == 0)
		{
			for (int i = 0; i < preloadMin; i++)
			{
				GameObject gameObject = Object.Instantiate(prefab);
				gameObject.name = prefab.name;
				gameObject.SetActiveRecursively(false);
				gameObject.transform.parent = base.transform;
				queue[num].Enqueue(gameObject);
			}
		}
		GameObject gameObject2 = queue[num].Dequeue();
		gameObject2.transform.parent = null;
		gameObject2.SetActiveRecursively(true);
		gameObject2.transform.position = pos;
		gameObject2.transform.rotation = rot;
		return gameObject2;
	}

	public static void Destroy(GameObject obj)
	{
		Instance.destroy2(obj);
	}

	public void destroy2(GameObject obj)
	{
		int num = 0;
		for (num = 0; num < listPrefab.Count && !(listPrefab[num].name == obj.name); num++)
		{
		}
		if (num >= listPrefab.Count)
		{
			Debug.LogError("Prefab inexistant");
			return;
		}
		obj.SetActiveRecursively(false);
		queue[num].Enqueue(obj);
		obj.transform.parent = base.transform;
	}
}
