using UnityEngine;
using System;

public class TouchPlayerInputSource : PlayerInputSource {

	/**
	 * Upper bound for the time between multi-taps.
	 */
	public float multiTapThreshold = 0.1f;

	/**
	 * Polls touch-screen state and returns a resulting PlayerInput.
	 */
	public override PlayerInput PollForInput(PlayerInput previous) {
		float primary = -1f;
		float secondary = -1f;

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Ended && touch.tapCount >= 2 && touch.deltaTime < multiTapThreshold) {
				return new PlayerInput(CursorPosition(touch.position.x), 0, PlayerInput.Action.Roll);
			}
			else if (touch.phase != TouchPhase.Ended || touch.phase != TouchPhase.Canceled) {
				if (primary == -1f) {
					primary = touch.position.x;
				}
				else if (secondary == -1f) {
					secondary = touch.position.x;
				}
				else {
					break;
				}
			}
		}

		if (primary == -1f && secondary == -1f) {
			return null;
		}

		float position = CursorPosition(primary);
		float flare = 0f;

		// If two touches are active, use their midpoint as the position and
		// calculate the flare based on distance apart
		if (secondary != -1f) {
			position = (position + CursorPosition(secondary)) / 2;
			flare = Math.Abs(primary - secondary) / Screen.width;
		}

		return new PlayerInput(position, flare);
	}

	/**
	 * Calculates a virtual-cursor position between -1 and 1 from the given
	 * touch relative to the total screen width.
	 */
	private float CursorPosition(float position) {
		return (position - (Screen.width / 2)) / Screen.width;
	}

}
