using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour {

	public Transform target;

	// Update is called once per frame
	void LateUpdate () {
		Vector3 new_pos = transform.position;
		new_pos.y = target.position.y;
		transform.position = new_pos;
	}
}
