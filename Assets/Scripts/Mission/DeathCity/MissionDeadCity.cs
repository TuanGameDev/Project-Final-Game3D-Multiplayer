using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionDeadCity : MonoBehaviourPun
{
    [Header("Mission DeadCity")]
    public int fuelMin;
    public int fuelMax;
    public TextMeshProUGUI missionText;
    [Header("PanelGuide")]
    public GameObject panelGuide;

    private void Start()
    {
        UpdateRefuelText();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePanel(panelGuide);
        }
    }
    [PunRPC]
    public void IncreaseRefuelCount()
    {
        fuelMin++;
        UpdateRefuelText();
    }
    public void UpdateRefuelText()
    {
        missionText.text = "Bus Refuel: " + fuelMin + "/" + fuelMax;
    }
    private void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
