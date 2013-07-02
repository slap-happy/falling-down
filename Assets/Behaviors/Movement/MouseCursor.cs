using UnityEngine;

public class MouseCursor : MonoBehaviour {

	public float speed = 1f;

	/**
	 * Moves the cursor left/right along the x axis based on mouse input.
	 */
	private void Update() {
		transform.Translate(Input.GetAxis("Mouse X") * speed * -1, 0, 0);
	}
}
