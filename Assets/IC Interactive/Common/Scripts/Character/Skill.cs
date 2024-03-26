using UnityEngine;
using System.Collections.Generic;

public class Skill : MonoBehaviour {
    [HideInInspector]
    public Character character;
    [HideInInspector]
    public SkillFirearmsVariables skillFirearmsVariables;

    public virtual void SetShootTrigger(bool value) { }
    public virtual bool GetShootTrigger() { return false; }
    public virtual bool GetIsAim()
    {
        return false;
    }
    public virtual void SetIsAim(bool value) { }
    public virtual bool GetIsArmed()
    {
        return false;
    }
    public virtual void SetIsShootHold(bool value) { }
    public virtual bool GetIsReloadWeapon()
    {
        return false;
    }

    public virtual void NextFireMode() { }
    public virtual void ReloadClipOut() { }

    public virtual void ReloadClipIn() { }

    public virtual void ReloadBreechMechanism() { }

    public virtual void PlayMeleeSound() { }

    public virtual void PickUpFirearm(GameObject pickedUpFirearm) { }

    public virtual void DropItemInHands() { }
    public virtual int GetMagazinesCount(int firearmIndex) { return 0; }
    public virtual bool MagazinePickUpCheck() { return false; }

    public virtual void ReloadWeapon() { }

    public virtual void ReloadFinish() { }

    public virtual void EngageArmPose() { }
    public virtual void WeaponSlot(int index) { }

    public virtual void Disarm() { }

    public virtual void StartGoToInteraction() { }

    public virtual bool CheckIfCanStartGoToInteraction() { return true; }

    public virtual void TakeNext() { }

    public virtual void ReloadClipDraw() { }

    public virtual void UnholsterWeapon(int index) { }

    public virtual void HolsterWeapon(int index) { }

    public virtual Transform GetRightHandAim()
    {
        return null;
    }
    public virtual Transform GetLeftHandAim()
    {
        return null;
    }

    public virtual void AddMagazine(string magazineFirearmName) { }
    public virtual int GetWeaponIndex() { return 0; }
    public virtual int FindWeaponIndex(string name) { return 0; }
    public virtual void SetCharacterToFirearm(GameObject firearmGO, int weaponIndex) { }
    public virtual Transform GetCurrentFirearmLeftHandArmed() { return null; }
    public virtual bool GetIsShootHold() { return false; }
    public virtual void Shoot() { }
}
