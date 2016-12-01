using UnityEngine;
using System;
using System.Collections;

namespace OilSpill
{
	public abstract class Vehicle : MonoBehaviour, IDamageable<float>, IExplodable
	{
		// Serialized
		[SerializeField] protected float maxSteeringAngle = 30f;
		[SerializeField] protected float maxTorque = 100f;
		[SerializeField] protected float wheelFriction = 10f;

		[SerializeField] protected Transform centerOfMass;

		[SerializeField] protected WheelCollider[] wheelColliders = new WheelCollider[4];
		[SerializeField] protected Transform[] wheels = new Transform[4];

		[SerializeField] protected float fuel = 100f;
		[SerializeField] protected float consumptionMultiplier = 1f;
		[SerializeField] protected float damage = 0f;
		[SerializeField] protected float collisionMultiplier = 1f;

		// On initialization
		protected Rigidbody _rigidbody;
		protected ParticleSystem _smokeEmitter;

		// Variables
		protected bool isGrounded = true;
		protected bool isUsed = false;
		protected GameObject playerObject;
		protected ushort playerID = 0;
		protected float smokeRate = 0f;
		protected float fuelConsumption = 0;

		protected bool noFuel = false;
		protected bool totalLoss = false;


		// Events

		/// <summary>Occurs when out of fuel.</summary>
		public event EventHandler<VehicleEventArgs> NoFuel;

		/// <summary>Occurs when receive damage.</summary>
		public event EventHandler<VehicleEventArgs> ReceiveDamage;

		/// <summary>Occurs when vehicle damage reaches total loss.</summary>
		public event EventHandler<VehicleEventArgs> TotalLoss;


		// Awake
		void Awake()
		{
			// Subscribe events
			NoFuel += OutOfFuel;
			ReceiveDamage += OnDamage;
			TotalLoss += OnTotalLoss;
		}

		// Initialization
		protected virtual void Start()
		{
			// Get components
			_smokeEmitter = GetComponentsInChildren<ParticleSystem>()[0];
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.centerOfMass = centerOfMass.localPosition;
		}
		
		// Every frame
		protected virtual void Update()
		{
			UpdateWheelPosition();

			// Action-button while vehicle is in use?
			if(isUsed && Input.GetKeyDown(KeyCode.Space))
			{
				ActionButton();
			}
		}

		// Physics
		void FixedUpdate()
		{
			// Calculate grounding force
			float groundingForce = _rigidbody.velocity.magnitude * wheelFriction;

			// Player drives the vehicle?
			if(isUsed)
			{
				float steeringAxis = Input.GetAxis("P" + playerID.ToString() + " Horizontal") * maxSteeringAngle;		// Steering angle through input
				float torque = Input.GetAxis("P" + playerID.ToString() + " Vertical") * maxTorque;						// Torque through input

				// Vehicle steering
				wheelColliders[0].steerAngle = steeringAxis;
				wheelColliders[1].steerAngle = steeringAxis;

				// Vehicle torque (enough fuel? no total loss?)
				if(fuel > 0f)
				{
					if(!totalLoss)
					{
						foreach(WheelCollider iterateWheels in wheelColliders)
							iterateWheels.motorTorque = torque;

						// Fuel consumption
						fuelConsumption = 0.001f * Mathf.Abs(torque) * consumptionMultiplier;
						fuel -= fuelConsumption;

						// Smoke amount
						var em = _smokeEmitter.emission;
						var rate = new ParticleSystem.MinMaxCurve(5f + _rigidbody.velocity.magnitude);
						em.rate = rate;
					}

					if(noFuel)
						noFuel = false;
				}
				else
				{
					// No fuel left

					if(!noFuel)
					{
						noFuel = true;
						fuelConsumption = 0f;

						// Fire 'NoFuel'-event
						if(NoFuel != null)
							NoFuel(this, new VehicleEventArgs(gameObject.name, gameObject));
					}
				}
			}

			// Grounding force release
			if(!IsGrounded)
				groundingForce *= 0.15f;

			_rigidbody.AddForce(Vector3.down * groundingForce);
		}

		// Collision
		protected virtual void OnCollisionEnter(Collision collisionInfo)
		{
			// Make damage
			if(collisionInfo.gameObject.tag != "Player")
			{
				float impact = Vector3.Dot(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity);
				float damageMulti = Mathf.Abs(impact * _rigidbody.mass * 0.05f) * collisionMultiplier;

				if(damageMulti > 5f)
				{
					MakeDamage(damageMulti * 0.5f);
				}
			}
		}


		// Own methods

		/// <summary>
		/// Updates the wheel position and transform to match the wheel colliders.
		/// </summary>
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


		// From events invoked methods

		/// <summary>
		/// Invoked when vehicle out of fuel.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="args">Event arguments.</param>
		protected virtual void OutOfFuel(object sender, VehicleEventArgs args)
		{
			// Stop vehicle
			foreach(WheelCollider iterateWheels in wheelColliders)
				iterateWheels.motorTorque = 0f;

			// Stop smoke
			var em = _smokeEmitter.emission;
			var rate = new ParticleSystem.MinMaxCurve(0f);
			em.rate = rate;

			if(Debug.isDebugBuild)
				Debug.Log("[OilSpill] Vehicle: '" + args.Name + "' ran out of fuel.");

		}

		/// <summary>
		/// Invoked when vehicle receives damage.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="args">Event arguments.</param>
		protected virtual void OnDamage(object sender, VehicleEventArgs args)
		{
			/*
			if(Debug.isDebugBuild)
				Debug.Log("[OilSpill] Vehicle: '" + args.Name + "' damaged.");
			*/
		}

		/// <summary>
		/// Invoked when vehicle damage reaches total loss.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="args">Event arguments.</param>
		protected virtual void OnTotalLoss(object sender, VehicleEventArgs args)
		{
			// Stop vehicle
			foreach(WheelCollider iterateWheels in wheelColliders)
				iterateWheels.motorTorque = 0f;

			// Stop smoke
			var em = _smokeEmitter.emission;
			var rate = new ParticleSystem.MinMaxCurve(0f);
			em.rate = rate;

			// Total loss
			totalLoss = true;

			if(Debug.isDebugBuild)
				Debug.Log("[OilSpill] Vehicle: '" + args.Name + "' suffered a total loss.");
		}


		// Public methods

		/// <summary>
		/// Starts the vehicle.
		/// </summary>
		/// <param name="playerID">ID of the player that starts the vehicle.</param>
		public virtual void StartVehicle(ushort playerID)
		{
			// Don't start car when timeScale is zero
			if(Time.timeScale == 0f)
				return;
			
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

			// Search player who wants to drive
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

			// Change to vehicle
			this.playerObject.SetActive(false);
			//this.isUsed = true;
			StartCoroutine(ReleaseVehicle());
			_smokeEmitter.Play();

			// Camera follow
			GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().ChangeTarget(transform);
		}

		/// <summary>
		/// Stops the vehicle.
		/// </summary>
		public virtual void StopVehicle()
		{
			// Don't start car when timeScale is zero
			if(Time.timeScale == 0f)
				return;
			
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

			// Stop vehicle driving physics
			foreach(WheelCollider iterateWheels in wheelColliders)
			{
				iterateWheels.steerAngle = 0f;
				iterateWheels.motorTorque = 0f;
			}

			// Stop smoke
			_smokeEmitter.Stop();
		}

		/// <summary>
		/// Enables driving. Used for coroutines.
		/// </summary>
		protected IEnumerator ReleaseVehicle()
		{
			yield return new WaitForEndOfFrame();
			this.isUsed = true;
		}

		/// <summary>
		/// Virtually press action-button.
		/// </summary>
		public virtual void ActionButton()
		{
			// Stop the vehicle
			StopVehicle();
		}

		/// <summary>
		/// Make damage on vehicle.
		/// </summary>
		public virtual void MakeDamage(float damage)
		{
			this.damage += damage;

			// Total loss?
			if(this.damage >= 100f)
			{
				// Fire 'TotalLoss'-event
				if(TotalLoss != null)
					TotalLoss(this, new VehicleEventArgs(gameObject.name, gameObject));
			}
			else
			{
				// Fire 'ReceiveDamage'-event
				if(ReceiveDamage != null)
					ReceiveDamage(this, new VehicleEventArgs(gameObject.name, gameObject));
			}
		}

		/// <summary>
		/// Vehicle explode.
		/// </summary>
		public abstract void Explode();


		// Properties

		/// <summary>
		/// Gets a value indicating whether this vehicle is used.
		/// </summary>
		/// <value><c>true</c> if this vehicle is used; otherwise, <c>false</c>.</value>
		public bool IsUsed
		{
			get { return this.isUsed; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is grounded.
		/// </summary>
		/// <value><c>true</c> if this instance is grounded; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets or sets the amount of fuel.
		/// </summary>
		/// <value>The amount fuel.</value>
		public float Fuel
		{
			get { return this.fuel; }
			set { this.fuel = value; }
		}

		/// <summary>
		/// Gets the fuel consumption per fixed frame.
		/// </summary>
		/// <value>The fuel consumption amount.</value>
		public float Consumption
		{
			get { return this.fuelConsumption; }
		}

		/// <summary>
		/// Gets the damage.
		/// </summary>
		/// <value>The damage.</value>
		public float Damage
		{
			get { return this.damage; }
		}

		/// <summary>
		/// Gets the cars rpm.
		/// </summary>
		/// <value>Current RPM.</value>
		public float RPM
		{
			get { return (wheelColliders[2].rpm > wheelColliders[3].rpm) ? wheelColliders[2].rpm : wheelColliders[3].rpm; }
		}
	}
}
