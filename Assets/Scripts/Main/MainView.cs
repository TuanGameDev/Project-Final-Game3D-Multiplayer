using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : MonoBehaviour
{
    public GameObject mainsreenPopup;
    public GameObject loadingPopup;
    public void ShowmainsreenPopup()
    {
        mainsreenPopup.SetActive(true);
        loadingPopup.SetActive(false);
    }
}
