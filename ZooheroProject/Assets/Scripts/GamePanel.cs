using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public static GamePanel Instance;

    public Slider _hpSlider;
    public Slider _expSlider;
    public TMP_Text _moneyCount;//���
    public TMP_Text _expCount;//�ȼ�
    public TMP_Text _hpCount;//����ֵ
    public TMP_Text _countDown;//�ؿ�����ʱ
    public TMP_Text _waveCount;//����


    private void Awake()
    {
        Instance = this;
        //�ҵ� ��Ӧ�Ķ����ҵ�����HpSlider���Slider�������Ȩ
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
        //���¾�����
        RenewExp();
        //����������
        RenewHp();
        //���½��
        RenewMoney();
        //���²�����Ϣ
        RenewWaveCount();
    }

    public void RenewMoney()
    {
        _moneyCount.text = Player.Instance.money.ToString();
    }

    public void RenewHp()
    {
        //��ȡ�ı�
        _hpCount.text = Player.Instance.hp + "/" + Player.Instance.maxHp;
        _hpSlider.value = Player.Instance.hp  /  Player.Instance.maxHp;

    }

    public void RenewExp()
    {
        // %���� ʣ�¶����� / 12
        _expSlider.value = Player.Instance.exp % 12 / 12;
        _expCount.text = "LV." + Player.Instance.exp / 12;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //���µ���ʱ
    public void RenewCountDown(float time)
    {
        //F0 ֻȡ����
        _countDown.text = time.ToString("F0");
    }

    //���²���
    public void RenewWaveCount()
    {
        _waveCount.text = "��" + GameManager.Instance.currentWave.ToString() + "��";
    }


}
