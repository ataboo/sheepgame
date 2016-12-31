using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface PlayerRowListener {
	void OnReadyChange();
}

public class PlayerRow : MonoBehaviour, INetworkCharacter {
	public Toggle readyToggle;
	public Button dogPickButton;
	public Button teamPickButton;
	public Text playerName;

	private PlayerRowListener listener;
	private CanvasGroup rowGroup;
	private CanvasGroup readyGroup;

	private PhotonView photonView;

	public void SetListener(PlayerRowListener listener) {
		this.listener = listener;
	}

	public void SetPlayerName() {
		string playerName = photonView.owner.NickName;

		if (playerName == "") {
			playerName = "Player " + photonView.ownerId;
		}

		if (photonView.isMine) {
			rowGroup.interactable = true;
			readyGroup.interactable = true;

			this.playerName.text = playerName + " (you)";
		} else {
			this.playerName.text = playerName; 
		}
	}

	void Awake() {
		photonView = GetComponent<PhotonView> ();
		rowGroup = GetComponent<CanvasGroup> ();
		readyGroup = readyToggle.GetComponent<CanvasGroup> ();
	}

	// Use this for initialization
	void Start () {
		GameObject.Find ("MainCanvas").GetComponent<MenuController> ().OnMakeLobbyPlayer (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnReadyChange(bool ready) {
		listener.OnReadyChange ();

		rowGroup.interactable = !ready;
	}

	public object[] GetSyncVars ()
	{
		return new object[] {
			readyToggle.isOn
		};
	}

	public void PutSyncVars (object[] syncVars)
	{
		readyToggle.isOn = (bool)syncVars [0];
	}

	public int GetSyncCount ()
	{
		return 1;
	}
}
