using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckThereOne : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CheckThereOne Instance;
    public WeaponData WeaponData;
    public GameObject _WaepomList;
    public GameManager GameManager;
    public List<WeaponData> CurrentWeapons = new List<WeaponData>();
    public List<WeaponData> NotEquippedcurrentWeapons = new List<WeaponData>();
    public GameObject _ThreeInOneButton;
    public GameObject _WeaponCardInfo;
    public GameObject _ViewWeaponName;
    public Button _button;
    public int NumberOfCardsWithTheSameName;

    private void Awake()
    {
        Instance = this;
        // WeaponData = transform.GetComponent<Weaponset>().WeaponData;
        // Debug.Log(WeaponData.name);
        _WaepomList = GameObject.Find("WaepomList");
        CurrentWeapons = GameManager.Instance.currentWeapons; //获取两个wepaonlist
        NotEquippedcurrentWeapons = GameManager.Instance.NotEquippedcurrentWeapons;
        _button = transform.GetComponent<Button>();
        _ThreeInOneButton = GameObject.Find("ThereInOneButton");
        _ViewWeaponName = GameObject.Find("ViewWeaponName");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
            if (transform.parent.name=="WaepomList")
            {
                _button.onClick.AddListener((() =>
                {
                    showThreeInOneInfo(WeaponData);
                    Debug.Log(this.NumberOfCardsWithTheSameName);
                    _ThreeInOneButton.GetComponent<ThreeInOne>().NumberOfCardsWithTheSameName =
                        this.NumberOfCardsWithTheSameName;
                    _ThreeInOneButton.GetComponent<ThreeInOne>().CardSource = "WaepomList";
                }));
            }else if (transform.parent.name == "PropsList")
            {
                _button.onClick.AddListener((() =>
                {
                    this.WeaponData = transform.GetComponent<WeaponDataSet>().WeaponData;
                    showThreeInOneInfo(WeaponData);
                    // Debug.Log(this.NumberOfCardsWithTheSameName);
                    _ThreeInOneButton.GetComponent<ThreeInOne>().NumberOfCardsWithTheSameName =
                        this.NumberOfCardsWithTheSameName;
                    _ThreeInOneButton.GetComponent<ThreeInOne>().CardSource = "PropsList";
                }));
            }
            
            _button.onClick.AddListener((() =>
            {
                _ViewWeaponName.GetComponent<TMP_Text>().text = WeaponData.name;
                _ThreeInOneButton.GetComponent<ThreeInOne>().synthesizedWeaponCard = this.WeaponData;
            }));
            
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void showThreeInOneInfo(WeaponData weaponData)
    {
        NumberOfCardsWithTheSameName = CountMatchingWeapons(weaponData, CurrentWeapons, NotEquippedcurrentWeapons);
        // GameManager.Instance.GameObjectShow(_WeaponCardInfo.GetComponent<CanvasGroup>());
        
        // Debug.Log(num);
    }
    
    public static int CountMatchingWeapons(WeaponData targetWeapon, List<WeaponData> list1, List<WeaponData> list2)
    {
        // 校验目标武器是否为null
        if (targetWeapon == null)
        {
            Debug.LogError("传入的目标武器为null，无法统计！");
            return 0;
        }

        // 处理列表为null的情况（视为空列表）
        list1 ??= new List<WeaponData>();
        list2 ??= new List<WeaponData>();

        int totalCount = 0;

        // 统计第一个列表中符合条件的数量
        totalCount += CountInSingleList(targetWeapon, list1);
        // 统计第二个列表中符合条件的数量
        totalCount += CountInSingleList(targetWeapon, list2);

        return totalCount;
    }

    /// <summary>
    /// 辅助方法：统计单个列表中符合条件的武器数量
    /// </summary>
    private static int CountInSingleList(WeaponData target, List<WeaponData> list)
    {
        int count = 0;
        foreach (var weapon in list)
        {
            // 跳过列表中的null元素，避免空引用
            if (weapon == null) continue;

            // 同时满足name、affection、rank匹配
            bool isNameMatch = string.Equals(target.name, weapon.name, System.StringComparison.Ordinal);
            bool isAffectionMatch = target.affection == weapon.affection;
            bool isRankMatch = target.rank == weapon.rank;
            
            if (isNameMatch && isAffectionMatch && isRankMatch)
            {

                count++;
            }
        }
        return count;
    }
}

