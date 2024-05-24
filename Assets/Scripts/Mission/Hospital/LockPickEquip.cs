using Photon.Pun;
using UnityEngine;

public class LockPickEquip : MonoBehaviourPunCallbacks
{
    public GameObject paneEquip;
    public Highlight highlight;
    private bool hasBeenPickedUp = false;

    public void PickUp()
    {
        hasBeenPickedUp = true;
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
            paneEquip.SetActive(false);
            if (highlight != null)
            {
                highlight.ToggleHighlight(false);
            }
        }
    }
}
