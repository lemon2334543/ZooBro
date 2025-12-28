using System;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DevelopmentSet : MonoBehaviour
{
    public GameObject _DevelopmentProjectImage;
    public GameObject _DevelopmentProjectImage1;
    public GameObject _DevelopmentPrice2;
    public GameObject _DevelopmentPrice1;
    public GameObject _NotInDemoPrice;
    public GameObject _DevelopmentProjectLevel;
    public GameObject _LevelUpImageBack;
    public Button Button;

    public OutsiderDevelopmentData OutsiderDevelopmentData;
    private void Awake()
    {
        _DevelopmentProjectImage = transform.Find("DevelopmentProjectImage").gameObject;
        _DevelopmentProjectImage1 = _DevelopmentProjectImage.transform.Find("DevelopmentProjectImage1").gameObject;
        _DevelopmentPrice2 = transform.Find("DevelopmentPrice").transform.Find("DevelopmentPrice2").gameObject;
        _DevelopmentPrice1 = transform.Find("DevelopmentPrice").transform.Find("DevelopmentPrice1").gameObject;
        _NotInDemoPrice = transform.Find("DevelopmentPrice").transform.Find("NotInDemoPrice").gameObject;
        _DevelopmentProjectLevel = transform.Find("DevelopmentProjectLevel").gameObject;
        _LevelUpImageBack = GameObject.Find("LevelUpImageBack").gameObject;
        Button = transform.GetComponent<Button>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button.onClick.AddListener(ButtonClick);
    }

    private void ButtonClick()
    {
        // Debug.Log("VAR");
        _LevelUpImageBack.GetComponent<LevelUPClick>().Data = this.OutsiderDevelopmentData;
        OutsiderLevelUpPanel.Instance.setData(OutsiderDevelopmentData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDate(OutsiderDevelopmentData outsiderDevelopmentData)
    {
        this.OutsiderDevelopmentData = outsiderDevelopmentData;
        
        if (outsiderDevelopmentData.enName!="NotInDemo")
        {
            int CurrentLevel = outsiderDevelopmentData.currentLevel; 
            Color targetColor;
            if (ColorUtility.TryParseHtmlString(outsiderDevelopmentData.color, out targetColor))
            {
                _DevelopmentProjectImage.GetComponent<Image>().color = targetColor;
            }
            
            _DevelopmentProjectImage1.GetComponent<Image>().sprite =
                UnityEngine.Resources.Load<Sprite>(outsiderDevelopmentData.image);

            _DevelopmentPrice2.GetComponent<TextMeshProUGUI>().text = (outsiderDevelopmentData.price[CurrentLevel]).ToString();

            _DevelopmentProjectLevel.GetComponent<Image>().sprite =
                UnityEngine.Resources.Load<Sprite>(GameManager.Instance.setNum(CurrentLevel));
        }
        else if (outsiderDevelopmentData.enName=="NotInDemo")
        {
            _DevelopmentProjectImage.GetComponent<Image>().color = Color.gray;
            _DevelopmentProjectImage1.GetComponent<Image>().sprite =
                UnityEngine.Resources.Load<Sprite>("Image/UI/锁");
            // _DevelopmentPrice2.GetComponent<TextMeshProUGUI>().text = "not in Demo";
            GameManager.Instance.GameObjectHide(_DevelopmentPrice1.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectHide(_DevelopmentPrice2.GetComponent<CanvasGroup>());
            GameManager.Instance.GameObjectShow(_NotInDemoPrice.GetComponent<CanvasGroup>());
        }
        
       
    }
}
