using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using System.Reflection;
using TMPro;
using System.Collections.Generic;

public class Repair_Ship : MonoBehaviourPunCallbacks
{
    public string loadlevel;
    public TextMeshProUGUI repairText;
    public TextMeshProUGUI shipText;
    public GameObject paneltxtRepair;
    public GameObject paneltxtNotRepair;
    private bool isBeingRepaired = false;
    private Coroutine repairCoroutine;
    private Coroutine startShipCoroutine;
    private bool isPlayerPressingE = false;

    bool isBeingStartShip = false;
    private bool isPlayerCountNotified = false;
    public GameObject panelWin;
    public GameObject panelMission;

    [Header("Spawn Zombie")]
    private PlayerController targetPlayer;
    private PlayerController[] playerInScene;
    public GameObject[] zombie;
    public float maxEnemies;
    public Transform[] spawnenemyPoint;
    public float spawnCheckTime;
    public float spawnCollisionRadius;
    private float lastSpawnCheckTime;
    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("AudioEnd Dead Village")]
    [SerializeField] public AudioSource footstepAudioEnd;
    [SerializeField] public AudioClip footstepEnd;
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            SpawnCheckDarius();
        }
    }
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
                        }
                        else
                        {
                            paneltxtRepair.SetActive(false);
                            paneltxtNotRepair.SetActive(true);
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
                shipText.text = "Press E to start";
                shipText.gameObject.SetActive(true);
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
            paneltxtRepair.SetActive(false);
            shipText.gameObject.SetActive(false);
            repairCoroutine = StartCoroutine(RepairProcess(playerViewID));
        }
    }

    private IEnumerator RepairProcess(int playerViewID)
    {
        float refuelTime = 10f;
        float currentRefuelTime = 0f;
        repairText.gameObject.SetActive(true);

        while (currentRefuelTime < refuelTime)
        {
            currentRefuelTime += Time.deltaTime;
            float refuelPercentage = (currentRefuelTime / refuelTime) * 100f;
            repairText.text = "Repairing: " + Mathf.RoundToInt(refuelPercentage) + "%";
            yield return null;
        }

        repairText.gameObject.SetActive(false);
        FinishRepair(playerViewID);
    }

    private void FinishRepair(int playerViewID)
    {
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
            startShipCoroutine = StartCoroutine(StartShipProcess());
        }
    }

    private IEnumerator StartShipProcess()
    {
        float refuelTime = 50f;
        float currentRefuelTime = 0f;
        shipText.gameObject.SetActive(true);

        while (currentRefuelTime < refuelTime)
        {
            currentRefuelTime += Time.deltaTime;
            float refuelPercentage = (currentRefuelTime / refuelTime) * 100f;
            shipText.text = "Start the ship: " + Mathf.RoundToInt(refuelPercentage) + "%";
            yield return null;
        }

        shipText.gameObject.SetActive(false);
        FinishStartShip();
    }

    private void FinishStartShip()
    {
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
        shipText.gameObject.SetActive(false);
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
    void SpawnZombie()
    {
        for (int x = currentEnemies.Count - 1; x >= 0; x--)
        {
            if (!currentEnemies[x])
            {
                currentEnemies.RemoveAt(x);
            }
        }

        if (currentEnemies.Count >= maxEnemies)
            return;
        int randomIndex = Random.Range(0, zombie.Length);
        GameObject enemy = PhotonNetwork.Instantiate(zombie[randomIndex].name, spawnenemyPoint[Random.Range(0, spawnenemyPoint.Length)].position, Quaternion.identity);
        currentEnemies.Add(enemy);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, spawnCollisionRadius);
    }
    private void SpawnCheckDarius()
    {
        playerInScene = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in playerInScene)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < spawnCollisionRadius)
            {
                targetPlayer = player;
                SpawnZombie();
            }
        }
    }
}
