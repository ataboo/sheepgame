using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;


public class GameLogic : MonoBehaviour, CountListener, DeathListener, FoodListener {
	public static PhotonView photonView;

	private const int SPAWN_COUNT = 8;

	public int totalSheep = SPAWN_COUNT;
	public int sheepInGoal = 0;
	public int deadSheep = 0;
	public UIInterface uiInterface;

	private bool paused = false;
	private EntitySpawner spawner;
	private float startTime;
	private List<GameObject> entities;

	private bool gameOver = false;

	public void Awake() {
		GameLogic.photonView = GetComponent<PhotonView> ();
		this.spawner = GetComponent<EntitySpawner> ();
	}

	public void Start() {
		Initialize ();
		SheepCounter sheepCounter = GameObject.FindGameObjectWithTag ("sheep-target").GetComponent<SheepCounter> ();
		sheepCounter.SetListener (this);

		if (PhotonNetwork.isMasterClient) {
			spawner.InitialSheepSpawn (SPAWN_COUNT);
		}

		string dogOptionName = (string)LevelSettings.DogOption.OPTION_KEYS [(int)PhotonNetwork.player.CustomProperties [SheepGamePlayerRow.DOG_SELECT_KEY]];
		LevelSettings.DogOption dogOption = LevelSettings.DogOption.OPTIONS[dogOptionName];

		spawner.SpawnDog (dogOption);
	}
		
	[PunRPC]
	public void OnCountChange(int newCount) {
		if (gameOver) {
			return;
		}

		this.sheepInGoal = newCount;

		UpdateHud ();

		if (PhotonNetwork.isMasterClient) {
			CheckEnd ();
		}
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
		
	public void IsKill(GameObject gameObject) {
		if (IsDog(gameObject)) {
			spawner.RespawnDog(gameObject);
		} else {
			DestroyImmediate (gameObject);

			if (IsSheep (gameObject)) {
				deadSheep++;
			}

			GetEntities (true);
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


	private bool IsSheep(GameObject gameObject) {
		return gameObject.GetComponent<SheepController> () != null;
	}

	private bool IsDog(GameObject gameObject) {
		return gameObject.GetComponent<PlayerControl> () != null;
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
		uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);
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

	private void CheckEnd() {
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

