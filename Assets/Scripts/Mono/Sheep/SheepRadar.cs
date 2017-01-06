using UnityEngine;
using System.Collections;

public interface SheepRadarListener {
	void DetectedGrassCounter (GrassCounter grassCounter);
	void LostGrassCounter (GrassCounter grassCounter);
}

public class SheepRadar : MonoBehaviour {
	private SheepRadarListener listener;


	// Use this for initialization
	void Start () {
		this.listener = GetComponentInParent<SheepController> ();

		if (listener == null) {
			Debug.LogError ("SheepRadar on " + gameObject.name + " couldn't find a listener.");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider collider) {
		if (listener == null) {
			return;
		}

		Debug.Log ("Sheepdar Einfart: " + collider.gameObject.name);

		GrassCounter grassCounter = collider.gameObject.GetComponent<GrassCounter>();

		if (grassCounter != null) {
			listener.DetectedGrassCounter (grassCounter);
		}
	}

	public void OnTriggerExit(Collider collider) {
		if (listener == null) {
			return;
		}

		Debug.Log ("Sheepdar Ausfart: " + collider.gameObject.name);

		GrassCounter grassCounter = collider.gameObject.GetComponent<GrassCounter>();

		if (grassCounter != null) {
			listener.LostGrassCounter (grassCounter);
		}
	}
}
