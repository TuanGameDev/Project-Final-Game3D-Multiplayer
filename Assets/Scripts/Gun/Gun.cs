using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviourPun
{
    [Header("Weapon Info")]
    public PlayerController.WeaponSlot weaponSlot;
    public enum WeaponType
    {
        Rifle,
        SMG_Pistol,
    }
    public WeaponType weaponType;
    public string weaponName;
    public Sprite gunIcon;
    private int warriorID;
    private bool isMine;
    public int damage;
    public int ammoCount;
    public int magSize;
    public GameObject magazine;
    public GameObject bloodSplatterPrefab;

    [SerializeField] float fireRate = 3f;
    public bool isFiring = false;

    float _nextFireTime = 0f;
    [HideInInspector] public GunRecoil recoil;

    [Header("Effects")]
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer trailEffect;
    private TrailRenderer currentTrail;
    public GameObject flashlight;
    public bool flashActive = false;

    [Header("Raycast")]
    public Transform raycast;
    public Transform raycastDestination;

    public Highlight highlight;

    Ray _ray;
    RaycastHit _hit;
    public bool isPickedUp = false;
    private void Awake()
    {
        recoil = GetComponent<GunRecoil>();
        flashlight.gameObject.SetActive(false);
        if (weaponType == WeaponType.Rifle)
        {
            ammoCount = Random.Range(1, magSize);
        }
    }

    void Update()
    {
        if (isFiring && Time.time >= _nextFireTime)
        {
            CmdFireBullet();
            _nextFireTime = Time.time + 1f / fireRate;
        }
    }
    [PunRPC]
    public void StartFiring()
    {
        isFiring = true;
        recoil.Reset();
    }

    [PunRPC]
    public void StopFiring()
    {
        isFiring = false;
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }

    public void SetAmmoCount(int count)
    {
        ammoCount = count;
    }
    void CmdFireBullet()
    {
        if (photonView.IsMine)
        {
            Vector3 origin = raycast.position;
            Vector3 direction = raycastDestination.position - raycast.position;
            photonView.RPC("FireBullet", RpcTarget.All, origin, direction);
        }
    }

    [PunRPC]
    void FireBullet(Vector3 origin, Vector3 dir)
    {
        if (ammoCount <= 0) return;
        ammoCount--;

        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        _ray.origin = origin;
        _ray.direction = dir;

        if (currentTrail == null)
        {
            currentTrail = Instantiate(trailEffect, _ray.origin, Quaternion.identity);
        }
        else
        {
            currentTrail.transform.position = _ray.origin;
        }

        currentTrail.Clear();
        currentTrail.AddPosition(_ray.origin);

        if (Physics.Raycast(_ray, out _hit))
        {
            ParticleSystem selectedHitEffect = hitEffect;
            if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombie"))
            {
                Initialized(PlayerController.me.id, photonView.IsMine);
                Instantiate(bloodSplatterPrefab, _hit.point, Quaternion.identity);
                currentTrail.transform.position = _hit.point;

                AIZombie zombieHealth = _hit.collider.GetComponent<AIZombie>();
                if (zombieHealth != null && photonView.IsMine)
                {
                    zombieHealth.photonView.RPC("TakeDamage", RpcTarget.MasterClient, warriorID, damage);
                }
            }
            else
            {
                selectedHitEffect.transform.position = _hit.point;
                selectedHitEffect.transform.forward = _hit.normal;
                selectedHitEffect.Emit(1);
            }

            currentTrail.transform.position = _hit.point;
        }
        else
        {
            currentTrail.transform.position = _ray.origin + _ray.direction * 100f;
        }

        recoil.GenerateRecoil(weaponName);
        AudioManager._audioManager.PlaySFX(9);
    }
    [PunRPC]
    public void TurnOnFlashlight()
    {
        flashActive = true;
        flashlight.gameObject.SetActive(true);
    }
    [PunRPC]
    public void TurnOffFlashlight()
    {
        flashActive = false;
        flashlight.gameObject.SetActive(false);
    }

    public void Initialized(int attackId, bool isMine)
    {
        this.warriorID = attackId;
        this.isMine = isMine;
    }
}
