using UnityEngine;

/**
 * A simple structure that represents player input.
 */
public class PlayerInput {
	public enum Action { None, Roll }

	/**
	 * The action intended by this input.
	 */
	public Action action { get { return _action; } }
	private Action _action;

	/**
	 * The desired position of our virtual cursor on a single axis between -1
	 * and 1.
	 */
	public float cursorPosition { get { return _cursorPosition; } }
	private float _cursorPosition = 0f;

	/**
	 * Time difference from when this input was made and the current frame time.
	 */
	public float deltaTime { get { return Time.time - createdAt; } }

	/**
	 * A float with range 0 to 1 that represents the desired amount of flare.
	 */
	public float flareMagnitude { get { return _flareMagnitude; } }
	private float _flareMagnitude = 0f;

	/**
	 * Whether an action was intended by this input.
	 */
	public bool isAction { get { return action != Action.None; } }

	/**
	 * Time at which the input was made.
	 */
	public float createdAt { get { return _createdAt; } }
	private float _createdAt;

	/**
	 * Constructor. Takes a cursor position and flare magnitude as arguments.
	 * The position can be no less than -1 and no greater than 1.
	 */
	public PlayerInput(float position, float magnitude, Action action = Action.None) {
		_cursorPosition = Mathf.Clamp(position, -1f, 1f);
		_flareMagnitude = magnitude;
		_createdAt = Time.time;
		_action = action;
	}
}
