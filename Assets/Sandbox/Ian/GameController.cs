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
	
	public GameObject sceneCamera;
	#endregion
	
	#region Properties
	public GameObject Player
	{
		get
		{
			if (player == null) {
				player = GameObject.FindWithTag("Player");

				if (player == null)
					player = GameObject.Instantiate(playerPrefab, playerSpawnRef.position, playerSpawnRef.rotation) as GameObject;
			}
			return player;
		}
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		Physics.gravity = new Vector3(0, -forceOfGravity, 0);
//		if (sceneCamera != null)
//			sceneCamera.SetActive(false);
	}
	
	void OnGUI()
	{
		bool returnKeyWentDown = false;
		bool escapeKeyWentDown = false;
		
		Event e = Event.current;
		if (e.type == EventType.KeyDown)
		{
			switch (e.keyCode)
			{
			case KeyCode.Return:
				returnKeyWentDown = true;
				break;
			case KeyCode.Escape:
				escapeKeyWentDown = true;
				break;
			}
		}
		GUILayout.BeginVertical();
		{
			if (gameState == GameState.Waiting)
			{
				if (GUILayout.Button("Start") || returnKeyWentDown)
					GameStart();
			}
			else
			{
				if (GUILayout.Button("Restart") || returnKeyWentDown)
					Restart();
				GUILayout.Space(5);
				if (GUILayout.Button("Finish") || escapeKeyWentDown)
					GameOver();
			}
		}
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

		GameObject camera = GameObject.FindWithTag("MainCamera");
		if (camera) {
			camera.GetComponent<TrackingCamera>().target = Player.transform;
		}
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
		GameStart();
	}
	#endregion
}
