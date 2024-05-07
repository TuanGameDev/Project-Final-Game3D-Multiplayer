using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickEquip : MonoBehaviour
{
    public GameObject paneEquip;
    public Highlight highlight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        if (other.CompareTag("Player"))
        {
            paneEquip.SetActive(false);
            if (highlight != null)
            {
                highlight.ToggleHighlight(false);
            }
        }
    }
}
