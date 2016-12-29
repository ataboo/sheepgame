using UnityEngine;
using System.Collections;


public interface ISheepDisplay {
	SheepState GetSheepState();
}

public class SheepDisplay : MonoBehaviour {
	public SheepState sheepState = SheepState.Idle;

	private Renderer rendComp;
	private ISheepDisplay iSheepDisplay;

	public void Start() {
		rendComp = GetComponentInChildren<Renderer> ();
		iSheepDisplay = GetComponent<ISheepDisplay> ();
	}

	// Update is called once per frame
	public void Update () {
		ColorizeBehavior ();
	}

	private void ColorizeBehavior() {
		Color color;

		sheepState = iSheepDisplay.GetSheepState ();

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
