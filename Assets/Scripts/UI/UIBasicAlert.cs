using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OilSpill
{
	public class UIBasicAlert : MonoBehaviour
	{
		[SerializeField] protected string title = "TITLE";
		[SerializeField][TextAreaAttribute] protected string text = "Hello world!";
		[SerializeField] protected string buttonText = "OK";

		[SerializeField] protected Text titleObject;
		[SerializeField] protected Text textObject;
		[SerializeField] protected Button buttonObject;

		// Initialization
		protected void Start()
		{
			// Add texts
			titleObject.text = title;
			textObject.text = text;
			buttonObject.transform.GetComponentInChildren<Text>().text = buttonText;
		}

		// Close alert window
		public void ClickButton()
		{
			gameObject.SetActive(false);
		}

		// Properties
		public string Title
		{
			get { return this.title; }
			set
			{
				this.title = value;
				titleObject.text = this.title;
			}
		}

		public string Text
		{
			get { return this.text; }
			set
			{
				this.text = value;
				textObject.text = this.text;
			}
		}

		public string ButtonText
		{
			get { return this.buttonText; }
			set
			{
				this.buttonText = value;
				buttonObject.transform.GetComponentInChildren<Text>().text = this.buttonText;
			}
		}
	}
}
