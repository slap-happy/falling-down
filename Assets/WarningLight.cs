using UnityEngine;
using System.Collections;

public class WarningLight : MonoBehaviour {

	void Update () {
		float rotationAmount = 360.0f;
		transform.Rotate(0, rotationAmount * Time.deltaTime, 0);
	}
}
