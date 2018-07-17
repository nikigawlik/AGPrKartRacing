using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kart : MonoBehaviour {
	public GameController gameController;
	public RaceController raceController;
	public GameObject cameraRig;

	public float forwardsAcceleration = 6f;
	public float backwardsAcceleration = 3f;

	public float maxSpeed = 16f;
	public float minSpeed = -6f; // going backwards

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
	private GameObject lasRespawnZone;
	private LayerMask onlyGroundMask;

	private float resetTimer;

	// Use this for initialization
	void Start () {
		onlyGroundMask = LayerMask.GetMask(new string[] {"Ground"});
	}

	private void Update() {
		KartController kc = GetComponent<KartController>();
		if(kc.restart) {
			ResetKart();
		}
	}
	
	void FixedUpdate () {
		KartController kc = GetComponent<KartController>();
		Rigidbody rb = GetComponent<Rigidbody>();

		RaycastHit hit;
		grounded = Physics.Raycast(transform.position + transform.up * rayStartHeight, -transform.up, 
			out hit, rayLength, onlyGroundMask);

		if(grounded)
        {
            // snap the rotation to match the ground normal (up to a certain angle)
            Quaternion rotation = Quaternion.LookRotation(hit.normal, transform.right)
                * Quaternion.Euler(180, -90, -90);
            if (Quaternion.Angle(rotation, transform.rotation) <= maxAngleChange)
            {
                transform.rotation = rotation;
            }

            // calculate speed in the forward direction
            float forwardSpeed = GetForwardSpeed();
            // calculate speed needed to get to desired speed and clamp accoring to accelerations
            float deltaSpeed;
            if (kc.acceleration > 0)
            {
                deltaSpeed = Mathf.Clamp(maxSpeed - forwardSpeed, 0, forwardsAcceleration * kc.acceleration);
            }
            else
            {
                deltaSpeed = Mathf.Clamp(minSpeed - forwardSpeed, backwardsAcceleration * kc.acceleration, 0);
            }

            rb.AddForce(transform.forward * deltaSpeed, ForceMode.Acceleration);

            // apply drag (different drag in different directions)
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            Vector3 localDrag = Vector3.Scale(-localVelocity, localDragFactors);

            rb.AddForce(transform.TransformDirection(localDrag), ForceMode.Acceleration);

            // rotate kart depending on steering and speed
            float angle = kc.steering * Time.fixedDeltaTime * maxSteering
                * speedToSteeringRatio.Evaluate(Vector3.Dot(rb.velocity, transform.forward) / maxSpeed);
            transform.Rotate(Vector3.up, angle);
        }
        else {
			// slowly rotate kart upright while in air
			Vector3 angles = transform.rotation.eulerAngles;
			angles = new Vector3(
				Mathf.LerpAngle(angles.x, 0, airRotationLerp * Time.fixedDeltaTime), 
				angles.y, 
				Mathf.LerpAngle(angles.z, 0, airRotationLerp * Time.fixedDeltaTime)
			);
			
			transform.rotation = Quaternion.Euler(angles);
		}
	}

    public float GetForwardSpeed()
    {
		Rigidbody rb = GetComponent<Rigidbody>();
        return Vector3.Dot(rb.velocity, transform.forward);
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
		// save respawn points
		if(other.CompareTag("RespawnZone")) {
			lasRespawnZone = other.gameObject;
		}
		// save checkpoints
		if(other.CompareTag("CheckpointZone")) {
			raceController.HitCheckpoint(other.gameObject);
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
        KartController kartController = GetComponent<KartController>();
        kartController.enabled = false;

		while(resetTimer > afterRespawnTime) {
			resetTimer -= Time.deltaTime;
			gameController.screenDarkenImage.color = new Color(0, 0, 0, 1f - ((resetTimer - afterRespawnTime) / resetTime));
			yield return null;
		} 
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.MovePosition(transform.position = lasRespawnZone.transform.position);
		rb.MoveRotation(transform.rotation = lasRespawnZone.transform.rotation);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;

		CameraControllerCustom ccc = cameraRig.GetComponent<CameraControllerCustom>();
		if(ccc != null) {
			ccc.ReFocusOnTarget();
		}

		while(resetTimer > 0) {
			resetTimer -= Time.deltaTime;
			gameController.screenDarkenImage.color = new Color(0, 0, 0, (resetTimer / afterRespawnTime));
			yield return null;
		} 
		gameController.screenDarkenImage.color = new Color(0, 0, 0, 0);
        kartController.enabled = true;
	}
}
