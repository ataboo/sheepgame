using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class EntityRadar : MonoBehaviour {
	protected List<Component> friendCandidates = new List<Component>();
	protected List<Component> spookCandidates = new List<Component>();
	private EntityController myEntity;

	protected abstract void OnRadarEntered(Collider collider);

	protected abstract void OnRadarExited(Collider collider);

	public void Awake() {
		myEntity = GetComponentInParent<EntityController> ();
	}

	public void Start() {
	}

	public void OnTriggerEnter(Collider collider) {
		EntityController entity = collider.gameObject.GetComponent<EntityController> ();
		if (entity != null) {
			AddEntity (entity);
		}

		OnRadarEntered (collider);
	}

	public void OnTriggerExit(Collider collider) {
		EntityController entity = collider.gameObject.GetComponent<EntityController> ();
		if (entity != null) {
			RemoveEntity (entity);
		}

		OnRadarExited (collider);
	}

	private void AddEntity(EntityController entity) {
		if (myEntity.IsFriend(entity)) {
			friendCandidates.Add (entity);
		}

		if (myEntity.IsSpooky(entity)) {
			spookCandidates.Add (entity);
		}
	}

	private void RemoveEntity(EntityController entity) {
		friendCandidates.Remove (entity);
		spookCandidates.Remove (entity);
	}


	public EntityController ClosestFriend(out float range) {
		return (EntityController)GetClosest (friendCandidates, out range);
	}

	public EntityController ClosestSpooking(out float rangeSqr) {
		return (EntityController)GetClosest (spookCandidates, out rangeSqr, (entity) => { return ((EntityController)entity).SpookerActive; });
	}

	public delegate bool ComponentCondition(Component entity);

	public Component GetClosest(IEnumerable<Component> components, out float rangeSqr, ComponentCondition condition = null) {
		Component closest = null;
		rangeSqr = float.MaxValue;

		foreach (Component component in components) {
			if (condition != null && !condition (component)) {
				continue;
			}

			float componentRange = DistanceSqr (component);
			if (closest == null || componentRange < rangeSqr) {
				closest = component;
				rangeSqr = componentRange;
			}
		}

		return closest;
	}

	private float DistanceSqr(Component monoBehavior) {
		return (monoBehavior.transform.position - transform.position).sqrMagnitude;
	}
}
