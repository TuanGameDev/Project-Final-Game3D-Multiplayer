using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandagePickUp : MonoBehaviourPun
{
    PlayerController activeItems;
    public bool _canPickup = true;
    public int value;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canPickup)
        {
            photonView.RPC("PickUpItems", RpcTarget.All);
        }
    }

    [PunRPC]
    void PickUpItems(PhotonMessageInfo info)
    {
        if (activeItems != null)
        {
            activeItems.pickupText.gameObject.SetActive(false);
            if (info.photonView.IsMine)
            {
                activeItems.photonView.RPC("GetBandage", RpcTarget.All, value);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        activeItems = other.gameObject.GetComponent<PlayerController>();
        if (activeItems)
        {
            _canPickup = true;
            activeItems.pickupText.gameObject.SetActive(true);
            activeItems.pickupText.text = " Press F to pick up: Bandage";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activeItems = other.gameObject.GetComponent<PlayerController>();
        if (activeItems)
        {
            _canPickup = false;
            activeItems.pickupText.gameObject.SetActive(false);
        }
    }
}