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

	[SerializeField] float fuel = 100f;
	[SerializeField] float damage = 0f;

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

			// Car torque (enough fuel?)
			if(fuel > 0f)
			{
				foreach(WheelCollider iterateWheels in wheelColliders)
					iterateWheels.motorTorque = torque;

				// Fuel consumption
				fuel -= 0.001f * Mathf.Abs(torque);
			}
			else
			{
				foreach(WheelCollider iterateWheels in wheelColliders)
					iterateWheels.motorTorque = 0f;
			}

		}

		// Grounding force release
		if(!IsGrounded)
			groundingForce *= 0.15f;

		_rigidbody.AddForce(Vector3.down * groundingForce);
	}

	// Collision
	void OnCollisionEnter(Collision collisionInfo)
	{
		//print(collisionInfo.contacts[0].normal);
		//print(collisionInfo.relativeVelocity);
		//print(Mathf.Pow(Mathf.Abs(Mathf.Cos(Vector3.Angle(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity))), 2f) * collisionInfo.relativeVelocity.magnitude);
		//Debug.DrawRay(collisionInfo.contacts[0].point, collisionInfo.relativeVelocity);
		//Debug.DrawRay(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal * 10f, Color.green);
		//Debug.LogError("Ray");

		float collisionAngle = Vector3.Angle(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity.normalized);
		float damageMulti = Mathf.Pow(Mathf.Abs(Mathf.Cos(collisionAngle)), 2f) * collisionInfo.relativeVelocity.magnitude;
		print(damageMulti);
		if(damageMulti > 5f)
		{
			damage += damageMulti * 0.5f;
		}

	}

	// Wheel object transform
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
				break;
			}
		}

		this.playerObject.SetActive(false);
		this.isUsed = true;

		// Camera follow
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().ChangeTarget(transform);
	}

	// Player leaves car
	public void StopCar()
	{
		Vector3 hitBoxCenter = transform.FindChild("LeftDoor").GetComponent<Collider>().bounds.center;
		Vector3 hitBoxExtents = transform.FindChild("LeftDoor").GetComponent<Collider>().bounds.extents;

		Collider[] hitColliders = Physics.OverlapBox(hitBoxCenter, hitBoxExtents * 0.5f, transform.rotation);

		// Vehicle door is blocked
		if(hitColliders.Length > 1)
		{
			// Try door on the right
			hitBoxCenter = transform.FindChild("RightDoor").GetComponent<Collider>().bounds.center;
			hitBoxExtents = transform.FindChild("RightDoor").GetComponent<Collider>().bounds.extents;
			hitColliders = Physics.OverlapBox(hitBoxCenter, hitBoxExtents * 0.5f, transform.rotation);

			// Door blocked too
			if(hitColliders.Length > 1)
			{
				return;
			}
		}

		isUsed = false;
		playerObject.transform.position = hitBoxCenter;
		playerObject.transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y));
		playerObject.SetActive(true);

		// Camera follow
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().ChangeTarget(playerObject.transform);

		// Stop car driving physics
		foreach(WheelCollider iterateWheels in wheelColliders)
		{
			iterateWheels.steerAngle = 0f;
			iterateWheels.motorTorque = 0f;
		}
	}

	// Damage car
	public void MakeDamage(float damage)
	{
		this.damage += damage;

		// driveable?
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

	// Vehicle fuel status
	public float Fuel
	{
		get { return this.fuel; }
		set { this.fuel = value; }
	}
}
