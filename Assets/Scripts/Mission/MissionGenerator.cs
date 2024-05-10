using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionGenerator : MonoBehaviourPun
{
    [Header("Mission Generator")]
    [SerializeField] public string loadlevel;
    [SerializeField] public float duration;
    [SerializeField] public float spawnCollisionRadius;
    [SerializeField] public Slider progressSlider;
    [SerializeField] public TextMeshProUGUI notificationText;
    [SerializeField] public AudioSource footstepAudioSource;
    [SerializeField] public AudioClip footstepSounds;
    [SerializeField] public GameObject uiWinGame;
    [SerializeField] public GameObject uiGenerator;
    [SerializeField] public GameObject lingred;
    [SerializeField] public GameObject linggreen;
    [Header("Boss Mission 3")]
    public GameObject bossPrefabs;
    public Transform spawnBossPoint;
    public float maxEnemies;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private PlayerController targetPlayer;
    private PlayerController[] playerInScene;
    private bool isInsideTrigger = false;
    private bool isGeneratorRunning = false;
    private bool isStartupSuccessful = false;
    private void Start()
    {
        progressSlider.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("AudioGeneratorStart", RpcTarget.All);
            }
        }
        if (!PhotonNetwork.IsMasterClient)
            return;
        SpawnCheckGenerator();
    }
    private void SpawnCheckGenerator()
    {
        playerInScene = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in playerInScene)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < spawnCollisionRadius)
            {
                targetPlayer = player;
                SpawnBoss();
            }
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
                uiGenerator.SetActive(true);
                isInsideTrigger = true;
                if (isGeneratorRunning)
                {
                    if (isStartupSuccessful)
                    {
                        notificationText.text = "Khởi động thành công!";
                        notificationText.color = Color.green;
                    }
                    else
                    {
                        notificationText.text = "Đang khởi động...";
                        notificationText.color = Color.yellow;
                    }
                }
                else
                {
                    notificationText.text = "Nhấn Q để khởi động";
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
                uiGenerator.SetActive(false);
                isInsideTrigger = false;
                isInsideTrigger = false;
                notificationText.text = "";
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnCollisionRadius);
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
        notificationText.text = "Đang khởi động...";
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

            notificationText.text = string.Format("Đang khởi động... {0}%", Mathf.RoundToInt(progress * 100f));
            notificationText.color = Color.yellow;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        progressSlider.value = 1f;
        photonView.RPC("AudioGeneratorStop", RpcTarget.All);
        notificationText.text = "Khởi động thành công! 100%";
        notificationText.color = Color.green;

        yield return new WaitForSeconds(5f);

        uiWinGame.SetActive(true);
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
