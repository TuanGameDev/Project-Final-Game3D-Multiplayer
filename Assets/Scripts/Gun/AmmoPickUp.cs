using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickUp : MonoBehaviourPun
{
    public enum AmmoType
    {
        RifleAmmo,
        SMG_PistolAmmo
    }

    PlayerController player;
    public AmmoType ammoType;
    [SerializeField] int amount;
    public bool _canPickUp = true;

    int maxAmount = 30;
    int minAmount = 10;

    private void Start()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canPickUp)
        {
            photonView.RPC("RPC_PickupAmmo", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_PickupAmmo(PhotonMessageInfo info)
    {
        if (player != null)
        {
            player.pickupText.gameObject.SetActive(false);
            if (info.photonView.IsMine)
            {
                player.photonView.RPC("PickUpAmmo", RpcTarget.All, ammoType, amount);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            _canPickUp = true;
            player.pickupText.gameObject.SetActive(true);
            player.pickupText.text = " Press F to pick up: " + amount + " " + ammoType;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        player = other.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            _canPickUp = false;
            player.pickupText.gameObject.SetActive(false);
        }
    }
}
