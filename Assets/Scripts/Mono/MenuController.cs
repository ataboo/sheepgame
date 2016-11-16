using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour {
	public void OpenSoloTest() {
		SceneManager.LoadScene("SoloTestLevel");
	}

	public void QuitGame() {
		Application.Quit();
	}
}
