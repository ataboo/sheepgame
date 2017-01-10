using UnityEngine;
using System.Collections;

public enum SheepState
{
	Idle,
	Panicking,
	Herding,
	Wandering,
	Pausing,
	MovingToFood,
	Eating,
	Dead,
	Ballistic,
	Recovering
}

public class SheepBehavior: MonoBehaviour {
	public float minFriendRange = 4f;

	public float walkingSpeed = 5;
	public float runningSpeed = 15;

	public float maxLaunchForce = 100f;
	public float launchDistance = 3f;

	public Vector2 wanderTimeRange = new Vector2(2.0f, 4.0f);
	public Vector2 eatTimeRange = new Vector2(2.0f, 3.0f);

	private NavMeshAgent navAgent;
	private SheepRadar sheepRadar;
	private SheepController sheepController;
	private FoodListener foodListener;

	private Vector3 grasstination;

	private SheepState sheepState {
		get{
			return sheepController.sheepState;
		}
		set {
			sheepController.sheepState = value;
		}
	}

	float actionTimeout;

	public void Awake() {
		navAgent = GetComponent<NavMeshAgent> ();
		sheepRadar = GetComponentInChildren<SheepRadar> ();
		sheepController = GetComponent<SheepController> ();
		foodListener = GameObject.FindGameObjectWithTag ("GameLogic").GetComponent<FoodListener> ();
	}
		
	public void Navigate() {
		//TODO: detect drowning, etc.

		//Spooky stuff
		EntityController spookyEntity = CheckSpooked ();
		if (spookyEntity != null) {
			RunAway (spookyEntity.gameObject);
			return;	
		}

		// Killing Time.
		if (Time.time > actionTimeout) {
			navAgent.speed = walkingSpeed;

			EntityController nearestFriend = IsLonely ();
			if (nearestFriend != null) {
				Herd (nearestFriend.gameObject);
				return;
			}

			GrassController nearestGrass = WantsToEat ();
			if (nearestGrass != null) {
				ManageEating (nearestGrass);
				return;
			}

			if (DeserveABreak ()) {
				TakeBreak ();
				return;
			}

			Wander ();
		}
	}

	#region Spooky
	private EntityController CheckSpooked() {
		//TODO: range stuff?
		float range = float.MaxValue;
		return sheepRadar.ClosestSpooking(out range);
	}
		
	private void RunAway(GameObject enemy) {
		sheepState = SheepState.Panicking;
		navAgent.speed = runningSpeed;

		actionTimeout = Time.time + Random.Range (eatTimeRange.x, eatTimeRange.y);

		Vector3 runPos = (sheepController.transform.position - enemy.transform.position).normalized * Random.Range(10f, 16f) + sheepController.transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(runPos, out hit, 16f, 1);

		SetNavDestination(hit.position);
	}
	#endregion

	#region Herding
	private EntityController IsLonely() {
		float range = float.MaxValue;
		EntityController nearestFriend = sheepRadar.ClosestFriend (out range);

		if (nearestFriend != null && (range > minFriendRange * minFriendRange)) {
			return nearestFriend;
		} 
		return null;	
	}

	private void Herd(GameObject friend) {
		navAgent.speed = walkingSpeed;
		sheepState = SheepState.Herding;

		actionTimeout = Time.time + 0.5f;
		SetNavDestination(friend.transform.position);
	}
	#endregion

	#region Eating
	private GrassController WantsToEat() {
		//TODO: range stuff?
		
		float range = float.MaxValue;
		return sheepRadar.ClosestGrassController (out range);
	}

	private void ManageEating(GrassController nearestGrass) {
		if (!nearestGrass.IsInside (gameObject)) {
			MoveIntoGrass (nearestGrass);
		} else {
			if (sheepState == SheepState.Eating) {
				JustAteGrass ();
				WanderInGrass (nearestGrass);
			} else {
				EatGrass ();
			}
		}
	}

	private void MoveIntoGrass(GrassController grassController) {
		sheepState = SheepState.MovingToFood;
		SetNavDestination (grassController.gameObject.transform.position);
		ActionTime (2f);
	}

	private void WanderInGrass(GrassController grassController) {
		sheepState = SheepState.MovingToFood;
		ActionTime (wanderTimeRange.x, wanderTimeRange.y);
		
		if (grasstination == Vector3.zero || navAgent.remainingDistance < 0.1) {
			grasstination = grassController.GetWanderPoint ();
		}

		Debug.Log ("grasstination: " + grasstination);
		
		SetNavDestination (grasstination);
	}

	private void EatGrass() {
		sheepState = SheepState.Eating;
		navAgent.speed = 0f;
		ActionTime (1f, 1.5f);
	}

	private void JustAteGrass() {
		if (foodListener != null) {
			foodListener.JustAteGrass (sheepController.TeamId);
		}
	}

	#endregion

	private void SetNavDestination(Vector3 position) {
		navAgent.SetDestination (position);
	}

	#region Wandering
	private bool DeserveABreak() {
		int roll = Random.Range(1, 10);
		
		return roll > 7;
	}
	private void Wander() {
		ActionTime(wanderTimeRange.x, wanderTimeRange.y);
		SetNavDestination(getWanderPos());
		sheepState = SheepState.Wandering;
	}
	
	private Vector3 getWanderPos() {
		Vector3 randomDirection = Random.insideUnitSphere * 30f;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, 30f, 1);
		return hit.position;
	}
	
	private void TakeBreak() {
		ActionTime(eatTimeRange.x, eatTimeRange.y);
		SetNavDestination(transform.position);
		sheepState = SheepState.Idle;
	}
	#endregion
		
	private void ActionTime(float min, float max) {
		ActionTime (Random.Range (min, max));
	}

	private void ActionTime(float delay) {
		actionTimeout = Time.time + delay;
	}
}
