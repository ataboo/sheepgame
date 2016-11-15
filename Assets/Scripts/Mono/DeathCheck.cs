using UnityEngine;
using System.Collections;

public class DeathCheck : MonoBehaviour {
	public bool respawnable = false;
	public float deathHeight = -20f;
	private SceneManager manager;
	private bool isSheep;
	public void Start() {
		manager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneManager>();
		isSheep = GetComponent<SheepController>() != null;
	}
	public void FixedUpdate () {
		if (transform.position.y < deathHeight) {
			isKill();
		}
	}

	private void isKill() {
		manager.IsKill(gameObject, this);
	}

	public bool IsSheep() {
		return isSheep;
	}
}
