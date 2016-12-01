using UnityEngine;
using System.Collections;

public class DestroyExplosion : MonoBehaviour
{
	// Initialization
	void Start()
	{
		Destroy(gameObject, 4f);
	}
}
