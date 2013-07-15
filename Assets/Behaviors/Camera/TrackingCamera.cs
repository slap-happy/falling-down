using UnityEngine;

public class TrackingCamera : MonoBehaviour {

	public Transform target;

	/**
	 * Lerps the camera position towards the target's vertical position.
	 */
	void Update() {
		//transform.position = Vector3.Lerp(transform.position, TargetPosition(), 1);
		transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
	}

	Vector3 TargetPosition() {
		return new Vector3(transform.position.x, target.position.y, transform.position.z);
	}

	float DistanceFromTarget() {
		return Vector3.Distance(transform.position, target.position);
	}
}
