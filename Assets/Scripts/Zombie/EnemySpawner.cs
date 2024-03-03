using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemySpawner : MonoBehaviourPun
{
    public string enemyprefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    public List<GameObject> currentEnemies = new List<GameObject>();
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }
    void TrySpawn()
    {
        for (int x = 0; x < currentEnemies.Count; x++)
        {
            if (!currentEnemies[x])
            {
                currentEnemies.RemoveAt(x);
            }
        }
        if (currentEnemies.Count >= maxEnemies)
            return;
        Vector3 randomIncircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyprefabPath, transform.position + randomIncircle, Quaternion.identity);
        currentEnemies.Add(enemy);
    }
}
