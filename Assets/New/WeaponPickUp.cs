using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] Gun weaponFab;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            Gun newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);
            Destroy(gameObject);
        }
    }
}
