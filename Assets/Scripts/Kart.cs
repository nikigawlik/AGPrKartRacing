using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart : MonoBehaviour {
	public GameController gameController;
	public float maxAcceleration = 6f;

	public float maxSpeed = 16f;
	public float minSpeed = -4f;

	public Vector3 localDragFactors;
	
	public float maxSteering = 45f;
	public AnimationCurve speedToSteeringRatio;

	public float jumpOnCollisionStrength = 0.6f;
	public float airRotationLerp = 0.1f;

	public float resetTime = 2f;
	public float afterRespawnTime = .2f;

	[Header("Ray based rotation settings")]
	public float rayStartHeight = 1f;
	public float rayLength = 2f;
	public float maxAngleChange = 30;

	private bool grounded;
	private GameObject lastCheckpointZone;
	private LayerMask onlyGroundMask;

	private float resetTimer;

	// Use this for initialization
	void Start () {
		onlyGroundMask = LayerMask.GetMask(new string[] {"Ground"});
	}

	private void Update() {
		if(Input.GetKey("r")) {
			ResetKart();
		}
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
			if(Quaternion.Angle(rotation, transform.rotation) <= maxAngleChange) {
				transform.rotation = rotation;
			}

			float forwardSpeed = Vector3.Dot(rb.velocity, transform.forward);
			float deltaSpeed = Mathf.Clamp(maxSpeed - forwardSpeed, 0, maxAcceleration * kc.acceleration);
			rb.AddForce(transform.forward * deltaSpeed, ForceMode.Acceleration);

			Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
			Vector3 localDrag = Vector3.Scale(-localVelocity, localDragFactors);

			rb.AddForce(transform.TransformDirection(localDrag), ForceMode.Acceleration);
		
			float angle = kc.steering * Time.fixedDeltaTime * maxSteering 
				* speedToSteeringRatio.Evaluate(Vector3.Dot(rb.velocity, transform.forward) / maxSpeed);
			transform.Rotate(Vector3.up, angle);
		} else {
			Vector3 angles = transform.rotation.eulerAngles;
			// angles = Vector3.Lerp(angles, new Vector3(0, angles.y, 0), airRotationLerp);
			angles = new Vector3(
				Mathf.LerpAngle(angles.x, 0, airRotationLerp * Time.fixedDeltaTime), 
				angles.y, 
				Mathf.LerpAngle(angles.z, 0, airRotationLerp * Time.fixedDeltaTime)
			);
			
			transform.rotation = Quaternion.Euler(angles);
		}
	}

	private void OnCollisionEnter(Collision other) {
		Vector3 sidewaysImpulse = other.impulse - Vector3.Dot(transform.up, other.impulse) * transform.up;
		GetComponent<Rigidbody>().AddForce(transform.up * sidewaysImpulse.magnitude * jumpOnCollisionStrength, ForceMode.Impulse);
    }

    private void ResetKart()
    {
		if(resetTimer > 0) {
			return;
		}
		resetTimer = resetTime + afterRespawnTime;
        StartCoroutine("ResetCoroutine");
    }

    private void OnTriggerEnter(Collider other) {
		// save checkpoints
		if(other.CompareTag("CheckpointZone")) {
			lastCheckpointZone = other.gameObject;
		}
		// check death
		if(other.gameObject.CompareTag("KillZone"))
        {
            ResetKart();
        }
	}
	
	private void OnDrawGizmosSelected() {
		// if(grounded) {
		// 	Gizmos.DrawSphere(transform.position, 1.3f);
		// }
		Vector3 from = transform.position + transform.up * rayStartHeight;
		Vector3 dir = -transform.up * rayLength;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(from, from + dir);
	}

	IEnumerator ResetCoroutine() {
		while(resetTimer > afterRespawnTime) {
			resetTimer -= Time.deltaTime;
			gameController.screenDarkenImage.color = new Color(0, 0, 0, 1f - ((resetTimer - afterRespawnTime) / resetTime));
			yield return null;
		} 
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.MovePosition(lastCheckpointZone.transform.position);
		rb.MoveRotation(transform.rotation = lastCheckpointZone.transform.rotation);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		while(resetTimer > 0) {
			resetTimer -= Time.deltaTime;
			gameController.screenDarkenImage.color = new Color(0, 0, 0, (resetTimer / afterRespawnTime));
			yield return null;
		} 
		gameController.screenDarkenImage.color = new Color(0, 0, 0, 0);
	}
}
