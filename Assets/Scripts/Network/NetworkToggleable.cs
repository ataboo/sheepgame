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
	private bool pastPreStart = false;

	protected bool _runningOnServer = false;

	protected LevelManager levelManager;
	protected IGameState iGameState;

	[Server]
	public virtual void ServerAwake () {}

	[Server]
	public virtual void ServerStart (){}

	[Server]
	public virtual void ServerUpdate (){}

	public virtual void BothAwake () {}

	public virtual void BothStart () {}

	public virtual void BothUpdate () {}

	public override void OnStartServer() {
		_runningOnServer = true;
	}

	public override void PreStartClient() {
		NetworkIdentity identity = GetComponent<NetworkIdentity> ();

		//Debug.Log ("Identity: " + identity);
		if (!identity.isServer && serverOnly) {
			enabled = false;
			return;
		}

		AttemptInit ();

		pastPreStart = true;
	}

	public override void OnStartClient() {
		if (_runningOnServer) {
			this.ServerStart ();
		}

		this.BothStart ();
	}

	public void Update() {
		if (!pastPreStart) {
			return;
		}

		if (initialized) {
			if (iGameState.IsPaused ()) {
				return;
			}

			if (_runningOnServer) {
				ServerUpdate ();
			}

			BothUpdate ();
		} else {
			AttemptInit ();
		}
	}

	private void AttemptInit() {
		GameObject levelManObj = GameObject.FindGameObjectWithTag ("LevelManager");

		if (levelManObj == null) {
			return;
		}

		this.levelManager = levelManObj.GetComponent<LevelManager> ();
		this.iGameState = (IGameState) levelManager;

		if (_runningOnServer) {
			this.ServerAwake ();
		}

		this.BothAwake ();

		initialized = true;
	}

}