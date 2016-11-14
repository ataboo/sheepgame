using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIInterface : MonoBehaviour {
	public Text hudText;

	private int sheepGoalCount = 0;
	private int totalSheepCount = 0;
	private int deadCount = 0;

	public void UpdateTotalSheep(int totalSheep) {
		this.totalSheepCount = totalSheep;
		UpdateText();
	}

	public void UpdateGoalCount(int sheepCount) {
		this.sheepGoalCount = sheepCount;
		UpdateText();
	}

	public void AddDeadSheep() {
		deadCount++;
		UpdateText();
	}

	private void UpdateText() {
		int wildCount = totalSheepCount - sheepGoalCount - deadCount;

		hudText.text = string.Format("Sheep Count\nInside Goal: {0}\nIn The Wild: {1}\nDeparted: {2}", sheepGoalCount, wildCount, deadCount);
	}
}
