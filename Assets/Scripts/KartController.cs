using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour {

	public float steering = 0;
	public float acceleration = 0;
	public bool restart;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		steering = Input.GetAxis("Horizontal");
		acceleration = Input.GetAxis("Vertical");
		restart = Input.GetKeyDown("r");
	}

	private void OnDisable() {
		steering = 0;
		acceleration = 0;
		restart = false;
	}
}
