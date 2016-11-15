using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour {
	private int sheepCount = 12;
	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	
	private List<Transform> sheepSpawns = new List<Transform>();
	private Transform dogSpawnOne;
	private Transform dogSpawnTwo;
	private UIInterface uiInterface;

	public int Reset() {
		if (dogSpawnOne == null) {
			GetSpawnPoints();
		}

		foreach(GameObject entity in GameObject.FindGameObjectsWithTag("entity")) {
			if (entity.name.Contains("Sheep")) {
				Destroy(entity);
			}

			if (entity.name.Contains("Dog")) {
				RespawnDog(entity);
			}
		}

		InitialSpawn();

		return sheepCount;
	}

	private void InitialSpawn() {
		for (int i=0; i < sheepCount; i++) {
			int spawnPointIndex = Random.Range(0, sheepSpawns.Count);

			SpawnSheep(sheepSpawns[spawnPointIndex]);
		}
	}

	public void RespawnDog(GameObject dog) {
		if (dog.name.Equals("DogOne")) {
			dog.transform.position = dogSpawnOne.position;
		} else if (dog.name.Equals("DogTwo")) {
			dog.transform.position = dogSpawnTwo.position;
		}

		dog.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}
	
	private void SpawnSheep(Transform basePosition) {
		Vector3 spawnPos = makeSpawnPoint(basePosition);

		Instantiate(sheepPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 359), 0));
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

			if (gameObject.name.Equals("DogSpawnOne")) {
				dogSpawnOne = gameObject.transform;
			}
			if (gameObject.name.Equals("DogSpawnTwo")) {
				dogSpawnTwo = gameObject.transform;
			}
		}
	}
}
