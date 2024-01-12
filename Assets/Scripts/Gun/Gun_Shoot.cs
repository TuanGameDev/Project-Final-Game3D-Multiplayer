using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;
using Photon.Pun.Demo.Asteroids;

public class Gun_Shoot : MonoBehaviourPun
{
    public static Gun_Shoot instance;

    public float damage = 10f;
    public float timeBetweenShooting, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft;
    int bulletsShot;
    bool shooting, readyToShoot, reloading;
    private float originalSpread;

    public Camera playerCamera;
    public Transform bulletTransForms;
    public RaycastHit rayHit;
    public TextMeshProUGUI txtAmmo;

    public float zoomFOV = 40f;
    public bool isZoomed = false;
    public float originalFOV;

    public GameObject muzzle, bulletPrefab;
    private float currentDamage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        bulletsLeft = magazineSize;
        currentDamage = damage;
        txtAmmo.text = bulletsLeft + " / " + magazineSize;
        readyToShoot = true;
    }

    public void Shooting()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Shoot(currentDamage);
        }
    }

    public void Reloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
    }

    public void ZoomGun()
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
    }

    void Shoot(float damageValue)
    {
        readyToShoot = false;
        Vector3 direction = playerCamera.transform.forward;
        photonView.RPC("ShootRPC", RpcTarget.All, bulletTransForms.position, direction, damageValue);
        bulletsLeft--;

        UpdateAmmoUI();
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    [PunRPC]
    void ShootRPC(Vector3 position, Vector3 direction, float damageValue)
    {
        muzzle.SetActive(true);
        StartCoroutine(HideMuzzleGun());
        Bullet bulletInstance = PhotonNetwork.Instantiate("Bullet", position, Quaternion.LookRotation(direction)).GetComponent<Bullet>();
        bulletInstance.GetComponent<Rigidbody>().velocity = direction * 20f;
        bulletInstance.bulletDamage = damageValue;
    }


    void ResetShot()
    {
        readyToShoot = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        UpdateAmmoUI();
    }

    IEnumerator HideMuzzleGun()
    {
        yield return new WaitForSeconds(0.1f);
        muzzle.SetActive(false);
    }

    void UpdateAmmoUI()
    {
        txtAmmo.text = bulletsLeft + " / " + magazineSize;
    }
}
