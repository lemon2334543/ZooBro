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
    public GameObject _ViewWeaponImageBack;
    public GameObject _ViewWeaponImageBackAvter;
    public GameObject _price;
    public GameObject _ViewWeaponInfo;
    public GameObject _Details;
    public GameObject _WeaponCardDetails;
    public Transform _ViewWeaponInfoTypes;
    public GameObject _WeaponType;
    public List<string> WeaponTypes = new List<string>();
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
        
        _ViewWeaponImageBack = GameObject.Find("ViewWeaponImageBack");
        _ViewWeaponInfoTypes = GameObject.Find("ViewWeaponInfoTypes").transform;
        _ViewWeaponImageBackAvter = GameObject.Find("ViewWeaponImage");
        _price = GameObject.Find("Pricetext11");
        _ViewWeaponInfo = GameObject.Find("ViewWeaponInfo");
        _Details = GameObject.Find("Details");
        _WeaponCardDetails = GameObject.Find("WeaponCardDetails");
        
        _WeaponType = UnityEngine.Resources.Load<GameObject>("Prefabs/UIprefabs/Label");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
            if (transform.parent.name=="WaepomList")
            {
                _button.onClick.AddListener((() =>
                {
                    showThreeInOneInfo(WeaponData);
                    // Debug.Log(this.NumberOfCardsWithTheSameName);
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
            }else if (transform.parent.name == "SelectAttr")
            {
                _button.onClick.AddListener((() =>
                {
                    this.WeaponData = transform.GetComponent<WeaponDataSet>().WeaponData;
                    _ThreeInOneButton.GetComponent<ThreeInOne>().NumberOfCardsWithTheSameName = 0;
                    showThreeInOneInfo(WeaponData);
          
                }));
            }
            
            _button.onClick.AddListener((() =>
            {
                setViewData();
                
            }));
            
        
        
    }


    public void setViewData()
    {
        _ViewWeaponName.GetComponent<TMP_Text>().text = WeaponData.name;
        _ViewWeaponImageBack.GetComponent<Image>().color = GameManager.Instance.setColor(WeaponData.rank);
        _ViewWeaponImageBackAvter.GetComponent<Image>().sprite =
            UnityEngine.Resources.Load<Sprite>(WeaponData.avatar);
        _ViewWeaponImageBackAvter.GetComponent<Image>().color = Color.white;
        _price.GetComponent<TMP_Text>().text = WeaponData.price.ToString();
        _ThreeInOneButton.GetComponent<ThreeInOne>().synthesizedWeaponCard = this.WeaponData;
        _ViewWeaponInfo.GetComponent<TextMeshProUGUI>().text = WeaponData.describe;
        _Details.GetComponent<showDetails>().WeaponData = WeaponData;
        if (_WeaponCardDetails.GetComponent<CanvasGroup>().alpha==1)
        {
            _Details.GetComponent<showDetails>().button.onClick.Invoke();
        }

        setWeaponTypes();
    }
    
    
    
    private void setWeaponTypes()
    {
        GameManager.Instance.DestroyAllChildren(_ViewWeaponInfoTypes.gameObject);
        WeaponTypes.Clear();
        
        if (WeaponData.familyname=="Animal")
        {
            WeaponTypes.Add("动物");
        }else if (WeaponData.familyname=="Demon")
        {
            WeaponTypes.Add("恶魔");
        }else if (WeaponData.familyname=="Machine")
        {
            WeaponTypes.Add("机械");
        }else if (WeaponData.familyname=="Neutral")
        {
            WeaponTypes.Add("中立");
        }
        
        
        WeaponTypes.AddRange(WeaponData.Type);
        
        // 如果类型列表为空，则直接返回，无需创建任何标签
        if (WeaponTypes == null || WeaponTypes.Count == 0)
        {
            return;
        }
        
        // 3. 遍历类型列表，创建标签
        for (int i = 0; i < WeaponTypes.Count; i++)
        {
            // 实例化一个新的标签对象
            GameObject newLabelObj = Instantiate(_WeaponType, _ViewWeaponInfoTypes);
            newLabelObj.transform.Find("LabelText").GetComponent<TextMeshProUGUI>().text = WeaponTypes[i];

        }
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
        if (weaponData.isLong==11||weaponData.isLong==12)
        {
            NumberOfCardsWithTheSameName = -1;
        }
        else
        {
            NumberOfCardsWithTheSameName = CountMatchingWeapons(weaponData, CurrentWeapons, NotEquippedcurrentWeapons);
        }
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

