using UnityEngine;

/**
 * Controller for Faller characters. Stores current state (replicated among
 * network peers) and animates character locally.
 */
public class Faller : MonoBehaviour {
	/**
	 * Possible character states.
	 */
	public enum State {
		Relaxed,
		Diving,
		Flaring,
		RollingLeft,
		RollingRight,
		Braking,
		Dead
	}

	public delegate void StateChange(State state);
	public event StateChange OnState;

	public GameObject relaxed;
	public GameObject flare;
	public GameObject dive;

	/**
	 * Numeric posture IDs used for bitwise network serialization.
	 */
	private const short relaxedId = 32;
	private const short flareId = 64;
	private const short diveId = 128;

	/**
	 * Our current state.
	 */
	public State state {
		get { return _state; }

		private set {
			if (value != _state) {
				_state = value;
				OnState(_state);
			}
		}
	}
	private State _state;

	/**
	 * State shifts.
	 */
	public void Relax() { state = State.Relaxed; }
	public void Dive() { state = State.Diving; }
	public void Flare() { state = State.Flaring; }
	public void RollLeft() { state = State.RollingLeft; }
	public void RollRight() { state = State.RollingRight; }
	public void Brake() { state = State.Braking; }
	public void Die() { state = State.Dead; }

	/**
	 * State interogators.
	 */
	public bool isRelaxed { get { return state == State.Relaxed; } }
	public bool isDiving { get { return state == State.Diving; } }
	public bool isFlaring { get { return state == State.Flaring; } }
	public bool isRolling { get { return isRollingLeft || isRollingRight; } }
	public bool isRollingLeft { get { return state == State.RollingLeft; } }
	public bool isRollingRight { get { return state == State.RollingRight; } }
	public bool isBraking { get { return state == State.Braking; } }
	public bool isDead { get { return state == State.Dead; } }

	/**
	 * The active posture.
	 */
	public GameObject posture { get; private set; }

	void Awake() {
		state = State.Relaxed;
		posture = relaxed;

		OnState += DebugState;
	}

	/**
	 * Handles network serialization of our state, active posture and active
	 * posture transform.
	 */
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		short data = 0;
		Vector3 position = posture.transform.position;
		Quaternion rotation = posture.transform.rotation;

		if (stream.isWriting) {
			data = (short)state;
			data |= PostureToId(posture);

			stream.Serialize(ref data);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		}
		else {
			stream.Serialize(ref data);
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);

			state = (State)(data & (relaxedId - 1));
			SetPosture(PostureFromId(data));
		}
	}

	void Update() {
		switch (state) {
			case State.Relaxed:
				SetPosture(relaxed);
				break;
			case State.Braking:
			case State.Flaring:
				SetPosture(flare);
				break;
			case State.RollingLeft:
			case State.RollingRight:
			case State.Diving:
				SetPosture(dive);
				break;
		}
	}

	void SetPosture(GameObject newPosture) {
		if (posture != newPosture) {
			newPosture.transform.position = posture.transform.position;
			newPosture.transform.rotation = posture.transform.rotation;

			posture = newPosture;

			relaxed.SetActive(relaxed == posture);
			flare.SetActive(flare == posture);
			dive.SetActive(dive == posture);
		}
	}

	void DebugState(State state) {
		Debug.Log("state changed to " + state);
	}

	short PostureToId(GameObject posture) {
		if (posture == relaxed)
			return relaxedId;
		else if (posture == flare)
			return flareId;
		else
			return diveId;
	}

	GameObject PostureFromId(short id) {
		switch (id & (relaxedId | flareId | diveId)) {
			case relaxedId:
				return relaxed;
			case flareId:
				return flare;
			default:
				return dive;
		}
	}
}
