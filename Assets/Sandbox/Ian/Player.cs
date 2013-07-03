using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	#region Unity
	void OnEnable()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
		GameController.OnGameEnded += HandleGameControllerOnGameEnded;
	}
	
	void Update()
	{
		Debug.Log(rigidbody.velocity);
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
		GameController.OnGameEnded -= HandleGameControllerOnGameEnded;
	}
	#endregion
	
	#region Handlers
	void HandleGameControllerOnGameStarted()
	{
		rigidbody.velocity = Vector3.down * 15;
	}
	
	void HandleGameControllerOnGameEnded()
	{
		
	}
	#endregion
}
