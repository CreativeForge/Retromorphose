using UnityEngine;
using System.Collections;

public class SmokeEmitter : MonoBehaviour
{
	// Serialized
	[SerializeField] private Transform emitter;
	[SerializeField] private GameObject[] smokeObjects;
	[SerializeField] private uint smokeAmount = 10;
	[SerializeField] private ushort smokeMultiplier = 1;
	[SerializeField] private bool startOnAwake = true;

	// Variables
	private bool emitting;
	private float time = 0f;

	// Initialization
	void Start()
	{
		emitting = startOnAwake;
	}

	// Update
	void Update()
	{
		if(emitting)
		{
			// Emit smoke
			if((1f / (float)smokeAmount) < time)
			{
				for(ushort i = 0;i < smokeMultiplier;i++)
					Instantiate(smokeObjects[Random.Range(0, smokeObjects.Length)], emitter.position, Random.rotation);
				
				time = 0f;
			}
			else
			{
				time += Time.deltaTime;
			}
		}
	}


	// Public methods

	// Set active
	public void SetActive(bool active)
	{
		emitting = active;
	}

	// Properties
	public uint Amount
	{
		get { return this.smokeAmount; }
		set { this.smokeAmount = value; }
	}
}
