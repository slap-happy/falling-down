using UnityEngine;

public class TrackingCamera : MonoBehaviour {

	public Transform target;

	private Transform container;

	void Start() {
		container = FindContainer(transform);
	}

	// Update is called once per frame
	void Update() {
		container.position = new Vector3(container.position.x, target.position.y, container.position.z);
	}

	private Transform FindContainer(Transform transform) {
		return (transform.parent == null) ? transform : FindContainer(transform.parent);
	}
}
