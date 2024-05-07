using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponPickUp : MonoBehaviourPun
{
    PhotonView view;
    [SerializeField] Gun weaponFab;
    PlayerController activeWeapon;
    public bool _canPickup = true;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canPickup)
        {
            view.RPC("PickUpWeapon", RpcTarget.All);
        }
    }
    [PunRPC]
    public void PickUpWeapon()
    {
        if (activeWeapon != null)
        {
            Gun newWeapon = Instantiate(weaponFab);
            activeWeapon.EquipWeapon(newWeapon);
            PhotonNetwork.Destroy(gameObject);
            //activeWeapon._anim.SetTrigger("PickUp");
            activeWeapon.pickupText.gameObject.SetActive(false);
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