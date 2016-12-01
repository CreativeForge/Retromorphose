using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public sealed class Car : Vehicle
	{
		[SerializeField] private GameObject explosion;
		[SerializeField] private float explosionRadius = 10f;
		[SerializeField] private float explosionForce = 10f;
		[SerializeField] private AudioClip[] crashSounds;

		private ParticleSystem _dollarEmitter;
		private AudioSource _audioSource;


		// Start routine
		protected override void Start()
		{
			base.Start();
			_dollarEmitter = GetComponentsInChildren<ParticleSystem>()[1];
			_audioSource = GetComponent<AudioSource>();
		}

		// Every frame
		protected override void Update()
		{
			base.Update();

			if(isUsed)
			{
				float inputVertical = Mathf.Abs(Input.GetAxis("P" + playerID.ToString() + " Vertical"));

				// Dollar amount
				var em = _dollarEmitter.emission;
				var rate = new ParticleSystem.MinMaxCurve(inputVertical * _rigidbody.velocity.magnitude * 0.5f);
				em.rate = rate;
			}
		}

		// On total loss
		protected override void OnTotalLoss(object sender, VehicleEventArgs args)
		{
			base.OnTotalLoss(sender, args);

			Explode();
		}

		// Start vehicle - add damage-bar
		public override void StartVehicle(ushort playerID)
		{
			DamageBar bar = GetComponent<DamageBar>();

			base.StartVehicle(playerID);

			if(bar != null)
				bar.enabled = true;
		}

		// Stop vehicle - remove damage-bar
		public override void StopVehicle()
		{
			DamageBar bar = GetComponent<DamageBar>();

			base.StopVehicle();

			if(bar != null)
				bar.enabled = false;
		}



		/// <summary>
		/// Vehicle explosion.
		/// </summary>
		public override void Explode()
		{
			Instantiate(explosion, transform.position, transform.rotation);

			if(isUsed)
				StopVehicle();

			// Apply explosion force to nearby rigidbodies
			Collider[] nearObjects = Physics.OverlapSphere(transform.position, explosionRadius);

			foreach(Collider nearObject in nearObjects)
			{
				Rigidbody nearRigidbody = nearObject.GetComponent<Rigidbody>();

				if(nearRigidbody != null)
				{
					nearRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2f);
				}
			}

			// Destroy GameObject
			Destroy(gameObject);
		}


		// Collision
		protected override void OnCollisionEnter(Collision collisionInfo)
		{
			switch(collisionInfo.gameObject.tag)
			{
				case "Player":
				case "Invisible":
					break;
				
				// Make damage
				default:
					
					float impact = Vector3.Dot(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity);
					float damageMulti = Mathf.Abs(impact * _rigidbody.mass * 0.05f) * collisionMultiplier;

					if(damageMulti > 5f)
					{
						MakeDamage(damageMulti);
						_audioSource.PlayOneShot(crashSounds[Random.Range(0, crashSounds.Length)]);
					}

					break;
			}
		}


		// Triggers
		private void OnTriggerEnter(Collider other)
		{
			switch(other.tag)
			{
				// Cross finish line
				case "Finish":

					if(isUsed)
						GameLogicPrototype.Main.FinishRace();

					break;

			
				default:
					break;
			}
		}
	}
}
