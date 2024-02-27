using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using static PlayerController;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    PhotonView view;
    CharacterController controller;
    Animator _anim;

    [Header("Camera")]
    [SerializeField] Camera _maincam;
    [SerializeField] Transform playerCam;
    [SerializeField] float turnspeed = 15;

    [Header("HUD")]
    public TextMeshProUGUI txtpickup;

    [Header("Movement")]
    [SerializeField] float speed;
    float _hor, _ver;
    float _turnCalmTime = 0.1f;
    float _turnCalmVelocity;

    [Header("Weapon")]
    [SerializeField] Transform[] weaponSlots;
    [SerializeField] Transform crosshairTarget;

    Gun[] _equipWeapons = new Gun[2];
    int activeWeaponIndex;
    bool _weaponActive = false;
    bool _isHolstered = false;

    [Header("Rigging")]
    [SerializeField] Animator rigController;
    [SerializeField] UnityEngine.Animations.Rigging.Rig handIk;

    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    void Awake()
    {
        view = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _maincam = Camera.main;

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (photonView.IsMine)
        {
            _maincam.gameObject.SetActive(true);
            Gun existingweapon = GetComponentInChildren<Gun>();
            if (existingweapon)
            {
                Equip(existingweapon);
            }
        }
        else
        {
            _maincam.gameObject.SetActive(false);
        }

    }

    void Update()
    {
        if (photonView.IsMine)
        {
            var _weapon = GetWeapon(activeWeaponIndex);
            if (_weapon != null && !_isHolstered)
            {
                _weaponActive = true;
                SetCam_WithWeapon();
                MovementWithWeapon();

                if (Input.GetMouseButtonDown(0))
                {
                    photonView.RPC("StartFiringRPC", RpcTarget.All);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    photonView.RPC("StopFiringRPC", RpcTarget.All);
                }

                handIk.weight = 1.0f;
                _anim.SetBool("weaponActive", _weaponActive);
            }
            else
            {
                _weaponActive = false;
                MovementWithoutWeapon();

                handIk.weight = 0.0f;
                _anim.SetBool("weaponActive", _weaponActive);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                photonView.RPC("ToggleWeaponHolsterRPC", RpcTarget.All, activeWeaponIndex);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                photonView.RPC("SetActiveWeaponRPC", RpcTarget.All, WeaponSlot.Primary);
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                photonView.RPC("SetActiveWeaponRPC", RpcTarget.All, WeaponSlot.Secondary);
            }
        }
    }

    void MovementWithWeapon()
    {
        _hor = Input.GetAxis("Horizontal");
        _ver = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(_hor, 0f, _ver).normalized;

        Vector3 moveDirection = transform.TransformDirection(direction) * speed;
        controller.Move(moveDirection * Time.deltaTime);

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
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
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

    public void Equip(Gun newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        var _weapon = GetWeapon(weaponSlotIndex);
        if (_weapon)
        {
            PhotonNetwork.Destroy(_weapon.gameObject);
        }

        _weapon = newWeapon;
        _weapon.raycastDestination = crosshairTarget;
        _weapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
        _equipWeapons[weaponSlotIndex] = _weapon;
        SetActiveWeapon(newWeapon.weaponSlot);
    }

    [PunRPC]
    void SetActiveWeaponRPC(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if (holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
        
    }

    void ToggleActiveWeapon()
    {
        photonView.RPC("ToggleWeaponHolsterRPC", RpcTarget.All, activeWeaponIndex);
    }

    [PunRPC]
    void ToggleWeaponHolsterRPC(int index)
    {
        bool _isHolster = rigController.GetBool("holster_weapon");
        if (_isHolster)
        {
            StartCoroutine(ActivateWeapon(index));
        }
        else
        {
            StartCoroutine(HolsterWeapon(index));
        }
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        activeWeaponIndex = (int)weaponSlot;
        if (photonView.IsMine)
        {
            photonView.RPC("SetActiveWeaponRPC", RpcTarget.Others, weaponSlot);
        }
    }

    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
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
            rigController.Play("weapon_" + weapon.weaponName);
            rigController.SetBool("holster_weapon", false);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            _isHolstered = false;
        }
    }

    [PunRPC]
    void StartFiringRPC()
    {
        var _weapon = GetWeapon(activeWeaponIndex);
        if (_weapon != null)
        {
            _weapon.StartFiring();
        }
    }

    [PunRPC]
    void StopFiringRPC()
    {
        var _weapon = GetWeapon(activeWeaponIndex);
        if (_weapon != null)
        {
            _weapon.StopFiring();
        }
    }

}
