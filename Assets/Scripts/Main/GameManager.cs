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
    public FPSController[] players;
    public Transform[] spawnPoint;
    public float respawnTime;
    private int playersInGame;

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
    public FPSController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId); 
    }

    public FPSController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }
}