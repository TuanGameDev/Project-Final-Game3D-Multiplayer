using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine.UI;
using System;
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
    [SerializeField] private Transform transformPickupRifle;
    [SerializeField] private Transform transformPickupPistol;

    [SerializeField] private GameObject pickedUpGunRifle;
    [SerializeField] private GameObject pickedUpGunPistol;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField] [Min(1)] private float hitRange = 1.5f;

    Rigidbody rb;
    Animator ani;
    PhotonView PV;
    CharacterController crl;
    Vector3 moveAmount;
    float verticalLookRotation;
    bool grounded;
    private bool isHoldingGun = false;
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
        UpdateHealthSlider(maxHP);
        if (player.IsLocal)
            me = this;
    }
    void Start()
    {
        if (!photonView.IsMine)
        {
            canvashealth.enabled = false;
        }
        if (PV.IsMine)
        {
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(crl);
            if (_flashLight)
                _flashLight.SetActive(false);
        }
    }
     void Update()
	{
        if (!PV.IsMine)
            return;

        Look();
        Move();
        Jump();
        CheckAiming();
        PickUp();
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHoldingGun && pickUpUI.activeSelf)
            {
                PickUpGun();
            }
        }
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ZoomGun();
        }
        else
        {
            ZoomGun();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isHoldingGun)
            {
                DropGun();
            }
        }
        if (Input.GetButtonDown("Flashlight"))
        {
            if (_flashLight)
                _flashLight.SetActive(!_flashLight.activeSelf);
        }
    }
    #region di chuyển,nhảy và máu
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
    void UpdateHealthSlider(int heal)
    {
        maxHealthValue = heal;
        healthSlider.value = 1.0f;
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
    public void PickUp()
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
                hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
                pickUpUI.SetActive(true);
            }
        }
    }
    void PickUpGun()
    {
        isHoldingGun = true;
        pickUpUI.SetActive(false);

        if (hit.collider.CompareTag("Rifle"))
        {
            pickedUpGunRifle = hit.collider.gameObject;
            EquipGun(pickedUpGunRifle);
        }
        else if (hit.collider.CompareTag("Pistol"))
        {
            pickedUpGunPistol = hit.collider.gameObject;
            EquipGun(pickedUpGunPistol);
        }
    }

    void DropGun()
    {
        isHoldingGun = false;

        if (pickedUpGunRifle != null)
        {
            UnequipGun(pickedUpGunRifle);
            pickedUpGunRifle = null;
        }

        if (pickedUpGunPistol != null)
        {
            UnequipGun(pickedUpGunPistol);
            pickedUpGunPistol = null;
        }
    }
    void EquipGun(GameObject gun)
    {
        if (gun.CompareTag("Rifle"))
        {
            gun.transform.parent = transformPickupRifle;
        }
        else if (gun.CompareTag("Pistol"))
        {
            gun.transform.parent = transformPickupPistol;
        }

        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.GetComponent<Rigidbody>().useGravity = false;
        gun.GetComponent<BoxCollider>().isTrigger = true;

        Gun_Shoot gunShoot = gun.GetComponent<Gun_Shoot>();
        if (gunShoot != null)
        {
            gunShoot.originalFOV = cameraHolder.fieldOfView;
            gunShoot.playerCamera = cameraHolder;
        }
    }

    void UnequipGun(GameObject gun)
    {
        gun.transform.parent = null;
        gun.GetComponent<Rigidbody>().useGravity = true;
        gun.GetComponent<BoxCollider>().isTrigger = false;
    }
    void Shoot()
    {
        if (pickedUpGunRifle != null)
        {
            IUsable usable = pickedUpGunRifle.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Shoot(this.gameObject);
            }
        }
         if (pickedUpGunPistol != null)
        {
            IUsable usable = pickedUpGunPistol.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Shoot(this.gameObject);
            }
        }
    }
    void ZoomGun()
    {
        if (pickedUpGunRifle != null)
        {
            IUsable usable = pickedUpGunRifle.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Zoom(this.gameObject);
            }
        }
        else if (pickedUpGunPistol != null)
        {
            IUsable usable = pickedUpGunPistol.GetComponent<IUsable>();
            if (usable != null)
            {
                usable.Zoom(this.gameObject);
            }
        }
    }

    public static implicit operator FPSController(PlayerHUD v)
    {
        throw new NotImplementedException();
    }
}
