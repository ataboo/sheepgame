using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	private EntitySpawner entitySpawner;
	private UIInterface uiInterface;
	private int totalSheep = 0;
	private int sheepInGoal = 0;
	private int deadSheep = 0;
	private float startTime;
	private bool gameOver = false;
	private bool paused = false;

	// Use this for initialization
	void Start() {
		this.entitySpawner = GetComponent<EntitySpawner>();
		this.uiInterface = GetComponent<UIInterface>();

		deadSheep = 0;
		startTime = Time.time;
		totalSheep = entitySpawner.GetSheepCount();
		UpdateHud();
	}

	void Update() {
		if (gameOver) {
			return;
		}

		if(Input.GetButtonDown("Cancel")) {
			TogglePause();
		}
	}

	public void IsKill(GameObject gameObject, DeathCheck deathCheck) {
		if (deathCheck.respawnable) {
			entitySpawner.RespawnDog(gameObject);
		} else {
			GameObject.Destroy(gameObject);
		}

		// if (deathCheck.IsSheep()) {
		// 	deadSheep++;
		// 	UpdateHud();
		// }
	}

	public void UpdateGoal(int sheepInGoal) {
		this.sheepInGoal = sheepInGoal;

		UpdateHud();
	}

	public void TogglePause() {
		this.paused = !paused;
		SetPause(paused);
		uiInterface.ShowPauseScreen(paused);
	}

	public void ExitToMenu() {
		SetPause(false);
		SceneManager.LoadScene("MenuScene");
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
