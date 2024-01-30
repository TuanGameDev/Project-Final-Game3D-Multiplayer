using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class AIZombie : MonoBehaviourPun
{
    [Header("Enemy Status")]
   [SerializeField] public string dead = "Death";
   [SerializeField] public int damage;
   [SerializeField] public string enemyName;
   [SerializeField] public float moveSpeed;
   [SerializeField] public float currentHP;
   [SerializeField] public float maxHP;
   [SerializeField] public float chaseRange;
   [SerializeField] public float attackRange;
   [SerializeField] public float playerDetectRate;
    private float lastPlayerDetectTime;
   [SerializeField] public float attackRate;
    private float lastAttackTime;
    public Animator aim;
    public Rigidbody rb;
    private Coroutine moveCoroutine;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Flash Light")
        {
            Debug.Log("Phát Hiện Flash");
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            FPSController player = other.GetComponentInParent<FPSController>();
            if (player != null)
            {
                targetPlayer = player;
                Vector3 direction = targetPlayer.transform.position - modelTransform.position;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                modelTransform.rotation = rotation;
                moveCoroutine = StartCoroutine(MoveToPlayer(player));
            }
        }
    }

    private IEnumerator MoveToPlayer(FPSController player)
    {
        while (true)
        {
            Vector3 direction = player.transform.position - transform.position;
            Vector3 moveVector = direction.normalized * moveSpeed;
            rb.velocity = moveVector;
            aim.SetBool("Move", true);
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= attackRange)
            {
                rb.velocity = Vector3.zero;
                aim.SetBool("Move", false);
                Attack();
                yield break;
            }

            yield return null;
        }
    }
}
