using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : EntityController, INetworkCharacter, IDogDisplay {
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

	float walkSpeed = 2.0f;
	float runSpeed = 6.0f;
	private float speed;
	private bool running = false;

	public DogControl dogControl;
	public bool isLocalControl = false;

	public bool isActive = false;
	private CharacterController controller;
	private CameraController cameraController;
	private PhotonView photonView;

	public override string UID_Class {
		get {
			return "dog";
		}
	}

	public override int RespawnTime {
		get {
			return 0;
		}
	}

	public override EntitySpawnPoint.EntityType SpawnType {
		get {
			return EntitySpawnPoint.EntityType.Dog;
		}
	}

	protected override void EntityAwake ()
	{
		spooker = GetComponent<Spooker> ();
		photonView = GetComponent <PhotonView> ();
		cameraController = GameObject.FindGameObjectWithTag ("Camera").GetComponent<CameraController> ();
		speed = walkSpeed;

		isLocalControl = photonView.isMine;
		this.spooker.localControl = isLocalControl;
	}

	protected override void EntityStart ()
	{
		if (isLocalControl) {
			dogControl = GameObject.FindGameObjectWithTag ("Camera").GetComponent<CameraController> ().RegisterDog (this);
			this.SetControls ();
		}
	}

	protected override void EntityUpdate ()
	{
		if (isLocalControl) {
			Move ();
			ProcessInput();
		} else {
			LerpRemoteTransform();
		}
	}

	protected override void EntityInit ()
	{
		Debug.Log("Entity Init.");
		Rigidbody rb = GetComponent<Rigidbody>();

		rb.isKinematic = false;
		rb.useGravity = true;
		Debug.Log("Level done Loading.");
	}

	public override void OnDestroy() {
		base.OnDestroy ();

		if (isLocalControl) {
			UnregisterFromCamera ();
		}
	}

	private void RegisterWithCamera() {
		dogControl =cameraController.RegisterDog (this);

		SetControls ();
	}

	private void UnregisterFromCamera() {
		if (cameraController != null) {
			cameraController.UnregisterDog (this);
		}
	}

	public void SetControls() {
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
		this.transform.position = position;
	}

	protected override object[] GetEntitySyncVars ()
	{
		return new object[] {
			transform.position, 
			transform.rotation, 
			velocity,
			spooker.Spooking
		};
	}

	protected override void PutEntitySyncVars (object[] syncVars)
	{
		correctPosition = (Vector3) syncVars [0];
		correctRotation = (Quaternion) syncVars [1];
		velocity = (Vector3) syncVars [2];
		spooker.Spooking = (bool)syncVars [3];
	}

	protected override int GetEntitySyncCount ()
	{
		return 4;
	}
	
	#region IDogDisplay
	public float GetVelocity() {
		return velocity.magnitude;
	}
	
	public bool IsBarking() {
		if (spooker == null) {
			return false;
		}

		return spooker.Spooking;
	}
	#endregion

	private void Move() {
		if(spooker.Spooking) {
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

		if (Input.GetButton (activateAxis) && !spooker.Spooking) {
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
