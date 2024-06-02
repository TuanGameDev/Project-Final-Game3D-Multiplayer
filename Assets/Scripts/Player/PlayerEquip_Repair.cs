using Photon.Pun;
using UnityEngine;

public class PlayerEquip_Repair : MonoBehaviourPunCallbacks
{
    public bool hasPickUp;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Repair_Ship_Equip repair = other.GetComponent<Repair_Ship_Equip>();
            
            if (repair != null)
            {
                PhotonView repairPhotonView = repair.GetComponent<PhotonView>();
                if (repairPhotonView != null)
                {
                    if (hasPickUp)
                    {
                        repair.ShowPickUpPanel();
                    }
                    else
                    {
                        int repairViewID = repairPhotonView.ViewID;
                        photonView.RPC("PickUpRepair", RpcTarget.All, repairViewID);

                    }
                }
            }
        }
    }

    [PunRPC]
    public void PickUpRepair(int repairViewID)
    {
        if (!hasPickUp)
        {
            PhotonView repairPhotonView = PhotonView.Find(repairViewID);
            if (repairPhotonView != null)
            {
                Repair_Ship_Equip repair = repairPhotonView.GetComponent<Repair_Ship_Equip>();
                if (repair != null)
                {
                    repair.PickUp();
                    hasPickUp = true;
                }
            }
        }
    }

    [PunRPC]
    public void ShowPickUpPanel(int repairViewID)
    {
        PhotonView repairPhotonView = PhotonView.Find(repairViewID);
        if (repairPhotonView != null)
        {
            Repair_Ship_Equip repair = repairPhotonView.GetComponent<Repair_Ship_Equip>();
            if (repair != null)
            {
                repair.ShowPickUpPanel();
            }
        }
    }
}
