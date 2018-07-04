using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart : MonoBehaviour {
	public float maxAcceleration = 6f;

	public float maxSpeed = 16f;
	public float minSpeed = -4f;

	public Vector3 localDragFactors;
	
	public float maxSteering = 45f;
	public AnimationCurve speedToSteeringRatio;

	[Header("Ray based rotation settings")]
	public float rayStartHeight = 1f;
	public float rayLength = 2f;

	private bool grounded;
	private LayerMask onlyGroundMask;

	// Use this for initialization
	void Start () {
		onlyGroundMask = LayerMask.GetMask(new string[] {"Ground"});
	}
	
	void FixedUpdate () {
		KartController kc = GetComponent<KartController>();
		Rigidbody rb = GetComponent<Rigidbody>();

		RaycastHit hit;
		grounded = Physics.Raycast(transform.position + transform.up * rayStartHeight, -transform.up, 
			out hit, rayLength, onlyGroundMask);

		if(grounded) {
			Quaternion rotation = Quaternion.LookRotation(hit.normal, transform.right)
				* Quaternion.Euler(180, -90, -90);
			transform.rotation = rotation;

			float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
			float deltaSpeed = Mathf.Clamp(maxSpeed - forwardSpeed, 0, maxAcceleration * kc.acceleration);
			rb.AddForce(transform.forward * deltaSpeed, ForceMode.Acceleration);

			Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			Vector3 localDrag = Vector3.Scale(-localVelocity, localDragFactors);

			rb.AddForce(transform.TransformDirection(localDrag), ForceMode.Acceleration);
		
			float angle = kc.steering * Time.fixedDeltaTime * maxSteering 
				* speedToSteeringRatio.Evaluate(Vector3.Dot(rb.velocity, transform.forward) / maxSpeed);
			transform.Rotate(Vector3.up, angle);
		}
	}
	
	private void OnDrawGizmosSelected() {
		// if(grounded) {
		// 	Gizmos.DrawSphere(transform.position, 1.3f);
		// }
	}
}
