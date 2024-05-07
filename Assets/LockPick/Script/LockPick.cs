using System.Collections;
using UnityEngine;

public class LockPick : MonoBehaviour
{
    float pickPos;
    public float PickPos
    {
        get { return pickPos;}
        set
        {
            pickPos = value;
            pickPos = Mathf.Clamp(pickPos, 0f, 1f);
        }
    }
    float cyllinderPos;
    public float CyllinderPos
    {
        get { return cyllinderPos;}
        set
        {
            cyllinderPos = value;
            cyllinderPos = Mathf.Clamp(cyllinderPos, 0f, MaxRotationDistance);
        }
    }

    [SerializeField] float cyllinderRotationSpeed = 2f;
    [SerializeField] float cyllinderRetentionSpeed = 1f;
    Animator anim;
    float targetPos;
    [SerializeField] float leanency = 0.1f;
    float MaxRotationDistance
    {
        get { return 1f - Mathf.Abs(targetPos - PickPos) + leanency;}

    }
    bool shaking;
    float tension = 0f;
    [SerializeField] float tensionMutiplicator = 1f;
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
        }
        Shaking();
        Cyllinder();
        Anim();
    }
    void Shaking()
    {
        shaking = MaxRotationDistance - cyllinderPos < 0.03f;
        if(shaking)
        {
            tension += Time.deltaTime * tensionMutiplicator;
            if(tension > 1f)
            {
                PickBreak();
            }
        }
    }
    void PickBreak()
    {
        Reset();
        DoorLockPick.instance.CloseMiniGame();
        DoorLockPick.instance.StartCoroutine(ShowPanelBreak());
    }
    void Cyllinder()
    {
        CyllinderPos -= cyllinderRetentionSpeed * Time.deltaTime;
        CyllinderPos += Mathf.Abs(Input.GetAxisRaw("Vertical")) * Time.deltaTime * cyllinderRotationSpeed;
        if (CyllinderPos > 0.99f)
        {
            UnLock();
        }
    }

    void UnLock()
    {
        DoorLockPick.instance.CloseMiniGame();
        DoorLockPick.instance.StartCoroutine(ShowPanelWin());
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
    public IEnumerator ShowPanelBreak()
    {
        DoorLockPick.instance.panelBreak.SetActive(true);
        yield return new WaitForSeconds(2f);
        DoorLockPick.instance.panelBreak.SetActive(false);
    }
    public IEnumerator ShowPanelWin()
    {
        DoorLockPick.instance.panelWin.SetActive(true);
        yield return new WaitForSeconds(2f);
        DoorLockPick.instance.panelWin.SetActive(false);
    }
}
