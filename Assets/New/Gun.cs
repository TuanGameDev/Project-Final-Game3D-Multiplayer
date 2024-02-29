using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public PlayerController.WeaponSlot weaponSlot;

    [SerializeField] float fireRate = 3f;
    float _nextFireTime = 0f;

    public bool isFiring = false;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer trailEffect;
    public Transform raycast;
    public Transform raycastDestination;
    public string weaponName;
    public GunRecoil recoil;
    public GameObject magazine; // mag prefab


    Ray _ray;
    RaycastHit _hit;

    private void Awake()
    {
        recoil = GetComponent<GunRecoil>();
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

    void FireBullet()
    {
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        _ray.origin = raycast.position;
        _ray.direction = raycastDestination.position - raycast.position;

        // Tạo hiệu ứng trail mới
        var newTrail = Instantiate(trailEffect, raycast.position, Quaternion.identity);
        newTrail.AddPosition(_ray.origin);

        if (Physics.Raycast(_ray, out _hit))
        {
            hitEffect.transform.position = _hit.point;
            hitEffect.transform.forward = _hit.normal;
            hitEffect.Emit(1);

            // Di chuyển hiệu ứng trail theo đường raycast
            newTrail.transform.position = _hit.point;
        }

        recoil.GenerateRecoil(weaponName);
    }
}
