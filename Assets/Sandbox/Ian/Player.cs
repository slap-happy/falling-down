using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Events
	public delegate void HitHazard(float relativeVelocity);
	
	public HitHazard OnHitHazard;
	#endregion
	
	#region Attributes
	public float minDrag;
	public float normalDrag;
	public float maxDrag;
	public float terminalVelocity;
	public GameObject dive;
	public GameObject normal;
	public GameObject flare;
	public Transform cameraTargetRef;
	#endregion
	
	private float rollForce = 40;
	private float rollPenaltyForce = 5;
	private float doubleTapSpeed = 0.5f;
	
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
	
	private bool warningHit = false;
	
	#region Unity
	void Awake()
	{
		instance = this;
		
		if (cameraTargetRef != null)
		{
			TrackingCamera trackingCamera = Camera.mainCamera.GetComponent<TrackingCamera>();
			if (trackingCamera != null)
				trackingCamera.SetTarget(cameraTargetRef);
		}
	}
	
	void OnEnable()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
		InputController.OnInput += HandleInputControllerOnInput;
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
	}
	
	void OnDestroy()
	{
		InputController.OnInput -= HandleInputControllerOnInput;
	}
	
	void OnGUI () {
		if (warningHit == true) {
			float width = 150;
			float height = 25;
			float left = (Screen.width / 2) - (width / 2);
			GUI.Box(new Rect(left, 10, width, height), "Prepare for landing!");	
		}
	}
	
	void Update()
	{
		DetectInput ();
		Animate ();
	}
		
	private float lastTapTimeLeft = 0;
	private float lastTapTimeRight = 0;
	void DetectInput () 
	{
		// Rolls are animated an can't be broken out of early
		if (currentState != State.RollLeft && currentState != State.RollRight) {
			
			if (Input.GetAxis("Vertical") < 0) {
				currentState = State.Dive;
			} else if (Input.GetAxis("Vertical") > 0) {
				currentState = State.Flare;
			} else {
				currentState = State.Normal;
			}
			
			// This all should be moved out into a PlayerInput class
			if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				if ((Time.time - lastTapTimeLeft) < doubleTapSpeed) {
					originalRotation = dive.transform.rotation;
					currentState = State.RollLeft;
				}
				lastTapTimeLeft = Time.time;
			}
			
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				if ((Time.time - lastTapTimeRight) < doubleTapSpeed) {
					originalRotation = dive.transform.rotation;
					currentState = State.RollRight;
				}
				lastTapTimeRight = Time.time;
			}
		}
	}
	
	void Animate ()
	{
		switch (currentState) {
			case State.RollLeft:
			case State.RollRight:
				BarrelRoll ();
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
		if (inputWasReceived)
		{
			rigidbody.AddForce(currentInput.DeltaForce, ForceMode.Impulse);
			float newDrag = currentInput.Drag * Time.deltaTime;
			if (newDrag != 0)
			{
				rigidbody.drag += newDrag;
				if (rigidbody.drag < minDrag || rigidbody.drag > maxDrag)
					rigidbody.drag = Mathf.Clamp(rigidbody.drag, minDrag, maxDrag);
					
			}
			inputWasReceived = false;
			
			if (currentState == State.RollLeft) {
				rigidbody.AddForce(new Vector3(-rollForce, rollPenaltyForce, 0), ForceMode.Impulse);
				
			}
			if (currentState == State.RollRight) {
				rigidbody.AddForce(new Vector3(rollForce, rollPenaltyForce, 0), ForceMode.Impulse);
				
			}
			
		} else {
			if (rigidbody.drag > normalDrag) {
				rigidbody.drag -= 1 * Time.deltaTime;
			} else if (rigidbody.drag < normalDrag) {
				rigidbody.drag += 1 * Time.deltaTime;
			}
		}
		
		if (rigidbody.velocity.y > terminalVelocity)
		{
			Vector3 newVelocity = rigidbody.velocity;
			newVelocity.y = terminalVelocity;
			rigidbody.velocity = newVelocity;
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
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (ControlInput input)
	{
		currentInput = input;
			
		inputWasReceived = true;
	}
	
	void HandleGameControllerOnGameStarted()
	{
		rigidbody.velocity = Vector3.down * 15;
		rigidbody.drag = normalDrag;
	}
	
	void HandleGameControllerOnGameEnded()
	{
		
	}
	#endregion
	
	#region Private
	private static Player instance;
	
	private ControlInput currentInput;
	private ControlInput previousInput;
	private bool inputWasReceived;
	#endregion
}
