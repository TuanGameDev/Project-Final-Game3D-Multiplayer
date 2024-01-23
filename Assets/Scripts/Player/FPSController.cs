using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class FPSController : MonoBehaviourPun
{
    [SerializeField] public int id;
    [SerializeField] public int currentHP;
    [SerializeField] public int maxHP;
    [SerializeField] public int def;

    [Header("UI")]
    [SerializeField] public TextMeshProUGUI hpText;
    [SerializeField] private Slider healthSlider;

    [SerializeField] public static FPSController me;
    [SerializeField] public Player photonPlayer;
    [SerializeField] private PlayerName playerName;
    [SerializeField] public Canvas canvashealth;

    [Header("Gun")]
    [SerializeField] public GameObject aimingObject;
    [SerializeField] private GameObject _flashLight;
    [SerializeField] Camera cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [Header("PickUp")]
    [SerializeField] private Transform transformPickup;
    [SerializeField] public GameObject pickedUpGun;
    [SerializeField] public List<GameObject> pickedUpGuns = new List<GameObject>();

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField][Min(1)] private float hitRange = 1.5f;
    private bool canDropGun = true;
    [SerializeField] private bool hasRifle = false;
    [SerializeField] private bool hasPistol = false;
    [Header("UI Icons")]
    [SerializeField] public Image rifleIconImage;
    [SerializeField] public Image pistolIconImage;

    Rigidbody rb;
    Animator ani;
    PhotonView PV;
    CharacterController crl;
    Vector3 moveAmount;
    float verticalLookRotation;
    bool grounded;
    private RaycastHit hit;
    private float maxHealthValue;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        ani = GetComponent<Animator>();
        crl = GetComponent<CharacterController>();
    }

    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        playerName.UpdateNameTag(player.NickName);
        GameManager.gamemanager.players[id - 1] = this;
        UpdateHpText(currentHP, maxHP);
        UpdateHealthSlider(currentHP);
        UpdateHeal(maxHP);
        if (player.IsLocal)
            me = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!photonView.IsMine)
        {
            canvashealth.enabled = false;
            if (cameraHolder)
                cameraHolder.gameObject.SetActive(false);

            if (_flashLight)
                _flashLight.SetActive(false);
            if (Gun_Shoot.instance != null)
            {
                Gun_Shoot.instance.txtAmmo.gameObject.SetActive(photonView.IsMine);
            }
        }
    }


    void Update()
    {
        if (photonView.IsMine)
        {
            Look();
            Move();
            Jump();
            CheckAiming();
            PickUp();
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpGun();
            }

            if (Input.GetKey(KeyCode.G) && canDropGun)
            {
                DropGun();
            }
            if (Input.GetMouseButton(0))
            {
                photonView.RPC("ShootRPC", RpcTarget.AllBuffered);
                SwitchGun.instance.canSwitch = false;
                Invoke("ResetSwitchCooldown", 0.2f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                photonView.RPC("ReloadGunRPC", RpcTarget.AllBuffered);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                photonView.RPC("ZoomGunRPC", RpcTarget.AllBuffered);
            }
            else
            {
                photonView.RPC("ZoomGunRPC", RpcTarget.AllBuffered);
            }

            if (Input.GetButtonDown("Flashlight"))
            {
                photonView.RPC("ToggleFlashlightRPC", RpcTarget.AllBuffered, !_flashLight.activeSelf);
            }
        }
    }

    #region di chuyển, nhảy và máu
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 30f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.magnitude <= 0.01f)
        {
            ani.SetFloat("X", 0f);
            ani.SetFloat("Y", 0f);
            ani.SetFloat("Speed", 0f);
        }
        else
        {
            ani.SetFloat("X", horizontalInput * walkSpeed);
            ani.SetFloat("Y", verticalInput * walkSpeed);

            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            transform.Translate(movement * targetSpeed * Time.deltaTime, Space.Self);
            ani.SetFloat("Speed", targetSpeed);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        int damageValue = amount - def;
        if (damageValue < 1)
        {
            damageValue = 1;
        }
        currentHP -= damageValue;
        UpdateHealthSlider(currentHP);
        if (currentHP <= 0)
        {
            //Die();
        }
        UpdateHpText(currentHP, maxHP);
    }

    void UpdateHpText(int curHP, int maxHP)
    {
        hpText.text = curHP + "/" + maxHP;
    }
    public void UpdateHeal(int maxVal)
    {
        maxHealthValue = maxVal;
        healthSlider.value = 1.0f;

    }
    void UpdateHealthSlider(int heal)
    {
        healthSlider.value = (float)heal / maxHealthValue;
    }
    #endregion
    public void CheckAiming()
    {
        Ray ray = cameraHolder.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.name == aimingObject.name || hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                return;
            Vector3 hitPoint = hit.point;
            if (aimingObject != null)
            {
                MoveObjectToPosition(hitPoint);
            }
        }
        else
        {
            Vector3 endPoint = ray.GetPoint(200f);
            if (aimingObject != null)
            {
                MoveObjectToPosition(endPoint);
            }
        }
    }
    private void MoveObjectToPosition(Vector3 targetPosition)
    {
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        aimingObject.transform.position = newPosition;
    }

    void PickUp()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            pickUpUI.SetActive(false);
        }

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange))
        {
            if (hit.collider.CompareTag("Pickable") || hit.collider.CompareTag("Rifle") || hit.collider.CompareTag("Pistol"))
            {
                if (!pickedUpGuns.Contains(hit.collider.gameObject))
                {
                    hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
                    pickUpUI.SetActive(true);
                }
            }
        }
    }
    public void PickUpGun()
    {
        if (pickUpUI != null && pickUpUI.activeSelf && hit.collider != null)
        {
            photonView.RPC("PickUpGunRPC", RpcTarget.AllBuffered, hit.collider.gameObject.GetPhotonView().ViewID);
        }
    }

    [PunRPC]
    void PickUpGunRPC(int gunViewID)
    {
        GameObject gun = PhotonView.Find(gunViewID)?.gameObject;

        if (gun != null)
        {
            if (!pickedUpGuns.Contains(gun))
            {
                if (gun.CompareTag("Rifle"))
                {
                    if (hasRifle)
                    {
                        photonView.RPC("DropGunRPC", RpcTarget.AllBuffered, pickedUpGun.GetPhotonView().ViewID);
                    }
                    hasRifle = true;
                }
                if (gun.CompareTag("Pistol"))
                {
                    if (hasPistol)
                    {
                        photonView.RPC("DropGunRPC", RpcTarget.AllBuffered, pickedUpGun.GetPhotonView().ViewID);
                    }
                    hasPistol = true;
                }

                gun.transform.parent = transformPickup;
                gun.transform.localPosition = Vector3.zero;
                gun.transform.localRotation = Quaternion.identity;
                gun.GetComponent<Rigidbody>().isKinematic = true;
                gun.GetComponent<BoxCollider>().isTrigger = true;
                pickedUpGun = gun;
                SelectGun();
                pickedUpGuns.Add(gun);
                Gun_Shoot gunShoot = gun.GetComponent<Gun_Shoot>();

                if (gunShoot != null)
                {
                    gunShoot.originalFOV = cameraHolder.fieldOfView;
                    gunShoot.playerCamera = cameraHolder;
                    gunShoot.txtAmmo.gameObject.SetActive(photonView.IsMine);
                    if (gun.CompareTag("Rifle"))
                    {
                        rifleIconImage.sprite = gunShoot.icon;
                        rifleIconImage.gameObject.SetActive(true);
                        pistolIconImage.gameObject.SetActive(false);
                    }
                    else if (gun.CompareTag("Pistol"))
                    {
                        pistolIconImage.sprite = gunShoot.icon;
                        pistolIconImage.gameObject.SetActive(true);
                        rifleIconImage.gameObject.SetActive(false);
                    }
                }
            }
        }
    }


    void DropGun()
    {
        if (pickedUpGun != null)
        {
            photonView.RPC("DropGunRPC", RpcTarget.AllBuffered, pickedUpGun.GetPhotonView().ViewID);
            StartCoroutine(DropGunCooldown());
        }
    }

    [PunRPC]
    void DropGunRPC(int gunViewID)
    {
        GameObject gun = PhotonView.Find(gunViewID).gameObject;
        if (gun != null && pickedUpGun == gun)
        {
            if (gun.CompareTag("Rifle"))
            {
                hasRifle = false;
                rifleIconImage.gameObject.SetActive(hasRifle);

                if (hasPistol)
                {
                    pistolIconImage.sprite = pickedUpGuns.FirstOrDefault(g => g.CompareTag("Pistol"))?.GetComponent<Gun_Shoot>().icon;
                    pistolIconImage.gameObject.SetActive(true);
                }
                else
                {
                    pistolIconImage.gameObject.SetActive(false);
                }
            }
            else if (gun.CompareTag("Pistol"))
            {
                hasPistol = false;
                pistolIconImage.gameObject.SetActive(hasPistol);

                if (hasRifle)
                {
                    rifleIconImage.sprite = pickedUpGuns.FirstOrDefault(g => g.CompareTag("Rifle"))?.GetComponent<Gun_Shoot>().icon;
                    rifleIconImage.gameObject.SetActive(true);
                }
                else
                {
                    rifleIconImage.gameObject.SetActive(false);
                }
            }

            gun.transform.parent = null;
            gun.GetComponent<Rigidbody>().isKinematic = false;
            gun.GetComponent<BoxCollider>().isTrigger = false;
            pickedUpGuns.Remove(gun);

            if (pickedUpGuns.Count > 0)
            {
                pickedUpGun = pickedUpGuns[0];
                SelectGun();
            }
            else
            {
                pickedUpGun = null;
            }

            Gun_Shoot gunShoot = gun.GetComponent<Gun_Shoot>();
            if (gunShoot != null)
            {
                gunShoot.txtAmmo.gameObject.SetActive(false);
            }
        }
    }



    IEnumerator DropGunCooldown()
    {
        canDropGun = false;
        yield return new WaitForSeconds(0.2f);
        canDropGun = true;
    }
    public void SelectGun()
    {
        foreach (GameObject gun in pickedUpGuns)
        {
            gun.SetActive(gun == pickedUpGun);
        }
    }
    public void UpdateSelectedGun(GameObject newGun)
    {
        pickedUpGun = newGun;
        SelectGun();

        if (photonView.IsMine)
        {
            photonView.RPC("UpdatePickedUpGunRPC", RpcTarget.Others, newGun.GetPhotonView().ViewID);
        }
    }
    [PunRPC]
    void UpdatePickedUpGunRPC(int newGunViewID)
    {
        GameObject newGun = PhotonView.Find(newGunViewID)?.gameObject;

        if (newGun != null)
        {
            pickedUpGun = newGun;
            SelectGun();
        }
    }
    [PunRPC]
    void ToggleFlashlightRPC(bool toggle)
    {
        if (_flashLight)
            _flashLight.SetActive(toggle);
    }
    [PunRPC]
    void ShootRPC()
    {
        if (pickedUpGun != null)
        {
            IUsable usable = pickedUpGun.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Shoot(gameObject);
            }
        }
    }

    [PunRPC]
    void ZoomGunRPC()
    {
        if (pickedUpGun != null)
        {
            IUsable usable = pickedUpGun.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Zoom(this.gameObject);
            }
        }
    }

    [PunRPC]
    void ReloadGunRPC()
    {
        if (pickedUpGun != null)
        {
            IUsable usable = pickedUpGun.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Reload(this.gameObject);
            }
        }
    }
    void ResetSwitchCooldown()
    {
        SwitchGun.instance.canSwitch = true;
    }
}
