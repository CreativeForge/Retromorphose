using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float distanceToTarget = 10f;
	[SerializeField] private float distanceToPlayer = 10f;
	[SerializeField] private float angle = 20f;
	[SerializeField] private float smooth = 0.5f;

	private float height;
	private Vector3 movePosition;
	private Vector3 velocity;

	// Initialization
	void Start()
	{
		velocity = Vector3.zero;

		// Camera move and look at
		if(target != null)
			ChangeTarget(target);
	}
	
	// Physics
	void FixedUpdate()
	{
		Vector3 movePosition;

		// Camera position
		movePosition = new Vector3(target.position.x + Mathf.Abs(Mathf.Tan(angle) * height), target.position.y + height, target.position.z);

		transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smooth);
	}

	// Public methods

	// Change target to follow
	public void ChangeTarget(Transform newTarget)
	{
		target = newTarget;

		// Height
		if(target.tag == "Player")
		{
			height = Mathf.Cos(angle) * distanceToPlayer;
		}
		else
		{
			height = Mathf.Cos(angle) * distanceToTarget;
		}
		height = Mathf.Abs(height);

		// Calculate distance on x-axis
		movePosition = new Vector3(target.position.x + Mathf.Abs(Mathf.Tan(angle) * height), target.position.y + height, target.position.z);
		transform.position = movePosition;
		transform.LookAt(target);

		/*
		if(newTarget.tag == "Player")
			height = 10f;
		else
			height = 20f;
		

		// Camera move and look at
		distanceToTarget = Mathf.Tan(angle) * height;

		Vector3 movePosition = target.position + new Vector3(distanceToTarget, height);
		transform.position = movePosition;
		transform.LookAt(target);
		*/
	}
}
