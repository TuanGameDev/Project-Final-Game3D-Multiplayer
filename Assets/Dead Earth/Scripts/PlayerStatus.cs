using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviourPun
{
    [Header("UI")]
    public TextMeshProUGUI playerName;
    public void InitializedPlayer(string text)
    {
        playerName.text = text;
    }
}
