using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RomButton : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public RomButton Instense;
    private Image _backgroundimage;
    private GameObject _RolistRanrom;
    private Button _button;
    public GameObject _backring;
    public List<Roleset> unLockRolesets = new List<Roleset>();
    
    
    public GameObject _Roimage;
    public Image _Roimages;
    private Animator _infoAnimator;
    public string roleinfoin = "roleinfoin";
    
    public bool isclick = false;
    
    public GameObject _record;
    public TextMeshProUGUI recordtext;

    private void Awake()
    {
        Instense = this;
        _RolistRanrom = GameObject.Find("Ro-list-Ranrom");
        _backgroundimage = _RolistRanrom.GetComponent<Image>();
        _button = this.GetComponent<Button>();
        _backring = GameObject.Find("backring");//选中绿色背景
        
        _Roimage = GameObject.Find("Ro-image");
        _Roimages = _Roimage.GetComponent<Image>();
        _infoAnimator = GameObject.Find("RO-Infooanel").GetComponent<Animator>();
        _record = GameObject.Find("HighestRecord");
        recordtext = _record.GetComponent<TextMeshProUGUI>();
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.onClick.AddListener((() =>
        {
            if (isclick==false)
            {
                isclick = true;
                //遍历所有rolepanel脚本下的_rolelist里的Roleset组件
                foreach (Roleset roleset in rolepanel.Instance._rolelist.GetComponentsInChildren<Roleset>())
                {
                    
                    
                    if (roleset.roleDate.unlock==1)
                    {
                        unLockRolesets.Add(roleset);
                    }
                }

                Roleset rolesetOne = GameManager.Instance.RandomOne(unLockRolesets) as Roleset;
                rolesetOne.ButtonClick(rolesetOne.roleDate);
                foreach (Roleset roleset in rolepanel.Instance._rolelist.GetComponentsInChildren<Roleset>())
                {
                    roleset.isclick = false;
                }



                _backring.transform.localPosition = new Vector3(Instense.transform.localPosition.x, Instense.transform.localPosition.y, 0);
                _Roimages.sprite = UnityEngine.Resources.Load<Sprite>("Image/UI/问号");
                // _infoAnimator.Play(roleinfoin, 0, 0f);
                rolepanel.Instance._rolename.text = "随机";
                rolepanel.Instance._RoleDes.text = "随机选择一个角色";
                rolepanel.Instance._abater.sprite = UnityEngine.Resources.Load<Sprite>("Image/UI/问号");
                recordtext.text = "";
            }
            
        }));
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // this.isclick = false;
        this._backgroundimage.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this._backgroundimage.color = new Color(0.204f, 0.204f, 0.204f);
    }
}
