using UnityEngine;
using System.Collections;

public class Spooker : MonoBehaviour {
	public bool permaSpooky = false;
	private bool active;
	private float spookspiry = 0;
	private Renderer rend;

	public void Start() {
		active = permaSpooky;
	}

	public void Update() {
		if (permaSpooky || !active) {
			return;
		}

		checkForExpiry();
	}

	public void Activate(float lifeSecs = 0) {
		this.active = true;

		spookspiry = Time.time + lifeSecs;
	}

	public bool IsActive() {
		return active;
	}
	private void checkForExpiry() {
		if (spookspiry <= Time.time) {
			this.active = false;
		}
	}
}
