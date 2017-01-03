using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SheepGameMenuController : MenuController {
	public string lobbyRowPrefabName = "PCLobbyRow";

	public override List<string> LevelOptions ()
	{
		return LevelSettings.LevelOption.OPTION_KEYS;
	}

	protected override string LobbyRowPrefabName ()
	{
		return lobbyRowPrefabName;
	}
}
