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
    public string prefabPath;
    public float maxEnemies;
    public float spawnCheckRadius;
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

    [Header("Spawn Enemy - Naval")]
    public ZombieInfo zombieSpawnInfo;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;

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

        if (zombieSpawnInfo == this.zombieSpawnInfo)
        {
            Vector3 randomIncircle = Random.insideUnitCircle * zombieSpawnInfo.spawnCheckRadius;
            GameObject enemy = PhotonNetwork.Instantiate(zombieSpawnInfo.prefabPath, transform.position + randomIncircle, Quaternion.identity);
            zombieSpawnInfo.currentEnemies.Add(enemy);
        }
        else if (zombieSpawnInfo == this.zombieSpawnInfo)
        {
            if (zombieSpawnInfo.spawnCheckRadiusPirate.Count == 0)
            {
                // Thực hiện xử lý khi danh sách spawnCheckRadiusPirate rỗng
                return;
            }

            int randomIndex = Random.Range(0, zombieSpawnInfo.spawnCheckRadiusPirate.Count);
            Transform spawnPoint = zombieSpawnInfo.spawnCheckRadiusPirate[randomIndex];
            Vector3 randomOffset = Random.insideUnitCircle * spawnPoint.localScale.x;
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            GameObject enemy = PhotonNetwork.Instantiate(zombieSpawnInfo.prefabPath, spawnPosition, Quaternion.identity);
            zombieSpawnInfo.currentEnemies.Add(enemy);
        }
    }
    void TrySpawnZombie()
    {
        TrySpawn(zombieSpawnInfo);
    }
    public FPSController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId); 
    }

    public FPSController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }
}