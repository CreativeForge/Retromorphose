using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class Barrel : MonoBehaviour, IExplodable
	{
		[SerializeField] protected float maxHitForce = 20f;
		[SerializeField] protected GameObject explosion;
		[SerializeField] protected float explosionRadius = 5f;
		[SerializeField] protected float explosionForce = 5f;
		[SerializeField] private GameObject explosionBonus;
		[SerializeField] private ulong bonusValue = 2000;

		// Collision
		protected void OnCollisionEnter(Collision colisionInfo)
		{
			// Too much force... explode
			if(colisionInfo.relativeVelocity.magnitude >= maxHitForce)
			{
				Invoke("Explode", Random.Range(0.1f, 0.5f));
			}
		}

		/// <summary>
		/// Barrel explosion.
		/// </summary>
		public void Explode()
		{
			Instantiate(explosion, transform.position, transform.rotation);
			Instantiate(explosionBonus, transform.position + (Vector3.up * 4f), Quaternion.Euler(Vector3.down * 90f));

			// Add explosion bonus
			GameLogicPrototype.Main.BonusMoney(bonusValue);

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
	}
}
