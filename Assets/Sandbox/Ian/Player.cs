using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Faller))]
[RequireComponent(typeof(NetworkView))]

public class Player : MonoBehaviour
{
	#region Events
	public delegate void HitHazard(float relativeVelocity);
	
	public HitHazard OnHitHazard;
	#endregion
	
	#region Attributes
	public float jetForce = 50f;
	public float jetDuration = 2;
	public float jetsAvailable = 3;
	public float rollBoost = 3f;
	public float rollPenaltyFactor = 0.6f;
	public float minDrag;
	public float maxDrag;
	public float terminalVelocity;
	public Transform cameraTargetRef;
	#endregion

	#region Unity
	void Awake() {
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;

		if (networkView.isMine) {
			character = GetComponent<Faller>();

			camera = Camera.mainCamera;

			if (cameraTargetRef != null) {
				TrackingCamera trackingCamera = camera.GetComponent<TrackingCamera>();
				if (trackingCamera != null)
					trackingCamera.SetPlayer(this);
			}
		}
	}
	
	void OnDestroy() {
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
		PlayerInputController.OnInput -= HandleInputControllerOnInput;
	}
	
	void OnGUI () {
		if (warningHit == true) {
			float width = 150;
			float height = 25;
			float left = (Screen.width / 2) - (width / 2);
			GUI.Box(new Rect(left, 10, width, height), "Prepare for landing!");	
		}

		GUI.Label(new Rect(Screen.width - 85, 10, 75, 20), jetsAvailable + " jets left");
	}

	/**
	 * Remove ourself and the rigidbody from instances created on other hosts.
	 */
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		if (!networkView.isMine) {
			Destroy(rigidbody);
			Destroy(this);
		}
	}

	void LateUpdate() {
		Animate();
	}

	void Animate() {
		switch (character.state) {
			case Faller.State.Braking:
				Brake();
				break;
			case Faller.State.RollingLeft:
			case Faller.State.RollingRight:
				BarrelRoll();
				break;
		}
	}

	void FixedUpdate() {
		MoveTowardCursor();
		ApplyBrakes();
		ApplyDrag();
		PerformActions();

		if (terminalVelocity > 0 && rigidbody.velocity.y < -terminalVelocity)
		{
			if (Debug.isDebugBuild)
				Debug.Log("Terminal velocity cap met.");
			Vector3 newVelocity = rigidbody.velocity;
			newVelocity.y = -terminalVelocity;
			rigidbody.velocity = newVelocity;
		}

		inputWasReceived = false;
	}

	/**
	 * Moves towards the virtual cursor according to player input. The cursor
	 * is relative to the camera screen, at the bottom normally or at the top if
	 * braking.
	 */
	void MoveTowardCursor() {
		// Calculate new cursor position, relative to the camera screen and our
		// current input
		float midwidth = camera.pixelWidth / 2;
		float relativeX = (currentInput == null) ? midwidth : (midwidth + (currentInput.cursorPosition * midwidth));
		float cameraDistance = Mathf.Abs(camera.transform.position.z - transform.position.z);

		cursor = camera.ScreenToWorldPoint(new Vector3(relativeX, character.isBraking ? camera.pixelHeight : 0, cameraDistance));

		if (Debug.isDebugBuild) {
			Debug.DrawLine(transform.position, cursor);
		}

		Vector3 desiredPosition = new Vector3(cursor.x, transform.position.y, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * boost);

		Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -1 * (cursor - transform.position));
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 15f);
	}

	/**
	 * Applies braking force.
	 */
	void ApplyBrakes() {
		if (character.isBraking) {
			rigidbody.AddForce(character.posture.transform.up * jetForce, ForceMode.Acceleration);
		}
	}

	/**
	 * Adjusts drag according to input and current roll state.
	 */
	void ApplyDrag() {
		float dragFactor = 0f;

		if (character.isRolling) {
			dragFactor = rollPenaltyFactor;
		}
		else if (inputWasReceived) {
			dragFactor = currentInput.flareMagnitude;
		}

		rigidbody.drag = minDrag + ((maxDrag - minDrag) * dragFactor);
	}

	/**
	 * Handles any input actions and adjusts player state.
	 */
	void PerformActions() {
		if (inputWasReceived) {
			switch (currentInput.action) {
				case PlayerInput.Action.Brake:
					if (!character.isBraking && jetsAvailable > 0) {
						jetsAvailable--;
						brakingStart = Time.time;
						character.Brake();
					}
					break;

				case PlayerInput.Action.Roll:
					originalRotation = character.posture.transform.rotation;

					if (currentInput.cursorPosition > 0)
						character.RollRight();
					else
						character.RollLeft();

					break;

				default:

					if (inputWasReceived) {
						if (currentInput.flareMagnitude > 0.7f) {
							character.Flare();
						}
						else if (currentInput.flareMagnitude > 0.3f) {
							character.Relax();
						}
						else {
							Relax();
						}
					}
					else {
						Relax();
					}

					break;
			}
		}
	}

	/**
	 * Sets the character state to the current relaxed state.
	 */
	void Relax() {
		if (Mathf.Abs(cursor.x - transform.position.x) > 15)
			character.Relax();
		else
			character.Dive();
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.transform.tag == "Hazard")
		{
			if (OnHitHazard != null)
				OnHitHazard(collision.relativeVelocity.y);
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.transform.tag == "Warning Line") {
			warningHit = true;
		}

		if (other.transform.tag == "Finish Line") {
			// Check speed
			// if too fast, squish death
			// If good speed, show popup cube with score. Freeze player position
		}
	}
	#endregion

	#region Animations
	private float totalRotation = 0;
	private Quaternion originalRotation;

	void BarrelRoll () {
		// Roll "animation"
		float rotate = 360 * Time.deltaTime * 3;
		totalRotation += rotate;

		if (character.isRolling) {
			character.posture.transform.Rotate(0, character.isRollingLeft ? -rotate : rotate, 0);
		}
		
		if (totalRotation >= 360) {
			totalRotation = 0;
			Relax();

			// Reset rotation in case he overrotated due to inexact math
			character.posture.transform.rotation = originalRotation;

			// Mostly kill his sideways velocity.
			Vector3 newVelocity = rigidbody.velocity;
			if (rigidbody.velocity.x > 0) {
				newVelocity.x = 5;
			} else if (rigidbody.velocity.x < 0) {
				newVelocity.x = -5;
			}
			rigidbody.velocity = newVelocity;
		}
	}

	void Brake() {
		bool canceled = !inputWasReceived || (inputWasReceived && (currentInput.action != PlayerInput.Action.Brake));
		if (canceled || (Time.time - brakingStart) >= jetDuration)
			Relax();
	}
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (PlayerInput input) {
		// Don't handle new input at all while we're rolling
		if (!character.isRolling) {
			currentInput = input;
			inputWasReceived = true;
		}
	}

	void HandleGameControllerOnGameStarted() {
		gameObject.SetActive(true);

		PlayerInputController.OnInput += HandleInputControllerOnInput;

		rigidbody.velocity = Vector3.down * 15;
		rigidbody.drag = minDrag;
		currentInput = null;
		inputWasReceived = false;
	}

	void HandleGameControllerOnGameEnded() {
		PlayerInputController.OnInput -= HandleInputControllerOnInput;
	}
	#endregion
	
	#region Private
	private Faller character;
	private Action<CameraEffectArgs> cameraEffectsDelegate;
	private PlayerInput currentInput;
	private PlayerInput previousInput;
	private bool inputWasReceived;
	private Vector3 cursor;
	private Camera camera;
	private float brakingStart;
	private bool warningHit = false;

	private float boost {
		get { return character.isRolling ? rollBoost : 1; }
	}
	#endregion
}
