using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SheepGameMenuController : MenuController {
	public string lobbyRowPrefabName = "PCLobbyRow";
	public const string TEAM_IDS_KEY = "room_team_ids";

	public override List<string> LevelOptions ()
	{
		return LevelSettings.LevelOption.OPTION_KEYS;
	}

	protected override string LobbyRowPrefabName ()
	{
		return lobbyRowPrefabName;
	}

	protected override void OnBeforeLaunch ()
	{
		FinalizeTeams ();
	}

	private void FinalizeTeams () {
		List<int> teamIds = new List<int> ();

		foreach (PhotonPlayer player in PhotonNetwork.playerList) {
			int teamId = (int)player.CustomProperties [SheepGamePlayerRow.TEAM_SELECT_KEY];
			if (!teamIds.Contains(teamId)) {
				teamIds.Add (teamId);
			}
		}

		PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable(){
			{TEAM_IDS_KEY, teamIds.ToArray()} 
		});
	}
}
