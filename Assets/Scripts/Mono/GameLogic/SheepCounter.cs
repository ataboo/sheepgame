using UnityEngine;

public interface CountListener {
	void OnCountChange(int newCount);
}

public class SheepCounter : MonoBehaviour {
	private CountListener countListener;
	private int sheepCount = 0;

	public void Awake() {
		countListener = GameObject.FindGameObjectWithTag ("GameLogic").GetComponent<CountListener> ();
	}

	public void OnTriggerEnter(Collider collider) {
		Debug.Log ("Trigger Enter.");

		SheepController sc = collider.gameObject.GetComponent<SheepController>();

		//if (true){
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
		
	private void SendCount() {
		countListener.OnCountChange (sheepCount);
	}
}
	