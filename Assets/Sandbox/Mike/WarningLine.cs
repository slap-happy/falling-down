using UnityEngine;
using System.Collections;

public class WarningLine : MonoBehaviour {
	
	public GameObject[] warningLights;
	
	// Update is called once per frame
	void Update () {
		float rotationAmount = 360.0f;
		
		foreach (GameObject light in warningLights) {
			light.transform.Rotate(0, rotationAmount * Time.deltaTime, 0);
		}
	}
}
