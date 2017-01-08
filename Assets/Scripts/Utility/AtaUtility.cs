using UnityEngine;
using System;
using System.Collections;

public static class AtaUtility {
	public static float GetFlatBearing(GameObject baseObject, GameObject target) {
		Vector3 deltaPos = target.transform.position - baseObject.transform.position;

		Vector2 heading = new Vector2(baseObject.transform.forward.x, baseObject.transform.forward.z);
		Vector2 targBearing = new Vector2(deltaPos.x, deltaPos.z);

		return AngleSigned(targBearing, heading);
	}

	public static float AngleSigned(Vector2 targBearing, Vector2 heading) {

		float angle = Mathf.Atan2(-targBearing.y * heading.x + targBearing.x * heading.y, targBearing.x * heading.x + targBearing.y * heading.y);

		return Mathf.Rad2Deg * angle;
	}

	public static string SecsToMinSec(float seconds) {
		int intSecs = Mathf.RoundToInt(seconds);

		return string.Format("{0}:{1}", intSecs / 60, intSecs % 60);
	}

	public static Vector3 RandPointInRadius(Vector3 basePosition, float minRadius, float maxRadius) {
		Vector2 randPoint = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(minRadius, maxRadius);

		return new Vector3(randPoint.x + basePosition.x, basePosition.y, randPoint.y + basePosition.z);
	}

	public static Vector2 RandInBounds(Vector2 firstBound, Vector2 secondBound) {
		return new Vector2 (RandInRange (firstBound.x, secondBound.x), RandInRange (firstBound.y, secondBound.y));
	}

	public static float RandInRange(float firstFloat, float secondFloat) {
		return UnityEngine.Random.Range (Math.Min (firstFloat, secondFloat), Math.Max (firstFloat, secondFloat));
	}

	/// <summary>
	/// Generates a random point within a 3d collider in 2d aligned to the z origin.
	/// Border factory pads the generated point from the outside edge.
	/// </summary>
	/// <returns>The Random point within the collider.</returns>
	/// <param name="collider">Collider.</param>
	/// <param name="borderPadding">Factor of collider demension to pad against outter boundary.</param>
	/// <exception cref="UnityException">Thrown on unsupported collider type.</exception>
	public static Vector3 RandPointInCollider(Collider collider, float padFactor = 0.1f) {
		//TODO: incorporate terrain check.

		if (collider is SphereCollider) {
			SphereCollider sphereCollider = (SphereCollider)collider;
			float colliderRadius = sphereCollider.radius;
			return AtaUtility.RandPointInRadius (sphereCollider.transform.position + sphereCollider.center, colliderRadius * padFactor, colliderRadius * (1f - padFactor));
		}

		if (collider is BoxCollider) {
			BoxCollider boxCollider = (BoxCollider)collider;
			Vector3 worldCenter = boxCollider.transform.position + boxCollider.center;

			// Padding and half the length dimension.
			float factorFromCenter = (1.0f - padFactor) / 2.0f;

			Vector2 lowerLeft = new Vector2 (-boxCollider.size.x + worldCenter.x, -boxCollider.size.z + worldCenter.z) * factorFromCenter;
			Vector2 upperRight = new Vector2 (boxCollider.size.x + worldCenter.x, boxCollider.size.z + worldCenter.z) * factorFromCenter;

			Vector2 randPoint = RandInBounds (lowerLeft, upperRight);

			return new Vector3 (randPoint.x, worldCenter.y, randPoint.y);
		}

		throw new UnityException ("RandPointInCollider only supports 3d Box and Sphere colliders.");
	}
}
