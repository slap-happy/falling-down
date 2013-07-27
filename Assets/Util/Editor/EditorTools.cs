using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorTools
{
	[MenuItem("Tools/Enalbe Or Disable Selected GameObjects #%d")]
	static void EnableDisableSelectedGameObjects()
	{
		GameObject[] gos = Selection.gameObjects;
		if (gos.Length > 0)
		{
			Undo.RegisterUndo(gos, "Toggle Selected GameObjects");
			foreach (GameObject go in gos)
				go.SetActive(!go.activeSelf);
		}
	}
}
