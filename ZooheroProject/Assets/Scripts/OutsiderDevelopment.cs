using System;
using Model;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OutsiderDevelopment : MonoBehaviour
{
    public OutsiderDevelopment Instance;
    public GameObject DevelopmentProject_Prefab;
    public GameObject _Panel2;
    public GameObject _OutsiderCurrencyCultivation1;
    
    private void Awake()
    {
        Instance = this;
        DevelopmentProject_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/DevelopmentProject");
        _Panel2 = GameObject.Find("Panel2");
        _OutsiderCurrencyCultivation1 = GameObject.Find("OutsiderCurrencyCultivation1");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetOutsiderDevelopment();
        
    }

    public void SetOutsiderDevelopment()
    {
        // Debug.Log("VAR");
        GameManager.Instance.DestroyAllChildren(_Panel2);
        
        foreach (OutsiderDevelopmentData outsiderDevelopmentData in GameManager.Instance.RealOutsiderDevelopmentDatas)
        {
            
            //循环生成预制体
            DevelopmentSet development = GameObject.Instantiate(DevelopmentProject_Prefab,_Panel2.transform).GetComponent<DevelopmentSet>();
            development.setDate(outsiderDevelopmentData);
            
        }

        setOutsiderCurrencyCultivation();
    }

    private void setOutsiderCurrencyCultivation()
    {
        _OutsiderCurrencyCultivation1.GetComponent<TextMeshProUGUI>().text =
            GameManager.Instance.OutsiderCurrencyCultivation.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
