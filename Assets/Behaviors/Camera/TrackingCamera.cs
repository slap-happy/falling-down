using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour
{
	#region Attributes
	public bool lookAtTarget;
	#endregion
	
	#region Properties
	/**
	 * Provides the distance between the player and the camera as a value between 0 and 1 to be used as a lerp.
	 */
	float DistanceFromTarget(Vector3 ourPosition, Vector3 target, bool clampValue)
	{
		float distance = Vector3.Distance(ourPosition, target);
		if (clampValue)
			distance = Mathf.Clamp(distance, 0, 1);
		return distance;
	}
	#endregion
	
	#region Actions
	/**
	 * Sets the camera's follow target
	 */
	public void SetTarget(Transform target)
	{
		StartCoroutine(TrackTarget(target));
	}
	#endregion
	
	#region Private
	/**
	 * Lerps the camera position towards the target's vertical position by the distance from the target.
	 */
	IEnumerator TrackTarget(Transform target)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("Began tracking target, '{0}'.", target.name));
		
		while (target != null)
		{
			Vector3 ourPosition = transform.position;
			Vector3 targetPosition = transform.position;
			targetPosition.y = target.position.y;
			transform.position = Vector3.Lerp(ourPosition, targetPosition, DistanceFromTarget(ourPosition, target.position, true));
			if (lookAtTarget)
				transform.LookAt(target);	// experimental property to allow the camera to pivot toward the target
			yield return null;
		}
	}
	#endregion
}
