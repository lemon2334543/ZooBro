using System;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class OutsiderLevelUpPanel : MonoBehaviour
{
    public static OutsiderLevelUpPanel Instance;
    public OutsiderDevelopmentData Data;
    
    // UI对象
    public GameObject _LevelUpImageBack;
    public Animator LevelUPImageLevelAni;
    public GameObject _LevelUpImage;
    public GameObject _CurrentLevel;
    public GameObject _LevelText1;
    public GameObject _LevelText2;
    public GameObject _LevelUpSlider;
    public GameObject _CurrentLevelValue;
    public GameObject _NextLevelValue;
    public GameObject _LevelText;
    public GameObject _LevelUpPrice;

    public Button LevelUpClick;
    

    private void Awake()
    {
        Instance = this;
        
        // 查找UI对象（建议改为Inspector拖拽赋值，避免Find失败）
        _LevelUpImageBack = GameObject.Find("LevelUpImageBack");
        LevelUPImageLevelAni = _LevelUpImageBack.GetComponent<Animator>();
        _LevelUpImage = GameObject.Find("LevelUpImage");
        _CurrentLevel = GameObject.Find("CurrentLevel");
        _LevelText1 = GameObject.Find("LevelText1");
        _LevelText2 = GameObject.Find("LevelText2");
        _LevelUpSlider = GameObject.Find("LevelUpSlider");
        _CurrentLevelValue = GameObject.Find("CurrentLevelValue");
        _NextLevelValue = GameObject.Find("NextLevelValue");
        _LevelText = GameObject.Find("LevelText");
        _LevelUpPrice = GameObject.Find("LevelUpPrice"); // 补充查找价格文本
        
        // 获取按钮组件
        LevelUpClick = _LevelUpImageBack.GetComponent<Button>();
    }



    #region UI设置逻辑
    public void setData(OutsiderDevelopmentData outsiderDevelopmentData)
    {
        this.Data = outsiderDevelopmentData;
        int CurrentLevel = outsiderDevelopmentData.currentLevel;
        LevelUPImageLevelAni.Play("LevelUpImageShow",0,0);
        if (_LevelUpImageBack != null)
        {
            // 1. 生成 -5 到 5 之间的随机浮点数（包含边界值）
            float randomZRotation = UnityEngine.Random.Range(-5f, 5f);
            // 2. 获取当前旋转，仅修改Z轴，保留X/Y轴原有旋转
            UnityEngine.Quaternion currentRot = _LevelUpImage.transform.rotation;
            // 3. 应用新的Z轴旋转（两种方式选其一）
            // 方式1：直接设置欧拉角（更直观）
            _LevelUpImage.transform.rotation = UnityEngine.Quaternion.Euler(
                currentRot.eulerAngles.x,
                currentRot.eulerAngles.y,
                randomZRotation
            );
        }

        // 玩家该属性已满级时
        if (CurrentLevel >= outsiderDevelopmentData.numberOfLevels)
        {
            setMaxLevel();
        }
        // 未满级
        else
        {
            setUnMaxLevel();
        }
    }

    private void setUnMaxLevel()
    {
        // 启用按钮交互
        LevelUpClick.interactable = true;
        
        // 设置颜色
        Color targetColor;
        if (ColorUtility.TryParseHtmlString(Data.color, out targetColor))
        {
            _LevelUpImageBack.GetComponent<Image>().color = targetColor;
        }
        
        // 设置图片
        _LevelUpImage.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>(ClearSpriteSuffix(Data.image));
        _CurrentLevel.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>(GameManager.Instance.setNum(Data.currentLevel));

        // 设置文本
        _LevelText1.GetComponent<TextMeshProUGUI>().text = Data.title;
        _LevelText2.GetComponent<TextMeshProUGUI>().text = Data.text;
        _CurrentLevelValue.GetComponent<TextMeshProUGUI>().text = Data.Value[Data.currentLevel].ToString();
        _NextLevelValue.GetComponent<TextMeshProUGUI>().text = Data.Value[Data.currentLevel + 1].ToString();
        _LevelUpPrice.GetComponent<TextMeshProUGUI>().text = (Data.price[Data.currentLevel]).ToString();
        _LevelText.GetComponent<TextMeshProUGUI>().text = $"LV.{Data.currentLevel}  ->  LV.{Data.currentLevel + 1}";
        _LevelUpSlider.GetComponent<Slider>().value = Data.priceRecord;
        _LevelUpSlider.GetComponent<Slider>().maxValue = Data.price[Data.currentLevel];
    }



    private void setMaxLevel()
    {
        // 禁用按钮交互
        LevelUpClick.interactable = false;
        
        // 设置满级样式（灰色遮罩/提示文本）
        _LevelUpImageBack.GetComponent<Image>().color = Color.gold;
        _LevelText.GetComponent<TextMeshProUGUI>().text = $"LV.{Data.currentLevel}（已满级）";
        _LevelUpPrice.GetComponent<TextMeshProUGUI>().text = "MAX";
        _NextLevelValue.GetComponent<TextMeshProUGUI>().text = Data.Value[Data.currentLevel].ToString();
    }

    // 工具方法：清除Sprite路径后缀（.png/.jpg）
    private string ClearSpriteSuffix(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        return path.Replace(".png", "").Replace(".jpg", "").Replace("Assets/Resources/", "");
    }
    #endregion

}