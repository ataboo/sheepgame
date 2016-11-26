using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkToggleable, CountListener {
	[SyncVar]
	public int totalSheep = 0;
	[SyncVar]
	public int sheepInGoal = 0;
	[SyncVar]
	public int deadSheep = 0;

	private UIInterface uiInterface;
	private EntitySpawner spawner;
	private float startTime;
	private bool gameOver = false;
	private bool paused = false;

	// Use this for initialization
	public override void ServerAwake() {
		this.uiInterface = GetComponent<UIInterface>();
		this.spawner = GetComponent<EntitySpawner> ();
		SheepCounter sheepCounter = GameObject.FindGameObjectWithTag ("SheepCounter").GetComponent<SheepCounter> ();

		sheepCounter.SetListener (this);
	}

	public override void ServerStart() {
		InitValues ();
		UpdateHud ();
	}

	public override void ServerUpdate() {
		if (gameOver) {
			// showScreen
		}

		UpdateHud();
	}

	public void IsKill(GameObject gameObject, DeathCheck deathCheck) {
		
	}

	public void UpdateGoal(int sheepInGoal) {
		this.sheepInGoal = sheepInGoal;
	}

	public void ExitToMenu() {
		SetPause(false);
		SceneManager.LoadScene("MenuScene");
	}

	//============== CountListener =============
	public void OnCountChange(int newCount) {
		this.totalSheep = newCount;
	}

	//============== DeathListener ===========
	public void IsKill(GameObject gameObject, bool respawnable) {
		if (respawnable) {
			spawner.RespawnDog(gameObject);
		} else {
			GameObject.Destroy(gameObject);
			deadSheep++;
		}
	}

	private void InitValues() {
		deadSheep = 0;
		startTime = Time.time;
	}
	
	private void UpdateHud() {
		uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);

		CheckEnd();
	}

	private void CheckEnd() {
		if (totalSheep - sheepInGoal - deadSheep == 0) {
			SetPause(true);
			uiInterface.ShowEndScreen(totalSheep, sheepInGoal, deadSheep, Time.time - startTime);
		}
	}

	private void SetPause(bool paused) {
		SendToEntities(paused ? "OnPause" : "OnResume");
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
}
