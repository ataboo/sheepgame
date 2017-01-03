using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SheepGamePlayerRow : PlayerRow {
	public const string DOG_SELECT_KEY = "dog_select";
	public const string TEAM_SELECT_KEY = "team_select";

	public Dropdown dogPick;
	public Dropdown teamPick;

	protected override void SetSelectOptions ()
	{
		dogPick.ClearOptions ();
		dogPick.AddOptions (LevelSettings.DogOption.OPTION_KEYS);

		OnDogChange (0);

		teamPick.ClearOptions ();
		teamPick.AddOptions (LevelSettings.TeamOption.OPTION_KEYS);

		OnTeamChange (0);
	}

	override protected object[] GetSubSyncVars() {
		return new object[] {
			dogPick.value,
			teamPick.value
		};
	}

	override protected void PutSubSyncVars(object[] syncVars) {
		dogPick.value = (int)syncVars[0];
		teamPick.value = (int)syncVars[1];
	}

	override protected int GetSubSyncCount() {
		return 2;
	}

	public void OnDogChange(int value) 
	{
		if (photonView.isMine) {
			base.SetPlayerProperty (DOG_SELECT_KEY, value);
		}
	}

	public void OnTeamChange(int value)
	{
		if (photonView.isMine) {
			base.SetPlayerProperty (TEAM_SELECT_KEY, value);
		}
	}

	protected override void SetOptionsInteractable (bool interactable)
	{
		dogPick.interactable = teamPick.interactable = interactable;
	}
}
