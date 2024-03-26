using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatorEvents : MonoBehaviour {

	[HideInInspector] public Character character;

    void StartInteraction () {//Event from animation
		character.interaction.StartInteraction ();
	}
	
	void RetrieveControlToCharacter () {//Event from animation
		character.interaction.RetrieveControlToCharacter ();
	}
	
	void EngageArmPose () {//Event from animation
		character.weaponsController.EngageArmPose ();
	}
	
	void TakeNext () {//Event from animation
		character.weaponsController.TakeNext();
	}
	
	void UnholsterWeapon (int weaponIndex)
    {//Event from animation
        character.weaponsController.UnholsterWeapon(weaponIndex);
    }
	
	void HolsterWeapon (int weaponIndex) {//Event from animation
        character.weaponsController.HolsterWeapon(weaponIndex);
	}
	
	void ReloadFinish () {//Event from animation
        character.weaponsController.ReloadFinish();
	}
	
	void ReloadClipDraw () {//Event from animation
        character.weaponsController.ReloadClipDraw();
	}
	
	void ReloadClipOut () {//Event from animation
		character.weaponsController.ReloadClipOut ();
	}
	
	void ReloadClipIn () {//Event from animation
		character.weaponsController.ReloadClipIn ();
	}
	
	void ReloadBreechMechanism () {//Event from animation
		character.weaponsController.ReloadBreechMechanism ();
	}
	
	void UseItemFinished () {//Event from animation
		character.ik.SetLookAtWeight(1f);
		character.inventory.UseSelectedItem ();
	}
	void ShowItem () {//Event from animation
		character.ItemsInHand[character.inventory.selectedItemVisualIndex].SetActive (true);
	}
	void HideItem () {//Event from animation
		for (int counter = 0; counter < character.ItemsInHand.Count; counter++) {
			character.ItemsInHand[counter].SetActive (false);
		}
	}
	
	void SetMeleeTrue () {//Event from animation
		//character.weaponsController.isMelee = true;
		character.animator.SetBool (Animator.StringToHash ("IsMelee"), true);
	}
	void SetMeleeFalse () {//Event from animation
		//character.weaponsController.isMelee = false;
		character.animator.SetBool (Animator.StringToHash ("IsMelee"), false);
	}
	
	void PlayMeleeSound () {//Event from animation
		character.weaponsController.PlayMeleeSound ();
		/*foreach (Character ch in character.weaponsController.currentFirearm.meleeWeapon.availableCharacters) {
			if (ch != character) {
				ch.ragdoll.health -= character.weaponsController.currentFirearm.meleeWeapon.damage;
			}
		}*/
	}

	void PlayInteractionAudio () {
		character.interaction.PlayInteractionAudio ();
	}
}