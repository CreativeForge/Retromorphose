using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class PlayerPrototype : MonoBehaviour
	{
		[SerializeField] private ushort playerID = 1;
		[SerializeField] private float walkSpeed = 5f;
		[SerializeField] private float turnSpeed = 3f;
		[SerializeField] private float jumpForce = 3f;
		[SerializeField] private float interactionRadius = 0.8f;

		// On initialization
		private Rigidbody _rigidbody;
		private Animator _anim;
		private SmokeEmitter _smokeEmitter;

		// Variables
		private bool isGrounded = false;		// Is player grounded?
		private uint ignoreGrounded = 0;		// How many frames should ground collision be ignored? (isGrounded fix)
		private bool actionButton = false;		// Action-button
		private bool smokeEmitter = false;		// Should emit smoke?
		private bool canMove = false;			// Is player freezed?
		private Transform vehicle = null;		// Vehicle in range?


		// Initialization
		void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_anim = GetComponent<Animator>();
			_smokeEmitter = GetComponent<SmokeEmitter>();
		}

		void Update()
		{
			bool vehicleInRange = false;
			ushort vehicleCount = 0;

			// Vehicle in range?
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRadius);

			foreach(Collider nearObject in hitColliders)
			{
				if(nearObject.tag == "VehicleDoor")
				{
					vehicleCount++;

					// Can drive that car
					vehicleInRange = true;

					// Old car?
					if(vehicle != null)
					{
						// Display driving hint if there is one
						if((vehicle.GetComponent<HintPrototype>() != null) &&
							!GameLogicPrototype.Main.RaceFinished)
						{
							// Hide old vehicle (out of range) driving-hint if new car
							if(!nearObject.transform.parent.Equals(vehicle))
							{
								vehicle.GetComponent<HintPrototype>().enabled = false;
							}
							else
							{
								// Same car
								vehicle.GetComponent<HintPrototype>().enabled = true;
								break;
							}
						}
					}

					// Assign new vehicle
					vehicle = nearObject.transform.parent;

					// Display driving hint if there is one
					if((vehicle.GetComponent<HintPrototype>() != null) &&
					   !GameLogicPrototype.Main.RaceFinished)
					{
						vehicle.GetComponent<HintPrototype>().enabled = true;
					}
					
					break;
				}
			}

			// No vehicle in range?
			if((vehicleCount == 0) && (vehicle != null))
			{
				// Hide old vehicle hint if there is one
				if(vehicle.GetComponent<HintPrototype>() != null)
					vehicle.GetComponent<HintPrototype>().enabled = false;
			}

			// Action-Button
			if(actionButton || Input.GetKeyDown(KeyCode.Space))
			{
				actionButton = false;

				// Drive?
				if(vehicleInRange)
				{
					if(vehicle != null)
					{
						isGrounded = false;

						// Hide vehicle hint if there is one
						if(vehicle.GetComponent<HintPrototype>() != null)
							vehicle.GetComponent<HintPrototype>().enabled = false;

						// Start vehicle
						vehicle.GetComponent<Vehicle>().StartVehicle(playerID);
					}
				}
				// Jump?
				else if(isGrounded)
				{
					_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, jumpForce, _rigidbody.velocity.z);
					isGrounded = false;
					ignoreGrounded = 5;
				}
			}

			// Collision fix: isGrounded detection
			if(ignoreGrounded > 0)
				ignoreGrounded--;
		}
		
		// Physics
		void FixedUpdate()
		{
			// Player movement
			if(!canMove)
				return;

			Vector3 direction = new Vector3(-Input.GetAxis("P" + playerID.ToString() + " Vertical"), 0f, Input.GetAxis("P" + playerID.ToString() + " Horizontal"));

			// Walking player should emit smoke
			smokeEmitter = direction.magnitude > 0f ? true : false;

			// Animation
			_anim.SetFloat("Speed", direction.normalized.magnitude);

			// Rotation
			Quaternion targetRotation = Quaternion.LookRotation(direction.magnitude != 0f ? direction.normalized : transform.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);

			// Position, Gravity
			if(!isGrounded)
			{
				_rigidbody.velocity -= transform.up;
			}
			else
			{
				_rigidbody.velocity = transform.forward * walkSpeed * direction.magnitude;

				// Emit smoke?
				if(smokeEmitter)
					_smokeEmitter.SetActive(true);
				else
					_smokeEmitter.SetActive(false);
			}
		}

		// Collision handling
		void OnCollisionStay(Collision collisionInfo)
		{
			bool collisionFeet = false;

			foreach(ContactPoint contact in collisionInfo.contacts)
			{
				if(contact.normal.y > 0f)
					collisionFeet = true;
			}

			if(collisionFeet && !isGrounded)
			{
				// Ignore grounded? Used for jumping...
				if(ignoreGrounded == 0)
				{
					isGrounded = true;
				}
			}
		}

		void OnCollisionExit(Collision collisionInfo)
		{
			isGrounded = false;
			_smokeEmitter.SetActive(false);
		}

		// Triggers
		void OnTriggerEnter(Collider other)
		{
			switch(other.tag)
			{
				case "Finish":
					GameLogicPrototype.Main.FinishRace();
					break;

				default:
					break;
			}
		}



		// Public methods

		// Virtually invert action-button
		public void ActionButton()
		{
			actionButton ^= true;
		}


		// Properties

		public ushort PlayerID
		{
			get { return this.playerID;}
		}
		
		public bool IsGrounded
		{
			get { return this.isGrounded; }
		}

		public bool CanMove
		{
			get { return this.canMove; }
			set
			{
				this.canMove = value;

				// Animation and smoke
				if(!value)
				{
					_anim.SetFloat("Speed", 0f);
					smokeEmitter = false;
					_smokeEmitter.SetActive(false);
				}
			}
		}
	}
}
