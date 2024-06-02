using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathVillage_DoorController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject panelOpen;
    public GameObject panelClose;
    public GameObject door;

    private Dictionary<int, bool> playerNearDoorMap = new Dictionary<int, bool>();

    private bool isDoorOpen = false;
    bool isOpening;
    private void Update()
    {
        if (!isOpening && Input.GetKeyDown(KeyCode.E) && IsLocalPlayerNearDoor())
        {
            photonView.RPC("ToggleDoor", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void ToggleDoor()
    {
        if (!isDoorOpen)
        {
            StartCoroutine(OpenDoor());
        }
        else
        {
            StartCoroutine(CloseDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpening = true;
        float timer = 0f;
        float openTime = 1.5f;
        Quaternion startRotation = door.transform.rotation;
        float newYRotation = startRotation.eulerAngles.y - 90f;
        Quaternion targetRotation = Quaternion.Euler(door.transform.rotation.eulerAngles.x, newYRotation, door.transform.rotation.eulerAngles.z);

        while (timer < openTime)
        {
            timer += Time.deltaTime;
            panelOpen.SetActive(false);
            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timer / openTime);
            yield return null;
        }

        isDoorOpen = true;
        isOpening = false;
    }

    private IEnumerator CloseDoor()
    {
        isOpening = true;
        float timer = 0f;
        float closeTime = 1.5f;
        Quaternion startRotation = door.transform.rotation;
        float newYRotation = startRotation.eulerAngles.y + 90f;
        Quaternion targetRotation = Quaternion.Euler(door.transform.rotation.eulerAngles.x, newYRotation, door.transform.rotation.eulerAngles.z);

        while (timer < closeTime)
        {
            timer += Time.deltaTime;
            panelClose.SetActive(false);
            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timer / closeTime);
            yield return null;
        }

        isDoorOpen = false;
        isOpening = false;
    }

    private bool IsLocalPlayerNearDoor()
    {
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        return playerNearDoorMap.ContainsKey(localPlayerId) && playerNearDoorMap[localPlayerId];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            int playerId = other.gameObject.GetComponent<PhotonView>().Owner.ActorNumber;
            playerNearDoorMap[playerId] = true;

            if (!isDoorOpen)
            {
                panelOpen.SetActive(true);
                panelClose.SetActive(false);
            }
            else
            {
                panelOpen.SetActive(false);
                panelClose.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            int playerId = other.gameObject.GetComponent<PhotonView>().Owner.ActorNumber;
            playerNearDoorMap[playerId] = false;

            if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
            {
                panelOpen.SetActive(false);
                panelClose.SetActive(false);
            }
        }
    }

    private bool IsAnyPlayerNearDoor()
    {
        foreach (var playerNearDoor in playerNearDoorMap)
        {
            if (playerNearDoor.Value)
            {
                return true;
            }
        }
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDoorOpen);
        }
        else
        {
            isDoorOpen = (bool)stream.ReceiveNext();
        }
    }
}
