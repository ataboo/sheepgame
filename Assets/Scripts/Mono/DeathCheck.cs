using UnityEngine;
using UnityEngine.Networking;

public interface DeathListener {
	void IsKill(GameObject gameObject, bool respawnable);
}

public class DeathCheck : NetworkToggleable {
	public bool respawnable = false;
	public float deathHeight = -20f;
	private DeathListener deathListener;

	override public void ServerAwake() {
		this.deathListener = (DeathListener) GameObject.FindGameObjectWithTag ("LevelManager").GetComponent<LevelManager> ();
	}

	override public void ServerUpdate () {

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
