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
		bool doFlare = currentInput.DeltaDrag > float.Epsilon;
		bool doDive = currentInput.DeltaDrag < float.Epsilon;
		normal.SetActive(!doFlare && !doDive);
//		flare.SetActive(doFlare);
		dive.SetActive(doDive);
		
		Debug.Log(string.Format("flare = {0}, dive = {1}, deltaDrag = {2}", doFlare, doDive, currentInput.DeltaDrag));
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
		}
		else
		{
		
		if (rigidbody.drag > normalDrag)
			rigidbody.drag -= 1 * Time.deltaTime;
		else if (rigidbody.drag < normalDrag)
			rigidbody.drag += 1 * Time.deltaTime;
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
	private ControlInput currentInput;
	private ControlInput previousInput;
	private bool inputWasReceived;
	#endregion
}
