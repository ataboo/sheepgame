using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour {
	public enum DogControl {
		DogOne,
		DogTwo,
		None
	}

	public string vertAxis;
	public string horizAxis;
	public string activateAxis;
	public string runAxis;
	float walkSpeed = 6.0f;
	float runSpeed = 10f;
	private float speed;
	private bool running = false;

	public override void OnStartLocalPlayer() 
	{
		speed = walkSpeed;
	}

	public override void OnStartClient () {
		WakeUp ();
	}

	void Update () {
		if (!isLocalPlayer || vertAxis == "") {
			return;
		}

		Move();

		ProcessInput();
	}

	void Move() {
		Vector3 movement = new Vector3(Input.GetAxis(horizAxis), 0, Input.GetAxis(vertAxis)).normalized * speed * Time.deltaTime;

		transform.position += movement;
	}

	void ProcessInput() {
		if (Input.GetButtonDown(runAxis)) {
			running = !running;
			speed = running ? runSpeed : walkSpeed;
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

	private void WakeUp() {
		GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<Rigidbody> ().isKinematic = false;
	}
}
