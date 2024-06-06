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

    int maxAmount = 100;
    int minAmount = 100;
    public Highlight highlight;

    private void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
    }

    public void PickUp(PlayerController player)
    {
        photonView.RPC("NotifyAmmoPickedUp", RpcTarget.All, player.photonView.ViewID);
    }

    [PunRPC]
    void NotifyAmmoPickedUp(int playerID)
    {
        PlayerController player = PhotonView.Find(playerID).GetComponent<PlayerController>();
        Debug.Log("Ammo picked up by player with ID: " + playerID);
        if (player != null)
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
            Destroy(gameObject);
        }
    }
}
