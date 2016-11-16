using UnityEngine;

public class SheepCounter : MonoBehaviour {
	private LevelManager levelManager;
	private int sheepCount;

	void Awake() {
		levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
		sheepCount = 0;
	}
	void Start() {
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
		levelManager.UpdateGoal(sheepCount);
	}
}
