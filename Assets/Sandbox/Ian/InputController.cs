using UnityEngine;
using System.Collections;
using System;

public struct Inputs
{
	#region Properties
	public Vector2 DeltaAcceleration
	{
		get { return accelerationValue; }
		
		set
		{
			HasChanged = accelerationValue != value;
			accelerationValue = value;
		}
	}
	
	public Vector3 DeltaForce
	{
		get { return new Vector3 (accelerationValue.x, accelerationValue.y, 0); }
	}
	
	public bool Attack
	{ 
		get
		{
			return attackButtonPressed;
		} 
		
		set
		{
			HasChanged = attackButtonPressed != value;
			attackButtonPressed = value;
		}
	}
	
	public bool HasChanged { get; private set; }
	#endregion
	
	#region Actions
	public void Refresh()
	{
		HasChanged = false;
	}
	
	public void Reset()
	{
		HasChanged = false;
		attackButtonPressed = false;
		accelerationValue = Vector2.zero;
	}
	#endregion
	
	#region Private
	private Vector2 accelerationValue;
	private bool attackButtonPressed;
	#endregion
}

public class InputController : MonoBehaviour
{
	#region Events
	public delegate void InputDelegate(Inputs input);
	
	public static event InputDelegate OnInput;
	#endregion
	
	#region Attributes
	public float maxHorizontalMovement;
	public float maxVerticalMovement;
	public float inputMultiplier;
	#endregion
	
	#region Unity
	void Awake()
	{
		input = new Inputs();
	}
	
	void Update()
	{
		PollMouse(ref input);
		PollTouches(ref input);
		PollKeyboard(ref input);
		input.DeltaAcceleration *= Time.deltaTime * inputMultiplier;
		if (input.HasChanged && OnInput != null)
		{
			OnInput(input);
			input.Refresh();
		}
			
	}
	#endregion
	
	#region Private
	private Inputs input;
	
	void PollMouse(ref Inputs input)
	{
		
	}
	
	void PollTouches(ref Inputs input)
	{
		if (Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			Vector2 newDeltaAcceleration = input.DeltaAcceleration;
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
			newDeltaAcceleration.x = Mathf.Clamp(newDeltaAcceleration.x, -maxHorizontalMovement, maxHorizontalMovement);
			newDeltaAcceleration.y = Mathf.Clamp(newDeltaAcceleration.y, -maxVerticalMovement, maxVerticalMovement);
			input.DeltaAcceleration = newDeltaAcceleration;
		}
	}
	
	void PollKeyboard(ref Inputs input)
	{
		input.DeltaAcceleration += new Vector2(Input.GetAxis("Horizontal") * maxHorizontalMovement, Input.GetAxis("Vertical") * maxVerticalMovement);
		input.Attack = Input.GetButton("Fire1");
	}
	#endregion
	
	#region Debug
	public bool debug;
	
	void Start()
	{
		if (debug)
			OnInput += HandleOnInput;
	}
	
	void HandleOnInput(Inputs input)
	{
		Debug.Log("OnInputCalled");
	}
	
	void OnDrawGizmos()
	{
		if (!debug)
			return;
		Vector3 position = new Vector3(input.DeltaAcceleration.x, input.DeltaAcceleration.y, 0);
		Gizmos.DrawSphere(position, 1f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(position, Vector3.zero);
	}
	#endregion
}
