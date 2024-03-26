using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemTwig : Item {
	public override string GetDescription () {
		base.GetDescription ();
		return "Twig";
	}
	public override string GetDitailedDescription () {
		base.GetDescription ();
		return "Wooden stick";
	}

	public override void AddToInventory (Inventory inventory) {
		base.AddToInventory (inventory);
		inventory.twigs.Add (this);
	}
	public override void RemoveFromInventory (Inventory inventory) {
		base.RemoveFromInventory (inventory);
		inventory.twigs.Remove (this);
	}
}
