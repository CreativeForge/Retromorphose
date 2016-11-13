using UnityEngine;
using System.Collections;

public class BigExplosion : MonoBehaviour
{
	// Initialization
	void Start()
	{
		Destroy(gameObject, 4f);
	}
}
