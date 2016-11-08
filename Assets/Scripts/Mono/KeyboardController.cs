using UnityEngine;
using System.Collections.Generic;

public class KeyboardController : MonoBehaviour {

	public float speed = 5.0f;

	public float jumpThrust = 500f;

	public float selfRightingForce = 5f;

	public float turningRate = 50f;

	private SheepMover sheepMover;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		this.rb = GetComponent<Rigidbody>();
		this.sheepMover = GetComponent<SheepMover>();
	}
	
	// Update is called once per frame
	void Update () {
		SheepState sheepState = sheepMover.GetSheepState();

		if (sheepState.CanMove()) {
			Move();
		}

		if (sheepState.CanSelfRight()) {
			//SelfRight();
		}
	}

	private void Move() {

		Vector3 move = transform.forward * Input.GetAxis("Vertical") * speed;
		transform.localPosition += move * Time.deltaTime;

		rb.transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * turningRate * Time.deltaTime, 0));
		//rb.AddTorque(new Vector3(0, Input.GetAxis("Horizontal") * turningTorque, selfRighting), ForceMode.Impulse);

		if(Input.GetKeyDown(KeyCode.Space)) {
			rb.AddForce(0,jumpThrust, 0, ForceMode.Impulse);
		}
	}

	 //make sure u replace "floor" with your gameobject name.on which player is standing
 
}
