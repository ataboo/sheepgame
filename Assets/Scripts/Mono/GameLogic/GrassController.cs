using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IGrassController {
	bool IsInside (GameObject gameObject);
	Vector3 GetWanderPoint ();
}

public interface GrassControllerListener {
	
}

public class GrassController : MonoBehaviour, IGrassController {
	List<int> idsInTrigger = new List<int>();
	Collider grassCollider;

	void Awake() {
		grassCollider = GetComponent<Collider> ();
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider collider) {
		idsInTrigger.Add (collider.gameObject.GetInstanceID());
	}

	public void OnTriggerExit(Collider collider) {
		idsInTrigger.Remove (collider.gameObject.GetInstanceID());
	}

	public bool IsInside (GameObject gameObject)
	{
		return idsInTrigger.Contains (gameObject.GetInstanceID());
	}

	public Vector3 GetWanderPoint()
	{
		return AtaUtility.RandPointInCollider (grassCollider);
	}
}
