using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : MonoBehaviour, INetworkCharacter, IDogDisplay {
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

	public Vector3 correctPosition;
	public Quaternion correctRotation;

	float walkSpeed = 4.0f;
	float runSpeed = 10f;
	private float speed;
	private bool running = false;

	public bool isLocalControl = false;

	public bool isActive = false;
	private Spooker spooker;
	private CharacterController controller;

	public void Awake() {
		spooker = GetComponent<Spooker> ();
		speed = walkSpeed;

		this.isLocalControl = GetComponent<PhotonView> ().isMine;
		this.spooker.isLocalControl = isLocalControl;
	}

	public void Start() {
		if (isLocalControl) {
			DogControl dogControl = GameObject.FindGameObjectWithTag ("Camera").GetComponent<CameraController> ().RegisterDog (this);
			this.SetControls (dogControl);
		}
	}

	public void Update () {
		if (!isActive) {
			return;
		}


		if (isLocalControl) {
			Move ();
			ProcessInput();
		} else {
			LerpRemoteTransform();
		}
	}

	public void InitCamera() {
		DogControl dogControl = GameObject.FindGameObjectWithTag ("Camera").GetComponent<CameraController>().RegisterDog (this);

		SetControls (dogControl);
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

	[PunRPC]
	public void Teleport(Vector3 position) {
		this.isActive = true;
		
		this.transform.position = position;
	}

	#region INetworkCharacter
	public object[] GetSyncVars() {
		if (spooker == null) {
			Debug.Log ("Confirmed... spooker is null.");
			spooker = GetComponent<Spooker> ();
		}


		object[] syncVars = new object[] {
			transform.position, 
			transform.rotation, 
			velocity,
			spooker.active
		};
		
		return syncVars;
	}
	
	public void PutSyncVars(object[] serialized) {
		correctPosition = (Vector3) serialized [0];
		correctRotation = (Quaternion) serialized [1];
		velocity = (Vector3) serialized [2];
		spooker.active = (bool)serialized [3];
	}
	
	public int GetSyncCount() {
		return 4;
	}
	#endregion
	
	#region IDogDisplay
	public float GetVelocity() {
		return velocity.magnitude;
	}
	
	public bool IsBarking() {
		if (spooker == null) {
			return false;
		}

		return spooker.active;
	}
	#endregion

	private void Move() {
		if(spooker.active) {
			return;
		}
		
		Vector3 oldPos = transform.position;
		
		Vector3 movement = new Vector3 (Input.GetAxis (horizAxis), 0, Input.GetAxis (vertAxis)).normalized * speed;
		
		//if (!IsGrounded()) {
			//movement += new Vector3 (0, -gravity, 0);
		//}
		
		movement *= Time.deltaTime;
		
		transform.position += movement;

		this.velocity = (transform.position - oldPos) / Time.deltaTime;

		if (velocity.sqrMagnitude < 0.5) {
			velocity = Vector3.zero;
		} else {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (velocity.normalized, Vector3.up), 10f);
		}
	}

	private void ProcessInput() {
		if (Input.GetButtonDown(runAxis)) {
			running = !running;
			speed = running ? runSpeed : walkSpeed;
		}

		if (Input.GetButton (activateAxis) && !spooker.active) {
			ActivateSpook ();
		}
	}

	private void ActivateSpook() {
		spooker.Activate (1.25f);
	}

	private void LerpRemoteTransform()
    {
		transform.position = Vector3.Lerp(transform.position, correctPosition, Time.deltaTime * 5);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctRotation, Time.deltaTime * 5);
    }

	private bool IsGrounded() {
		//return (lastCollisionFlag & CollisionFlags.CollidedBelow) != 0;
		return true;
	}
		

}
