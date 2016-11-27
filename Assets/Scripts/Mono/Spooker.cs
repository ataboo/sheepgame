using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Spooker : NetworkToggleable {
	public bool permaSpooky = false;
	public float forceRadius = 4f;
	public float forcePower = 10000f;

	[SyncVar(hook="OnSpookChange")]
	public bool active = false;

	private float spookspiry = 0;
	private Renderer rend;

	override public void ServerStart() {
		active = permaSpooky;
	}

	override public void ServerUpdate() {
		if (permaSpooky || !active) {
			return;
		}

		checkForExpiry();
	}

	[Server]
	public void Activate(float lifeSecs = 0) {
		if (!active) {
			Explode();
		}


		this.active = true;
		spookspiry = Time.time + lifeSecs;
	}

	[Command]
	public void CmdActivate(float lifeSecs) {
		Activate (lifeSecs);
	}

	[Server]
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

	[Server]
	private void checkForExpiry() {
		if (spookspiry <= Time.time) {
			this.active = false;
		}
	}

	public void OnSpookChange(bool activeChange) {
		this.active = activeChange;
	}
}
