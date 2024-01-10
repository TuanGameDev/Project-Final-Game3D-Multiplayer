using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class PlayerName : MonoBehaviourPun
{
    public TextMeshProUGUI nametagText;
    public void UpdateNameTag(string name)
    {
        nametagText.text =name;
    }
}
