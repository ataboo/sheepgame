using UnityEngine;
using System.Collections;

public class Spooker : MonoBehaviour {
	public bool permaSpooky = false;

	public Material activeMat;
	public Material innactiveMat;
	private bool active;
	private float spookspiry = 0;
	private Renderer rend;

	public void Start() {
		active = permaSpooky;

		rend = GetComponent<Renderer>();
		rend.material = active ?  activeMat : innactiveMat;
	}

	public void Update() {
		if (permaSpooky || !active) {
			return;
		}

		rend.material = active ?  activeMat : innactiveMat;
		checkForExpiry();
	}

	public void Activate(float lifeSecs) {
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
