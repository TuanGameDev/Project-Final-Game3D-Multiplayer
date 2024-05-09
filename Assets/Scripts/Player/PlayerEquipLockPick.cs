using Photon.Pun;
using UnityEngine;

public class PlayerEquipLockPick : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform inventoryTransform;
    public bool hasLockPick = false;

    private void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine) return;

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
