using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Reflection;

public class Repair_Ship : MonoBehaviourPunCallbacks
{
    public string loadlevel;
    public GameObject repairSliderUI;
    public Slider repairSlider;
    public GameObject paneltxtRepair;
    public GameObject paneltxtNotRepair;
    private bool isBeingRepaired = false;
    private Coroutine repairCoroutine;
    private Coroutine startShipCoroutine;
    private bool isPlayerPressingE = false;

    public GameObject startShipSliderUI;
    public GameObject paneltxtStartShip;
    public Slider startShipSlider;
    bool isBeingStartShip = false;
    private bool isPlayerCountNotified = false;
    public GameObject panelWin;
    public GameObject panelMission;
    [Header("AudioEnd Hospital")]
    [SerializeField] public AudioSource footstepAudioEnd;
    [SerializeField] public AudioClip footstepEnd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCollider") && other.GetComponentInParent<PhotonView>().IsMine)
        {
            PlayerEquip_Repair playerEquip = other.GetComponentInParent<PlayerEquip_Repair>();
            if (playerEquip != null)
            {
                MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
                if (mission != null && mission.repairCount < mission.repair)
                {
                    if (paneltxtRepair != null)
                    {
                        if (playerEquip.hasPickUp && !isBeingRepaired)
                        {
                            paneltxtRepair.SetActive(true);
                            paneltxtNotRepair.SetActive(false);
                            paneltxtStartShip.SetActive(false);
                        }
                        else
                        {
                            paneltxtRepair.SetActive(false);
                            paneltxtNotRepair.SetActive(true);
                            paneltxtStartShip.SetActive(false);
                        }
                        mission.photonView.RPC("HideRepairShip", RpcTarget.All);
                        mission.photonView.RPC("ShowPanelFindRepairShipFirstTime", RpcTarget.All);
                    }
                }
                else if (mission.repairCount >= mission.repair)
                {
                    photonView.RPC("NotifyPlayerOnShipCountPlus", RpcTarget.All);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCollider") && other.GetComponentInParent<PhotonView>().IsMine)
        {
            MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
            paneltxtNotRepair.SetActive(false);
            paneltxtRepair.SetActive(false);
            paneltxtStartShip.SetActive(false);
            if (isBeingRepaired)
            {
                ResetRepair();
            }
            isPlayerPressingE = false;
            if (mission != null && mission.repairCount >= mission.repair)
            {
                if (mission.playerCount > 0)
                {
                    photonView.RPC("NotifyPlayerOnShipCountMinus", RpcTarget.All);
                }
                if (isBeingStartShip)
                {
                    ResetStartShip();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCollider") && other.GetComponentInParent<PhotonView>().IsMine)
        {
            MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
            PlayerEquip_Repair playerEquip = other.GetComponentInParent<PlayerEquip_Repair>();
            if (playerEquip != null && playerEquip.hasPickUp && Input.GetKeyDown(KeyCode.E))
            {
                if (!isBeingRepaired && playerEquip.hasPickUp)
                {
                    bool isSpecificPlayer = other.GetComponentInParent<PhotonView>().IsMine;
                    if (isSpecificPlayer)
                    {
                        isPlayerPressingE = true;
                        photonView.RPC("StartRepair", RpcTarget.All, other.GetComponentInParent<PhotonView>().ViewID);
                    }
                }
            }
            if (mission.repairCount >= mission.repair && !isPlayerCountNotified)
            {
                photonView.RPC("NotifyPlayerOnShipCountPlus", RpcTarget.All);
                isPlayerCountNotified = true;
            }
            if (mission.playerCount == mission.player)
            {
                paneltxtStartShip.SetActive(true);
                paneltxtRepair.SetActive(false);
                paneltxtNotRepair.SetActive(false);
            }
            if (mission.playerCount == mission.player && Input.GetKeyDown(KeyCode.E))
            {
                isPlayerPressingE = true;
                photonView.RPC("StartShip", RpcTarget.All);
            }
            else if (mission.playerCount < mission.player)
            {
                ResetStartShip();
            }
        }
    }

    [PunRPC]
    public void StartRepair(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PlayerEquip_Repair playerEquip = playerPhotonView.GetComponent<PlayerEquip_Repair>();
        if (!isBeingRepaired && isPlayerPressingE && playerEquip.hasPickUp)
        {
            isBeingRepaired = true;
            repairSliderUI.SetActive(true);
            paneltxtRepair.SetActive(false);
            repairCoroutine = StartCoroutine(RepairProcess(playerViewID));
        }
    }

    private IEnumerator RepairProcess(int playerViewID)
    {
        float repairTime = 10f;
        repairSlider.maxValue = repairTime;
        repairSlider.value = 0;
        while (repairSlider.value < repairTime)
        {
            repairSlider.value += Time.deltaTime;
            yield return null;
        }
        FinishRepair(playerViewID);
    }

    private void FinishRepair(int playerViewID)
    {
        repairSliderUI.SetActive(false);
        isBeingRepaired = false;
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PlayerEquip_Repair playerEquip = playerPhotonView.GetComponent<PlayerEquip_Repair>();
        if (playerEquip != null)
        {
            playerEquip.hasPickUp = false;
        }
        photonView.RPC("NotifyRepairFinished", RpcTarget.All);
        photonView.RPC("ResetPickUp", RpcTarget.All, playerViewID);
        OnTriggerExit(playerPhotonView.GetComponent<Collider>());
    }

    [PunRPC]
    private void ResetPickUp(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PlayerEquip_Repair playerEquip = playerPhotonView.GetComponent<PlayerEquip_Repair>();
        playerEquip.hasPickUp = false;
    }

    private void ResetRepair()
    {
        StopCoroutine(repairCoroutine);
        repairCoroutine = null;
        isBeingRepaired = false;
        repairSliderUI.SetActive(false);
        repairSlider.value = 0;
        paneltxtRepair.SetActive(false);
        paneltxtNotRepair.SetActive(false);
    }

    [PunRPC]
    public void NotifyRepairFinished()
    {
        MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
        if (mission != null)
        {
            mission.IncreaseRepairCount();
        }
    }

    [PunRPC]
    public void NotifyPlayerOnShipCountPlus()
    {
        MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
        if (mission != null)
        {
            mission.IncreasePlayerOnShipCountPlus();
        }
    }

    [PunRPC]
    public void NotifyPlayerOnShipCountMinus()
    {
        MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
        if (mission != null)
        {
            mission.IncreasePlayerOnShipCountMinus();
        }
    }

    [PunRPC]
    public void StartShip()
    {
        MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
        if (mission.playerCount >= mission.player)
        {
            isBeingStartShip = true;
            startShipSliderUI.SetActive(true);
            paneltxtStartShip.SetActive(false);
            startShipCoroutine = StartCoroutine(StartShipProcess());
        }
    }

    private IEnumerator StartShipProcess()
    {
        float startShipTime = 7f;
        startShipSlider.maxValue = startShipTime;
        startShipSlider.value = 0;
        while (startShipSlider.value < startShipTime)
        {
            startShipSlider.value += Time.deltaTime;
            yield return null;
        }
        FinishStartShip();
    }

    private void FinishStartShip()
    {
        startShipSliderUI.SetActive(false);
        isBeingStartShip = false;
        StartCoroutine(ShowWinPanel());
    }

    private void ResetStartShip()
    {
        if (startShipCoroutine != null)
        {
            StopCoroutine(startShipCoroutine);
            startShipCoroutine = null;
        }
        isBeingStartShip = false;
        startShipSliderUI.SetActive(false);
        startShipSlider.value = 0;
        paneltxtStartShip.SetActive(false);
    }

    private IEnumerator ShowWinPanel()
    {
        if (!footstepAudioEnd.isPlaying)
        {
            footstepAudioEnd.clip = footstepEnd;
            footstepAudioEnd.Play();
        }
        panelWin.SetActive(true);
        panelMission.SetActive(false);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(11f);
        Loadlevel();
    }

    public void Loadlevel()
    {
        PhotonNetwork.LoadLevel(loadlevel);
        Time.timeScale = 1;
    }
}
