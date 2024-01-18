using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


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

    [PunRPC]
    public void ShootingRPC(Vector3 position, Vector3 direction, float damageValue)
    {
        readyToShoot = false;
        muzzle.SetActive(true);
        StartCoroutine(HideMuzzleGun());
        Bullet bulletInstance = Instantiate(bulletPrefab, position, Quaternion.LookRotation(direction)).GetComponent<Bullet>();
        bulletInstance.GetComponent<Rigidbody>().velocity = direction * 20f;
        bulletInstance.bulletDamage = damageValue;

        bulletsLeft--;
        UpdateAmmoUI();
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    public void Shooting()
    {
        if (!photonView.IsMine) return;

        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Vector3 direction = playerCamera.transform.forward;
            photonView.RPC("ShootingRPC", RpcTarget.All, bulletTransForms.position, direction, currentDamage);
        }
    }

    [PunRPC]
    public void ReloadRPC()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    public void Reloading()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            photonView.RPC("ReloadRPC", RpcTarget.All);
        }
    }

    public void ZoomGun()
    {
        if (!photonView.IsMine) return;

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

    void ResetShot()
    {
        readyToShoot = true;
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
