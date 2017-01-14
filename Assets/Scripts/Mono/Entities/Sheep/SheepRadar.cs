using UnityEngine;
using System.Collections.Generic;

public class SheepRadar : EntityRadar {
	private List<Component> grassControllers = new List<Component>();

	public GrassController ClosestGrass {
		get{
			float rangeSqr = 0;
			return ClosestGrassController (out rangeSqr);
		}
	}

	public GrassController ClosestGrassController(out float rangeSqr) {
		if (grassControllers.Count == 0) {
			rangeSqr = float.MaxValue;
			return null;
		}

		return (GrassController)GetClosest (grassControllers, out rangeSqr);
	}
		
	protected override void OnRadarEntered (Collider collider)
	{
		GrassController grassController = collider.gameObject.GetComponent<GrassController>();

		if (grassController != null) {
			grassControllers.Add (grassController);
		}
	}

	protected override void OnRadarExited (Collider collider)
	{
		GrassController grassController = collider.gameObject.GetComponent<GrassController>();

		if (grassController != null) {
			grassControllers.Remove (grassController);
		}
	}
}
