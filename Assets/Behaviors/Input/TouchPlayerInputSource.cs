using UnityEngine;
using System;

public class PlayerTouches {
	public int count {
		get { return _count; }
	}

	/**
	 * Whether there was an upwards swipe.
	 */
	public bool hasSwipe {
		get {
			if (_count > 1) {
				return touches[1].deltaPosition.y > swipeThreshold;
			}
			else if (_count > 0) {
				return touches[0].deltaPosition.y > swipeThreshold;
			}
			else {
				return false;
			}
		}
	}

	/**
	 * Position of a single touch or the midpoint between two touches.
	 */
	public float position {
		get {
			if (_count > 1) {
				return (touches[0].position.x + touches[1].position.x) / 2;
			}
			else if (_count > 0) {
				return touches[0].position.x;
			}
			else {
				return 0;
			}
		}
	}

	/**
	 * Calculates a virtual-cursor position between -1 and 1 from the given
	 * touch relative to the total screen width.
	 */
	public float relativePosition {
		get { return (position - (Screen.width / 2)) / Screen.width; }
	}

	/**
	 * Spread factor of two touches.
	 */
	public float spread {
		get { return (_count > 1) ? Math.Abs(touches[0].position.x - touches[1].position.x) / Screen.width : 0; }
	}

	private Touch[] touches;
	private int _count;

	private float swipeThreshold = 5;

	public PlayerTouches(int max) {
		touches = new Touch[max];
	}

	public int Add(Touch touch) {
		touches[_count] = touch;
		return _count++;
	}
}

public class TouchPlayerInputSource : PlayerInputSource {

	/**
	 * Upper bound for the time between multi-taps.
	 */
	public float multiTapThreshold = 0.1f;

	/**
	 * Polls touch-screen state and returns a resulting PlayerInput.
	 */
	public override PlayerInput PollForInput(PlayerInput previous) {
		PlayerTouches touches = new PlayerTouches(Input.touchCount);

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began && touch.tapCount >= 2 && touch.deltaTime < multiTapThreshold) {
				touches.Add(touch);
				return new PlayerInput(touches.relativePosition, 0, PlayerInput.Action.Roll);
			}
			else if (touch.phase != TouchPhase.Ended || touch.phase != TouchPhase.Canceled) {
				if (touches.Add(touch) == 2) {
					break;
				}
			}
		}

		if (touches.count < 1) {
			return null;
		}
		else {
			return new PlayerInput(
				touches.relativePosition,
				touches.spread,
				touches.hasSwipe ? PlayerInput.Action.Brake : PlayerInput.Action.None
			);
		}
	}
}
