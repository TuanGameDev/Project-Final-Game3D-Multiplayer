using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FPSController : MonoBehaviourPun
{
    [HideInInspector]
    public Animator playerAnim;
    [Header("Player HUD")]
    PhotonView PV;
    Rigidbody rb;
    Animator ani;
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
    public static FPSController me;
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
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        ani = GetComponent<Animator>();
    }
    void Start()
    {
        if(!PV.IsMine)
        {
            canvas.enabled = false;
        }
        if (PV.IsMine)
        {
            Debug.Log("...");
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }
    void Update()
    {
        if (!PV.IsMine)
            return;
        Move();
    }
    #region di chuyển và nhảy và shoot
   
    void Move()
    {
       
    }
    #endregion
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