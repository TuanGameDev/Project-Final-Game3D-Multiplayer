using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviourPun
{
    public int id;
    public static PlayerTest me;
    public Player photonplayer;
    public float _health;
  /*  [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonplayer = player;
        GameManager.gamemanager.playerCtrl[id - 1] = this;
        if (player.IsLocal)
            me = this;
    }
    [PunRPC]
    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount;
        if (_health <= 0)
        {

        }
    }*/
}
