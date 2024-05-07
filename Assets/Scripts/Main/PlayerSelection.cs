using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
enum PlayerChoose
{

}
public class PlayerSelection : MonoBehaviour
{
    [Header("Select Player")]
    public GameObject iconPlayer;

    [Header("Popup UI")]
    public GameObject iconPlayerPopup;
    public GameObject playerselectionPopup;

    public TextMeshProUGUI choseCharterText;
    public string playerPrefabName;
    public GameObject[] playerModel;
    public int selectedCharacter=0;
    public static PlayerSelection playerselection;

    private void Awake()
    {
        playerselection = this;
    }
    private void Update()
    {
        choseCharterText.text = "CHOOSE CHARACTER: " + playerPrefabName;
    }
    public void IconClick(int characterIndex)
    {
        foreach (GameObject player in playerModel)
        {
            iconPlayerPopup.SetActive(false);
        }
        selectedCharacter = characterIndex;
        playerPrefabName = playerModel[selectedCharacter].GetComponent<PlayerModelName>().playerName;
        playerModel[selectedCharacter].SetActive(true);
    }
    public void ShowPlayerSelection()
    {
        playerselectionPopup.SetActive(true);
    }
    public void ChooseChacter()
    {
        iconPlayerPopup.SetActive(true);
    }
}