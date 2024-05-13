﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionGenerator : MonoBehaviourPun
{
    [Header("Mission Generator")]
    [SerializeField] public string loadlevel;
    public int playerCount = 0;
    public int maxPlayerCount = 10;
    [SerializeField] public float duration;
    [SerializeField] public float spawnCollisionRadius;
    [SerializeField] public float cooldownDuration = 1f;
    private float cooldownTimer = 0f;
    [SerializeField] public Slider progressSlider;
    [SerializeField] public TextMeshProUGUI notificationText;
    [SerializeField] public TextMeshProUGUI generatorText;
    [SerializeField] public TextMeshProUGUI timerText;
    [Header("AudioEdit Generator")]
    [SerializeField] public AudioSource footstepAudioEdit;
    [SerializeField] public AudioClip footstepSoundsEdit;
    [Header("AudioStart Generator")]
    [SerializeField] public AudioSource footstepAudioSource;
    [SerializeField] public AudioClip footstepSounds;
    [Header("AudioEnd Generator")]
    [SerializeField] public AudioSource footstepAudioEnd;
    [SerializeField] public AudioClip footstepEnd;

    [SerializeField] public GameObject uiEndGame;
    [SerializeField] public GameObject lingred;
    [SerializeField] public GameObject linggreen;
    [Header("Boss Mission 3")]
    public GameObject bossPrefabs;
    public Transform spawnBossPoint;
    public float maxEnemies;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool isInsideTrigger = false;
    private bool isGeneratorRunning = false;
    private bool isStartupSuccessful = false;
    private void Start()
    {
        progressSlider.gameObject.SetActive(false);
    }
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
        if (playerCount == maxPlayerCount && Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("AudioGeneratorStart", RpcTarget.All);
        }
        UpdateCooldown();
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (playerCount == maxPlayerCount)
        {
            notificationText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(false);
            generatorText.gameObject.SetActive(false);
            footstepAudioEdit.Stop();
            SpawnBoss();
        }
    }
    void SpawnBoss()
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
        GameObject enemy = PhotonNetwork.Instantiate(bossPrefabs.name, spawnBossPoint.position, Quaternion.identity);
        currentEnemies.Add(enemy);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                  isInsideTrigger = true;
                timerText.gameObject.SetActive(true);
                if (isGeneratorRunning)
                {
                    if (isStartupSuccessful)
                    {
                        notificationText.text = "Successful launch!";
                        notificationText.color = Color.green;
                    }
                    else
                    {
                        notificationText.text = "Starting up...";
                        notificationText.color = Color.yellow;
                    }
                }
                else
                {
                    notificationText.text = "Press Q to start";
                    notificationText.color = Color.red;
                }
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
                notificationText.text = "";
                timerText.gameObject.SetActive(false);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnCollisionRadius);
    }
    [PunRPC]
    void IncreasePlayerCount()
    {
        if (playerCount < maxPlayerCount)
        {
            playerCount++;
        }
        if (!footstepAudioEdit.isPlaying)
        {
            footstepAudioEdit.clip = footstepSoundsEdit;
            footstepAudioEdit.Play();
        }
    }
    [PunRPC]
    void UpdatePlayerCount(int count, int countMax)
    {
        generatorText.text = "Members who need to repair generators: " + count + "/" + countMax;
    }
    [PunRPC]
    void AudioGeneratorStart()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.clip = footstepSounds;
            footstepAudioSource.Play();
        }
        linggreen.gameObject.SetActive(true);
        lingred.gameObject.SetActive(false);
        isGeneratorRunning = true;
        notificationText.text = "Starting up...";
        notificationText.color = Color.yellow;
        progressSlider.gameObject.SetActive(true);
        StartCoroutine(UpdateSliderValue());
    }
    [PunRPC]
    void AudioGeneratorStop()
    {
        footstepAudioSource.Stop();
        progressSlider.gameObject.SetActive(false);
        lingred.gameObject.SetActive(true);
        linggreen.gameObject.SetActive(false);
    }
    private IEnumerator UpdateSliderValue()
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            progressSlider.value = progress;

            notificationText.text = string.Format("Starting up... {0}%", Mathf.RoundToInt(progress * 100f));
            notificationText.color = Color.yellow;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        progressSlider.value = 1f;
        photonView.RPC("AudioGeneratorStop", RpcTarget.All);
        notificationText.text = "Successful launch! 100%";
        notificationText.color = Color.green;

        yield return new WaitForSeconds(5f);

        if (!footstepAudioEnd.isPlaying)
        {
            footstepAudioEnd.clip = footstepEnd;
            footstepAudioEnd.Play();
        }
        uiEndGame.SetActive(true);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(11f);
        Loadlevel();
    }
    public void Loadlevel()
    {
        PhotonNetwork.LoadLevel(loadlevel);
        Time.timeScale = 1;
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
            timerText.text = "Time remaining: " + cooldownTimer.ToString("0.0");
            timerText.color = Color.yellow;
        }
        else
        {
            timerText.text = "Press F to edit";
            timerText.color = Color.green;
        }
    }
}
