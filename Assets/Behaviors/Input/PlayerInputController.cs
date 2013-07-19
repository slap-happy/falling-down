using UnityEngine;
using SlapHappy;

/**
 * Polls input sources and sends the resulting player input to registered
 * delegates.
 */
public class PlayerInputController : MonoBehaviour {
	/**
	 * Delegates that are subscribed to input.
	 */
	public delegate void InputDelegate(PlayerInput input);
	public static event InputDelegate OnInput;

	/**
	 * Input from the previous cycle. This may be useful in cases where you'd
	 * need to compare timed input (double taps/clicks, etc.).
	 */
	protected PlayerInput previousInput;

	/**
	 * All input sources available.
	 */
	private PlayerInputSource[] sources;

	void Awake() {
		previousInput = new PlayerInput(0, 0);

		// Get all input sources that are defined
		sources = Reflector.InstantiateAll<PlayerInputSource>();
	}

	/**
	 * Polls for player input and passes it off to OnInput delegates.
	 */
	void Update() {
		if (OnInput != null) {
			foreach (PlayerInputSource source in sources) {
				PlayerInput input = source.PollForInput(previousInput);

				if (input != null) {
					OnInput(input);
					previousInput = input;
				}
			}
		}
	}
}
