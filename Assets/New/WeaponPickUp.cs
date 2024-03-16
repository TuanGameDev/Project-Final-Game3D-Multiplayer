using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPickUp : MonoBehaviour
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        activeWeapon = other.gameObject.GetComponent<PlayerController>();
        if (activeWeapon)
        {
            canPickUp = true; // Đặt trạng thái có thể nhặt vũ khí thành true
            activeWeapon.pickupText.gameObject.SetActive(true);
            activeWeapon.pickupText.text = "Take " + weaponFab.weaponName;
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

    private void PickUpWeapon()
    {
        Gun newWeapon = Instantiate(weaponFab);
        newWeapon._canpickup = false;
        PlayerController playerController = FindObjectOfType<PlayerController>(); // Tìm người chơi trong cảnh
        playerController.EquipWeapon(newWeapon);

        Destroy(gameObject);
    }
}
