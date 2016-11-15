using UnityEngine;

public class SheepCounter : MonoBehaviour {
	private SceneManager sceneManager;
	private int sheepCount = 0;

	void Start() {
		sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();

		UpdateGoal();
	}

	public void OnTriggerEnter(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount++;
			UpdateGoal();
		}
	}

	public void OnTriggerExit(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount--;
			UpdateGoal();
		}
	}

	private void UpdateGoal() {
		Debug.Log("Sheep Count now: " + sheepCount);

		sceneManager.UpdateGoal(sheepCount);
	}
}
