using UnityEngine;
using System;
using System.Collections;

namespace OilSpill
{
	// Public class used for the arguments for Vehicle-events
	public class VehicleEventArgs : EventArgs
	{
		// GameObject name and reference as arguments
		public string Name { get; private set; }
		public GameObject VehicleObj { get; private set; }

		// Constructor
		public VehicleEventArgs(string name, GameObject reference)
		{
			this.Name = name;
			this.VehicleObj = reference;
		}
	}
}
