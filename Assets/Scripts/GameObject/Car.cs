using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public sealed class Car : Vehicle
	{
		/// <summary>
		/// Vehicle explosion.
		/// </summary>
		public override void Explode()
		{
			// Boom
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
					
					float collisionAngle = Vector3.Angle(collisionInfo.contacts[0].normal, collisionInfo.relativeVelocity.normalized);
					float damageMulti = Mathf.Pow(Mathf.Abs(Mathf.Cos(collisionAngle)), 2f) * collisionInfo.relativeVelocity.magnitude;

					//damageMulti *= _rigidbody.velocity.normalized.magnitude;

					if(damageMulti > 5f)
					{
						MakeDamage(damageMulti * 0.5f);
					}

					break;
			}
		}

	}
}
