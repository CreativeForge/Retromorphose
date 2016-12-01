using UnityEngine;
using System.Collections;

namespace OilSpill
{
	public class PlayerHintPrototype : HintPrototype
	{
		[SerializeField] protected float hintTime;

		protected ushort _playerID;

		private float time = 0;


		// Initialization
		protected override void Start()
		{
			base.Start();

			_playerID = GetComponent<PlayerPrototype>().PlayerID;
		}

		// UI
		protected override void OnGUI()
		{
			if(!GameLogicPrototype.Main.RaceStarted)
				return;
			
			Vector3 hintPos = Camera.main.WorldToScreenPoint(transform.position);

			// Position on screen
			float posX = hintPos.x - (imageScale.x / 2f);
			float posY = Screen.height - (hintPos.y + (imageScale.y / 2f)) - offset;

			// Draw image
			GUI.DrawTexture(new Rect(posX, posY, imageScale.x, imageScale.y), hintImage, ScaleMode.ScaleToFit);
		}

		// Every frame
		protected virtual void Update()
		{
			// Check for Input
			// How long should hint be displayed?
			float input = Mathf.Abs(Input.GetAxis("P" + _playerID.ToString() + " Vertical"));
			input += Mathf.Abs(Input.GetAxis("P" + _playerID.ToString() + " Horizontal"));
			input = Mathf.Clamp01(input);

			// Count time - disable component when done
			if(time > hintTime)
			{
				enabled = false;
			}
			else
			{
				if(GameLogicPrototype.Main.RaceStarted)
				{
					time += input * Time.deltaTime;
				}
			}
		}
	}
}
