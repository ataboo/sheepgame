using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SheepCounter : MonoBehaviour {
	public UIInterface uiInterface;
	private int sheepCount = 0;

	void Start() {
		UpdateDisplay();
	}

	public void OnTriggerEnter(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount++;
			UpdateDisplay();
		}
	}

	public void OnTriggerExit(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount--;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay() {
		uiInterface.UpdateGoalCount(sheepCount);
	}
}
