using UnityEngine;
using UnityEngine.Networking;

public interface DeathListener {
	void IsKill(GameObject gameObject);
}

public class DeathCheck : MonoBehaviour {
	public float deathHeight = -20f;
	private DeathListener deathListener;

	public void Awake() {
		this.deathListener = (DeathListener) GameObject.FindGameObjectWithTag ("GameLogic").GetComponent<DeathListener> ();
	}

	public void Update () {
		if (!PhotonNetwork.isMasterClient) {
			return;
		}
		if (transform.position.y < deathHeight) {
			isKill();
		}
	}

	private void isKill() {
		if (deathListener == null) {
			Debug.LogError("DeathCheck has no entitydelegate.");
		} else {
			deathListener.IsKill(gameObject);
		}
	}
}
