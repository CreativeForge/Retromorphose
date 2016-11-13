using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OilSpill
{
	public class GameLogicPrototype : MonoBehaviour
	{
		public static GameLogicPrototype Main { get; private set; }

		[SerializeField] private GameObject infoTextObj;

		private GameObject[] _cars;
		private GameObject _playerObj;

		private Text infoText;

		// Before load
		void Awake()
		{
			// Existing GameLogic?
			if(Main != null)
			{
				print("[OilSpill] GameLogic: Destroying doubles.");
				Destroy(this);
			}
			else
			{
				Main = this;

				// Do not destroy this game logic
				DontDestroyOnLoad(gameObject);

				print("[OilSpill] " + GameLogicPrototype.Main.name + "is now assigned as GameLogic.");
			}

		}

		// Initialization
		void Start()
		{
			// Debug car info output
			_cars = GameObject.FindGameObjectsWithTag("Vehicle");
			infoText = infoTextObj.GetComponent<Text>();

			_playerObj = GameObject.FindGameObjectWithTag("Player");

			// Race start countdown
			StartCoroutine(StartCountdown());
		}
		
		// GUI
		void Update()
		{
			/*
			infoText.text = "Car 1 - Fuel: " + _cars[0].GetComponent<Car>().Fuel.ToString("F0") +
				"\nCar 2 - Fuel: " + _cars[1].GetComponent<Car>().Fuel.ToString("F0");
			*/
		}

		// Race countdown
		IEnumerator StartCountdown()
		{
			// 3
			print(3);
			yield return new WaitForSeconds(1f);
			// 2
			print(2);
			yield return new WaitForSeconds(1f);
			// 1
			print(1);
			yield return new WaitForSeconds(1f);
			// Go
			print("GO!");
			_playerObj.GetComponent<PlayerPrototype>().CanMove = true;

		}
	}
}
