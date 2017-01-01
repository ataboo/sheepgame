using UnityEngine;

public interface NetworkListener {
	void OnPhotonError(string message);
}

public class GameNetworking : Photon.PunBehaviour
{
	private PhotonView myPhotonView;
	private EntitySpawner entitySpawner;
	public NetworkListener networkListener;

	private const int MAX_PLAYERS = 4;

	private static GameNetworking instance;

	public static GameNetworking Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType (typeof(GameNetworking)) as GameNetworking;
			}

			return instance;
		}
	}


	// Use this for initialization

	public void Awake() {
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

		entitySpawner = GetComponent<EntitySpawner> ();
	}

	public void Start()
	{
	}

	public void ConnectToPhoton() {
		if( PhotonNetwork.connectionState != ConnectionState.Disconnected )
		{
			return;
		}

		try
		{
			PhotonNetwork.ConnectUsingSettings( "0.1" );
			PhotonNetwork.autoJoinLobby = true;
		}
		catch
		{
			SendError ("Couldn't connect to server.");
		}
	}

	public void JoinRoom(string roomName) {
		PhotonNetwork.JoinRoom (roomName);
	}

	public override void OnFailedToConnectToPhoton (DisconnectCause cause)
	{
		SendError ("Failed to connect to Photon Network.");
		Debug.LogError ("Failed to connect to photon with cause: " + cause);
	}

	public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
	{
		SendError ((string)codeAndMsg[1]);
		Debug.LogError ("Failed to join room with code: " + codeAndMsg[0] + " and message: " + codeAndMsg[1]);
	}

	public override void OnPhotonCreateRoomFailed (object[] codeAndMsg)
	{
		SendError ((string)codeAndMsg[1]);
		Debug.LogError ("Failed to create room with code: " + codeAndMsg[0] + " and message: " + codeAndMsg[1]);
	}

	public void HostRoom(string roomName) {
		RoomOptions roomOptions = new RoomOptions () { IsVisible = true, MaxPlayers = MAX_PLAYERS };

		PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
	}

	private void SendError(string message) {
		Debug.LogError ("Sending Photon Error: " + message);

		if (networkListener != null) {
			networkListener.OnPhotonError (message);
		}
	}
}
