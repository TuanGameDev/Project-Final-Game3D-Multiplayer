using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerSelection : MonoBehaviour
{
    [Header("Select Player")]
    public GameObject iconPlayer;

    [Header("Popup UI")]
    public GameObject PlayerPopup;
    public GameObject iconPlayerPopup;
    public GameObject selectButton;
    public GameObject playerselectionPopup;


    public string playerPrefabName;
    public GameObject[] playerModel;
    public int selectedCharacter=0;
    public static PlayerSelection playerselection;

    private void Awake()
    {
        playerselection = this;
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
        PlayerPopup.SetActive(false);
    }
    public void ShowPlayerSelection()
    {
        playerselectionPopup.SetActive(true);
    }
    public void ChooseChacter()
    {
        iconPlayerPopup.SetActive(true);
        PlayerPopup.SetActive(true);
    }
}