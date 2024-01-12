using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float maxBulletDistance = 20f;

    private void Update()
    {
        MoveBullet();
        CheckMaxDistance();
    }

    private void MoveBullet()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    private void CheckMaxDistance()
    {
        if (Vector3.Distance(transform.position, transform.position + transform.forward * bulletSpeed * Time.deltaTime) > maxBulletDistance)
        {
            DestroyBullet();
        }
    }

    [PunRPC]
    private void DestroyBullet()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
