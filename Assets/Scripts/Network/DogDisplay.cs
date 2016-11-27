using UnityEngine;
using System.Collections;

public class DogDisplay : NetworkToggleable {
	private Renderer rendComp;
	private PlayerControl pc;

	override public void BothStart() {
		this.rendComp = GetComponentInChildren<Renderer>();
		this.pc = GetComponent<PlayerControl> ();
	}

	override public void BothUpdate() {
		DogState dogState = pc.dogState;

		Color rendColor = Color.white;

		switch(dogState) {
		case DogState.Normal:
			rendColor = Color.grey;
			break;
		case DogState.Running:
			rendColor = Color.blue;
			break;
		case DogState.Spooking:
			rendColor = Color.red;
			break;
		default:
			rendColor = Color.white;
			break;
		}

		rendComp.material.color = rendColor;
	}
}
