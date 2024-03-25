using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gun;

public class Items : MonoBehaviour
{
    public enum ItemsType
    {
        RifleAmmo,
        PistolAmmo,
        Bandage,
        FirstAidKit
    }
    public ItemsType item;

    public string nameofitem;
    public int value;
    PlayerController playerActive;
    bool _canPickup;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canPickup)
        {
            PickUp();
            playerActive.pickupText.gameObject.SetActive(false);
        }
    }

    void PickUp()
    {
        Debug.Log("You take: " + nameofitem);
    }
    private void OnTriggerEnter(Collider other)
    {
        playerActive = other.gameObject.GetComponent<PlayerController>();
        if (playerActive != null)
        {
            _canPickup = true;
            playerActive.pickupText.gameObject.SetActive(true);
            playerActive.pickupText.text = " Press F To Pick Up: " + nameofitem;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerActive = other.gameObject.GetComponent<PlayerController>();
        if (playerActive != null)
        {
            _canPickup = false;
            playerActive.pickupText.gameObject.SetActive(false);
        }
    }
}
