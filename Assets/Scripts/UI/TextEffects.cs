﻿using UnityEngine;
using System.Collections;

public class TextEffects : MonoBehaviour
{
	[SerializeField] protected float scaleTime = 0.3f;
	[SerializeField] protected float scaleSize = 1.3f;

	// Every frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
			StartCoroutine(ScaleFeedback());
	}

	// Scale up and back down
	public IEnumerator ScaleFeedback()
	{
		Vector3 newScale = Vector3.one * scaleSize;
		float elapsedTime = 0;

		// Not already scaled
		if(transform.localScale.Equals(Vector3.one))
		{
			// Scale up
			while(elapsedTime < scaleTime)
			{
				transform.localScale = Vector3.Lerp(Vector3.one, newScale, (elapsedTime / scaleTime));
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			// Scale down
			transform.localScale = newScale;
			elapsedTime = 0;

			while(elapsedTime < scaleTime)
			{
				transform.localScale = Vector3.Lerp(newScale, Vector3.one, (elapsedTime / scaleTime));
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			transform.localScale = Vector3.one;
		}
	}
}
