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
    public Highlight highlight;
    public GameObject panelPickUp;
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
        if (activeItems && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _canPickup = true;
            highlight.ToggleHighlight(true);
            panelPickUp.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activeItems = other.gameObject.GetComponent<PlayerController>();
        if (activeItems && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _canPickup = false;
            highlight.ToggleHighlight(false);
            panelPickUp.SetActive(false);
        }
    }
}