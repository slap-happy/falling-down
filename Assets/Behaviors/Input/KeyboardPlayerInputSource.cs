using UnityEngine;

public class KeyboardPlayerInputSource : PlayerInputSource {

	/**
	 * The increment at which the keyboard left/right moves the cursor (between
	 * -1 and 1).
	 */
	public float cursorMovement = 0.007f;

	/**
	 * Delay limit for double taps.
	 */
	public float doubleTapSpeed = 0.5f;

	/**
	 * Timestamps to keep track of doubletaps.
	 */
	private float lastTapTimeLeft = 0;
	private float lastTapTimeRight = 0;

	/**
	 * Polls keyboard state and returns a resulting PlayerInput.
	 */
	public override PlayerInput PollForInput(PlayerInput previous) {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		PlayerInput.Action action = PlayerInput.Action.None;

		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if ((Time.time - lastTapTimeLeft) < doubleTapSpeed) {
				action = PlayerInput.Action.Roll;
			}

			lastTapTimeLeft = Time.time;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if ((Time.time - lastTapTimeRight) < doubleTapSpeed) {
				action = PlayerInput.Action.Roll;
			}

			lastTapTimeRight = Time.time;
		}

		if (horizontal != 0 || vertical != 0 || action != PlayerInput.Action.None) {
			return new PlayerInput(previous.cursorPosition + (horizontal * cursorMovement), vertical > 0 ? 1f : 0, action);
		}

		return null;
	}
}
