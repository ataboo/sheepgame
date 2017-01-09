using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public abstract class MenuController : MonoBehaviour, PlayerRowListener, NetworkListener {
	public const string LEVEL_SELECT_KEY = "level_select";

	public CanvasGroup homeMenuGroup;

	public CanvasGroup homeOnlineGroup;
	public InputField roomNameField;
	public InputField playerNameField;

	public GameObject lobbyMenu;
	public Text lobbyTitle;
	public Text countDownText;
	public Dropdown levelSelect;

	public Transform playerRowHolder;

	private PhotonView photonView;

	private int countTime = -1;

	private IGameNetworking gameNetworking;

	IEnumerator countdownCoroutine;

	abstract public List<string> LevelOptions ();

	abstract protected string LobbyRowPrefabName ();

	abstract protected void OnBeforeLaunch ();

	public void Awake() {
		this.gameNetworking = GameNetworking.Instance;
		gameNetworking.SetNetworkListener(this);
		this.photonView = GetComponent<PhotonView> ();
	}

	public void Start() {
		levelSelect.ClearOptions ();
		levelSelect.AddOptions (LevelOptions());
	}

	public void OnEnable() {
		SceneManager.sceneLoaded += OnLevelDoneLoading;
	}

	public void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelDoneLoading;
	}

	public void OnLevelDoneLoading(Scene scene, LoadSceneMode mode) {
		PhotonNetwork.isMessageQueueRunning = true;

		if (PhotonNetwork.inRoom) {
			OnJoinedRoom ();
		}

		if (PhotonNetwork.insideLobby) {
			OnJoinedLobby ();
		} else {
			gameNetworking.ConnectToPhoton ();
		}
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void OnJoinedLobby() {
		homeOnlineGroup.interactable = true;
	}

	public void OnLobbyBack() {
		PhotonNetwork.LeaveRoom ();
		lobbyMenu.SetActive(false);
		ShowHomeMenu (true);
	}

	private void SortPlayerRows() {
		PhotonView[] photonViews = playerRowHolder.GetComponentsInChildren<PhotonView> (true);

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
		InitLobbyPlayer ();

		lobbyMenu.SetActive (true);

		if (PhotonNetwork.isMasterClient) {
			levelSelect.enabled = true;
			if (!PhotonNetwork.room.CustomProperties.ContainsKey (LEVEL_SELECT_KEY)) {
				OnLevelSelect (0);
			}
		}
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
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		if (AllPlayersReady ()) {
			StartCountdown ();
		} else {
			AbortCountdown ();
		}
	}

	private void StartCountdown() {
		levelSelect.enabled = false;
		this.countTime = 5;

		countdownCoroutine = CountDown ();
		StartCoroutine (countdownCoroutine);
	}

	private void AbortCountdown() {
		if (countdownCoroutine != null) {
			StopCoroutine (countdownCoroutine);
		}

		levelSelect.enabled = true;
		photonView.RPC ("SetCountdown", PhotonTargets.All, -1);
	}

	private bool AllPlayersReady() {
		foreach(GameObject playerRow in PlayerRows()) {
			if (!playerRow.GetComponent<IPlayerRow>().IsReady()) {
				return false;
			}
		}
		return true;
	}

	private GameObject[] PlayerRows() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		return Array.FindAll (players, player => player.GetComponent<IPlayerRow> () != null);
	}

	[PunRPC]
	public void SetCountdown(int count)
	{
		countTime = count;
		countDownText.enabled = countTime > -1;

		if (countTime == 0) {
			countDownText.text = "Launching...";
			countTime = -1;
			LaunchLevel ();
		} else {
			countDownText.text = "Starting in " + count + "...";
		}
	}

	private IEnumerator CountDown() {
		while (countTime >= 0) {
			Debug.Log ("Sending Count: " + countTime);
			photonView.RPC ("SetCountdown", PhotonTargets.All, countTime);
			countTime--;
			yield return new WaitForSeconds(1);
		}
	}

	private void LaunchLevel() {
		OnBeforeLaunch ();

		PhotonNetwork.isMessageQueueRunning = false;
		string levelName = LevelSettings.LevelOption.OPTION_KEYS[(int)PhotonNetwork.room.CustomProperties [LEVEL_SELECT_KEY]];
		string sceneName = LevelSettings.LevelOption.OPTIONS [levelName].SceneName;

		SceneManager.LoadScene(sceneName);
	}

	/// <summary>
	/// Adds a newly created lobby player prefab to playerRows.
	/// </summary>
	/// <param name="lobbyPlayer">Lobby player prefab.</param>
	public void DockLobbyPlayer(GameObject lobbyPlayer) {
		lobbyPlayer.transform.SetParent (playerRowHolder, false);
		SortPlayerRows ();
	}

	private void InitLobbyPlayer() {
		PhotonNetwork.Instantiate (LobbyRowPrefabName(), Vector3.zero, Quaternion.Euler (Vector3.zero), 0);
	}

	public void OnPlayerPropertiesChanged (PhotonPlayer player, ExitGames.Client.Photon.Hashtable changedProperties)
	{
	}

	public void OnRoomPropertiesChanged (ExitGames.Client.Photon.Hashtable changedProperties)
	{
		if (PhotonNetwork.isMasterClient) {
			return;
		}

		if (changedProperties.ContainsKey(LEVEL_SELECT_KEY)) {
			levelSelect.value = (int)changedProperties[LEVEL_SELECT_KEY];	
		}
	}

	public void OnLevelSelect(int value) {
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		PhotonNetwork.room.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { {LEVEL_SELECT_KEY, value } }, null, false);
	}
}
