using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] public float bulletDamage;
    private int attackerId;
    private bool isMine;

    private void Start()
    {
       // Invoke("DestroyBullet", destroyDelay);
    }

    private void Update()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag== "Zombie" )
        {
            AIZombie zombie = other.GetComponent<AIZombie>();
            zombie.photonView.RPC("TakeDamage", RpcTarget.MasterClient,bulletDamage);
            DestroyBullet();
        }
    }
    private void DestroyBullet()
    {
        Destroy(gameObject,2);
    }
    public void Initialized(int attackId, bool isMine)
    {
        this.attackerId = attackId;
        this.isMine = isMine;
    }
}
