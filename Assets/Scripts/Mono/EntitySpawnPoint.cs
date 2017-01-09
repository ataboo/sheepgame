using UnityEngine;
using System.Collections;

public class EntitySpawnPoint : MonoBehaviour {
	public enum SpawnTeams {
		TeamOne = 0,
		TeamTwo = 1,
		TeamThree = 2,
		TeamFour = 3
	}

	public enum EntityType {
		Dog,
		Sheep,
		All
	}

	public bool SpawnsDogs {
		get {
			return entityType == EntityType.Dog || entityType == EntityType.All;
		}
	}

	public bool SpawnsSheep {
		get {
			return entityType == EntityType.Sheep || entityType == EntityType.All;
		}
	}

	public SpawnTeams spawnTeam;
	public EntityType entityType;
}
