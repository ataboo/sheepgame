using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIInterface : MonoBehaviour {
	public class GameSummary {
		const float WinRatio = 0.75f;
		public int sheepInGoal;
		public int deadSheep;
		public int totalSheep;
		public float timeSecs;
		
		public GameSummary(int totalSheep, int sheepInGoal, int deadSheep, float runTime) {
			this.sheepInGoal = sheepInGoal;
			this.deadSheep = deadSheep;
			this.totalSheep = totalSheep;
			this.timeSecs = runTime;
		}

		public string Title() {
			return SuccessRatio() > WinRatio ? "Success" : "Failed";
		}

		public string Subtitle() {
			return string.Format("Saved {0}% (Goal {1}%)", Mathf.Floor(SuccessRatio() * 100f), WinRatio * 100f);
		}

		public string Details() {
			return string.Format("Grade: {0}\nTime: {1}", Grade(SuccessRatio()), AtaUtility.SecsToMinSec(timeSecs));
		}

		private float SuccessRatio() {
			return (float) sheepInGoal / (float) totalSheep;
		}
		private string Grade(float savedRatio) {

			if (savedRatio == 0) {
				return "Complete and Utter Baasacre.";
			}

			if (savedRatio < 0.50) {
				return "Worthy of Lambasting.";
			}

			if (savedRatio < 0.75) {
				return "Mutton to Write home about.";
			}

			if (savedRatio < 1.0) {
				return "Insert Sheep Pun";
			}

			return "Everyone is Sheep Shape";
		}
	}

	public Text hudText;
	public GameObject endPanel;
	public Text endTitle;
	public Text endSubtitle;
	public Text endDetail;
	public GameObject pausePanel;

	public void UpdateHud(int totalSheep, int sheepInGoal, int deadSheep) {
		int wildCount = totalSheep - sheepInGoal - deadSheep;

		hudText.text = string.Format("Sheep Count\nInside Goal: {0}\nIn The Wild: {1}\nDeparted: {2}", sheepInGoal, wildCount, deadSheep);
	}

	public void ShowEndScreen(int totalSheep, int sheepInGoal, int deadSheep, float runTime) {
		Debug.Log(string.Format("Showed end screen!, Total: {0} Goal: {1}, Dead: {2}", totalSheep, sheepInGoal, deadSheep));

		GameSummary summary = new GameSummary(totalSheep, sheepInGoal, deadSheep, runTime);

		endTitle.text = summary.Title();
		endSubtitle.text = summary.Subtitle();
		endDetail.text = summary.Details();
		
		pausePanel.SetActive(false);
		endPanel.SetActive(true);
	}

	public void ShowPauseScreen(bool show) {
		pausePanel.SetActive(show);
	}

	public void HideEndScreen() {
		endPanel.SetActive(false);
	}
}
