using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class Gun_Shoot : MonoBehaviourPun
{
    public static Gun_Shoot instance;

    public float damage = 10f;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
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
        if(instance == null)
        {
            instance = this;
        }
        bulletsLeft = magazineSize;
        currentDamage = damage;
        originalSpread = spread;
        txtAmmo.text = bulletsLeft + " / " + magazineSize;
        readyToShoot = true;
    }
    public void Shooting()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
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
            spread = 0.01f;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isZoomed = false;
            playerCamera.fieldOfView = originalFOV;
            spread = originalSpread;
        }
    }
    void Shoot(float damageValue)
    {
        readyToShoot = false;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = (FPSController.me.aimingObject.transform.position - bulletTransForms.position).normalized + new Vector3(x, y, 0);
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
        direction = direction.normalized + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0) + 
            (FPSController.me.aimingObject.transform.position - bulletTransForms.position).normalized;
        Bullet bulletInstance = PhotonNetwork.Instantiate("Bullet", position, Quaternion.LookRotation(direction)).GetComponent<Bullet>();
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