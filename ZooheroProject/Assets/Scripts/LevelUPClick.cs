using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Model;
using TMPro;

public class LevelUPClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // 外部引用（需在Inspector面板拖拽赋值）
    [Header("核心配置")]
    public OutsiderDevelopmentData Data; // 养成数据
    [Tooltip("初始触发间隔（秒）")]
    public float initTriggerInterval = 0.5f; // 初始间隔（松手后复原到此值）
    [Tooltip("最小触发间隔（秒）- 防止加速过快")]
    public float minTriggerInterval = 0.05f; // 最小间隔（建议≥0.05，避免性能问题）
    [Tooltip("间隔衰减速率（秒/秒）- 值越大加速越快")]
    public float intervalDecayRate = 0.1f; // 每长按1秒，间隔减少0.1秒

    public bool OnPlayAni = false;
    
    [Header("UI对象")]
    public GameObject _LevelUpImageBack;
    public GameObject _LevelUpImage;
    public GameObject _CurrentLevel;
    public GameObject _LevelText1;
    public GameObject _LevelText2;
    public GameObject _LevelUpSlider;
    public GameObject _CurrentLevelValue;
    public GameObject _NextLevelValue;
    public GameObject _LevelText;
    public GameObject _LevelUpPrice;
    public GameObject _OutsiderCurrencyCultivation1;
    public GameObject _OOutsiderDevelopment;
    

    // 组件缓存（Awake中一次性获取）
    private Image _levelUpImageBackImg;
    private Animator _levelUpImageBackImgAni;
    private Image _levelUpImageImg;
    private Image _currentLevelImg;
    private TextMeshProUGUI _levelText1Tmp;
    private TextMeshProUGUI _levelText2Tmp;
    private Slider _levelUpSliderComp;
    private TextMeshProUGUI _currentLevelValueTmp;
    private TextMeshProUGUI _nextLevelValueTmp;
    private TextMeshProUGUI _levelTextTmp;
    private TextMeshProUGUI _levelUpPriceTmp;
    private TextMeshProUGUI _outsiderCurrencyCultivationTmp;
    private OutsiderDevelopment __OOutsiderDevelopment;
    
    
    // 长按状态
    private bool isButtonPressed = false;
    private Coroutine levelUpCoroutine;
    private float currentTriggerInterval; // 实时变化的触发间隔
    private float pressStartTime; // 长按开始时间（用于计算持续时长）

    private void Awake()
    {
        // 初始化UI对象（如需通过Find查找，保留此逻辑；否则注释掉，仅用Inspector赋值）
        InitUIObjects();
        
        // 一次性获取所有组件（无空值容错）
        InitComponentCache();
        
        // 初始化触发间隔为初始值
        currentTriggerInterval = initTriggerInterval;
    }

    /// <summary>
    /// 初始化UI对象（可选：保留Find逻辑，或仅用Inspector赋值）
    /// </summary>
    private void InitUIObjects()
    {
        _LevelUpImageBack = GameObject.Find("LevelUpImageBack");
        _LevelUpImage = GameObject.Find("LevelUpImage");
        _CurrentLevel = GameObject.Find("CurrentLevel");
        _LevelText1 = GameObject.Find("LevelText1");
        _LevelText2 = GameObject.Find("LevelText2");
        _LevelUpSlider = GameObject.Find("LevelUpSlider");
        _CurrentLevelValue = GameObject.Find("CurrentLevelValue");
        _NextLevelValue = GameObject.Find("NextLevelValue");
        _LevelText = GameObject.Find("LevelText");
        _LevelUpPrice = GameObject.Find("LevelUpPrice");
        _OutsiderCurrencyCultivation1 = GameObject.Find("OutsiderCurrencyCultivation1");
        __OOutsiderDevelopment = GameObject.Find("OutsiderDevelopment").GetComponent<OutsiderDevelopment>();
    }

    /// <summary>
    /// 初始化组件缓存（Awake中一次性获取，无空值容错）
    /// </summary>
    private void InitComponentCache()
    {
        _levelUpImageBackImg = _LevelUpImageBack.GetComponent<Image>();
        _levelUpImageBackImgAni = _LevelUpImageBack.GetComponent<Animator>();
        _levelUpImageImg = _LevelUpImage.GetComponent<Image>();
        _currentLevelImg = _CurrentLevel.GetComponent<Image>();
        _levelText1Tmp = _LevelText1.GetComponent<TextMeshProUGUI>();
        _levelText2Tmp = _LevelText2.GetComponent<TextMeshProUGUI>();
        _levelUpSliderComp = _LevelUpSlider.GetComponent<Slider>();
        _currentLevelValueTmp = _CurrentLevelValue.GetComponent<TextMeshProUGUI>();
        _nextLevelValueTmp = _NextLevelValue.GetComponent<TextMeshProUGUI>();
        _levelTextTmp = _LevelText.GetComponent<TextMeshProUGUI>();
        _levelUpPriceTmp = _LevelUpPrice.GetComponent<TextMeshProUGUI>();
        _outsiderCurrencyCultivationTmp = _OutsiderCurrencyCultivation1.GetComponent<TextMeshProUGUI>();
    }

    #region 长按检测核心逻辑（动态加速）
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (OnPlayAni==false)
        {
            
            isButtonPressed = true;
            pressStartTime = Time.time;
            currentTriggerInterval = initTriggerInterval;

            if (levelUpCoroutine == null)
            {
                levelUpCoroutine = StartCoroutine(ContinuousLevelUpCoroutine());
            }
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        //Palypred保存数据
        PlayerPrefsSave();
        
        ResetPressState();
        
        // _levelUpImageBackImgAni.Play("LevelUpImage",0,0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Palypred保存数据
        PlayerPrefsSave();
        
        ResetPressState();
        
        // _levelUpImageBackImgAni.Play("LevelUpImage",0,0);
    }

    /// <summary>
    /// 重置长按状态（松手/移出时调用）
    /// </summary>
    private void ResetPressState()
    {
        isButtonPressed = false;
        currentTriggerInterval = initTriggerInterval;

        if (levelUpCoroutine != null)
        {
            StopCoroutine(levelUpCoroutine);
            levelUpCoroutine = null;
        }
    }

    /// <summary>
    /// 持续升级协程（动态调整触发间隔）
    /// </summary>
    private IEnumerator ContinuousLevelUpCoroutine()
    {
        while (isButtonPressed)
        {
            if (CanLevelUp())
            {
                LevelUpButtonClick();
            }
            else
            {
                ResetPressState();
                break;
            }

            // 计算长按持续时长，动态调整触发间隔
            float pressDuration = Time.time - pressStartTime;
            currentTriggerInterval = Mathf.Max(
                initTriggerInterval - (pressDuration * intervalDecayRate),
                minTriggerInterval
            );

            yield return new WaitForSeconds(currentTriggerInterval);
        }
    }
    #endregion

    #region 升级业务逻辑
    /// <summary>
    /// 核心升级逻辑
    /// </summary>
    private void LevelUpButtonClick()
    {
        
        
        int cost = 1;

        // 扣减养成货币
        GameManager.Instance.OutsiderCurrencyCultivation -= cost;
        if (OnPlayAni==false)
        {
            _levelUpImageBackImgAni.Play("LevelUpImageIng",0,0);
        }
       
        // 更新升级进度
        Data.priceRecord += cost;
        if (Data.priceRecord >= Data.price[Data.currentLevel])
        {
            Data.priceRecord = 0;
            Data.currentLevel++;
            
            _levelUpImageBackImgAni.Play("LevelUPImageLevelUP",0,0);
            StartCoroutine(ResetPlayAniAfterAni());
            OnPlayAni = true;
            
            
            __OOutsiderDevelopment.Instance.SetOutsiderDevelopment();
        }

        // 刷新UI
        RefreshLevelUpUI();
        
        // 满级校验
        if (Data.currentLevel >= Data.numberOfLevels)
        {
            SetMaxLevelUI();
            ResetPressState();
        }
    }

    private IEnumerator ResetPlayAniAfterAni()
    {
        // 等待动画播放完成（时长需与实际动画一致）
        yield return new WaitForSeconds(GameManager.Instance.GetAnimationClip("LevelUPImageLevelUP",_levelUpImageBackImgAni).length);
        // 重置标记
        OnPlayAni = false;
        // 可选：动画完成后恢复默认动画
        // _levelUpImageBackImgAni.Play("LevelUpImage", 0, 0);
    }

    /// <summary>
    /// 升级条件校验
    /// </summary>
    /// <returns>是否可升级</returns>
    private bool CanLevelUp()
    {
        // 满级校验
        if (Data.currentLevel >= Data.numberOfLevels)
        {
            _levelUpImageBackImgAni.Play("LevelUpImage",0,0);
            return false;
        }

        // 货币校验
        int needCost = 1;
        if (GameManager.Instance.OutsiderCurrencyCultivation < needCost)
        {
            // Debug.Log("养成货币不足，无法升级");
            _levelUpImageBackImgAni.Play("LevelUpImage",0,0);
            return false;
        }

        return true;
    }
    #endregion

    #region UI刷新逻辑
    /// <summary>
    /// 设置养成数据并初始化UI
    /// </summary>
    /// <param name="data">养成数据</param>
    public void SetLevelUpData(OutsiderDevelopmentData data)
    {
        this.Data = data;
        if (data.currentLevel >= data.numberOfLevels)
        {
            SetMaxLevelUI();
        }
        else
        {
            SetUnMaxLevelUI();
        }
    }

    /// <summary>
    /// 刷新升级UI
    /// </summary>
    private void RefreshLevelUpUI()
    {
        if (Data.currentLevel < Data.numberOfLevels)
        {
            SetUnMaxLevelUI();
        }
        else
        {
            SetMaxLevelUI();
        }
        UpdateLevelSlider();
        _LevelUpPrice.GetComponent<TextMeshProUGUI>().SetText((Data.price[Data.currentLevel]-Data.priceRecord).ToString());
        
        // 刷新货币显示
        _outsiderCurrencyCultivationTmp.SetText(GameManager.Instance.OutsiderCurrencyCultivation.ToString());
        

    }

    private void PlayerPrefsSave()
    {
        PlayerPrefs.SetInt("OutsiderCurrencyCultivation",GameManager.Instance.OutsiderCurrencyCultivation);
        foreach (OutsiderDevelopmentData outsiderDevelopmentData in GameManager.Instance.RealOutsiderDevelopmentDatas)
        {
            if (outsiderDevelopmentData.id == this.Data.id)
            {
                outsiderDevelopmentData.currentLevel = this.Data.currentLevel;
                outsiderDevelopmentData.priceRecord = this.Data.priceRecord;
            }
        }
        
        GameManager.Instance.SaveByPlayerPrefs(Data.enName,Data);
        
        
    }

    /// <summary>
    /// 未满级UI设置
    /// </summary>
    private void SetUnMaxLevelUI()
    {
        // 设置按钮背景色
        Color targetColor;
        if (ColorUtility.TryParseHtmlString(Data.color, out targetColor))
        {
            _levelUpImageBackImg.color = targetColor;
        }

        // 设置图片
        _levelUpImageImg.sprite = UnityEngine.Resources.Load<Sprite>(ClearSpritePathSuffix(Data.image));
        _currentLevelImg.sprite = UnityEngine.Resources.Load<Sprite>(GameManager.Instance.setNum(Data.currentLevel));

        // 设置文本
        _currentLevelValueTmp.SetText(Data.Value[Data.currentLevel].ToString());
        _nextLevelValueTmp.SetText(Data.Value[Data.currentLevel + 1].ToString());
        _levelUpPriceTmp.SetText(Data.price[Data.currentLevel].ToString());
        _levelTextTmp.SetText($"LV.{Data.currentLevel}  ->  LV.{Data.currentLevel + 1}");
    }

    /// <summary>
    /// 满级UI设置
    /// </summary>
    private void SetMaxLevelUI()
    {
        // 满级金色样式
        _levelUpImageBackImg.color = Color.gold;
        
        _currentLevelValueTmp.SetText($"LV.{Data.currentLevel}（已满级）");
        _nextLevelValueTmp.SetText("MAX");
        _levelTextTmp.SetText(Data.Value[Data.currentLevel].ToString());
    }

    /// <summary>
    /// 更新升级进度条
    /// </summary>
    private void UpdateLevelSlider()
    {
        
        
        
        _levelUpSliderComp.maxValue = Data.price[Data.currentLevel];
        _levelUpSliderComp.value = Data.priceRecord;
    }

    /// <summary>
    /// 清理Sprite路径后缀
    /// </summary>
    /// <param name="path">原始路径</param>
    /// <returns>清理后的路径</returns>
    private string ClearSpritePathSuffix(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return path.Replace(".png", "").Replace(".jpg", "").Replace("Assets/Resources/", "");
    }
    #endregion

    /// <summary>
    /// 防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        ResetPressState();
    }
}