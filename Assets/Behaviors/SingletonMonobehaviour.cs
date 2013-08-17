using UnityEngine;
using System.Collections;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
{
	private static T instance;
	
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(T)) as T;
				
				if (instance == null)
				{
					Debug.LogWarning(string.Format("No instance of {0} found; created a temporary one."));
					string rootName = string.Format("Root_{0}", Application.loadedLevelName);
					GameObject root = GameObject.Find(rootName);
					if (root == null)
						root = new GameObject(rootName, typeof(T));
					instance = root.GetComponent<T>();
					
					if (instance == null)
						Debug.LogError("Something went wrong.  Sorry, Charlie!");
				}
			}
			return instance;
		}
	}
}
