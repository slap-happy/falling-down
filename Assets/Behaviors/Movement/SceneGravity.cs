using UnityEngine;

public class SceneGravity : MonoBehaviour {

	public Vector3 force;

	private void Start() {
		Physics.gravity = force;
	}
}
