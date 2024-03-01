using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using static PlayerController;
using Photon.Realtime;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPun
{
    PhotonView PV;
    CharacterController _controller;
    Animator _anim;
    public static PlayerController me;

    [Header("CAMERA")]
    [SerializeField] Camera _maincam;
    [SerializeField] Transform playerCam;
    CinemachineFreeLook CMfreelook;
    float turnspeed = 20;

    [Header("HUD")]
    public int id;
    public int currentHP;
    public int maxHP;
    public int def;
    public Canvas canvashealth;
    public Player photonPlayer;
    public TextMeshProUGUI txtpickup;
    public TextMeshProUGUI txtAmmo;
    [SerializeField] private PlayerName playerName;

    [Header("MOVEMENT")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpHeight;
    float _hor, _ver;
    float _turnCalmTime = 0.1f;
    float _turnCalmVelocity;
    bool _isJumping = false;
    Vector3 _velocity;

    [Header("WEAPON")]
    [SerializeField] Transform[] weaponSlots;
    [SerializeField] Transform crosshairTarget;

    float aimZoomDistance = 35f;
    float zoomSpeed = 5f; // Tốc độ zoom

    private float defaultZoomDistance;
    private float targetZoomDistance; // Khoảng cách zoom mục tiêu

    GameObject magazineHand;
    Gun[] _equipWeapons = new Gun[2];
    int _activeWeaponIndex;
    bool _weaponActive = false;
    bool _isHolstered = false;
    bool _aiming = false;
    bool _isReloading = false;

    [Header("RIGGING")]
    [SerializeField] Animator rigController;
    [SerializeField] Transform leftHand;
    [SerializeField] UnityEngine.Animations.Rigging.Rig handIk;
    [HideInInspector] public WeaponAnimationEvents animationEvents;

    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        CMfreelook = GetComponentInChildren<CinemachineFreeLook>();
        animationEvents = GetComponentInChildren<WeaponAnimationEvents>();
        
    }
    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        playerName.UpdateNameTag(player.NickName);
        GameManager.gamemanager.playerCtrl[id - 1] = this;
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
            if (_maincam)
                _maincam.gameObject.SetActive(false);
        }

        defaultZoomDistance = CMfreelook.m_Lens.FieldOfView;
        targetZoomDistance = defaultZoomDistance;

        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
        Gun existingweapon = GetComponentInChildren<Gun>();
         if (existingweapon)
         {
            Equip(existingweapon);
         }
        
    }
    void Update()
    {
        if (!photonView.IsMine) return;
        HandleInput();
        UpdateWeaponState();
        _anim.SetBool("weaponActive", _weaponActive);
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        UpdateWeaponState();
        if (_weaponActive && !_isHolstered)
        {
            SetCam_WithWeapon();
            MovementWithWeapon();
        }
        else
        {
            MovementWithoutWeapon();
        }
    }
    //
    #region MOVEMENT
    void HandleInput()
    {
        var _weapon = GetWeapon(_activeWeaponIndex);
        if (_weapon != null && !_isHolstered)
        {
            _weaponActive = true;
            if (Input.GetMouseButton(0))
            {
                if (!_isReloading)
                {
                    _weapon.StartFiring();
                    UpdateAmmo();
                }
            }
            else
            {
                _weapon.StopFiring();
            }

            if (Input.GetMouseButton(1))
            {
                _aiming = true;
                _weapon.recoil.recoilModifier = _aiming ? 0.3f : .1f;
                CMfreelook.m_Lens.FieldOfView = Mathf.Lerp(CMfreelook.m_Lens.FieldOfView, aimZoomDistance, Time.deltaTime * zoomSpeed);
            }
            else
            {
                _aiming = false;
                CMfreelook.m_Lens.FieldOfView = Mathf.Lerp(CMfreelook.m_Lens.FieldOfView, defaultZoomDistance, Time.deltaTime * zoomSpeed);
            }

            if (Input.GetKeyDown(KeyCode.R) && _weapon.ammoCount < _weapon.magSize)
            {
                if (!_isReloading)
                {
                    rigController.SetTrigger("reload");
                    StartCoroutine(DelayedReload());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(WeaponSlot.Primary);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(WeaponSlot.Secondary);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleWeaponHolster();
        }
    }
    void MovementWithWeapon()
    {
        _hor = Input.GetAxis("Horizontal");
        _ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(_hor, 0f, _ver).normalized;

        Vector3 moveDirection = transform.TransformDirection(direction);

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        _controller.Move(moveDirection * targetSpeed * Time.deltaTime);

        if (direction.magnitude >= 0.1)
        {
            _anim.SetFloat("xValue", _hor);
            _anim.SetFloat("zValue", _ver);
        }
        else
        {
            _anim.SetFloat("xValue", 0f);
            _anim.SetFloat("zValue", 0f);
        }
    }
    void MovementWithoutWeapon()
    {
        _hor = Input.GetAxisRaw("Horizontal");
        _ver = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(_hor, 0f, _ver).normalized;

        if (direction.magnitude >= 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnCalmVelocity, _turnCalmTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * targetSpeed * Time.deltaTime);
            _anim.SetFloat("xValue", _hor);
            _anim.SetFloat("zValue", _ver);
        }
        else
        {
            _anim.SetFloat("xValue", 0f);
            _anim.SetFloat("zValue", 0f);
        }
    }
    void SetCam_WithWeapon()
    {
        float yCam = _maincam.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yCam, 0), turnspeed * Time.fixedDeltaTime);
    }
    #endregion
    //
    #region WEAPON
    void UpdateWeaponState()
    {
        var _weapon = GetWeapon(_activeWeaponIndex);
        if (_weapon != null && !_isHolstered)
        {
            _weaponActive = true;
        }
        else
        {
            _weaponActive = false;
        }
    }
    Gun GetWeapon(int index)
    {
        if (index < 0 || index >= _equipWeapons.Length)
        {
            return null;
        }
        return _equipWeapons[index];
    }
    Gun GetActiveWeapon() => GetWeapon(_activeWeaponIndex);
    public void Equip(Gun newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var _weapon = GetWeapon(weaponSlotIndex);
        if (_weapon)
        {
            Destroy(_weapon.gameObject);
        }

        _weapon = newWeapon;
        _weapon.raycastDestination = crosshairTarget;
        _weapon.recoil.playerCamera = CMfreelook;
        _weapon.recoil.rig = rigController;
        _weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        _equipWeapons[weaponSlotIndex] = _weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
        UpdateAmmo();
        _weapon.PV.TransferOwnership(PhotonNetwork.LocalPlayer);
    }
    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = _activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }
    void ToggleWeaponHolster()
    {
        bool _isHolster = rigController.GetBool("holster_weapon");
        if (_isHolster)
        {
            StartCoroutine(ActivateWeapon(_activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(_activeWeaponIndex));
        }
    }
    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        _activeWeaponIndex = activateIndex;
        UpdateAmmo();

    }
    IEnumerator HolsterWeapon(int index)
    {
        _isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon != null)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
    }
    IEnumerator ActivateWeapon(int index)
    {
        var weapon = GetWeapon(index);
        if (weapon != null)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("weapon_" + weapon.weaponName);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            _isHolstered = false;
        }
    }
    void DropWeapon()
    {
        var weapon = GetActiveWeapon();
        if (weapon != null && !_isHolstered)
        {
            // Tạo một vũ khí mới tại vị trí hiện tại của người chơi và xóa vũ khí hiện tại
            Gun dropWeapon = Instantiate(weapon);
            dropWeapon.AddComponent<Rigidbody>();
            dropWeapon.AddComponent<BoxCollider>();
            dropWeapon.transform.position = transform.position;
            dropWeapon.transform.rotation = transform.rotation;
            Equip(dropWeapon);
            Destroy(weapon.gameObject);
        }
    }
    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "detach_mag": DetachMag(); break;
            case "drop_mag": DropMag(); break;
            case "refill_mag": RefillMag(); break;
            case "attach_mag": AttachMag(); break;
        }
    }
    void DetachMag()
    {
        Gun weapon = GetActiveWeapon();
        magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }
    void DropMag()
    {
        GameObject dropMag = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        dropMag.AddComponent<Rigidbody>();
        dropMag.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
        Destroy(dropMag, 3f);
    }
    void RefillMag()
    {
        magazineHand.SetActive(true);
    }
    void AttachMag()
    {
        Gun weapon = GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
        weapon.ammoCount = weapon.magSize;
        rigController.ResetTrigger("reload");
        UpdateAmmo();
    }
    void UpdateAmmo() // gọi hàm sau mỗi hành động liên quan đến weapon
    {
        Gun weapon = GetActiveWeapon();
        if (weapon != null)
        {
            txtAmmo.text = weapon.ammoCount + "/" + weapon.magSize;
        }
    }
    IEnumerator DelayedReload()
    {   
        _isReloading = true;
        yield return new WaitForSeconds(1.8f);
        _isReloading = false;
    }
    #endregion
    //
}
