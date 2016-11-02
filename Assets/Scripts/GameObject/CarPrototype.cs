using UnityEngine;
using System.Collections;

public class CarPrototype : MonoBehaviour
{
	[SerializeField] ushort playerNr = 1;

	[SerializeField] float maxSteeringAngle = 30f;
	[SerializeField] float maxTorque = 100f;
	[SerializeField] float wheelFriction = 10f;

	[SerializeField] Transform centerOfMass;

	[SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] Transform[] wheels = new Transform[4];

	private Rigidbody _rigidbody;

	private bool isGrounded = true;

	// Use this for initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.centerOfMass = centerOfMass.localPosition;
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateWheelPosition();
	}

	void FixedUpdate()
	{
		float steeringAxis = Input.GetAxis("P" + playerNr.ToString() + " Horizontal") * maxSteeringAngle;
		float torque = Input.GetAxis("P" + playerNr.ToString() + " Vertical") * maxTorque;
		float groundingForce = _rigidbody.velocity.magnitude * wheelFriction;

		// Is car grounded?
		IsGrounded();

		wheelColliders[0].steerAngle = steeringAxis;
		wheelColliders[1].steerAngle = steeringAxis;

		wheelColliders[0].motorTorque = torque;
		wheelColliders[1].motorTorque = torque;
		wheelColliders[2].motorTorque = torque;
		wheelColliders[3].motorTorque = torque;
		/*
		if(torque == 0f)
		{
			wheelColliders[0].brakeTorque = 3f;
			wheelColliders[1].brakeTorque = 3f;
		}
		else
		{
			wheelColliders[0].brakeTorque = 0f;
			wheelColliders[1].brakeTorque = 0f;
		}
		*/
		if(!isGrounded)
			groundingForce *= 0.15f;
		
		_rigidbody.AddForce(Vector3.up * -groundingForce);
	}

	void UpdateWheelPosition()
	{
		for(int i = 0;i < wheelColliders.Length;i++)
		{
			Vector3 wheelPosition;
			Quaternion wheelRotation;

			wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation);

			// Apply collider position and rotation on mesh
			wheels[i].position = wheelPosition;
			wheels[i].rotation = wheelRotation;

			// Prototype-fix
			wheels[i].Rotate(new Vector3(0f, 0f, 90f));
		}

	}

	void IsGrounded()
	{
		isGrounded = true;
		foreach(WheelCollider currentWheel in wheelColliders)
		{
			isGrounded &= currentWheel.isGrounded;
		}
	}
}
