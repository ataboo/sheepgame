using UnityEngine;
using UnityEngine.Networking;

public interface EatingListener {
	void JustAte (EntityController entity);
}
	
public class SheepController : EntityController {
	NavMeshAgent navAgent;

	public SheepState sheepState = SheepState.Idle;

	private Rigidbody rb;

	private Vector3 correctPosition;
	private Quaternion correctRotation;
	private SheepBehavior sheepBehavior;
	private Vector3 navDestination;

	public Vector3 NavDestination {
		get {
			return navDestination;
		}
	}

	public override string UID_Class {
		get {
			return "SH";
		}
	}

	public override int RespawnTime {
		get {
			return -1;
		}
	}

	public override EntitySpawnPoint.EntityType SpawnType {
		get {
			return EntitySpawnPoint.EntityType.Sheep;
		}
	}


	protected override void EntityAwake ()
	{
		rb = GetComponent<Rigidbody> ();
		navAgent = GetComponent<NavMeshAgent> ();
		sheepBehavior = GetComponent<SheepBehavior> ();
	}

	protected override void EntityStart() {
		
	}

	protected override void EntityInit() {
		if (!isMasterClient) {
			throw new UnityException ("SheepController init should only be called on the master client");
		}

		navAgent.enabled = isMasterClient;
	}


	protected override void EntityUpdate ()
	{
		if (isMasterClient) {
			UpdateMovement ();
		} else {
			LerpRemoteTransform ();
		}
	}

	private void UpdateMovement() {
		sheepBehavior.Navigate();
		this.navDestination = navAgent.destination;
	}

	private void LerpRemoteTransform()
	{
		if (correctPosition == null) {
			return;
		}

		transform.position = Vector3.Lerp(transform.position, correctPosition, Time.deltaTime * 5);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctRotation, Time.deltaTime * 5);
	}

	protected override object[] GetEntitySyncVars() {
		object[] syncVars = {
			transform.position, 
			transform.rotation, 
			(int)sheepState,
			navDestination
		};

		return syncVars;
	}

	protected override void PutEntitySyncVars(object[] serialized) {
		correctPosition = (Vector3) serialized [0];
		correctRotation = (Quaternion) serialized [1];
		sheepState = (SheepState)serialized [2];
		navDestination = (Vector3)serialized [3];
	}

	protected override int GetEntitySyncCount() {
		return 4;
	}

	//TODO: onMasterClientChange... navtarget...
}
