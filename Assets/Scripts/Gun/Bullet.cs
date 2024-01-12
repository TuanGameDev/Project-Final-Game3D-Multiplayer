using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float maxBulletDistance = 20f;
    private Rigidbody rb;
    private float currentSpeed = 0f;
    private int attackerId;
    private bool isMine;
    public int damage;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        rb.velocity = transform.forward * currentSpeed;
        currentSpeed = bulletSpeed;
        if (Vector3.Distance(transform.position, transform.position + rb.velocity * Time.deltaTime) > maxBulletDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Zombie")
        {
            Debug.Log("Va cham voi zombie");

            AIZombie zombie = other.GetComponent<AIZombie>();
            zombie.photonView.RPC("TakeDamage", RpcTarget.MasterClient,this.attackerId, damage);
        }
    }
    public void Initialized(int attackId, bool isMine)
    {
        this.attackerId = attackId;
        this.isMine = isMine;
    }
}
