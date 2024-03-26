using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorMovement : MonoBehaviour {

	[HideInInspector] public Character character;
	[HideInInspector] public Rigidbody physicsRigidbody;
	[HideInInspector] public Transform physicsTransform;
	private Animator animator;
	private float rotateSmoothness = 1f;

    // Use this for initialization
    void Awake () {
		animator = GetComponent<Animator>();
    }

    void OnAnimatorMove () {
		if (character.followPhysics) {
			Vector3 velocity = animator.deltaPosition / Time.deltaTime;
			velocity.y = physicsRigidbody.velocity.y;
			physicsRigidbody.velocity = velocity;
		}
		else {
			transform.position += animator.deltaPosition;
			transform.rotation *= animator.deltaRotation;
		}
	}

	public void SetPositionRotation () {
		character.horizontalForward.y = 0f;

		Vector3 axis = Vector3.Cross (physicsTransform.forward.normalized, character.horizontalForward.normalized);
		float angleCharacterCamera = Vector3.Angle (physicsTransform.forward.normalized, character.horizontalForward.normalized) * (axis.y < 0f ? 1f : -1f);
		angleCharacterCamera = Mathf.Clamp (angleCharacterCamera, -30f, 30f);

		if (character.weaponsController.GetIsAim() || !character.canControl) rotateSmoothness = character.rotateSmoothnessAim;
		else rotateSmoothness = (((Vector3.Dot (physicsTransform.forward.normalized, character.horizontalForward.normalized)) + 1.5f)/2.5f);
		
		if (character.weaponsController.GetIsAim() || character.isMove) {
			character.targetTransform.forward = character.horizontalForward;
			character.targetTransform.RotateAround (physicsTransform.position, physicsTransform.forward, angleCharacterCamera * 0.5f * character.input.vertical/5.204679f);
			physicsTransform.rotation = Quaternion.Slerp (physicsTransform.rotation, character.targetTransform.rotation, Time.deltaTime * 3f/rotateSmoothness);
		}
		else {
			character.targetTransform.forward = new Vector3 (physicsTransform.forward.x, 0f, physicsTransform.forward.z);
			physicsTransform.rotation = Quaternion.Slerp (physicsTransform.rotation, character.targetTransform.rotation, Time.deltaTime * 1f/character.rotateSmoothnessAim);
		}
		
		if (character.followPhysics) {
			transform.position = physicsTransform.position;
			transform.rotation = physicsTransform.rotation;
		}
	}
}