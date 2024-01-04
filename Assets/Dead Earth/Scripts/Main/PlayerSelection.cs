using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [Header("Select Player")]
    public GameObject selectButton;
    public GameObject creatRoomButton;
    public GameObject findRoomButton;
    public GameObject changeButton;
    public GameObject iconPlayer;
    public string playerPrefabName;
    public GameObject[] playerModel;
    public int selectedCharacter=0;
    public Button[] characterIcons;
    public string selectedCharacterName;
    public TextMeshProUGUI messageText;
    public static PlayerSelection playerselection;

    private void Awake()
    {
        playerselection = this;
    }

    void Start()
    {
        selectButton.SetActive(true);
        creatRoomButton.SetActive(false);
        findRoomButton.SetActive(false);
        if (PlayerPrefs.HasKey("SelectedCharacter"))
        {
            selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
            playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
            selectButton.SetActive(true);
            iconPlayer.SetActive(false);
        }
        else
        {
            selectedCharacter = 0;
            playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        }
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        playerModel[selectedCharacter].SetActive(true);
    }
    public void IconClick(int characterIndex)
    {
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        selectedCharacter = characterIndex;
        playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        playerModel[selectedCharacter].SetActive(true);
    }
    public void SelectChar()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    public void ShowuiRoom()
    {
        selectButton.SetActive(false);
        changeButton.SetActive(false);
        creatRoomButton.SetActive(true);
        findRoomButton.SetActive(true);
    }
}