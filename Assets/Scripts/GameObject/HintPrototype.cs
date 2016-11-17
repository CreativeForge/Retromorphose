using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class HintPrototype : MonoBehaviour
	{
		[SerializeField] protected Texture hintImage;
		[SerializeField] protected float offset = 10f;

		// Use this for initialization
		void Start()
		{
			if(hintImage == null)
				Debug.LogError("[OilSpill] Hint: Assign a Texture in the inspector.");
		}
		
		// Update is called once per frame
		void Update()
		{
		
		}

		void OnGUI()
		{
			Vector3 hintPos = Camera.main.WorldToScreenPoint(transform.position);

			// Position on screen
			float posX = hintPos.x - (hintImage.width / 2f);
			float posY = Screen.height - (hintPos.y - (hintImage.height / 2f)) - offset;

			// Draw image
			GUI.DrawTexture(new Rect(posX, posY, hintImage.width, hintImage.height), hintImage, ScaleMode.ScaleToFit);
		}
	}
}
