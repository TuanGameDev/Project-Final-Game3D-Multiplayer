using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using TMPro;
using Photon.Realtime;

[System.Serializable]
public class ZombieInfo
{
    public GameObject[] prefabPath;
    public float maxEnemies;
    public List<Transform> spawnCheckRadiusPirate = new List<Transform>();
    public List<GameObject> currentEnemies = new List<GameObject>();
}

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public FPSController[] players;
    public Transform[] spawnPoint;
    public float respawnTime;
    private int playersInGame;

    [Header("Spawn Zombie")]
    public ZombieInfo zombieSpawnInfo;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;

    [Header("Spawn Gun")]
    public GameObject[] gun;
    public Transform[] spawnGun;

    public static GameManager gamemanager;

    private void Awake()
    {
        gamemanager = this;
    }

    void Start()
    {
        players = new FPSController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawnZombie();
        }
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
            SpawnGun();
    }

    private void SpawnGun()
    {
        for (int i = 0; i < gun.Length; i++)
        {
            PhotonNetwork.Instantiate(gun[i % gun.Length].name, spawnGun[i].position, spawnGun[i].rotation);
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate(PlayerSelection.playerselection.playerPrefabName, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);
        playerObject.GetComponent<PhotonView>().RPC("Initialized", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    void TrySpawn(ZombieInfo zombieSpawnInfo)
    {
        for (int x = zombieSpawnInfo.currentEnemies.Count - 1; x >= 0; x--)
        {
            if (zombieSpawnInfo.currentEnemies[x] == null)
            {
                zombieSpawnInfo.currentEnemies.RemoveAt(x);
            }
        }

        if (zombieSpawnInfo.currentEnemies.Count >= zombieSpawnInfo.maxEnemies)
            return;
        else if (zombieSpawnInfo == this.zombieSpawnInfo)
        {
            if (zombieSpawnInfo.spawnCheckRadiusPirate.Count == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, zombieSpawnInfo.spawnCheckRadiusPirate.Count);
            Transform spawnPoint = zombieSpawnInfo.spawnCheckRadiusPirate[randomIndex];
            Vector3 randomOffset = Random.insideUnitCircle * spawnPoint.localScale.x;
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            if (zombieSpawnInfo.prefabPath.Length == 0)
            {
                Debug.LogError("Prefab array is empty!");
                return;
            }

            int randomPrefabIndex = Random.Range(0, zombieSpawnInfo.prefabPath.Length);
            GameObject prefab = zombieSpawnInfo.prefabPath[randomPrefabIndex];

            if (prefab == null)
            {
                Debug.LogError("Prefab at index " + randomPrefabIndex + " is null!");
                return;
            }

            GameObject enemy = PhotonNetwork.Instantiate(prefab.name, spawnPosition, Quaternion.identity);
            zombieSpawnInfo.currentEnemies.Add(enemy);
        }
    }

    void TrySpawnZombie()
    {
        TrySpawn(zombieSpawnInfo);
    }
}
