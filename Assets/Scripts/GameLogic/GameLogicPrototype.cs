using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OilSpill
{
	public class GameLogicPrototype : MonoBehaviour
	{
		public static GameLogicPrototype Main { get; private set; }

		[SerializeField] private float dollarsPerFullTank;

		[SerializeField] private GameObject moneyTextObj;
		[SerializeField] private GameObject infoTextObj;

		private GameObject[] _cars;
		private GameObject _playerObj;

		private Text infoText;
		private Text moneyText;

		private ulong money = 0;

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
			_playerObj = GameObject.FindGameObjectWithTag("Player");

			//Get debug text element
			if(infoTextObj != null)
				infoText = infoTextObj.GetComponent<Text>();

			// Money text element
			moneyText = moneyTextObj.GetComponent<Text>();

			// Race start countdown
			StartCoroutine(StartCountdown());
		}
		
		// GUI
		void Update()
		{
			bool driving = !_playerObj.activeInHierarchy;

			if(driving)
			{
				_cars = GameObject.FindGameObjectsWithTag("Vehicle");

				foreach(GameObject car in _cars)
				{
					if(car.GetComponent<Car>().IsUsed)
					{
						money += (uint)(car.GetComponent<Car>().Consumption > 0f ? 1 : 0) * 100;
						break;
					}
				}
			}
		}

		// GUI Update
		void OnGUI()
		{
			moneyText.text = money.ToString("N0") + "$";
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
