using UnityEngine;

public static class AtaUtility {
	public static float GetFlatBearing(GameObject baseObject, GameObject target) {
		Vector3 baseHeading = Vector3.ProjectOnPlane(baseObject.transform.forward, Vector3.up);

		Vector3 deltaPos = target.transform.position - baseObject.transform.position;

		Vector3 bearing = Vector3.ProjectOnPlane(deltaPos, Vector3.up);

		// Debug.Log("Unit" + ((baseHeading.normalized - bearing.normalized) / 2f).ToString());

		// Debug.DrawLine(baseObject.transform.position, baseObject.transform.position + baseObject.transform.up * 100, Color.green);
		// Debug.DrawLine(baseObject.transform.position, target.transform.position, Color.red);
		// //Debug.DrawLine(target.transform.position, target.transform.position + heading, Color.blue);
		// //Debug.DrawLine(baseObject.transform.position, baseObject.transform.position + projection, Color.yellow);

		return AngleSigned((baseHeading.normalized - bearing.normalized) / 2f);
	}

	public static float AngleSigned(Vector3 vector) {
		float bearingRads = Mathf.Atan2(vector.x, -vector.z);

		float degs = 2f *  bearingRads * Mathf.Rad2Deg;
		
		//return degs;
		//return bearingRads;
		return degs > 180 ? -(360 - degs) : degs;
	}
}
