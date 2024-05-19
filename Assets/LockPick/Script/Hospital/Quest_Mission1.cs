using UnityEngine;
using TMPro;
using Photon.Pun;

public class Quest_Mission1 : MonoBehaviourPunCallbacks
{
    [Header("PanelQuest")]
    public GameObject FindPainKiller;
    public GameObject FindLockPick;
    public GameObject FindUnlock_Location;
    [Header("PanelGuide")]
    public GameObject panelGuide;
    public GameObject panelGuide_FindPainKiller, panelGuide_FindLockPick, panelGuide_FindUnlock_Location, panelGuide_LockPick;

    public TextMeshProUGUI txtFindPainKiller;
    public GameObject panelQuest;
    public int painKiller = 2;
    public int painKillerCount;
    private bool hasShownGuide = false;
    private bool isLockPickPickedUp = false;

    private void Start()
    {
        painKillerCount = 0;
        UpdatePainKillerText();
        panelGuide.SetActive(true);
        panelGuide_FindPainKiller.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TogglePanel(panelQuest);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePanel(panelGuide);
        }
    }

    [PunRPC]
    public void IncreasePainKillerCount()
    {
        painKillerCount++;
        UpdatePainKillerText();
    }

    [PunRPC]
    public void UpdatePainKillerText()
    {
        txtFindPainKiller.text = "Find PainKiller: " + painKillerCount + "/" + painKiller;

        if (painKillerCount >= painKiller)
        {
            FindPainKiller.SetActive(false);
            FindLockPick.SetActive(true);
            FindUnlock_Location.SetActive(false);
            panelQuest.SetActive(true);
            panelGuide.SetActive(true);
            panelGuide_FindPainKiller.SetActive(false);
            panelGuide_FindLockPick.SetActive(true);
            panelGuide_FindUnlock_Location.SetActive(false);
            panelGuide_LockPick.SetActive(false);
            if (isLockPickPickedUp)
            {
                FindPainKiller.SetActive(false);
                FindLockPick.SetActive(false);
                FindUnlock_Location.SetActive(true);
                panelQuest.SetActive(true);
                panelGuide.SetActive(true);
                panelGuide_FindPainKiller.SetActive(false);
                panelGuide_FindLockPick.SetActive(false);
                panelGuide_FindUnlock_Location.SetActive(true);
                panelGuide_LockPick.SetActive(false);
            }
        }
        else if (!isLockPickPickedUp)
        {
            panelGuide_FindPainKiller.SetActive(true);
        }
    }

    [PunRPC]
    public void LockPickPickedUp()
    {
        if (painKillerCount >= painKiller)
        {
            FindPainKiller.SetActive(false);
            FindLockPick.SetActive(false);
            FindUnlock_Location.SetActive(true);
            panelQuest.SetActive(true);
            panelGuide.SetActive(true);
            panelGuide_FindPainKiller.SetActive(false);
            panelGuide_FindLockPick.SetActive(false);
            panelGuide_FindUnlock_Location.SetActive(true);
            panelGuide_LockPick.SetActive(false);

            isLockPickPickedUp = true;
        }
        else
        {
            isLockPickPickedUp = true;
        }
    }

    [PunRPC]
    public void ShowPanelGuideFirstTime()
    {
        if (!hasShownGuide)
        {
            hasShownGuide = true;
            panelGuide.SetActive(true);
            panelGuide_FindPainKiller.SetActive(false);
            panelGuide_FindLockPick.SetActive(false);
            panelGuide_FindUnlock_Location.SetActive(false);
            panelGuide_LockPick.SetActive(true);
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
