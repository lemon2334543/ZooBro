using System;
using System.Collections.Generic;
using model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponList : MonoBehaviour
{
    public static WeaponList Instance;

    public List<WeaponData> WeaponDatas = new List<WeaponData>();//获取json
    public GameObject CurrentWeaponList;
    
    public GameObject Weapon_Prefab;//预制件

    private void Awake()
    {
        Instance = this;
        Weapon_Prefab = UnityEngine.Resources.Load<GameObject>("Prefabs/propback");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // SetCurrentWeapons();
    }

    public void SetCurrentWeapons()
    {

        for (int i = CurrentWeaponList.transform.childCount - 1; i >= 0; i--)
        {
            // 获取子对象
            Transform child = CurrentWeaponList.transform.GetChild(i);
            // 销毁子对象（若需要彻底删除，用 Destroy；若需要暂时隐藏，用 SetActive(false)）
            Destroy(child.gameObject);
        }
        
        foreach (WeaponData weaponData in GameManager.Instance.currentWeapons)
        {
            Weaponset r = GameObject.Instantiate(Weapon_Prefab,CurrentWeaponList.transform).GetComponent<Weaponset>();
            r.setDate(weaponData);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
