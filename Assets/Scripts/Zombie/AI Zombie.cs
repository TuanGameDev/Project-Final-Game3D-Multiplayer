using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

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
    private Animator anim;
    private Rigidbody rb;
    private Coroutine moveCoroutine;
    private PlayerController targetPlayer;
    private PlayerController[] playerInScene;
    private NavMeshAgent agent;
    private void Start()
    {
        UpdateHealth(maxHP);
        GetReferences();
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
            else if (distanceToPlayer > chaseRange)
            {
                anim.SetBool("Move", false);
                isPlayerDetected = false;
                targetPlayer = null;
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        lastAttackTime = Time.time;
        targetPlayer.photonView.RPC("TakeDamage", RpcTarget.All, damage);
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
        }
    }

    [PunRPC]
    public void UpdateHealth(float maxVal)
    {
        currentHP = maxVal;
    }

    void Die()
    {
        PlayerController player = GameManager.gamemanager.GetPlayer(curAttackerID);
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
    private void GetReferences()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
}