using UnityEngine;

public class TestMatchmaker : Photon.PunBehaviour
{
    private PhotonView myPhotonView;
	private EntitySpawner entitySpawner;

    // Use this for initialization
    public void Start()
    {
		entitySpawner = GetComponent<EntitySpawner> ();

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
		RoomOptions roomOptions = new RoomOptions () { IsVisible = true, MaxPlayers = 2 };
		PhotonNetwork.JoinOrCreateRoom ("TestRoom", roomOptions, TypedLobby.Default);
		Debug.Log ("OnJoinedLobby");
    }

//    public override void OnConnectedToMaster()
//    {
////        // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
////        PhotonNetwork.JoinRandomRoom();
//
//		
//    }

    public void OnPhotonRandomJoinFailed()
    {
        //PhotonNetwork.CreateRoom("BestTestRoom");
    }

    public override void OnJoinedRoom()
    {
        

		entitySpawner.SpawnDog ();
		entitySpawner.SpawnDog ();
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
