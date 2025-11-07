using System;
using Resources.script.model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Roleset : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Roleset Instense;
    public RoleDate roleDate;
    public Image _backgroundimage; // 背景
    public Image _avater; // 头像
    public Button _button; // 按钮

    public GameObject _Roimage;
    public Image _Roimages;
    public GameObject _backring; // 选中绿色背景
    public GameObject _showlock; // 锁定图标

    // 角色展示相关
    public GameObject _showimage;
    public Animator _Animator;
    private Animator _infoAnimator;
    public GameObject _roleshow;

    public string jumpin = "RO-image"; // 角色展示动画
    public string roleinfoin = "roleinfoin"; // 角色信息面板动画

    public GameObject _roleInfotPanal; // 角色信息面板
    public bool isclick = false; // 防止重复点击
    public GameObject _rombutton; // 随机角色按钮
    public GameObject _record; // 最高记录显示
    public TextMeshProUGUI recordtext; // 记录文本
    public GameObject _nextButton; // 下一步按钮

    private void Awake()
    {
        Instense = this;
        // 初始化UI组件引用
        _backgroundimage = transform.GetChild(0).GetComponent<Image>();
        _avater = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _button = GetComponent<Button>();
        _backring = GameObject.Find("backring");
        _Roimage = GameObject.Find("Ro-image");
        _Roimages = _Roimage.GetComponent<Image>();
        _showlock = GameObject.Find("showlock");
        _showimage = GameObject.Find("Ro-image");
        _Animator = _showimage.GetComponent<Animator>();
        _infoAnimator = GameObject.Find("RO-Infooanel").GetComponent<Animator>();
        _roleshow = GameObject.Find("Ro-image");
        _roleInfotPanal = GameObject.Find("RO-Infooanel");
        _rombutton = GameObject.Find("Ro-list-Ranrom");
        _record = GameObject.Find("HighestRecord");
        recordtext = _record.GetComponent<TextMeshProUGUI>();
        _nextButton = GameObject.Find("next");
    }

    void Start()
    {
    }

    void Update()
    {
    }

    /// <summary>
    /// 设置角色数据并初始化UI
    /// </summary>
    public void setDate(RoleDate roleDate)
    {
        this.roleDate = roleDate;

        // 初始化头像和背景色
        if (this.roleDate.unlock == 0)
        {
            _avater.sprite = Resources.Load<Sprite>("Image/UI/锁");
        }
        else
        {
            _avater.sprite = Resources.Load<Sprite>(roleDate.avatar);
            // 根据角色记录设置背景色
            SetBackgroundByRecord(roleDate.record);
        }

        // 绑定按钮点击事件
        _button.onClick.AddListener(() =>
        {
            // 重置其他角色的选中状态
            foreach (Roleset roleset in rolepanel.Instance._rolelist.GetComponentsInChildren<Roleset>())
            {
                if (roleset.roleDate.id != Instense.roleDate.id)
                {
                    roleset.isclick = false;
                }
            }
            _rombutton.GetComponent<RomButton>().isclick = false;

            // 未选中时执行点击逻辑
            if (!isclick)
            {
                ButtonClick(roleDate);
            }
        });
    }

    /// <summary>
    /// 按钮点击逻辑
    /// </summary>
    public void ButtonClick(RoleDate roleDate1)
    {
        isclick = true;

        // 显示角色展示面板
        if (_roleshow.GetComponent<CanvasGroup>().alpha == 0)
        {
            _roleshow.GetComponent<CanvasGroup>().alpha = 1;
        }

        // 更新UI和角色展示
        RenewUI(roleDate);
        setImage(roleDate1);
        GameManager.Instance.RoleDate = roleDate1;
        roleInfotPanalshow();

        // 更新最高记录显示
        UpdateRecordText(roleDate1.record);

        // 移动选中背景到当前角色位置
        _backring.transform.localPosition = new Vector3(Instense.transform.localPosition.x, Instense.transform.localPosition.y, 0);
    }

    /// <summary>
    /// 显示角色信息面板
    /// </summary>
    private void roleInfotPanalshow()
    {
        if (_roleInfotPanal.GetComponent<CanvasGroup>().alpha == 0)
        {
            _roleInfotPanal.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    /// <summary>
    /// 设置角色展示图片
    /// </summary>
    private void setImage(RoleDate roleDate1)
    {
        _Roimages.sprite = Resources.Load<Sprite>(roleDate1.avatar);
        if (roleDate1.unlock == 0)
        {
            _Roimages.color = Color.black;
            _showlock.GetComponent<CanvasGroup>().alpha = 1;
            // 锁定状态下禁用下一步按钮
            SetNextButtonState(false);
        }
        else
        {
            _Animator.Play(jumpin, 0, 0f);
            _Roimages.color = Color.white;
            _showlock.GetComponent<CanvasGroup>().alpha = 0;
            // 解锁状态下启用下一步按钮
            SetNextButtonState(true);
        }
    }

    /// <summary>
    /// 更新角色信息UI
    /// </summary>
    private void RenewUI(RoleDate roleDate1)
    {
        if (roleDate1.unlock == 0)
        {
            rolepanel.Instance._rolename.text = "???";
            rolepanel.Instance._abater.sprite = Resources.Load<Sprite>("Image/UI/锁");
            rolepanel.Instance._RoleDes.text = roleDate1.unlockConditions;
        }
        else
        {
            _infoAnimator.Play(roleinfoin, 0, 0f);
            rolepanel.Instance._rolename.text = roleDate1.name;
            rolepanel.Instance._RoleDes.text = roleDate1.describe;
            rolepanel.Instance._abater.sprite = Resources.Load<Sprite>(roleDate1.avatar);
        }
    }

    /// <summary>
    /// 鼠标悬停时触发
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _backgroundimage.color = Color.white;
    }

    /// <summary>
    /// 鼠标离开时触发
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        SetBackgroundByRecord(this.roleDate.record);
    }

    /// <summary>
    /// 根据角色记录设置背景色
    /// </summary>
    private void SetBackgroundByRecord(int record)
    {
        switch (record)
        {
            case -1:
                _backgroundimage.color = GameManager.Instance.color_1;
                break;
            case 0:
                _backgroundimage.color = GameManager.Instance.color0;
                break;
            case 1:
                _backgroundimage.color = GameManager.Instance.color1;
                break;
            case 2:
                _backgroundimage.color = GameManager.Instance.color2;
                break;
            case 3:
                _backgroundimage.color = GameManager.Instance.color3;
                break;
            case 4:
                _backgroundimage.color = GameManager.Instance.color4;
                break;
            case 5:
                _backgroundimage.color = GameManager.Instance.color5;
                break;
        }
    }

    /// <summary>
    /// 更新记录文本显示
    /// </summary>
    private void UpdateRecordText(int record)
    {
        switch (record)
        {
            case -1:
                recordtext.text = "未通过";
                recordtext.color = GameManager.Instance.color_1;
                break;
            case 0:
                recordtext.text = "难度零";
                recordtext.color = GameManager.Instance.color0;
                break;
            case 1:
                recordtext.text = "难度一";
                recordtext.color = GameManager.Instance.color1;
                break;
            case 2:
                recordtext.text = "难度二";
                recordtext.color = GameManager.Instance.color2;
                break;
            case 3:
                recordtext.text = "难度三";
                recordtext.color = GameManager.Instance.color3;
                break;
            case 4:
                recordtext.text = "难度四";
                recordtext.color = GameManager.Instance.color4;
                break;
            case 5:
                recordtext.text = "难度五";
                recordtext.color = GameManager.Instance.color5;
                break;
        }
    }

    /// <summary>
    /// 设置下一步按钮状态
    /// </summary>
    private void SetNextButtonState(bool isEnable)
    {
        CanvasGroup nextBtnCanvas = _nextButton.GetComponent<CanvasGroup>();
        nextBtnCanvas.alpha = isEnable ? 1 : 0;
        nextBtnCanvas.interactable = isEnable;
        nextBtnCanvas.blocksRaycasts = isEnable;
    }
}