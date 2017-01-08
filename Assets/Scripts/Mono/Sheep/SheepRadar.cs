using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SheepRadar : EntityRadar {
	private List<Component> grassControllers = new List<Component>();

	private GrassController closestGrassController;
	public GrassController ClosestGrass {
		get{
			if (closestGrassController == null) {
				float range = 0;
				ClosestGrassController (out range);
			}
			return closestGrassController;
		}
	}

	public GrassController ClosestGrassController(out float range) {
		if (grassControllers.Count == 0) {
			range = float.MaxValue;
			return null;
		}

		closestGrassController = (GrassController)GetClosest (grassControllers, out range);

		return closestGrassController;
	}
		
	protected override void OnRadarEntered (Collider collider)
	{
		Debug.Log ("Sheepdar Einfart: " + collider.gameObject.name);

		GrassController grassController = collider.gameObject.GetComponent<GrassController>();

		if (grassController != null) {
			grassControllers.Add (grassController);
		}
	}

	protected override void OnRadarExited (Collider collider)
	{
		Debug.Log ("Sheepdar Ausfart: " + collider.gameObject.name);

		GrassController grassController = collider.gameObject.GetComponent<GrassController>();

		if (grassController != null) {
			grassControllers.Remove (grassController);
			if (grassController == closestGrassController) {
				closestGrassController = null;
			}
		}
	}
}
