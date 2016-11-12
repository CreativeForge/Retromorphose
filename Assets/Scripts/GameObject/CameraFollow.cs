using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float height = 10f;
	[SerializeField] private float angle = 20f;
	[SerializeField] private float smooth = 5f;

	private float distanceToTarget;

	// Use this for initialization
	void Start()
	{
		distanceToTarget = Mathf.Tan(angle) * height;
	}
	
	// Update is called once per frame
	void Update()
	{
		transform.position = Vector3.Slerp(transform.position, target.position + new Vector3(distanceToTarget, height), Time.deltaTime * smooth);
		transform.LookAt(target);
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
	}
}
