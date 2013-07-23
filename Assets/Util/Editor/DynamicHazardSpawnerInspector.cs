using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DynamicHazardSpawner))]
public class DynamicHazardSpawnerInspector : Editor
{
	DynamicHazardSpawner hazard;
	/*
	public override void OnInspectorGUI()
	{
		hazard = target as DynamicHazardSpawner;
		hazard.spawnCondition = EditorGUILayout.EnumPopup("Spawn Condition", hazard.spawnCondition);
	}
	*/
	void OnSceneGUI()
	{
		hazard = target as DynamicHazardSpawner;
		hazard.toDirectionRef.position = Handles.PositionHandle(hazard.toDirectionRef.position, Quaternion.LookRotation(hazard.toDirectionRef.position - hazard.transform.position));
	}
}
