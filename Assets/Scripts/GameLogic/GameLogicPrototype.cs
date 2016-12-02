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
		[SerializeField] private bool timeLimit = false;

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
		private float startTime = 0f;

		// Before load
		void Awake()
		{
			// Existing GameLogic?
			// [Edit] not master logic...
			Main = this;
			/*
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
			*/

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

			// Set start time
			startTime = time;
		}
		
		// GUI
		void Update()
		{
			// Count time
			if(RaceStarted && !RaceFinished)
			{
				if(timeLimit)
				{
					time -= Time.deltaTime;
				}
				else
				{
					time += Time.deltaTime;
				}
			}

			// Restart debug
			if(Input.GetKeyDown(KeyCode.Escape))
				SceneManager.LoadScene("Menu");

			if(Input.GetKeyDown(KeyCode.Alpha1))
				SceneManager.LoadScene("Track1");

			if(Input.GetKeyDown(KeyCode.Alpha2))
				SceneManager.LoadScene("Track2");

			if(Input.GetKeyDown(KeyCode.Alpha3))
				SceneManager.LoadScene("Track3");
			
		}

		// GUI Update
		void OnGUI()
		{
			moneyText.text = money.ToString("N0") + "$";
			timeText.text = "Time: " + time.ToString("F1") + "s";

			// Timer feedback
			if((time <= 10) && timeLimit && !timerFeedback)
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


		// Load new or retry old level
		public void LoadLevel()
		{
			// Start race if not started yet?
			if(!RaceStarted)
			{
				StartRace();
			}
			// Load new or retry current level
			else if(RaceFinished)
			{
				int nextSceneIndex = SceneManager.GetActiveScene().buildIndex;

				if(reachedGoal)
				{
					nextSceneIndex++;
					ResetLogic();

					if(SceneManager.sceneCountInBuildSettings > nextSceneIndex)
						SceneManager.LoadScene(nextSceneIndex);
					else
						SceneManager.LoadScene("Menu");
				}
				else
				{
					ResetLogic();
					SceneManager.LoadScene(nextSceneIndex);
				}
			}
		}

		// Start race
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

			// Played with time limit?
			if(timeLimit)
			{
				// Penalty?
				if(time < 0)
				{
					float penalty = Mathf.Abs(time) * 1000f;
					money -= (ulong)penalty;

					if(money < goal)
						reachedGoal = false;

					// Reached goal?
					if(reachedGoal)
					{
						alertWindow.Title = "Congratulations!";
						alertWindow.Text = "The investors are very happy!" +
							"\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
							"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + (startTime - time).ToString("F1") + " seconds" +
							"\nTime penalty:\t\t\t\t\t\t\t-" + penalty.ToString("N0") + "$";
						alertWindow.ButtonText = "Next";
					}
					else
					{
						alertWindow.Title = "Mission failed!";
						alertWindow.Text = "Drive more, walk less!" +
							"\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
							"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + (startTime - time).ToString("F1") + " seconds" +
							"\nTime penalty:\t\t\t\t\t\t\t-" + penalty.ToString("N0") + "$";
						alertWindow.ButtonText = "Retry";
					}
				}
				else
				{
					// Reached goal?
					if(reachedGoal)
					{
						alertWindow.Title = "Congratulations!";
						alertWindow.Text = "The investors are very happy!" +
							"\n\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
							"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + (startTime - time).ToString("F1") + " seconds";
						alertWindow.ButtonText = "Next";
					}
					else
					{
						alertWindow.Title = "Mission failed!";
						alertWindow.Text = "Drive more, walk less!" +
							"\n\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
							"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + (startTime - time).ToString("F1") + " seconds";
						alertWindow.ButtonText = "Retry";
					}
				}
			}
			else
			{
				// Reached goal?
				if(reachedGoal)
				{
					alertWindow.Title = "Congratulations!";
					alertWindow.Text = "The investors are very happy!" +
						"\n\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
						"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + time.ToString("F1") + " seconds";
					alertWindow.ButtonText = "Next";
				}
				else
				{
					alertWindow.Title = "Mission failed!";
					alertWindow.Text = "Drive more, walk less!" +
						"\n\nEarned money:\t\t\t\t\t\t\t" + money.ToString("N0") + "$" +
						"\nTime:\t\t\t\t\t\t\t\t\t\t\t" + time.ToString("F1") + " seconds";
					alertWindow.ButtonText = "Retry";
				}
			}

			// Display end screen
			Invoke("EndScreen", 2f);
		}

		// Add bonus
		public void BonusMoney(ulong bonus)
		{
			money += bonus;
		}

		// Reset game logic
		public void ResetLogic()
		{
			// Reset values
			time = 0f;
			money = 0;
			moneyTenThousand = 0;
			timerFeedback = false;
			reachedGoal = false;

			// Stop running coroutines
			StopCoroutine(moneyCount);
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
