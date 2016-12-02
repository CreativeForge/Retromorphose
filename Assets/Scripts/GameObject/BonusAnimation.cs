using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class BonusAnimation : MonoBehaviour
	{
		[SerializeField] protected float scaleFactor = 2f;
		[SerializeField] protected float alphaTime = 1f;
		[SerializeField] protected float scaleTime = 1f;

		protected SpriteRenderer _renderer;

		// Initialization
		void Start()
		{
			_renderer = GetComponent<SpriteRenderer>();
			StartCoroutine(AnimateSprite());
		}
		
		IEnumerator AnimateSprite()
		{
			float elapsedTime = 0f;
			float originalScale = transform.localScale.x;
			Color alphaColor = new Color(1f, 1f, 1f, 0f);
			Color finalColor = new Color(1f, 1f, 1f, 1f);

			// Alpha in
			while(_renderer.color.a < 1f)
			{
				_renderer.color = Color.Lerp(alphaColor, finalColor, (elapsedTime / alphaTime));
				elapsedTime += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}

			elapsedTime = 0f;

			// Scale out
			while(originalScale < (originalScale * scaleFactor))
			{
				transform.localScale = Vector3.Lerp(Vector3.one * originalScale, Vector3.one * (originalScale * scaleFactor), (elapsedTime / scaleTime));
				_renderer.color = Color.Lerp(finalColor, alphaColor, (elapsedTime / scaleTime));
				elapsedTime += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}

			// Destroy gameobject in the end
			Destroy(gameObject);
		}
	}
}
