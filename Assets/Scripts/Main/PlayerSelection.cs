using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum PlayerName
{
    Nick,
    Rochelle,
    Coach,
    Ellis
}
public class PlayerSelection : MonoBehaviour
{
    [Header("Select Player")]
    public GameObject iconPlayer;

    [Header("Popup UI")]
    public GameObject selectButton;
    public GameObject inputfielnamePopup;
    public GameObject playerselectionPopup;


    public string playerPrefabName;
    public GameObject[] playerModel;
    public int selectedCharacter=1;
    public TextMeshProUGUI SelectedCharactertext;
    public static PlayerSelection playerselection;

    private void Awake()
    {
        playerselection = this;
    }
    private void Update()
    {
        SelectedCharactertext.text = "SELECT PLAYER: " + ((PlayerName)selectedCharacter).ToString();
    }
    public void IconClick(int characterIndex)
    {
        foreach (GameObject player in playerModel)
        {
            player.SetActive(false);
        }
        selectButton.SetActive(true);
        selectedCharacter = characterIndex;
        playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        playerModel[selectedCharacter].SetActive(true);
    }
    public void SelectPlayer()
    {
        selectButton.SetActive(false);
        iconPlayer.SetActive(false);
        inputfielnamePopup.SetActive(true);
    }
    public void ShowPlayerSelection()
    {
        playerselectionPopup.SetActive(true);
    }
}