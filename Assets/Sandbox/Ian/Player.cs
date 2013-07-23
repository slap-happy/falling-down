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
	
	private State currentState = State.Normal;

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
	
	private float totalRotation = 0;
	void BarrelRoll ()
	{
		// Roll "animation"
		float rotate = 360 * Time.deltaTime * 3;
		totalRotation += rotate;
		
		SetPosture(dive);
		
		if (currentState == State.RollLeft) {
			dive.transform.Rotate(0, -rotate, 0);
		}
		if (currentState == State.RollRight) {
			dive.transform.Rotate(0, rotate, 0);
		}
		
		if (totalRotation >= 360) {
			totalRotation = 0;
			currentState = State.Normal;
			// Reset rotation in case he overrotated due to inexact math
			dive.transform.rotation = originalRotation;
			
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
	 * is always calculated as a fixed distance beneath the player while falling.
	 */
	void MoveTowardCursor() {
		// Calculate new cursor position, relative to the camera screen and our
		// current input
		float midwidth = camera.pixelWidth / 2;
		float relativeX = (currentInput == null) ? midwidth : (midwidth + (currentInput.cursorPosition * midwidth));
		float cameraDistance = Mathf.Abs(camera.transform.position.z - transform.position.z);

		cursor = camera.ScreenToWorldPoint(new Vector3(relativeX, 0, cameraDistance));

		if (Debug.isDebugBuild) {
			Debug.DrawLine(transform.position, cursor);
		}

		Vector3 desiredPosition = new Vector3(cursor.x, transform.position.y, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * boost);

		Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -1 * (cursor - transform.position));
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 15f);
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
	 * Handles any inputed actions.
	 */
	void PerformActions() {
		if (inputWasReceived) {
			switch (currentInput.action) {
				case PlayerInput.Action.Roll:
					originalRotation = dive.transform.rotation;
					currentState = currentInput.cursorPosition > 0 ? State.RollRight : State.RollLeft;
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
		// Don't handle new input while we're rolling
		if (!isRolling) {
			currentInput = input;
			inputWasReceived = true;
		}
	}

	void HandleGameControllerOnGameStarted()
	{
		rigidbody.velocity = Vector3.down * 15;
		rigidbody.drag = minDrag;
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
	#endregion
}
