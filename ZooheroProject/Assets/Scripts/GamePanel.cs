using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public static GamePanel Instance;

    public Slider _hpSlider;
    public Slider _expSlider;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public TMP_Text _moneyCount;//½ğ±Ò
    public TMP_Text _expCount;//µÈ¼¶
    public TMP_Text _hpCount;//ÉúÃüÖµ
    public TMP_Text _countDown;//¹Ø¿¨µ¹¼ÆÊ±
    public TMP_Text _waveCount;//²¨´Î
=======
=======
>>>>>>> Stashed changes
    public Slider _armorpSlider;
    public TMP_Text _moneyCount;//é‡‘å¸
    // public TMP_Text _expCount;//ç­‰çº§
    public TMP_Text _hpCount;//ç”Ÿå‘½å€¼
    public TMP_Text _armorount;//ç”Ÿå‘½å€¼
    public TMP_Text _countDown;//å…³å¡å€’è®¡æ—¶
    public TMP_Text _waveCount;//æ³¢æ¬¡
>>>>>>> Stashed changes


    private void Awake()
    {
        Instance = this;
        //ÕÒµ½ ¶ÔÓ¦µÄ¶ÔÏó£¬ÕÒµ½Ãû×ÖHpSlider»ñµÃSlider×é¼ş¿ØÖÆÈ¨
        _hpSlider = GameObject.Find("HpSlider").GetComponent<Slider>();
        _expSlider = GameObject.Find("ExpSlider").GetComponent<Slider>();
        _moneyCount = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        // _expCount = GameObject.Find("ExpCount").GetComponent<TMP_Text>();
        _hpCount = GameObject.Find("HpCount").GetComponent<TMP_Text>();
        _countDown = GameObject.Find("CountDown").GetComponent<TMP_Text>();
        _waveCount = GameObject.Find("WaveCount").GetComponent<TMP_Text>();
        
        _armorpSlider = GameObject.Find("ArmorSlider").GetComponent<Slider>();
        _armorount = GameObject.Find("ArmorCount").GetComponent<TMP_Text>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //¸üĞÂ¾­ÑéÌõ
        RenewExp();
        //¸üĞÂÉúÃüÖĞ
        RenewHp();
        //¸üĞÂ½ğ±Ò
        RenewMoney();
        //¸üĞÂ²¨´ÎĞÅÏ¢
        RenewWaveCount();
    }
    
    

    public void RenewMoney()
    {
        _moneyCount.text = Player.Instance.money.ToString();
    }

    public void RenewHp()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //»ñÈ¡ÎÄ±¾
        _hpCount.text = Player.Instance.hp + "/" + Player.Instance.maxHp;
        _hpSlider.value = Player.Instance.hp  /  Player.Instance.maxHp;
=======
=======
>>>>>>> Stashed changes
        RectTransform hpSliderRect = _hpSlider.GetComponent<RectTransform>();

// å›ºå®šå·¦è¾¹ç•Œå¸ƒå±€
        hpSliderRect.anchorMin = new Vector2(0, 0.5f);
        hpSliderRect.anchorMax = new Vector2(0, 0.5f);
        hpSliderRect.pivot = new Vector2(0, 0.5f);
        hpSliderRect.anchoredPosition = new Vector2(20, 0);

// è·å–æœ€å¤§è¡€é‡
        float maxHp = GameManager.Instance.propData.maxHp;
        float b = 15f;
        float targetWidth=0;
        float q = 0.95f;
// åˆ†æ®µè®¡ç®—å®½åº¦ï¼ˆè‡ªåŠ¨é€‚é…ä»»æ„xå€¼ï¼‰
        if (maxHp<=20)
        {
            targetWidth = b * maxHp;
        }
        else
        {
            // 21    41
            targetWidth += 20 * b;
            for (int i = 2; i < Mathf.FloorToInt(maxHp / 20f); i++)
            {
                targetWidth += b * q;
            }

            targetWidth += Mathf.FloorToInt(maxHp % 20f) * b * q;
        }
        

// åº”ç”¨å®½åº¦
        hpSliderRect.sizeDelta = new Vector2(targetWidth, hpSliderRect.sizeDelta.y);


        Debug.Log(maxHp);
        _hpCount.text = GameManager.Instance.hp + "/" + maxHp;
        _hpSlider.value = GameManager.Instance.hp  / maxHp;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    }

    public void RenewExp()
    {
<<<<<<< Updated upstream
        // %³ıÓà Ê£ÏÂ¶àÉÙÔÙ / 12
        _expSlider.value = Player.Instance.exp % 12 / 12;
        _expCount.text = "LV." + Player.Instance.exp / 12;
=======
        // %é™¤ä½™ å‰©ä¸‹å¤šå°‘å† / 12
        _expSlider.value = GameManager.Instance.exp % 12 / 12;
        // _expCount.text = "LV." + GameManager.Instance.exp / 12;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //¸üĞÂµ¹¼ÆÊ±
    public void RenewCountDown(float time)
    {
        //F0 Ö»È¡ÕûÊı
        _countDown.text = time.ToString("F0");
    }

    //¸üĞÂ²¨´Î
    public void RenewWaveCount()
    {
        _waveCount.text = "µÚ" + GameManager.Instance.currentWave.ToString() + "¹Ø";
    }


    public void RenewArmor()
    {
        if (GameManager.Instance.Armor>GameManager.Instance.propData.maxHp)
        {
            GameManager.Instance.Armor = GameManager.Instance.propData.maxHp;
        }
        
        RectTransform armorSliderRect = _armorpSlider.GetComponent<RectTransform>();

// å›ºå®šå·¦è¾¹ç•Œå¸ƒå±€
        armorSliderRect.anchorMin = new Vector2(0, 0.5f);
        armorSliderRect.anchorMax = new Vector2(0, 0.5f);
        armorSliderRect.pivot = new Vector2(0, 0.5f);
        armorSliderRect.anchoredPosition = new Vector2(10, 0);

// è·å–æœ€å¤§è¡€é‡
        float maxHp = GameManager.Instance.propData.maxHp;
        float b = 15f;
        float targetWidth=0;
        float q = 0.95f;
// åˆ†æ®µè®¡ç®—å®½åº¦ï¼ˆè‡ªåŠ¨é€‚é…ä»»æ„xå€¼ï¼‰
        if (maxHp<=20)
        {
            targetWidth = b * maxHp;
        }
        else
        {
            // 21    41
            targetWidth += 20 * b;
            for (int i = 2; i < Mathf.FloorToInt(maxHp / 20f); i++)
            {
                targetWidth += b * q;
            }

            targetWidth += Mathf.FloorToInt(maxHp % 20f) * b * q;
        }
        

// åº”ç”¨å®½åº¦
        armorSliderRect.sizeDelta = new Vector2(targetWidth, armorSliderRect.sizeDelta.y);


        
        _armorount.text = GameManager.Instance.Armor.ToString();
        _armorpSlider.value = GameManager.Instance.Armor  / maxHp;

        if (GameManager.Instance.Armor==0)
        {
            GameObject.Find("Armor").GetComponent<CanvasGroup>().alpha = 0;
        }
        else if(GameManager.Instance.Armor>0)
        {
            GameObject.Find("Armor").GetComponent<CanvasGroup>().alpha = 1;
        }

    }
}
