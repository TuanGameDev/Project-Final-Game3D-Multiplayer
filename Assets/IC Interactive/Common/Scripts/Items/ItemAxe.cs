using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemAxe : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Axe";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Tool for cutting down trees. Rrequires: 1xTwig, 1xFlint";
	}

	public override bool canCraft (Character character) {
		if (character.inventory.flints.Count > 0 && character.inventory.twigs.Count > 0) {
			character.inventory.RemoveItem (character.inventory.flints[0]);
			character.inventory.RemoveItem (character.inventory.twigs[0]);
			return true;
		}
		else return false;
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.axes.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.axes.Remove (this);
	}

	public override bool CheckAvailability (Inventory inventory) {
		base.CheckAvailability (inventory);
		if (inventory.axes.Count > 0) {
			return true;
		}
		else return false;
	}
}
