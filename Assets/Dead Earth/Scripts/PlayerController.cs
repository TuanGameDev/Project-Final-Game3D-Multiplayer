using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public Animator playerAnim;
    public Rigidbody rb;
    public static PlayerController me;
    public PlayerStatus playerstatus;
    [Header("Player Status")]
    public int id;
    public int damage;
    public float moveSpeed;
    public int coin;
    public int diamond;
    public int currentHP;
    public int maxHP;
    public int def;
    public bool dead;
    public Player photonPlayer;
    public bool faceRight;

    [Header("Text UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI messageText;
   [SerializeField] private Slider healthSlider;
    private float maxHealthValue;
    public Canvas canvas;
    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.gamemanager.players[id - 1] = this;
        playerstatus.InitializedPlayer(player.NickName);
        if (PlayerPrefs.HasKey("Coin"))
        {
            coin = PlayerPrefs.GetInt("Coin");
        }
        else
        {
            coin = 0;
        }
        if (PlayerPrefs.HasKey("Diamond"))
        {
            diamond = PlayerPrefs.GetInt("Diamond");
        }
        else
        {
            diamond = 0;
        }
        PlayerStatusInfo(maxHP);
        UpdateHpText(currentHP, maxHP);
        if (player.IsLocal)
            me = this;
        else
            rb.isKinematic = false;
    }
    private void Start()
    {
        if (!photonView.IsMine)
        {
            canvas.enabled = false;
        }
    }
    private void Update()
    {
        UpdateHpText(currentHP, maxHP);
        if (!photonView.IsMine)
            return;
        MoveCharacter();
    }
    void MoveCharacter()
    {
        if (!dead)
        {
            float x, y;
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");
            }
        }
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
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP);

    }
    void Die()
    {
        dead = true;
        rb.isKinematic = true;
        coin -= 10;
        PlayerPrefs.SetInt("Coin", coin);
        transform.position = new Vector3(0, 90, 0);
        Vector3 spawnPos = GameManager.gamemanager.spawnPoint[Random.Range(0, GameManager.gamemanager.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.gamemanager.respawnTime));
        StartCoroutine(CountdownAndHideMessage(10f));
    }
    IEnumerator Spawn(Vector3 spawnPos,float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        rb.isKinematic = false;
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP);
    }
    [PunRPC]
    void Heal(int amountToHeal)
    {
        currentHP = Mathf.Clamp(currentHP + amountToHeal, 0, maxHP);
        messageText.text = " You have picked up the chicken thighs " +"+"+ amountToHeal.ToString("N0")+" HP ";
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP);
    }
    [PunRPC]
    void GetGold(int goldToGive)
    {
        coin += goldToGive;
        PlayerPrefs.SetInt("Coin", coin);
        messageText.text = " You have picked up the coin "+ "+" + goldToGive.ToString("N0");
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    [PunRPC]
    void Diamond(int diamondToGive)
    {
        diamond += diamondToGive;
        PlayerPrefs.SetInt("Diamond", diamond);
        messageText.text = " You have picked up the diamond " + diamondToGive.ToString("N0");
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    public void PlayerStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
        healthSlider.value = 1.0f;
    }
    void UpdateHpText(int curHP, int maxHP)
    {
        hpText.text = curHP + "/" + maxHP;
    }
    void UpdateHealthSlider(int heal)
    {
        healthSlider.value = (float)heal / maxHealthValue;
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    IEnumerator CountdownAndHideMessage(float countdownTime)
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