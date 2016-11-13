using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public sealed class Car : Vehicle
	{
		[SerializeField] private GameObject explosion;
		[SerializeField] private float explosionRadius = 10f;
		[SerializeField] private float explosionForce = 10f;

		// On total loss
		protected override void OnTotalLoss(object sender, VehicleEventArgs args)
		{
			base.OnTotalLoss(sender, args);

			Explode();
		}


		/// <summary>
		/// Vehicle explosion.
		/// </summary>
		public override void Explode()
		{
			Instantiate(explosion, transform.position, transform.rotation);
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
			// Make damage
			switch(collisionInfo.gameObject.tag)
			{
				case "Player":
					break;

				default:
					
					float impact = Vector3.Dot(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity);
					float damageMulti = Mathf.Abs(impact * _rigidbody.mass * 0.05f) * collisionMultiplier;

					if(damageMulti > 5f)
					{
						MakeDamage(damageMulti);
					}

					break;
			}
		}

	}
}
