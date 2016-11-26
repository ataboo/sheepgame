using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SheepDisplay : NetworkBehaviour {
	[SyncVar]
	public SheepController.SheepState sheepState = SheepController.SheepState.Idle;

	private Renderer rendComp;


	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (rendComp == null) {
			rendComp = GetComponentInChildren<Renderer> ();

			if (rendComp == null) {
				return;
			}
		}

		ColorizeBehavior ();
	}

	private void ColorizeBehavior() {
		Color color;

		switch (sheepState) {
		case SheepController.SheepState.Wandering:
			color = Color.yellow;
			break;
		case SheepController.SheepState.Herding:
			color = Color.blue;
			break;
		case SheepController.SheepState.Panicking:
			color = Color.red;
			break;
		case SheepController.SheepState.Eating:
			color = Color.green;
			break;
		default:
			color = Color.black;
			break;
		}

		rendComp.material.color = color;
	}
}
