using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	enum GameState { Waiting, Running }
	
	#region Events
	public delegate void GameStarted();
	public delegate void GameEnded();
	
	public static event GameStarted OnGameStarted;
	public static event GameEnded OnGameEnded;
	#endregion
	
	#region Attributes
	public float forceOfGravity;
	public GameObject playerPrefab;
	public Transform playerSpawnRef;
	#endregion
	
	#region Properties
	public GameObject Player
	{
		get
		{
			if (player == null)
				player = GameObject.Instantiate(playerPrefab, playerSpawnRef.position, playerSpawnRef.rotation) as GameObject;
			return player;
		}
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		Physics.gravity = new Vector3(0, -forceOfGravity, 0);
	}
	
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			{
				GUILayout.FlexibleSpace();
				if (gameState == GameState.Waiting)
				{
					if (GUILayout.Button("Start"))
						GameStart();
				}
				else
				{
					if (GUILayout.Button("Restart"))
						Restart();
					GUILayout.Space(5);
					if (GUILayout.Button("Finish"))
						GameOver();
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
	}
	#endregion
	
	#region Private
	private GameState gameState;
	private GameObject player;
	
	void GameStart()
	{
		gameState = GameState.Running;
		Player.SetActive(true);
		if (OnGameStarted != null)
			OnGameStarted();
	}
	
	void GameOver()
	{
		gameState = GameState.Waiting;
		Player.SetActive(false);
		Restart();
		if (OnGameEnded != null)
			OnGameEnded();
	}
	
	void Restart()
	{
		Player.transform.position = playerSpawnRef.position;
		Player.transform.rotation = playerSpawnRef.rotation;
	}
	#endregion
}
