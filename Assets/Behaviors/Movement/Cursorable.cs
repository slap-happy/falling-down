using UnityEngine;

public class Cursorable : MonoBehaviour {

	public Transform cursor;
	public float damping = 3.0f;
	public float rotationDamping = 15.0f;

	/**
	 * Fixed distance away from object on the y-axis.
	 */
	private float distance;

	/**
	 * Calculates lead distance.
	 */
	private void Start() {
		distance = transform.position.y - cursor.position.y;
	}

	private void LateUpdate() {
		// Keep cursor ahead of us (at our fixed distance on y)
		cursor.position = new Vector3(cursor.position.x, transform.position.y - distance, cursor.position.z);

		// Translate our position toward the cursor (on x only)
		Vector3 desiredPosition = new Vector3(cursor.position.x, transform.position.y, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
		
		// Rotate towards cursor
		Quaternion desiredRotation = Quaternion.LookRotation(Vector3.forward, -1 * (cursor.position - transform.position));
		transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamping);
	}
}

