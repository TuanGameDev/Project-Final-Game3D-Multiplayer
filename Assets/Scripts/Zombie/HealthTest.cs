using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviourPun
{
    public float currentHP;
    public float maxHP;
    private float maxHealthValue;
    private void Start()
    {
        EnemyStatusInfo(maxHP);

    }
    [PunRPC]
    public void TakeDamage(float damageAmount)
    {
        currentHP -= damageAmount;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void EnemyStatusInfo(float maxVal)
    {
        maxHealthValue = maxVal;
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
