using UnityEngine;
using System.Collections;

public class CarPrototype : MonoBehaviour
{
	[SerializeField] float maxSteeringAngle = 30f;
	[SerializeField] float maxTorque = 500f;

	[SerializeField] Transform centerOfMass;

	[SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] Transform[] wheels = new Transform[4];

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
		UpdateWheelPosition();
	}

	void FixedUpdate()
	{
		float steeringAxis = Input.GetAxis("Horizontal") * maxSteeringAngle;
		float torque = Input.GetAxis("Vertical") * maxTorque;

		wheelColliders[0].steerAngle = steeringAxis;
		wheelColliders[1].steerAngle = steeringAxis;

		wheelColliders[0].motorTorque = torque;
		wheelColliders[1].motorTorque = torque;
		wheelColliders[2].motorTorque = torque;
		wheelColliders[3].motorTorque = torque;

		if(torque == 0f)
		{
			wheelColliders[0].brakeTorque = 30f;
			wheelColliders[1].brakeTorque = 30f;
		}
		else
		{
			wheelColliders[0].brakeTorque = 0f;
			wheelColliders[1].brakeTorque = 0f;
		}
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
}
