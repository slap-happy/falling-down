using UnityEngine;
using System.Collections;

[System.Serializable]
public struct DynamicHazardArgs
{
	public Vector3 toDirection;
	public ForceMode forceMode;
	public float force;
}

[RequireComponent(typeof(SphereCollider))]
public class DynamicHazardSpawner : MonoBehaviour
{
	public enum SpawnCondition { OnTriggerEnter, OnTriggerExit, OnBecameVisible }
	const string TO_DIRECTION_REF_NAME = "toDirectionRef";
	
	#region Attributes
	public SpawnCondition spawnCondition = SpawnCondition.OnTriggerEnter;
	public Hazard hazardPrefab;
	public ForceMode forceModeToUse;
	public float forceToUse;
	public float radius = 10;
	#endregion
	
	#region Unity
	void Awake()
	{
		(collider as SphereCollider).radius = radius;
		cachedHazard = (GameObject.Instantiate(hazardPrefab.gameObject, transform.position, transform.rotation) as GameObject).GetComponent<Hazard>();
		cachedHazard.gameObject.SetActive(false);
		toDirectionRef = transform.FindChild(TO_DIRECTION_REF_NAME);
	}
	
	void Reset()
	{
		Transform t = transform.FindChild(TO_DIRECTION_REF_NAME);
		if (t == null)
		{
			GameObject go = new GameObject(TO_DIRECTION_REF_NAME);
			t = go.transform;
			t.parent = transform;
			t.localPosition = Vector3.zero;
		}
		toDirectionRef = t;
	}
	
	void OnTriggerEnter()
	{
		if (spawnCondition == SpawnCondition.OnTriggerEnter)
			Activate();
	}
	
	void OnTriggerExit()
	{
		if (spawnCondition == SpawnCondition.OnTriggerExit)
			Activate();
	}
	
	void OnBecameVisible()
	{
		if (spawnCondition == SpawnCondition.OnBecameVisible)
			Activate();
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, radius);
		if (toDirectionRef != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.position, toDirectionRef.position);
			Gizmos.color = Color.white;
			Gizmos.DrawSphere(toDirectionRef.position, 0.25f);
		}
	}
	#endregion
	
	#region Private
	private Transform toDirectionRef;
	private Hazard cachedHazard;
	
	void Activate()
	{
		cachedHazard.ActivateWithArgs(new DynamicHazardArgs
		{
			toDirection = toDirectionRef.position,
			forceMode = forceModeToUse,
			force = forceToUse,
			
		});
		gameObject.SetActive(false);
	}
	#endregion
}
