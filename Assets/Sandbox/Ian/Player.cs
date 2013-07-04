using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Unity
	void OnEnable()
	{
		InputController.OnInput += HandleInputControllerOnInput;
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
	}
	
	void FixedUpdate()
	{
		rigidbody.AddForce(currentInput.DeltaForce, ForceMode.Impulse);
//		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(currentInput.DeltaAcceleration.x, 0, 0)), Time.deltaTime);
	}
	#endregion
	
	#region Handlers
	void HandleInputControllerOnInput (Inputs input)
	{
		currentInput = input;
	}
	
	void HandleGameControllerOnGameStarted()
	{
		rigidbody.velocity = Vector3.down * 15;
	}
	
	void HandleGameControllerOnGameEnded()
	{
		
	}
	#endregion
	
	#region Private
	private Inputs currentInput;
	#endregion
}
