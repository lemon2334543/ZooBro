using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public static GamePanel Instance;

    public Slider _hpSlider;
    public Slider _expSlider;
    public TMP_Text _moneyCount;//金币
    public TMP_Text _expCount;//等级
    public TMP_Text _hpCount;//生命值
    public TMP_Text _countDown;//关卡倒计时
    public TMP_Text _waveCount;//波次


    private void Awake()
    {
        Instance = this;
        //找到 对应的对象，找到名字HpSlider获得Slider组件控制权
        _hpSlider = GameObject.Find("HpSlider").GetComponent<Slider>();
        _expSlider = GameObject.Find("ExpSlider").GetComponent<Slider>();
        _moneyCount = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        _expCount = GameObject.Find("ExpCount").GetComponent<TMP_Text>();
        _hpCount = GameObject.Find("HpCount").GetComponent<TMP_Text>();
        _countDown = GameObject.Find("CountDown").GetComponent<TMP_Text>();
        _waveCount = GameObject.Find("WaveCount").GetComponent<TMP_Text>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //更新经验条
        RenewExp();
        //更新生命中
        RenewHp();
        //更新金币
        RenewMoney();
        //更新波次信息
        RenewWaveCount();
    }

    public void RenewMoney()
    {
        _moneyCount.text = Player.Instance.money.ToString();
    }

    public void RenewHp()
    {
        //获取文本
        _hpCount.text = Player.Instance.hp + "/" + Player.Instance.maxHp;
        _hpSlider.value = Player.Instance.hp  /  Player.Instance.maxHp;

    }

    public void RenewExp()
    {
        // %除余 剩下多少再 / 12
        _expSlider.value = Player.Instance.exp % 12 / 12;
        _expCount.text = "LV." + Player.Instance.exp / 12;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //更新倒计时
    public void RenewCountDown(float time)
    {
        //F0 只取整数
        _countDown.text = time.ToString("F0");
    }

    //更新波次
    public void RenewWaveCount()
    {
        _waveCount.text = "第" + GameManager.Instance.currentWave.ToString() + "关";
    }


}
