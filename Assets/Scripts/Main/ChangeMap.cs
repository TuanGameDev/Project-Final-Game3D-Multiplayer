using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMap : MonoBehaviour
{
    private int currentMapIndex;
    public TextMeshProUGUI currentMapText;
    public Image[] mapImages;

    private void Start()
    {
        currentMapIndex = 0;
        DisplayMapImage();
    }
    public void Update()
    {
        currentMapText.text = " MAP: " + currentMapIndex;
    }
    public void ChangeNext()
    {
        currentMapIndex = (currentMapIndex + 1) % mapImages.Length;
        DisplayMapImage();
    }

    public void ChangeBack()
    {
        currentMapIndex = (currentMapIndex - 1 + mapImages.Length) % mapImages.Length;
        DisplayMapImage();
    }

    private void DisplayMapImage()
    {
        for (int i = 0; i < mapImages.Length; i++)
        {
            if (i == currentMapIndex)
            {
                mapImages[i].gameObject.SetActive(true);
            }
            else
            {
                mapImages[i].gameObject.SetActive(false);
            }
        }
    }
}