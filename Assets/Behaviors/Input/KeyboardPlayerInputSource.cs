using UnityEngine;

public class KeyboardPlayerInputSource : PlayerInputSource {

	/**
	 * The increment at which the keyboard left/right moves the cursor (between
	 * -1 and 1).
	 */
	public float cursorMovement = 0.001f;

	/**
	 * Polls keyboard state and returns a resulting PlayerInput.
	 */
	public override PlayerInput PollForInput(PlayerInput previous) {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		if (horizontal != 0 || vertical != 0) {
			return new PlayerInput(previous.cursorPosition + (horizontal * cursorMovement), vertical > 0 ? 1f : 0);
		}

		return null;
	}

}
