using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Sreens")]
    public int maxPlayers = 0;
    [SerializeField] private string playerName;
    [SerializeField] private GameObject nameInput;
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject createRoomSreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject lobbyBrowserScreen;
    [Header("Main Screen")]
    [SerializeField] private Button playermultiplayerButton;
    [SerializeField] private Button findRoomButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private TextMeshProUGUI maxPlayersText;
    [Header("Lobby")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private Button startGameButton;
    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefabs;
    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();
    void Start()
    {
        playermultiplayerButton.interactable = false;
        findRoomButton.interactable = false;
        settingButton.interactable = false;
        exitGameButton.interactable = false;
        Cursor.lockState = CursorLockMode.None;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Conecting...");
    }
    public void Update()
    {
        maxPlayersText.text = " TEAM MODES: " + maxPlayers + " Player ";
    }
    public void OnBackToMainSreen()
    {
        mainScreen.SetActive(true);
        lobbyScreen.SetActive(false);
        createRoomSreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
    }
    public void OnCreateRoomSreen()
    {
        createRoomSreen.SetActive(true);
        mainScreen.SetActive(false);
    }
    public void OnPlayerNameChanged(TMP_InputField playerNameInput)
    {
        playerName = playerNameInput.text;
        int maxNameLength = 5;
        if (playerName.Length > maxNameLength)
        {
            playerName = playerName.Substring(0, maxNameLength);
            playerNameInput.text = playerName;
        }
        PhotonNetwork.NickName = playerName;
    }
    public void OnFindRoomButton()
    {
        lobbyBrowserScreen.SetActive(true);
        mainScreen.SetActive(false);
    }
    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        if (roomNameInput.text.Length < 2)
            return;
        else
            CreateRoom(roomNameInput.text);
    }
    public override void OnJoinedRoom()
    {
        lobbyScreen.SetActive(true);
        createRoomSreen.SetActive(false);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }
    [PunRPC]
    void UpdateLobbyUI()
    {
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";
        roomInfoText.text = "<b> Room Name </b> \n" + PhotonNetwork.CurrentRoom.Name;
    }
    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        photonView.RPC("ChangeScene", RpcTarget.All,"Scene Test");
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        mainScreen.SetActive(true);
        lobbyScreen.SetActive(false);
    }
    GameObject CreateRoomButton()
    {
        GameObject buttonObject = Instantiate(roomButtonPrefabs, roomListContainer.transform);
        roomButtons.Add(buttonObject);
        return buttonObject;
    }
    void UpdateLobbyBrowserUI()
    {
        foreach (GameObject button in roomButtons)
        {
            button.SetActive(false);
        }
        for (int x = 0; x < roomList.Count; x++)
        {
            GameObject button = x >= roomButtons.Count ? CreateRoomButton() : roomButtons[x];
            button.SetActive(true);
            button.transform.Find("Room Name Text").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("Player Counter Text").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + "/" + roomList[x].MaxPlayers;

            Button buttoncomp = button.GetComponent<Button>();
            string roomName = roomList[x].Name;
            buttoncomp.onClick.RemoveAllListeners();
            buttoncomp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });
        }
    }
    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }
    public void OnJoinRoomButton(string roomName)
    {
        JoinRoom(roomName);
    }
    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
    public override void OnConnectedToMaster()
    {
        playermultiplayerButton.interactable = true;
        findRoomButton.interactable = true;
        settingButton.interactable = true;
        exitGameButton.interactable = true;
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
    #region Team Modes
    public void Solo(int value)
    {
        maxPlayers = (int)value;
    }
    public void Dou(int value)
    {
        maxPlayers = (int)value;
    }
    public void Squad(int value)
    {
        maxPlayers = (int)value;
    }
    #endregion
}
