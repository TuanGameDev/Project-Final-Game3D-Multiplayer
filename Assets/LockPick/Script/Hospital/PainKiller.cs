using Photon.Pun;
using UnityEngine;

public class PainKiller : MonoBehaviourPunCallbacks
{
    public GameObject paneEquip;
    public Highlight highlight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
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
