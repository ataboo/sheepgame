using UnityEngine;
using System.Collections.Generic;


public enum SheepState {
	Resting, Moving, Rolling, Airborne, Dead
}

public static class SheepStateExtensions {
	public static bool CanMove(this SheepState sheepState) {
		return sheepState == SheepState.Moving || sheepState == SheepState.Resting;
	}

	public static bool CanSelfRight(this SheepState sheepState) {
		return sheepState != SheepState.Dead;
	}
}

public class SheepMover : MonoBehaviour {

	public float turnRate = 10f;
	public float moveRate = 5f;

	public float selfRighting = 10f;

	public float maxVelocityMag = 10f;

	private GameObject sheepTarget;
	private List<string> standingOn = new List<string>();

	private Vector3 lastStandableUp = Vector3.up;
	private Vector3 lastMove;

	private Rigidbody rb;
	



	// Use this for initialization
	void Start () {
		this.sheepTarget = GameObject.FindGameObjectWithTag("sheep-target");
		this.rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
		SheepState sheepState = GetSheepState();

		if (sheepState.CanMove()) {
			Move();
			LimitVelocity();
		}

		if (sheepState.CanSelfRight()) {
			SelfRight();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
	
	public SheepState GetSheepState() {
		if (!this.IsGrounded()) {
			return SheepState.Airborne;
		}

		//TODO: Rolling.

		return lastMove.sqrMagnitude > 0 ? SheepState.Moving : SheepState.Resting;
	}

	private void Move() {
		float bearing = AtaUtility.GetFlatBearing(gameObject, sheepTarget);

		float steering = Mathf.Clamp(bearing / 4f, -turnRate * Time.deltaTime, turnRate * Time.deltaTime);

		transform.Rotate(new Vector3(0, steering, 0));


		Vector3 moveDirection = Vector3.ProjectOnPlane(sheepTarget.transform.position - rb.transform.position, lastStandableUp);

		float moveForce = Mathf.Min(moveRate, moveDirection.sqrMagnitude * 2);
		if(bearing > 20) {
			moveForce /= 2;
		}

		Debug.DrawLine(rb.transform.position, transform.position + moveDirection, Color.red);

		rb.AddForceAtPosition(moveDirection.normalized * moveForce, rb.transform.TransformPoint( -1.2f * rb.transform.up),  ForceMode.Force);
	}

	private void SelfRight() {
		if (lastStandableUp == null) {
			return;
		}

		Vector3 upDirection = (Vector3) lastStandableUp;

		Vector3 upForce = upDirection * Time.deltaTime * selfRighting;

		rb.AddForceAtPosition(upForce, transform.TransformPoint(upDirection * 2), ForceMode.Force);
		rb.AddForceAtPosition(-upForce, transform.TransformPoint(upDirection * -2), ForceMode.Force);
	}

	private void LimitVelocity() {
		float velMagSqr = rb.velocity.sqrMagnitude;
		float maxMagSqr = maxVelocityMag * maxVelocityMag;

		if (velMagSqr > maxVelocityMag) {
			rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocityMag); 
		}
	}


	void OnCollisionEnter(Collision collision){
     if(collision.gameObject.tag == "standable")
     {
		 lastStandableUp = collision.gameObject.transform.up;
         standingOn.Add(collision.gameObject.name);
     }
	}
	
	void OnCollisionExit(Collision collision){
		if(collision.gameObject.tag == "standable")
		{
			standingOn.Remove(collision.gameObject.name);
		}

		// if(standingOn.Count == 0) {
		// 	lastStandableDirection = null;
		// }
	}

	public bool IsGrounded() { 
		return standingOn.Count > 0;
	}
}
