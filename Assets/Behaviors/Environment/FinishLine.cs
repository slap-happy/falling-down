using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {
		
	void OnTriggerEnter(Collider other) {
		Color newColor = renderer.material.color;
		newColor.r = 1;
		newColor.g = 0;
		newColor.b = 0;
		renderer.material.color = newColor;
	}
}
