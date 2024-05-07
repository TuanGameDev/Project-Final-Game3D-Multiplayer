using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorLockPick : MonoBehaviour
{
    public static DoorLockPick instance;
    public GameObject panelLockPick;
    public GameObject panelWarning;
    public GameObject minigameLockPick;
    private PlayerEquipLockPick playerEquipLockPick;
    private PlayerController playerController;
    public GameObject lock1;
    public GameObject lock2;
    public GameObject cameraLockpick;
    public GameObject panelWin, panelBreak;
    private bool isInMiniGame = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerEquipLockPick != null && playerEquipLockPick.HasLockPick() && !isInMiniGame)
        {
            OpenMiniGame();


        }
        if (Input.GetKeyDown(KeyCode.Escape) && isInMiniGame)
        {
            CloseMiniGame();

        }
    }

    private void OpenMiniGame()
    {
        if (minigameLockPick != null)
        {
            minigameLockPick.SetActive(true);
            playerController.enabled = false;
            lock1.SetActive(false);
            lock2.SetActive(true);
            cameraLockpick.SetActive(true);
            isInMiniGame = true;
        }
    }

    public void CloseMiniGame()
    {
        if (minigameLockPick != null)
        {
            minigameLockPick.SetActive(false);
            playerController.enabled = true;
            isInMiniGame = false;
            lock1.SetActive(true);
            lock2.SetActive(false);
            cameraLockpick.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEquipLockPick = other.GetComponent<PlayerEquipLockPick>();
            playerController = other.GetComponent<PlayerController>();
            if (playerEquipLockPick != null && playerController != null)
            {
                if (playerEquipLockPick.HasLockPick())
                {
                    panelLockPick.SetActive(true);
                    panelWarning.SetActive(false);
                }
                else
                {
                    panelLockPick.SetActive(false);
                    panelWarning.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEquipLockPick = null;
            playerController = null;
            panelLockPick.SetActive(false);
            panelWarning.SetActive(false);
        }
    }
}