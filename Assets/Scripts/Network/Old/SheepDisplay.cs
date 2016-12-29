using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public interface ISheepDisplay {
	void SetState(SheepState sheepState);
}

public class SheepDisplay : NetworkToggleable, ISheepDisplay {
	public SheepState sheepState = SheepState.Idle;

	private Renderer rendComp;

	public override void BothAwake() {
		rendComp = GetComponentInChildren<Renderer> ();
	}

	// Update is called once per frame
	public override void BothUpdate () {
		ColorizeBehavior ();
	}

	//================== ISheepDisplay =================
	public void SetState(SheepState sheepState) {
		this.sheepState = sheepState;
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

		rendComp.material.color = color;
	}
}
