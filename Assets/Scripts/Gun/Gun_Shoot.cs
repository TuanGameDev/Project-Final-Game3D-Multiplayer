using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Gun_Shoot : MonoBehaviourPun
{
    public static Gun_Shoot instance;
    public float damage = 10f;
    public float fireRate = 10f;
    public float zoomFOV = 40f;
    private float nextTimeToFire = 0f;
    public Camera playerCamera;
    public Transform bulletTransForms;
    public GameObject MuzzleGun;
    public GameObject bulletPrefab;
    private bool isZoomed = false;
    private float originalFOV;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        playerCamera = GameObject.FindWithTag("CameraFPS").GetComponent<Camera>();
        originalFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isZoomed = true;
                playerCamera.fieldOfView = zoomFOV;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isZoomed = false;
                playerCamera.fieldOfView = originalFOV;
            }

            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        MuzzleGun.SetActive(true);

        StartCoroutine(HideMuzzleGun());

        Vector3 shootDirection = (FPSController.me.aimingObject.transform.position - bulletTransForms.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletTransForms.position, Quaternion.LookRotation(shootDirection));
        bullet.GetComponent<Rigidbody>().velocity = shootDirection * 20f;
        Destroy(bullet, 3f);
    }

    IEnumerator HideMuzzleGun()
    {
        yield return new WaitForSeconds(0.1f);
        MuzzleGun.SetActive(false);
    }
}