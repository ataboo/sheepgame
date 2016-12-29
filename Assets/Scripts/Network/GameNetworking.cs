using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour
{

	private static PhotonView scenePhotonView;

    // Use this for initialization
    public void Start()
    {
        scenePhotonView = this.GetComponent<PhotonView>();
    }

    public void OnJoinedRoom()
    {
		Debug.Log ("OnJoinedRoom");

		/*
        // game logic: if this is the only player, we're "it"
        if (PhotonNetwork.playerList.Length == 1)
        {
            playerWhoIsIt = PhotonNetwork.player.ID;
        }

        Debug.Log("playerWhoIsIt: " + playerWhoIsIt);
		*/
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);

        if (PhotonNetwork.isMasterClient)
        {
        }

    }

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisconnected: " + player);
		
		if (PhotonNetwork.isMasterClient)
		{

		}
	}
	
	public void OnMasterClientSwitched()
	{
		Debug.Log("OnMasterClientSwitched");
	}

	/*
    public static void TagPlayer(int playerID)
    {
        Debug.Log("TagPlayer: " + playerID);
        scenePhotonView.RPC("TaggedPlayer", PhotonTargets.All, playerID);
    }

    [PunRPC]
    public void TaggedPlayer(int playerID)
    {
        playerWhoIsIt = playerID;
        Debug.Log("TaggedPlayer: " + playerID);
    }
	*/

}
