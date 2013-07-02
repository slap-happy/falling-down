using UnityEngine;

public class BasicTouchCursor : MonoBehaviour {

	public float speed = 1f;

	/**
	 * Moves the cursor left/right along the x axis based on mouse input.
	 */
	private void Update() {
		if (Input.touches.Length > 0)
			transform.Translate(Input.touches[0].deltaPosition.x * speed * -1, 0, 0);
	}
}
