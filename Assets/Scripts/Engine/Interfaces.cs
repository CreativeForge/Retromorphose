using UnityEngine;
using System;
using System.Collections;

namespace OilSpill
{
	/// <summary>Damageable object.</summary>
	interface IDamageable<T>
	{
		// Properties
		float Damage { get; }

		// Methods
		void MakeDamage(T damage);

		// Events
		event EventHandler<VehicleEventArgs> ReceiveDamage;
	}

	/// <summary>Explodable object.</summary>
	interface IExplodable
	{
		// Methods
		void Explode();
	}
}
