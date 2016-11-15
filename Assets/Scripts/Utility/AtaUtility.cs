using UnityEngine;

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
}
