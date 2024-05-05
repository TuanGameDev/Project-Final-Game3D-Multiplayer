using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Services.Analytics.Internal;

public class EnemySpawner : MonoBehaviourPun
{
    public GameObject[] enemyPrefabs;
    private PlayerController targetPlayer;
    private PlayerController[] playerInScene;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;
    public float spawnCollisionRadius;
    private float lastSpawnCheckTime;
    private List<GameObject> currentEnemies = new List<GameObject>();

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

    void TrySpawn()
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

        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + randomInCircle;
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabs[randomIndex].name, spawnPosition, Quaternion.identity);
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
                TrySpawn();
            }
        }
    }
}