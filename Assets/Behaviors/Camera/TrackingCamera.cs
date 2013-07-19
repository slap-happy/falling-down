using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour
{
	#region Attributes
	public bool lookAtTarget;
	public float trackingSpeed = 0.5f;
	#endregion
	
	#region Unity
	/*
	 * Ensures that the component will be initialized to inactive.
	 */
	void Awake()
	{
		startingPosition = transform.position;
		enabled = false;
	}
	
	void OnEnable()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
	}
	
	/**
	 * Lerps the camera position towards the target's vertical position by the distance from the target.
	 */
	void FixedUpdate()
	{
		Vector3 ourPosition = transform.position;
		Vector3 targetPosition = target.position;
		targetPosition.x = startingPosition.x;
		targetPosition.z = startingPosition.z;
		
		transform.position = Vector3.SmoothDamp(ourPosition, targetPosition, ref currentVelocity, trackingSpeed);
		
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
	
	#region Handlers
	void HandleGameControllerOnGameStarted()
	{
		Vector3 currentPosition = transform.position;
		Vector3 placementPosition = new Vector3(currentPosition.x, target.position.y, currentPosition.z);
		transform.position = placementPosition;
		(cameraShake ?? (cameraShake = GetComponent<CameraShake>())).enabled = true;
		cameraShake.enabled = true;
	}
	#endregion
	
	#region Private
	private Vector3 startingPosition;
	private CameraShake cameraShake;
	private Vector3 currentVelocity;
	private Transform target;
	#endregion
}
