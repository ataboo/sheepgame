using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface PlayerRowListener {
	void OnReadyChange();
}

public interface IPlayerRow {
	bool IsReady ();
}

public abstract class PlayerRow : MonoBehaviour, INetworkCharacter, IPlayerRow {
	public Toggle readyToggle;
	public Text playerName;

	protected PhotonView photonView;
	private PlayerRowListener listener;
	private CanvasGroup rowGroup;

	/// <summary>
	/// Set the options for any dropdown menus.
	/// </summary>
	protected abstract void SetSelectOptions ();

	/// <summary>
	/// Fetches an array of variables to add to those being synced between instances.
	/// This will be called on the PlayerRow instance owned by the local player.
	/// </summary>
	/// <returns>The sub sync variables.</returns>
	protected abstract object[] GetSubSyncVars ();

	/// <summary>
	/// Apply an array of variables to the instances of other players ontop of the ones already synced.
	/// </summary>
	/// <param name="syncVars">Sync variables.</param>
	protected abstract void PutSubSyncVars (object[] syncVars);

	/// <summary>
	/// Get the number of SyncVars being used.
	/// </summary>
	/// <returns>The sub sync count.</returns>
	protected abstract int GetSubSyncCount ();

	/// <summary>
	/// Set if ui options other than the ready button are interactable.
	/// </summary>
	/// <param name="interactable">If set to <c>true</c> interactable.</param>
	protected abstract void SetOptionsInteractable (bool interactable);

	void Awake() {
		photonView = GetComponent<PhotonView> ();
		rowGroup = GetComponent<CanvasGroup> ();
	}

	void Start () {
		SetPlayerName ();

		SetSelectOptions ();

		MenuController menuController = GameObject.Find ("MainCanvas").GetComponent<MenuController> ();

		this.listener = menuController;
		menuController.DockLobbyPlayer (gameObject);
	}
	
	private void SetPlayerName() {
		string playerName = photonView.owner.NickName;
		
		if (playerName == "" || playerName == null) {
			playerName = "Player " + photonView.ownerId;
		}
		this.playerName.text = photonView.isMine ? playerName + " (you)" : playerName;

		rowGroup.interactable = readyToggle.enabled = photonView.isMine;
	}

	public void OnReadyChange(bool ready) {
		listener.OnReadyChange ();

		SetOptionsInteractable(!ready);
	}

	/// <summary>
	/// Attaches a player custom property.
	/// Useful for referencing selections made by the player in the lobby in other scenes.
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	protected void SetPlayerProperty(string key, object value) {
		if (!photonView.isMine) {
			Debug.LogWarning ("On Select Change probably shouldn't be getting called from Player Row with non-local photonview!");
		}
		
		PhotonNetwork.player.SetCustomProperties (new ExitGames.Client.Photon.Hashtable () { { key, value } }, null, false);
	}

	#region INetworkCharacter
	public object[] GetSyncVars ()
	{
		object[] baseVars = new object[] {
			readyToggle.isOn
		};

		return (object[])baseVars.Append (GetSubSyncVars ());
	}

	/// <summary>
	/// Apply synced variables recieved from Photon.
	/// </summary>
	/// <param name="syncVars">Sync variables.</param>
	public void PutSyncVars (object[] syncVars)
	{
		readyToggle.isOn = (bool)syncVars [0];

		PutSubSyncVars(syncVars.SubArray (1, GetSubSyncCount()));
	}

	public int GetSyncCount ()
	{
		return 1 + GetSubSyncCount ();
	}
	#endregion


	#region IPlayerRow
	public bool IsReady() {
		return this.readyToggle.isOn;
	}
	#endregion
}
