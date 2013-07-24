using UnityEngine;

public class KeyboardPlayerInputSource : PlayerInputSource {

	/**
	 * The increment at which the keyboard left/right moves the cursor (between
	 * -1 and 1).
	 */
	public float cursorMovement = 0.014f;

	/**
	 * The rate at which the cursor returns to 0 while there's no input.
	 */
	public float cursorReturn { get { return cursorMovement * 3; } }

	/**
	 * Delay limit for double taps.
	 */
	public float doubleTapSpeed = 0.5f;

	/**
	 * Timestamps to keep track of doubletaps.
	 */
	private float lastTapTimeLeft = 0;
	private float lastTapTimeRight = 0;
	private float lastTapTimeUp = 0;

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
				horizontal = -1;
			}

			lastTapTimeLeft = Time.time;
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			if ((Time.time - lastTapTimeRight) < doubleTapSpeed) {
				action = PlayerInput.Action.Roll;
				horizontal = 1;
			}

			lastTapTimeRight = Time.time;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			if ((Time.time - lastTapTimeUp) < doubleTapSpeed) {
				action = PlayerInput.Action.Brake;
			}

			lastTapTimeUp = Time.time;
		}

		// Immediately move left or right of center a ways when starting a roll
		if (action == PlayerInput.Action.Roll) {
			return new PlayerInput(horizontal * cursorMovement * 30, 0, action);
		}
		else if (horizontal != 0 || vertical != 0 || action != PlayerInput.Action.None) {
			float multiplier = (action == PlayerInput.Action.Roll) ? 30 : 1;

			return new PlayerInput(
				previous.cursorPosition + (horizontal * cursorMovement * multiplier),
				vertical > 0 ? 1f : 0,
				action
			);
		}

		return null;
	}
}
