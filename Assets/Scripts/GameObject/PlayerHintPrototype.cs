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

		// Every frame
		protected virtual void Update()
		{	
			// Check for Input
			// How long should hint be displayed?

			float input = Mathf.Abs(Input.GetAxis("P" + _playerID.ToString() + " Vertical"));

			// Count time - disable component when done
			if(time > hintTime)
			{
				enabled = false;
			}
			else
			{
				if(GameLogicPrototype.Main.RaceStarted)
					time += input * Time.deltaTime;
			}
		}
	}
}
