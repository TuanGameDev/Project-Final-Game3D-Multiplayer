using UnityEngine;
using System.Collections;

public class BodyPartAlive : MonoBehaviour {

	public float damage;
	public CharacterRagdollController ragdoll;
	[HideInInspector] public Vector3 velocity;
	private Vector3 newpos;
	private Vector3 oldpos;
	
	void FixedUpdate () {
		newpos = transform.position;
		Vector3 media = (newpos - oldpos);
		velocity = media /Time.deltaTime;
		oldpos = newpos;
	}
}
