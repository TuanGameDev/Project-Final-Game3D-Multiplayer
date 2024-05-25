using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Hospital_LockPick : MonoBehaviourPunCallbacks
{
    float pickPos;
    [Header("Audio LockPick")]
    [SerializeField] public AudioSource footstepAudioSource;
    [SerializeField] public AudioClip footstepSounds;
    public float PickPos
    {
        get { return pickPos; }
        set { pickPos = Mathf.Clamp01(value); }
    }

    float cyllinderPos;
    public float CyllinderPos
    {
        get { return cyllinderPos; }
        set { cyllinderPos = Mathf.Clamp(value, 0f, MaxRotationDistance); }
    }

    [SerializeField] float cyllinderRotationSpeed = 2f;
    [SerializeField] float cyllinderRetentionSpeed = 1f;
    Animator anim;
    float targetPos;
    [SerializeField] float leanency = 0.1f;
    float MaxRotationDistance
    {
        get { return 1f - Mathf.Abs(targetPos - PickPos) + leanency; }
    }

    bool shaking;
    float tension = 0f;
    [SerializeField] float tensionMutiplicator = 1f;
    bool isBroken = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        Reset();
        targetPos = Random.value;
    }

    public void Reset()
    {
        CyllinderPos = 0;
        PickPos = 0;
        tension = 0f;
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            Pick();
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = footstepSounds;
                footstepAudioSource.Play();
            }
        }
        Shaking();
        Cyllinder();
        Anim();
    }

    void Shaking()
    {
        shaking = MaxRotationDistance - cyllinderPos < 0.03f;
        if (shaking)
        {
            tension += Time.deltaTime * tensionMutiplicator;
            if (tension > 1f)
            {
                PickBreak();
            }
        }
    }

    void PickBreak()
    {
        isBroken = true;
        Hospital_DoorLockPick.instance.IsBreak();
        Hospital_DoorLockPick.instance.CloseMiniGame();
        Reset();
    }
    void Cyllinder()
    {
        CyllinderPos -= cyllinderRetentionSpeed * Time.deltaTime;
        CyllinderPos += Mathf.Abs(Input.GetAxisRaw("Vertical")) * Time.deltaTime * cyllinderRotationSpeed;
        if (CyllinderPos > 0.99f)
        {
            if (!isBroken)
            {
                UnLock();
            }
        }
    }

    void UnLock()
    {
        photonView.RPC("UnlockOnNetwork", RpcTarget.AllBuffered);
        Hospital_DoorLockPick.instance.CloseMiniGame();
    }

    [PunRPC]
    void UnlockOnNetwork()
    {
        Hospital_DoorLockPick.instance.SetUnlockState(true);
    }

    void Pick()
    {
        PickPos += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 0.5f;
    }

    void Anim()
    {
        anim.SetFloat("PickPosition", PickPos);
        anim.SetFloat("LockOpen", CyllinderPos);
        anim.SetBool("Shake", shaking);
    }
}
