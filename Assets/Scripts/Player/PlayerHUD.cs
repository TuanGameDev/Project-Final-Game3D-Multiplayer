using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class PlayerHUD : MonoBehaviourPun
{
    public TextMeshProUGUI nametagText;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void UpdateNameTag(string name)
    {
        nametagText.text =name;
    }
}
