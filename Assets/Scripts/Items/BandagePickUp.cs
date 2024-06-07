using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandagePickUp : MonoBehaviourPun
{
    public bool _canPickup = true;
    public int value;
    public Highlight highlight;
    public GameObject panelPickUp;

    public void PickUpBandage(PlayerController player)
    {
        photonView.RPC("PickUpItems", RpcTarget.All, player.photonView.ViewID);
    }

    [PunRPC]
    void PickUpItems(int playerid)
    {
        PlayerController player = PhotonView.Find(playerid).GetComponent<PlayerController>();
        if (player != null)
        {
            player.pickupText.gameObject.SetActive(false);
            if (player.photonView.IsMine)
            {
                player.photonView.RPC("GetBandage", RpcTarget.All, value);
                Destroy(gameObject);
            }
        }
    }
}