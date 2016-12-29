using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Spooker : MonoBehaviour {
	public bool permaSpooky = false;
	public float forceRadius = 4f;
	public float forcePower = 10000f;

	public bool active = false;
	public bool isLocalControl = false;

	private float spookspiry = 0;
	private Renderer rend;

	public void Start() {
		active = permaSpooky;
		Debug.Log ("Ran  Spooker Start");
	}

	public void Awake() {
		Debug.Log ("Ran Spooker Awake");
	}

	public void Update() {
		if (!isLocalControl || permaSpooky || !active) {
			return;
		}

		checkForExpiry();
	}
		
	public void Activate(float lifeSecs = 0) {
//		if (!active) {
//			Explode();
//		}
			
		this.active = true;
		spookspiry = Time.time + lifeSecs;
	}
		
	private void Explode() {
 		Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, forceRadius);
        foreach (Collider hit in colliders) {
			if (hit.gameObject == gameObject) {
				continue;
			}

			SheepController sc = hit.GetComponent<SheepController>();
            
			if (sc != null) {
				sc.Launch(forcePower, explosionPos, forceRadius, 2.0f);
			}
        }
	}
		
	private void checkForExpiry() {
		if (spookspiry <= Time.time) {
			this.active = false;
		}
	}

	public void OnSpookChange(bool activeChange) {
		this.active = activeChange;
	}
}
