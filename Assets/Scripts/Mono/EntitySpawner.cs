﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EntitySpawner : NetworkToggleable {

	public int sheepCount = 12;
	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	public GameObject dogPrefab;
	public GameObject netDogPrefab;
	
	private List<Transform> sheepSpawns = new List<Transform>();
	private Transform dogSpawnOne;
	private UIInterface uiInterface;
	private CameraController camController;

	public override void BothAwake() {
		GetSpawnPoints();
		GameObject camObj = GameObject.FindGameObjectWithTag ("Camera");
		camController = camObj.GetComponent<CameraController>();
	}

	public override void ServerStart() {
		InitialSpawn ();
	}
		
	public GameObject SpawnNetDog(string name) {
		GameObject dog = (GameObject) GameObject.Instantiate(netDogPrefab, MakeSpawnPoint(dogSpawnOne), Quaternion.Euler(0, 0, 0));
		dog.name = name;

		return dog;
	}
		
	public void RespawnDog(PlayerControl playerControl) {
		if (dogSpawnOne == null) {
			GetSpawnPoints ();

			if (dogSpawnOne == null) {
				Debug.LogError ("Couldn't find dog spawn");

				return;
			}
		}
  
		Vector3 dogSpawn = MakeSpawnPoint (dogSpawnOne);

		playerControl.Teleport (dogSpawn);
	}
	
	private void SpawnSheep(Transform basePosition) {
		Vector3 spawnPos = MakeSpawnPoint(basePosition);

		GameObject sheep = (GameObject) Instantiate(sheepPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 359), 0));
		NetworkServer.Spawn(sheep);
	}

	public int GetSheepCount() {
		return sheepCount;
	}

	public void InitialSpawn() {		
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
