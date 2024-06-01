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
    public PlayerController[] playerCtrl;
    public Transform[] spawnPoint;
    public float respawnTime;
    public int playersInGame;

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
        playerCtrl = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void ImInGame()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
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
    public PlayerController GetPlayer(int playerId)
    {
        return playerCtrl.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return playerCtrl.First(x => x.gameObject == playerObject);
    }
}
