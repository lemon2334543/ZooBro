using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public static GamePanel Instance;

    public Slider _hpSlider;
    public Slider _expSlider;
    public Slider _armorpSlider;
    public TMP_Text _moneyCount;//金币
    public TMP_Text _hpCount;//生命值
    public TMP_Text _armorount;//护甲值
    public TMP_Text _countDown;//关卡倒计时
    public TMP_Text _waveCount;//波次

    private void Awake()
    {
        Instance = this;
        // 查找UI组件并获取控制权
        _hpSlider = GameObject.Find("HpSlider").GetComponent<Slider>();
        _expSlider = GameObject.Find("ExpSlider").GetComponent<Slider>();
        _moneyCount = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        _hpCount = GameObject.Find("HpCount").GetComponent<TMP_Text>();
        _countDown = GameObject.Find("CountDown").GetComponent<TMP_Text>();
        _waveCount = GameObject.Find("WaveCount").GetComponent<TMP_Text>();
        _armorpSlider = GameObject.Find("ArmorSlider").GetComponent<Slider>();
        _armorount = GameObject.Find("ArmorCount").GetComponent<TMP_Text>();
    }

    void Start()
    {
        // 初始化UI显示
        RenewExp();
        RenewHp();
        RenewMoney();
        RenewWaveCount();
        RenewArmor(); // 初始化护甲显示
    }

    /// <summary>
    /// 更新金币显示
    /// </summary>
    public void RenewMoney()
    {
        _moneyCount.text = GameManager.Instance.money.ToString();
    }

    /// <summary>
    /// 更新生命值显示（自适应血条宽度）
    /// </summary>
    public void RenewHp()
    {
        float maxHp = GameManager.Instance.propData.maxHp;
        float currentHp = GameManager.Instance.hp;

        // 血条自适应宽度计算
        RectTransform hpSliderRect = _hpSlider.GetComponent<RectTransform>();
        SetSliderLayout(hpSliderRect, 20);
        float hpSliderWidth = CalculateAdaptiveWidth(maxHp);
        hpSliderRect.sizeDelta = new Vector2(hpSliderWidth, hpSliderRect.sizeDelta.y);

        // 更新文本和滑块值
        _hpCount.text = $"{currentHp}/{maxHp}";
        _hpSlider.value = currentHp / maxHp;
    }

    /// <summary>
    /// 更新经验值显示
    /// </summary>
    public void RenewExp()
    {
        float totalExp = GameManager.Instance.exp;
        _expSlider.value = totalExp % 12 / 12;
    }

    /// <summary>
    /// 更新倒计时显示
    /// </summary>
    public void RenewCountDown(float time)
    {
        _countDown.text = time.ToString("F0");
    }

    /// <summary>
    /// 更新波次显示
    /// </summary>
    public void RenewWaveCount()
    {
        _waveCount.text = "第" + GameManager.Instance.currentWave.ToString() + "关";
    }

    /// <summary>
    /// 更新护甲显示（自适应护甲条宽度）
    /// </summary>
    public void RenewArmor()
    {
        float maxHp = GameManager.Instance.propData.maxHp;
        float currentArmor = GameManager.Instance.Armor;

        // 限制护甲不超过最大生命值
        if (currentArmor > maxHp)
            currentArmor = maxHp;
        GameManager.Instance.Armor = currentArmor;

        // 护甲条自适应宽度计算
        RectTransform armorSliderRect = _armorpSlider.GetComponent<RectTransform>();
        SetSliderLayout(armorSliderRect, 10);
        float armorSliderWidth = CalculateAdaptiveWidth(maxHp);
        armorSliderRect.sizeDelta = new Vector2(armorSliderWidth, armorSliderRect.sizeDelta.y);

        // 更新文本、滑块值和显示状态
        _armorount.text = currentArmor.ToString();
        _armorpSlider.value = currentArmor / maxHp;

        // 护甲为0时隐藏护甲UI
        CanvasGroup armorCanvasGroup = GameObject.Find("Armor").GetComponent<CanvasGroup>();
        armorCanvasGroup.alpha = currentArmor > 0 ? 1 : 0;
    }

    /// <summary>
    /// 设置滑块布局（固定左边界）
    /// </summary>
    private void SetSliderLayout(RectTransform sliderRect, float xPos)
    {
        sliderRect.anchorMin = new Vector2(0, 0.5f);
        sliderRect.anchorMax = new Vector2(0, 0.5f);
        sliderRect.pivot = new Vector2(0, 0.5f);
        sliderRect.anchoredPosition = new Vector2(xPos, 0);
    }

    /// <summary>
    /// 计算自适应宽度（分段缩放）
    /// </summary>
    private float CalculateAdaptiveWidth(float maxValue)
    {
        float baseWidthPerUnit = 15f;
        float scaleFactor = 0.95f;
        float maxWidth = 0;

        if (maxValue <= 20)
        {
            maxWidth = baseWidthPerUnit * maxValue;
        }
        else
        {
            // 前20单位按基础宽度，后续每20单位按缩放系数
            maxWidth += 20 * baseWidthPerUnit;
            float remainingValue = maxValue - 20;
            int fullSegments = Mathf.FloorToInt(remainingValue / 20f);
            float lastSegmentValue = remainingValue % 20f;

            for (int i = 0; i < fullSegments; i++)
            {
                maxWidth += 20 * baseWidthPerUnit * scaleFactor;
            }
            maxWidth += lastSegmentValue * baseWidthPerUnit * scaleFactor;
        }

        return maxWidth;
    }
}