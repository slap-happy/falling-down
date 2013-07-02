using UnityEngine;
using System.Collections;

public class TrackingCamera : MonoBehaviour {

	public Transform target;

	// Update is called once per frame
	void Update() {
		transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
	}
}
