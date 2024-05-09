using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionGenerator : MonoBehaviourPun
{
    [Header("Mission Generator")]
    [SerializeField] public AudioSource footstepAudioSource;
    [SerializeField] public AudioClip footstepSounds;
    [SerializeField] public GameObject uiGenerator;
    [SerializeField] public float spawnCollisionRadius;
    private PlayerController targetPlayer;
    private PlayerController[] playerInScene;
    private bool isInsideTrigger = false;
    private void Update()
    {
        if (isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                photonView.RPC("AudioGenerator", RpcTarget.All);
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
                uiGenerator.SetActive(true);
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
                uiGenerator.SetActive(false);
                isInsideTrigger = false;
                isInsideTrigger = false;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnCollisionRadius);
    }
    [PunRPC]
    void AudioGenerator()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.clip = footstepSounds;
            footstepAudioSource.Play();
        }
    }
}
