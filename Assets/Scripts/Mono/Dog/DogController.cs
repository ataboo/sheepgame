using UnityEngine;
using System.Collections.Generic;

public class DogController : MonoBehaviour {

	public enum DogState {
		Normal,
		Spooking,
		Running,
		Dead
	}

	public float walkingSpeed = 5.0f;
	public float runningSpeed = 8.0f;
	public string vertAxis;
	public string horizAxis;
	public string activateAxis;
	public string runAxis;
	private Spooker spooker;
	private Renderer rend;
	private DogState dogState = DogState.Normal;
	private bool paused = false;

	void Start () {
		spooker = GetComponent<Spooker>();

		rend = GetComponent<Renderer>();
	}
	
	void Update () {
		if(paused) {
			return;
		}

		this.dogState = UpdateState();

		Move();

		Colorize();
	}

	void OnPause() {
		this.paused = true;
	}

	void OnResume() {
		this.paused = false;
	}

	// public void SetControl(EntitySpawner.DogControl dogControl) {
	// 	switch(dogControl) {
	// 		case EntitySpawner.DogControl.DogOne:
	// 			vertAxis = "Vertical";
	// 			horizAxis = "Horizontal";
	// 			activateAxis = "OneActivate";
	// 			runAxis = "OneRun";
	// 		break;
	// 		case EntitySpawner.DogControl.DogTwo:
	// 			vertAxis = "TwoVertical";
	// 			horizAxis = "TwoHorizontal";
	// 			activateAxis = "TwoActivate";
	// 			runAxis = "TwoRun";
	// 		break;
	// 		default:
	// 		Debug.LogError("DogController shouldn't be handling Non Player DogControl.");
	// 		break;
	// 	}
	// }

	private void Colorize() {
		Color rendColor = Color.white;

		switch(dogState) {
			case DogState.Normal:
				rendColor = Color.grey;
				break;
			case DogState.Running:
				rendColor = Color.blue;
				break;
			case DogState.Spooking:
				rendColor = Color.red;
				break;
		}

		rend.material.color = rendColor;
	}

	private DogState UpdateState() {
		if (spooker.active) {
			return DogState.Spooking;
		}
		
		if (Input.GetAxis(activateAxis) == 1) {
			spooker.Activate(1.0f);
			return DogState.Spooking;
		}

		if (Input.GetAxis(runAxis) == 1 && AttemptRun()) {
			return DogState.Running;
		}

		RecoverStamina();

		return DogState.Normal;
	}

	private void Move() {
		Vector3 move = (Vector3.forward * Input.GetAxis(vertAxis) + Vector3.right * Input.GetAxis(horizAxis)) * GetSpeed();

		transform.position += move * Time.deltaTime;
	}

	private float GetSpeed() {
		switch(dogState) {
			case DogState.Normal:
				return walkingSpeed;
			case DogState.Running:
				return runningSpeed;
			case DogState.Spooking:
				return 0;
				default:
			return 0;
		}
	}

	private bool AttemptRun() {
		return true;
	}

	private void RecoverStamina() {

	}
}
