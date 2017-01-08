﻿using UnityEngine;
using System.Collections;

public enum SheepState
{
	Idle,
	Panicking,
	Herding,
	Wandering,
	Pausing,
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
		sheepRadar = GetComponent<SheepRadar> ();
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

		navAgent.SetDestination(hit.position);
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
		navAgent.SetDestination(friend.transform.position);
	}
	#endregion

	#region Eating
	private GrassController WantsToEat() {
		//TODO: range stuff?
		
		float range = float.MaxValue;
		return sheepRadar.ClosestGrassController (out range);
	}

	private bool ManageEating(GrassController nearestGrass) {
		if (!nearestGrass.IsInside (gameObject)) {
			MoveIntoGrass ();
		} else {
			if (sheepState == SheepState.Eating) {
				JustAteGrass ();
				WanderInGrass ();
			} else {
				EatGrass ();
			}
		}

		return true;
	}

	private void MoveIntoGrass() {
		sheepState = SheepState.Wandering;
		ActionTime (2f);
	}

	private void WanderInGrass() {
		sheepState = SheepState.Wandering;
		ActionTime (wanderTimeRange.x, wanderTimeRange.y);
		
		if (grasstination == null || navAgent.remainingDistance < 0.1) {
			grasstination = sheepRadar.ClosestGrass.GetWanderPoint ();
		}
		
		navAgent.SetDestination (grasstination);
	}

	private void EatGrass() {
		sheepState = SheepState.Eating;
		navAgent.speed = 0f;
		ActionTime (1f, 1.5f);
	}

	private void JustAteGrass() {
		if (foodListener != null) {
			foodListener.JustAte (sheepController);
		}
	}
	#endregion

	#region Wandering
	private bool DeserveABreak() {
		int roll = Random.Range(1, 10);
		
		return roll > 7;
	}
	private void Wander() {
		ActionTime(wanderTimeRange.x, wanderTimeRange.y);
		navAgent.SetDestination(getWanderPos());
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
		navAgent.SetDestination(transform.position);
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
