using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class HintPrototype : MonoBehaviour
	{
		[SerializeField] protected Texture hintImage;
		[SerializeField] protected int imageWidth;
		[SerializeField] protected float offset = 10f;

		protected Vector2 imageScale; 

		// Use this for initialization
		void Start()
		{
			if(hintImage == null)
			{
				Debug.LogError("[OilSpill] Hint: Assign a Texture in the inspector.");
			}
			else
			{
				if(imageWidth > 0)
				{
					float ratio = (float)imageWidth / hintImage.width;
					
					imageScale = new Vector2(imageWidth, hintImage.height * ratio);
				}
				else
				{
					imageScale = new Vector2(hintImage.width, hintImage.height);
				}
			}
		}
		
		// Update is called once per frame
		void Update()
		{	
		
		}

		void OnGUI()
		{
			Vector3 hintPos = Camera.main.WorldToScreenPoint(transform.position);

			// Position on screen
			float posX = hintPos.x - (imageScale.x / 2f);
			float posY = Screen.height - (hintPos.y + (imageScale.y / 2f)) - offset;

			// Draw image
			GUI.DrawTexture(new Rect(posX, posY, imageScale.x, imageScale.y), hintImage, ScaleMode.ScaleToFit);
		}
	}
}
