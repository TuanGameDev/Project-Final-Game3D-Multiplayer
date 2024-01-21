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
    [SerializeField] private string playerName;
    [SerializeField] private GameObject nameInput;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject createRoomSreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject lobbyBrowserScreen;
    [SerializeField] private GameObject selectCharPopup;
    [Header("Main Screen")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button findRoomButton;
    [SerializeField] private Button selectChar;
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
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;
        selectChar.interactable = false;
        Cursor.lockState = CursorLockMode.None;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }
    public void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        createRoomSreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        screen.SetActive(true);
        if (screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }
    public void OnBackToMainSreen()
    {
        SetScreen(mainScreen);
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
    public void OnScreenRoomButton()
    {
        if(playerName.Length<2)
        {
            return;
        }
        else
        {
            SetScreen(createRoomSreen);
        }
    }
    public void OnFindRoomButton()
    {
        if (playerName.Length < 2)
        {
            return;
        }
        else
        {
            SetScreen(lobbyBrowserScreen);
        }
    }
    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        if (roomNameInput.text.Length < 2)
            return;
        else
        NetWorkManager._networkmanager.CreateRoom(roomNameInput.text);
    }
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
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
        NetWorkManager._networkmanager.photonView.RPC("ChangeScene", RpcTarget.All,(object)"Death Village");
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
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
        NetWorkManager._networkmanager.JoinRoom(roomName);
    }
    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
        selectChar.interactable = true;
    }
}
