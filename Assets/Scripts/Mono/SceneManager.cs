using UnityEngine;

public class SceneManager : MonoBehaviour {
	private EntitySpawner entitySpawner;
	private UIInterface uiInterface;
	private int totalSheep = 0;
	private int sheepInGoal = 0;
	private int deadSheep = 0;
	private float startTime;

	// Use this for initialization
	void Start () {
		this.entitySpawner = GetComponent<EntitySpawner>();
		this.uiInterface = GetComponent<UIInterface>();

		Init();
	}

	private void Init() {
		totalSheep = entitySpawner.Reset();
		deadSheep = 0;
		startTime = Time.time;
		UpdateHud();
	}

	public void IsKill(GameObject gameObject, DeathCheck deathCheck) {
		if (deathCheck.respawnable) {
			entitySpawner.RespawnDog(gameObject);
		} else {
			GameObject.Destroy(gameObject);
		}

		if (deathCheck.IsSheep()) {
			deadSheep++;
			UpdateHud();
		}
	}

	public void UpdateGoal(int sheepInGoal) {
		this.sheepInGoal = sheepInGoal;

		UpdateHud();
	}

	public void Restart() {
		SetPause(false);

		entitySpawner.Reset();
		uiInterface.HideEndScreen();

		Init();
	}

	private void UpdateHud() {
		uiInterface.UpdateHud(totalSheep, sheepInGoal, deadSheep);

		CheckEnd();
	}

	private void CheckEnd() {
		Debug.Log("totalSheep: " + totalSheep);

		if (totalSheep - sheepInGoal - deadSheep == 0) {
			SetPause(true);
			uiInterface.ShowEndScreen(totalSheep, sheepInGoal, deadSheep, Time.time - startTime);
		}
	}

	private void SetPause(bool paused) {
		foreach(GameObject entity in GameObject.FindGameObjectsWithTag("entity")) {
				entity.SendMessage(paused ? "OnPause" : "OnResume", SendMessageOptions.DontRequireReceiver);
		}

		Time.timeScale = paused ? 0f : 1f;
	}
}
