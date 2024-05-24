using System.Collections;
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
    public bool isUnLocked = false;
    public GameObject panelOpen;
    public GameObject panelClose;
    public GameObject door;
    private bool isDoorOpen = false;
    private bool isPlayerNearDoor = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (!isUnLocked)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerEquipLockPick != null && playerEquipLockPick.HasLockPick() && !isInMiniGame && isPlayerNearDoor)
            {
                OpenMiniGame();
            }
            if (Input.GetKeyDown(KeyCode.Escape) && isInMiniGame)
            {
                CloseMiniGame();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && isPlayerNearDoor)
            {
                ToggleDoor();
            }
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
        if (minigameLockPick != null && playerController != null)
        {
            minigameLockPick.SetActive(false);
            playerController.enabled = true;
            isInMiniGame = false;
            lock1.SetActive(true);
            lock2.SetActive(false);
            cameraLockpick.SetActive(false);
        }
    }


    private void ToggleDoor()
    {
        if (!isInMiniGame)
        {
            if (!isDoorOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else
            {
                StartCoroutine(CloseDoor());
            }
        }
    }


    private IEnumerator OpenDoor()
    {
        float timer = 0f;
        float openTime = 1.5f;
        Quaternion startRotation = door.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, -90f, 0f);

        while (timer < openTime)
        {
            timer += Time.deltaTime;
            panelOpen.SetActive(false);
            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timer / openTime);
            yield return null;
        }

        isDoorOpen = true;
    }

    private IEnumerator CloseDoor()
    {
        float timer = 0f;
        float closeTime = 1.5f;
        Quaternion startRotation = door.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);

        while (timer < closeTime)
        {
            timer += Time.deltaTime;
            panelClose.SetActive(false);
            door.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timer / closeTime);
            yield return null;
        }

        isDoorOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearDoor = true;
            playerEquipLockPick = other.GetComponent<PlayerEquipLockPick>();
            playerController = other.GetComponent<PlayerController>();
            if (playerEquipLockPick != null && playerController != null)
            {
                if (!isUnLocked)
                {
                    if (playerEquipLockPick.HasLockPick())
                    {
                        panelLockPick.SetActive(true);
                        panelWarning.SetActive(false);
                        panelOpen.SetActive(false);
                        panelClose.SetActive(false);
                    }
                    else
                    {
                        panelLockPick.SetActive(false);
                        panelWarning.SetActive(true);
                        panelOpen.SetActive(false);
                        panelClose.SetActive(false);
                    }
                }
                else if (!isDoorOpen)
                {
                    panelLockPick.SetActive(false);
                    panelWarning.SetActive(false);
                    panelOpen.SetActive(true);
                    panelClose.SetActive(false);
                }
                else
                {
                    panelLockPick.SetActive(false);
                    panelWarning.SetActive(false);
                    panelOpen.SetActive(false);
                    panelClose.SetActive(true);
                }


            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearDoor = false;
            playerEquipLockPick = null;
            playerController = null;
            panelLockPick.SetActive(false);
            panelWarning.SetActive(false);
            panelOpen.SetActive(false);
            panelClose.SetActive(false);
        }
    }
}
