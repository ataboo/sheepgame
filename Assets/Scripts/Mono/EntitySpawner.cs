using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EntitySpawner : NetworkBehaviour {

	private int sheepCount = 12;
	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	public GameObject dogPrefab;
	public GameObject netDogPrefab;
	
	private List<Transform> sheepSpawns = new List<Transform>();
	private Transform dogSpawnOne;
	private UIInterface uiInterface;
	private CameraController camController;

	public void Awake() {
		GetSpawnPoints();
		camController = GameObject.Find("Camera").GetComponent<CameraController>();

		//InitialSpawn();
	}

	public override void OnStartServer() {
		Debug.Log ("Started Server");

		InitialSpawn ();
	}

	// public GameObject SpawnDog(PlayerControl.DogControl dogControl, string name, bool camFollow = false) {
	// 	GameObject dog = (GameObject) GameObject.Instantiate(dogPrefab, MakeSpawnPoint(dogSpawnOne), Quaternion.Euler(0, 0, 0));
	// 	dog.name = name;
	// 	//dog.GetComponent<DogController>().SetControl(dogControl);

	// 	if (camFollow) {
	// 		camController.RpcFollowDog(dog, false);
	// 	}

	// 	return dog;
	// }

	public GameObject SpawnNetDog(string name) {
		GameObject dog = (GameObject) GameObject.Instantiate(netDogPrefab, MakeSpawnPoint(dogSpawnOne), Quaternion.Euler(0, 0, 0));
		dog.name = name;

		return dog;
	}
	public void RespawnDog(GameObject dog) {
		dog.transform.position = MakeSpawnPoint(dogSpawnOne);

		dog.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

//[Command]
//	public void CmdRequestSpawn() {
//		GameObject dogOne = SpawnNetDog("DogOne_" + netId);
//	
//		if (playerControllerId > 0) {
//			NetworkServer.AddPlayerForConnection(connectionToClient, dogOne, GetComponent<NetworkIdentity>().playerControllerId);
//
//		} else {
//			ClientScene.AddPlayer(0);
//
//		}
//	        dogOne.GetComponent<PlayerControl>().RpcSetControl(PlayerControl.DogControl.DogOne);
//
//        
//        Debug.Log("Ran spawn");
//
//	}
	
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
