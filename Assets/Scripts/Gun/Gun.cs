using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Gun : MonoBehaviourPun
{
    PhotonView photon;
    public GameObject prefabsDrop;
    [Header("WEAPON INFOR")]
    public PlayerController.WeaponSlot weaponSlot;
    public enum WeaponType
    {
        Rifle,
        Pistol
    }
    public WeaponType weaponType;
    public string weaponName;
    public Sprite gunIcon;
    private int warriorID;
    private bool isMine;
    public int damage;
    public int ammoCount; // đạn
    public int magSize; // số viên đạn trong 1 băng
    public int mag; // băng đạn
    public GameObject magazine; // mag gameobject
    public GameObject bloodSplatterPrefab;

    [SerializeField] float fireRate = 3f;
    public bool isFiring = false;

    float _nextFireTime = 0f;
    [HideInInspector] public GunRecoil recoil;


    [Header("EFFECTS")]
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    //public ParticleSystem zombieHitEffect;
    public TrailRenderer trailEffect; // tia lửa 
    private TrailRenderer currentTrail;
    public GameObject flashlight;
    public bool flashActive = false;

    [Header("RAYCAST")]
    public Transform raycast;
    public Transform raycastDestination;

    Ray _ray;
    RaycastHit _hit;

    private void Awake()
    {
        photon = GetComponent<PhotonView>();
        recoil = GetComponent<GunRecoil>();
        flashlight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isFiring && Time.time >= _nextFireTime)
        {
            FireBullet();
            _nextFireTime = Time.time + 1f / fireRate;
        }
        
    }

    public void StartFiring()
    {
        isFiring = true;
        recoil.Reset();

    }
    public void StopFiring()
    {
        isFiring= false;
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }
    public void SetAmmoCount(int count)
    {
        ammoCount = count;
    }

    void FireBullet()
    {
    if (ammoCount <= 0) return;
    ammoCount--;

    foreach (var particle in muzzleFlash)
    {
        particle.Emit(1);
    }

    _ray.origin = raycast.position;
    _ray.direction = raycastDestination.position - raycast.position;

    if (currentTrail == null)
    {
        // Tạo hiệu ứng trail mới nếu không có hiệu ứng trail nào tồn tại

        currentTrail = Instantiate(trailEffect, _ray.origin, Quaternion.identity);
    }
    else
    {
        // Di chuyển hiệu ứng trail hiện tại đến vị trí xuất phát mới
        currentTrail.transform.position = _ray.origin;
    }

    // Đặt điểm xuất phát của hiệu ứng trail tại vị trí xuất phát của raycast
    currentTrail.Clear();
    currentTrail.AddPosition(_ray.origin);

        if (Physics.Raycast(_ray, out _hit))
        {
            ParticleSystem selectedHitEffect = hitEffect;          

            // Kiểm tra nếu đối tượng va chạm có layer là "Zombie"

            if (_hit.collider.gameObject.layer == LayerMask.NameToLayer("Zombie"))
            {
                Initialized(PlayerController.me.id, photonView.IsMine);
                Instantiate(bloodSplatterPrefab, _hit.point, Quaternion.identity);
                currentTrail.transform.position = _hit.point;

                Debug.Log("Trúng zombie");
                AIZombie zombieHealth = _hit.collider.GetComponent<AIZombie>();
                if (zombieHealth != null && photonView.IsMine)
                {
                    zombieHealth.photonView.RPC("TakeDamage", RpcTarget.MasterClient,warriorID, damage);
                }
            }

            selectedHitEffect.transform.position = _hit.point;
            selectedHitEffect.transform.forward = _hit.normal;
            selectedHitEffect.Emit(1);

            // Di chuyển hiệu ứng trail đến hit point

            currentTrail.transform.position = _hit.point;
        }
        else
        {
            // Nếu không có va chạm, di chuyển hiệu ứng trail theo raycast

            currentTrail.transform.position = _ray.origin + _ray.direction * 100f;
        }

        recoil.GenerateRecoil(weaponName);
        AudioManager._audioManager.PlaySFX(9);

    }
    public void Initialized(int attackId, bool isMine)
    {
        this.warriorID = attackId;
        this.isMine = isMine;
    }
}
