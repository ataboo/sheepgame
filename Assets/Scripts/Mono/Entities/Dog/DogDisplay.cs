using UnityEngine;
using System.Collections;

public interface IDogDisplay {
	bool IsBarking();
	float GetVelocity();
}

public class DogDisplay: MonoBehaviour {
	private Animator animator;
	private IDogDisplay iDogDisplay;

	public void Awake() {
		this.animator = GetComponent<Animator> ();
		this.iDogDisplay = GetComponent<IDogDisplay> ();
	}

	public void Update() {
		animator.SetFloat ("speed", iDogDisplay.GetVelocity());

		animator.SetBool("barking", iDogDisplay.IsBarking());
	}
}
