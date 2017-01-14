using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityRegistry : MonoBehaviour, CountListener, FoodListener {

	public class Team {
		private int teamId;
		public int TeamId{
			get {
				return teamId;
			}
		}
		public int dogsKilled = 0;
		public int sheepKilled = 0;
		public int grassEaten = 0;
		public int sheepAlive = 0;
		public int sheepInGoal = 0;

		public int SheepInWild{
			get {
				return sheepAlive - sheepInGoal;
			}
		}

		public int SheepStartCount {
			get {
				return sheepKilled + sheepAlive;
			}
		}

		public List<string> memberUIDs = new List<string>();

		public Team(int teamId) {
			this.teamId = teamId;
		}
	}

	public Team[] TeamArray {
		get {
			Team[] teamArray = new Team[teams.Count];
			teams.Values.CopyTo(teamArray, 0);
			return teamArray;
		}
	}

	public UIInterface uiInterface;

	private GameLogic gameLogic;

	int entityIncrement = 0;
	Dictionary<int, Team> teams = new Dictionary<int, Team>();

	public void Awake() {
		gameLogic = GetComponent<GameLogic> ();
	}

	public string CheckIn(EntityController entity)
	{
		string uid = MakeUID (entity);

		AddEntity (entity);

		entityIncrement++;
		return uid;
	}
		
	public void CheckOut(EntityController entity)
	{
		RemoveEntity (entity);
		Team team = teams [entity.TeamId];

		if (entity.SpawnType == EntitySpawnPoint.EntityType.Sheep) {
			team.sheepAlive--;
			team.sheepKilled++;
			//TODO: Sheep killed with last blow.
		} else {
			team.dogsKilled++;
		}

		UpdateStatsDisplay (team);
	}
		
	public void JustAteGrass(int teamId) {

		if (!PhotonNetwork.isMasterClient) {
			Debug.LogError ("JustAteGrass should only be called on the master client.");
			return;
		}
		Team team = teams [teamId];
		GetComponent<PhotonView> ().RPC ("UpdateGrassCount", PhotonTargets.AllBuffered, teamId, team.grassEaten + 1);
		Debug.Log("Just called just ate grass.");
	}

	[PunRPC]
	public void UpdateGrassCount(int teamId, int grassCount) {
		Team team = teams [teamId];

		team.grassEaten = grassCount;

		UpdateStatsDisplay (team);
	}

	public void OnCountChange(int sheepInGoal) {
		Team team = teams [0];

		team.sheepInGoal = sheepInGoal;

		UpdateStatsDisplay (team);
	}

	private string MakeUID(EntityController entity) {
		return entity.UID_Class + "_" + entityIncrement;
	}

	private void AddEntity(EntityController entity) {
		if (!teams.ContainsKey (entity.TeamId)) {
			teams.Add (entity.TeamId, new Team(entity.TeamId));
		}

		Team team = teams [entity.teamId];
		teams [entity.TeamId].memberUIDs.Add (entity.UID);

		if (entity.SpawnType == EntitySpawnPoint.EntityType.Sheep) {
			team.sheepAlive++;
		}

		UpdateStatsDisplay (team);
	}

	private void RemoveEntity(EntityController entity) {
		teams[entity.TeamId].memberUIDs.Remove (entity.UID);
	}

	private void UpdateStatsDisplay(Team team) {
		if (gameLogic.GameOver) {
			return;
		}

		uiInterface.UpdateHud ();

		if (PhotonNetwork.isMasterClient) {
			gameLogic.CheckEndConditions (team);		
		}
	}
}
