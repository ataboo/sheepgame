using UnityEngine;
using System.Collections;

public class DogDisplay : NetworkToggleable {
	private Animator animator;
	private PlayerControl pc;

	override public void BothStart() {
		this.animator = GetComponent<Animator> ();
		this.pc = GetComponent<PlayerControl> ();
	}

	override public void BothUpdate() {
		DogState dogState = pc.dogState;

		animator.SetFloat ("speed", pc.velocity.magnitude);

		animator.SetBool("barking", dogState == DogState.Spooking);
	}
}
