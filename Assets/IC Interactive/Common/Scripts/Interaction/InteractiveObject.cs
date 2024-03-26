using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

	public Collider interactionStartCollider;
	public string animatorTriggerStart;

	public string description;
	public Transform GUIPosition;
	public bool isInterruptable;
	public bool isPickable;
	protected bool isAvailable = true;
	public bool GetAvailability () {
		return isAvailable;
	}
	public void SetAvailability (bool av) {
		isAvailable = av;
	}

	public virtual bool InteractionConditionsCheck (Character character) {
		return true;
	}
	public virtual void PlayInteractionAudio (Character character) {}
	public virtual void MoveColliders (Character character) {}

	public virtual void StartGoToInteraction (Character character) {
		isAvailable = false;
	}
	public virtual void ReachInteraction (Character character) {}
	public virtual void StartInteraction (Character character) {}
	public virtual void InterruptInteraction (Character character) {
		isAvailable = true;
	}
	public virtual void RetrieveControlToCharacter (Character character) {
		isAvailable = true;
	}
	public virtual void DropItem (Character character) {
		isAvailable = true;
	}
}
