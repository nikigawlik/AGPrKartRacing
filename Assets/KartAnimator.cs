using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartAnimator : MonoBehaviour {
	public GameObject[] frontWheels;
	public GameObject[] backWheels;
	public GameObject steeringWheel;
	public GameObject body;

	public float wheelSpeedFactor = 0.1f;
	public float steeringAngle = 60f;

	private GameObject[] allWheels;
	private Quaternion[] initialWheelRotations;
	private float wheelRot;

	// Use this for initialization
	void Start () {
		// fil array that contains all wheels
		allWheels = new GameObject[frontWheels.Length + backWheels.Length];
		Array.Copy(frontWheels, 0, allWheels, 0, frontWheels.Length);
		Array.Copy(backWheels, 0, allWheels, frontWheels.Length, backWheels.Length);
		
		initialWheelRotations = new Quaternion[allWheels.Length];
		for(int i = 0; i < allWheels.Length; i++) {
			initialWheelRotations[i] = allWheels[i].transform.localRotation;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		KartController kc = GetComponent<KartController>();
		Kart kart = GetComponent<Kart>();

		wheelRot += kart.GetForwardSpeed() * wheelSpeedFactor;

		for(int i = 0; i < allWheels.Length; i++) {
			GameObject obj = allWheels[i];
			obj.transform.localRotation = initialWheelRotations[i];
			obj.transform.Rotate(-Vector3.right, wheelRot);
		}

		float steeringOffset = kc.steering * steeringAngle;

		foreach (GameObject obj in frontWheels)
		{
			obj.transform.Rotate(Vector3.up, steeringOffset, Space.World);
		}
	}
}
