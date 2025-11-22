using System;
using model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class familyset : MonoBehaviour
{
    public familyset Instance;
    public FamilyDate FamilyDate;
    public Image _backgroundimage; //背景
    public Image _avater; // 头像
    public Button _button; //按钮
    public TextMeshProUGUI _familyname;
    public TextMeshProUGUI _familyde;

    public bool isSelect=false;

    public Image backColor;
    public Color Selectcolor = new Color(0.204f, 0.204f, 0.204f);
    public Color unSelectcolor = new Color(0.8f, 0.8f, 0.8f);
    
    private void Awake()
    {
        Instance = this;
        // _backgroundimage = GetComponent<Image>();
        _avater = transform.Find("familyavback/familyavhead").GetComponent<Image>();
        _button = GetComponent<Button>();
        _familyname = transform.Find("familyname").GetComponent<TextMeshProUGUI>();
        _familyde = transform.Find("familyDescription").GetComponent<TextMeshProUGUI>();
        backColor = transform.GetComponent<Image>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        _button.onClick.AddListener((() =>
        {
            
            selectFamily();
        }));
    }

    private void selectFamily()
    {

        if (isSelect == false && FamilyDate.unlock == 1) 
        {
            if (GameManager.Instance.CurrentFamilyDates.Count<=3)
            {
                // Debug.Log(GameManager.Instance.FamilyDates.Count);
                isSelect = true;
                GameManager.Instance.CurrentFamilyDates.Add(Instance.FamilyDate);
            }
            else
            {
                // Debug.Log("noVAR");
            }
        }
        else
        {
            GameManager.Instance.CurrentFamilyDates.Remove(Instance.FamilyDate);
            isSelect = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        
        
        if (isSelect)
        {
            backColor.color = Selectcolor;
        }
        else
        {
            backColor.color = unSelectcolor;
        }
    }

    public void setDate(FamilyDate familyDate)
    {
        this.FamilyDate = familyDate;
        
        if (this.FamilyDate.unlock == 0)
        {
            _avater.sprite = UnityEngine.Resources.Load<Sprite>("Image/UI/锁");
            // _button.enabled = false;
            _familyname.text = "未解锁";
            _familyde.text= "未解锁";
        }
        else
        {
            _avater.sprite = UnityEngine.Resources.Load<Sprite>(FamilyDate.avatar);
            // Debug.Log(FamilyDate.name);
            _familyname.text = FamilyDate.name;
            _familyde.text= FamilyDate.describe;
        }
        
        
    }
    

}
