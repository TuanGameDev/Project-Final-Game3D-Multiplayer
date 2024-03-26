using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInteraction : MonoBehaviour {
	public enum State {
		Move,
		WalkToInteraction,
		ReachInteraction,
		Interact,
		ContinuouslyInteract,
		InterruptInteraction
	}
	private State state = State.Move;
	public State GetState () {
		return state;
	}
	public void SetState (State st) {
		state = st;
	}
	
	[HideInInspector] public Character character;
	public float characterLerpTime = 0.5f;
	[HideInInspector] public InteractiveObject closestInteractiveObject;
	private InteractiveObject currentInteractiveObject;
	[HideInInspector] public List<InteractiveObject> availableInteractiveObjects = new List<InteractiveObject>();
	private bool isSmallTriggerReached = false;
	private bool calculateClosest;
	private bool startLerpCharacterLocationTrigger;
	public LayerMask blockerDetectionLayerMask;
	private InteractiveObject currentInterruptable;

	// Use this for initialization
	void Awake () {
		calculateClosest = true;
		startLerpCharacterLocationTrigger = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (calculateClosest) CalculateClosestInteractiveObject ();
		
		if (startLerpCharacterLocationTrigger) {
			startLerpCharacterLocationTrigger = false;
			StartCoroutine (LerpCharacterLocation (character.animator.transform, currentInteractiveObject.interactionStartCollider.transform, characterLerpTime));
		}
	}

	void CalculateClosestInteractiveObject () {
		float shortestDistance = 0f;
		if (availableInteractiveObjects.Count > 0) {
			for (int i = 0; i < availableInteractiveObjects.Count; i++) {
				if (i == 0) {
					shortestDistance = (transform.position - availableInteractiveObjects[0].transform.position).magnitude;
					closestInteractiveObject = availableInteractiveObjects[0];
				}
				else {
					if ((transform.position - availableInteractiveObjects[i].transform.position).magnitude < shortestDistance) {
						shortestDistance = (transform.position - availableInteractiveObjects[i].transform.position).magnitude;
						closestInteractiveObject = availableInteractiveObjects[i];
					}
				}
			}
		}
		else {
			closestInteractiveObject = null;
			currentInteractiveObject = null;
		}
	}

	public void StartGoToInteraction (InteractiveObject interactiveObject) {
		if (state == State.Move && interactiveObject.GetAvailability ()) {
			if (interactiveObject.InteractionConditionsCheck (character)) {
				currentInteractiveObject = interactiveObject;
				interactiveObject.StartGoToInteraction (character);
				interactiveObject.MoveColliders(character);
				if (isSmallTriggerReached) {
					state = State.ReachInteraction;
					ReachInteraction ();
				}
				else {
					character.canControl = false;
					character.moveTarget.position = interactiveObject.interactionStartCollider.transform.position;
					state = State.WalkToInteraction;
				}
				calculateClosest = false;
				availableInteractiveObjects.Clear ();
				closestInteractiveObject = null;
			}
			else character.gui.guiAudio.PlayOneShot (character.gui.failSound);
		}
	}

	void OnTriggerStay (Collider other) {
		if (state == State.Move) {
			if (other.gameObject.GetComponent<InteractiveObject>()) {
				InteractiveObject interactiveObject = other.gameObject.GetComponent<InteractiveObject>();
				if (Physics.Linecast (transform.position, interactiveObject.transform.position, blockerDetectionLayerMask)) {
					if (availableInteractiveObjects.Contains(interactiveObject)) {
						availableInteractiveObjects.Remove (interactiveObject);
					}
				}
				else {
					if (!availableInteractiveObjects.Contains(interactiveObject)) {
						availableInteractiveObjects.Add (interactiveObject);
					}
				}
			}
		}

		if (currentInteractiveObject) {
			if (other == currentInteractiveObject.interactionStartCollider) {
				if (state == State.WalkToInteraction) {
					state = State.ReachInteraction;
					ReachInteraction ();
				}
				isSmallTriggerReached = true;
			}
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.gameObject.GetComponent<InteractiveObject>()) {
			availableInteractiveObjects.Remove (other.gameObject.GetComponent<InteractiveObject>());
		}
		if (currentInteractiveObject) {
			if (other == currentInteractiveObject.interactionStartCollider) isSmallTriggerReached = false;
		}
	}

	IEnumerator LerpCharacterLocation (Transform character, Transform target, float time) {
		float i = 0.0f;
		float rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			character.position = Vector3.Lerp (character.position, target.position, i);
			character.rotation = Quaternion.Lerp (character.rotation, target.rotation, i);
			yield return null;
		}
	}

	void ReachInteraction () {
		if (currentInteractiveObject) {
			character.ik.SetLookAtWeight(0f);

			character.followPhysics = false;
			character.GetComponent<Rigidbody>().isKinematic = true;
			character.GetComponent<Collider>().isTrigger = true;

			character.transform.position = currentInteractiveObject.interactionStartCollider.transform.position;
			character.transform.rotation = currentInteractiveObject.interactionStartCollider.transform.rotation;
			startLerpCharacterLocationTrigger = true;
			
			character.animator.SetTrigger (Animator.StringToHash (currentInteractiveObject.animatorTriggerStart));
			currentInteractiveObject.ReachInteraction (character);
		}
		character.StopCharacter ();
	}
	
	public void StartInteraction () {
		if (state == State.ReachInteraction) {
			if (currentInteractiveObject.isInterruptable) {
				state = State.ContinuouslyInteract;
				currentInterruptable = currentInteractiveObject;
			}
			else state = State.Interact;

			currentInteractiveObject.StartInteraction (character);
		}
	}
	
	public void InterruptInteraction () {
		if (state == State.ContinuouslyInteract) {
			currentInterruptable.InterruptInteraction (character);
		}
		else if (state == State.WalkToInteraction) {
			InterruptGoToInteraction ();
			currentInteractiveObject.SetAvailability (true);
		}
	}

	private void InterruptGoToInteraction () {
		character.StopCharacter ();
		
		character.canControl = true;
		character.followPhysics = true;
        character.ik.SetLookAtWeight(1f);
		
		character.GetComponent<Rigidbody>().isKinematic = false;
		character.GetComponent<Collider>().isTrigger = false;
		character.originalParent.parent = null;
		
		character.transform.position = character.animator.transform.position;
		character.transform.rotation = character.animator.transform.rotation;
		
		character.horizontalForward = character.animator.transform.forward;
		character.transform.forward = character.animator.transform.forward;
		character.horizontalForward.y = 0f;

		calculateClosest = true;
		state = State.Move;
	}

	public void RetrieveControlToCharacter () {
		if (state == State.Interact || state == State.InterruptInteraction) {
			state = State.Move;
			if (currentInteractiveObject) {
				calculateClosest = true;
				if (currentInteractiveObject.isPickable) availableInteractiveObjects.Remove (currentInteractiveObject);
				currentInteractiveObject.RetrieveControlToCharacter (character);

				character.canControl = true;
				character.followPhysics = true;
                character.ik.SetLookAtWeight(1f);

				character.GetComponent<Rigidbody>().isKinematic = false;
				character.GetComponent<Collider>().isTrigger = false;
				character.originalParent.parent = null;

				character.transform.position = character.animator.transform.position;
				character.transform.rotation = character.animator.transform.rotation;

				isSmallTriggerReached = false;
				character.horizontalForward = character.animator.transform.forward;
				character.transform.forward = character.animator.transform.forward;
				character.horizontalForward.y = 0f;
			}
		}
	}

	public void PlayInteractionAudio () {
		if (currentInteractiveObject) currentInteractiveObject.PlayInteractionAudio (character);
	}
}
