using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Resources.script.model;
// using script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class rolepanel : MonoBehaviour
{
    public static rolepanel Instance;
    public List<RoleDate> RoleDates = new List<RoleDate>();//获取json
    public TextAsset textAsset;//json文本z

    public Transform _rolelist;//UI列表
    public GameObject role_Prefab;//预制件
    
    public TextMeshProUGUI _rolename;//TextMeshProUGUI组件
    public Image _abater;//头像
    public TextMeshProUGUI _RoleDes;//角色表述

    public GameObject _fistRole;
    private void Awake()
    {
        Instance = this;

        _rolelist = GameObject.Find("rolelist").transform;
        role_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Ro-Profile-role");
        
        textAsset = UnityEngine.Resources.Load<TextAsset>("Data/role");
        RoleDates = JsonConvert.DeserializeObject<List<RoleDate>>(textAsset.text);

        _rolename = GameObject.Find("Ro-name").GetComponent<TextMeshProUGUI>();
        _abater = GameObject.Find("Ro-info-image").GetComponent<Image>();
        _RoleDes = GameObject.Find("Ro-Description").GetComponent<TextMeshProUGUI>();

        _fistRole = Instance.transform.GetChild(0).gameObject;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        foreach (RoleDate roleDate in RoleDates)
        {
            //循环生成预制体
            Roleset r = GameObject.Instantiate(role_Prefab,_rolelist.transform).GetComponent<Roleset>();
            r.setDate(roleDate);
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
