using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    private Rigidbody rb;
    [SerializeField] private float maxBulletDistance = 20f;
    private float currentSpeed = 0f;

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
            DestroyBullet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
