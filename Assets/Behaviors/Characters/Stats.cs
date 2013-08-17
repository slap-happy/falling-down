using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour
{
	#region Attributes
	public int startingHealth;
	public Player player;
	#endregion
	
	public int Health { get; private set; }
	
	#region Unity
	void Awake()
	{
		player = GetComponent<Player>();
		player.OnHitHazard += HandlePlayerOnHitHazard;
		GameController.OnGameStarted += HandleGameControllerOnGameStarted;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width - 85, 30, 75, 20), string.Format("Health: {0}", Health));
	}
	#endregion
	
	#region Handlers
	void HandlePlayerOnHitHazard(float velocity)
	{
		// do some fancy stuff with numbers to get this right
		
		Health -= (int)velocity;
		
		if (Health <= 0)
		{
			player.Kill();
			enabled = false;
		}
	}
	
	void HandleGameControllerOnGameStarted()
	{
		Health = startingHealth;
		enabled = true;
	}
	#endregion
}
