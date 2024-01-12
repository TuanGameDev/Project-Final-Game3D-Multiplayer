using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviourPun
{
    public int currentHP;
    public int maxHP;
    private float maxHealthValue;
    private void Start()
    {
        EnemyStatusInfo(maxHP);

    }
    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void EnemyStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
