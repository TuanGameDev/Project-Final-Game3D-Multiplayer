using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabPath;
    public PlayerController[] players;
    public Transform[] spawnPoint;
    public float respawnTime;
    private int playersInGame;

    [Header("Spawn Enemy - Naval")]
    public EnemySpawnInfo enemyNavalSpawnInfo;

    [Header("Spawn Enemy - Pirate")]
    public EnemySpawnInfo enemyPirateSpawnInfo;

    [Header("Boss Smoker")]
    public BossInfo bossSmoker;

    [Header("Boss Enel")]
    public BossInfo bossEnel;

    [Header("Boss Aokiji")]
    public BossInfo bossAokiji;

    [Header("Boss Kizaru")]
    public BossInfo bossKizaru;

    [Header("Boss Akainu")]
    public BossInfo bossAkainu;

    [System.Serializable]
    public class BossInfo
    {
        public float maxBoss;
        public string bossPrefabPath;
        public Transform spawnbossCheckRadius;
        public List<GameObject> currentBoss = new List<GameObject>();
    }
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public string prefabPath;
        public float maxEnemies;
        public float spawnCheckRadius;
        public List<Transform> spawnCheckRadiusPirate = new List<Transform>();
        public List<GameObject> currentEnemies = new List<GameObject>();
    }
    [Header("SpawnBossTime")]
    public float spawnbossCheckTime;
    private float lastSpawnbossCheckTime;
    [Header("SpawnEnemyTime")]
    public float spawnenemyCheckTime;
    private float lastSpawnenemyCheckTime;
    [Header("UI Text")]
    public TextMeshProUGUI bossAppearText;
    public TextMeshProUGUI bossDeathText;

    public static GameManager gamemanager;
    private void Awake()
    {
        gamemanager = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (Time.time - lastSpawnenemyCheckTime > spawnenemyCheckTime)
        {
            lastSpawnenemyCheckTime = Time.time;
            TrySpawnEnemy_Naval();
            TrySpawnEnemy_Pirate();
        }
        if (Time.time - lastSpawnbossCheckTime > spawnbossCheckTime)
        {
            lastSpawnbossCheckTime = Time.time;
            TrySpawnBoss_Smoker(bossSmoker.spawnbossCheckRadius);
            TrySpawnBoss_Enel(bossEnel.spawnbossCheckRadius);
            TrySpawnBoss_Aokiji(bossAokiji.spawnbossCheckRadius);
            TrySpawnBoss_Kizaru(bossKizaru.spawnbossCheckRadius);
            TrySpawnBoss_Akainu(bossAkainu.spawnbossCheckRadius);
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
    #region SpawnBoss
    void TrySpawnEnemy(EnemySpawnInfo enemySpawnInfo)
    {
        for (int x = enemySpawnInfo.currentEnemies.Count - 1; x >= 0; x--)
        {
            if (enemySpawnInfo.currentEnemies[x] == null)
            {
                enemySpawnInfo.currentEnemies.RemoveAt(x);
            }
        }

        if (enemySpawnInfo.currentEnemies.Count >= enemySpawnInfo.maxEnemies)
            return;

        if (enemySpawnInfo == enemyNavalSpawnInfo)
        {
            Vector3 randomIncircle = Random.insideUnitCircle * enemySpawnInfo.spawnCheckRadius;
            GameObject enemy = PhotonNetwork.Instantiate(enemySpawnInfo.prefabPath, transform.position + randomIncircle, Quaternion.identity);
            enemySpawnInfo.currentEnemies.Add(enemy);
        }
        else if (enemySpawnInfo == enemyPirateSpawnInfo)
        {
            if (enemySpawnInfo.spawnCheckRadiusPirate.Count == 0)
            {
                // Thực hiện xử lý khi danh sách spawnCheckRadiusPirate rỗng
                return;
            }

            int randomIndex = Random.Range(0, enemySpawnInfo.spawnCheckRadiusPirate.Count);
            Transform spawnPoint = enemySpawnInfo.spawnCheckRadiusPirate[randomIndex];
            Vector3 randomOffset = Random.insideUnitCircle * spawnPoint.localScale.x;
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            GameObject enemy = PhotonNetwork.Instantiate(enemySpawnInfo.prefabPath, spawnPosition, Quaternion.identity);
            enemySpawnInfo.currentEnemies.Add(enemy);
        }
    }

    void TrySpawnEnemy_Naval()
    {
        TrySpawnEnemy(enemyNavalSpawnInfo);
    }

    void TrySpawnEnemy_Pirate()
    {
        TrySpawnEnemy(enemyPirateSpawnInfo);
    }
    void TrySpawnBoss(BossInfo bossInfo, Transform spawnBoss)
    {
        for (int x = bossInfo.currentBoss.Count - 1; x >= 0; x--)
        {
            if (bossInfo.currentBoss[x] == null)
            {
                bossInfo.currentBoss.RemoveAt(x);
            }
        }

        if (bossInfo.currentBoss.Count >= bossInfo.maxBoss)
            return;

        GameObject boss = PhotonNetwork.Instantiate(bossInfo.bossPrefabPath, spawnBoss.position, Quaternion.identity);
        bossInfo.currentBoss.Add(boss);

        photonView.RPC("ShowBossAppearText", RpcTarget.All);
        StartCoroutine(WaitForBossDeath(boss));
    }

    void TrySpawnBoss_Smoker(Transform targetSmoker)
    {
        TrySpawnBoss(bossSmoker, targetSmoker);
    }

    void TrySpawnBoss_Enel(Transform spawnBoss)
    {
        TrySpawnBoss(bossEnel, spawnBoss);
    }

    void TrySpawnBoss_Aokiji(Transform spawnBoss)
    {
        TrySpawnBoss(bossAokiji, spawnBoss);
    }

    void TrySpawnBoss_Kizaru(Transform spawnBoss)
    {
        TrySpawnBoss(bossKizaru, spawnBoss);
    }

    void TrySpawnBoss_Akainu(Transform spawnBoss)
    {
        TrySpawnBoss(bossAkainu, spawnBoss);
    }
    [PunRPC]
    void ShowBossAppearText()
    {
        bossAppearText.gameObject.SetActive(true);
        bossAppearText.text = "Boss has appeared!";
        bossDeathText.color = Color.yellow;
        StartCoroutine(HideTextAfterDelay(bossAppearText, 2f));
    }

    IEnumerator WaitForBossDeath(GameObject boss)
    {
        yield return new WaitUntil(() => boss == null);

        photonView.RPC("ShowBossDeathText", RpcTarget.All);
        StartCoroutine(HideTextAfterDelay(bossDeathText, 2f));

        yield return new WaitForSeconds(spawnbossCheckTime);

        TrySpawnBoss_Smoker(bossSmoker.spawnbossCheckRadius);
        TrySpawnBoss_Enel(bossEnel.spawnbossCheckRadius);
        TrySpawnBoss_Aokiji(bossAokiji.spawnbossCheckRadius);
        TrySpawnBoss_Kizaru(bossKizaru.spawnbossCheckRadius);
        TrySpawnBoss_Akainu(bossAkainu.spawnbossCheckRadius);
    }

    [PunRPC]
    void ShowBossDeathText()
    {
        bossDeathText.gameObject.SetActive(true);
        bossDeathText.text = "Boss has been defeated!";
        bossDeathText.color = Color.red;
        StartCoroutine(HideTextAfterDelay(bossDeathText, 2f));
    }

    IEnumerator HideTextAfterDelay(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);

        text.gameObject.SetActive(false);
    }
    #endregion
    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId); 
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }
}