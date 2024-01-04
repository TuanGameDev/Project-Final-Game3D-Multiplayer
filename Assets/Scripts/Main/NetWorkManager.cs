using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 0;
    public static NetWorkManager _networkmanager;
    private void Awake()
    {
        if (_networkmanager != null && _networkmanager != this)
            gameObject.SetActive(false);
        else
        {
            _networkmanager = this;
            DontDestroyOnLoad(gameObject);
        }
    }
   private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conecting...");
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Conected...");

    }
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;
        PhotonNetwork.CreateRoom(roomName, options);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
