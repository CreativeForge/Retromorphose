using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OilSpill
{
	public class GameLogicPrototype : MonoBehaviour
	{
		public static GameLogicPrototype Main { get; private set; }

		public bool RaceStarted { get; private set; }
		public bool RaceFinished { get; private set; }

		[SerializeField] private float time;
		[SerializeField] private ulong goal;

		[SerializeField] private GameObject timeTextObj;
		[SerializeField] private GameObject moneyTextObj;
		[SerializeField] private GameObject infoTextObj;
		[SerializeField] private GameObject fuelTankUI;
		[SerializeField] private Image fuelIndicator;
		[SerializeField] private UIBasicAlert alertWindow;

		private GameObject[] _cars;
		private GameObject _playerObj;

		private Text infoText;
		private Text timeText;
		private Text moneyText;
		private Coroutine moneyCount;

		private ulong money = 0;
		private uint moneyTenThousand = 0;			// Counts the ten-thousands money - for money feedback
		private bool timerFeedback = false;
		private bool reachedGoal = false;

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

			// Default values properties
			RaceStarted = false;
			RaceFinished = false;
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
		}
		
		// GUI
		void Update()
		{
			// Count time
			if(RaceStarted)
			{
				if(time > 0)
					time -= Time.deltaTime;
				else
					Time.timeScale = 0f;
			}
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

			// Money feedback? Every ten-thousand
			if((money / 10000) > moneyTenThousand)
			{
				moneyTenThousand = (uint)(money / 10000);
				StartCoroutine(moneyTextObj.GetComponent<UIEffects>().DuplicateFade());
			}

			// Fuel indicator
			bool driving = !_playerObj.activeInHierarchy;

			if(driving)
			{
				if(!fuelTankUI.activeInHierarchy)
					fuelTankUI.SetActive(true);

				// Get driving car
				_cars = GameObject.FindGameObjectsWithTag("Vehicle");

				// Iterate through cars
				foreach(GameObject car in _cars)
				{
					// Is car used?
					if(car.GetComponent<Car>().IsUsed)
					{
						// Add money for fuel consumption
						fuelIndicator.rectTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, (car.GetComponent<Car>().Fuel / 100f) * 106f - 53f));
						break;
					}
				}
			}
			else if(fuelTankUI.activeInHierarchy)
				fuelTankUI.SetActive(false);
		}


		// Start eace
		public void StartRace()
		{
			if(RaceStarted)
				return;
			
			// Race start countdown
			RaceStarted = false;
			StartCoroutine(StartCountdown());

			// Start money count coroutine
			moneyCount = StartCoroutine(FuelToMoney());
		}

		// Finish line
		public void FinishRace()
		{
			if(RaceFinished)
				return;
			
			bool driving = !_playerObj.activeInHierarchy;

			RaceFinished = true;
			print("[OilSpill] Reached the finish line!");

			// If car is driving
			if(driving)
			{
				_cars = GameObject.FindGameObjectsWithTag("Vehicle");

				// Iterate through cars
				foreach(GameObject car in _cars)
				{
					// Is car used?
					if(car.GetComponent<Car>().IsUsed)
					{
						// Stop car after 1 second
						StartCoroutine(ExitCarFinish(1f, car));
						break;
					}
				}
			}
			else
			{
				_playerObj.GetComponent<PlayerPrototype>().CanMove = false;
			}

			// Reached goal?
			if((ulong)(time * 1000f) + money >= goal)
			{
				alertWindow.Title = "Congratulations!";
				alertWindow.ButtonText = "Next";
			}
			else
			{
				alertWindow.Title = "Mission failed!";
				alertWindow.ButtonText = "Retry";
			}

			// Display text
			alertWindow.Text = "Earned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
				"\nTime bonus:\t\t\t\t\t\t\t\t" + (time * 1000f).ToString("N0") + "$" +
				"\n\nTotal:\t\t\t\t\t\t\t\t\t\t\t" + ((time * 1000f) + money).ToString("N0") + "$";

			// Calculate money
			money += (ulong)(time * 1000f);

			// Display end screen
			Invoke("EndScreen", 2f);

		}

		// End screen
		void EndScreen()
		{
			time = 0f;
			alertWindow.gameObject.SetActive(true);
		}



		// Money per fuel consumption
		IEnumerator FuelToMoney()
		{
			while(true)
			{
				bool driving = !_playerObj.activeInHierarchy;

				// If car is driving
				if(driving)
				{
					_cars = GameObject.FindGameObjectsWithTag("Vehicle");

					// Iterate through cars
					foreach(GameObject car in _cars)
					{
						// Is car used?
						if(car.GetComponent<Car>().IsUsed)
						{
							// Add money for fuel consumption
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
				StartCoroutine(timeTextObj.GetComponent<UIEffects>().ScaleFeedback());
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

		// Exit the car after delay and finish race
		IEnumerator ExitCarFinish(float delay, GameObject car)
		{
			yield return new WaitForSeconds(delay);

			if(car != null)
			{
				car.GetComponent<Car>().StopVehicle();
				_playerObj.GetComponent<PlayerPrototype>().CanMove = false;
			}
		}
	}
}
