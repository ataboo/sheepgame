using UnityEngine;
using UnityEngine.Networking;

public class PlayerMover : NetworkTransform {
	public string verticalAxis = "Vertical";
	public string horizontalAxis = "Horizontal";
	
	private float speed = 5.0f;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		Move();
	}

	void Move() {
		Vector3 movement = new Vector3(Input.GetAxis(horizontalAxis), 0, Input.GetAxis(verticalAxis)) * speed * Time.deltaTime;
		transform.position += movement;
	
	}
}
