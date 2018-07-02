using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart : MonoBehaviour {
	public float maxAcceleration = 6f;
	public float maxSpeed = 16f;
	public float minSpeed = -4f;
	public float friction = 0.01f;
	public float maxSteering = 45f;
	public AnimationCurve speedToSteeringRatio;

	private Vector3 velocity;

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
		KartController kc = GetComponent<KartController>();
		
		velocity += kc.acceleration * Time.fixedDeltaTime * maxAcceleration 
			* transform.forward;

		float forwardSpeed = Vector3.Dot(velocity, transform.forward);
		forwardSpeed *= 1 - friction;
		forwardSpeed = Mathf.Clamp(forwardSpeed, minSpeed, maxSpeed);

		velocity = forwardSpeed * transform.forward;

		transform.position += velocity;
		
        float angle = kc.steering * Time.fixedDeltaTime * maxSteering * speedToSteeringRatio.Evaluate(forwardSpeed / maxSpeed);
        transform.Rotate(transform.up, angle);
	}
}
