using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair_Ship_Equip : MonoBehaviourPun
{
    public GameObject paneEquip;
    public GameObject isPickUp;
    public Highlight highlight;
    private bool hasBeenPickedUp = false;

    public void PickUp()
    {
        hasBeenPickedUp = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine && !hasBeenPickedUp)
        {
            paneEquip.SetActive(true);
            if (highlight != null)
            {
                highlight.ToggleHighlight(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!hasBeenPickedUp)
            {
                paneEquip.SetActive(false);
            }
            if (highlight != null)
            {
                highlight.ToggleHighlight(false);
            }
        }
    }

    public void ShowPickUpPanel()
    {
        if (isPickUp != null)
        {
            isPickUp.SetActive(true);
            paneEquip.SetActive(false);
            StartCoroutine(TimeShow());
        }
    }

    IEnumerator TimeShow()
    {
        paneEquip.SetActive(false);
        yield return new WaitForSeconds(2f);
        isPickUp.SetActive(false);
    }
}
