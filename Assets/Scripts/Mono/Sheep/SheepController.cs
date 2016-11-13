using UnityEngine;
using System.Collections;

public class SheepController : MonoBehaviour {
	public enum SheepState
	{
		Idle,
		Panicking,
		Herding,
		Wandering,
		Dead,
	}
	NavMeshAgent navAgent;

	public float maxFriendRange = 100f;
	public float minFriendRange = 0.5f;
	public float maxEnemyRange = 30f;
	public SheepState sheepState = SheepState.Idle;

	public float walkingSpeed = 5;
	public float runningSpeed = 15;


	void Awake() {
		navAgent = GetComponent <NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement();

		ColorizeBehavior();
	}

	private void ColorizeBehavior() {
		Color color;

		switch (sheepState) {
			case SheepState.Wandering:
				color = Color.yellow;
				break;
			case SheepState.Herding:
				color = Color.blue;
				break;
			case SheepState.Panicking:
				color = Color.red;
				break;
			default:
				color = Color.black;
				break;
		}

		Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, color);
	}

	private void UpdateMovement() {
		if (sheepState != SheepState.Dead) {
			Navigate();
		}

	}
	private void Navigate() {
		bool friend;
		float magsqr;
		GameObject closestEntity = ClosestEntity(out friend, out magsqr);

		if (closestEntity != null) {
			if (!friend) {
				RunAway(closestEntity);
				return;
			}


			if (magsqr > minFriendRange * minFriendRange) {
				Herd(closestEntity);
				return;
			}
		} 

		Wander();
	}

	private GameObject ClosestEntity(out bool friend, out float magsqr) {
		float closestFriendSqr = maxFriendRange * maxFriendRange;
		float closestEnemySqr = maxEnemyRange * maxEnemyRange;

		GameObject closestFriend = null;
		GameObject closestEnemy = null;

		foreach (GameObject entity in GameObject.FindGameObjectsWithTag("entity"))
		{
			if (entity == gameObject) {
				continue;
			}

			if (IsEnemy(entity)) {
				CloserEntity(ref closestEnemy, ref closestEnemySqr, entity);
			} else if (IsFriend(entity))  {
				CloserEntity(ref closestFriend, ref closestFriendSqr, entity);
			}
		}

		if (closestEnemy == null) {
			friend = true;
			magsqr = closestFriendSqr;
			return closestFriend;
		}

		friend = false;
		magsqr = closestEnemySqr;
		return closestEnemy;
	}

	private void CloserEntity(ref GameObject oldClosest, ref float oldMagSqr, GameObject newEntity) {
		float magSqr = (newEntity.transform.position - transform.position).sqrMagnitude;

		if (magSqr < oldMagSqr) {
			oldClosest = newEntity;
			oldMagSqr = magSqr;
		}
	}

	private bool IsFriend(GameObject entity) {
		return entity.GetComponent<SheepController>() != null;
	}

	private bool IsEnemy(GameObject entity) {
		Spooker spooker = entity.GetComponent<Spooker>();

		return spooker != null && spooker.IsActive();
	}

	private void RunAway(GameObject enemy) {
		if (sheepState != SheepState.Panicking) {
			sheepState = SheepState.Panicking;
			navAgent.speed = runningSpeed;
		}

		Vector3 runPos = (transform.position - enemy.transform.position).normalized * 100f + transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(runPos, out hit, 100f, 1);

		navAgent.SetDestination(hit.position);
	}

	private void Wander() {
		if (sheepState !=  SheepState.Wandering || !navAgent.hasPath) {
			navAgent.speed = walkingSpeed;
			navAgent.SetDestination(getWanderPos());
			sheepState = SheepState.Wandering;
		}
	}

	private void Herd(GameObject friend) {
		if (sheepState != SheepState.Herding) {
			navAgent.speed = walkingSpeed;
			sheepState = SheepState.Herding;
		}

		navAgent.SetDestination(friend.transform.position);
	}

	private Vector3 getWanderPos() {
		Vector3 randomDirection = Random.insideUnitSphere * 30f;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(randomDirection, out hit, 30f, 1);
		return hit.position;
	}
}
