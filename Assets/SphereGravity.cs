using UnityEngine;
using System.Collections;

public class SphereGravity : MonoBehaviour {
	
	public GameObject player;
	
	private float gravity = 50f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate() {
		player.rigidbody.velocity += (transform.position - player.transform.position).normalized * gravity * Time.deltaTime;	
	}
}
