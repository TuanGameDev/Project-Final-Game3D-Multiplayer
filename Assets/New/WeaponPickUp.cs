using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponPickUp : MonoBehaviourPun
{
    [SerializeField] Gun weaponFab;
    PlayerController activeWeapon;
    // Thêm một biến bool để theo dõi trạng thái có thể nhặt vũ khí hay không
    private bool canPickUp = false;

    void Update()
    {
        // Nếu có thể nhặt vũ khí và người chơi nhấn phím F
        if (canPickUp && Input.GetKeyDown(KeyCode.F))
        {
            PickUpWeapon();
            activeWeapon.pickupText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            canPickUp = true; // Đặt trạng thái có thể nhặt vũ khí thành true
            activeWeapon.pickupText.gameObject.SetActive(true);
            activeWeapon.pickupText.text = " Press F To Pick Up: " + weaponFab.weaponName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            canPickUp = false; // Đặt trạng thái có thể nhặt vũ khí thành false khi không còn trong vùng nhặt
            activeWeapon.pickupText.gameObject.SetActive(false);
        }
    }
    void PickUpWeapon()
    {
        Gun newWeapon = Instantiate(weaponFab);
        newWeapon._canpickup = false;
        PlayerController[] playerInScene = FindObjectsOfType<PlayerController>(); // Tìm tất cả người chơi trong cảnh
        foreach (PlayerController playerController in playerInScene)
        {
            playerController.EquipWeapon(newWeapon);
            Debug.Log(playerController);
        }
        Destroy(gameObject);
    }
}
