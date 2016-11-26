using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Network toggleable.
/// Scripts inheriting this will deactivate themselves according to settings.
/// </summary>
[RequireComponent (typeof(NetworkIdentity))]
public abstract class NetworkToggleable : NetworkBehaviour {
	public bool serverOnly = false;
	private bool initialized = false;

	private bool _runningOnServer = false;
	public bool runningOnServer;

	[Server]
	public virtual void ServerAwake () {}

	[Server]
	public virtual void ServerStart (){}

	[Server]
	public virtual void ServerUpdate (){}

	public virtual void BothAwake () {}

	public virtual void BothStart () {}

	public virtual void BothUpdate() {}

	public override void OnStartServer() {
		_runningOnServer = true;
	}

	public override void PreStartClient() {
		NetworkIdentity identity = GetComponent<NetworkIdentity> ();

		if (!identity.isServer && serverOnly) {
			enabled = false;
			return;
		}

		initialized = true;

		if (_runningOnServer) {
			this.ServerAwake ();
		}

		this.BothAwake ();
	}

	public override void OnStartClient() {
		if (_runningOnServer) {
			this.ServerStart ();
		}

		this.BothStart ();
	}

	public void Update() {
		runningOnServer = _runningOnServer;

		if (initialized && _runningOnServer) {
			if (_runningOnServer) {
				ServerUpdate ();
			}

			BothUpdate ();
		}
	}

}