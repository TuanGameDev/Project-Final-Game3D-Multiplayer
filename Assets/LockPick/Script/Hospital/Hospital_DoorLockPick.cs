using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital_DoorLockPick : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Hospital_DoorLockPick instance;
    public GameObject panelLockPick;
    public GameObject panelWarning;
    public GameObject minigameLockPick;
    private PlayerEquipLockPick playerEquipLockPick;
    private PlayerController playerController;
    public GameObject lock1;
    public GameObject lock2;
    public GameObject cameraLockpick;
    public GameObject panelWin, panelBreak;
    private bool isInMiniGame = false;
    public bool isUnLocked = false;
    private bool isPlayerNearDoor = false;
    private Dictionary<int, bool> playerNearDoorMap = new Dictionary<int, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (!isUnLocked)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerEquipLockPick != null && playerEquipLockPick.HasLockPick() && !isInMiniGame && isPlayerNearDoor)
            {
                photonView.RPC("OpenMiniGame", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            if (Input.GetKeyDown(KeyCode.Escape) && isInMiniGame)
            {
                CloseMiniGame();
            }
        }
    }

    [PunRPC]
    private void OpenMiniGame(int playerActorNumber)
    {
        if (minigameLockPick != null && PhotonNetwork.LocalPlayer.ActorNumber == playerActorNumber)
        {
            minigameLockPick.SetActive(true);
            playerController.enabled = false;
            lock1.SetActive(false);
            lock2.SetActive(true);
            cameraLockpick.SetActive(true);
            isInMiniGame = true;

            if (PhotonNetwork.IsMasterClient)
            {
                minigameLockPick.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }

    public void CloseMiniGame()
    {
        if (minigameLockPick != null && playerController != null)
        {
            minigameLockPick.SetActive(false);
            playerController.enabled = true;
            isInMiniGame = false;
            lock1.SetActive(true);
            lock2.SetActive(false);
            cameraLockpick.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            isPlayerNearDoor = true;
            playerEquipLockPick = other.GetComponent<PlayerEquipLockPick>();
            playerController = other.GetComponent<PlayerController>();
            if (playerEquipLockPick != null && playerController != null)
            {
                if (!isUnLocked)
                {
                    if (playerEquipLockPick.HasLockPick())
                    {
                        panelLockPick.SetActive(true);
                        panelWarning.SetActive(false);
                    }
                    else
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(true);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            isPlayerNearDoor = false;
            playerEquipLockPick = null;
            playerController = null;
            panelLockPick.SetActive(false);
            panelWarning.SetActive(false);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isUnLocked);
        }
        else
        {
            isUnLocked = (bool)stream.ReceiveNext();
        }
    }

    public void SetUnlockState(bool state)
    {
        isUnLocked = state;
        if (state)
        {
            photonView.RPC("ShowWinPanel", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void ShowWinPanel()
    {
        panelWin.SetActive(true);
    }

    private bool IsLocalPlayerNearDoor()
    {
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        return playerNearDoorMap.ContainsKey(localPlayerId) && playerNearDoorMap[localPlayerId];
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
}
