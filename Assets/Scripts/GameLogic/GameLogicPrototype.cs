using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OilSpill
{
	public class GameLogicPrototype : MonoBehaviour
	{
		public static GameLogicPrototype Main { get; private set; }

		public bool RaceStarted { get; private set; }

		[SerializeField] private float time;

		[SerializeField] private GameObject timeTextObj;
		[SerializeField] private GameObject moneyTextObj;
		[SerializeField] private GameObject infoTextObj;

		private GameObject[] _cars;
		private GameObject _playerObj;

		private Text infoText;
		private Text timeText;
		private Text moneyText;
		private Coroutine moneyCount;

		private ulong money = 0;
		private bool timerFeedback = false;

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

			// GUI elements
			timeText = timeTextObj.GetComponent<Text>();
			moneyText = moneyTextObj.GetComponent<Text>();

			// Race start countdown
			RaceStarted = false;
			StartCoroutine(StartCountdown());

			// Start money count coroutine
			moneyCount = StartCoroutine(FuelToMoney());
		}
		
		// GUI
		void Update()
		{
			// Count time
			if(RaceStarted)
				time -= Time.deltaTime;
		}

		// GUI Update
		void OnGUI()
		{
			moneyText.text = money.ToString("N0") + "$";
			timeText.text = "Timer: " + time.ToString("F1") + "s";

			if((time <= 10) && !timerFeedback)
			{
				timerFeedback = true;

				// Color and animation
				timeText.color = new Color(0.8f, 0f, 0f);
				StartCoroutine(TimerFeedback());
			}
		}


		// Money per fuel consumption
		IEnumerator FuelToMoney()
		{
			while(true)
			{
				bool driving = !_playerObj.activeInHierarchy;

				if(driving)
				{
					_cars = GameObject.FindGameObjectsWithTag("Vehicle");

					foreach(GameObject car in _cars)
					{
						if(car.GetComponent<Car>().IsUsed)
						{
							money += (uint)(car.GetComponent<Car>().Consumption * 10000f);
							break;
						}
					}
				}

				yield return new WaitForSeconds(0.2f);
			}
		}

		// Not much time left
		IEnumerator TimerFeedback()
		{
			while(time <= 10f)
			{
				StartCoroutine(timeTextObj.GetComponent<TextEffects>().ScaleFeedback());
				yield return new WaitForSeconds(1f);
			}

			// Reset feedback value
			timerFeedback = false;
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
			RaceStarted = true;

		}
	}
}
