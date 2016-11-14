using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour {
	public int sheepCount = 10;
	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	
	private List<Transform> sheepSpawns = new List<Transform>();
	private Transform dogSpawn;
	private UIInterface uiInterface;

	public void Start () {
		GetSpawnPoints();
		InitialSpawn();

		this.uiInterface = GetComponent<UIInterface>();
		uiInterface.UpdateTotalSheep(sheepCount);
	}

	public void SpawnSheep(Transform basePosition) {
		Vector3 spawnPos = makeSpawnPoint(basePosition);

		Instantiate(sheepPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 359), 0));
	}

	public void IsKill(GameObject gameObject, DeathCheck deathCheck, bool isSheep) {
		if (deathCheck.respawnable) {
			gameObject.transform.position = dogSpawn.position;
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		} else {
			GameObject.Destroy(gameObject);
		}

		if (isSheep) {
			uiInterface.AddDeadSheep();
		}
	}

	private Vector3 makeSpawnPoint(Transform basePosition) {
		int tryCount = 0;

		while(true) {
			Vector3 spawnPos = randPoint(basePosition);
			if(!Physics.CheckSphere(spawnPos, spawnCheckRad, LayerMask.GetMask("Default"))) {
				return spawnPos;
			}

			if (tryCount == 10) {
				Debug.Log("Gave up on spawn: " + gameObject.name);
				return basePosition.position;
			}

			tryCount++;
		}
	}

	private Vector3 randPoint(Transform basePosition) {
		Vector2 randPoint = Random.insideUnitCircle * Random.Range(spawnRadRange.x, spawnRadRange.y);

		return new Vector3(randPoint.x + basePosition.position.x, basePosition.position.y, randPoint.y + basePosition.position.z);
	}

	private void GetSpawnPoints() {
		foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Respawn")) {
			if (gameObject.name.Contains("Sheep")) {
				sheepSpawns.Add(gameObject.transform);
				continue;
			}

			if (gameObject.name.Contains("Dog")) {
				dogSpawn = gameObject.transform;
			}
		}
	}

	private void InitialSpawn() {
		for (int i=0; i < sheepCount; i++) {
			int spawnPointIndex = Random.Range(0, sheepSpawns.Count);

			SpawnSheep(sheepSpawns[spawnPointIndex]);
		}
	}
}
