using UnityEngine;
using TMPro;
using Photon.Pun;

public class Quest_Mission1 : MonoBehaviourPunCallbacks
{
    public GameObject FindPainKiller, FindLockPick, FindUnlock_Location;
    public TextMeshProUGUI txtFindPainKiller;
    public int painKiller = 2;
    private int painKillerCount;

    private void Start()
    {
        painKillerCount = 0;
        UpdatePainKillerText();
    }

    [PunRPC]
    public void IncreasePainKillerCount()
    {
        painKillerCount++;
        UpdatePainKillerText();
    }

    [PunRPC]
    public void UpdatePainKillerText()
    {
        txtFindPainKiller.text = "Find PainKiller: " + painKillerCount + "/" + painKiller;

        if (painKillerCount >= painKiller)
        {
            if (FindPainKiller != null)
                FindPainKiller.SetActive(false);
            if (FindLockPick != null)
                FindLockPick.SetActive(true);
        }
    }
}
