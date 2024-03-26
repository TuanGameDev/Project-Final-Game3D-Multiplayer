using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemApple : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Apple";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Reducing hunger";
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.consumableItems.Add (this);
		inventory.apples.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.consumableItems.Remove (this);
		inventory.apples.Remove (this);
	}
}
