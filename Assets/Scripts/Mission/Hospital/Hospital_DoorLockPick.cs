using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital_DoorLockPick : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Hospital_DoorLockPick instance;
    public string loadlevel;
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
    public GameObject warningQuest;
    bool isBreak = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        MissionHospital quest_Mission = FindObjectOfType<MissionHospital>();
        if (!isUnLocked && isPlayerNearDoor && playerEquipLockPick != null && playerEquipLockPick.HasLockPick())
        {
            if (Input.GetKeyDown(KeyCode.E) && !isInMiniGame && quest_Mission != null && quest_Mission.painKillerCount >= quest_Mission.painKiller && !isBreak)
            {
                photonView.RPC("OpenMiniGame", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (Input.GetKeyDown(KeyCode.E) && quest_Mission != null && quest_Mission.painKillerCount < quest_Mission.painKiller)
            {
                warningQuest.SetActive(true);
            }
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
                    MissionHospital quest_Mission = FindObjectOfType<MissionHospital>();

                    if (playerEquipLockPick.HasLockPick() && quest_Mission != null && quest_Mission.painKillerCount >= quest_Mission.painKiller)
                    {
                        panelLockPick.SetActive(true);
                        panelWarning.SetActive(false);
                        warningQuest.SetActive(false);
                        quest_Mission.photonView.RPC("ShowPanelGuideFirstTime", RpcTarget.All);
                    }
                    else if (playerEquipLockPick.HasLockPick() && quest_Mission != null && quest_Mission.painKillerCount < quest_Mission.painKiller)
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(false);
                        warningQuest.SetActive(true);
                    }
                    else
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(true);
                        warningQuest.SetActive(false);
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
            warningQuest.SetActive(false);
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
            photonView.RPC("GameIsWin", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void GameIsWin()
    {
        StartCoroutine(ShowWinPanel());
    }

    IEnumerator ShowWinPanel()
    {
        panelWin.SetActive(true);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(11f);
        Loadlevel();
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

    public void Loadlevel()
    {
        PhotonNetwork.LoadLevel(loadlevel);
        Time.timeScale = 1;
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
