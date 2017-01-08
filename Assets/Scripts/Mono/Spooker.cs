using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Spooker : MonoBehaviour {
	public bool permaSpooky = false;

	private bool spooking = false;
	public bool Spooking {
		get {
			return spooking || permaSpooky;
		}

		set {
			spooking = value;
		}
	}
	public bool localControl = false;

	private float spookspiry = 0f;
	private Renderer rend;

	public void Awake() {
		localControl = GetComponent<PhotonView> ().isMine;
	}
		
	public void Update() {
		if (!localControl || !spooking) {
			return;
		}

		checkForExpiry();
	}
		
	public void Activate(float lifeSecs = 0) {
		if (!localControl) {
			Debug.LogError ("Spooker should only be activated locally");
			return;
		}

		this.spooking = true;
		spookspiry = Time.time + lifeSecs;
	}
		
	private void checkForExpiry() {
		if (spookspiry <= Time.time) {
			spooking = false;
			spookspiry = 0f;
		}
	}
}
