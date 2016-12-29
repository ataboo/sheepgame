using UnityEngine;
using UnityEngine.Networking;

public interface DeathListener {
	void IsKill(GameObject gameObject, bool respawnable);
}

public class DeathCheck : MonoBehaviour {
	public bool respawnable = false;
	public float deathHeight = -20f;
	private DeathListener deathListener;

	public void Awake() {
		this.deathListener = (DeathListener) GameObject.FindGameObjectWithTag ("LevelManager").GetComponent<GameNetworking> ();
	}

	public void Update () {

		if (transform.position.y < deathHeight) {
			isKill();
		}
	}

	private void isKill() {
		if (deathListener == null) {
			Debug.LogError("DeathCheck has no entitydelegate.");
		} else {
			deathListener.IsKill(gameObject, respawnable);
		}
	}
}
