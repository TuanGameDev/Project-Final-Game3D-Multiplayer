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
    [SerializeField] private Transform transformPickup;
    [SerializeField] public GameObject pickedUpGun;
    [SerializeField] public List<GameObject> pickedUpGuns = new List<GameObject>();

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField][Min(1)] private float hitRange = 1.5f;
    private bool canDropGun = true;
    private float dropGunCooldown = 0.5f;

    Rigidbody rb;
    Animator ani;
    PhotonView PV;
    CharacterController crl;
    Vector3 moveAmount;
    float verticalLookRotation;
    bool grounded;
    public bool isHoldingGun = false;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!photonView.IsMine)
        {
            canvashealth.enabled = false;
        }

        if (PV.IsMine)
        {
            Gun_Shoot.instance.txtAmmo.gameObject.SetActive(false);
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
        if (photonView.IsMine)
        {
            Look();
            Move();
            Jump();
            CheckAiming();
            PickUp();
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (pickUpUI != null && pickUpUI.activeSelf && hit.collider != null)
                {
                    photonView.RPC("PickUpGunRPC", RpcTarget.All, hit.collider.transform.position, hit.collider.transform.rotation.eulerAngles);
                }
            }

            if (Input.GetMouseButton(0))
            {
                photonView.RPC("ShootRPC", RpcTarget.AllViaServer);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                photonView.RPC("ReloadGunRPC", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                photonView.RPC("ZoomGunRPC", RpcTarget.All);
            }
            else
            {
                photonView.RPC("ZoomGunRPC", RpcTarget.All);
            }

            if (Input.GetKey(KeyCode.G) && canDropGun)
            {
                photonView.RPC("DropGunRPC", RpcTarget.All);
                StartCoroutine(DropGunCooldown());
            }
            if (Input.GetButtonDown("Flashlight"))
            {
                photonView.RPC("ToggleFlashlightRPC", RpcTarget.All, !_flashLight.activeSelf);
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

    [PunRPC]
    public void PickUpGunRPC(Vector3 gunPosition, Vector3 gunRotation, PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
            return;

        isHoldingGun = true;
        pickUpUI.SetActive(false);

        if (hit.collider != null)
        {
            GameObject newPickedUpGun = hit.collider.gameObject;
            if (!newPickedUpGun.GetComponent<PhotonView>().IsMine)
                return;

            pickedUpGuns.Add(newPickedUpGun);
            newPickedUpGun.transform.parent = transformPickup;
            newPickedUpGun.transform.localPosition = Vector3.zero;
            newPickedUpGun.transform.localRotation = Quaternion.identity;
            newPickedUpGun.GetComponent<Rigidbody>().isKinematic = true;
            newPickedUpGun.GetComponent<BoxCollider>().isTrigger = true;
            photonView.RPC("SetGunTransformRPC", RpcTarget.All, newPickedUpGun.GetComponent<PhotonView>().ViewID, transformPickup.position, transformPickup.rotation.eulerAngles, true);

            Gun_Shoot gunShoot = newPickedUpGun.GetComponent<Gun_Shoot>();
            if (gunShoot != null)
            {
                gunShoot.originalFOV = cameraHolder.fieldOfView;
                gunShoot.playerCamera = cameraHolder;
                gunShoot.txtAmmo.gameObject.SetActive(true);
            }

            if (pickedUpGuns.Count > 0)
            {
                pickedUpGun = pickedUpGuns[0];
                SelectGun();
                photonView.RPC("SetIsHoldingGunRPC", RpcTarget.All, true);
            }
        }
    }

    [PunRPC]
    public void SetGunTransformRPC(int gunViewID, Vector3 gunPosition, Vector3 gunRotation, bool isPickingUp)
    {
        PhotonView gunView = PhotonView.Find(gunViewID);
        if (gunView != null)
        {
            GameObject gunObject = gunView.gameObject;
            gunObject.transform.position = gunPosition;
            gunObject.transform.rotation = Quaternion.Euler(gunRotation);

            if (!isPickingUp)
            {
                gunObject.GetComponent<Rigidbody>().isKinematic = false;
                gunObject.GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }


    [PunRPC]
    public void DropGunRPC(PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
            return;

        isHoldingGun = false;
        GameObject droppedGun = pickedUpGun;
        if (pickedUpGun == droppedGun)
        {
            pickedUpGun = null;
        }
        droppedGun.transform.parent = null;
        droppedGun.GetComponent<Rigidbody>().isKinematic = false;
        droppedGun.GetComponent<BoxCollider>().isTrigger = false;
        photonView.RPC("SetGunTransformRPC", RpcTarget.All, droppedGun.GetComponent<PhotonView>().ViewID, droppedGun.transform.position, droppedGun.transform.rotation.eulerAngles, false);

        pickedUpGuns.Remove(droppedGun);
        Gun_Shoot gunShoot = droppedGun.GetComponent<Gun_Shoot>();
        if (gunShoot != null)
        {
            gunShoot.txtAmmo.gameObject.SetActive(false);
        }

        if (pickedUpGuns.Count > 0)
        {
            pickedUpGun = pickedUpGuns[0];
            SelectGun();
            photonView.RPC("SetIsHoldingGunRPC", RpcTarget.AllBuffered, false);
        }
    }



    [PunRPC]
    void SetIsHoldingGunRPC(bool holding)
    {
        isHoldingGun = holding;
        SelectGun();
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
    }

    IEnumerator DropGunCooldown()
    {
        canDropGun = false;
        yield return new WaitForSeconds(dropGunCooldown);
        canDropGun = true;
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
}
