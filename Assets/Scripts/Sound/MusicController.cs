using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public sealed class MusicController : MonoBehaviour
	{
		public static MusicController Main { get; private set; }

		// Before load
		void Awake()
		{
			// Existing MusicController?
			if(Main != null)
			{
				print("[OilSpill] MusicController: Destroying doubles.");
				Destroy(gameObject);
			}
			else
			{
				Main = this;

				// Do not destroy this music controller
				DontDestroyOnLoad(gameObject);

				print("[OilSpill] " + MusicController.Main.name + "is now assigned as MusicController.");
			}
		}
	}
}
