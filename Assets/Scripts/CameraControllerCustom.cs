using UnityEngine;
using System.Collections;

public class CameraControllerCustom : MonoBehaviour
{
	public float lookSpeed = 4f;
	public GameObject following;
	
	Transform followTransform;
	float initialDistance;
	Vector3 initialPos;

	private void Start() {
		following = following != null? following : GameObject.Find("HappyPoint");
		// Debug.Log("Found the cam target object:");
		// Debug.Log(following);
		followTransform = following.transform;

		initialDistance = CalcPlaneDistance(transform.position, followTransform.position);
		initialPos = (transform.position - followTransform.position);
	}

	void FixedUpdate() {
		
		try
		{
			// rotate camera around the target
			Vector3 localPos = followTransform.InverseTransformPoint(transform.position);
			localPos = Quaternion.Euler(Input.GetAxis("LookY") * lookSpeed, Input.GetAxis("LookX") * lookSpeed, 0f) * localPos;
			transform.position = followTransform.TransformPoint(localPos);
		}
		catch (System.ArgumentException)
		{
			// axis not set up, do nothing
		}

		// normal follow behaviour
		Vector3 diffVector = transform.position - followTransform.position;
		diffVector = new Vector3(diffVector.x, 0, diffVector.z);
		float currentDistance = diffVector.magnitude;
		float nextDistance = (currentDistance + initialDistance) / 2;

		transform.position = followTransform.position + diffVector.normalized * nextDistance;
		transform.position = new Vector3(
			transform.position.x, 
			followTransform.position.y + initialPos.y, 
			transform.position.z
		);

		transform.rotation = Quaternion.LookRotation(-diffVector + new Vector3(0, -initialPos.y, 0));	
	}

	private void OnDrawGizmos() {
		if (followTransform != null)
			Gizmos.DrawSphere(followTransform.position, .1f);
	}
	
	private float CalcPlaneDistance(Vector3 a, Vector3 b) {
		Vector3 flatA = new Vector3(a.x, 0f, a.z);
		Vector3 flatB = new Vector3(b.x, 0f, b.z);
		return (flatA - flatB).magnitude;
	}
}
