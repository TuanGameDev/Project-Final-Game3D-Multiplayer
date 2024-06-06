using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionDeadCity : MonoBehaviourPun
{
    [Header("Mission DeadCity")]
    [SerializeField] public float duration;
    [SerializeField] public string loadlevel;
    [SerializeField] public int fuelMin;
    [SerializeField] public int fuelMax;
    [SerializeField] public Slider progressSlider;
    [SerializeField] public TextMeshProUGUI missionText;
    [Header("PanelGuide")]
    [SerializeField] public GameObject panelGuide;
    [SerializeField] public GameObject endGameUI;
    public bool isPlayerPressingE = false;
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
        if (fuelMin >= fuelMax && Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("WinGame", RpcTarget.All);
        }
        if (fuelMin == fuelMax)
        {
            missionText.text = "Press Q to start the bus";
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
    [PunRPC]
    void WinGame()
    {
        StartCoroutine(UpdateSliderValue());
    }
    private IEnumerator UpdateSliderValue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            missionText.text = string.Format("Starting up... {0}%", Mathf.RoundToInt(progress * 100f));
            missionText.color = Color.yellow;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        endGameUI.SetActive(true);
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
