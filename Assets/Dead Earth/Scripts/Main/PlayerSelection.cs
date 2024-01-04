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
    [Header("Coin/Diamond")]
    public int coin;
    public int diamond;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI diamondText;
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
        if (PlayerPrefs.HasKey("Coin"))
        {
            coin = PlayerPrefs.GetInt("Coin");
        }
        else
        {
            coin = 0;
        }
        if (PlayerPrefs.HasKey("Diamond"))
        {
            diamond = PlayerPrefs.GetInt("Diamond");
        }
        else
        {
            diamond = 0;
        }
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
    private void Update()
    {
        coinText.text = "" + coin.ToString("N0");
        diamondText.text = "" + diamond.ToString("N0");
    }
    public void IconClick(int characterIndex)
    {
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
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
    public void ChangeChar(int amount)
    {
        if (coin >= amount)
        {
            coin -= amount;
            PlayerPrefs.SetInt("Coin", coin);
            PlayerPrefs.DeleteKey("SelectedCharacter");
            iconPlayer.SetActive(true);
            changeButton.SetActive(false);
            messageText.text = "You have changed the character to -1000 Coin";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Not enough coins to change character!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
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
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("SelectedCharacter");
        PlayerPrefs.DeleteKey("Coin");
        PlayerPrefs.DeleteKey("Diamond");
        PlayerPrefs.DeleteKey("PlayerLevel");
        PlayerPrefs.DeleteKey("DamageMin");
        PlayerPrefs.DeleteKey("DamageMax");
        PlayerPrefs.DeleteKey("CurrentExp");
        PlayerPrefs.DeleteKey("MaxExp");
        PlayerPrefs.DeleteKey("maxHP");
        PlayerPrefs.DeleteKey("maxMP");
        PlayerPrefs.DeleteKey("DF");
        iconPlayer.SetActive(true);
        changeButton.SetActive(false);
    }
}