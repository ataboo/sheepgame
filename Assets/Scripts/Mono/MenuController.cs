using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System;

public class MenuController : MonoBehaviour, PlayerRowListener {
	public CanvasGroup homeMenuGroup;
	public CanvasGroup onlineGroup;
	public GameObject lobbyMenu;

	public InputField roomNameField;
	public InputField playerNameField;
	public Text lobbyTitle;
	public Text countDownText;

	public GameObject pcLobbyRow;

	public Transform playerRows;

	private PhotonView photonView;

	private int countTime = -1;

	private GameNetworking gameNetworking;
	private static MenuController instance;

	IEnumerator countdownCoroutine;

	public static MenuController Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(MenuController)) as MenuController;
			}

			return instance;
		}
	}

//	public void OpenSoloTest() {
//		SceneManager.LoadScene("SoloTestLevel");
//	}



//	public void OpenMultiTest()
//	{
//		SceneManager.LoadScene ("Lobby");
//	}

	public void Awake() {
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}

		this.gameNetworking = GetComponent<GameNetworking> ();
		this.photonView = GetComponent<PhotonView> ();
	}

	public void Start() {
		DontDestroyOnLoad (gameObject);

		ShowHomeMenu (true);
		lobbyMenu.SetActive (false);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void OnJoinedLobby() {
		onlineGroup.interactable = true;
	}


	public void OnLobbyBack() {
		gameNetworking.RoomDisconnect ();
		lobbyMenu.SetActive(false);
		ShowHomeMenu (true);
	}

	public void OnMakeLobbyPlayer(GameObject lobbyPlayer) {
		PlayerRow playerRow = lobbyPlayer.GetComponent<PlayerRow> ();

		playerRow.SetPlayerName ();
		lobbyPlayer.GetComponent<PlayerRow> ().SetListener (this);
		lobbyPlayer.transform.SetParent (playerRows, false);
		SortPlayerRows ();
	}

	private void SortPlayerRows() {
		PhotonView[] photonViews = playerRows.GetComponentsInChildren<PhotonView> (true);

		Array.Sort (photonViews, delegate(PhotonView photon1, PhotonView photon2) {
			return photon1.ownerId.CompareTo (photon2.ownerId);
		});

		for (int i=0; i< photonViews.Length; i++) {
			photonViews [i].gameObject.transform.SetSiblingIndex (i);
		}

	}

	public void OnHostClick() {
		string roomName = PrepareToOpenLobby ();

		gameNetworking.HostRoom (roomName);
	}

	public void OnJoinClick() {
		string roomName = PrepareToOpenLobby ();

		gameNetworking.JoinRoom (roomName);
	}

	private string PrepareToOpenLobby() {
		ShowHomeMenu (false);

		string roomName = GetRoomName ();
		lobbyTitle.text = "Lobby: " + roomName;

		PhotonNetwork.player.NickName = GetPlayerName ();

		return roomName;
	}

	public void OnJoinedRoom() {
		lobbyMenu.SetActive (true);
	}

	public void OnPhotonError(string errorMessage) {
		lobbyMenu.SetActive (false);
		ShowHomeMenu (true);

		//TODO display error.
	}

	private string GetRoomName() {
		//TODO: validation?

		return roomNameField.text != "" ? roomNameField.text : "TestRoom";
	}

	private string GetPlayerName() {
		//TODO: validation?

		return playerNameField.text;
	}

	private void ShowHomeMenu(bool show) {
		homeMenuGroup.interactable = show;
		homeMenuGroup.alpha = show ? 1.0f : 0.4f;
		homeMenuGroup.blocksRaycasts = show;
	}


	public void OnReadyChange ()
	{
		photonView.RPC ("CheckAllReady", PhotonTargets.MasterClient);
	}

	[PunRPC]
	public void CheckAllReady() 
	{
		ResetCountdown ();

		foreach(GameObject rowObject in GameObject.FindGameObjectsWithTag("Player")) {
			PlayerRow playerRow = rowObject.GetComponent<PlayerRow> ();
			if (playerRow == null) {
				continue;
			}
			if (!playerRow.readyToggle.isOn) {
				photonView.RPC ("SetCountdown", PhotonTargets.All, -1);
				return;
			}
		}

		countdownCoroutine = CountDown ();

		this.countTime = 5;
		StartCoroutine (countdownCoroutine);
	}

	private void ResetCountdown() {
		if (countdownCoroutine != null) {
			StopCoroutine (countdownCoroutine);
		}
	}

	[PunRPC]
	public void SetCountdown(int count)
	{
		countTime = count;

		if (countTime > 0) {
			countDownText.enabled = true;
			countDownText.text = "Starting in " + count + "...";
		} else if (countTime == 0) {
			countDownText.text = "Launching...";
			countTime = -1;
		} else {
			countDownText.enabled = false;
		}
	}

	private IEnumerator CountDown() {
		Debug.Log ("Ran Coroutine...");

		while (countTime >= 0) {
			Debug.Log ("Sending Count: " + countTime);
			photonView.RPC ("SetCountdown", PhotonTargets.All, countTime);
			countTime--;
			yield return new WaitForSeconds(1);
		}
	}
}
