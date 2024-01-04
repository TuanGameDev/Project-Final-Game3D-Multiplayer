using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviourPun
{
    [Header("UI")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerLevel;
    public void InitializedPlayer(string text, int level)
    {
        playerName.text = text;
        playerLevel.text = "Lv. " + level;
    }
    [PunRPC]
    void UpdatePlayerLevel(int value)
    {
        playerLevel.text = "Lv. " + value;
        playerLevel.color = Color.red;
    }
}
