using UnityEngine;

public class GameNetworking : Photon.PunBehaviour
{
	private PhotonView myPhotonView;
	private EntitySpawner entitySpawner;
	private MenuController menuController;

	private const int MAX_PLAYERS = 2;


	// Use this for initialization
	public void Start()
	{
		entitySpawner = GetComponent<EntitySpawner> ();
		menuController = GetComponent<MenuController> ();

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
			Debug.LogWarning( "Couldn't connect to server" );
		}
	}

	public override void OnJoinedLobby()
	{
		Debug.Log ("OnJoinedLobby");

		menuController.OnJoinedLobby ();
	}

	public void JoinRoom(string roomName) {
		PhotonNetwork.JoinRoom (roomName);
	}

	public override void OnFailedToConnectToPhoton (DisconnectCause cause)
	{
		menuController.OnPhotonError ("Failed to connect to Photon Network.");
		Debug.LogError ("Failed to connect to photon with cause: " + cause);
	}

	public override void OnPhotonJoinRoomFailed (object[] codeAndMsg)
	{
		menuController.OnPhotonError ((string)codeAndMsg[1]);
		Debug.LogError ("Failed to join room with code: " + codeAndMsg[0] + " and message: " + codeAndMsg[1]);
	}

	public override void OnPhotonCreateRoomFailed (object[] codeAndMsg)
	{
		menuController.OnPhotonError ((string)codeAndMsg[1]);
		Debug.LogError ("Failed to create room with code: " + codeAndMsg[0] + " and message: " + codeAndMsg[1]);
	}

	public void HostRoom(string roomName) {
		RoomOptions roomOptions = new RoomOptions () { IsVisible = true, MaxPlayers = MAX_PLAYERS };

		PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log ("OnJoinedRoom");
		menuController.OnJoinedRoom ();
		InitLobbyPlayer();
	}

	private GameObject InitLobbyPlayer() {
		return PhotonNetwork.Instantiate ("PCLobbyRow", Vector3.zero, Quaternion.Euler (Vector3.zero), 0);
	}

	private void InitLevel() {
		entitySpawner.SpawnDog ();
		entitySpawner.SpawnDog ();

		if (PhotonNetwork.isMasterClient) {
			entitySpawner.InitialSheepSpawn ();
		}
	}

	public void RoomDisconnect() {
		PhotonNetwork.LeaveRoom ();
	}

	//	public override void OnPlayerJoined()
	//	{
	//
	//	}

	//    public void OnGUI()
	//    {
	//        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	//
	//        if (PhotonNetwork.inRoom)
	//        {
	//            bool shoutMarco = GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;
	//
	//            if (shoutMarco && GUILayout.Button("Marco!"))
	//            {
	//                myPhotonView.RPC("Marco", PhotonTargets.All);
	//            }
	//            if (!shoutMarco && GUILayout.Button("Polo!"))
	//            {
	//                myPhotonView.RPC("Polo", PhotonTargets.All);
	//            }
	//        }
	//    }
}
