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
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public static PlayerController me;
    public PlayerStatus playerstatus;
    [Header("Player Status")]
    public int id;
    public int warriorID;
    private bool isMine;
    public Transform attackpoint;
    public int damageMin;
    public int damageMax;
    public float attackRange;
    public float attackDelay;
    public float lastAttackTime;
    public float cooldownDuration;
    public float moveSpeed;
    public int coin;
    public int diamond;
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int def;
    public int playerLevel = 1;
    public int currentExp;
    public int maxExp = 500;
    public bool dead;
    public Player photonPlayer;
    public bool faceRight;

    [Header("Cooldown Skill")]
    public float cooldownSkill1;
    public float cooldownSkill2;
    public float cooldownSkill3;
    public float cooldownSkill4;

    [Header("Text UI")]
    public GameObject respawn;
    public GameObject skillUI;
    public GameObject damPopUp;
    public GameObject expPopUp;
    public GameObject levelupEffect;
    public GameObject dieEffect;
    public Image cooldownAttack;
    public Image cooldownImageSKill1;
    public Image cooldownImageSKill2;
    public Image cooldownImageSKill3;
    public Image cooldownImageSKill4;
    public GameObject panelDie;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI expplayerText;
    public TextMeshProUGUI messageText;
   [SerializeField] private Slider healthSlider;
   [SerializeField] private Slider ExpSlider;
    private float maxHealthValue;
    public Canvas canvas;

    [System.Serializable]
    public class MissionData
    {
        public int count = 0;
        public int requiredCount = 1;
        public TextMeshProUGUI missionText;
        public Button claimRewardButton;
        public bool isQuestCompleted = false;
    }
    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.gamemanager.players[id - 1] = this;
        playerstatus.InitializedPlayer(player.NickName, playerLevel);
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
        playerstatus.photonView.RPC("UpdatePlayerLevel", RpcTarget.All, playerLevel);
        PlayerStatusInfo(maxHP);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        currentHP = maxHP;
        currentMP = maxMP;
        UpdateExpSlider(currentExp, maxExp,playerLevel);
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
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
        if (!photonView.IsMine)
            return;
        MoveCharacter();
    }
    #region di chuyển và tấn công và cooldown skill
  
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
    #endregion

    #region máu và damage text
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
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
    }
    [PunRPC]
    void FlasDamage()
    {
        StartCoroutine(DamageFlash());
        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }
    void Die()
    {
        panelDie.SetActive(true);
        skillUI.SetActive(false);
        dead = true;
        rb.isKinematic = true;
        coin -= 10;
        PlayerPrefs.SetInt("Coin", coin);
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        transform.position = new Vector3(0, 90, 0);
        Vector3 spawnPos = GameManager.gamemanager.spawnPoint[Random.Range(0, GameManager.gamemanager.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.gamemanager.respawnTime));
        StartCoroutine(CountdownAndHideMessage(10f));
    }
    #endregion

    IEnumerator Spawn(Vector3 spawnPos,float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        dead = false;
        transform.position = spawnPos;
        currentHP = maxHP;
        currentMP = maxMP;
        rb.isKinematic = false;
        //respawn.SetActive(false);
        panelDie.SetActive(false);
        skillUI.SetActive(true);
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
    }
    [PunRPC]
    void Heal(int amountToHeal)
    {
        currentHP = Mathf.Clamp(currentHP + amountToHeal, 0, maxHP);
        messageText.text = " You have picked up the chicken thighs " +"+"+ amountToHeal.ToString("N0")+" HP ";
        messageText.color = Color.yellow;
        StartCoroutine(HideMessageAfterDelay(2f));
        UpdateHealthSlider(currentHP);
        UpdateHpText(currentHP, maxHP, currentMP, maxMP);
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
    [PunRPC]
     void EarnExp(int xpAmount)
    {
        currentExp += xpAmount;
        if (expPopUp != null)
        {
            Vector3 popUpPosition = transform.position + new Vector3(0,0.4f,0);
            GameObject instance = Instantiate(expPopUp, popUpPosition, Quaternion.identity);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = "+"+xpAmount.ToString("N0") +" EXP ";
            Animator animator = instance.GetComponentInChildren<Animator>();
            if (xpAmount<=1000)
            {
                animator.Play("normal");
            }
        }
        LevelUp();
        PlayerPrefs.SetInt("CurrentExp", currentExp);
        UpdateExpSlider(currentExp, maxExp, playerLevel);
        playerstatus.photonView.RPC("UpdatePlayerLevel", RpcTarget.All, playerLevel);
    }
     public void LevelUp()
    {
        while(currentExp>=maxExp)
        {
            Instantiate(levelupEffect, transform.position, Quaternion.identity);
            currentExp -= maxExp;
            maxExp=(int)(maxExp * 1.1f);
            playerLevel++;
            damageMin += 10;
            damageMax += 10;
            currentHP += 20;
            maxHP += 20;
            playerstatus.photonView.RPC("UpdatePlayerLevel", RpcTarget.All,playerLevel);
            UpdateExpSlider(currentExp, maxExp, playerLevel);
            PlayerPrefs.SetInt("CurrentExp", currentExp);
            PlayerPrefs.SetInt("MaxExp", maxExp);
            PlayerPrefs.SetInt("PlayerLevel", playerLevel);
            PlayerPrefs.SetInt("DamageMin", damageMin);
            PlayerPrefs.SetInt("DamageMax", damageMax);
            PlayerPrefs.SetInt("maxHP", maxHP);
            PlayerPrefs.SetInt("currentHP", currentHP);
        }
    }
    public void PlayerStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
        healthSlider.value = 1.0f;
    }
    void UpdateHpText(int curHP, int maxHP,int curMP,int maxMP)
    {
        hpText.text = curHP + "/" + maxHP;
        mpText.text = curMP + "/" + maxMP;

    }
    void UpdateHealthSlider(int heal)
    {
        healthSlider.value = (float)heal / maxHealthValue;
    }
    public void UpdateExpSlider(int currentExp, int maxExp,int level)
    {
        ExpSlider.maxValue = maxExp;
        ExpSlider.value = currentExp;

        float percentage = (float)currentExp / maxExp * 100f;
        string formattedPercentage = " LV. " + level +" + "+ percentage.ToString(" 0.00 ") + "%";
        expplayerText.text = formattedPercentage;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(attackpoint.position, attackRange);
    }

}