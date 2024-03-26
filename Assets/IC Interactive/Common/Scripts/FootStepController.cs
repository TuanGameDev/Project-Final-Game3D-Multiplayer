using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootStepController : MonoBehaviour {
	public List<AudioClip> dirtSounds;
	private AudioSource legs;
	public Animator animator;
	private float currentFootstepCurve;
	private float previousFootstepCurve;

	void Awake () {
		legs = GetComponent<AudioSource>();
	}

	void PlayFootStepAudio () {
		int randomClip = Random.Range (0, dirtSounds.Count);
		legs.PlayOneShot (dirtSounds[randomClip]);
	}

	void Update () {
		if(animator)currentFootstepCurve = animator.GetFloat ("FootstepCurve");
		if (currentFootstepCurve * previousFootstepCurve < 0) {
			PlayFootStepAudio ();
		}
		previousFootstepCurve = currentFootstepCurve;
	}
}
