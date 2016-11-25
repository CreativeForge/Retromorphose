using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
		[SerializeField] private GameObject goalTextObj;
		[SerializeField] private GameObject infoTextObj;
		[SerializeField] private GameObject fuelTankUI;
		[SerializeField] private Image fuelIndicator;
		[SerializeField] private UIBasicAlert alertWindow;

		[SerializeField] private Image[] countdownImages;

		private GameObject[] _cars;
		private GameObject _playerObj;

		private Text infoText;
		private Text timeText;
		private Text moneyText;
		private Text goalText;
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
			goalText = goalTextObj.GetComponent<Text>();

			// GUI write goal
			goalText.text = "Earn at least " + goal.ToString("N0") + "$";
		}
		
		// GUI
		void Update()
		{
			// Count time
			if(RaceStarted && !RaceFinished)
			{
				if(time > 0)
					time -= Time.deltaTime;
				else
					Time.timeScale = 0f;
			}

			// Restart debug
			/*
			if(Input.GetKeyDown(KeyCode.Backspace))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			*/
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

			// Green goal text if reached
			if((money >= goal) && !reachedGoal)
			{
				goalText.color = new Color(0f, 0.7f, 0f);
				reachedGoal = true;
				StartCoroutine(goalTextObj.GetComponent<UIEffects>().DuplicateFade());
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
			while(!RaceFinished)
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
			while((time <= 10f) && !RaceFinished)
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
			// Ready
			StartCoroutine(CountdownEffect(0, 10f));
			yield return new WaitForSeconds(1f);
			// Set
			StartCoroutine(CountdownEffect(1, -10f));
			yield return new WaitForSeconds(1f);
			// Go
			StartCoroutine(CountdownEffect(2, 10f));
			yield return new WaitForSeconds(0.2f);
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

		// Countdown
		public IEnumerator CountdownEffect(int index, float rotation)
		{
			Vector3 newScale = Vector3.one * 5f;
			Quaternion newRotation = Quaternion.Euler(Vector3.forward * rotation);
			float elapsedTime = 0f;

			countdownImages[index].rectTransform.localRotation = newRotation;
			countdownImages[index].rectTransform.localScale = newScale;
			countdownImages[index].gameObject.SetActive(true);

			// Not already scaled
			if(transform.localScale.Equals(Vector3.one))
			{
				// Scale up
				while(elapsedTime < 0.5f)
				{
					countdownImages[index].rectTransform.localScale = Vector3.Lerp(newScale, Vector3.one, elapsedTime * 2);
					countdownImages[index].rectTransform.localRotation = Quaternion.Lerp(newRotation, Quaternion.identity, elapsedTime * 2);
					elapsedTime += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}

				// Wait some more
				yield return new WaitForSeconds(0.5f);
				countdownImages[index].gameObject.SetActive(false);
			}
		}
	}
}
