using UnityEngine;
using System.Collections;
using System;

public struct ControlInput
{
	#region Properties
	public Vector2 CurrentAcceleration
	{
		get { return accelerationValue; }
		
		set
		{
			HasChanged = HasChanged || (accelerationValue != value);
			accelerationValue = value;
		}
	}
	
	public float DeltaDrag 
	{
		get { return accelerationValue.y - lastAccelerationValue.y; }
	}
	
	public float Drag
	{
		get { return accelerationValue.y; }
	}
	
	public Vector3 DeltaForce
	{
		get { return new Vector3 (accelerationValue.x, 0, 0); }
	}
	
	public bool Attack
	{ 
		get
		{
			return attackButtonPressed;
		} 
		
		set
		{
			HasChanged = HasChanged || (attackButtonPressed != value);
			attackButtonPressed = value;
		}
	}
	
	public bool HasChanged { get; private set; }
	#endregion
	
	#region Actions
	public void Refresh()
	{
		HasChanged = false;
		lastAccelerationValue = accelerationValue;
		accelerationValue = Vector2.zero;
	}
	
	public void Reset()
	{
		HasChanged = false;
		attackButtonPressed = false;
	}
	#endregion
	
	#region Private
	private Vector2 accelerationValue;
	private Vector2 lastAccelerationValue;
	private bool attackButtonPressed;
	#endregion
}

public class InputController : MonoBehaviour
{
	#region Events
	public delegate void InputDelegate(ControlInput input);
	
	public static event InputDelegate OnInput;
	#endregion
	
	#region Attributes
	public float maxHorizontalMovement;
	public float maxVerticalMovement;
	public float mouseInputMultiplier;
	public float inputMultiplier;
	#endregion
	
	#region Unity
	void Awake()
	{
		input = new ControlInput();
		lastMousePostion = Vector3.zero;
	}
	
	void Update()
	{
		PollMouse(ref input);
		PollTouches(ref input);
		PollKeyboard(ref input);
		input.CurrentAcceleration *= Time.deltaTime * inputMultiplier;
		if (input.HasChanged && OnInput != null)
		{
			OnInput(NormalizedInput);
		}
		input.Refresh();
	}
	#endregion
	
	#region Private
	private Vector3 mouseStartPosition;
	private Vector3 lastMousePostion;
	private ControlInput input;
	
	void PollKeyboard(ref ControlInput input)
	{
		input.CurrentAcceleration += new Vector2(Input.GetAxis("Horizontal") * maxHorizontalMovement, Input.GetAxis("Vertical") * maxVerticalMovement);
		input.Attack = Input.GetButton("Fire1");
	}
	
	void PollMouse(ref ControlInput input)
	{
		Vector3 position = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * mouseInputMultiplier;
		if (position != lastMousePostion)
		{
			input.CurrentAcceleration = position;
			lastMousePostion = position;
		}
	}
	
	void PollTouches(ref ControlInput input)
	{
		if (Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			Vector2 newDeltaAcceleration = input.CurrentAcceleration;
			Vector2 touchOneDelta = touches[0].deltaPosition;
			switch (touches.Length)
			{
			case 1:
				newDeltaAcceleration.x = touchOneDelta.x;
				break;
			case 2:
				Vector2 touchTwoDelta = touches[1].deltaPosition;
				bool firstFingerOnLeft = touches[0].position.x < touches[1].position.x;
					newDeltaAcceleration += new Vector2(touchOneDelta.x + touchTwoDelta.x,
						touchOneDelta.x - touchTwoDelta.x) * (firstFingerOnLeft ? -1 : 1);
				break;
			}
			input.CurrentAcceleration = newDeltaAcceleration;
		}
	}
	
	ControlInput NormalizedInput
	{
		get
		{
			Vector2 newInput = input.CurrentAcceleration;
			newInput.x = Mathf.Clamp(newInput.x, -maxHorizontalMovement, maxHorizontalMovement);
			newInput.y = Mathf.Clamp(newInput.y, -maxVerticalMovement, maxVerticalMovement);
			input.CurrentAcceleration = newInput;
			return input;
		}
	}
	#endregion
	
	#region Debug
	public bool debugMode;
	
	void Start()
	{
		if (debugMode)
			OnInput += HandleOnInput;
	}
	
	void HandleOnInput(ControlInput input)
	{
		Debug.Log(string.Format("HandleOnInput: DeltaForce = {0} Drag = {1}, DeltaDrag = {2}", input.DeltaForce, input.Drag, input.DeltaDrag));
	}
	
	void OnDrawGizmos()
	{
		if (!debugMode)
			return;
		Vector3 position = new Vector3(input.DeltaForce.x, input.Drag, 0);
		Gizmos.DrawSphere(position, 1f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position, Vector3.zero);
	}
	#endregion
}
