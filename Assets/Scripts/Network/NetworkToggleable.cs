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

	public override void OnStartClient() {
		NetworkIdentity identity = GetComponent<NetworkIdentity> ();

		if (!identity.isServer && serverOnly) {
			enabled = false;
		}
	}
}