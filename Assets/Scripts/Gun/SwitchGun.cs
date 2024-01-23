using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGun : MonoBehaviourPunCallbacks, IPunObservable
{
    public static SwitchGun instance;
    public int selectedWeapon = 0;
    public float switchCooldown = 0.1f;
    public bool canSwitch = true;
    private float lastSwitchTime;
    private bool gunActivated = true;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            int previousSelectedWeapon = selectedWeapon;

            if (canSwitch)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    SwitchWeapon();
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2))
                {
                    SwitchWeapon();
                }
            }

            if (!canSwitch && Time.time >= lastSwitchTime + switchCooldown)
            {
                canSwitch = true;
            }
        }
    }

    void SwitchWeapon()
    {
        if (photonView.IsMine)
        {
            int previousSelectedWeapon = selectedWeapon;

            int childCount = transform.childCount;

            if (childCount == 0)
            {
                Debug.LogWarning("No weapons available!");
                return;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                selectedWeapon = (selectedWeapon + 1) % childCount;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                selectedWeapon = (selectedWeapon - 1 + childCount) % childCount;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectWeaponWithTag("Rifle");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && childCount >= 2)
            {
                SelectWeaponWithTag("Pistol");
            }

            if (previousSelectedWeapon != selectedWeapon)
            {
                lastSwitchTime = Time.time;
                canSwitch = false;
                GameObject newGun = transform.GetChild(selectedWeapon).gameObject;

                if (newGun != null)
                {
                    gunActivated = true;
                    FPSController.me.UpdateSelectedGun(newGun);
                    if (newGun.CompareTag("Rifle"))
                    {
                        FPSController.me.rifleIconImage.sprite = newGun.GetComponent<Gun_Shoot>().icon;
                        FPSController.me.rifleIconImage.gameObject.SetActive(true);
                        FPSController.me.pistolIconImage.gameObject.SetActive(false);
                    }
                    else if (newGun.CompareTag("Pistol"))
                    {
                        FPSController.me.pistolIconImage.sprite = newGun.GetComponent<Gun_Shoot>().icon;
                        FPSController.me.pistolIconImage.gameObject.SetActive(true);
                        FPSController.me.rifleIconImage.gameObject.SetActive(false);
                    }
                    if (previousSelectedWeapon != -1)
                    {
                        ToggleMuzzle(false, previousSelectedWeapon);
                    }
                }
                else
                {
                    gunActivated = false;
                }

                SelectWeapon();
            }          
        }
    }
    void ToggleMuzzle(bool toggle, int weaponIndex)
    {
        Transform weaponToToggle = transform.GetChild(weaponIndex);
        if (weaponToToggle != null)
        {
            Gun_Shoot gunScript = weaponToToggle.GetComponent<Gun_Shoot>();
            if (gunScript != null)
            {
                gunScript.ToggleMuzzle(toggle);
            }
        }
    }
    void SelectWeaponWithTag(string weaponTag)
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (weapon.CompareTag(weaponTag))
            {
                selectedWeapon = i;
                break;
            }
            i++;
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(selectedWeapon);
        }
        else
        {
            selectedWeapon = (int)stream.ReceiveNext();
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                if (gunActivated)
                {
                    weapon.gameObject.SetActive(true);
                }
                else
                {
                    weapon.gameObject.SetActive(false);
                }
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
