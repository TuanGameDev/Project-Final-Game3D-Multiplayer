using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPick : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Pick";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Tool for mining. Requires: 2xTwig, 2xFlint";
	}

	public override bool canCraft (Character character) {
		if (character.inventory.flints.Count > 1 && character.inventory.twigs.Count > 1) {
			character.inventory.RemoveItem (character.inventory.flints[0]);
			character.inventory.RemoveItem (character.inventory.flints[0]);
			character.inventory.RemoveItem (character.inventory.twigs[0]);
			character.inventory.RemoveItem (character.inventory.twigs[0]);
			return true;
		}
		else return false;
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.picks.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.picks.Remove (this);
	}

	public override bool CheckAvailability (Inventory inventory) {
		base.CheckAvailability (inventory);
		if (inventory.picks.Count > 0) {
			return true;
		}
		else return false;
	}
}
