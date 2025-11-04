using System;
using Resources.script.model;
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
=======
using TMPro;
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
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
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs



    private void Awake()
    {
        Instense = this;
        _backgroundimage = GetComponent<Image>();
        _avater = transform.GetChild(0).GetComponent<Image>();
=======
    
    //用于判定是否重复点击
    public bool isclick = false;

    public GameObject _rombutton;

    public GameObject _record;
    public TextMeshProUGUI recordtext;

    public GameObject _nextButton;
    
    private void Awake()
    {
        Instense = this;
        _backgroundimage = transform.GetChild(0).GetComponent<Image>();
        _avater = transform.GetChild(0).GetChild(0).GetComponent<Image>();
<<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
<<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
<<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
========
>>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
========
>>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
========
>>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
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
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs


=======
        
        _rombutton = GameObject.Find("Ro-list-Ranrom"); 
        _record = GameObject.Find("HighestRecord");
        recordtext = _record.GetComponent<TextMeshProUGUI>();

        _nextButton = GameObject.Find("next");
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs

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
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
            
=======
            switch (this.roleDate.record)
            {
                case -1:  
                    this._backgroundimage.color = GameManager.Instance.color_1;
                    break;  
                case 0:
                    this._backgroundimage.color = GameManager.Instance.color0;
                    break;
                case 1:
                    this._backgroundimage.color = GameManager.Instance.color1;
                    break;
                case 2:
                    this._backgroundimage.color = GameManager.Instance.color2;
                    break;
                case 3:
                    this._backgroundimage.color =  GameManager.Instance.color3;
                    break;
                case 4:
                    this._backgroundimage.color = GameManager.Instance.color4;
                    break;
                case 5:
                    this._backgroundimage.color = GameManager.Instance.color5;
                    break;
                default:  
                    
                    break;
            }
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
        }
        //点击监听
        _button.onClick.AddListener((() =>
        {
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
            ButtonClick(roleDate);
=======
            
            foreach (Roleset roleset in rolepanel.Instance._rolelist.GetComponentsInChildren<Roleset>())
            {
                if (roleset.roleDate.id!=Instense.roleDate.id)
                {
                    roleset.isclick = false;
                }
                
            }

            _rombutton.GetComponent<RomButton>().isclick = false;

            if (isclick == false)
            {
                ButtonClick(roleDate);
            }
            
                
            
            
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
        }));
        
    }

    public void ButtonClick(RoleDate roleDate1)
    {
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
        if (GameManger.Instance.RoleDate.id!=roleDate1.id)
        {
=======
        
        isclick = true;
        // if (GameManager.Instance.RoleDate.id!=roleDate1.id)
        // {
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
            if (_roleshow.GetComponent<CanvasGroup>().alpha==0)
            {
                _roleshow.GetComponent<CanvasGroup>().alpha = 1;
            }

            
            RenewUI(roleDate);
            setImage(roleDate1);
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
            GameManger.Instance.RoleDate = roleDate1;
            roleInfotPanalshow();
            
           
            Instense = this;
            Debug.Log(Instense.transform.position.x);
            Debug.Log(Instense.transform.position.y);
            Debug.Log(Instense);
            
            _backring.transform.position = new Vector3(Instense.transform.position.x, Instense.transform.position.y, 0);
            
        }
=======
            GameManager.Instance.RoleDate = roleDate1;
            roleInfotPanalshow();


            switch (roleDate1.record)
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
                default:  
                    
                    break;
            }
            
           
            Instense = this;
            
            _backring.transform.localPosition = new Vector3(Instense.transform.localPosition.x, Instense.transform.localPosition.y, 0);
            
        // }
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
          
        

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
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
=======
            
            _nextButton.GetComponent<CanvasGroup>().alpha=0;
            _nextButton.GetComponent<CanvasGroup>().interactable=false;
            _nextButton.GetComponent<CanvasGroup>().blocksRaycasts=false;
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
        }
        else
        {
            
            _Animator.Play(jumpin, 0, 0f);
            _Roimages.color = Color.white;
            _showlock.GetComponent<CanvasGroup>().alpha = 0;
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
=======
            
            _nextButton.GetComponent<CanvasGroup>().alpha=1;
            _nextButton.GetComponent<CanvasGroup>().interactable=true;
            _nextButton.GetComponent<CanvasGroup>().blocksRaycasts=true;
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
        }
      
            
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
        
        //背景颜色
        this._backgroundimage.color = new Color(207/255f, 207/255f, 207/255f);
=======
        // isclick = false;
        //背景颜色
        this._backgroundimage.color = Color.white;
        
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
        

        
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
<<<<<<< Updated upstream:ZooheroProject/Assets/Resources/script/Roleset.cs
        this._backgroundimage.color = new Color(34/255f, 34/255f, 34/255f);   
=======
        switch (this.roleDate.record)
        {
            case -1:  
                this._backgroundimage.color = GameManager.Instance.color_1;
                break;  
            case 0:
                this._backgroundimage.color = GameManager.Instance.color0;
                break;
            case 1:
                this._backgroundimage.color = GameManager.Instance.color1;
                break;
            case 2:
                this._backgroundimage.color = GameManager.Instance.color2;
                break;
            case 3:
                this._backgroundimage.color =  GameManager.Instance.color3;
                break;
            case 4:
                this._backgroundimage.color = GameManager.Instance.color4;
                break;
            case 5:
                this._backgroundimage.color = GameManager.Instance.color5;
                break;
            default:  
                    
                break;
        }  
>>>>>>> Stashed changes:ZooheroProject/Assets/Scripts/UI/Roleset.cs
    }
}
