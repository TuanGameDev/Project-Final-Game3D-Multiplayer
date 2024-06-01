using Photon.Pun;
using UnityEngine;

public class DeathVillage_LockPick : MonoBehaviourPunCallbacks
{
    float pickPos;
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

    public bool shaking;
    float tension = 0f;
    [SerializeField] float tensionMutiplicator = 1f;
    public bool isBroken = false;

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
        isBroken=false;
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Vertical") == 0)
        {
            Pick();
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
                DeathVillage_DoorLockPick mission = FindObjectOfType<DeathVillage_DoorLockPick>();
                mission.CloseMiniGame();
                PickBreak();
            }
        }
    }

    void PickBreak()
    {
        if (!isBroken)
        {
            isBroken = true;
            DeathVillage_DoorLockPick mission = FindObjectOfType<DeathVillage_DoorLockPick>();
            mission.IsBreak();
        }
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
        DeathVillage_DoorLockPick mission = FindObjectOfType<DeathVillage_DoorLockPick>();
        mission.CloseMiniGame();
        photonView.RPC("UnlockOnNetwork", RpcTarget.All);
    }
    [PunRPC]
    void UnlockOnNetwork()
    {
        DeathVillage_DoorLockPick mission = FindObjectOfType<DeathVillage_DoorLockPick>();
        mission.SetUnlockState(true);
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
