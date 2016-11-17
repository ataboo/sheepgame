using UnityEngine;
using UnityEngine.Networking;

public class SheepNetworkManager : NetworkManager {
    private EntitySpawner entitySpawner;

    void Awake() {
        entitySpawner = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<EntitySpawner>();
    }
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) 
    {
        GameObject dogOne = entitySpawner.SpawnNetDog("DogOne_" + playerControllerId);
        NetworkServer.AddPlayerForConnection(conn, dogOne, playerControllerId);
        dogOne.GetComponent<PlayerControl>().RpcSetControl((int) PlayerControl.DogControl.DogOne);
    }
}
