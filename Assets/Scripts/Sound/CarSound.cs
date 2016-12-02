using UnityEngine;
using System.Collections;

namespace OilSpill
{
	[RequireComponent(typeof(AudioSource))]
	public class CarSound : MonoBehaviour
	{
		[SerializeField] protected float pitchInfluence = 1f;
		[SerializeField] protected AudioClip startCar;
		[SerializeField] protected AudioClip stopCar;

		protected Car _car;
		protected Rigidbody _rigidbody;
		protected AudioSource _sound;

		protected float mainPitch;
		protected bool carUsedBefore = false;

		// Use this for initialization
		void Start()
		{
			_car = GetComponent<Car>();
			_sound = GetComponent<AudioSource>();

			mainPitch = _sound.pitch;

			// Out of fuel event
			_car.NoFuel += OutOfFuel;
		}
		
		// Update is called once per frame
		void Update()
		{
			if(_car.IsUsed)
			{
				float torque = Mathf.Abs(Input.GetAxis("P1 Vertical"));
				float rpm = Mathf.Abs(_car.RPM);

				float pitch = mainPitch + (torque * (rpm / 2000f) * pitchInfluence);

				_sound.pitch = pitch;

				if(!carUsedBefore)
				{
					// Start car
					if(_car.Fuel > 0)
					{
						_sound.PlayOneShot(startCar);
						_sound.PlayDelayed(0.5f);
					}

					carUsedBefore = true;
				}
			}
			else if(carUsedBefore)
			{
				_sound.Stop();

				if(_car.Fuel > 0)
					_sound.PlayOneShot(stopCar);
				
				carUsedBefore = false;
			}
		}

		// When car out of fuel
		protected void OutOfFuel(object sender, VehicleEventArgs e)
		{
			_sound.Stop();
			_sound.PlayOneShot(stopCar);
		}
	}
}
