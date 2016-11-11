using UnityEngine;
using System.Collections.Generic;

public class KeyboardController : MonoBehaviour {

	public float speed = 5.0f;
	public GameObject camObject;

	private Rigidbody rb;
	private Camera camera;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		camera = camObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	private void Move() {
		Vector3 forward = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z);
		Vector3 right = new Vector3(camera.transform.right.x, 0f, camera.transform.right.z);
		Vector3 move = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")).normalized * speed;

		transform.position += move * Time.deltaTime;
	}

	 //make sure u replace "floor" with your gameobject name.on which player is standing
 
}
