using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathVillage_DoorLockPick : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Lockpick")]
    public GameObject panelLockPick;
    public GameObject panelWarning;
    public GameObject minigameLockPick;
    public GameObject lock1;
    public GameObject lock2;
    public GameObject cameraLockpick;
    public GameObject panelWin, panelBreak;
    public GameObject DoorController_canvasPanelDeathVillage;
    public GameObject DoorLockPick_canvasPanelDeathVillage;

    private PlayerEquipLockPick playerEquipLockPick;
    private PlayerController playerController;

    private bool isInMiniGame = false;
    public bool isUnLocked = false;
    private Dictionary<int, bool> playerNearDoorMap = new Dictionary<int, bool>();
    private bool isPlayerNearDoor = false;
    bool isBreak = false;
    [Header("DoorController")]
    public GameObject panelOpen;
    public GameObject panelClose;
    public GameObject door;
    private bool isDoorOpen = false;
    bool isOpening = false;
    private void Update()
    {
        Mission_DeathVillage quest_Mission = FindObjectOfType<Mission_DeathVillage>();
        if (!isUnLocked && isPlayerNearDoor && playerEquipLockPick != null && playerEquipLockPick.HasLockPick())
        {
            if (Input.GetKeyDown(KeyCode.E) && !isInMiniGame && quest_Mission != null && !isBreak)
            {
                photonView.RPC("OpenMiniGame", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
        else if (isUnLocked && !isOpening && isPlayerNearDoor && Input.GetKeyDown(KeyCode.E))
        {
            photonView.RPC("ToggleDoor", RpcTarget.AllBuffered);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isInMiniGame)
        {
            CloseMiniGame();
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
            if (!isBreak)
            {
                isPlayerNearDoor = true;
                playerEquipLockPick = other.GetComponent<PlayerEquipLockPick>();
                playerController = other.GetComponent<PlayerController>();
                if (playerEquipLockPick != null && playerController != null)
                {
                    Mission_DeathVillage quest_Mission = FindObjectOfType<Mission_DeathVillage>();

                    if (playerEquipLockPick.HasLockPick() && !isUnLocked && quest_Mission != null)
                    {
                        panelLockPick.SetActive(true);
                        panelWarning.SetActive(false);
                        panelOpen.SetActive(false);
                        panelClose.SetActive(false);
                        quest_Mission.ShowPanelLockPickFirstTime();
                    }
                    else
                    {
                        panelWarning.SetActive(true);
                        if (!isUnLocked)
                        {
                            quest_Mission.ShowPanelFindLockPickFirstTime();
                        }
                    }
                    if (isUnLocked && !isDoorOpen)
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(false);
                        panelOpen.SetActive(true);
                        panelClose.SetActive(false);
                    }
                    else if (isUnLocked && !isDoorOpen)
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(false);
                        panelOpen.SetActive(false);
                        panelClose.SetActive(true);
                    }
                }
            }
            else
            {
                panelLockPick.SetActive(false);
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
            panelOpen.SetActive(false);
            panelClose.SetActive(false);
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
            photonView.RPC("GameIsWin", RpcTarget.All);
        }
        else
        {
            DoorController_canvasPanelDeathVillage.SetActive(false);
        }
    }
    [PunRPC]
    void GameIsWin()
    {
        StartCoroutine(ShowWinPanel());
        isUnLocked = true;
        panelWin.SetActive(true);
        DoorController_canvasPanelDeathVillage.SetActive(true);
        DoorLockPick_canvasPanelDeathVillage.SetActive(false);
    }
    IEnumerator ShowWinPanel()
    {
        yield return new WaitForSecondsRealtime(2f);
        panelWin.SetActive(false);

    }

    public void IsBreak()
    {
        StartCoroutine(ShowPanelBreak());
    }

    IEnumerator ShowPanelBreak()
    {
        isBreak = true;
        panelLockPick.SetActive(false);
        panelBreak.SetActive(true);
        yield return new WaitForSecondsRealtime(5f);
        panelBreak.SetActive(false);
        isBreak = false;
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
