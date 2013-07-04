using UnityEngine;
using System.Collections;

public class TestHazardController : MonoBehaviour
{
	[System.Serializable]
	public class Hazards
	{
		public GameObject[] topTier;
		public GameObject[] secondTier;
		public GameObject[] thirdTier;
		public GameObject[] bottomTier;
	}
	
	#region Attributes
	public Hazards hazards;
	#endregion
	
	#region Unity
	void Awake()
	{
		ResetHazards();
	}
	
	void OnEnable()
	{
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
	}
	
	void OnDisable()
	{
		GameController.OnGameStarted -= HandleGameControllerOnGameStarted;
	}
	#endregion
	
	#region Handlers
	void HandleGameControllerOnGameStarted()
	{
		ResetHazards();
		hazards.topTier[Random.Range(0,2)].SetActive(true);
		hazards.secondTier[Random.Range(0,2)].SetActive(true);
		hazards.thirdTier[Random.Range(0,2)].SetActive(true);
		hazards.bottomTier[Random.Range(0,2)].SetActive(true);
	}
	#endregion
	
	#region Private
	void ResetHazards()
	{
		foreach (GameObject go in hazards.topTier)
			go.SetActive(false);
		foreach (GameObject go in hazards.secondTier)
			go.SetActive(false);
		foreach (GameObject go in hazards.thirdTier)
			go.SetActive(false);
		foreach (GameObject go in hazards.bottomTier)
			go.SetActive(false);
	}
	#endregion
}
