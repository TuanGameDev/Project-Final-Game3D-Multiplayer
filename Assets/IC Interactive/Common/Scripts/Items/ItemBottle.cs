using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemBottle : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Bottle";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Reducing thirst";
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.consumableItems.Add (this);
		inventory.bottles.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.consumableItems.Remove (this);
		inventory.bottles.Remove (this);
	}
}
