using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterRagdollController : MonoBehaviour {

	[HideInInspector] public Character character;
	
	public List<BodyPartAlive> bodyPartsAlive;
	[HideInInspector] public bool isAlive;
	[HideInInspector] public bool killTrigger;
	[HideInInspector] public Vector3 velocity;
	private Vector3 newpos;
	private Vector3 oldpos;
	public float health = 100f;
	public CameraOrbitCenterInfo deadCameraOrbitCenter;
	public BodyPartAlive shootTargetBodyPart;

	// Use this for initialization
	void Awake () {
		isAlive = true;
		killTrigger = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (killTrigger) {
			killTrigger = false;
			if (character) {
				character.weaponsController.DropItemInHands ();
				character.input.isAim = false;
				character.cameraController.isZoom = false;

				Destroy (character.animator);
				Destroy (character.animatorMovement);
				Destroy (character);
				Destroy (character.input);
				Destroy (character.weaponsController);
				Destroy (character.inventory);
				Destroy (character.GetComponent<Collider>());
				Destroy (character.GetComponent<Rigidbody>());
			}
			isAlive = false;
			for (int i = 0; i < bodyPartsAlive.Count; i++) {
				bodyPartsAlive[i].GetComponent<Collider>().isTrigger = false;
				bodyPartsAlive[i].GetComponent<Rigidbody>().isKinematic = false;
				bodyPartsAlive[i].GetComponent<Rigidbody>().velocity = bodyPartsAlive[i].velocity;
			}
			
			character.cameraController.ChangeOrbitCenter (deadCameraOrbitCenter);
		}

		if ((health < 0f) && isAlive) {
			killTrigger = true;
		}

		newpos = transform.position;
		Vector3 media = (newpos - oldpos);
		velocity = media /Time.deltaTime;
		oldpos = newpos;
	}
}
