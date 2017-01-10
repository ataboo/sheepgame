using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class EntitySpawner : MonoBehaviour {

	public Vector2 spawnRadRange = new Vector2(2f, 10f);
	public float spawnCheckRad = 1f;
	public GameObject sheepPrefab;
	public GameObject dogPrefab;
	public GameObject netDogPrefab;
	
	private List<EntitySpawnPoint> sheepSpawns = new List<EntitySpawnPoint>();
	private List<EntitySpawnPoint> dogSpawns = new List<EntitySpawnPoint> ();
	private Transform dogSpawnOne;
	private UIInterface uiInterface;

	public void Awake() {
		GetSpawnPoints();
	}
		
	public void RespawnEntity(EntityController entity) {
		Vector3 basePoint;

		if (entity.SpawnType == EntitySpawnPoint.EntityType.Dog) {
			basePoint = GetBaseForTeam (dogSpawns, entity.TeamId);
		} else {
			basePoint = GetBaseForTeam (sheepSpawns, entity.TeamId);
		}

		Vector3 spawnPosition = MakePointAroundBase (basePoint);

		entity.GetComponent<PhotonView> ().RPC ("Teleport", PhotonTargets.AllBuffered, spawnPosition);
	}

	public void SpawnDog(LevelSettings.DogOption dogOption) {
		int teamId = (int)PhotonNetwork.player.CustomProperties [SheepGamePlayerRow.TEAM_SELECT_KEY];

		for(int i=0; i<dogOption.DogCount; i++) {
			GameObject dog = PhotonNetwork.Instantiate(dogOption.PrefabName, Vector3.zero, Quaternion.identity, 0);
			dog.GetComponent<PhotonView> ().RPC("Initialize", PhotonTargets.AllBuffered, teamId);
		}
	}
	
	private void SpawnSheep(int teamId) {
		Vector3 spawnPos = MakePointAroundBase (GetBaseForTeam (sheepSpawns, teamId));

		GameObject sheep = PhotonNetwork.InstantiateSceneObject("NPCNetSheep", spawnPos, Quaternion.Euler(0, Random.Range(0, 359), 0), 0, null);
		sheep.GetComponent<PhotonView> ().RPC ("Initialize", PhotonTargets.AllBuffered, teamId);
	}

	public bool ShouldBeActive(GameObject entity) {
		return true;
	}

	public void InitialSheepSpawn(int[] teamIds, int sheepPerTeam) {
		foreach (int teamId in teamIds) {
			for (int i=0; i < sheepPerTeam; i++) {
				SpawnSheep(teamId);
			}
		}
	}
		
	private Vector3 GetBaseForTeam(List<EntitySpawnPoint> points, int teamId) {
		List<EntitySpawnPoint> teamSpawns = points.FindAll ((point) => {
			return (int)point.spawnTeam == teamId;
		});

		return teamSpawns[Random.Range (0, teamSpawns.Count - 1)].transform.position;
	}

	private Vector3 MakePointAroundBase(Vector3 basePosition) {
		int tryCount = 0;

		while(true) {
			Vector3 spawnPos = AtaUtility.RandPointInRadius(basePosition, spawnRadRange.x, spawnRadRange.y);
			if(!Physics.CheckSphere(spawnPos, spawnCheckRad, LayerMask.GetMask("Default"))) {
				return spawnPos;
			}

			if (tryCount == 10) {
				Debug.Log("Gave up on spawn: " + gameObject.name);
				return basePosition;
			}

			tryCount++;
		}
	}

	private void GetSpawnPoints() {
		foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("Respawn")) {
			EntitySpawnPoint spawnPoint = gameObject.GetComponent<EntitySpawnPoint> ();

			if (spawnPoint == null) {
				Debug.LogError ("GameObject tagged as Respawn named " + gameObject.name + " should have a EntitySpawnPoint attached.");
				continue;
			}

			if (spawnPoint.SpawnsSheep) {
				sheepSpawns.Add(spawnPoint);
			}

			if (spawnPoint.SpawnsDogs) {
				dogSpawns.Add(spawnPoint);
			}
		}
	}
}
