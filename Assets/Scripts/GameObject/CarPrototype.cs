using UnityEngine;
using System.Collections;

public class CarPrototype : MonoBehaviour
{
	// Serialized
	[SerializeField] float maxSteeringAngle = 30f;
	[SerializeField] float maxTorque = 100f;
	[SerializeField] float wheelFriction = 10f;

	[SerializeField] Transform centerOfMass;

	[SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];
	[SerializeField] Transform[] wheels = new Transform[4];

	// On initialization
	private Rigidbody _rigidbody;

	// Variables
	private bool isGrounded = true;
	private bool isUsed = false;
	private GameObject playerObject;
	private ushort playerID = 0;


	// Initialization
	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.centerOfMass = centerOfMass.localPosition;
	}
	
	// Once per frame
	void Update()
	{
		UpdateWheelPosition();

		// Stop car?
		if(isUsed && Input.GetKeyDown(KeyCode.Space))
		{
			StopCar();
		}
	}

	// Physics
	void FixedUpdate()
	{

		float groundingForce = _rigidbody.velocity.magnitude * wheelFriction;									// Calculate grounding force

		// Player drives the car?
		if(isUsed)
		{
			float steeringAxis = Input.GetAxis("P" + playerID.ToString() + " Horizontal") * maxSteeringAngle;		// Steering angle through input
			float torque = Input.GetAxis("P" + playerID.ToString() + " Vertical") * maxTorque;						// Torque through input

			// Car steering
			wheelColliders[0].steerAngle = steeringAxis;
			wheelColliders[1].steerAngle = steeringAxis;

			// Car toque
			wheelColliders[0].motorTorque = torque;
			wheelColliders[1].motorTorque = torque;
			wheelColliders[2].motorTorque = torque;
			wheelColliders[3].motorTorque = torque;
		}

		// Grounding force release
		if(!IsGrounded)
			groundingForce *= 0.15f;

		_rigidbody.AddForce(Vector3.down * groundingForce);
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


	// Public Methods

	// Player drives car
	public void StartCar(ushort playerID)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		// Search player which wants to drive
		foreach(GameObject iteratePlayer in players)
		{
			ushort id = iteratePlayer.GetComponent<PlayerPrototype>().PlayerID;

			if(id == playerID)
			{
				this.playerID = playerID;
				this.playerObject = iteratePlayer;
			}
		}

		this.playerObject.SetActive(false);
		this.isUsed = true;
	}

	// Player drives car
	public void StopCar()
	{
		isUsed = false;
		playerObject.SetActive(true);
	}


	// Properties

	// Is car grounded? (Read only)
	public bool IsGrounded
	{
		get
		{
			this.isGrounded = true;

			foreach(WheelCollider currentWheel in wheelColliders)
			{
				this.isGrounded &= currentWheel.isGrounded;
			}

			return this.isGrounded;
		}
	}
}
