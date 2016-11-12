using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float height = 10f;
	[SerializeField] private float angle = 20f;
	[SerializeField] private float smooth = 5f;

	private float distanceToTarget;
	private Vector3 velocity;

	// Initialization
	void Start()
	{
		velocity = Vector3.zero;

		ChangeTarget(target);
	}
	
	// Physics
	void FixedUpdate()
	{
		Vector3 movePosition = target.position + new Vector3(distanceToTarget, height);
		float distanceBetween = Vector3.Distance(transform.position, movePosition);

		transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smooth);
	}

	// Public methods

	// Change target to follow
	public void ChangeTarget(Transform newTarget)
	{
		target = newTarget;

		if(newTarget.tag == "Player")
			height = 10f;
		else
			height = 20f;

		// Camero move and look at
		distanceToTarget = Mathf.Tan(angle) * height;

		Vector3 movePosition = target.position + new Vector3(distanceToTarget, height);
		transform.position = movePosition;
		transform.LookAt(target);
	}
}
