using System;
using Resources.script.model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Roleset : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    public Roleset Instense;
    public RoleDate roleDate;
    public Image _backgroundimage; //背景
    public Image _avater; // 头像
    public Button _button; //按钮

    public GameObject _Roimage;
    public Image _Roimages;
    public GameObject _backring;

    public GameObject _showlock;
    
    //角色展示
    public GameObject _showimage;
    public Animator _Animator;
    private Animator _infoAnimator;
    
    public GameObject _roleshow;
    
    public string jumpin = "RO-image";
    public string roleinfoin = "roleinfoin";

    public GameObject _roleInfotPanal;



    private void Awake()
    {
        Instense = this;
        _backgroundimage = GetComponent<Image>();
        _avater = transform.GetChild(0).GetComponent<Image>();
        _button = GetComponent<Button>();
        _backring = GameObject.Find("backring");//选中绿色背景
        
        _Roimage = GameObject.Find("Ro-image");
        _Roimages = _Roimage.GetComponent<Image>();
        _showlock = GameObject.Find("showlock");
        
        _showimage = GameObject.Find("Ro-image");
        _Animator = _showimage.GetComponent<Animator>();
        _infoAnimator = GameObject.Find("RO-Infooanel").GetComponent<Animator>();
        _roleshow = GameObject.Find("Ro-image");

        _roleInfotPanal = GameObject.Find("RO-Infooanel");



    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDate(RoleDate roleDate)
    {

        this.roleDate = roleDate;


        if (this.roleDate.unlock == 0)
        {
            _avater.sprite = UnityEngine.Resources.Load<Sprite>("Image/UI/锁");
            
        }
        else
        {
            _avater.sprite = UnityEngine.Resources.Load<Sprite>(roleDate.avatar);
            
        }
        //点击监听
        _button.onClick.AddListener((() =>
        {
            ButtonClick(roleDate);
        }));
        
    }

    public void ButtonClick(RoleDate roleDate1)
    {
        if (GameManger.Instance.RoleDate.id!=roleDate1.id)
        {
            if (_roleshow.GetComponent<CanvasGroup>().alpha==0)
            {
                _roleshow.GetComponent<CanvasGroup>().alpha = 1;
            }

            
            RenewUI(roleDate);
            setImage(roleDate1);
            GameManger.Instance.RoleDate = roleDate1;
            roleInfotPanalshow();
            
           
            Instense = this;
            Debug.Log(Instense.transform.position.x);
            Debug.Log(Instense.transform.position.y);
            Debug.Log(Instense);
            
            _backring.transform.position = new Vector3(Instense.transform.position.x, Instense.transform.position.y, 0);
            
        }
          
        

    }

    private void roleInfotPanalshow()
    {
        if (_roleInfotPanal.GetComponent<CanvasGroup>().alpha==0)
        {
            _roleInfotPanal.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    private void setImage(RoleDate roleDate1)
    {
        _Roimages.sprite = UnityEngine.Resources.Load<Sprite>(roleDate1.avatar);
        if (roleDate1.unlock==0)
        {
            // _Animator.Play(jumpin, 0, 0f);
            
            _Roimages.color = Color.black;
            _showlock.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            
            _Animator.Play(jumpin, 0, 0f);
            _Roimages.color = Color.white;
            _showlock.GetComponent<CanvasGroup>().alpha = 0;
        }
      
            
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        //背景颜色
        this._backgroundimage.color = new Color(207/255f, 207/255f, 207/255f);
        

        
    }

    private void RenewUI(RoleDate roleDate1)
    {
        if (roleDate1.unlock==0)
        {
            rolepanel.Instance._rolename.text = "???";
            rolepanel.Instance._abater.sprite = UnityEngine.Resources.Load<Sprite>("Image/UI/锁");
            rolepanel.Instance._RoleDes.text = roleDate1.unlockConditions;
            // rolepanel.Instance._Text2.text = "尚无记录";
        }
        else//已解锁
        {
            _infoAnimator.Play(roleinfoin, 0, 0f);
            rolepanel.Instance._rolename.text = roleDate1.name;
            rolepanel.Instance._RoleDes.text = roleDate1.describe;
            rolepanel.Instance._abater.sprite = UnityEngine.Resources.Load<Sprite>(roleDate1.avatar);
            // rolepanel.Instance._Text2.text = GetRecord(roleDate1.record);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this._backgroundimage.color = new Color(34/255f, 34/255f, 34/255f);   
    }
}
