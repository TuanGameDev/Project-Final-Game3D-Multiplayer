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
    public string ammoName;
    public int amount;

    int maxAmount = 50;
    int minAmount = 50;
    public Highlight highlight;

    private void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
    }

    public void PickUp(PlayerController player)
    {
        int playerID = player.photonView.ViewID;
        photonView.RPC("NotifyAmmoPickedUp", RpcTarget.All, playerID);
    }

    [PunRPC]
    void NotifyAmmoPickedUp(int playerID, PhotonMessageInfo info)
    {
        PlayerController player = PhotonView.Find(playerID).GetComponent<PlayerController>();
        Debug.Log("Ammo picked up by player with ID: " + playerID);
        if (player != null && info.photonView.IsMine)
        {
            switch (ammoType)
            {
                case AmmoType.RifleAmmo:
                    player.rifleAmmo += amount;
                    break;
                case AmmoType.SMGAmmo:
                    player.smgAmmo += amount;
                    break;
                default:
                    break;
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
