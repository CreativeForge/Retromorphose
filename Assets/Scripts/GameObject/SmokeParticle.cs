using UnityEngine;
using System.Collections;

public class SmokeParticle : MonoBehaviour
{
	// Serialized
	[SerializeField] private float lifeTime = 2f;
	[SerializeField] private float sinkingSpeed = 0.2f;
	[SerializeField] private float growingSpeed = 50f;
	[SerializeField] private float smokeSize = 10f;

	// Variables
	private float startTime;
	private Vector3 randomVector;

	// Initialization
	void Start()
	{
		randomVector = new Vector3(Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f), Random.Range(0.3f, 0.5f));
		startTime = Time.time;
		Destroy(gameObject, lifeTime);
	}
	
	// Update
	void Update()
	{
		transform.localScale = Vector3.Slerp(transform.localScale, randomVector * smokeSize, (Time.time - startTime) / (500f / growingSpeed));

		transform.position += Vector3.down * Time.deltaTime * sinkingSpeed;
	}
}
