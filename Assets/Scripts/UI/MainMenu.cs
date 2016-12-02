using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace OilSpill
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField] protected GameObject creditsWindow;

		public void StartGame()
		{
			SceneManager.LoadScene("Track1");
		}

		public void Credits()
		{
			creditsWindow.SetActive(true);
		}

		public void Exit()
		{
			Application.Quit();
		}
	}
}
