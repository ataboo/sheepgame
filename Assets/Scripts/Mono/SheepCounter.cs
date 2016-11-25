using UnityEngine;

public interface CountListener {
	void OnCountChange(int newCount);
}
public class SheepCounter : NetworkToggleable {
	private CountListener countListener;
	private int sheepCount = 0;

	void Start() {
		SendCount();
	}

	public void OnTriggerEnter(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount++;
			SendCount();
		}
	}

	public void OnTriggerExit(Collider collider) {
		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount--;
			SendCount();
		}
	}

	private void SendCount() {
		if (countListener == null) {
			Debug.LogError("SheepCounter has no listener.");
		} else {
			countListener.OnCountChange(sheepCount);
		}
	}
}
