using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;

public interface IGameState {
	bool IsPaused();
}

public class LevelManager : NetworkToggleable, CountListener, DeathListener, IGameState {
	[SyncVar]
	public int totalSheep = 0;
	[SyncVar]
	public int sheepInGoal = 0;
	[SyncVar]
	public int deadSheep = 0;
	[SyncVar]
	private bool paused = false;

	private UIInterface uiInterface;
	private EntitySpawner spawner;
	private float startTime;
	private List<GameObject> entities;

	//private bool gameOver = false;

	public void Awake() {
		this.uiInterface = GetComponent<UIInterface>();
		this.spawner = GetComponent<EntitySpawner> ();
	}

	public override void BothAwake() {
		
	}

	public override void ServerAwake() {
		InitValues ();
		this.totalSheep = spawner.sheepCount;
		SheepCounter sheepCounter = GameObject.FindGameObjectWithTag ("sheep-target").GetComponent<SheepCounter> ();
		sheepCounter.SetListener (this);
	}

	public override void ServerStart() {
	}

	public override void ServerUpdate() {
		UpdateHud ();
	}

	public void ExitToMenu() {
		SetPause(false);
		SceneManager.LoadScene("MenuScene");
	}

	//============== CountListener =============
	[Server]
	public void OnCountChange(int newCount) {
		this.sheepInGoal = newCount;

		UpdateHud ();

		RpcUpdateHud ();
	}

	//============== DeathListener ================
	[Server]
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
		

	[Server]
	private void UpdateHud() {
		uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);

		RpcUpdateHud ();

		CheckEnd();
	}

	[ClientRpc]
	void RpcUpdateHud() {
		uiInterface.UpdateHud (totalSheep, sheepInGoal, deadSheep);
	}

	[ClientRpc]
	void RpcShowEndScreen() {
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

	[Server]
	private void CheckEnd() {
		if (totalSheep - sheepInGoal - deadSheep == 0) {
			SetPause(true);
			uiInterface.ShowEndScreen(totalSheep, sheepInGoal, deadSheep, Time.time - startTime);
			RpcShowEndScreen ();
		}
	}
		
	private void SetPause(bool paused) {
		this.paused = paused;
		Time.timeScale = paused ? 0f : 1f;
	}

	private void SendToEntities(string message) {
		foreach(GameObject entity in GameObject.FindGameObjectsWithTag("entity")) {
			entity.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SendToGameObjects(string message) {
		foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()) {
			obj.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void RebuildEntityList() {
		Debug.Log("Ran Rebuild local.");
		this.entities = null;
	}

	[Command]
	public void CmdRebuildEntityList() {
		Debug.Log("Ran REbuild command.");
		this.entities = null;
	}
}

