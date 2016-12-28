using UnityEngine;
using UnityEngine.Networking;

public enum DogState {
	Normal,
	Spooking,
	Running,
	Dead
}

public class PlayerControl : NetworkToggleable {
	public enum DogControl {
		DogOne,
		DogTwo,
		None
	}

	public string vertAxis;
	public string horizAxis;
	public string activateAxis;
	public string runAxis;
	public Vector3 velocity;
	public DogState dogState = DogState.Normal;

	float walkSpeed = 4.0f;
	float runSpeed = 10f;
	private float speed;
	private bool running = false;

	[SyncVar(hook="OnAwakeChange")]
	public bool isAwake = false;

	private Rigidbody rb;
	private Spooker spooker;
	private Renderer rendComp;

	public override void BothAwake () 
	{
		speed = walkSpeed;
		this.rb = GetComponent<Rigidbody> ();
		this.spooker = GetComponent<Spooker> ();
		this.rendComp = GetComponentInChildren<Renderer> ();

		rb.centerOfMass = new Vector3 (10, 10, 10);
	}

	public override void BothStart () {
	}

	public override void BothUpdate () {
		if (!isLocalPlayer || vertAxis == "") {
			return;
		}

		Move();

		ProcessInput();

		this.dogState = UpdateState ();
	}

	void Move() {
		if (spooker.active) {
			return;
		}

		Vector3 oldPos = transform.position;

		this.velocity = new Vector3 (Input.GetAxis (horizAxis), 0, Input.GetAxis (vertAxis)).normalized * speed;

		Vector3 movement = velocity * Time.deltaTime;

		transform.position += movement;

		if (movement.sqrMagnitude > 0) {
			transform.rotation = Quaternion.RotateTowards (rb.rotation, Quaternion.LookRotation (movement.normalized, Vector3.up), 10f);
		}
	}

	private DogState UpdateState() {
		if (spooker.active) {
			return DogState.Spooking;
		}

		return DogState.Normal;
	}

	void ProcessInput() {
		if (Input.GetButtonDown(runAxis)) {
			running = !running;
			speed = running ? runSpeed : walkSpeed;
		}

		if (Input.GetButton (activateAxis)) {
			ActivateSpook ();
		}
	}

	private void ActivateSpook() {
		if (spooker.active) {
			return;
		}

		if (_runningOnServer) {
			spooker.Activate (1.25f);
		} else {
			spooker.CmdActivate (1.25f);
		}
	}
		
	public void SetControls(DogControl dogControl) {
		switch(dogControl) {
			case DogControl.DogOne:
				vertAxis = "Vertical";
				horizAxis = "Horizontal";
				activateAxis = "OneActivate";
				runAxis = "OneRun";
			break;
			case DogControl.DogTwo:
				vertAxis = "TwoVertical";
				horizAxis = "TwoHorizontal";
				activateAxis = "TwoActivate";
				runAxis = "TwoRun";
			break;
			default:
			Debug.LogError("DogController shouldn't be handling Non Player DogControl.");
			break;
		}
	}

	public void Teleport(Vector3 position) {
		Debug.Log ("Called Teleport");

		this.transform.position = position;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;

		//this.isAwake = true;
		if (_runningOnServer) {
			this.isAwake = true;
			levelManager.RebuildEntityList ();
		} else {
			CmdSetActive (true);
			SyncAwake ();
		}
	}

	[Command]
	private void CmdSetActive(bool active) {
		this.isAwake = active;

		levelManager.RebuildEntityList ();
	}

	private void SyncAwake() {
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			PlayerControl pc = player.GetComponent<PlayerControl> ();
			pc.OnAwakeChange (pc.isAwake);
		}
	}

	public void OnAwakeChange(bool awake) {

		this.isAwake = awake;
		//GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<Rigidbody> ().isKinematic = !awake;
	}
}
