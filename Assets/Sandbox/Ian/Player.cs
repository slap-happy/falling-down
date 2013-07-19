using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Events
	public delegate void HitHazard(float relativeVelocity);
	
	public HitHazard OnHitHazard;
	#endregion
	
	#region Attributes
	public float cursorDistance = 100f;
	public float cursorSpaceWidth = 50f;
	public float minDrag;
	public float maxDrag;
	public float terminalVelocity;
	public GameObject dive;
	public GameObject normal;
	public GameObject flare;
	#endregion
	
	private float rollForce = 40;
	private float rollPenaltyForce = 5;
	
	private Quaternion originalRotation;
	
	private State currentState = State.Normal;
	
	private enum State {
		Normal,
		Dive,
		Flare,
		RollLeft,
		RollRight,
		Dead
	}
	
	#region Unity
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
			case State.Flare:
			case State.Dive:
				SetPosture(currentState);
				break;
		}
	}
	
	private void SetPosture(State state) {
		bool doDive = false;
		bool doFlare = false;
		if (state == State.Dive) {
			doDive = true;
		} else if (state == State.Flare) {
			doFlare = true;
		} 		
		
		normal.SetActive(!doFlare && !doDive);
		flare.SetActive(doFlare);
		dive.SetActive(doDive);
	}
	
	private float totalRotation = 0;
	void BarrelRoll ()
	{
		// Roll "animation"
		float rotate = 360 * Time.deltaTime * 3;
		totalRotation += rotate;
		
		SetPosture (State.Dive);
		
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
		// Rolls are animated an can't be broken out of early
		if (currentState != State.RollLeft && currentState != State.RollRight) {
			MoveTowardCursor();
			ApplyDrag();
			PerformActions();
		}

		if (terminalVelocity > 0 && rigidbody.velocity.y > terminalVelocity)
		{
			Vector3 newVelocity = rigidbody.velocity;
			newVelocity.y = terminalVelocity;
			rigidbody.velocity = newVelocity;
		}

		inputWasReceived = false;
	}

	/**
	 * Moves towards the virtual cursor according to player input. The cursor
	 * is always calculated as a fixed distance beneath the player while falling.
	 */
	void MoveTowardCursor() {
		cursor = new Vector3(
				inputWasReceived ? (currentInput.cursorPosition * cursorSpaceWidth) : cursor.x,
				transform.position.y - cursorDistance,
				transform.position.z
		);

		Vector3 desiredPosition = new Vector3(cursor.x, transform.position.y, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 1f);

		Quaternion rotation = Quaternion.LookRotation(Vector3.forward, -1 * (cursor - transform.position));
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 15f);

		// Apply a penalty force if rolling
		if (inputWasReceived) {
			switch (currentState) {
				case State.RollLeft:
					rigidbody.AddForce(new Vector3(-rollForce, rollPenaltyForce, 0), ForceMode.Impulse);
					break;
				case State.RollRight:
					rigidbody.AddForce(new Vector3(rollForce, rollPenaltyForce, 0), ForceMode.Impulse);
					break;
			}
		}
	}

	/**
	 * Adjusts drag according to input.
	 */
	void ApplyDrag() {
		float flare = inputWasReceived ? currentInput.flareMagnitude : 0;
		rigidbody.drag = minDrag + ((maxDrag - minDrag) * flare);
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
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (PlayerInput input)
	{
		currentInput = input;
		inputWasReceived = true;
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
	private PlayerInput currentInput;
	private PlayerInput previousInput;
	private bool inputWasReceived;
	private Vector3 cursor;
	#endregion
}
