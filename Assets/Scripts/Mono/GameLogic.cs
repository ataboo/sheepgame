using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;


public class GameLogic : MonoBehaviour, CountListener, DeathListener {
	public static PhotonView photonView;
	
	public int totalSheep = 0;
	public int sheepInGoal = 0;
	public int deadSheep = 0;
	public UIInterface uiInterface;

	private bool paused = false;
	private EntitySpawner spawner;
	private float startTime;
	private List<GameObject> entities;

	public void Awake() {
		GameLogic.photonView = GetComponent<PhotonView> ();
		this.spawner = GetComponent<EntitySpawner> ();
	}

	public void Start() {
		InitValues ();
		this.totalSheep = spawner.sheepCount;
		SheepCounter sheepCounter = GameObject.FindGameObjectWithTag ("sheep-target").GetComponent<SheepCounter> ();
		sheepCounter.SetListener (this);
	}

	public void ExitToMenu() {
		SetPause(false);
		SceneManager.LoadScene("MenuScene");
	}
		
	[PunRPC]
	public void OnCountChange(int newCount) {
		this.sheepInGoal = newCount;

		UpdateHud ();

		if (PhotonNetwork.isMasterClient) {
			CheckEnd ();
		}
	}

	//============== DeathListener ================
	public void IsKill(GameObject gameObject, bool respawnable) {
		if (respawnable) {
			spawner.RespawnDog(gameObject);
		} else {
			DestroyImmediate (gameObject);
			deadSheep++;

			GetEntities (true);
			UpdateHud ();
		}
	}

	//============== IGameState ================
	public bool IsPaused() {
		return paused;
	}

	private void InitValues() {
		deadSheep = 0;
		startTime = Time.time;
	}
		
	private void UpdateHud() {
		uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);
	}

	[PunRPC]
	void ShowEndScreen() {
		SetPause(true);
		uiInterface.ShowEndScreen(totalSheep, sheepInGoal, deadSheep, Time.time - startTime);
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
			SetPause(true);
			//Show EndScreen
		}
	}
		
	private void SetPause(bool paused) {
		this.paused = paused;
		Time.timeScale = paused ? 0f : 1f;
	}
}

