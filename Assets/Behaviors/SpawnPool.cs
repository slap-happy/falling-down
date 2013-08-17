using UnityEngine;
using System.Collections;

public class SpawnPool : SingletonMonobehaviour<SpawnPool>
{
	public ObjectCache[] caches;
	public Hashtable activeCachedObjects;
	
	[System.Serializable]
	public class ObjectCache
	{
		public GameObject prefab;
		public int cacheSize = 10;
	
		private GameObject[] objects;
		private int cacheIndex;
	
		public void Initialize ()
		{
			objects = new GameObject[cacheSize];
	
			// Instantiate the objects in the array and set them to be inactive
			for (var i = 0; i < cacheSize; i++)
			{
				objects[i] = MonoBehaviour.Instantiate (prefab) as GameObject;
				objects[i].SetActive(false);
				objects[i].name = objects[i].name + i;
			}
		}
	
		public GameObject GetNextObjectInCache()
		{
			GameObject obj = null;
	
			// The cacheIndex starts out at the position of the object created
			// the longest time ago, so that one is usually free,
			// but in case not, loop through the cache until we find a free one.
			for (int i = 0; i < cacheSize; i++)
			{
				obj = objects[cacheIndex];
	
				// If we found an inactive object in the cache, use that.
				if (!obj.activeInHierarchy)
					break;
	
				// If not, increment index and make it loop around
				// if it exceeds the size of the cache
				cacheIndex = (cacheIndex + 1) % cacheSize;
			}
	
			// The object should be inactive. If it's not, log a warning and use
			// the object created the longest ago even though it's still active.
			if (obj.activeInHierarchy)
			{
				Debug.LogWarning ( "Spawn of " + prefab.name + " exceeds cache size of " + cacheSize + "! Reusing already active object.", obj);
				Destroy(obj);
			}
	
			// Increment index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
	
			return obj;
		}
	}
	
	void Awake()
	{
		// Total number of cached objects
		int amount = 0;
	
		// Loop through the caches
		for (var i = 0; i < caches.Length; i++)
		{
			// Initialize each cache
			caches[i].Initialize();
	
			// Count
			amount += caches[i].cacheSize;
		}
	
		// Create a hashtable with the capacity set to the amount of cached objects specified
		activeCachedObjects = new Hashtable (amount);
	}
	
	public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		ObjectCache cache = null;
	
		// Find the cache for the specified prefab
		for (var i = 0; i < caches.Length; i++)
			if (caches[i].prefab == prefab)
				cache = caches[i];
	
		// If there's no cache for this prefab type, just instantiate normally
		if (cache == null)
		{
			return Instantiate (prefab, position, rotation) as GameObject;
		}
	
		// Find the next object in the cache
		GameObject obj = cache.GetNextObjectInCache ();
	
		// Set the position and rotation of the object
		obj.transform.position = position;
		obj.transform.rotation = rotation;
	
		// Set the object to be active
		obj.SetActive(true);
		activeCachedObjects[obj] = true;
	
		return obj;
	}
	
	public void Destroy(GameObject objectToDestroy)
	{
		if (activeCachedObjects.ContainsKey(objectToDestroy))
		{
			objectToDestroy.SetActive(false);
			activeCachedObjects[objectToDestroy] = false;
		}
		else
			Destroy(objectToDestroy);
	}
}
