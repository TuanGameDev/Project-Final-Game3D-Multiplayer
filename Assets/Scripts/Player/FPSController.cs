using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
public class FPSController : MonoBehaviourPun
{
    Rigidbody rb;
    Animator ani;
    PhotonView PV;
    CharacterController crl;
    [SerializeField] public int id;
    [SerializeField] Camera cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    float verticalLookRotation;
    bool grounded;
    Vector3 moveAmount;
    [SerializeField] public static FPSController me;
    [SerializeField] private Player photonPlayer;
    [SerializeField] private PlayerName playerHUD;
    [Header("Gun")]
    [SerializeField] public GameObject aimingObject;
    [SerializeField] private GameObject _flashLight;
    [Header("PickUp")]
    [SerializeField] private Transform transformPickup;
    private bool isHoldingGun = false;
    private GameObject pickedUpGun;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject pickUpUI;
    [SerializeField] [Min(1)] private float hitRange = 1.5f;
    private RaycastHit hit;


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
        playerHUD.UpdateNameTag(player.NickName);
        GameManager.gamemanager.players[id - 1] = this;
        if (player.IsLocal)
            me = this;
    }
    void Start()
    {
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
            if (hit.collider.CompareTag("Pickable"))
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
        pickedUpGun = hit.collider.gameObject;
        pickedUpGun.transform.parent = transformPickup;
        pickedUpGun.transform.localPosition = Vector3.zero;
        pickedUpGun.transform.localRotation = Quaternion.identity;
        pickedUpGun.GetComponent<Rigidbody>().useGravity = false;
        pickedUpGun.GetComponent<BoxCollider>().isTrigger = true;

        // Set isHand to true when picking up the gun
        Gun_Shoot gunShoot = pickedUpGun.GetComponent<Gun_Shoot>();
        if (gunShoot != null)
        {
            gunShoot.originalFOV = cameraHolder.fieldOfView;
            gunShoot.playerCamera = cameraHolder;
        }
    }
    void DropGun()
    {
        isHoldingGun = false;
        pickedUpGun.transform.parent = null;
        pickedUpGun.GetComponent<Rigidbody>().useGravity = true;
        pickedUpGun.GetComponent<BoxCollider>().isTrigger = false;
        pickedUpGun = null;
    }
    void Shoot()
    {
        if(pickedUpGun != null)
        {
            IUsable usable = pickedUpGun.GetComponent<IUsable>();
            if(usable != null)
            {
                usable.Shoot(this.gameObject);
            }
        }
    }
    void ZoomGun()
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
}
