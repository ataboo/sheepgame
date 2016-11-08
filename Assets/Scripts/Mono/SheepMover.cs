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

	private GameObject sheepTarget;
	private List<string> standingOn = new List<string>();
	private Vector3 lastMove;

	private Rigidbody rb;
	



	// Use this for initialization
	void Start () {
		this.sheepTarget = GameObject.FindGameObjectWithTag("sheep-target");
		this.rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		SheepState sheepState = GetSheepState();

		Debug.Log("bearing == " + AtaUtility.GetFlatBearing(gameObject, sheepTarget));

		if (sheepState.CanMove()) {
			Move();
		}

		if (sheepState.CanSelfRight()) {
			SelfRight();
		}
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

		//Debug.Log("Steering" + steering);

		//rb.AddForce((sheepTarget.transform.position - transform.position).normalized * moveRate, ForceMode.Impulse);
		
		//Vector3 move = transform.forward * Input.GetAxis("Vertical") * moveRate;
		//transform.localPosition += move * Time.deltaTime;



		// rb.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * turnRate * Time.deltaTime, 0));
		// //rb.AddTorque(new Vector3(0, Input.GetAxis("Horizontal") * turningTorque, selfRighting), ForceMode.Impulse);

		// if(Input.GetKeyDown(KeyCode.Space)) {
		// 	rb.AddForce(0,jumpThrust, 0, ForceMode.Impulse);
		// }

	}

	private void SelfRight() {
		Vector3 selfRightingTorque = (Vector3.up - transform.up) * selfRighting * Time.deltaTime;

		Debug.Log("Tran up: " + transform.up);

		Debug.Log("Global up: " + Vector3.up);

		Debug.Log("Delta: " + (transform.up - Vector3.up));

		//Debug.Log("Torque: " + selfRightingTorque);

		//rb.AddRelativeTorque(selfRightingTorque, ForceMode.Impulse);
	}


	void OnCollisionEnter(Collision collision){
     if(collision.gameObject.tag == "standable")
     {
         standingOn.Add(collision.gameObject.name);
     }

	 Debug.Log("Added " + collision.gameObject.name);
	}
	
	void OnCollisionExit(Collision collision){
		if(collision.gameObject.tag == "standable")
		{
			standingOn.Remove(collision.gameObject.name);
		}

		Debug.Log("Removed " + collision.gameObject.name);
	}

	public bool IsGrounded() { 
		return standingOn.Count > 0;
	}
}
