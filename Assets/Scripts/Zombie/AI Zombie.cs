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
    public float playerDetectRate;
    private float lastPlayerDetectTime;
    public float attackRate;
    private float lastAttackTime;
    public Animator aim;
    public Rigidbody rb;
    public Transform modelTransform;
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
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
            else if (dist > attackRange)
            {
                Vector3 dir = targetPlayer.transform.position - transform.position;
                rb.velocity = dir.normalized * moveSpeed;
                aim.SetBool("Move", true);
            }
            else
            {
                rb.velocity = Vector3.zero;
                aim.SetBool("Move", false);
            }
        }
        DetectPlayer();
    }

    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            FPSController[] playerInScene = FindObjectsOfType<FPSController>();
            foreach (FPSController player in playerInScene)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        targetPlayer = null;
                        aim.SetBool("Move", false);
                        rb.velocity = Vector3.zero;
                    }
                }
                else if (dist < chaseRange)
                {
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                    }
                    if (modelTransform != null)
                    {
                        Vector3 direction = targetPlayer.transform.position - modelTransform.position;
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                        modelTransform.rotation = rotation;
                    }
                }
            }
        }
    }
    void Attack()
    {
        aim.SetTrigger("Attack");
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
        Debug.Log("Attack");
    }
    [PunRPC]
    public void TakeDamage(float damageAmount)
    {
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
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Instantiate(dead, transform.position, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
