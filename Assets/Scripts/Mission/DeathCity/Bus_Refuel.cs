using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class Bus_Refuel : MonoBehaviourPunCallbacks
{
    public string loadlevel;
    public GameObject refuelSliderUI;
    public Slider repairSlider;
    public GameObject paneltxtRefuel;
    public GameObject paneltxtNotRefuel;
    private bool isBeingRefuel = false;
    private Coroutine refuelCoroutine;
    private bool isPlayerPressingE = false;
    public GameObject panelWin;
    public GameObject panelMission;
    [Header("AudioEnd Hospital")]
    [SerializeField] public AudioSource footstepAudioEnd;
    [SerializeField] public AudioClip footstepEnd;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            PlayerEquip_Repair playerEquip = other.GetComponent<PlayerEquip_Repair>();
            if (playerEquip != null)
            {
                MissionDeadCity mission = FindObjectOfType<MissionDeadCity>();
                if (mission != null && mission.fuelMin < mission.fuelMax)
                {
                    if (paneltxtRefuel != null)
                    {
                        if (playerEquip.hasPickUp && !isBeingRefuel)
                        {
                            paneltxtRefuel.SetActive(true);
                            paneltxtNotRefuel.SetActive(false);
                        }
                        else
                        {
                            paneltxtRefuel.SetActive(false);
                            paneltxtNotRefuel.SetActive(true);
                        }
/*                        mission.photonView.RPC("HideRepairShip", RpcTarget.All);
                        mission.photonView.RPC("ShowPanelFindRepairShipFirstTime", RpcTarget.All);*/
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MissionDeathVillage mission = FindObjectOfType<MissionDeathVillage>();
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            paneltxtNotRefuel.SetActive(false);
            paneltxtRefuel.SetActive(false);
            if (isBeingRefuel)
            {
                ResetRefuel();
            }
            isPlayerPressingE = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            PlayerEquip_Repair playerEquip = other.GetComponent<PlayerEquip_Repair>();
            if (playerEquip != null && playerEquip.hasPickUp && Input.GetKeyDown(KeyCode.E))
            {
                if (!isBeingRefuel && playerEquip.hasPickUp)
                {
                    bool isSpecificPlayer = other.GetComponent<PhotonView>().IsMine;
                    if (isSpecificPlayer)
                    {
                        isPlayerPressingE = true;
                        photonView.RPC("StartRefuel", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
                    }
                }
            }
        }
    }



    [PunRPC]
    public void StartRefuel(int playerViewID)
    {
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PlayerEquip_Repair playerEquip = playerPhotonView.GetComponent<PlayerEquip_Repair>();
        if (!isBeingRefuel && isPlayerPressingE && playerEquip.hasPickUp)
        {
            isBeingRefuel = true;
            refuelSliderUI.SetActive(true);
            paneltxtRefuel.SetActive(false);
            refuelCoroutine = StartCoroutine(RefuelProcess(playerViewID));
        }
    }
    private IEnumerator RefuelProcess(int playerViewID)
    {
        float refuelTime = 10f;
        repairSlider.maxValue = refuelTime;
        repairSlider.value = 0;
        while (repairSlider.value < refuelTime)
        {
            repairSlider.value += Time.deltaTime;
            yield return null;
        }
        FinishRefuel(playerViewID);
    }
    private void FinishRefuel(int playerViewID)
    {
        refuelSliderUI.SetActive(false);
        isBeingRefuel = false;
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PlayerEquip_Repair playerEquip = playerPhotonView.GetComponent<PlayerEquip_Repair>();
        if (playerEquip != null)
        {
            playerEquip.hasPickUp = false;
        }
        photonView.RPC("NotifyRefuelFinished", RpcTarget.All);
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

    private void ResetRefuel()
    {
        StopCoroutine(refuelCoroutine);
        refuelCoroutine = null;
        isBeingRefuel = false;
        refuelSliderUI.SetActive(false);
        repairSlider.value = 0;
        paneltxtRefuel.SetActive(false);
        paneltxtNotRefuel.SetActive(false);
    }
    [PunRPC]
    public void NotifyRefuelFinished()
    {
        MissionDeadCity mission = FindObjectOfType<MissionDeadCity>();
        if (mission != null)
        {
            mission.IncreaseRefuelCount();
        }
    }
}

