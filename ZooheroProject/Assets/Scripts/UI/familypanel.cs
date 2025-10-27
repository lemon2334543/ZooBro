using System;
using System.Collections.Generic;
using model;
using Newtonsoft.Json;
using Resources.script.model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class familypanel : MonoBehaviour
{
    public static familypanel Instance;
    public List<FamilyDate> familyDates = new List<FamilyDate>();//获取json
    public TextAsset textAsset;//json文本z

    public Transform _familylist;//UI列表
    public GameObject family_Prefab;//预制件
    
    public Image _abater;//头像
    public TextMeshProUGUI _familyDes;//角色表述


    private void Awake()
    {
        Instance = this;

        _familylist = GameObject.Find("familypannel").transform;
        family_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/family");
        
        textAsset = UnityEngine.Resources.Load<TextAsset>("Data/Family");
        familyDates = JsonConvert.DeserializeObject<List<FamilyDate>>(textAsset.text);
        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (FamilyDate familyDate in familyDates)
        {
            if (familyDate.id==1)
            {
                continue;
            }
            // Debug.Log(familyDate.name+"111111111111");
            //循环生成预制体
            familyset r = GameObject.Instantiate(family_Prefab,_familylist.transform).GetComponent<familyset>();
            r.setDate(familyDate);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
