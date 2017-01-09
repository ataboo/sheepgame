using UnityEngine;
using System.Collections;


public class SheepDisplay : MonoBehaviour {
	private Renderer rendComp;
	private SheepController sheepController;

	public void Start() {
		rendComp = GetComponentInChildren<Renderer> ();
		sheepController = GetComponent<SheepController> ();
	}

	// Update is called once per frame
	public void Update () {
		ColorizeBehavior ();
		DebugNav ();
	}

	private void DebugNav() {
		if (sheepController.NavDestination != null) {
			Debug.DrawLine (transform.position, sheepController.NavDestination, Color.cyan);
		}
	}

	private void ColorizeBehavior() {
		Color color;

		switch (sheepController.sheepState) {
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

		rendComp.material.color = color;
	}
}
