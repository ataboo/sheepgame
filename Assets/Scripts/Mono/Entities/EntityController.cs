using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(NetworkCharacter))]
public abstract class EntityController: MonoBehaviour, INetworkCharacter {
	protected EntityRegistry registry;
	protected Spooker spooker;
	protected bool isMasterClient = false;
	protected string uid;
	public int teamId;

	public const int BASE_SYNC_COUNT = 2;

	public abstract string UID_Class { get; }

	public string UID {
		get {
			return uid;
		}
	}

	public int TeamId {
		get {
			return teamId;
		}
	}

	public bool HasSpooker {
		get {
			return spooker != null;
		}
	}

	public bool SpookerActive {
		get {
			return HasSpooker && spooker.Spooking;
		}
	}

	public bool IsMasterClient {
		get {
			return isMasterClient;
		}
	}

	public abstract int RespawnTime { get; }

	public abstract EntitySpawnPoint.EntityType SpawnType { get; }

	protected abstract object[] GetEntitySyncVars();

	protected abstract void PutEntitySyncVars(object[] syncVars);

	protected abstract int GetEntitySyncCount ();

	protected abstract void EntityAwake();

	protected abstract void EntityStart();

	protected abstract void EntityUpdate();

	protected abstract void EntityInit ();

	public override bool Equals (object obj) {
		if (!(obj is EntityController)) {
			return false;
		}

		return uid == ((EntityController)obj).UID;
	}

	public override int GetHashCode () {
		return uid.GetHashCode();
	}

	public void Awake() {
		registry = GameObject.FindGameObjectWithTag ("GameLogic").GetComponent<EntityRegistry> ();
		spooker = GetComponent<Spooker> ();
		isMasterClient = GetComponent<PhotonView>().isMine;


		EntityAwake ();
	}

	// Use this for initialization
	public void Start () {
		EntityStart ();
	}
	
	// Update is called once per frame
	public void Update () {
		if (this.uid == null) {
			Debug.Log ("Entity named " + gameObject.name + " don't have no UID yet... What's the deal?");
			return;
		}
		EntityUpdate ();
	}

	[PunRPC]
	public void Initialize(int teamId) {
		this.teamId = teamId;

		if (registry == null) {
			Debug.LogError ("EntityController for " + gameObject.name + " couldn't find a EntityRegistry");
			return;
		}

		this.uid = registry.CheckIn (this);

		EntityInit ();
	}
	public virtual void OnDestroy() {
		registry.CheckOut (this);
	}


	public object[] GetSyncVars ()
	{
		object[] baseVars = new object[] {
			uid,
			teamId
		};

		return baseVars.Appended (GetEntitySyncVars ());
	}

	public void PutSyncVars (object[] syncVars)
	{
		this.uid = (string)syncVars [0];
		this.teamId = (int)syncVars [1];

		PutEntitySyncVars(syncVars.SubArray(BASE_SYNC_COUNT, GetEntitySyncCount()));
	}

	public int GetSyncCount ()
	{
		return GetEntitySyncCount () + BASE_SYNC_COUNT;
	}

	public virtual bool IsSpooky (EntityController target)
	{
		return target.HasSpooker;
	}

	public virtual bool IsFriend (EntityController target)
	{
		return !target.HasSpooker && TeamId == target.TeamId;
	}
}
