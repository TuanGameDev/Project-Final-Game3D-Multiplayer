using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Gun_Rifle : MonoBehaviourPun
{
    public float damage = 10f;
    public float fireRate = 10f;
    public float zoomFOV = 40f;
    public Camera playerCamera;
    public Transform bulletTransForms;
    public GameObject MuzzleGun;
    public GameObject bulletPrefab;
    private bool isZoomed = false;
    bool isDelayShootRunning = false;
    private float originalFOV;
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

            if (Input.GetMouseButton(0) && !isDelayShootRunning)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        isDelayShootRunning = true;

        MuzzleGun.SetActive(true);

        while (Input.GetMouseButton(0))
        {
            Vector3 shootDirection = (FPSController.me.aimingObject.transform.position - bulletTransForms.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, bulletTransForms.position, Quaternion.LookRotation(shootDirection));
            bullet.GetComponent<Rigidbody>().velocity = shootDirection * 20f;
            Destroy(bullet, 3f);
            yield return new WaitForSeconds(1f / fireRate);
        }
        MuzzleGun.SetActive(false);
        isDelayShootRunning = false;
    }
}
