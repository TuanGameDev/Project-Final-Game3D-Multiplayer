using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
	
	[HideInInspector] public Character character;
	public float maxVolume; // cubic meter
	[HideInInspector] public float occupiedVolume;
	[HideInInspector] public int selectedItemVisualIndex;
	[HideInInspector] public Item selectedItem;
	[HideInInspector] public List<Item> items;

	//Items by categories
	[HideInInspector] public List<Item> consumableItems;

	//Items by type
	[HideInInspector] public List<Item> apples;
	[HideInInspector] public List<Item> bottles;
	[HideInInspector] public List<Item> flints;
	[HideInInspector] public List<Item> twigs;
	[HideInInspector] public List<Item> axes;
	[HideInInspector] public List<Item> picks;

	public List<Item> craftItems;

	public void UseSelectedItem () {
		selectedItem.amountLeft -= 0.2f;
		if (selectedItem.amountLeft < 0.01f) {
			RemoveItem (selectedItem);
			if (consumableItems.Count > 0) {
				selectedItem = consumableItems [0];
				selectedItemVisualIndex = selectedItem.visualIndex;
			}
			else selectedItem = null;
		}
	}

	public void AddItem (Item item) {
		occupiedVolume += item.volume;
		items.Add (item);
		item.AddToInventory (this);
		if (consumableItems.Count < 2 && consumableItems.Count > 0) {
			selectedItem = consumableItems[0];
			selectedItemVisualIndex = selectedItem.visualIndex;
		}
	}

	public void RemoveItem (Item item) {
		occupiedVolume -= item.volume;
		items.Remove (item);
		item.RemoveFromInventory (this);
	}
}
