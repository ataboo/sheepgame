using UnityEngine;
using UnityEngine.Networking;


public class SheepController : NetworkToggleable {
	public enum SheepState
	{
		Idle,
		Panicking,
		Herding,
		Wandering,
		Eating,
		Dead,
		Ballistic,
		Recovering
	}
	NavMeshAgent navAgent;

	public float maxFriendRange = 100f;
	public float minFriendRange = 4f;
	public float maxEnemyRange = 30f;
	public SheepState sheepState = SheepState.Idle;

	public float walkingSpeed = 5;
	public float runningSpeed = 15;

	public float maxLaunchForce = 100f;
	public float launchDistance = 3f;

	public Vector2 wanderTimeRange = new Vector2(2.0f, 4.0f);

	public Vector2 eatTimeRange = new Vector2(3.0f, 4.0f);

	private float actionTimeout = 0f;

	private Rigidbody rb;
	private Renderer rendComponent;
	private bool paused = false;


	void Start() {

		navAgent = GetComponent <NavMeshAgent>();
		rb = GetComponent<Rigidbody>();
		rendComponent = GetComponentInChildren<Renderer> ();
	}

	[Server]
	void Update () {
		UpdateMovement();

		ColorizeBehavior();
	}

	[Server]
	void OnPause() {
		this.paused = true;
	}

	[Server]
	void OnResume() {
		this.paused = false;
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
			case SheepState.Eating:
				color = Color.green;
				break;
			default:
				color = Color.black;
				break;
		}

		rendComponent.material.color = color;
	}

	private void UpdateMovement() {
		if (sheepState != SheepState.Dead) {
			Navigate();
		}

	}
	private void Navigate() {
		if (sheepState == SheepState.Ballistic) {
			if (ShouldRecover()) {
				sheepState = SheepState.Recovering;
			} else {
				return;
			}
		}

		if (sheepState == SheepState.Recovering) {
			if (!Recover()) {
				return;
			}
		}

		bool friend;
		float magsqr;
		GameObject closestEntity = ClosestEntity(out friend, out magsqr);

		if (closestEntity != null && !friend) {
			RunAway(closestEntity);
		} else {
			KillTime(closestEntity, magsqr);
		}
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
		sheepState = SheepState.Panicking;
		navAgent.speed = runningSpeed;

		actionTimeout = Time.time + Random.Range(0.5f, 1.5f);

		Vector3 runPos = (transform.position - enemy.transform.position).normalized * 10f + transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(runPos, out hit, 10f, 1);

		navAgent.SetDestination(hit.position);
	}

	public void Launch (float forcePower, Vector3 explosionPos, float forceRadius, float depth) {
			navAgent.updatePosition = false;
			navAgent.updateRotation = false;
			navAgent.enabled = false;
			rb.constraints = RigidbodyConstraints.None;

			sheepState = SheepState.Ballistic;

			actionTimeout = Time.time + 0.1f;

			rb.AddExplosionForce(forcePower, explosionPos, forceRadius, depth);	
	}
	private bool ShouldRecover() {
		return rb.velocity.sqrMagnitude < 4f;
	}

	private bool Recover() {
		Quaternion homeRotation = Quaternion.Euler(0, 0, 0);

		if (Quaternion.Angle(rb.rotation, homeRotation) < 5f) {
			rb.rotation = homeRotation;
			rb.constraints = RigidbodyConstraints.FreezeRotation;
			navAgent.updatePosition = true;
			navAgent.updateRotation = true;
			navAgent.enabled = true;
			sheepState = SheepState.Idle;
			return true;
		}

		rb.rotation = Quaternion.RotateTowards(rb.rotation, homeRotation, 100f * Time.deltaTime);
		//rb.velocity = new Vector3(0, 0, 0);
			
		return false;
	}

	private void KillTime(GameObject nearestFriend, float rangeSqr) {
		if (Time.time > actionTimeout) {
			navAgent.speed = walkingSpeed;
			if (nearestFriend != null && rangeSqr > minFriendRange * minFriendRange) {
				Herd(nearestFriend);
				return;
			}

			if (ShouldEat()) {
				Eat();
			} else {
				Wander();
			}
		}
	}

	private void Eat() {
		actionTimeout = Time.time + Random.Range(eatTimeRange.x, eatTimeRange.y);
		navAgent.SetDestination(transform.position);
		sheepState = SheepState.Eating;
	}

	private void Wander() {
		actionTimeout = Time.time + Random.Range(wanderTimeRange.x, wanderTimeRange.y);
		navAgent.SetDestination(getWanderPos());
		sheepState = SheepState.Wandering;
	}

	private bool ShouldEat() {
		//TODO Grass.

		int roll = Random.Range(1, 10);

		return roll > 7;
	}

	private void Herd(GameObject friend) {
		navAgent.speed = walkingSpeed;
		sheepState = SheepState.Herding;

		actionTimeout = Time.time + 0.2f;
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
