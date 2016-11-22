using UnityEngine;
using UnityEngine.Networking;

public interface EntityDelegate {
	void IsKill(GameObject gameObject, bool imortal);
}
public class DeathCheck : MonoBehaviour {
	public bool respawnable = false;
	public float deathHeight = -20f;
	private EntityDelegate entDelegate;

	public void Awake() 
	{
		if (!GetComponent<NetworkIdentity> ().isServer) {
			enabled = false;
		}
	}

	public void FixedUpdate () {

		if (transform.position.y < deathHeight) {
			isKill();
		}
	}

	private void isKill() {
		if (entDelegate == null) {
			Debug.LogError("DeathCheck has no entitydelegate.");
		} else {
			entDelegate.IsKill(gameObject, respawnable);
		}
	}
}
