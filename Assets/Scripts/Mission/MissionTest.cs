using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class MissionTest : MonoBehaviourPun
{
    public int playerCount = 0;
    public int maxPlayerCount = 4;
    public float cooldownDuration = 1f;
    public TextMeshProUGUI playercountText;
    public GameObject ui;
    public bool isInsideTrigger = false;

    private float cooldownTimer = 0f;
    public TextMeshProUGUI timerText;
    private void Update()
    {
        photonView.RPC("UpdatePlayerCount", RpcTarget.All, playerCount, maxPlayerCount);
        if (isInsideTrigger)
        {
            if (cooldownTimer <= 0f && Input.GetKeyDown(KeyCode.F) && playerCount < maxPlayerCount)
            {
                photonView.RPC("IncreasePlayerCount", RpcTarget.All);
                StartCooldown();
            }
        }
        if (playerCount == maxPlayerCount)
        {
            ui.SetActive(false);
        }
        if (playerCount == maxPlayerCount && Input.GetKeyDown(KeyCode.Q))
        {
            //photonView.RPC("AudioGeneratorStart", RpcTarget.All);
            Debug.Log("Đang khởi động");
        }

        UpdateCooldown();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                ui.SetActive(true);
                isInsideTrigger = true;
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
                ui.SetActive(false);
                isInsideTrigger = false;
            }
        }
    }
    [PunRPC]
    void IncreasePlayerCount()
    {
        if (playerCount < maxPlayerCount)
        {
            playerCount++;
        }
    }

    [PunRPC]
    void UpdatePlayerCount(int count, int countMax)
    {
        playercountText.text = "4 Người để mở khóa: " + count + "/" + countMax;
    }

    void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
    }

    void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            timerText.text ="Thời gian còn lại: " + cooldownTimer.ToString("0.0");
        }
        else
        {
            timerText.text = "Nhấn F để sửa";
        }
    }
}