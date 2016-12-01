using UnityEngine;
using System.Collections;

public class UIEffects : MonoBehaviour
{
	[SerializeField] protected float scaleTime = 0.3f;
	[SerializeField] protected float scaleSize = 1.3f;
	[SerializeField] protected float duplicateTime = 1f;
	[SerializeField] protected float duplicateSize = 2f;

	// Every frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			StartCoroutine(DuplicateFade());
		}
	}

	// Scale up and back down
	public IEnumerator ScaleFeedback()
	{
		Vector3 newScale = Vector3.one * scaleSize;
		float elapsedTime = 0f;

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
			elapsedTime = 0f;

			while(elapsedTime < scaleTime)
			{
				transform.localScale = Vector3.Lerp(newScale, Vector3.one, (elapsedTime / scaleTime));
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			transform.localScale = Vector3.one;
		}
	}

	// Duplicate and fade effect
	public IEnumerator DuplicateFade()
	{
		GameObject duplicate = Instantiate(gameObject, transform.parent) as GameObject;
		Vector3 newScale = Vector3.one * duplicateSize;
		float newAlpha = 1f;
		float elapsedTime = 0f;

		// Remove this script from duplicate
		Destroy(duplicate.GetComponent<UIEffects>());

		// Scale up and fade
		while(elapsedTime < duplicateTime)
		{
			duplicate.transform.localScale = Vector3.Lerp(Vector3.one, newScale, (elapsedTime / duplicateTime));
			newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime / duplicateTime));

			duplicate.GetComponent<CanvasRenderer>().SetAlpha(newAlpha);
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		// Destroy duplicate
		Destroy(duplicate);
	}
}
