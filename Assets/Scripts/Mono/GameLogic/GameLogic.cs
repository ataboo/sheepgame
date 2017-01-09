using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;


public class GameLogic : MonoBehaviour, DeathListener {
	public static PhotonView photonView;

	private const int SPAWN_COUNT = 8;

	public int totalSheep = SPAWN_COUNT;
	public int sheepInGoal = 0;
	public int deadSheep = 0;
	public UIInterface uiInterface;

	public bool GameOver {
		get {
			return gameOver;
		}
	}

	public LevelSettings.LevelOption levelOption = LevelSettings.LevelOption.KOTH_CASTLE;

	private bool paused = false;
	private EntitySpawner spawner;
	private float startTime;
	private List<GameObject> entities;

	private bool gameOver = false;

	public void Awake() {
		GameLogic.photonView = GetComponent<PhotonView> ();
		spawner = GetComponent<EntitySpawner> ();
	}

	public void Start() {
		Initialize ();

		if (PhotonNetwork.isMasterClient) {
			int[] teamIds = (int[])PhotonNetwork.room.CustomProperties [SheepGameMenuController.TEAM_IDS_KEY];
			spawner.InitialSheepSpawn (teamIds, levelOption.sheepPerTeam);
		}

		LevelSettings.DogOption dogOption = LevelSettings.DogOption.GetOption((int)PhotonNetwork.player.CustomProperties[SheepGamePlayerRow.DOG_SELECT_KEY]);
		spawner.SpawnDog (dogOption);
	}

	public void OnEnable() {
		SceneManager.sceneLoaded += OnLevelDoneLoading;
	}

	public void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelDoneLoading;
	}

	public void OnLevelDoneLoading(Scene scene, LoadSceneMode mode) {
		// Startup Photon Networking again now that the level is loaded.
		PhotonNetwork.isMessageQueueRunning = true;
	}
		
	public void IsKill(EntityController entityController) {
		if (entityController.RespawnTime >= 0) {
			//TODO: schedule delayed respawn
			spawner.RespawnEntity(entityController);
		} else {
			DestroyImmediate (entityController.gameObject);
			// Count updated via CheckOut in EntityRegistry.
			UpdateHud ();
		}
	}

	public void QuitToMain(bool leaveRoom) {
		if (leaveRoom) {
			PhotonNetwork.LeaveRoom ();
		}

		LoadLobby ();
	}

	private void LoadLobby() {
		PhotonNetwork.isMessageQueueRunning = false;
		SceneManager.LoadScene ("Lobby");
	}
		
	public bool IsPaused() {
		return paused;
	}

	private void Initialize() {
		deadSheep = 0;
		totalSheep = SPAWN_COUNT;
		startTime = Time.time;
	}
		
	private void UpdateHud() {
		//uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);
	}

	private void DisplayEndScreen() {
		uiInterface.ShowEndScreen(totalSheep, sheepInGoal, deadSheep, Time.time - startTime);
	}

	[PunRPC]
	void EndGame(int totalSheep, int sheepInGoal, int deadSheep) {
		//SetPause(true);
		this.totalSheep = totalSheep; 
		this.sheepInGoal = sheepInGoal; 
		this.deadSheep = deadSheep;

		UpdateHud();
		DisplayEndScreen ();

		gameOver = true;

		StartCoroutine (CountDown (5));
	}

	public List<GameObject> GetEntities(bool refresh = false) {
		if (entities == null || refresh) {
			Debug.Log ("Rebuilding Entities");

			entities = new List<GameObject> ();

			entities.AddRange (GameObject.FindGameObjectsWithTag ("entity"));
			// May need to filter innactive.
			entities.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		}

		return entities;
	}

	public void CheckEndConditions(EntityRegistry.Team team) {
		


		if (totalSheep - sheepInGoal - deadSheep == 0) {
			photonView.RPC ("EndGame", PhotonTargets.All, totalSheep, sheepInGoal, deadSheep);
		}
	}
		
	private void SetPause(bool paused) {
		this.paused = paused;
		Time.timeScale = paused ? 0f : 1f;
	}

	private IEnumerator CountDown(int startTime) {
		int count = startTime;

		while (count > 0) {
			uiInterface.SetLeaveCount (count);
			yield return new WaitForSeconds(1);
			count--;
		}

		QuitToMain (false);
	}

	public void JustAte(EntityController entity) {
		//TODO: Increment food count
	}
}

