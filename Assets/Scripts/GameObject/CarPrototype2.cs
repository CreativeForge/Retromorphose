using UnityEngine;
using System.Collections;

public class CarPrototype2 : MonoBehaviour
{
	[SerializeField] private float maxTorque = 100f;
	[SerializeField] private float steeringForce = 100f;
	[SerializeField] private Transform centerOfMass;
	private Rigidbody _rigidbody;

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.centerOfMass = centerOfMass.localPosition;
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}

	void FixedUpdate()
	{
		float forwardForce = Input.GetAxis("Vertical") * maxTorque;
		float angularForce = Input.GetAxis("Horizontal") * Input.GetAxis("Vertical") * steeringForce;

		_rigidbody.AddForce(transform.forward * forwardForce);
		_rigidbody.AddTorque(transform.up * angularForce);
	}
}
