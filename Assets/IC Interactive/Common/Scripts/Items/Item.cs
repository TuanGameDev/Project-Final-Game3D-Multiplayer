using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    public float amountLeft = 1f;
    public int visualIndex;
    public float volume; // cubic meter
                         //public string description;
                         //public string ditailedDescription;
    public virtual string GetDescription()
    {
        return "";
    }
    public virtual string GetDitailedDescription()
    {
        return "";
    }

    public virtual bool canCraft(Character character)
    {
        return false;
    }

    public virtual void AddToInventory(Inventory inventory) { }
    public virtual void RemoveFromInventory(Inventory inventory) { }
    public virtual bool CheckAvailability(Inventory inventory)
    {
        return true;
    }
}
