using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour
{
	#region Attributes
	public bool lookAtTarget;
	#endregion
	
	#region Unity
	/*
	 * Ensures that the component will be initialized to inactive.
	 */
	void Awake()
	{
		enabled = false;
	}
	
	/**
	 * Lerps the camera position towards the target's vertical position by the distance from the target.
	 */
	void Update()
	{
		Vector3 ourPosition = transform.position;
		Vector3 targetPosition = transform.position;
		targetPosition.y = target.position.y;
		
		float lerpValue = DistanceFromTarget(ourPosition, target.position, true);
		transform.position = Vector3.Lerp(ourPosition, targetPosition, lerpValue);
		
		// experimental: causes the camera to pivot to face the target
		if (lookAtTarget)
			transform.LookAt(target);
	}
	#endregion
	
	#region Properties
	/**
	 * Provides the distance between the player and the camera.
	 * If clampValue is true, returns a value between 0 and 1.
	 */
	float DistanceFromTarget(Vector3 ourPosition, Vector3 targetPosition, bool clampValue)
	{
		float distance = Vector3.Distance(ourPosition, targetPosition);
		if (clampValue)
			distance = Mathf.Clamp(distance, 0, 1);
		return distance;
	}
	#endregion
	
	#region Actions
	/**
	 * Sets the camera's follow target and enables the component.
	 */
	public void SetTarget(Transform target)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("Began tracking target, '{0}'.", target.name));
		this.target = target;
		enabled = true;
	}
	
	/**
	 * Unsets the target and disables the component.
	 */
	public void RemoveTarget()
	{
		enabled = false;
		target = null;
	}
	#endregion
	
	#region Private
	private Transform target;
	#endregion
}
