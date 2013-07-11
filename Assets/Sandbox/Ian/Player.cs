using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Attributes
	public float minDrag;
	public float normalDrag;
	public float maxDrag;
	public float terminalVelocity;
	#endregion
	
	#region Unity
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
	
	void Update()
	{
		if (rigidbody.velocity.y > terminalVelocity)
		{
			Vector3 newVelocity = rigidbody.velocity;
			newVelocity.y = terminalVelocity;
			rigidbody.velocity = newVelocity;
		}
	}
	
	void FixedUpdate()
	{
		rigidbody.AddForce(currentInput.DeltaForce, ForceMode.Impulse);
		float newDrag = currentInput.Drag * Time.deltaTime;
		if (newDrag != 0)
		{
			rigidbody.drag += newDrag;
			if (rigidbody.drag < minDrag || rigidbody.drag > maxDrag)
				rigidbody.drag = Mathf.Clamp(rigidbody.drag, minDrag, maxDrag);
				
		}
		enabled = false;
	}
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (ControlInput input)
	{
		currentInput = input;
		enabled = true;
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
	private ControlInput currentInput;
	#endregion
}
