using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MiniMapScript : MonoBehaviourPun
{
    public Camera cameraMinimap;
    private Canvas canvas;
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        if (!photonView.IsMine)
        {
            canvas.enabled = false;
            cameraMinimap.gameObject.SetActive(false);
        }
    }
}
