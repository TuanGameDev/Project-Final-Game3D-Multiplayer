using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickUp : MonoBehaviourPun
{
    public enum AmmoType
    {
        RifleAmmo,
        SMGAmmo
    }
    public AmmoType ammoType;
    public int amount;

    int maxAmount = 30;
    int minAmount = 10;

    private void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
    }
    public void DestroyObj()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
