using UnityEngine;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine.UIElements;

public class Gun_Shoot : MonoBehaviourPun
{
    public static Gun_Shoot instance;
    public float damage = 10f;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;
    public Camera playerCamera;
    public Transform bulletTransForms;
    public RaycastHit rayHit;
    public float zoomFOV = 40f;
    private bool isZoomed = false;
    public float originalFOV;
    public GameObject muzzle, bulletPrefab;
    public TextMeshProUGUI txt;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        txt.SetText(bulletsLeft + " / " + magazineSize);
    }
    public void Shooting()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
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
    void Shoot()
    {
        readyToShoot = false;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = playerCamera.transform.forward + new Vector3(x, y, 0);
        if (Physics.Raycast(playerCamera.transform.position, direction,out rayHit, range))
        {
/*            if (rayHit.collider.CompareTag("Enemy"))
            {
              rayHit.collider.GetComponent<ZombieHeal>.TakeDame(damage);  Dame Zombie
            }*/
        }
        muzzle.SetActive(true);
        StartCoroutine(HideMuzzleGun());
        GameObject bullet = Instantiate(bulletPrefab, bulletTransForms.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().velocity = direction * 20f;
        Destroy(bullet, 3f);
        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);
        if(bulletsShot >0 && bulletsLeft > 0)
        Invoke("Shoot", timeBetweenShots);
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
    }
    IEnumerator HideMuzzleGun()
    {
        yield return new WaitForSeconds(0.1f);
        muzzle.SetActive(false);
    }
}