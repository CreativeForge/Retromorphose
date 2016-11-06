using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogicPrototype : MonoBehaviour
{
	//public static GameLogicPrototype main = this;

	[SerializeField] private GameObject infoTextObj;

	private GameObject[] _cars;

	private Text infoText;

	// Use this for initialization
	void Start()
	{
		_cars = GameObject.FindGameObjectsWithTag("Vehicle");
		infoText = infoTextObj.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update()
	{
		infoText.text = "Car 1 - Fuel: " + _cars[0].GetComponent<CarPrototype>().Fuel.ToString("F0") +
			"\nCar 2 - Fuel: " + _cars[1].GetComponent<CarPrototype>().Fuel.ToString("F0");
	}
}
