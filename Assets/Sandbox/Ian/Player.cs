using UnityEngine;
using System;
using System.Collections;

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
	public GameObject dive;
	public GameObject normal;
	public GameObject flare;
	public Transform cameraTargetRef;
	#endregion
	
	private Quaternion originalRotation;
	
	/**
	 * Our current state.
	 */
	private State currentState = State.Dive;

	/**
	 * Our current relaxed state, taking into account how quickly we're turning.
	 */
	private State currentRelaxedState {
		get { return (cursor != null && Mathf.Abs(cursor.x - transform.position.x) > 15) ? State.Normal : State.Dive; }
	}

	/**
	 * The active posture.
	 */
	private GameObject posture;
	
	private enum State {
		Normal,
		Dive,
		Flare,
		RollLeft,
		RollRight,
		Braking,
		Dead
	}
	
	private bool warningHit = false;
	
	#region Unity
	void Awake()
	{
		instance = this;
		camera = Camera.mainCamera;

		if (cameraTargetRef != null)
		{
			TrackingCamera trackingCamera = camera.GetComponent<TrackingCamera>();
			if (trackingCamera != null)
				trackingCamera.SetPlayer(this);
		}

		posture = normal;
	}
	
	void OnEnable()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
		PlayerInputController.OnInput += HandleInputControllerOnInput;
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
	}
	
	void OnDestroy()
	{
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
	
	void LateUpdate()
	{
		Animate();
	}
		
	void Animate()
	{
		switch (currentState) {
			case State.RollLeft:
			case State.RollRight:
				BarrelRoll();
				break;
			case State.Braking:
				Brake();
				break;
			case State.Normal:
				SetPosture(normal);
				break;
			case State.Flare:
				SetPosture(flare);
				break;
			case State.Dive:
				SetPosture(dive);
				break;
		}
	}
	
	private void SetPosture(GameObject newPosture) {
		if (posture != newPosture) {
			newPosture.transform.position = posture.transform.position;
			newPosture.transform.rotation = posture.transform.rotation;

			posture = newPosture;

			normal.SetActive(normal == posture);
			flare.SetActive(flare == posture);
			dive.SetActive(dive == posture);
		}
	}

	void Brake() {
		bool brakeWasCanceled = !inputWasReceived || (inputWasReceived && (currentInput.action != PlayerInput.Action.Brake));

		if (brakeWasCanceled || (Time.time - brakingStart) >= jetDuration) {
			currentState = currentRelaxedState;
		}
		else {
			// TODO create a braking posture
			SetPosture(flare);
		}
	}
	
	private float totalRotation = 0;
	void BarrelRoll ()
	{
		// Roll "animation"
		float rotate = 360 * Time.deltaTime * 3;
		totalRotation += rotate;
		
		SetPosture(dive);
		
		if (currentState == State.RollLeft) {
			posture.transform.Rotate(0, -rotate, 0);
		}
		if (currentState == State.RollRight) {
			posture.transform.Rotate(0, rotate, 0);
		}
		
		if (totalRotation >= 360) {
			totalRotation = 0;
			currentState = currentRelaxedState;
			// Reset rotation in case he overrotated due to inexact math
			posture.transform.rotation = originalRotation;
			
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

	void FixedUpdate()
	{
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

		cursor = camera.ScreenToWorldPoint(new Vector3(relativeX, isBraking ? camera.pixelHeight : 0, cameraDistance));

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
		if (isBraking) {
			rigidbody.AddForce(posture.transform.up * jetForce, ForceMode.Acceleration);
		}
	}

	/**
	 * Adjusts drag according to input and current roll state.
	 */
	void ApplyDrag() {
		float dragFactor = 0f;

		if (isRolling) {
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
					if (!isBraking && jetsAvailable > 0) {
						jetsAvailable--;
						brakingStart = Time.time;
						currentState = State.Braking;
					}
					break;

				case PlayerInput.Action.Roll:
					originalRotation = posture.transform.rotation;
					currentState = currentInput.cursorPosition > 0 ? State.RollRight : State.RollLeft;
					break;

				default:

					if (inputWasReceived) {
						if (currentInput.flareMagnitude > 0.7f) {
							currentState = State.Flare;
						}
						else if (currentInput.flareMagnitude > 0.3f) {
							currentState = State.Normal;
						}
						else {
							currentState = currentRelaxedState;
						}
					}
					else {
						currentState = currentRelaxedState;
					}

					break;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
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
	
	#region Properties
	public static Player Instance
	{
		get
		{
			return instance;
		}
	}

	public bool isBraking {
		get { return currentState == State.Braking; }
	}

	public bool isRolling {
		get { return currentState == State.RollLeft || currentState == State.RollRight; }
	}

	public float boost {
		get { return isRolling ? rollBoost : 1; }
	}
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (PlayerInput input)
	{
		// Don't handle new input at all while we're rolling
		if (!isRolling) {
			currentInput = input;
			inputWasReceived = true;
		}
	}

	void HandleGameControllerOnGameStarted()
	{
		rigidbody.velocity = Vector3.down * 15;
		rigidbody.drag = minDrag;
		currentInput = null;
		inputWasReceived = false;
	}
	
	void HandleGameControllerOnGameEnded()
	{
		
	}
	#endregion
	
	#region Private
	private static Player instance;
	
	private Action<CameraEffectArgs> cameraEffectsDelegate;
	private PlayerInput currentInput;
	private PlayerInput previousInput;
	private bool inputWasReceived;
	private Vector3 cursor;
	private Camera camera;
	private float brakingStart;
	#endregion
}
