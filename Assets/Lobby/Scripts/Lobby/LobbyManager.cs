using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;


namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;

        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;

        public RectTransform networkHomePanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        protected bool _disconnectServer = false;
        
        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = networkHomePanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (conn.playerControllers[0].unetView.isClient)
                    {
                        backDelegate = StopHostClbk;
                    }
                    else
                    {
                        backDelegate = StopClientClbk;
                    }
                }
                else
                {
                    ChangeTo(networkHomePanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != networkHomePanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
                //_isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
			topPanel.isInGame = false;
        }

        public void GoExitButton()
        {
			StopHost ();

			SceneManager.LoadScene ("MenuScene");

			Invoke("DestroySelf", 0.01f);
        }

		public void DestroySelf() {
			Destroy (gameObject);
		}

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

//		public override void OnClientSceneChanged(NetworkConnection conn) 
//		{
//			base.OnClientSceneChanged(conn);
//
//			EntitySpawner spawner = GameObject.FindGameObjectWithTag ("LevelManager").GetComponent<EntitySpawner> ();
//
//			foreach (PlayerController playerController in ClientScene.localPlayers) {
//				if (playerController.gameObject == null) {
//					GameObject dogOne = spawner.SpawnNetDog("DogOne_" + playerController.playerControllerId);
//					NetworkServer.AddPlayerForConnection(conn, dogOne, playerController.playerControllerId);
//
//					dogOne.GetComponent<PlayerControl> ().SetControls (PlayerControl.DogControl.DogOne);
//				}
//			}
//
//		Debug.Log("Ran spawn");
//		}

//		public override GameObject OnLobbyServerCreateGamePlayer (NetworkConnection conn, short playerControllerId) {
//			EntitySpawner spawner = GameObject.FindGameObjectWithTag ("LevelManager").GetComponent<EntitySpawner> ();
//
//			GameObject dogOne = spawner.SpawnNetDog("DogOne_" + playerControllerId);
//			//NetworkServer.AddPlayerForConnection(conn, dogOne, playerControllerId);
//			
//			dogOne.GetComponent<PlayerControl> ().SetControls (PlayerControl.DogControl.DogOne);
//
//			return dogOne;
//		}

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(networkHomePanel);
        }
                 
		public void StopHostClbk() {
            StopHost();

            ChangeTo(networkHomePanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            ChangeTo(networkHomePanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(networkHomePanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }
			
        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

//		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
//		{
//			base.OnMatchCreate(success, extendedInfo, matchInfo);
//            _currentMatchID = (System.UInt64)matchInfo.networkId;
//		}
//
//		public override void OnDestroyMatch(bool success, string extendedInfo)
//		{
//			base.OnDestroyMatch(success, extendedInfo);
//			if (_disconnectServer)
//            {
//                StopHost();
//            }
//        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);

			UpdateLobbyPlayers ();

            return obj;
        }


        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
			UpdateLobbyPlayers ();
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
			UpdateLobbyPlayers ();
        }

		private void UpdateLobbyPlayers() {
			for (int i = 0; i < lobbySlots.Length; ++i)
			{
				LobbyPlayer p = lobbySlots[i] as LobbyPlayer;
				
				if (p != null)
				{
					p.RpcUpdateRemoveButton();
					p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
				}
			}
		}

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
			bool allready = true;
			for(int i = 0; i < lobbySlots.Length; ++i)
			{
				if(lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

			if(allready)
				StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            ServerChangeScene(playScene);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(networkHomePanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(networkHomePanel);
            infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }
    }
}
