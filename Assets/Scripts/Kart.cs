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
	public float gravity = 1f;

	[Header("Ray based rotation settings")]
	public Vector3 localFrontCheck = Vector3.forward;
	public Vector3 localBackCheck = Vector3.back;
	public float rayStartHeight = 1f;
	public float rayLength = 2f;
	public float floorOffset = .2f;
	public float maximuChangeAnglePerSecond = 90;

	private Vector3 velocity;

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
		KartController kc = GetComponent<KartController>();
		
		velocity += Vector3.down * gravity * Time.fixedDeltaTime;

		RaycastHit hit;
		if(Physics.Raycast(transform.position + transform.up * rayStartHeight, -transform.up, out hit, rayLength)) {
			Quaternion rotation = Quaternion.LookRotation(hit.normal, transform.right)
				* Quaternion.Euler(180, -90, -90);
			// if(Quaternion.Angle(transform.rotation, rotation) < maximuChangeAnglePerSecond * Time.fixedDeltaTime) {
			transform.rotation = rotation;
			// }

			velocity += kc.acceleration * Time.fixedDeltaTime * maxAcceleration 
			* transform.forward;

			float forwardSpeed = Vector3.Dot(velocity, transform.forward);
			forwardSpeed *= 1 - friction;
			forwardSpeed = Mathf.Clamp(forwardSpeed, minSpeed, maxSpeed);

			velocity = forwardSpeed * transform.forward;

			transform.position = hit.point + hit.normal * floorOffset;
			velocity -= Vector3.Dot(velocity, hit.normal) * hit.normal;
		}
		
        float angle = kc.steering * Time.fixedDeltaTime * maxSteering * speedToSteeringRatio.Evaluate(Vector3.Dot(velocity, transform.forward) / maxSpeed);
        transform.Rotate(transform.up, angle);
		// rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(angle, transform.up));
		
		transform.position += velocity;
		// rb.MovePosition(rb.position + velocity);
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.TransformPoint(localFrontCheck), .2f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.TransformPoint(localBackCheck), .2f);
	}
}
