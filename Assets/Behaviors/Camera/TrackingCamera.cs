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
	void FixedUpdate()
	{
		Vector3 ourPosition = transform.position;
		Vector3 targetPosition = transform.position;
		targetPosition.y = target.position.y;
		
		transform.position = Vector3.SmoothDamp(ourPosition, targetPosition, ref currentVelocity, 0.5f);
		Debug.Log(currentVelocity);
		// experimental: causes the camera to pivot to face the target
		if (lookAtTarget)
			transform.LookAt(target);
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
	private Vector3 currentVelocity;
	private Transform target;
	#endregion
}
