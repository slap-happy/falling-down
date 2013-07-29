using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	enum GameState { Connecting, Waiting, Running }
	
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
		network = GetComponent<NetworkController>();

		if (network != null) {
			network.gameType = Debug.isDebugBuild ? "Falling Down Development" : "Falling Down";
		}
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

		switch (gameState) {
			case GameState.Connecting:
				if (network == null) {
					gameState = GameState.Waiting;
					GUILayout.Space(5);
				}
				else {
					if (GUILayout.Button("Host Game") || returnKeyWentDown) {
						HostGame();
					}

					GUILayout.Space(5);

					foreach (HostData server in network.servers) {
						if (GUILayout.Button("Join " + server.gameName + " (" + server.connectedPlayers + ")")) {
							JoinGame(server);
						}
					}
				}
				break;

			case GameState.Waiting:
				if (GUILayout.Button("Start") || returnKeyWentDown)
					GameStart();

				break;
			default:
				if (GUILayout.Button("Restart") || returnKeyWentDown)
					Restart();

				GUILayout.Space(5);

				if (GUILayout.Button("Finish") || escapeKeyWentDown)
					GameOver();

				break;
		}
	}
	#endregion
	
	#region Private
	private GameState gameState;
	private GameObject player;
	private NetworkController network;

	void HostGame() {
		network.Host();
		gameState = GameState.Waiting;
	}

	void JoinGame(HostData server) {
		network.Join(server);
		gameState = GameState.Waiting;
	}

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
		GameStart();
	}
	#endregion
}
