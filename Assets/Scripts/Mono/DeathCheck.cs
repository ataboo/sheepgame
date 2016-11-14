using UnityEngine;
using System.Collections;

public class DeathCheck : MonoBehaviour {

	public bool respawnable = false;
	public float deathHeight = -20f;
	private EntitySpawner spawner;
	private bool isSheep;
	public void Start() {
		spawner = GameObject.FindGameObjectWithTag("EntitySpawner").GetComponent<EntitySpawner>();
		isSheep = GetComponent<SheepController>() != null;
	}
	public void FixedUpdate () {
		if (transform.position.y < deathHeight) {
			isKill();
		}
	}

	private void isKill() {
		spawner.IsKill(gameObject, this, isSheep);
	}
}
