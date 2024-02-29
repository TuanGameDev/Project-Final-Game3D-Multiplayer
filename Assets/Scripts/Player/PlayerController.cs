using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using static PlayerController;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviourPun
{

    PhotonView _view;
    CharacterController _controller;
    Animator _anim;

    [Header("Camera")]
    [SerializeField] Camera _maincam;
    [SerializeField] Cinemachine.CinemachineFreeLook CMfreelook;
    [SerializeField] Transform playerCam;
    [SerializeField] float turnspeed = 15;

    [Header("HUD")]
    [SerializeField] public int id;
    [SerializeField] public int currentHP;
    [SerializeField] public int maxHP;
    [SerializeField] public int def;
    [SerializeField] private PlayerName playerName;
    [SerializeField] public Player photonPlayer;
    [SerializeField] public TextMeshProUGUI txtpickup;


    [Header("Movement")]
    [SerializeField] float speed;
    float _hor, _ver;
    float _turnCalmTime = 0.1f;
    float _turnCalmVelocity;

    [Header("Weapon")]
    [SerializeField] Transform[] weaponSlots;
    [SerializeField] Transform crosshairTarget;

    Gun[] _equipWeapons = new Gun[2];
    int _activeWeaponIndex;
    bool _weaponActive = false;
    bool _isHolstered = false;

    [Header("Rigging")]
    [SerializeField] Animator rigController;
    [SerializeField] Transform leftHand;
    [SerializeField] UnityEngine.Animations.Rigging.Rig handIk;
    [SerializeField] public static PlayerController me;

    public WeaponAnimationEvents animationEvents;

    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    void Awake()
    {
        _view = GetComponent<PhotonView>();
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        CMfreelook = GetComponentInChildren<CinemachineFreeLook>();
        animationEvents = GetComponentInChildren<WeaponAnimationEvents>();
        
    }
    /*[PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        playerName.UpdateNameTag(player.NickName);
        GameManager.gamemanager.playerCtrl[id - 1] = this;
      *//*  UpdateHpText(currentHP, maxHP);
        UpdateHealthSlider(currentHP);
        UpdateHeal(maxHP);*//*
        if (player.IsLocal)
            me = this;
    }*/
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);


        Gun existingweapon = GetComponentInChildren<Gun>();
         if (existingweapon)
         {
            Equip(existingweapon);
         }
        
    }
    void Update()
    {
        HandleInput();
        UpdateWeaponState();
        _anim.SetBool("weaponActive", _weaponActive);
    }
    void FixedUpdate()
    {
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
    void HandleInput()
    {
        var _weapon = GetWeapon(_activeWeaponIndex);
        if (_weapon != null && !_isHolstered)
        {
            _weaponActive = true;
            if (Input.GetMouseButton(0))
            {
                _weapon.StartFiring();
            }
            else
            {
                _weapon.StopFiring();
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleWeaponHolster();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(WeaponSlot.Primary);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(WeaponSlot.Secondary);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rigController.SetTrigger("reload");
        }
    }
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
    void MovementWithWeapon()
    {
        _hor = Input.GetAxis("Horizontal");
        _ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(_hor, 0f, _ver).normalized;

        Vector3 moveDirection = transform.TransformDirection(direction) * speed;
        _controller.Move(moveDirection * Time.deltaTime);

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

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * speed * Time.deltaTime);
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

    Gun GetWeapon(int index)
    {
        if (index < 0 || index >= _equipWeapons.Length)
        {
            return null;
        }
        return _equipWeapons[index];
    }
    public void GetActiveWeapon() => GetWeapon(_activeWeaponIndex);

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
    }
    void DropMag()
    {

    }
    void RefillMag()
    {

    }
    void AttachMag()
    {

    }


}
