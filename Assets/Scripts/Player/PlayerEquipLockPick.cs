using Photon.Pun;
using UnityEngine;

public class PlayerEquipLockPick : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform inventoryTransform;
    public bool hasLockPick = false;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LockPickEquip lockPickEquip = other.GetComponent<LockPickEquip>();
            if (lockPickEquip != null)
            {
                PhotonView lockPickPhotonView = lockPickEquip.GetComponent<PhotonView>();
                if (lockPickPhotonView != null)
                {
                    int lockPickViewID = lockPickPhotonView.ViewID;
                    photonView.RPC("EquipLockPick", RpcTarget.AllBuffered, lockPickViewID);
                    lockPickEquip.PickUp();

                    MissionHospital questMission = FindObjectOfType<MissionHospital>();
                    if (questMission != null)
                    {
                        questMission.photonView.RPC("LockPickPickedUp", RpcTarget.AllBuffered);
                    }
                }
            }
            else
            {
                PainKiller painKiller = other.GetComponent<PainKiller>();
                if (painKiller != null)
                {
                    PhotonView painKillerPhotonView = painKiller.GetComponent<PhotonView>();
                    if (painKillerPhotonView != null)
                    {
                        int painKillerViewID = painKillerPhotonView.ViewID;
                        photonView.RPC("EquipPainKiller", RpcTarget.AllBuffered, painKillerViewID);
                        if (photonView.IsMine)
                        {
                            MissionHospital questMission = FindObjectOfType<MissionHospital>();
                            if (questMission != null)
                            {
                                questMission.photonView.RPC("IncreasePainKillerCount", RpcTarget.AllBuffered);
                            }
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void EquipLockPick(int lockPickViewID)
    {
        GameObject lockPickObj = PhotonView.Find(lockPickViewID).gameObject;
        LockPickEquip lockPickEquip = lockPickObj.GetComponent<LockPickEquip>();

        if (lockPickEquip != null)
        {
            lockPickEquip.transform.parent = inventoryTransform;
            lockPickEquip.transform.localPosition = Vector3.zero;
            lockPickEquip.transform.localRotation = Quaternion.identity;
            lockPickEquip.gameObject.SetActive(false);
            hasLockPick = true;
        }
    }

    [PunRPC]
    private void EquipPainKiller(int painKillerViewID)
    {
        GameObject painKillerObj = PhotonView.Find(painKillerViewID).gameObject;
        PainKiller painKillerEquip = painKillerObj.GetComponent<PainKiller>();

        if (painKillerEquip != null)
        {
            painKillerEquip.transform.parent = inventoryTransform;
            painKillerEquip.transform.localPosition = Vector3.zero;
            painKillerEquip.transform.localRotation = Quaternion.identity;
            painKillerEquip.gameObject.SetActive(false);
        }
    }

    public bool HasLockPick()
    {
        return hasLockPick;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hasLockPick);
        }
        else
        {
            hasLockPick = (bool)stream.ReceiveNext();
        }
    }
}
