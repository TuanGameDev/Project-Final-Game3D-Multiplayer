using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemFlint : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Flint";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Could be used to make tools";
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.flints.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.flints.Remove (this);
	}
}
