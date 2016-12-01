using UnityEngine;
using System.Collections;

namespace OilSpill
{
	[RequireComponent(typeof(AudioSource))]
	public class PlayerSound : MonoBehaviour
	{
		[SerializeField] protected AudioClip[] footsteps;

		protected AudioSource _sound;

		// Use this for initialization
		void Start()
		{
			_sound = GetComponent<AudioSource>();
		}

		// Play footstep sound
		public void PlayFootstep()
		{
			int index = Random.Range(0, footsteps.Length);
			float pitch = 1f + Random.Range(-0.1f, 0.1f);

			_sound.pitch = pitch;
			_sound.PlayOneShot(footsteps[index]);
		}
	}
}
