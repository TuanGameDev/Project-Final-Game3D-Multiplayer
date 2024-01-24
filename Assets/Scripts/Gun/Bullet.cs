using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] public float bulletDamage;
    public GameObject bloodPrefab;
    private void Start()
    {
        Invoke("DestroyBullet", destroyDelay);
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
            ShowBloodEffect(other.transform.position);
            zombie.photonView.RPC("TakeDamage", RpcTarget.MasterClient,bulletDamage);
        }
    }
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
    private void ShowBloodEffect(Vector3 position)
    {
        GameObject blood = Instantiate(bloodPrefab, position, Quaternion.identity);
        float particleDuration = blood.GetComponent<ParticleSystem>().main.duration;
        Destroy(blood, particleDuration);
    }
}
