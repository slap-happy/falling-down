using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Wallyworld
{
	[InitializeOnLoad]
	public class GoGoGrouper
	{	
		#region Private
		private static Transform sampleTransform;
		
		[MenuItem ("Wallyworld/Group Selected GameObjects %g")]
		static void OnGroupAsChild()
		{
			// Get selected objects and a reference to the first transform
			GameObject[] rawSelection = Selection.gameObjects;
			if (rawSelection == null || rawSelection.Length == 0)
				return;
			
			Transform selection = rawSelection[0].transform;
			
			// Ungroup single selections
			if (rawSelection.Length == 1 && IsGroup(rawSelection[0]))
			{
				List<GameObject> ungroupedObjects = new List<GameObject>();
				while (selection.childCount > 0)
				{
					GameObject child = selection.GetChild(0).gameObject;
					if (!ungroupedObjects.Contains(child))
						ungroupedObjects.Add(child);
					child.transform.parent = selection.parent;
				}
				GameObject.DestroyImmediate(selection.gameObject);
				Selection.objects = ungroupedObjects.ToArray();
			}
			
			// Group multiple selections
			else if (rawSelection.Length > 1)
			{
				GameObject group = new GameObject("New Group");
				group.transform.position = selection.position;
				group.transform.rotation = selection.rotation;
				group.transform.parent = selection.parent;
				
				group.AddComponent<GroupObject>().hideFlags = HideFlags.HideInInspector;
				foreach (GameObject go in Selection.gameObjects)
					go.transform.parent = group.transform;
				Selection.activeGameObject = group;
			}
		}
		
		[MenuItem("Wallyworld/Copy Transform")]
		static void CopyTransform()
		{
			GameObject selected = Selection.activeGameObject;
			if (selected != null)
				sampleTransform = selected.transform;
		}
		
		[MenuItem("Wallyworld/Paste Transform ")]
		static void PasteTransform()
		{
			GameObject[] selected = Selection.gameObjects;
			if (selected != null)
			{
				foreach (GameObject go in selected)
				{
					go.transform.position = sampleTransform.position;
					go.transform.localScale = sampleTransform.localScale;
					go.transform.rotation = sampleTransform.rotation;
				}
			}
		}
		
		static void RemoveGroupObject(GameObject go)
		{
			Component.DestroyImmediate(go.GetComponent<GroupObject>());
		}
		
		static bool IsGroup(GameObject sample)
		{
			return sample.GetComponent<GroupObject>() != null;
		}
		#endregion
		
		#region Menu Item Validation
		[MenuItem ("Wallyworld/Group Selected GameObjects %g", true)]
		static bool OnValidateGroupAsChild()
		{
			return Selection.gameObjects.Length > 0;
		}
		#endregion
	}
	
	public class GroupObject : MonoBehaviour{}
}