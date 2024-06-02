using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEditor;

public class Mission_DeathVillage : MonoBehaviourPunCallbacks
{
    [Header("PanelQuest")]
    public GameObject FindShip;
    public GameObject RepairShip;
    public GameObject StartTheShip;

    [Header("PanelGuide")]
    public GameObject panelGuide;
    public GameObject panelGuide_FindShip, panelGuide_FindRepair, panelGuide_FindLockPick, panelGuide_LockPick;

    public bool hasShownFindRepairShip = false;
    public bool hasShownRepairShip = false;
    public bool hasShownLockPick = false;

    public TextMeshProUGUI txtRepair;
    public int repair = 4;
    public int repairCount;

    public TextMeshProUGUI txtPlayer;
    public int player;
    public int playerCount;

    private void Start()
    {
        player = PhotonNetwork.PlayerList.Length;
        panelGuide.SetActive(true);
        panelGuide_FindShip.SetActive(true);
        UpdateRepairText();
        UpdatePlayerOnShipText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePanel(panelGuide);
        }
    }

    public void UpdateRepairText()
    {
        txtRepair.text = "Repair Ship: " + repairCount + "/" + repair;

        if (repairCount >= repair)
        {
            FindShip.SetActive(false);
            RepairShip.SetActive(false);
            StartTheShip.SetActive(true);
        }
    }
    public void UpdatePlayerOnShipText()
    {
        txtPlayer.text = "Player Near Ship: " + playerCount + "/" + player;

        if (playerCount >= player)
        {
            FindShip.SetActive(false);
            RepairShip.SetActive(false);
            StartTheShip.SetActive(true);
        }
    }
    [PunRPC]
    public void IncreaseRepairCount()
    {
        repairCount++;
        UpdateRepairText();
    }
    [PunRPC]
    public void IncreasePlayerOnShipCountPlus()
    {
        playerCount++;
        UpdatePlayerOnShipText(); 
    }
    [PunRPC]
    public void IncreasePlayerOnShipCountMinus()
    {
        playerCount--;
        UpdatePlayerOnShipText();
    }
    private void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
    #region Hide Quest
    [PunRPC]
    public void HideFindShip()
    {
        FindShip.SetActive(true);
        RepairShip.SetActive(false);
        StartTheShip.SetActive(false);

    }
    [PunRPC]
    public void HideRepairShip()
    {
        FindShip.SetActive(false);
        RepairShip.SetActive(true);
        StartTheShip.SetActive(false);

    }
    [PunRPC]
    public void HideStartTheShip()
    {
        FindShip.SetActive(false);
        RepairShip.SetActive(false);
        StartTheShip.SetActive(true);

    }
    [PunRPC]
    public void ShowPanelFindRepairShipFirstTime()
    {
        if (!hasShownFindRepairShip)
        {
            hasShownFindRepairShip = true;
            panelGuide.SetActive(true);
            panelGuide_FindShip.SetActive(false);
            panelGuide_FindRepair.SetActive(true);
            panelGuide_FindLockPick.SetActive(false);
            panelGuide_LockPick.SetActive(false);
        }
    }
    [PunRPC]
    public void ShowPanelFindLockPickFirstTime()
    {
        if (!hasShownFindRepairShip)
        {
            hasShownFindRepairShip = true;
            panelGuide.SetActive(true);
            panelGuide_FindShip.SetActive(false);
            panelGuide_FindRepair.SetActive(false);
            panelGuide_FindLockPick.SetActive(true);
            panelGuide_LockPick.SetActive(false);
        }
    }
    [PunRPC]
    public void ShowPanelLockPickFirstTime()
    {
        if (!hasShownLockPick)
        {
            hasShownLockPick = true;
            panelGuide.SetActive(true);
            panelGuide_FindShip.SetActive(false);
            panelGuide_FindRepair.SetActive(false);
            panelGuide_FindLockPick.SetActive(false);
            panelGuide_LockPick.SetActive(true);
        }
    }
    #endregion
}
