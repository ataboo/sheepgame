using UnityEngine;
using System.Collections.Generic;

public interface INetworkCharacter {
	object[] GetSyncVars ();

	int GetSyncCount();

	void PutSyncVars (object[] syncVars);
}

public class NetworkCharacter : Photon.MonoBehaviour
{
      void Update()
    {

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		INetworkCharacter iNetCharacter = GetComponent<INetworkCharacter>();
        if (stream.isWriting) {
			// We own this player: send the others our data
			foreach (object syncVar in iNetCharacter.GetSyncVars()) {
				stream.SendNext (syncVar);
			}
        } else {
			int syncCount = iNetCharacter.GetSyncCount ();
            
			object[] syncVars = new object[stream.Count];
			for (int i = 0; i < syncCount; i++) {
				syncVars [i] = stream.ReceiveNext ();
			}

			iNetCharacter.PutSyncVars (syncVars);
        }
    }
}
