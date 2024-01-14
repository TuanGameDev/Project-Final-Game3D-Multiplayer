using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class AIZombie : MonoBehaviourPun
{
    [Header("Enemy Status")]
    public string dead = "Death";
    public int damage;
    public string enemyName;
    public float moveSpeed;
    public float currentHP;
    public float maxHP;
    public float chaseRange;
    public float attackRange;
    public float playerdetectRate;
    private float lastPlayerDetectTime;
    public float attackrate;
    private float lastattackTime;
    public Rigidbody rb;
    private FPSController[] playerInScene;
    private FPSController targetPlayer;
    private void Start()
    {
        EnemyStatusInfo(maxHP);

    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (targetPlayer != null)
        {
            float dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            float face = targetPlayer.transform.position.x - transform.position.x;

            if (face > 0)
            {
               // photonView.RPC("FlipLeft", RpcTarget.All);
            }
            else
            {
               // photonView.RPC("FlipRight", RpcTarget.All);
            }

            if (dist < attackRange && Time.time - lastattackTime >= attackrate)
            {
                Attack();
            }
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rb.velocity = dir.normalized * moveSpeed;
               // aim.SetBool("Move", true);
            }
            else
            {
                rb.velocity = Vector2.zero;
               // aim.SetBool("Move", false);
            }
        }

        DetectPlayer();
    }
    void Attack()
    {
      //  aim.SetTrigger("Attack");
        lastattackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
        Debug.Log("Attack");
    }

    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerdetectRate)
        {
            lastPlayerDetectTime = Time.time;
            playerInScene = FindObjectsOfType<FPSController>();
            foreach (FPSController player in playerInScene)
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        targetPlayer = null;
                      //  aim.SetBool("Move", false);
                        rb.velocity = Vector2.zero;
                    }
                }
                else if (dist < chaseRange)
                {
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                    }
                }
            }
        }
    }
    [PunRPC]
    public void TakeDamage(float damageAmount)
    {
        if (!photonView.IsMine)
            return;

        currentHP -= damageAmount;
        photonView.RPC("EnemyStatusInfo", RpcTarget.All, currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    [PunRPC]
    public void EnemyStatusInfo(float maxVal)
    {
        currentHP = maxVal;
    }
    void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Instantiate(dead, transform.position, Quaternion.identity);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
