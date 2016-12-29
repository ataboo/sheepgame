using UnityEngine;

public interface CountListener {
	void OnCountChange(int newCount);
}

public class SheepCounter : MonoBehaviour {
	private CountListener countListener;
	private int sheepCount = 0;

	public void OnTriggerEnter(Collider collider) {
		Debug.Log ("Trigger Enter.");

		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount++;
			SendCount();
		}
	}

	public void OnTriggerExit(Collider collider) {
		Debug.Log ("Trigger Exit.");

		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		if (sc != null) {
			sheepCount--;
			SendCount();
		}
	}

	public void SetListener(CountListener countListener) {
		this.countListener = countListener;

		SendCount ();
	}
		
	private void SendCount() {
		Debug.Log ("Sheep Count Change: " + sheepCount);

		countListener.OnCountChange (sheepCount);
	}
}
	