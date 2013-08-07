using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]

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
	public GameObject player { get; private set; }
	
	public bool isHosting {
		get { return (network != null) && network.isHosting; }
	}
	#endregion

	public GameObject Instantiate(GameObject prefab, Transform spawnRef) {
		if (network == null) {
			return GameObject.Instantiate(prefab, spawnRef.position, spawnRef.rotation) as GameObject;
		}
		else {
			return Network.Instantiate(prefab, spawnRef.position, spawnRef.rotation, 0) as GameObject;
		}
	}

	#region Unity
	void Awake() {
		Physics.gravity = new Vector3(0, -forceOfGravity, 0);
		network = GetComponent<NetworkController>();

		if (network != null) {
			network.gameType = Debug.isDebugBuild ? "Falling Down Development" : "Falling Down";
		}
	}

	void OnConnectedToServer() {
		ReadyPlayer();
	}

	void OnPlayerConnected() {
		if (gameState == GameState.Waiting) {
			ReadyPlayer();
		}
	}
	
	void OnGUI() {
		bool returnKeyWentDown = false;
		bool escapeKeyWentDown = false;
		
		Event e = Event.current;
		if (e.type == EventType.KeyDown) {
			switch (e.keyCode) {
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
				if (isHosting && (GUILayout.Button("Start") || returnKeyWentDown))
					GameStart();

				break;
			default:
				if (isHosting && (GUILayout.Button("Restart") || returnKeyWentDown))
					Restart();

				GUILayout.Space(5);

				if (isHosting && (GUILayout.Button("Finish") || escapeKeyWentDown))
					GameOver();

				break;
		}
	}
	#endregion
	
	#region Private
	private GameState gameState;
	private NetworkController network;

	void ReadyPlayer() {
		if (player == null) {
			player = Instantiate(playerPrefab, playerSpawnRef);
			player.SetActive(false);
		}
	}

	void HostGame() {
		network.Host();
		gameState = GameState.Waiting;
	}

	void JoinGame(HostData server) {
		network.Join(server);
		gameState = GameState.Waiting;
	}

	void GameStart() {
		ReadyPlayer();
		networkView.RPC("NotifyGameStarted", RPCMode.All);
	}

	void GameOver() {
		gameState = GameState.Waiting;
		player.SetActive(false);
		Restart();
		if (OnGameEnded != null)
			OnGameEnded();
	}
	
	void Restart() {
		player.transform.position = playerSpawnRef.position;
		player.transform.rotation = playerSpawnRef.rotation;
		GameStart();
	}

	[RPC]
	void NotifyGameStarted() {
		gameState = GameState.Running;
		if (OnGameStarted != null)
			OnGameStarted();
	}
	#endregion
}
