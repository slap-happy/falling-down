using UnityEngine;
using System;

/**
 * A simple network controller.
 */
public class NetworkController : MonoBehaviour {
	/**
	 * Port to listen on.
	 */
	public int port = 42666;

	/**
	 * Maximum client connections.
	 */
	public int maxConnections = 3;

	/**
	 * Interval at which we poll for available servers.
	 */
	public float pollInterval = 10f;
	private float nextPoll = 0f;
	private float nextCheck = 0f;

	/**
	 * Game type used to advertise network servers.
	 */
	public string gameType = "Some Dumb Game";

	/**
	 * Game name used to advertise network servers.
	 */
	public string gameName;

	/**
	 * Whether or not this is the network server.
	 */
	public bool isHosting {
		get { return _isHosting; }
	}
	private bool _isHosting = false;

	/**
	 * Existing server hosts that we can connect to.
	 */
	public HostData[] servers {
		get {
			if (_servers == null || Time.time > nextPoll) {
				nextPoll = Time.time + pollInterval;
				Debug.Log("polling for registered servers of " + gameType);
				MasterServer.RequestHostList(gameType);
			}

			if (_servers == null || Time.time > nextCheck) {
				nextCheck = Time.time + 1;
				_servers = MasterServer.PollHostList();
				Debug.Log("found " + _servers.Length + " servers");
			}

			return _servers;
		}
	}
	private HostData[] _servers;

	/**
	 * Registers a new game with the Unity master server.
	 */
	public bool Host() {
		NetworkConnectionError result = Network.InitializeServer(maxConnections, port, !Network.HavePublicAddress());

		switch (result) {
			case NetworkConnectionError.NoError:
				Debug.Log("listing on port " + port + " for connections");
				break;
			default:
				Debug.Log("failed to initialize server: " + result);
				return false;
		}

		MasterServer.RegisterHost(gameType, gameName);
		_isHosting = true;

		return true;
	}

	/**
	 * Joins an existing game.
	 */
	public bool Join(HostData server) {
		NetworkConnectionError result = Network.Connect(server.guid);

		switch (result) {
			case NetworkConnectionError.NoError:
				Debug.Log("joined game: " + server.gameName);
				break;
			default:
				Debug.Log("failed to connect to server: " + result);
				return false;
		}

		return true;
	}

	void Awake() {
		if (gameName == null || gameName.Length == 0) {
			gameName = Environment.MachineName;
		}
	}

	void OnApplicationQuit() {
		Network.Disconnect();
		MasterServer.UnregisterHost();
	}
}
