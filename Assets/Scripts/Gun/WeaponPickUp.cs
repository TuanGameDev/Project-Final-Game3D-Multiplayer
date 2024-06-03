using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponPickUp : MonoBehaviourPun
{
    [SerializeField] Gun weaponFab;
    PlayerController activeWeapon;
    public bool _canPickup = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canPickup)
        {
            photonView.RPC("PickUpWeapon", RpcTarget.All);
            
        }
    }

    [PunRPC]
    void PickUpWeapon(PhotonMessageInfo info)
    {
        if (activeWeapon != null)
        {
            Gun newWeapon = Instantiate(weaponFab);
            //activeWeapon.EquipWeapon(newWeapon);
            //activeWeapon._anim.SetTrigger("PickUp");
            activeWeapon.pickupText.gameObject.SetActive(false);
            if (info.photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            _canPickup = true;
            activeWeapon.pickupText.gameObject.SetActive(true);
            activeWeapon.pickupText.text = " Press F to pick up: " + weaponFab.weaponName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            _canPickup = false;
            activeWeapon.pickupText.gameObject.SetActive(false);
        }
    }
}