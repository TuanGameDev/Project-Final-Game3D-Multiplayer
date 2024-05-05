using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.AI;

public class ZombieTest : MonoBehaviourPun
{
    public int damage;
    public float chaseRange;
    public float attackRange;
    public float attackRate;
    public float lastAttackTime;
    public PlayerController targetPlayer;
    public PlayerController[] playerInScene;
    private NavMeshAgent agent;
    void Start()
    {
        GetReferences();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate)
            {
                Attack();
                agent.speed = 0;
            }
        }
        MoveToTarget();
    }
    void Attack()
    {
        lastAttackTime = Time.time;
        targetPlayer.TakeDamage(damage);
        Debug.Log("Enemy Attack");
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
            }
            else if (distanceToPlayer > chaseRange)
            {
                targetPlayer = null;
                agent.speed = 2;
            }
        }
    }
    private void GetReferences()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
