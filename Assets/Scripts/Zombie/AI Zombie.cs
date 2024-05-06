using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Unity.Burst.Intrinsics;

public class AIZombie : MonoBehaviourPun
{
    [Header("Enemy")]
    public string enemyName;
    public string dead = "Death";
    [Header("Tấn công và Di chuyển")]
    public float damage;
    public float chaseRange;
    public float attackRange;
    public float attackRate;
    private bool isPlayerDetected = false;
    public int curAttackerID;
    private float lastAttackTime;
    [Header("Máu")]
    public float currentHP;
    public float maxHP;
    public Animator anim;
    public Rigidbody rb;
    public Rigidbody[] _ragdollRigibodies;
    private Coroutine moveCoroutine;
    public PlayerController targetPlayer;
    public PlayerController[] playerInScene;
    public NavMeshAgent agent;
    private void Start()
    {
        UpdateHealth(maxHP);
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
                anim.SetBool("Move", false);
            }
        }

        if (isPlayerDetected)
        {
            agent.speed = 1;
        }
        else
        {
            agent.speed = 0;
        }

        MoveToTarget();
    }
    #region HEALTH + ATTACK + MOVE
    private void MoveToTarget()
    {
        playerInScene = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in playerInScene)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < chaseRange)
            {
                targetPlayer = player;
                agent.SetDestination(targetPlayer.transform.position);
                anim.SetBool("Move", true);
                isPlayerDetected = true;
            }
            else if (player == targetPlayer)
            {
                if (distanceToPlayer > chaseRange)
                {
                    targetPlayer = null;
                    anim.SetBool("Move", false);
                    isPlayerDetected = false;
                }
            }
        }
    }
    void Attack()
    {
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
        photonView.RPC("TriggerAttackAnimation", RpcTarget.All);
    }
    [PunRPC]
    void TriggerAttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        currentHP -= damageAmount;
        curAttackerID = attackerId;
        photonView.RPC("UpdateHealth", RpcTarget.All, currentHP);
        if (currentHP <= 0)
        {
            Die();
            photonView.RPC("EnableRagdoll", RpcTarget.All);
        }
    }

    [PunRPC]
    public void UpdateHealth(float maxVal)
    {
        currentHP = maxVal;
    }
    #endregion
    #region RAGDOLLRIGIBODIES
    private void DisableRagdoll()
    {
        foreach(var rigibody in _ragdollRigibodies)
        {
            rigibody.isKinematic = true;
        }
    }
    void Die()
    {
        PlayerController player = GameManager.gamemanager.GetPlayer(curAttackerID);
        PhotonNetwork.Instantiate(dead, transform.position, Quaternion.identity);
    }
    [PunRPC]
    void EnableRagdoll()
    {
        foreach (var rigibody in _ragdollRigibodies)
        {
            rigibody.isKinematic = false;
        }
        anim.enabled = false;
        targetPlayer = null;
        isPlayerDetected = false;
        attackRange = 0;
        rb.useGravity = false;
        chaseRange = 0;
        Destroy(gameObject, 3);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Flash Light")
        {
            Debug.Log("Phát Hiện Flash");
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                targetPlayer = player;
                agent.SetDestination(targetPlayer.transform.position);
            }
        }
    }
}