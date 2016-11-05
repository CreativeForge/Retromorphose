using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogicPrototype : MonoBehaviour
{
	//public static GameLogicPrototype main = this;

	[SerializeField] private GameObject infoTextObj;

	private Text infoText;

	// Use this for initialization
	void Start()
	{
		infoText = infoTextObj.GetComponent<Text>();
		infoText.text = "Car 1 - Fuel: x\nCar 2 - Fuel: x";
		print(infoText);
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
}
