using UnityEngine;

public class TouchCursor : MonoBehaviour {

	public float speed = 1f;

	public float screenWidth = 240f;

	private Transform leftLight;
	private Transform rightLight;

	private void Start() {
		leftLight = transform.Find("TouchL");
		rightLight = transform.Find("TouchR");

		leftLight.light.enabled = false;
		rightLight.light.enabled = false;
	}

	/**
	 * Moves the cursor left/right along the x axis based on mouse input.
	 */
	private void Update() {
		if (Input.touches.Length > 0) {
			if (Input.touches.Length > 1) {
				Touch ltouch;
				Touch rtouch;

				if (Input.touches[0].position.x > Input.touches[1].position.x) {
					ltouch = Input.touches[1];
					rtouch = Input.touches[0];
			 	}
				else {
					ltouch = Input.touches[0];
					rtouch = Input.touches[1];
				}

				Debug.Log("Touched left at " + ltouch.position.x);
				Debug.Log("Touched right at " + rtouch.position.x);

				float lpoint = NormalizePosition(ltouch.position.x);
				float rpoint = NormalizePosition(rtouch.position.x);

				leftLight.position = new Vector3(lpoint, leftLight.position.y, leftLight.position.z);
				rightLight.position = new Vector3(rpoint, rightLight.position.y, rightLight.position.z);

				leftLight.light.enabled = true;
				rightLight.light.enabled = true;

				//transform.Translate((rpoint - lpoint) * speed * -1, 0, 0);
			}
			else {
				Touch touch = Input.touches[0];
				Debug.Log("Touched at " + touch.position.x);

				float point = NormalizePosition(touch.position.x);

				leftLight.position = new Vector3(point, leftLight.position.y, leftLight.position.z);

				leftLight.light.enabled = true;
				rightLight.light.enabled = false;

				//transform.Translate(point * speed * -1, 0, 0);
			}
		}
	}

	/**
	 * Translates the absolute x position of the touch to a point relative to
	 * the visible touch bar.
	 */
	private float NormalizePosition(float x) {
		float width = screenWidth / 2;
		return transform.localScale.x * ((x - width) / width) * -1;
	}
}
