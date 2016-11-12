using UnityEngine;
using System.Collections.Generic;

public class KeyboardController : MonoBehaviour {

	public float speed = 5.0f;
	public string vertAxis;
	public string horizAxis;

	private Rigidbody rb;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	private void Move() {
		Vector3 move = (Vector3.forward * Input.GetAxis(vertAxis) + Vector3.right * Input.GetAxis(horizAxis)) * speed;

		transform.position += move * Time.deltaTime;
	}

	 //make sure u replace "floor" with your gameobject name.on which player is standing
 
}
