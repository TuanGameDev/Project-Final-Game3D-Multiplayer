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
    [SerializeField] public string loadlevel;
    public TextMeshProUGUI battleBusText;
    public TextMeshProUGUI missionText;
    [Header("PanelGuide")]
    public GameObject panelGuide;
    private bool isInsideTrigger = false;
    public void Update()
    {
        photonView.RPC("UpdatefuelCount", RpcTarget.All, fuelMin, fuelMax);
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePanel(panelGuide);
        }
        if (isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.F) && fuelMin < fuelMax)
            {
                photonView.RPC("IncreaseRefuel", RpcTarget.All);
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                isInsideTrigger = true;
                battleBusText.text = "Press F to Refuel";
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                isInsideTrigger = false;
                battleBusText.text = "";
            }
        }
    }
    [PunRPC]
    void IncreaseRefuel()
    {
        if (fuelMin < fuelMax)
        {
            fuelMin++;
        }
    }
    [PunRPC]
    void UpdatefuelCount(int count, int countMax)
    {
        missionText.text = "\r\nAmount of fuel required: " + count + "/" + countMax;
    }
    private void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
