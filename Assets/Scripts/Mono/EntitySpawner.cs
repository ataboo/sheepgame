using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour {

	public enum DogControl {
		DogOne,
		DogTwo,
		None
	}
	private int sheepCount = 12;
	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	public GameObject dogPrefab;
	
	private List<Transform> sheepSpawns = new List<Transform>();
	private Transform dogSpawnOne;
	private UIInterface uiInterface;
	private CameraController camController;

	public void Awake() {
		GetSpawnPoints();
		camController = GameObject.Find("Camera").GetComponent<CameraController>();
	}

	public void Start() {
		InitialSpawn();
	}

	public GameObject SpawnDog(DogControl dogControl, string name, bool camFollow = false) {
		GameObject dog = (GameObject) GameObject.Instantiate(dogPrefab, MakeSpawnPoint(dogSpawnOne), Quaternion.Euler(0, 0, 0));
		dog.name = name;
		dog.GetComponent<DogController>().SetControl(dogControl);

		if (camFollow) {
			camController.FollowDog(dog, false);
		}

		return dog;
	}
	public void RespawnDog(GameObject dog) {
		dog.transform.position = MakeSpawnPoint(dogSpawnOne);

		dog.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}
	
	private void SpawnSheep(Transform basePosition) {
		Vector3 spawnPos = MakeSpawnPoint(basePosition);

		Instantiate(sheepPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 359), 0));
	}

	public int GetSheepCount() {
		return sheepCount;
	}

	private void InitialSpawn() {
		GameObject dogOne = SpawnDog(DogControl.DogOne, "DogOne", true);
		GameObject dogTwo = SpawnDog(DogControl.DogTwo, "DogTwo", true);
		
		for (int i=0; i < sheepCount; i++) {
			int spawnPointIndex = Random.Range(0, sheepSpawns.Count);

			SpawnSheep(sheepSpawns[spawnPointIndex]);
		}
	}

	private Vector3 MakeSpawnPoint(Transform basePosition) {
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
		}
	}
}
