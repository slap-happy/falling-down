using UnityEngine;

public class InputDebugger : MonoBehaviour {
	void Awake() {
		PlayerInputController.OnInput += delegate (PlayerInput input) {
			Debug.Log(
					"Input(cursorPosition: " + input.cursorPosition +
					" flareMagnitude: " + input.flareMagnitude +
					" action: " + input.action
			);
		};
	}
}
