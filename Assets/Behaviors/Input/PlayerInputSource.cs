/**
 * An abstract class for translating device-specific input into PlayerInput.
 */
public abstract class PlayerInputSource {
	/**
	 * Returns PlayerInput translated from device-specific metrics. Implement in
	 * derived device-specific controllers.
	 */
	public abstract PlayerInput PollForInput(PlayerInput previous);
}
