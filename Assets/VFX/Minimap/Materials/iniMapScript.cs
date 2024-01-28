using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MiniMapScript : MonoBehaviourPunCallbacks
{
    public GameObject playerIconPrefab;
    private Transform[] playerIcons;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            InitializePlayerIcons();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            UpdatePlayerIcons();
        }
    }

    void InitializePlayerIcons()
    {
        // Instantiate player icons for each player in the room
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        playerIcons = new Transform[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(0f, 10f), 0f, Random.Range(0f, 10f));
            playerIcons[i] = Instantiate(playerIconPrefab, spawnPosition, Quaternion.identity).transform;
        }
    }

    void UpdatePlayerIcons()
    {
        // Update the positions of player icons based on the actual player positions
        for (int i = 0; i < playerIcons.Length; i++)
        {
            if (playerIcons[i] != null)
            {
                // Use the correct method or property to get the player's GameObject
                GameObject playerObject = GetPlayerObjectForIndex(i);

                if (playerObject != null)
                {
                    Vector3 playerWorldPosition = playerObject.transform.position;
                    Vector2 miniMapPosition = WorldToMiniMapPosition(playerWorldPosition);
                    playerIcons[i].GetComponent<RectTransform>().anchoredPosition = miniMapPosition;
                }
            }
        }
    }

    // Replace this with the correct method or property to get the player's GameObject
    GameObject GetPlayerObjectForIndex(int playerIndex)
    {
        // Implement your logic to get the player's GameObject based on the index
        // For example, you might want to use PhotonView.Find to find the player's object by their ID.
        // Note: This is a placeholder and needs to be replaced with your actual logic.
        return null;
    }

    Vector2 WorldToMiniMapPosition(Vector3 worldPosition)
    {
        // Convert world position to mini-map space
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        RectTransform miniMapRect = GetComponent<RectTransform>();
        Vector2 miniMapPosition = new Vector2(viewportPosition.x * miniMapRect.sizeDelta.x, viewportPosition.y * miniMapRect.sizeDelta.y);
        return miniMapPosition;
    }
}
