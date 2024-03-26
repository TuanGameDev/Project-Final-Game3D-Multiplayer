using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WeaponSettings
{
    public string name;
    public Transform aim;
    public Transform leftHandAim;
    public Transform rightHandAim;
    public Transform leftHandArmed;
    public Transform fromRifleToCamTarget;
    public Transform slotSpine;
    public Transform slotHand;
    public GameObject magazineInHand;
    public int magazinesCount;
    public int slotIndex;
}

public class SkillFirearmsVariables : MonoBehaviour {

    public int magazinesCount;
    public int magazinesMax;
    public List<WeaponSettings> weapons;
    [HideInInspector]
    public string currentFirearmIndicator;
}
