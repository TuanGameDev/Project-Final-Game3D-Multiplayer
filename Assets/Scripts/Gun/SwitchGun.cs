using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGun : MonoBehaviourPunCallbacks, IPunObservable
{
    public int selectedWeapon = 0;
    public float switchCooldown = 0.1f;
    private bool canSwitch = true;
    private float lastSwitchTime;
    private bool gunActivated = true;
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

                if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) || (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3))
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
                selectedWeapon = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && childCount >= 2)
            {
                selectedWeapon = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && childCount >= 3)
            {
                selectedWeapon = 2;
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
                }
                else
                {
                    gunActivated = false;
                }

                SelectWeapon();
            }
            photonView.RPC("SyncSelectedWeapon", RpcTarget.Others, selectedWeapon);
        }
    }

    [PunRPC]
    void SyncSelectedWeapon(int newSelectedWeapon)
    {
        selectedWeapon = newSelectedWeapon;
        SelectWeapon();
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
