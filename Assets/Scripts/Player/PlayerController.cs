using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
public enum PlayerMoveStatus { NotMoving, Crouching, Walking, Running, NotGrounded, Landing }
public enum CurveControlledBobCallbackType { Horizontal, Vertical }
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public Animator playerAnim;
    [Header("Player HUD")]
    [SerializeField] private float _walkSpeed = 1.0f;
    [SerializeField] private float _runSpeed = 4.5f;
    [SerializeField] private float _jumpSpeed = 7.5f;
    [SerializeField] private float _stickToGroundForce = 5.0f;
    [SerializeField] private float _gravityMultiplier = 2.5f;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.MouseLook _mouseLook = new UnityStandardAssets.Characters.FirstPerson.MouseLook();

    // Private internals
    private Camera _camera;
    private bool _jumpButtonPressed = false;
    private Vector2 _inputVector = Vector2.zero;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _previouslyGrounded = false;
    private bool _isWalking = true;
    private bool _isJumping = false;

    // Timers
    private float _fallingTimer = 0.0f;

    private CharacterController _characterController = null;
    private PlayerMoveStatus _movementStatus = PlayerMoveStatus.NotMoving;

    // Public Properties
    public PlayerMoveStatus movementStatus { get { return _movementStatus; } }
    public float walkSpeed { get { return _walkSpeed; } }
    public float runSpeed { get { return _runSpeed; } }
    //
    public int id;
    public int currentHP;
    public int maxHP;
    public int def;
    public bool dead;

    [Header("Text UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI messageText;
    public Slider healthSlider;
    private float maxHealthValue;
    public Canvas canvas;
    public static PlayerController me;
    public PlayerStatus playerstatus;
    public Player photonPlayer;
    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.gamemanager.players[id - 1] = this;
        playerstatus.InitializedPlayer(player.NickName);
        PlayerStatusInfo(maxHP);
        UpdateHpText(currentHP, maxHP, currentHP);
        if (player.IsLocal)
            me = this;
    }
    protected void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _movementStatus = PlayerMoveStatus.NotMoving;
        _fallingTimer = 0.0f;
        _mouseLook.Init(transform, _camera.transform);
        if(!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    protected void Update()
    {
        if (!photonView.IsMine)
            return;
        if (_characterController.isGrounded) _fallingTimer = 0.0f;
        else _fallingTimer += Time.deltaTime;
        if (Time.timeScale > Mathf.Epsilon)
            _mouseLook.LookRotation(transform, _camera.transform);
        if (!_jumpButtonPressed)
            _jumpButtonPressed = Input.GetButtonDown("Jump");
        if (!_previouslyGrounded && _characterController.isGrounded)
        {
            if (_fallingTimer > 0.5f)
            {
                // TODO: Play Landing Sound
            }

            _moveDirection.y = 0f;
            _isJumping = false;
            _movementStatus = PlayerMoveStatus.Landing;
        }
        else
            if (!_characterController.isGrounded)
            _movementStatus = PlayerMoveStatus.NotGrounded;
        else
            if (_characterController.velocity.sqrMagnitude < 0.01f)
            _movementStatus = PlayerMoveStatus.NotMoving;
        else
            if (_isWalking)
            _movementStatus = PlayerMoveStatus.Walking;
        else
            _movementStatus = PlayerMoveStatus.Running;

        _previouslyGrounded = _characterController.isGrounded;
    }

    protected void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool waswalking = _isWalking;
        _isWalking = !Input.GetKey(KeyCode.LeftShift);
        float speed = _isWalking ? _walkSpeed : _runSpeed;
        _inputVector = new Vector2(horizontal, vertical);

        if (_inputVector.sqrMagnitude > 1) _inputVector.Normalize();
        Vector3 desiredMove = transform.forward * _inputVector.y + transform.right * _inputVector.x;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo, _characterController.height / 2f, 1))
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
        _moveDirection.x = desiredMove.x * speed;
        _moveDirection.z = desiredMove.z * speed;
        if (_characterController.isGrounded)
        {
            _moveDirection.y = -_stickToGroundForce;
            if (_jumpButtonPressed)
            {
                _moveDirection.y = _jumpSpeed;
                _jumpButtonPressed = false;
                _isJumping = true;
                // TODO: Play Jumping Sound
            }
        }
        else
        {
            _moveDirection += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
        }
        _characterController.Move(_moveDirection * Time.fixedDeltaTime);
    }
    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        int damageValue = damageAmount - def;
        if(damageValue<1)
        {
            damageValue = 1;
        }
        currentHP -= damageValue;
        UpdateHpText(currentHP, maxHP, currentHP);
    }
    void Die()
    {
        dead = true;
        transform.position = new Vector3(0, 90, 0);
        Vector3 spawnPos = GameManager.gamemanager.spawnPoint[Random.Range(0, GameManager.gamemanager.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.gamemanager.respawnTime));
        StartCoroutine(CountdownDie(10f));
    }
    public void SpawnerPlayer()
    {
        Vector3 spawnPos = GameManager.gamemanager.spawnPoint[Random.Range(0, GameManager.gamemanager.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.gamemanager.respawnTime));
    }
    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        currentHP = maxHP;
        UpdateHpText(currentHP, maxHP, currentHP);
    }
    [PunRPC]
    void Heal(int amountToHeal)
    {
        currentHP = Mathf.Clamp(currentHP + amountToHeal, 0, maxHP);
        messageText.text = " You have picked up the chicken thighs " +"+"+ amountToHeal.ToString("N0")+" HP ";
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
        UpdateHpText(currentHP, maxHP, currentHP);
    }
    public void PlayerStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
        healthSlider.value = 1.0f;
    }
    void UpdateHpText(int curHP, int maxHP, int heal)
    {
        hpText.text = curHP + "/" + maxHP;
        healthSlider.value = (float)heal / maxHealthValue;
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    IEnumerator CountdownDie(float countdownTime)
    {
        float timeRemaining = countdownTime;
        while (timeRemaining > 0)
        {
            messageText.text = "You are dead and will respawn after " + timeRemaining.ToString("F0") + " seconds and -10 Coins!";
            messageText.color = Color.red;
            yield return new WaitForSecondsRealtime(1f);
            timeRemaining--;
        }
        messageText.text = "";
    }
}