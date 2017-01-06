using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IGrassCounter {
	bool IsInside (GameObject gameObject);
	void JustAte (SheepController sheepController);
}

public class GrassCounter : MonoBehaviour, IGrassCounter {
	List<int> idsInTrigger = new List<int>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider collider) {
		idsInTrigger.Add (collider.gameObject.GetInstanceID);
	}

	public void OnTriggerExit(Collider collider) {
		idsInTrigger.Remove (collider.gameObject.GetInstanceID);
	}

	public bool IsInside (GameObject gameObject)
	{
		return idsInTrigger.Contains (gameObject.GetInstanceID);
	}

	public void JustAte (SheepController sheepController)
	{
		
	}
}
