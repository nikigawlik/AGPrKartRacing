using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour {

	public float steering = 0;
	public float acceleration = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		steering = Input.GetAxis("Horizontal");
		acceleration = Input.GetAxis("Vertical");
	}
}
