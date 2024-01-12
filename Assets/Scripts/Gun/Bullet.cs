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
        if (other.CompareTag("Zombie"))
        {
            Debug.Log("Va cham voi zombie");

            AIZombie zombie = other.GetComponent<AIZombie>();
            if (zombie != null)
            {
                // Thay đổi tên của phương thức RPC từ "TakeDamage" thành "TakeDamageWithAttackerId" (hoặc bạn có thể đặt tên khác)
                string rpcMethodName = "TakeDamage";
                int attackerId = this.attackerId; // Giá trị attackerId của bạn

                // Gọi RPC với các tham số mới bao gồm attackerId
                zombie.photonView.RPC(rpcMethodName, RpcTarget.MasterClient, attackerId, damage);
            }
        }
    }
    public void Initialized(int attackId, bool isMine)
    {
        this.attackerId = attackId;
        this.isMine = isMine;
    }
}
